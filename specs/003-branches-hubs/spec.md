# Feature Specification: Branches & Hubs Module

**Feature Branch**: `[003-branches-hubs]`

**Created**: 2026-08-17

**Status**: Draft

**Input**: User description: "Build the Branches & Hubs module for a parcel delivery company.

  A Branch is a physical location in the logistics network. It can be one of four types:
  Headquarters, Hub (distribution center), SalesPoint, or PickupPoint.

  Each Branch has a name, type, full address (street, city, state, zip code),
  optional geographic coordinates (latitude/longitude), optional phone, and
  an active/inactive status. Branches can be deactivated but never deleted.

  Each Branch has a weekly schedule: one entry per day of the week, where each entry
  defines opening and closing times, or marks the day as closed. A Branch must have
  at least one schedule entry. No duplicate days are allowed in the same branch's schedule.
  Opening time must be earlier than closing time on non-closed days.

  Users can create, update, list, and deactivate branches.
  Listing supports optional filters by active status and by branch type.
  Retrieving a single branch always includes its schedule."

## Clarifications

### Session 2026-08-17

- Q: Should creating, updating, or deactivating a branch be restricted to certain user roles, or can any authenticated user perform these actions? → A: No role restriction — any authenticated user can create, update, list, and deactivate branches.
- Q: When a schedule entry is marked "closed" but also includes opening/closing times, should the submission be rejected or should the closed flag override and ignore the times? → A: Reject the submission as invalid — closed days must not include opening/closing times.
- Q: Should the branch address's "state" field be restricted to a fixed set of values (e.g., valid US state/territory codes), or accepted as free-text? → A: Free-text, required non-empty string.
- Q: Should branch names be required to be unique within the same city, or is the current rule (no uniqueness constraint at all, anywhere) still correct? → A: No uniqueness constraint at all — same-named branches are allowed anywhere, including within the same city.
- Q: What time zone do a branch's opening/closing times represent — the branch's own local time zone, or a single fixed reference time zone? → A: Branch-local time — `opensAt`/`closesAt` are plain time-of-day values with no time zone attached.
- Q: Can a branch's address, phone, coordinates, or schedule be edited via update while that branch is inactive, or is update restricted to reactivating it until it's active again? → A: No restriction — any field can be edited on an inactive branch via update, independent of reactivating it.
- Q: When create/update receives a branch type value that isn't one of the four defined types, should the request be rejected or fall back to a default type? → A: Reject the request — an unrecognized branch type on create/update is a validation error, same treatment as the list filter.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Register a new branch (Priority: P1)

An operations staff member adds a new physical location to the logistics network, recording its type, address, and full weekly operating schedule so it can begin appearing in company systems.

**Why this priority**: Without the ability to create a branch, no other capability in this module (listing, updating, deactivating) has anything to operate on. This is the foundation of the module.

**Independent Test**: Can be fully tested by submitting a new branch with a name, type, complete address, and a 7-day schedule, and confirming the branch is created, marked active, and its schedule is stored exactly as entered.

**Acceptance Scenarios**:

1. **Given** no existing branch with the submitted details, **When** a user creates a branch of type Hub with a complete address and a full 7-day schedule (each day either has opening/closing times or is marked closed), **Then** the branch is created, is marked active by default, and its schedule matches what was submitted.
2. **Given** a user is creating a branch, **When** they omit optional fields (coordinates, phone), **Then** the branch is created successfully without those values.
3. **Given** a user is creating a branch, **When** they submit a schedule with fewer than 7 days, more than 7 days, a duplicate day, or an opening time that is not earlier than the closing time on a non-closed day, **Then** the branch is rejected and the specific validation problem is reported.
4. **Given** a user is creating a branch, **When** they omit a required address field (street, city, state, or zip code) or the branch type, **Then** the branch is rejected and the specific validation problem is reported.

---

### User Story 2 - Find and review branches (Priority: P2)

An operations staff member browses the network to find branches by type or status, and drills into a specific branch to review its full details and weekly schedule.

**Why this priority**: Once branches exist, staff need to locate and inspect them (e.g., to plan routes, verify coverage, or check operating hours) before any updates or deactivations are meaningful.

**Independent Test**: Can be fully tested by listing branches with and without filters, and retrieving a single branch to confirm its schedule is always included.

**Acceptance Scenarios**:

1. **Given** branches of multiple types and statuses exist, **When** a user lists branches with no filters applied, **Then** only active branches are returned.
2. **Given** branches of multiple types and statuses exist, **When** a user lists branches filtered by a specific type (e.g., Hub), **Then** only active branches of that type are returned.
3. **Given** both active and inactive branches exist, **When** a user lists branches explicitly filtered to inactive status, **Then** only inactive branches are returned.
4. **Given** a user lists branches filtered by both status and type simultaneously, **When** the filters are applied, **Then** only branches matching both criteria are returned.
5. **Given** an existing branch, **When** a user retrieves that single branch by its identifier, **Then** the response includes all branch details plus its complete weekly schedule.
6. **Given** a branch identifier that does not exist, **When** a user retrieves it, **Then** the system reports that the branch was not found.

---

### User Story 3 - Update branch information (Priority: P3)

An operations staff member corrects or refreshes a branch's details — such as its address, contact phone, coordinates, or weekly schedule — as real-world conditions change.

**Why this priority**: Branch data drifts over time (new phone numbers, adjusted hours, corrected addresses). This capability keeps the network directory accurate but is only meaningful after branches exist and can be found.

**Independent Test**: Can be fully tested by updating an existing branch's address and schedule, then retrieving the branch to confirm the changes were persisted and re-validated.

**Acceptance Scenarios**:

1. **Given** an existing active branch, **When** a user updates its address, phone, or coordinates, **Then** the branch reflects the new values on next retrieval.
2. **Given** an existing branch, **When** a user submits a replacement weekly schedule that is valid (7 days, no duplicates, valid opening/closing times), **Then** the branch's schedule is fully replaced with the new one.
3. **Given** an existing branch, **When** a user submits an update with an invalid schedule (duplicate day, missing day, or opening time not earlier than closing time), **Then** the update is rejected, the specific validation problem is reported, and the branch's prior data remains unchanged.
4. **Given** an inactive branch, **When** a user updates its active status back to active, **Then** the branch becomes active again and continues to appear in default listings.
5. **Given** an inactive branch, **When** a user updates its address, phone, coordinates, or schedule without changing its active status, **Then** the update succeeds and the branch remains inactive — editing other fields is not restricted while a branch is inactive.

---

### User Story 4 - Deactivate a branch (Priority: P4)

An operations staff member retires a branch that has closed or is no longer in service, removing it from active operations while preserving its full history.

**Why this priority**: Deactivation is a lower-frequency action than creating, viewing, or updating, and depends on a branch already existing. Preserving history (never deleting) protects downstream records (e.g., past shipments) that reference the branch.

**Independent Test**: Can be fully tested by deactivating an active branch and confirming it no longer appears in default listings but remains fully retrievable by its identifier with all data intact.

**Acceptance Scenarios**:

1. **Given** an active branch, **When** a user deactivates it, **Then** its status becomes inactive and it no longer appears in default (unfiltered) listings.
2. **Given** an inactive branch, **When** a user retrieves it directly by identifier, **Then** all of its details and schedule are still returned unchanged.
3. **Given** an already-inactive branch, **When** a user deactivates it again, **Then** the branch remains inactive without error (the action is idempotent).
4. **Given** a branch (active or inactive), **When** any user attempts to permanently delete it, **Then** no such capability exists — deactivation is the only way to retire a branch.

---

### Edge Cases

- What happens when a schedule is submitted with zero entries? The branch is rejected — at least one entry is required, and (per the full-week rule) a complete schedule requires all 7 days.
- What happens when latitude/longitude are submitted outside valid geographic ranges? The branch is rejected with a validation error.
- What happens when opening time equals closing time on a non-closed day? Rejected — opening must be strictly earlier than closing.
- What happens when a day is marked closed but also has opening/closing times supplied? The submission is rejected as inconsistent — closed days must not carry opening/closing times.
- How does the system handle an update that changes a branch's type (e.g., SalesPoint to Hub)? Type is an editable attribute like any other field; no special restriction applies.
- What happens when listing filters use an invalid/unrecognized branch type value? The request is rejected with a validation error rather than silently returning no results.
- What happens when a create or update request uses a branch type value outside the four defined types? The request is rejected with a validation error, the same treatment as an invalid type on the listing filter — never a silent fallback to a default type.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow users to create a branch with a name, a branch type (Headquarters, Hub, SalesPoint, or PickupPoint), and a full address (street, city, state, zip code), where each address field is a required non-empty string (state is free-text, not restricted to a fixed list of codes). A branch type value outside these four is a validation error on both create and update (FR-008) — never a silent fallback to a default type.
- **FR-002**: System MUST allow a branch's geographic coordinates (latitude, longitude) and phone number to be optionally provided, and MUST allow branches to be created and saved without them.
- **FR-003**: System MUST mark every newly created branch as active by default.
- **FR-004**: System MUST require every branch to have exactly one schedule entry for each of the 7 days of the week at all times (on creation and after any update).
- **FR-005**: System MUST reject a branch schedule that contains more than one entry for the same day.
- **FR-006**: System MUST require that, for any schedule entry not marked as closed, the opening time is strictly earlier than the closing time.
- **FR-007**: System MUST allow a schedule entry to be marked as closed for a given day, in which case no opening/closing times apply for that day.
- **FR-008**: System MUST allow users to update an existing branch's name, type, address, coordinates, phone, active status, and/or weekly schedule, regardless of whether the branch is currently active or inactive — updating other fields is never restricted or gated behind reactivating the branch first.
- **FR-009**: System MUST re-validate all schedule and address rules (FR-004 through FR-007, FR-001's address completeness) whenever a branch is updated, and MUST reject the update — leaving prior data unchanged — if validation fails.
- **FR-010**: System MUST allow users to deactivate an active branch, setting its status to inactive.
- **FR-011**: System MUST treat deactivating an already-inactive branch as a no-op that does not raise an error.
- **FR-012**: System MUST NOT provide any capability to permanently delete a branch; deactivation is the only supported way to retire one.
- **FR-013**: System MUST allow users to list branches, optionally filtered by active status, by branch type, or by both simultaneously.
- **FR-014**: System MUST return only active branches when a list request is made with no active-status filter specified.
- **FR-015**: System MUST allow users to retrieve a single branch by its identifier, and the response MUST always include that branch's complete weekly schedule.
- **FR-016**: System MUST report a clear not-found result when a user retrieves or updates a branch identifier that does not exist.
- **FR-017**: System MUST reject a schedule where a day is simultaneously marked closed and given opening/closing times, reporting it as an inconsistent submission.
- **FR-018**: System MUST validate that submitted latitude and longitude values, when present, fall within valid real-world geographic ranges.
- **FR-019**: System MUST treat a schedule entry's opening and closing times as branch-local time-of-day values with no time zone attached — the same time of day at any branch means that branch's own local time, not a shared reference zone.

### Key Entities

- **Branch**: A physical location in the logistics network. Attributes: name, branch type (one of Headquarters, Hub, SalesPoint, PickupPoint), full address (street, city, state, zip code), optional coordinates (latitude, longitude), optional phone, active/inactive status. Has exactly one weekly schedule.
- **Schedule Entry**: One day's operating-hours record within a branch's weekly schedule. Attributes: day of week, closed flag, opening time and closing time (present only when not closed, expressed as branch-local time-of-day with no time zone attached — FR-019). Belongs to exactly one branch; exactly one entry exists per day of the week.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A staff member can register a new branch, including its full 7-day schedule, in a single submission without needing follow-up corrections for well-formed input.
- **SC-002**: 100% of branches that exist in the system have a complete, valid 7-day schedule — no branch can ever be saved with a missing day, a duplicate day, or an invalid opening/closing time pair.
- **SC-003**: A staff member can find every active branch of a given type in a single lookup, without having to manually inspect deactivated or unrelated branches.
- **SC-004**: Deactivated branches retain 100% of their original details and schedule, retrievable at any time, with zero data loss.
- **SC-005**: A staff member can determine any branch's operating hours for any specific day with a single retrieval, without contacting another team or system.

## Assumptions

- **Weekly schedule completeness**: A branch's schedule always contains exactly 7 entries (one per day of the week), each either specifying opening/closing times or marked closed. This was confirmed during clarification.
- **Default listing scope**: When no active-status filter is supplied, listing returns active branches only. This was confirmed during clarification.
- **Closed-day consistency**: Confirmed during clarification — a schedule entry cannot be marked closed and also carry opening/closing times; that combination is rejected as an invalid, inconsistent submission rather than silently ignoring the extra times.
- **No authorization tiers**: Confirmed during clarification — there is no role-based restriction (e.g., admin-only) on who may create, update, or deactivate branches; any authenticated user of the system may perform these actions.
- **No uniqueness constraint on branch name**: Confirmed during clarification — branch names are not required to be unique across the system, including within the same city; two branches may share a name (e.g., two "Downtown" locations in the same city or in different cities) since address/coordinates are the true differentiators. A per-city uniqueness rule was explicitly considered and rejected as unnecessary scope.
- **Full replacement on schedule update**: When a branch's schedule is updated, the new schedule fully replaces the prior one (rather than being merged entry-by-entry).
- **Reactivation via update**: There is no separate "activate" action; setting an inactive branch's status back to active is done through the standard update capability (see FR-008).
- **No cross-module referential impact defined**: This module does not define any relationship between a Branch and other entities (e.g., shipments routed through it). Deactivating a branch has no defined effect outside this module — any such relationship, and what deactivation should do to it, is out of scope here and would need its own specification if introduced later.
