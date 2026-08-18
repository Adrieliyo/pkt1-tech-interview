# Feature Specification: Authentication & Authorization

**Feature Branch**: `[008-authentication-authorization]`

**Created**: 2026-08-18

**Status**: Draft

**Input**: User description: "Add authentication and role-based authorization to the existing
  parcel delivery company API.

  Only internal company staff can authenticate — no external customers.
  Authentication is handled via ASP.NET Core Identity with cookie-based
  sessions. There are no third-party identity providers.

  Every employee in the system already has a corresponding Employee entity
  with a Role field (EmployeeRole enum: Operator, Driver, WarehouseStaff,
  BranchManager). Authentication must be tied to this existing Employee
  entity — a new ApplicationUser must reference the Employee it belongs to.

  Users authenticate by providing their email and password.
  On success, a session cookie is issued. On logout, the session is
  invalidated server-side.

  Authorization is role-based. Each API endpoint is restricted to one
  or more roles according to the following rules:

  BranchManager:
  - Full access to all endpoints across all modules.
  - Can create, update, and deactivate Employees and Vehicles.
  - Can confirm and convert Orders.
  - Can issue and cancel Invoices.

  Operator:
  - Can create and update Orders.
  - Can confirm Orders.
  - Can register ShipmentEvents (except DeliveryAttempted).
  - Can read Customers, Branches, Shipments and their events.
  - Cannot access Employee management, Vehicle management,
    or Invoice financial data.

  Driver:
  - Can register ShipmentEvents including DeliveryAttempted.
  - Can read their own assigned Shipments only.
  - Cannot access Orders, Customers, Branches, Employees,
    Vehicles or Invoices.

  WarehouseStaff:
  - Can register ShipmentEvents of types ReceivedAtBranch,
    DepartedFromBranch and InTransit only.
  - Can read Shipments and their event history.
  - Cannot access Orders, Customers, Employees, Vehicles or Invoices.

  The public tracking endpoint GET /api/shipments/tracking/{number}
  remains unauthenticated — no cookie required.

  A SuperAdmin role must exist for system administration tasks
  such as seeding initial users. It has unrestricted access to all
  endpoints and is not tied to any EmployeeRole.

  Password reset via email is out of scope for this implementation.
  Social login is out of scope.
  Two-factor authentication is out of scope."

## Clarifications

### Session 2026-08-18

- Q: What does "Driver can read their own assigned Shipments only" mean, given no Shipment currently has any concept of an assigned Driver? → A: Derive "assigned" from `ShipmentEvent` history — a Shipment is a Driver's once they've registered at least one `OutForDelivery`/`DeliveryAttempted` event on it. No new field or assignment mechanism is introduced.
- Q: WarehouseStaff's permitted event types (`ReceivedAtBranch`, `DepartedFromBranch`, `InTransit`) don't exist in the current `ShipmentEventType` enum. Should this feature add them? → A: Yes — add all three as new `ShipmentEventType` members as part of this feature's scope, registerable through the existing generic register-event capability (module `007`), not a new endpoint.
- Q: Should the system lock an account after repeated failed login attempts, or is there no limit on retries? → A: Lock the account temporarily after repeated failed attempts (count/duration are a plan-level detail), then allow retry.
- Q: Can a BranchManager also provision new staff accounts, or is that exclusively a SuperAdmin capability? → A: Account provisioning stays SuperAdmin-only. BranchManager's "full access to all endpoints across all modules" describes the pre-existing operational modules, not this new administrative capability.
- Q: Should ApplicationUser's login email be the same value as the linked Employee's existing Email field, or can they differ? → A: For the four staff roles, ApplicationUser's email is sourced from (kept in sync with) its linked Employee.Email at provisioning time — not independently entered or edited — so the two never disagree. SuperAdmin, having no linked Employee, is the sole case where its email is set directly, since there is no Employee record to source it from.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Staff member logs in (Priority: P1)

A company staff member (already represented by an Employee record) provides their email and password
and, on success, receives a session cookie that identifies them and their role for every subsequent
request.

**Why this priority**: Nothing else in this feature has any meaning until a staff member can actually
authenticate — this is the foundation every other capability depends on.

**Independent Test**: Can be fully tested by attempting to log in with a valid, already-provisioned
staff account's email and password (provisioned via User Story 4) and confirming a session cookie is
issued; a wrong password or unknown email is rejected without revealing which one was wrong.

**Acceptance Scenarios**:

1. **Given** a staff member has a provisioned account linked to an active Employee, **When** they
   submit their correct email and password, **Then** they receive a session cookie and are
   recognized as authenticated with their role on every subsequent request.
2. **Given** a staff member submits an unknown email, or the correct email with a wrong password,
   **When** they attempt to log in, **Then** the attempt is rejected with a generic invalid-credentials
   message that does not reveal whether the email or the password was the problem.
3. **Given** a staff member's linked Employee record has been deactivated, **When** they attempt to
   log in with otherwise-correct credentials, **Then** the attempt is rejected.
4. **Given** a staff member has submitted several consecutive wrong passwords for the same account,
   **When** they reach the failed-attempt threshold, **Then** the account is temporarily locked and
   further login attempts are rejected even with the correct password until the lockout expires.

---

### User Story 2 - Staff member logs out (Priority: P2)

An authenticated staff member ends their session, after which their session cookie no longer grants
access to anything.

**Why this priority**: A meaningful, low-risk capability once login exists, but it depends on User
Story 1 being in place first.

**Independent Test**: Can be fully tested by logging in (via User Story 1), logging out, and then
confirming a request using the same cookie is treated as unauthenticated.

**Acceptance Scenarios**:

1. **Given** an authenticated staff member, **When** they log out, **Then** their session is
   invalidated server-side and the same cookie no longer grants access to any endpoint that requires
   authentication.
2. **Given** a staff member who is not currently authenticated, **When** they attempt to log out,
   **Then** the system handles it gracefully without error.

---

### User Story 3 - Access is restricted by role (Priority: P3)

Every endpoint across every module enforces the role-based permission rules: a staff member can only
do what their role allows, and anyone unauthenticated or authenticated with an insufficient role is
rejected.

**Why this priority**: This is the actual value of "authorization" — the enforcement itself — but it
depends on User Story 1 (a way to be authenticated as a given role) to be testable at all.

**Independent Test**: Can be fully tested by authenticating as each of the five roles in turn (via
User Story 1) and confirming, for a representative sample of endpoints per role, that permitted
actions succeed and everything outside that role's permission matrix (see Requirements) is rejected —
including fully unauthenticated requests to any endpoint other than the public tracking one.

**Acceptance Scenarios**:

1. **Given** a staff member authenticated as `BranchManager`, **When** they call any endpoint across
   any module — including creating/updating/deactivating Employees or Vehicles, confirming or
   converting Orders — **Then** the action is permitted.
2. **Given** a staff member authenticated as `Operator`, **When** they create or update an Order,
   confirm an Order, register a permitted `ShipmentEvent`, or read Customers/Branches/Shipments/their
   events, **Then** the action is permitted; **When** they attempt Employee management, Vehicle
   management, `DeliveryAttempted` events, or converting an Order, **Then** it is rejected.
3. **Given** a staff member authenticated as `Driver`, **When** they register a `ShipmentEvent`
   (including `DeliveryAttempted`) or read one of their own assigned Shipments, **Then** the action is
   permitted; **When** they attempt to read a Shipment not assigned to them, or access Orders,
   Customers, Branches, Employees, Vehicles, or Invoices, **Then** it is rejected.
4. **Given** a staff member authenticated as `WarehouseStaff`, **When** they register one of their
   permitted `ShipmentEvent` types or read Shipments/their event history, **Then** the action is
   permitted; **When** they attempt Orders, Customers, Employees, Vehicles, or Invoices, **Then** it is
   rejected.
5. **Given** no authentication is provided at all, **When** a request is made to any endpoint other
   than the public tracking endpoint, **Then** it is rejected.
6. **Given** a staff member authenticated as `SuperAdmin`, **When** they call any endpoint, **Then**
   the action is permitted, regardless of any `EmployeeRole`-based rule.

---

### User Story 4 - SuperAdmin provisions a staff account (Priority: P4)

A SuperAdmin creates a new staff login, linking it to an existing Employee record and assigning it the
role that lets that person authenticate and use the system going forward.

**Why this priority**: Every other story depends on at least one account existing, but this is ranked
by the value it delivers directly (administrative account provisioning) rather than by strict technical
ordering — the same convention already used elsewhere in this project's specs.

**Independent Test**: Can be fully tested by a SuperAdmin creating a new staff login referencing an
existing, active Employee, and confirming that Employee's staff member can subsequently complete User
Story 1 with the new credentials.

**Acceptance Scenarios**:

1. **Given** an existing, active Employee with no login yet, **When** a SuperAdmin provisions a
   staff account for them with an email and password, **Then** the account is created, linked to that
   Employee, and can be used to log in immediately.
2. **Given** an Employee that already has a linked account, **When** a SuperAdmin attempts to
   provision a second one for the same Employee, **Then** it is rejected.
3. **Given** an Employee that does not exist or is inactive, **When** a SuperAdmin attempts to
   provision an account for it, **Then** it is rejected.
4. **Given** a staff member authenticated as `BranchManager` (or any role other than `SuperAdmin`),
   **When** they attempt to provision a new staff account, **Then** it is rejected.

---

### User Story 5 - Public tracking stays open (Priority: P5)

Anyone with a tracking number — with no login at all — can still look up a shipment's public tracking
information, exactly as before this feature existed.

**Why this priority**: Lowest priority only because it's a "must not regress" guarantee rather than new
functionality — but it is an explicit, named requirement of this feature and must be verified
directly.

**Independent Test**: Can be fully tested by calling the public tracking endpoint with no session
cookie at all and confirming it succeeds exactly as it did before this feature.

**Acceptance Scenarios**:

1. **Given** no authentication of any kind, **When** the public tracking endpoint is called with a
   valid tracking number, **Then** the shipment's public tracking information is returned exactly as
   it was before this feature existed.

---

### Edge Cases

- What happens when a session cookie is presented after logout, or after it has expired? Treated as
  fully unauthenticated — same rejection as never having logged in.
- What happens when an Employee's role changes after their account was created (e.g., promoted from
  `Operator` to `BranchManager`)? Their access reflects their current role on every request going
  forward — not the role in effect when they logged in or when the account was created.
- What happens when a deactivated Employee's account is used to attempt any authenticated action, not
  just login? Rejected the same way an expired/invalid session is — deactivating the Employee revokes
  access even mid-session.
- What happens when a `SuperAdmin` account (not linked to any Employee) has its own active/inactive
  concept? `SuperAdmin` accounts are managed independently of the `Employee` table entirely — see
  Assumptions.
- What happens when an account is locked out due to repeated failed login attempts? Every login
  attempt against it is rejected — including one with the correct password — until the lockout
  expires; the account is not otherwise disabled and unlocks automatically on its own.

## Requirements *(mandatory)*

### Functional Requirements

#### Authentication

- **FR-001**: System MUST allow a staff member to authenticate with an email and password, issuing a
  session cookie on success.
- **FR-002**: System MUST reject an authentication attempt with an unknown email or an incorrect
  password using a single, generic message that does not reveal which one was wrong.
- **FR-002a**: System MUST temporarily lock an account after a threshold of consecutive failed login
  attempts against it, rejecting further attempts — including ones with the correct password — until
  the lockout expires on its own (exact attempt count and lockout duration are a plan-level detail).
- **FR-003**: System MUST reject an authentication attempt, even with correct credentials, when the
  account's linked Employee is not active.
- **FR-004**: System MUST allow an authenticated staff member to log out, invalidating their session
  server-side such that the same cookie no longer grants access afterward.
- **FR-005**: System MUST re-evaluate an Employee's current active status and role on every request,
  not only at login time — a deactivation or role change takes effect immediately, without requiring
  the affected user to log out and back in.
- **FR-006**: System MUST NOT provide password-reset-via-email, social login, or two-factor
  authentication in this feature.

#### Account provisioning

- **FR-007**: System MUST allow a `SuperAdmin` to provision a new staff login for an existing, active
  Employee that does not already have one, assigning it the role corresponding to that Employee's
  `EmployeeRole` and sourcing its login email from that Employee's existing `Email` field (not a
  separately-supplied value).
- **FR-008**: System MUST reject provisioning a second login for an Employee that already has one, and
  reject provisioning one for an Employee that doesn't exist or is inactive.
- **FR-009**: System MUST support a `SuperAdmin` role that is independent of the `Employee` table —
  not linked to any Employee record — for system-administration accounts.
- **FR-009a**: System MUST restrict account provisioning (User Story 4) to `SuperAdmin` — a
  `BranchManager`'s "full access to all endpoints across all modules" (FR-012) covers the pre-existing
  operational modules and does not extend to provisioning new staff logins.

#### Authorization

- **FR-010**: System MUST reject any request to any endpoint other than the public tracking endpoint
  when no valid authentication is presented.
- **FR-011**: System MUST grant a `SuperAdmin`-authenticated request access to every endpoint,
  regardless of any role-based rule that would otherwise apply.
- **FR-012**: System MUST grant a `BranchManager`-authenticated request access to every endpoint
  across every pre-existing operational module, company-wide (not scoped to a single Branch) —
  excluding account provisioning, which stays `SuperAdmin`-only (FR-009a).
- **FR-013**: System MUST restrict an `Operator`-authenticated request to: creating and updating
  Orders; confirming Orders; registering `ShipmentEvent`s other than `DeliveryAttempted`; and reading
  Customers, Branches, Shipments, and their events — rejecting Employee management, Vehicle
  management, Invoice access, converting Orders, and registering `DeliveryAttempted` events.
- **FR-014**: System MUST restrict a `Driver`-authenticated request to: registering any
  `ShipmentEvent` (including `DeliveryAttempted`); and reading only Shipments the Driver is
  "assigned" to — defined as any Shipment for which that Driver has registered at least one
  `OutForDelivery` or `DeliveryAttempted` `ShipmentEvent` (derived from existing event history, no new
  assignment field or mechanism) — rejecting Orders, Customers, Branches, Employee management, Vehicle
  management, and Invoice access, and rejecting reads of Shipments the Driver has no such event on.
- **FR-015**: System MUST restrict a `WarehouseStaff`-authenticated request to: registering
  `ShipmentEvent`s of the types `ReceivedAtBranch`, `DepartedFromBranch`, and `InTransit` — three new
  event types this feature adds to `ShipmentEventType` alongside the existing `OrderConverted`,
  `OutForDelivery`, and `DeliveryAttempted`, registerable through the same existing generic
  register-event capability (module `007`), not a new endpoint — and reading Shipments and their event
  history; rejecting Orders, Customers, Employee management, Vehicle management, and Invoice access.
- **FR-016**: System MUST leave the public tracking endpoint
  (`GET /api/shipments/tracking/{trackingNumber}`) reachable with no authentication at all, unchanged
  from its behavior before this feature.
- **FR-017**: System MUST deny access to any module or capability not explicitly granted to a given
  role in this specification's permission rules (least-privilege default) — this applies in particular
  to any existing or future module not named in a role's rule set above.

### Key Entities

- **ApplicationUser**: A staff login. A securely-hashed password, an assigned role (`BranchManager`,
  `Operator`, `Driver`, `WarehouseStaff`, or `SuperAdmin`), and a reference to the `Employee` it
  belongs to — required for the four staff roles, absent for `SuperAdmin` (see Assumptions). Its login
  email is sourced from, and stays in sync with, the linked `Employee.Email` for the four staff roles —
  it is never independently entered or edited for those; `SuperAdmin`, having no linked Employee, is
  the sole case where the email is set directly on the account itself.
- **Employee** *(existing entity, referenced)*: Already carries an `EmployeeRole`
  (`Operator`/`Driver`/`WarehouseStaff`/`BranchManager`) and an active/inactive status, both of which
  this feature relies on directly — the ApplicationUser's role mirrors its Employee's `EmployeeRole`,
  and login/every subsequent request is rejected once the Employee is inactive.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A staff member with valid, active credentials can log in and immediately begin using
  every capability their role permits, with zero capabilities outside their role ever succeeding.
- **SC-002**: 100% of requests to any endpoint other than the public tracking one are rejected when no
  valid session is presented.
- **SC-003**: Deactivating a staff member's linked Employee record revokes their access within the
  same request cycle — no stale session continues to work after deactivation.
- **SC-004**: The public tracking endpoint's behavior for an anonymous caller is unchanged before and
  after this feature ships.
- **SC-005**: A SuperAdmin can provision a working login for any active Employee without needing
  direct database access.
- **SC-006**: An account subjected to repeated wrong-password attempts becomes temporarily unusable
  for login — including with the correct password — until its lockout expires, with no manual
  intervention required to restore it.

## Assumptions

- **One role per account, mirrored from `Employee.EmployeeRole`**: the four staff-facing role names
  (`BranchManager`, `Operator`, `Driver`, `WarehouseStaff`) are exactly the four `EmployeeRole` enum
  members — an ApplicationUser's authorization role is derived from, and stays in sync with, its
  linked Employee's `EmployeeRole`, not independently assigned. `SuperAdmin` is the sole
  independently-assigned role, for accounts with no linked Employee.
- **No self-service registration**: consistent with "only internal company staff can authenticate" and
  "SuperAdmin... for system administration tasks such as seeding initial users," accounts are always
  created by a `SuperAdmin` (see User Story 4) — there is no public or self-service sign-up endpoint.
- **Least-privilege default for unnamed modules**: a role's access is limited to exactly what this
  specification's permission rules name for it; any module not explicitly mentioned for a role
  (including any future module) is denied by default rather than assumed open (formalized as FR-017).
- **`BranchManager` access is company-wide, not scoped to a single Branch**: the source description
  states "full access to all endpoints across all modules" without a Branch qualifier, so despite the
  role's name, this specification takes that literally rather than inferring branch-level scoping.
- **Concurrent sessions across devices/browsers are allowed**: nothing in the source description asks
  for single-session enforcement, and logging in again does not invalidate a staff member's other,
  still-active sessions — each login simply issues its own independent session cookie. Logging out,
  however, invalidates *all* of that account's active sessions, not only the one that logged out — true
  per-session (rather than per-account) revocation would need a server-side session store this feature
  does not introduce; see `research.md` Decision 18 for the mechanism and reasoning.
- **The Invoices module referenced in the permission rules does not exist yet in this system**:
  `BranchManager`'s "issue and cancel Invoices" and the other roles' "cannot access Invoice financial
  data" describe permission rules for a module that hasn't been built. This feature records those rules
  now (so they're already defined once Invoices exists) but does not implement any Invoice
  entity/endpoints itself — enforcing an Invoice-scoped rule is deferred until that module exists.
- **Standard ASP.NET Core Identity defaults apply** for password complexity requirements and session
  cookie expiration/sliding-expiration behavior, since none were specified.
- **`ReceivedAtBranch`/`DepartedFromBranch`/`InTransit` are added as plain new `ShipmentEventType`
  members, with no new business rules of their own beyond the role restriction**: unlike
  `OutForDelivery` (requires a Driver, transitions `Shipment.Status`) or `DeliveryAttempted` (gated on
  current status, creates a `DeliveryAttempt`), this feature does not add any transition-legality or
  child-entity behavior for the three new types — they are ordinary event types a `WarehouseStaff` (or
  any other permitted role) can register via the existing generic endpoint, carrying only the fields
  every `ShipmentEvent` already supports. Any such richer behavior for these types (e.g. required
  status transitions) is out of scope here and left to a future shipment-tracking feature.
- **Whether `Operator`/`BranchManager` can also register the three new event types, and whether
  `Driver` can register them too (beyond `OutForDelivery`/`DeliveryAttempted`), is a plan-level
  authorization-matrix detail**: the source description states each role's permissions independently;
  reconciling the full cross-role event-type matrix (which exact `ShipmentEventType` values each of
  the five roles may register) is worked out during `/speckit-plan`, not re-litigated here — the
  business intent already stated per role in this spec's User Story 3 and Requirements is authoritative.
