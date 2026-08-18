# Feature Specification: Shipment Tracking Events

**Feature Branch**: `[007-shipment-tracking-events]`

**Created**: 2026-08-18

**Status**: Draft

**Input**: User description: "Extend the existing ShipmentEvent entity and create the new
  DeliveryAttempt entity for the Shipment Tracking module
  of a parcel delivery company.

  ShipmentEvent already exists in the database with the following
  columns: Id (int, autoincrement, PK), ShipmentId (FK), EventType,
  StatusSnapshot and OccurredAt. The table has a migration already
  applied. No columns must be dropped or renamed.

  The following columns must be ADDED to ShipmentEvent via a new
  additive migration:
  - EmployeeId: nullable FK to Employee. Represents the staff member
    who performed or registered the event. Required only when
    EventType is OutForDelivery (must be a Driver).
  - LocationLabel: nullable string. A human-readable location
    description such as "Hub CDMX Norte" or "Colonia Doctores".
  - Notes: nullable string. Free-text observations about the event.
  - CreatedAt: DateTime, not nullable. The system timestamp when
    the record was inserted, distinct from OccurredAt which is
    the real-world time of the event.

  A new table DeliveryAttempt must be created with:
  - Id: int, autoincrement, PK.
  - ShipmentEventId: int, FK to ShipmentEvent, unique (one-to-one).
    A DeliveryAttempt record only exists for events of type
    DeliveryAttempted.
  - AttemptNumber: int, not nullable. Calculated automatically by
    the system as the count of previous DeliveryAttempt records
    for the same Shipment plus one. Never provided by the caller.
  - FailureReason: enum (NoOneHome, WrongAddress, Refused,
    AccessDenied, Other), not nullable.
  - NextAttemptAt: DateTime, nullable. The scheduled datetime for
    the next delivery attempt. Must be later than OccurredAt of
    the associated ShipmentEvent when provided.

  The state transition rules already defined for ShipmentEvent
  remain unchanged. DeliveryAttempt is only created when EventType
  is DeliveryAttempted and the current Shipment status is
  OutForDelivery.

  The public tracking endpoint must not expose EmployeeId or any
  employee personal data. LocationLabel and Notes are safe to expose."

## Clarifications

### Session 2026-08-18

- Q: After a delivery attempt fails, does the shipment need to be marked "out for delivery" again before another attempt can be logged, or can several attempts be logged one after another while it stays in the same "out for delivery" period? → A: A shipment can have several delivery-attempt events logged consecutively while it remains "out for delivery" from a single out-for-delivery event — a new one isn't required before each attempt, only that the shipment's status is currently "out for delivery."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Mark a shipment out for delivery (Priority: P1)

A driver (or an operator on the driver's behalf) records that a shipment has left for its final delivery run, identifying the driver responsible and, optionally, the location the shipment departed from.

**Why this priority**: A shipment must be marked out for delivery before any delivery attempt can be logged against it — this is the foundation every other capability in this feature depends on.

**Independent Test**: Can be fully tested by recording an out-for-delivery event for an in-transit shipment with a valid, active Driver identified, and confirming the shipment's status reflects that it is now out for delivery and the event is stored with that driver, an optional location label, and the system/real-world timestamps.

**Acceptance Scenarios**:

1. **Given** a shipment that is in transit, **When** an out-for-delivery event is recorded identifying an active employee who holds the Driver role, **Then** the event is stored with that employee, the shipment's status becomes "out for delivery," and both the real-world event time and the system's record-creation time are captured.
2. **Given** an out-for-delivery event is being recorded, **When** no employee is identified, or the identified employee does not exist, is not active, or does not hold the Driver role, **Then** the event is rejected with a clear explanation.
3. **Given** an out-for-delivery event is being recorded, **When** a location label and/or notes are supplied, **Then** they are stored with the event exactly as given.

---

### User Story 2 - Log a failed delivery attempt (Priority: P2)

A driver logs the outcome of an unsuccessful delivery attempt for a shipment that is currently out for delivery, recording why it failed and, if known, when the next attempt is planned.

**Why this priority**: This is the feature's central new capability — the reason `DeliveryAttempt` exists — but it structurally depends on User Story 1 (a shipment must already be out for delivery).

**Independent Test**: Can be fully tested by logging a delivery-attempt event with a failure reason against a shipment that is out for delivery, and confirming a matching attempt record is created with an automatically-computed sequence number; logging a second failed attempt for the same shipment confirms the sequence number increments correctly.

**Acceptance Scenarios**:

1. **Given** a shipment that is out for delivery, **When** a delivery-attempt event is recorded with a failure reason, **Then** the event is stored and exactly one matching attempt record is created, numbered one higher than any previous attempt recorded for that shipment (the first attempt for a shipment is numbered 1).
2. **Given** a shipment that is out for delivery and already has one failed attempt recorded, **When** a second delivery-attempt event is recorded — without any new out-for-delivery event in between, since the shipment's status is still "out for delivery" from the first one — **Then** the new attempt record is numbered 2, independent of any other shipment's attempt count.
3. **Given** a shipment that is **not** currently out for delivery (e.g., still in transit, or already delivered/cancelled), **When** a delivery-attempt event is recorded, **Then** it is rejected and no attempt record is created.
4. **Given** a delivery-attempt event is being recorded, **When** no failure reason is supplied, or an unrecognized one is given, **Then** it is rejected.
5. **Given** a delivery-attempt event is being recorded, **When** a next-attempt datetime is supplied that is not later than the event's real-world occurrence time, **Then** it is rejected.
6. **Given** a delivery-attempt event is being recorded, **When** no next-attempt datetime is supplied, **Then** the attempt record is still created, simply without one.

---

### User Story 3 - View a shipment's public tracking timeline (Priority: P3)

A customer (or anyone with the tracking number) looks up a shipment's tracking information and sees the chronological sequence of what happened to it — including out-for-delivery and delivery-attempt events — without seeing which staff member was involved.

**Why this priority**: Read-only visibility is valuable on its own once events exist, but it depends on User Stories 1 and 2 having produced events worth showing; it is also the specific privacy requirement called out in this feature.

**Independent Test**: Can be fully tested by looking up a shipment that has out-for-delivery and delivery-attempt events recorded against it, and confirming the response includes each event's type, status snapshot, location label, notes, and timestamps, while never including the identity of the employee who registered any event.

**Acceptance Scenarios**:

1. **Given** a shipment with recorded events, **When** its public tracking information is retrieved, **Then** the response includes, for each event, its type, the shipment status snapshot at that point, its location label and notes (when present), and when it occurred — but no employee identifier or any other employee personal data.
2. **Given** a shipment with no events recorded yet, **When** its public tracking information is retrieved, **Then** the shipment's own details are returned with an empty event timeline, not an error.
3. **Given** a shipment with a logged delivery attempt, **When** its public tracking information is retrieved, **Then** the failure reason and next-attempt datetime (when set) for that attempt are included, since neither reveals employee identity.

---

### Edge Cases

- What happens when an out-for-delivery event is recorded for a shipment that is already out for delivery? Allowed — a shipment can legitimately go out for delivery more than once across its lifetime (e.g., after a failed attempt is rescheduled), so this is not rejected on that basis alone.
- What happens when an out-for-delivery event is recorded for a shipment that has already been delivered or cancelled? Rejected — those are terminal states for a shipment's delivery lifecycle.
- What happens when a delivery-attempt event's employee reference is omitted? Permitted — only out-for-delivery events require an identified Driver; a delivery-attempt event does not require one.
- What happens when the same delivery-attempt event is somehow submitted twice? Each accepted delivery-attempt event always produces its own new attempt record with the next sequence number — there is no concept of "resubmitting" a prior attempt.
- Does this feature ever delete or edit a previously recorded event or attempt? No — both are permanent, append-only history, consistent with every other record-keeping entity in this system.

## Requirements *(mandatory)*

### Functional Requirements

#### Extending the shipment event record

- **FR-001**: System MUST continue to capture every previously recorded piece of information about a shipment event (its shipment, type, status snapshot, and real-world occurrence time) unchanged — no existing information is removed or renamed.
- **FR-002**: System MUST additionally allow capturing, for any shipment event: the staff member who performed or registered it (when applicable), a human-readable location label, free-text notes, and the system timestamp at which the record was created (distinct from its real-world occurrence time).
- **FR-003**: System MUST require an identified staff member for an out-for-delivery event, and MUST reject the event if that staff member does not exist, is not active, or does not hold the Driver role.
- **FR-004**: System MUST treat the identified staff member as optional for every other kind of shipment event.

#### Delivery attempts

- **FR-005**: System MUST allow logging a delivery-attempt event, with a failure reason, only while the shipment's current status is "out for delivery," and MUST reject it otherwise. This is a status check, not an event-adjacency check — no new out-for-delivery event is required between consecutive delivery-attempt events as long as the shipment's status remains "out for delivery."
- **FR-006**: When a delivery-attempt event is accepted, System MUST automatically create exactly one matching attempt record — never more than one per event, and never supplied or numbered by the caller.
- **FR-007**: System MUST compute each attempt record's sequence number automatically as one more than the number of attempt records already recorded for that same shipment, starting at 1.
- **FR-008**: System MUST require a recognized failure reason for every attempt record, and MAY optionally accept a next-attempt datetime.
- **FR-009**: System MUST reject a supplied next-attempt datetime that is not strictly later than its event's real-world occurrence time.
- **FR-010**: System MUST NOT provide any capability to edit or delete a previously recorded shipment event or attempt record.

#### Shipment lifecycle

- **FR-011**: System MUST recognize "out for delivery" as a distinct phase in a shipment's delivery lifecycle, reachable from an in-transit shipment and preceding final delivery.

#### Public tracking visibility

- **FR-012**: System MUST allow retrieving a shipment's chronological event timeline as part of its publicly accessible tracking information, including out-for-delivery and delivery-attempt events.
- **FR-013**: System MUST exclude the identity of the staff member associated with any event — and any other employee personal data — from the publicly accessible tracking information.
- **FR-014**: System MUST include each event's location label and notes (when present) in the publicly accessible tracking information, since neither reveals employee identity.
- **FR-015**: System MUST include each logged delivery attempt's failure reason and next-attempt datetime (when present) in the publicly accessible tracking information.

### Key Entities

- **Shipment Event** *(existing entity, extended)*: A record of something that happened to a shipment over its lifecycle. Already captures which shipment it belongs to, its type, a snapshot of the shipment's status, and when it really happened. Extended by this feature to optionally capture the staff member involved, a location label, free-text notes, and when the record was created in the system.
- **Delivery Attempt** *(new entity)*: A record of one unsuccessful delivery try for a shipment, created automatically alongside a delivery-attempt event — never on its own. Captures which attempt number it is for its shipment (computed, never supplied), why it failed, and optionally when the next attempt is planned.
- **Employee** *(existing entity, referenced)*: A shipment event may optionally identify the staff member who performed or registered it; an out-for-delivery event requires one, and that employee must be active and hold the Driver role.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A driver can mark a shipment out for delivery and log the outcome of every attempt against it — including scheduling a next attempt — without needing any system outside this one.
- **SC-002**: 100% of delivery attempts are numbered correctly in sequence for their shipment, with no manual numbering ever required or accepted from a caller.
- **SC-003**: No delivery attempt can ever exist for a shipment that was never marked out for delivery.
- **SC-004**: 100% of publicly viewable shipment tracking timelines omit employee-identifying information while still showing what happened, where, and when.
- **SC-005**: Every shipment event recorded before this feature shipped remains fully intact and retrievable afterward.

## Assumptions

- **Only two new event types are introduced**: this feature adds exactly the two shipment-event types its own rules require — "out for delivery" and "delivery attempted" — no other event types are introduced speculatively.
- **A delivery-attempt event always represents a failed attempt**: since a failure reason is always required for its attempt record, this feature does not model a *successful* delivery through the delivery-attempt event type — recording that a shipment was ultimately delivered continues to work the way it already does today (out of scope for this feature).
- **A shipment can go out for delivery more than once**: a shipment may be marked out for delivery again for a subsequent try (e.g., on a later day); this feature does not require a shipment to leave the "out for delivery" phase before another out-for-delivery event is recorded, only that it hasn't already reached delivered/cancelled. See Clarifications for the related, now-confirmed rule that multiple delivery attempts can also be logged within a single out-for-delivery period without a new out-for-delivery event between them.
- **No cap on the number of delivery attempts**: this feature does not introduce a maximum-attempts policy; every rejected attempt can be followed by another.
- **No authorization tiers**: consistent with the rest of this system, no role-based restriction is assumed on who may record events beyond the explicit Driver requirement stated for out-for-delivery events; any authenticated user may otherwise perform these operations.
- **Public tracking information is reached through the shipment's existing tracking lookup**: this feature extends the shipment's existing publicly-reachable tracking-by-number information to include the event timeline, rather than introducing a separate lookup mechanism.
- **Location label is free text**: not validated against any fixed list of branches or zones, consistent with how free-text descriptive fields are handled elsewhere in this system.
