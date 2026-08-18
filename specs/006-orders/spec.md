# Feature Specification: Orders Module

**Feature Branch**: `[006-orders]`

**Created**: 2026-08-17

**Status**: Draft

**Input**: User description: "Build the Orders module for a parcel delivery company.

  An Order represents a shipment request created by a registered Customer
  before any physical package is collected. It captures the sender's intent:
  who is receiving the package, where it is going, how it will be picked up,
  and the estimated dimensions and weight.

  An Order belongs to exactly one Customer. It references an optional origin
  Branch — required only when pickup_type is DropOff (the customer brings
  the package to a branch). When pickup_type is HomePickup, a pickup address
  and a scheduled pickup datetime are required instead.

  Orders have a unique, human-readable order number generated automatically
  at creation using the pattern ORD-YYYYMMDD-XXXX where XXXX is a zero-padded
  sequential number per day.

  An Order moves through the following statuses:
  - Pending: just created, editable.
  - Confirmed: reviewed and accepted by an operator, no longer editable.
  - Converted: a Shipment has been generated from this Order. Terminal state.
  - Cancelled: cancelled before conversion. Terminal state.

  Only Pending orders can be edited or cancelled.
  Only Confirmed orders can be converted to a Shipment.

  Converting an Order to a Shipment is the central business operation
  of this module. It must: validate the order is Confirmed, generate a unique
  tracking number for the new Shipment (format: format TRK-YYYYMMDD-XXXX),
  create the Shipment entity, register the first ShipmentEvent of type
  OrderConverted, update the Order status to Converted,
  and commit everything atomically in a single transaction.

  Users can: create orders, confirm orders, cancel orders, update pending orders,
  list orders with filters, retrieve a single order, and convert a confirmed
  order to a shipment."

## Clarifications

### Session 2026-08-17

- Q: When an Order is converted to a Shipment, should the new Shipment record be extended with new fields to carry the Order's full detail (destination, dimensions, weight, pickup info), or should Shipment keep its current minimal shape and the full detail remain reachable only via a new `OrderId` reference back to the originating Order? → A: Shipment keeps its current minimal shape (tracking number, recipient, status, dates); a new required `OrderId` back-reference is added instead, and the Order — retained permanently — stays the system of record for destination/dimensions/weight/pickup detail.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a new order (Priority: P1)

A registered Customer (or an operator acting on their behalf) creates an Order describing a package to be sent: who will receive it, where it's going, how it will be picked up (dropped off at a branch or picked up at home), and its estimated size and weight.

**Why this priority**: No other capability in this module has anything to operate on until an Order exists. This is the foundation for every other capability, including the module's central operation (conversion to a Shipment).

**Independent Test**: Can be fully tested by submitting a new Order with `pickupType: DropOff` and a valid origin Branch, and a second Order with `pickupType: HomePickup` and a valid pickup address/datetime, and confirming both are created as `Pending` with a unique, correctly-formatted order number.

**Acceptance Scenarios**:

1. **Given** a registered, active Customer, **When** an Order is created with `pickupType: DropOff`, a valid active origin Branch, recipient details, destination, and package dimensions/weight, **Then** the Order is created with status `Pending` and a system-generated order number matching `ORD-YYYYMMDD-XXXX` for today's date.
2. **Given** a registered, active Customer, **When** an Order is created with `pickupType: HomePickup`, a pickup address, a future scheduled pickup datetime, recipient details, destination, and package dimensions/weight, **Then** the Order is created with status `Pending`.
3. **Given** two Orders created on the same calendar day, **When** both are created, **Then** their order numbers share the same date segment and have two different, sequential, zero-padded numeric suffixes.
4. **Given** an Order is being created with `pickupType: DropOff`, **When** no origin Branch is provided, or the provided Branch does not exist or is inactive, **Then** the creation is rejected and the specific validation problem is reported.
5. **Given** an Order is being created with `pickupType: HomePickup`, **When** the pickup address or the scheduled pickup datetime is omitted, or the scheduled pickup datetime is in the past, **Then** the creation is rejected.
6. **Given** an Order is being created with `pickupType: DropOff`, **When** a pickup address or scheduled pickup datetime is also supplied, **Then** the creation is rejected as inconsistent with the declared pickup type (and symmetrically, an origin Branch supplied on a `HomePickup` order is also rejected).
7. **Given** a Customer identifier that does not exist, or that refers to an inactive Customer, **When** an Order is created for it, **Then** the creation is rejected.
8. **Given** package weight or any dimension submitted as zero or negative, **When** the Order is created, **Then** the creation is rejected.

---

### User Story 2 - Find and review orders (Priority: P2)

An operator browses the order queue to find Orders by status, by Customer, or by date, and drills into a specific Order to review its full detail before deciding whether to confirm, cancel, or convert it.

**Why this priority**: Once Orders exist, an operator needs to locate and inspect them before any lifecycle action (confirm, cancel, convert) is meaningful.

**Independent Test**: Can be fully tested by listing Orders with and without filters, and retrieving a single Order by its identifier to confirm all captured details are returned.

**Acceptance Scenarios**:

1. **Given** Orders in multiple statuses exist, **When** an operator lists Orders with no filters applied, **Then** all Orders are returned, most recently created first.
2. **Given** Orders in multiple statuses exist, **When** an operator lists Orders filtered by status `Pending`, **Then** only `Pending` Orders are returned.
3. **Given** Orders from multiple Customers exist, **When** an operator lists Orders filtered by a specific Customer, **Then** only that Customer's Orders are returned.
4. **Given** an existing Order, **When** an operator retrieves it by its identifier, **Then** the response includes its order number, status, Customer, pickup details, recipient, destination, and package dimensions/weight.
5. **Given** an order identifier that does not exist, **When** an operator retrieves it, **Then** the system reports that the Order was not found.

---

### User Story 3 - Confirm a pending order (Priority: P3)

An operator reviews a `Pending` Order for correctness and confirms it, locking its details and marking it ready to be converted into an actual Shipment.

**Why this priority**: Confirmation is the required gate before the module's central operation (conversion) can happen — it has to exist and work before conversion can be built and tested end-to-end.

**Independent Test**: Can be fully tested by confirming a `Pending` Order and verifying its status becomes `Confirmed`, and that a subsequent edit or cancel attempt on it is rejected.

**Acceptance Scenarios**:

1. **Given** a `Pending` Order, **When** an operator confirms it, **Then** its status becomes `Confirmed`.
2. **Given** an Order that is `Confirmed`, `Converted`, or `Cancelled`, **When** an operator attempts to confirm it again, **Then** the confirmation is rejected.

---

### User Story 4 - Convert a confirmed order to a shipment (Priority: P4)

An operator converts a `Confirmed` Order into an actual Shipment once the package is ready to enter the delivery network — the central business operation of this module.

**Why this priority**: This is the module's stated central operation and its entire reason for existing, but it structurally depends on Orders being creatable, listable, and confirmable first (P1-P3), so it is sequenced after them despite its importance.

**Independent Test**: Can be fully tested by converting a `Confirmed` Order and verifying a new Shipment exists with a correctly-formatted, unique tracking number, that the Shipment has an initial `OrderConverted` event recorded, and that the originating Order's status becomes `Converted`.

**Acceptance Scenarios**:

1. **Given** a `Confirmed` Order, **When** an operator converts it, **Then** a new Shipment is created with a system-generated tracking number matching `TRK-YYYYMMDD-XXXX` for today's date, an initial `OrderConverted` event is recorded for that Shipment, and the Order's status becomes `Converted`.
2. **Given** an Order that is `Pending`, `Converted`, or `Cancelled`, **When** an operator attempts to convert it, **Then** the conversion is rejected and no Shipment is created.
3. **Given** a `Confirmed` Order being converted, **When** any step of the conversion (Shipment creation, event registration, Order status update) would fail, **Then** none of the changes are applied — the Order remains `Confirmed` and no Shipment or event is left behind.
4. **Given** two Shipments created (via conversion) on the same calendar day, **When** both are created, **Then** their tracking numbers share the same date segment and have two different, sequential, zero-padded numeric suffixes.

---

### User Story 5 - Cancel a pending order (Priority: P5)

An operator or the requesting Customer cancels a `Pending` Order that is no longer needed, before it is confirmed.

**Why this priority**: An alternate, lower-frequency path off the main create → confirm → convert flow; valuable but not blocking for the module's primary value.

**Independent Test**: Can be fully tested by cancelling a `Pending` Order and confirming its status becomes `Cancelled`, that it is retained (not deleted), and that a `Confirmed` Order cannot be cancelled this way.

**Acceptance Scenarios**:

1. **Given** a `Pending` Order, **When** it is cancelled, **Then** its status becomes `Cancelled` and it can no longer be edited, confirmed, or converted.
2. **Given** an Order that is `Confirmed`, `Converted`, or already `Cancelled`, **When** a cancellation is attempted, **Then** it is rejected.
3. **Given** a cancelled Order, **When** it is retrieved by its identifier, **Then** all of its original details are still returned unchanged.

---

### User Story 6 - Update a pending order (Priority: P6)

The requesting Customer or an operator corrects an Order's details — recipient, destination, pickup information, or package dimensions/weight — while it is still `Pending` and hasn't yet been reviewed.

**Why this priority**: Data-correction convenience for the earliest, most error-prone stage of an Order's life; lowest priority because a mistaken Pending Order can also simply be cancelled and recreated.

**Independent Test**: Can be fully tested by updating a `Pending` Order's destination and package weight, confirming the changes persist, and confirming the same update is rejected once the Order is no longer `Pending`.

**Acceptance Scenarios**:

1. **Given** a `Pending` Order, **When** its recipient, destination, pickup details, or package dimensions/weight are updated to valid, consistent values, **Then** the Order reflects the new values.
2. **Given** an Order that is `Confirmed`, `Converted`, or `Cancelled`, **When** an update is attempted, **Then** it is rejected and the Order's data remains unchanged.
3. **Given** a `Pending` Order being updated, **When** the update would leave it in an inconsistent state (e.g., switching to `HomePickup` without a pickup address, or supplying both an origin Branch and a pickup address), **Then** the update is rejected — the same structural rules from creation apply.

---

### Edge Cases

- What happens when an Order references a Customer that existed at creation time but has since been deactivated? The Order itself is retained unchanged and remains fully retrievable; only the ability to submit *new* Orders for that Customer is affected (User Story 1, scenario 7).
- What happens when an Order's origin Branch (for `DropOff`) is deactivated after the Order was created but before it is converted? The Order and its Branch reference remain unchanged and retrievable; Branch-active status is only checked at Order creation and update time, not at confirmation or conversion time.
- What happens when the daily order-number/tracking-number sequence would exceed 4 digits (more than 9,999 in one day)? The numeric suffix grows beyond 4 digits rather than rejecting the creation or colliding with an earlier number.
- What happens when an order number or tracking number collision is attempted (extremely unlikely given the sequential generation, but as a safeguard)? The system guarantees uniqueness of both identifiers at all times; a would-be collision is prevented by the sequential generation itself.
- Does cancelling or converting an Order ever remove it from the system? No — Orders are never deleted, matching every other module in this system; `Cancelled` and `Converted` are permanent, retained states.

## Requirements *(mandatory)*

### Functional Requirements

#### Order lifecycle and creation

- **FR-001**: System MUST allow creating an Order for exactly one existing, active Customer, capturing recipient information, destination, pickup information, and package dimensions/weight.
- **FR-002**: System MUST require an origin Branch — which must exist and be active — when an Order's `pickupType` is `DropOff`, and MUST reject a `DropOff` Order that also supplies a pickup address or scheduled pickup datetime.
- **FR-003**: System MUST require a pickup address and a scheduled pickup datetime in the future when an Order's `pickupType` is `HomePickup`, and MUST reject a `HomePickup` Order that also supplies an origin Branch.
- **FR-004**: System MUST generate a unique order number automatically at creation, following the pattern `ORD-YYYYMMDD-XXXX`, where `YYYYMMDD` is the creation date and `XXXX` is a zero-padded sequential number restarting at 1 each calendar day.
- **FR-005**: System MUST reject Order creation or update if package weight or any dimension is zero or negative.
- **FR-006**: System MUST mark every newly created Order with status `Pending`.
- **FR-007**: System MUST NOT provide any capability to permanently delete an Order.

#### Editing, confirming, cancelling

- **FR-008**: System MUST allow updating an Order's recipient, destination, pickup information, or package dimensions/weight only while it is `Pending`, re-validating all applicable creation rules (FR-002, FR-003, FR-005) on every update.
- **FR-009**: System MUST reject any update attempt on an Order that is not `Pending`, leaving its data unchanged.
- **FR-010**: System MUST allow confirming an Order only while it is `Pending`, transitioning it to `Confirmed`.
- **FR-011**: System MUST reject a confirmation attempt on an Order that is not `Pending`.
- **FR-012**: System MUST allow cancelling an Order only while it is `Pending`, transitioning it to `Cancelled`.
- **FR-013**: System MUST reject a cancellation attempt on an Order that is not `Pending`.
- **FR-014**: System MUST treat `Converted` and `Cancelled` as terminal statuses — no further status transition is possible once reached.

#### Conversion to Shipment

- **FR-015**: System MUST allow converting an Order to a Shipment only while it is `Confirmed`, and MUST reject the conversion for an Order in any other status.
- **FR-016**: When converting, system MUST generate a unique tracking number automatically, following the pattern `TRK-YYYYMMDD-XXXX`, where `YYYYMMDD` is the conversion date and `XXXX` is a zero-padded sequential number restarting at 1 each calendar day, independent of the order-number sequence.
- **FR-017**: When converting, system MUST create a new Shipment — in its existing minimal shape, unchanged by this module — carrying a required reference back to the originating Order, and MUST record its first lifecycle event as type `OrderConverted`. The Order, never deleted, remains the system of record for destination, package dimensions/weight, and pickup detail; the Shipment does not duplicate them.
- **FR-018**: When converting, system MUST update the originating Order's status to `Converted`.
- **FR-019**: System MUST apply Shipment creation, the initial event registration, and the Order status update as a single atomic operation — if any part fails, none of it is applied and the Order remains `Confirmed`.

#### Finding and reviewing

- **FR-020**: System MUST allow listing Orders, optionally filtered by status, by Customer, or by both simultaneously.
- **FR-021**: System MUST allow retrieving a single Order by its identifier, returning its full captured detail regardless of status.
- **FR-022**: System MUST report a clear not-found result when an operator retrieves, updates, confirms, cancels, or converts an Order identifier that does not exist.

### Key Entities

- **Order**: A shipment request placed by one Customer, before any package is physically collected. Attributes: system-generated order number (unique, immutable), status (`Pending`/`Confirmed`/`Converted`/`Cancelled`), the owning Customer, pickup type (`DropOff`/`HomePickup`) with its type-specific fields (origin Branch, or pickup address + scheduled pickup datetime), recipient information, destination, package dimensions and weight, creation and last-update timestamps. Never deleted.
- **Shipment** *(existing entity from prior modules)*: The physical parcel once it has entered the delivery network. Created only through Order conversion in this module. Keeps its existing minimal shape unchanged; gains a required reference back to the originating Order, which remains the system of record for destination, dimensions/weight, and pickup detail (see Clarifications).
- **ShipmentEvent**: A record of something that happened to a Shipment over its lifecycle. This module introduces the concept and creates the first entry for every newly-converted Shipment, of type `OrderConverted`; the type is modeled openly so future modules can register further event types (e.g., pickup, in-transit, delivery) without a structural change.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: An operator can take an Order from creation through confirmation to a converted Shipment in a single continuous workflow, without re-entering any previously captured data.
- **SC-002**: 100% of order numbers and tracking numbers generated by this module are unique and match their documented format.
- **SC-003**: No Shipment can ever be created from an Order that was not explicitly confirmed first.
- **SC-004**: An operator can find every Order in a given status, or belonging to a given Customer, in a single lookup, without manually inspecting unrelated records.
- **SC-005**: Cancelled and converted Orders remain permanently retrievable with their original detail intact — no Order is ever lost through deletion.

## Assumptions

- **No authorization tiers**: Consistent with the rest of this system, no role-based restriction is assumed on who may perform these operations (Customer self-service vs. operator); any authenticated user of the system may create, list, retrieve, update, confirm, cancel, or convert Orders.
- **Active-Customer and active-Branch requirement, checked at creation and update time only**: Consistent with the precedent established for Employee/Vehicle referencing Branch, and Business/Individual Customer uniqueness, an Order's Customer must be active at creation time, and (for `DropOff`) its origin Branch must be active at creation and update time. Neither is re-checked at confirm, cancel, or convert time (see Edge Cases).
- **Order number and tracking number sequences are global, not scoped per Branch or Customer**: The stated formats (`ORD-YYYYMMDD-XXXX`, `TRK-YYYYMMDD-XXXX`) contain no Branch or Customer segment, so the daily sequential counter is company-wide.
- **Recipient and destination fields**: A recipient name and phone are captured, along with a destination address using the same shape already established for Branch/Customer addresses (street, city, state, zip code, country), all free text with no fixed code lists.
- **Package dimensions and weight units**: Weight is captured in kilograms and dimensions (length, width, height) in centimeters, consistent with the unit convention already used for Vehicle capacity in this system; no other unit system is supported.
- **Pickup address is a single free-text field**: Not broken into street/city/state/zip/country sub-fields, since the request describes it simply as "a pickup address" rather than a structured address like the Customer/Branch/destination address.
- **No relationship beyond the Order↔Customer and Order↔Branch references**: This module does not model any other cross-module relationship (e.g., to Employee or Vehicle); assigning staff or a vehicle to a converted Shipment is out of scope here.
- **Orders and their referenced Shipment/ShipmentEvent records are never deleted**: Matching the soft-delete-only convention used by every other module in this system, `Cancelled` and `Converted` are permanent, retained end states.
