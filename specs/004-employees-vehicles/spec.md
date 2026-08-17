# Feature Specification: Employees & Vehicles Module

**Feature Branch**: `[004-employees-vehicles]`

**Created**: 2026-08-17

**Status**: Draft

**Input**: User description: "Build the Employees & Vehicles module for a parcel delivery company.

  An Employee is a member of the company's own operational staff — no freelancers
  or external contractors. Every employee belongs to exactly one Branch.
  Employees have a role that defines their function in the logistics network:
  Operator (handles pickups), Driver (last-mile delivery), WarehouseStaff,
  or BranchManager. They have a unique employee number, a unique email,
  a hire date, and an active/inactive status.
  Employees can be deactivated but never deleted.

  A Vehicle is a company-owned asset assigned to a Branch (not to a specific employee).
  It has a unique license plate, a type (Motorcycle, Van, Truck), brand, model,
  year of manufacture, maximum load capacity in kilograms, and an active/inactive status.
  A Vehicle can be reassigned to a different Branch.
  Vehicles can be deactivated but never deleted.

  The system must allow listing Drivers filtered by Branch, since this query
  is needed when assigning a Driver to a shipment event.

  Users can create, update, list, and deactivate both Employees and Vehicles.
  Listing Employees supports optional filters by Branch and by Role.
  Listing Vehicles supports an optional filter by Branch."

## Clarifications

### Session 2026-08-17

- Q: When creating or updating an Employee or Vehicle, must the Branch they're assigned to be active, or can they be assigned to a currently-inactive Branch? → A: Active branches only — create/update is rejected if the referenced branch is inactive.
- Q: Can an Employee be reassigned to a different Branch via update, the same way Vehicles explicitly can be? → A: Yes — Branch is just another editable field on Employee update, same as Vehicle reassignment.
- Q: Are employee number and email required to be unique company-wide, or only within the branch the employee belongs to? → A: Unique company-wide — no two employees anywhere in the company can share an employee number or email.
- Q: Can a former employee's employee number or email be reused by a new hire once that employee is deactivated, or does the uniqueness rule apply forever, even against inactive records? → A: Blocked forever, even if inactive — uniqueness checks compare against every employee record, active or inactive.
- Q: Should the system provide a way to retrieve a single Employee or Vehicle by its identifier, separate from list filtering? → A: Yes — add single-record retrieval by id for both Employee and Vehicle, consistent with the existing Branch module.
- Q: Should there be any restriction on which role an employee can transition to when their role is updated? → A: Unrestricted — an employee's role can be changed to any of the four values at any time, no transition rules.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Register a new employee (Priority: P1)

An operations staff member adds a new employee to the company's roster, recording their role, assigned branch, employee number, email, and hire date so they can begin appearing in staffing and assignment workflows.

**Why this priority**: No other capability in this module has anything to operate on until employees exist. This is the foundation for staffing visibility and, ultimately, for assigning drivers to shipments.

**Independent Test**: Can be fully tested by submitting a new employee with a unique employee number and email, a valid role, and an active branch, and confirming the employee is created and marked active.

**Acceptance Scenarios**:

1. **Given** an active branch exists, **When** a user creates an employee with role Driver, a unique employee number, a unique email, and a hire date, assigned to that branch, **Then** the employee is created and marked active by default.
2. **Given** an employee number or email already used by another employee, **When** a user attempts to create a new employee with that same employee number or email, **Then** the creation is rejected and the specific validation problem is reported.
3. **Given** a branch that is inactive (or a branch id that does not exist), **When** a user attempts to create an employee assigned to that branch, **Then** the creation is rejected and the specific validation problem is reported.
4. **Given** a user is creating an employee, **When** they submit a role value outside the four defined roles (Operator, Driver, WarehouseStaff, BranchManager), **Then** the creation is rejected and the specific validation problem is reported.

---

### User Story 2 - Find drivers available at a branch (Priority: P2)

An operations staff member preparing to assign a driver to a shipment event looks up the active drivers currently staffed at a specific branch.

**Why this priority**: This is the explicit operational need driving this module — shipment assignment cannot proceed without a reliable way to find eligible, active drivers at the right branch. It only requires employees to exist (User Story 1), not vehicles.

**Independent Test**: Can be fully tested by creating employees with different roles and branches (via US1), then listing employees filtered by a specific branch and role Driver, and confirming only active drivers at that branch are returned.

**Acceptance Scenarios**:

1. **Given** employees of different roles exist at a branch, **When** a user lists employees filtered by that branch and role Driver, **Then** only active employees with role Driver at that branch are returned.
2. **Given** employees exist across multiple branches, **When** a user lists employees filtered by a specific branch only (no role filter), **Then** only active employees at that branch are returned, regardless of role.
3. **Given** employees of a specific role exist across multiple branches, **When** a user lists employees filtered by that role only (no branch filter), **Then** only active employees with that role are returned, regardless of branch.
4. **Given** a branch with no active drivers, **When** a user lists employees filtered by that branch and role Driver, **Then** an empty list is returned, not an error.
5. **Given** an existing employee, **When** a user retrieves that single employee by their identifier, **Then** the employee's full details are returned.
6. **Given** an employee identifier that does not exist, **When** a user retrieves it, **Then** the system reports that the employee was not found.

---

### User Story 3 - Register a new vehicle (Priority: P3)

An operations staff member adds a new company-owned vehicle to the fleet, recording its license plate, type, brand, model, year, load capacity, and the branch it's assigned to.

**Why this priority**: Establishes the fleet roster, the foundation for fleet visibility per branch. Independent of the employee-related stories above.

**Independent Test**: Can be fully tested by submitting a new vehicle with a unique license plate and a valid type, assigned to an active branch, and confirming the vehicle is created and marked active.

**Acceptance Scenarios**:

1. **Given** an active branch exists, **When** a user creates a vehicle with a unique license plate, type Van, brand, model, year, and load capacity, assigned to that branch, **Then** the vehicle is created and marked active by default.
2. **Given** a license plate already used by another vehicle, **When** a user attempts to create a new vehicle with that same license plate, **Then** the creation is rejected and the specific validation problem is reported.
3. **Given** a branch that is inactive (or a branch id that does not exist), **When** a user attempts to create a vehicle assigned to that branch, **Then** the creation is rejected and the specific validation problem is reported.
4. **Given** a user is creating a vehicle, **When** they submit a type value outside the three defined types (Motorcycle, Van, Truck), **Then** the creation is rejected and the specific validation problem is reported.

---

### User Story 4 - View the fleet at a branch (Priority: P4)

An operations staff member looks up the active vehicles currently assigned to a specific branch.

**Why this priority**: Mirrors User Story 2 for vehicles — fleet visibility per branch is a direct, explicitly requested capability, but ranked after driver lookup since the shipment-assignment need is the module's primary stated driver.

**Independent Test**: Can be fully tested by creating vehicles across different branches (via US3), then listing vehicles filtered by a specific branch, and confirming only active vehicles at that branch are returned.

**Acceptance Scenarios**:

1. **Given** vehicles exist across multiple branches, **When** a user lists vehicles filtered by a specific branch, **Then** only active vehicles at that branch are returned.
2. **Given** a branch with no active vehicles, **When** a user lists vehicles filtered by that branch, **Then** an empty list is returned, not an error.
3. **Given** no filter is applied, **When** a user lists vehicles, **Then** all active vehicles across all branches are returned.
4. **Given** an existing vehicle, **When** a user retrieves that single vehicle by its identifier, **Then** the vehicle's full details are returned.
5. **Given** a vehicle identifier that does not exist, **When** a user retrieves it, **Then** the system reports that the vehicle was not found.

---

### User Story 5 - Update employee information (Priority: P5)

An operations staff member corrects or updates an employee's details — such as their role, hire date, employee number, email, or assigned branch — as real-world conditions change (e.g., a promotion or a transfer to another branch).

**Why this priority**: Keeps staffing records accurate over time, but is only meaningful once employees exist and can be found.

**Independent Test**: Can be fully tested by updating an existing employee's role and branch assignment, then retrieving the employee (or re-listing) to confirm the changes were persisted and re-validated.

**Acceptance Scenarios**:

1. **Given** an existing employee, **When** a user updates their role, hire date, employee number, or email to valid, non-conflicting values, **Then** the employee reflects the new values.
2. **Given** an existing employee, **When** a user updates their assigned branch to a different active branch, **Then** the employee is now associated with the new branch and no longer appears in listings filtered by the old branch.
3. **Given** an existing employee, **When** a user attempts to update their employee number or email to a value already used by another employee, **Then** the update is rejected and the employee's prior data remains unchanged.
4. **Given** an existing employee, **When** a user attempts to reassign them to an inactive branch (or a branch id that does not exist), **Then** the update is rejected and the employee's prior data remains unchanged.

---

### User Story 6 - Update vehicle information (Priority: P6)

An operations staff member corrects or updates a vehicle's details — such as its brand, model, load capacity, or assigned branch — as real-world conditions change (e.g., a vehicle transferred to another branch).

**Why this priority**: Mirrors User Story 5 for vehicles; keeps fleet records accurate over time.

**Independent Test**: Can be fully tested by updating an existing vehicle's branch assignment, then re-listing to confirm the vehicle now appears under its new branch and not its old one.

**Acceptance Scenarios**:

1. **Given** an existing vehicle, **When** a user updates its brand, model, year, or load capacity to valid values, **Then** the vehicle reflects the new values.
2. **Given** an existing vehicle, **When** a user reassigns it to a different active branch, **Then** the vehicle is now associated with the new branch and no longer appears in listings filtered by the old branch.
3. **Given** an existing vehicle, **When** a user attempts to update its license plate to a value already used by another vehicle, **Then** the update is rejected and the vehicle's prior data remains unchanged.
4. **Given** an existing vehicle, **When** a user attempts to reassign it to an inactive branch (or a branch id that does not exist), **Then** the update is rejected and the vehicle's prior data remains unchanged.

---

### User Story 7 - Deactivate an employee (Priority: P7)

An operations staff member deactivates an employee who has left the company, removing them from staffing and assignment workflows while preserving their record.

**Why this priority**: Lower-frequency action than the above, and depends on employees already existing.

**Independent Test**: Can be fully tested by deactivating an active employee and confirming they no longer appear in any listing (including driver-by-branch lookups) but their record is preserved (not deleted).

**Acceptance Scenarios**:

1. **Given** an active employee, **When** a user deactivates them, **Then** their status becomes inactive and they no longer appear in any employee listing, including driver-by-branch lookups.
2. **Given** an already-inactive employee, **When** a user deactivates them again, **Then** they remain inactive without error (the action is idempotent).
3. **Given** an employee (active or inactive), **When** any user attempts to permanently delete them, **Then** no such capability exists — deactivation is the only way to remove an employee from active operations.

---

### User Story 8 - Deactivate a vehicle (Priority: P8)

An operations staff member retires a vehicle that is no longer in service, removing it from the active fleet while preserving its record.

**Why this priority**: Mirrors User Story 7 for vehicles; lowest-frequency action in the module.

**Independent Test**: Can be fully tested by deactivating an active vehicle and confirming it no longer appears in fleet listings but its record is preserved (not deleted).

**Acceptance Scenarios**:

1. **Given** an active vehicle, **When** a user deactivates it, **Then** its status becomes inactive and it no longer appears in any vehicle listing.
2. **Given** an already-inactive vehicle, **When** a user deactivates it again, **Then** it remains inactive without error (the action is idempotent).
3. **Given** a vehicle (active or inactive), **When** any user attempts to permanently delete it, **Then** no such capability exists — deactivation is the only way to retire a vehicle.

---

### Edge Cases

- What happens when an employee number, email, or license plate collides with an existing one only after trimming whitespace or differing by case? Treated as a duplicate and rejected — uniqueness checks are case-insensitive and ignore leading/trailing whitespace.
- What happens when a vehicle's year of manufacture is set in the future, or to an implausible value? Rejected — year must be a real, non-future year.
- What happens when a vehicle's maximum load capacity is zero or negative? Rejected — load capacity must be a positive number.
- What happens when listing employees or vehicles with an invalid/unrecognized branch id, role, or type filter value? The request is rejected with a validation error rather than silently returning no results.
- What happens when a branch referenced by existing employees or vehicles is later deactivated? Existing employees/vehicles remain associated with that now-inactive branch (their data is not altered), but any *new* create/reassignment to that branch is rejected until it becomes active again (see Clarifications).
- Does deactivating or reassigning a vehicle affect any employee, or vice versa? No — vehicles are assigned to a Branch only, never to a specific employee, so there is no direct relationship between the two entities to cascade.

## Requirements *(mandatory)*

### Functional Requirements

#### Employees

- **FR-001**: System MUST allow users to create an Employee with a name, a unique employee number, a unique email, a role (Operator, Driver, WarehouseStaff, or BranchManager), a hire date, and an assigned Branch.
- **FR-002**: System MUST reject Employee creation or update if the employee number or email is already in use by a different employee, company-wide, regardless of whether that other employee is active or inactive — a used employee number or email is never available for reuse.
- **FR-003**: System MUST reject Employee creation or update if the assigned Branch does not exist or is not currently active.
- **FR-004**: System MUST mark every newly created Employee as active by default.
- **FR-005**: System MUST allow users to update an existing Employee's name, role, hire date, employee number, email, and assigned Branch (reassignment), re-validating all rules in FR-001 through FR-003 on every update, and MUST reject the update — leaving prior data unchanged — if validation fails. Role may be changed to any of the four defined roles at any time; there are no restricted transition paths between roles.
- **FR-006**: System MUST allow users to deactivate an active Employee, setting their status to inactive.
- **FR-007**: System MUST treat deactivating an already-inactive Employee as a no-op that does not raise an error.
- **FR-008**: System MUST NOT provide any capability to permanently delete an Employee.
- **FR-009**: System MUST allow users to list Employees, optionally filtered by Branch, by Role, or both simultaneously.
- **FR-010**: System MUST include only active Employees in any Employee listing — deactivated employees are never returned, including when filtering by Branch and Role together for driver lookup.

#### Vehicles

- **FR-011**: System MUST allow users to create a Vehicle with a unique license plate, a type (Motorcycle, Van, or Truck), brand, model, year of manufacture, maximum load capacity, and an assigned Branch.
- **FR-012**: System MUST reject Vehicle creation or update if the license plate is already in use by a different vehicle, company-wide, regardless of whether that other vehicle is active or inactive — a used license plate is never available for reuse (consistent with FR-002's rule for employees; a real-world plate is never reissued to a different vehicle either).
- **FR-013**: System MUST reject Vehicle creation or update if the assigned Branch does not exist or is not currently active.
- **FR-014**: System MUST mark every newly created Vehicle as active by default.
- **FR-015**: System MUST allow users to update an existing Vehicle's license plate, type, brand, model, year, load capacity, and assigned Branch (reassignment), re-validating all rules in FR-011 through FR-013 on every update, and MUST reject the update — leaving prior data unchanged — if validation fails.
- **FR-016**: System MUST allow users to deactivate an active Vehicle, setting its status to inactive.
- **FR-017**: System MUST treat deactivating an already-inactive Vehicle as a no-op that does not raise an error.
- **FR-018**: System MUST NOT provide any capability to permanently delete a Vehicle.
- **FR-019**: System MUST allow users to list Vehicles, optionally filtered by Branch.
- **FR-020**: System MUST include only active Vehicles in any Vehicle listing.

#### Validation

- **FR-021**: System MUST validate that a Vehicle's year of manufacture is a real year that is not in the future.
- **FR-022**: System MUST validate that a Vehicle's maximum load capacity is a positive number.

#### Retrieval

- **FR-023**: System MUST allow users to retrieve a single Employee by their identifier.
- **FR-024**: System MUST allow users to retrieve a single Vehicle by its identifier.
- **FR-025**: System MUST report a clear not-found result when a user retrieves or updates an Employee or Vehicle identifier that does not exist.

### Key Entities

- **Employee**: A member of the company's own operational staff. Attributes: name, employee number (unique, company-wide), email (unique, company-wide), role (one of Operator, Driver, WarehouseStaff, BranchManager), hire date, active/inactive status. Belongs to exactly one Branch at a time.
- **Vehicle**: A company-owned asset. Attributes: license plate (unique, company-wide), type (one of Motorcycle, Van, Truck), brand, model, year of manufacture, maximum load capacity (kilograms), active/inactive status. Assigned to exactly one Branch at a time — never to a specific employee.
- **Branch** *(existing entity, referenced by this module)*: Every Employee and Vehicle references an active Branch. This module does not modify Branch itself.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A staff member can register a new employee, fully assigned to a branch and role, in a single submission without needing follow-up corrections for well-formed input.
- **SC-002**: A staff member can find every active driver at a given branch in a single lookup when preparing a shipment assignment, without manually filtering through unrelated staff or inactive records.
- **SC-003**: 100% of employees and vehicles in the system are associated with a Branch that exists and was active at the time of assignment — no orphaned or invalid branch references can ever be created.
- **SC-004**: A staff member can register a new vehicle and view the complete active fleet at any branch in a single lookup.
- **SC-005**: Deactivated employees and vehicles are never returned in any listing (eliminating the risk of assigning an inactive driver or vehicle to a shipment), while their historical records remain fully intact and are never lost through deletion.

## Assumptions

- **Employee name field**: Although not explicitly listed among the described attributes, a name field is included as a baseline requirement for identifying staff — an employee record without a human-readable name would not be usable in practice.
- **Company-wide uniqueness**: Confirmed during clarification — employee number and email are unique across the whole company, not just within a branch; the same is assumed for Vehicle license plate by the same logic (a physical plate cannot be reused across branches either). This uniqueness is permanent: confirmed during clarification that it applies against inactive records too, so a retired employee number, email, or license plate can never be reused (FR-002, FR-012).
- **Active-branch-only assignment**: Confirmed during clarification — an Employee or Vehicle can only be created or reassigned to a Branch that currently exists and is active. Existing assignments are not retroactively invalidated if their Branch is later deactivated.
- **Employee branch reassignment**: Confirmed during clarification — an Employee's Branch can be changed via update, the same as Vehicle reassignment.
- **Unrestricted role changes**: Confirmed during clarification — an Employee's role can be changed to any of the four defined roles at any time via update; unlike Shipment status, there is no transition-rule validator restricting which role changes are allowed.
- **Single-record retrieval**: Confirmed during clarification — both Employee and Vehicle support retrieval by id, in addition to filtered listing (FR-023, FR-024).
- **No inactive-record listing**: Unlike the Branches module, no active-status filter was requested for Employee or Vehicle listings. Listings always return only active records — this is treated as a safety requirement (SC-005) so a deactivated driver or vehicle can never be surfaced for operational assignment.
- **No authorization tiers**: Consistent with the rest of this system (see Branches & Hubs module), no role-based restriction is assumed on who may perform these operations; any authenticated user of the system may create, update, list, and deactivate Employees and Vehicles.
- **No Employee–Vehicle relationship**: Vehicles are assigned to a Branch only, never to a specific Employee, per the explicit description — this module does not model driver-to-vehicle assignment.
- **Out of scope**: Contractor/freelancer tracking (explicitly excluded by the description), and any relationship between Employees/Vehicles and Shipments beyond the driver-lookup query described here.
