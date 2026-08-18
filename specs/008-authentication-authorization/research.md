# Research: Authentication & Authorization

**Feature**: `008-authentication-authorization` | **Date**: 2026-08-18

This document resolves every conflict found between the plan input (a generic ASP.NET Core Identity
technical brief) and (a) the real, current state of this codebase, and (b) `spec.md`'s clarified
decisions. Each entry follows Decision / Rationale / Alternatives considered.

## Decision 1: Identity classes live in `Core` and `Infrastructure`, not `Application`/`API`

The plan input's paths (`Infrastructure/Identity/...`, `Application/Constants/Roles.cs`,
`Application/DTOs/Auth/`, `Application/Validators/Auth/`, `Application/Services/`,
`API/Controllers/AuthController.cs`) describe a 5-6 project Clean Architecture layout
(`Domain/Application/Infrastructure/API`) that does not exist in this solution. The real solution has
exactly four projects: `ShipmentTracker.Core`, `ShipmentTracker.Infrastructure`,
`ShipmentTracker.Services`, `ShipmentTracker.Web`, with a strict one-directional dependency flow
enforced by the constitution (Principle II).

- **`ApplicationUser`/`ApplicationRole`** → `ShipmentTracker.Core/Identity/` — **corrected during
  implementation** (originally planned for `Infrastructure`, moved once `UserService`
  (`ShipmentTracker.Services`) needed to construct/manage them via `UserManager<ApplicationUser>` in
  `CreateUserForEmployeeAsync`, which `Services` cannot do if the class lives in `Infrastructure` —
  `Services` is only allowed to depend on `Core`, constitution Principle II). This is safe because
  `IdentityUser`/`IdentityRole` (from `Microsoft.Extensions.Identity.Stores`) and `UserManager<TUser>`/
  `RoleManager<TRole>` (from `Microsoft.Extensions.Identity.Core`) have **no EF Core dependency and no
  ASP.NET Core web/`HttpContext` dependency** — only `SignInManager<TUser>` and `IdentityDbContext<>`
  do, and both of those correctly stay out of `Core` (`SignInManager` used only in `AuthController`/
  `Web`; `IdentityDbContext<ApplicationUser, ApplicationRole, string>` used only by `AppDbContext`/
  `Infrastructure`, which already depends on `Core` and can reference the entities from there like any
  other entity). `Core.csproj` takes one new lightweight package
  (`Microsoft.Extensions.Identity.Stores`); `Services.csproj` takes one new lightweight package
  (`Microsoft.Extensions.Identity.Core`) for `UserManager`/`RoleManager` — neither pulls in EF Core or
  `Microsoft.AspNetCore.App`. `Infrastructure`'s `ApplicationUserConfiguration`
  (`IEntityTypeConfiguration<ApplicationUser>`) still lives in `Infrastructure` — genuinely EF-specific
  — configuring an entity defined in `Core`, exactly like every other entity in this codebase.
- **Role constants** → `ShipmentTracker.Core/Constants/Roles.cs`. Both `Services` (for the
  `SuperAdmin`-bypass authorization handler design, see Decision 6) and `Web` (for
  `[Authorize(Roles=...)]` attributes) need this string list, and `Core` is the only project both of
  them are already allowed to depend on.
- **Auth/session DTOs** (`LoginDto`, `UserSessionDto`, `ChangePasswordDto`) →
  `ShipmentTracker.Core/DTOs/Auth/`, following the same per-module DTO folder convention as every
  other module (`DTOs/Orders/`, `DTOs/ShipmentEvents/`, etc.).
- **`LoginDtoValidator`** → `ShipmentTracker.Services/Validators/Auth/`, matching the existing
  `Validators/<Module>/` convention.
- **`IUserService`** → `ShipmentTracker.Core/Interfaces/Services/`; **`UserService`** →
  `ShipmentTracker.Services/`. Same split as every existing service.
- **`AuthController`/`UsersController`** → `ShipmentTracker.Web/Controllers/`.

**Rationale**: keeps the existing, constitution-enforced dependency direction intact
(`Infrastructure → Core`, `Services → Core`, `Web → all three`) instead of introducing a 5th
conceptual layer that doesn't exist anywhere else in the solution.

**Alternatives considered**: creating a new `ShipmentTracker.Application` project to match the plan's
paths literally — rejected, this is a 4th-project-scale structural change the feature doesn't need
and the constitution's Principle IV (small, reversible changes) argues against adding a new project
just to match a generic template's naming.

## Decision 2: `ApplicationUser.EmployeeId`/`Employee` must be nullable

The plan input declares `public int EmployeeId { get; set; }` and `public Employee Employee { get; set; }`
(both non-nullable, implying every `ApplicationUser` has exactly one `Employee`). This directly
contradicts `spec.md`'s Key Entities section and Clarification 5, which state the `Employee` link is
"required for the four staff roles, **absent for `SuperAdmin`**" — `SuperAdmin` is explicitly not tied
to any `EmployeeRole` or `Employee` record (also stated verbatim in the original feature description).

**Decision**: `ApplicationUser.EmployeeId` is `int?`, `ApplicationUser.Employee` is `Employee?`. A
check constraint is not enforced at the DB level (out of scope, no precedent for DB-level CHECK
constraints anywhere in this codebase); instead `UserService` enforces "every non-SuperAdmin account
has exactly one linked, unique `Employee`" at creation time (mirrors the existing
`ValidateBusinessRulesAsync` DB-round-trip-validation convention).

**Rationale**: literal plan text is stale relative to the just-clarified spec; spec.md is the source
of truth per this session's established precedent (module 006/007 research docs resolved similar
plan/spec conflicts the same way).

**Alternatives considered**: giving `SuperAdmin` a synthetic/placeholder `Employee` row — rejected,
spec.md's Assumptions section is explicit that `SuperAdmin` is the sole exception with its own direct
`Email`, and inventing a fake `Employee` row would corrupt Employee-module reports/listings.

## Decision 3: No `ShipmentsController` exists — apply role mapping to the real controllers

The plan's authorization mapping repeatedly references a `ShipmentsController` with routes
`GET /api/shipments` (list) and `GET /api/shipments/{id}` (detail, with an `AssignedDriverId` runtime
check) plus `GET /api/shipments/tracking/{trackingNumber}` (public). **No such controller exists.**
The real routes, verified by reading all controllers directly, are split across two distinct,
already-shipped controllers:

- **`ShipmentController`** (module 001/002, route prefix `api/shipment`, **singular**): `GET`
  (paginated list, filter by `status`), `GET {trackingNumber}` (detail by tracking number — there is
  no get-by-id route), `POST` (create), `PATCH {trackingNumber}/status` (manual status update).
- **`ShipmentEventController`** (module 007, route prefix `api/shipments`, **plural**):
  `POST {id}/events`, `POST {id}/events/delivery-attempt`, `GET {id}/events`,
  `GET tracking/{trackingNumber}` (the actual public tracking endpoint named in spec.md FR-016 and in
  the plan's `[AllowAnonymous]` list).

**Decision**: the plan's intended role restrictions are mapped onto these two real controllers instead
of a new plural duplicate. Full mapping in Decision 8 below. The plan's `GET /api/shipments/{id}`
entry has no real equivalent (no get-by-id-only route exists for Shipment) and is dropped; its intent
(single-shipment detail with Driver assignment-scoping) is instead satisfied by
`ShipmentEventController.GetEventsByShipment`, which already returns per-shipment detail and is where
the Driver-scoping check is applied (Decision 4).

**Rationale**: the plan input is a generic template partially grounded in an idealized/different
version of this API; correcting it to the real, already-shipped route surface is required to produce
an implementable plan. Renaming/consolidating the singular and plural controllers is explicitly out of
scope — an unrelated refactor the constitution's Principle IV forbids bundling into this feature.

**Alternatives considered**: creating a new `GET /api/shipment/{id}` route to match the plan literally
— rejected as unrequested scope creep with no use case named in spec.md.

## Decision 4: Driver "assigned Shipments" is derived from `ShipmentEvent` history, not `AssignedDriverId`

The plan's `AssignedDriverId` runtime-check field directly contradicts spec.md's own Clarification 1
(resolved during `/speckit-specify`): *"Derive 'assigned' from `ShipmentEvent` history — a Shipment is
a Driver's once they've registered at least one `OutForDelivery`/`DeliveryAttempted` event on it. No
new field or assignment mechanism is introduced."*

**Decision**: no `AssignedDriverId` column is added anywhere. The Driver-scoping check is implemented
as a query: a Driver-authenticated caller may read a Shipment (via
`ShipmentEventController.GetEventsByShipment`) only if
`ShipmentEvents.Any(e => e.ShipmentId == id && e.EmployeeId == callerEmployeeId && (e.EventType == OutForDelivery || e.EventType == DeliveryAttempted))`
returns true; otherwise `403`. This reuses the existing `ShipmentEvent`/`DeliveryAttempt` schema from
module 007 with no new migration for this specific check.

**Rationale**: spec.md's clarification is unambiguous and was reached deliberately (a prior
`AskUserQuestion` interaction in this same session); the plan text predates it.

**Alternatives considered**: adding `AssignedDriverId` to `Shipment` as the plan describes — rejected,
directly contradicts an explicit, already-resolved spec clarification.

## Decision 5: Driver also gets access to the generic `POST {id}/events` endpoint

Spec.md FR-014 states a Driver-authenticated request may "register any ShipmentEvent (including
DeliveryAttempted)". The plan input lists `Driver` only under
`POST /api/shipments/{id}/events/delivery-attempt`, omitting it from the generic
`POST /api/shipments/{id}/events` endpoint's role list — meaning a Driver could log a failed delivery
attempt but not the natural preceding `OutForDelivery` event on the same shipment.

**Decision**: add `Driver` to `POST {id}/events`'s allowed roles (`BranchManager,Operator,Driver,WarehouseStaff`),
matching spec.md's literal "any ShipmentEvent" grant. The module-007 business rule that already
requires `OutForDelivery`'s linked `EmployeeId` to reference a `Driver`-role Employee
(`ShipmentEventService.ValidateEmployeeAsync(employeeId, requireDriver: true)`) is unchanged and keeps
acting as a second, independent layer of defense — an Operator-authenticated caller who is now also
allowed to *call* this endpoint still cannot successfully register an `OutForDelivery` event unless the
`EmployeeId` they supply resolves to a Driver.

**Rationale**: closes a gap the plan input left inconsistent with its own referenced spec requirement.

**Alternatives considered**: leaving Driver off this endpoint and requiring a BranchManager/Operator to
register `OutForDelivery` on the Driver's behalf — rejected, contradicts FR-014's literal wording and
removes the Driver's own agency to mark themselves out for delivery, which is the endpoint's evident
real-world purpose.

## Decision 6: `SuperAdmin` bypasses every role check via a custom `IAuthorizationHandler`, not manual per-controller listing

The plan input manually appends `SuperAdmin` to only two of its controller role lists
(`EmployeesController`, `VehiclesController`), omitting it from `CustomersController`,
`BranchesController`, `OrdersController`, `ShipmentEventsController`, and the (nonexistent)
`ShipmentsController`. Implemented literally, this **locks `SuperAdmin` out** of most of the API,
directly violating spec.md FR-011: *"grant a SuperAdmin-authenticated request access to every
endpoint, regardless of any role-based rule."*

**Decision**: register a custom `IAuthorizationHandler` (`SuperAdminAuthorizationHandler`, in
`ShipmentTracker.Web/Authorization/`, since it is ASP.NET Core-specific and Web is the only project
allowed to reference `Microsoft.AspNetCore.Authorization` types beyond what Identity itself needs)
that intercepts every `RolesAuthorizationRequirement` and calls `context.Succeed(requirement)` when the
authenticated user is in the `SuperAdmin` role — registered alongside (not replacing) the framework's
built-in `RolesAuthorizationHandler` via
`builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminAuthorizationHandler>()`. ASP.NET
Core's authorization evaluation succeeds a requirement if **any** registered handler calls `Succeed()`
for it, so this transparently satisfies every existing and future `[Authorize(Roles="...")]` attribute
without ever listing `SuperAdmin` explicitly in a controller. Per-controller role strings therefore
name only the real EmployeeRole-derived roles that need access; `SuperAdmin` is never written into
them.

**Rationale**: a single, centrally-tested bypass mechanism removes the exact class of inconsistency the
plan input itself demonstrates (silently forgetting to add `SuperAdmin` to five out of seven
controllers). It also satisfies spec.md FR-011 by construction — new endpoints added by future
features automatically grant `SuperAdmin` access with zero extra code, rather than depending on every
future author remembering to append the string.

**Alternatives considered**: manually including `SuperAdmin` in every `[Authorize(Roles=...)]` string
as the plan does — rejected, the plan input's own inconsistency is direct evidence this approach is
error-prone at this codebase's scale (7+ controllers today, more added every module).

## Decision 7: `InvoicesController`/`PaymentsController` are excluded entirely

The plan input's `InvoicesController` and `PaymentsController` sections describe endpoints
(`POST /api/invoices`, `POST /api/invoices/{id}/issue`, `DELETE /api/invoices/{id}`, a full
`PaymentsController`) for modules that **do not exist anywhere in this codebase** — no `Invoice` or
`Payment` entity, DTO, service, or controller. Spec.md's own Assumptions section states explicitly:
*"The Invoices module referenced in the permission rules does not exist yet in this system... this
feature does not implement any Invoice entity/endpoints itself."* `Payments` is not mentioned anywhere
in spec.md.

**Decision**: neither controller (nor any supporting Invoice/Payment entity/service) is created by this
feature. `BranchManager`'s "Can issue and cancel Invoices" capability (spec.md, User Story 4 /
Assumptions) is documented as a **future-module capability the role will have once Invoices ships**,
not something this feature builds.

**Rationale**: matches spec.md's explicit, already-agreed scope boundary; building these would be
large, unrequested scope creep and a constitution Principle IV violation (bundling unrelated feature
work into this change).

**Alternatives considered**: stubbing empty controllers now for forward-compatibility — rejected, no
current use case, and stub controllers with no backing entities would 404 or throw regardless, adding
dead code the constitution's minimalism principle argues against.

## Decision 8: Full endpoint-to-role authorization matrix (grounded in real routes)

Consolidating Decisions 3-7 plus a direct pass over spec.md's per-role bullets against every route
found by reading all seven controllers' source directly, and resolving several endpoints the plan
input left unaddressed via spec.md FR-017's stated least-privilege default (deny unless explicitly
granted):

| Controller | Route | Roles |
|---|---|---|
| `AuthController` (new) | `POST /api/auth/login` | `[AllowAnonymous]` |
| `AuthController` (new) | `POST /api/auth/logout` | any authenticated role |
| `AuthController` (new) | `GET /api/auth/me` | any authenticated role |
| `UsersController` (new) | `POST /api/users` (provision account, FR-007/FR-009a) | `SuperAdmin` only — explicitly excluded from `BranchManager`, per spec.md Clarification 4 |
| `CustomerController` | `POST individual`, `POST business`, `PUT {id}`, `DELETE {id}` | `BranchManager` (plan's `BranchManager,Operator` on writes over-grants — Operator is read-only on Customers per spec.md FR-013) |
| `CustomerController` | `GET`, `GET {id}` | `BranchManager,Operator` |
| `BranchController` | `POST`, `PUT {id}`, `DELETE {id}` | `BranchManager` |
| `BranchController` | `GET`, `GET {id}` | `BranchManager,Operator` |
| `EmployeeController` | all actions | `BranchManager` |
| `VehicleController` | all actions | `BranchManager` |
| `OrderController` | `POST`, `PUT {id}`, `POST {id}/confirm` | `BranchManager,Operator` |
| `OrderController` | `GET`, `GET {id}`, `GET number/{orderNumber}` | `BranchManager,Operator` (Operator's implicit read-access corollary — see note below) |
| `OrderController` | `DELETE {id}` (cancel) | `BranchManager,Operator` |
| `OrderController` | `POST {id}/convert` | `BranchManager` only — spec.md's Operator bullet grants confirm but not convert; only `BranchManager`'s bullet says "confirm **and convert**" |
| `ShipmentEventController` | `POST {id}/events` | `BranchManager,Operator,Driver,WarehouseStaff` (Decision 5); `WarehouseStaff` further restricted at the Service layer to `ReceivedAtBranch`/`DepartedFromBranch`/`InTransit` only (Decision 9) |
| `ShipmentEventController` | `POST {id}/events/delivery-attempt` | `BranchManager,Driver` |
| `ShipmentEventController` | `GET {id}/events` | `BranchManager,Operator,Driver,WarehouseStaff`; `Driver` further restricted at the Service layer to shipments they're assigned to (Decision 4) |
| `ShipmentEventController` | `GET tracking/{trackingNumber}` | `[AllowAnonymous]` — the one endpoint named in spec.md FR-016 |
| `ShipmentController` | `GET` (list), `GET {trackingNumber}` | `BranchManager,Operator,WarehouseStaff` — not `Driver` (see note below) |
| `ShipmentController` | `POST` (create), `PATCH {trackingNumber}/status` | `BranchManager` — least-privilege default (FR-017); neither endpoint is named in any role's spec.md bullet |

**Note on Operator + Orders read-access**: spec.md FR-013 explicitly grants Operator's read scope as
"Customers, Branches, Shipments, and their events" without separately naming Orders, while also
granting Operator create/update/confirm on Orders. A role that can create and confirm Orders but
cannot list or re-read the ones it just created would be unusable; this is treated as a spec
enumeration gap filled by the obvious corollary (matches this session's established precedent of
accepting a more-specific, sensible plan detail to refine an underspecified spec bullet, as done
previously in modules 006/007), not a contradiction requiring a spec amendment.

**Note on Driver + `ShipmentController`'s list/detail routes**: spec.md restricts Driver to reading
"only Shipments assigned to them." `ShipmentController.GetShipments`/`GetShipmentByTrackingNumber` are
company-wide, unfiltered-by-caller endpoints with no assignment-scoping mechanism, and adding one would
require modifying `ShipmentService` (a module owned by an earlier, already-shipped feature) beyond this
feature's minimal-touch principle. Driver's assignment-scoped shipment access is instead fully served
by `ShipmentEventController.GetEventsByShipment` (Decision 4), which already returns per-shipment
detail (including status) and already needs a per-request scoping check for this feature regardless.
Driver is therefore not added to `ShipmentController`'s roles.

## Decision 9: Caller-identity context flows from Controller to Service via explicit parameters, not `IHttpContextAccessor` in Services

Two new business rules require the Service layer to know *who* is calling, not just *whether* they're
authorized to hit the endpoint at all:
1. `ShipmentEventService.RegisterEventAsync` must reject a `WarehouseStaff`-authenticated caller trying
   to register an event type outside `{ReceivedAtBranch, DepartedFromBranch, InTransit}` (spec.md
   FR-015).
2. `ShipmentEventService.GetEventsByShipmentAsync` must reject a `Driver`-authenticated caller reading a
   shipment they're not assigned to, per the `ShipmentEvent`-history check in Decision 4.

This is the first time any Service method in this codebase needs caller-identity context — no prior
module had authentication at all.

**Decision**: `ShipmentTracker.Services` continues to have zero dependency on ASP.NET Core packages
(`Microsoft.AspNetCore.Http`, `IHttpContextAccessor`, etc.), consistent with the constitution's layer
boundaries. Instead, `ShipmentEventController` extracts the caller's role (`User.IsInRole(...)` /
`User.FindFirstValue(ClaimTypes.Role)`) and, for `Driver`/`WarehouseStaff` callers, their linked
`EmployeeId` (from a custom claim set at sign-in, see Decision 10) and passes these as **plain
parameters** into the existing Service method signatures (`RegisterEventAsync(shipmentId, dto,
callerRole, callerEmployeeId)`, `GetEventsByShipmentAsync(shipmentId, callerRole, callerEmployeeId)`).
This mirrors the codebase's established pattern of Controllers extracting request-scoped values
(`[FromQuery]`, `[FromRoute]`) and passing plain values down — the caller's role/EmployeeId are simply
another request-scoped input, sourced from the authentication cookie's claims instead of the query
string.

**Rationale**: keeps `Services` framework-agnostic and unit-testable without an ASP.NET Core test host,
matching the constitution's Principle II layer-integrity requirement.

**Alternatives considered**: injecting `IHttpContextAccessor` directly into `ShipmentEventService` —
rejected, this is the exact anti-pattern Clean Architecture layering forbids (a business-logic project
depending on a web-framework concern), and no other Service in this codebase does this.

## Decision 10: `EmployeeId` and `Role` are added as custom claims at sign-in

`SignInManager<ApplicationUser>.PasswordSignInAsync` by default only issues the standard Identity
claims (`NameIdentifier`, `Name`, role claims from `AddRoles`). Decision 9 needs the caller's linked
`EmployeeId` available on every authenticated request without a DB round-trip per request.

**Decision**: override `ApplicationUser`'s claim generation by supplying a custom
`IUserClaimsPrincipalFactory<ApplicationUser>` (`ApplicationUserClaimsPrincipalFactory`, in
`ShipmentTracker.Infrastructure/Identity/`) that appends a `"EmployeeId"` claim (the linked
`Employee.Id`, only when non-null — omitted entirely for `SuperAdmin`) to the principal generated at
sign-in. This makes `EmployeeId` available via `User.FindFirstValue("EmployeeId")` in any controller
for the lifetime of the cookie, refreshed automatically on next login (module 008 does not need
mid-session claim refresh — sessions are short-lived at 8 hours per the plan's cookie config, and an
Employee's role changing mid-session is out of this feature's scope per spec.md, matching the existing
"no auth" baseline's total absence of any session-invalidation-on-data-change mechanism).

**Rationale**: avoids a repository round-trip on every authorized request just to resolve
`ApplicationUser → Employee → Id`, and keeps the claims-based-authorization pattern idiomatic to
ASP.NET Core Identity rather than inventing a parallel lookup mechanism.

**Alternatives considered**: looking up `Employee` via `IUnitOfWork` inside the controller on every
request — rejected as an unnecessary extra DB round-trip per request when the claim is available for
free from the already-decrypted auth cookie.

## Decision 11: Cookie config, password policy, and lockout as specified — no changes needed

The plan input's `AddIdentity` options (password: 8 chars min, digit + uppercase required, no
non-alphanumeric requirement; lockout: 5 failed attempts, 15-minute lockout) and
`ConfigureApplicationCookie` options (HttpOnly, Secure, SameSite=Strict, 8-hour sliding expiration,
custom `OnRedirectToLogin`/`OnRedirectToAccessDenied` returning 401/403 JSON instead of HTML redirects)
match spec.md's User Story 1 (login, 4 scenarios including lockout) and Clarification 3 (account
lockout after repeated failed attempts) with no contradiction found. Adopted as-is.

**Rationale**: this is the one large section of the plan input that was already internally consistent
with the finalized spec; no correction needed. The `OnRedirectToLogin`/`OnRedirectToAccessDenied`
override is required specifically because this is a pure JSON API with no HTML login page — matches
the existing codebase-wide convention (every other module returns JSON error bodies on `400`, never
HTML).

## Decision 12: New migration `AddIdentityTables`, strictly additive

Per the plan input and this codebase's established "additive-only migrations" convention (documented
in `CLAUDE.md`, applied in module 007's `ExtendShipmentEventsAndAddDeliveryAttempts` migration): the
new migration adds only the standard `AspNetUsers`/`AspNetRoles`/`AspNetUserRoles`/etc. tables (via
`IdentityDbContext`) plus the `EmployeeId` FK column on `AspNetUsers` (added by `ApplicationUser`).
`Employee`'s own table is untouched — the FK direction is `AspNetUsers.EmployeeId → Employees.Id`
(`OnDelete(DeleteBehavior.Restrict)`, matching the established FK-to-never-hard-deleted-entity
convention already used for `Employee.BranchId`/`Vehicle.BranchId`/`ShipmentEvent.EmployeeId`), not the
reverse. No existing table's columns are dropped, altered, or renamed.

**Rationale**: consistent with the existing migration convention and the constitution's Principle IV
(small, additive, reversible changes).

## Decision 13: NuGet dependency — `Microsoft.AspNetCore.Identity.EntityFrameworkCore`

Confirmed via `ShipmentTracker.Infrastructure.csproj`: no Identity package is currently referenced
(only `Microsoft.EntityFrameworkCore`, `.Design`, `.SqlServer`, `.Tools`). This one package must be
added to `ShipmentTracker.Infrastructure.csproj`. It is justified per the constitution's Principle III
(minimalism) because ASP.NET Core Identity's EF Core store integration is the only viable option for
"cookie-based sessions backed by the existing SQL Server database with zero new dependencies beyond
what Identity itself requires" — there is no BCL/already-present-dependency way to get
password-hashing, lockout tracking, and role management without it.

## Decision 15: Per-request Employee active-status/role re-validation via a custom `OnValidatePrincipal` cookie event

Spec.md FR-005 requires an Employee's current active status and role to be re-evaluated on **every
request**, not just at login — a deactivation or role change must take effect immediately, without the
user logging out and back in (Edge Cases, SC-003). The plan input's `ConfigureApplicationCookie` block
did not address this at all. ASP.NET Core Identity's default `SecurityStampValidator` (auto-wired by
`AddIdentity` into the cookie's `OnValidatePrincipal` event, throttled by a 30-minute
`ValidationInterval` by default) only detects `ApplicationUser`-level changes (password/security-stamp
changes, lockout) — it has no awareness of the *linked* `Employee.IsActive`/`Employee.Role`, which are
domain-specific fields Identity knows nothing about.

**Decision**: two changes, both in `Program.cs`:
1. `services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.Zero);` —
   forces the validation callback to run on every request instead of every 30 minutes.
2. Set a custom `options.Events.OnValidatePrincipal` handler on the application cookie (after
   `AddIdentity`, so it overrides the default) that: (a) resolves the current `ApplicationUser` from the
   principal, (b) for non-`SuperAdmin` accounts, loads the linked `Employee` and rejects the principal
   (`context.RejectPrincipal(); await context.HttpContext.SignOutAsync(...)`) if the Employee is no
   longer active, and (c) if the Employee's `EmployeeRole` no longer matches the account's current
   Identity role (a promotion/demotion since account creation — also named in Edge Cases), replaces the
   principal's role claim with the current one via `context.ReplaceIssuedPrincipal` rather than the
   stale one baked into the existing cookie.

This logic lives in `ShipmentTracker.Web/Authorization/` (e.g.
`EmployeeSessionValidator.cs`, a static/DI-resolved delegate registered in `Program.cs`) since it
needs `IUnitOfWork` (via a scoped `IServiceProvider` resolved inside the event, the standard pattern
for DB access from cookie events) — an ASP.NET Core-specific concern that belongs in `Web`, consistent
with Decision 9's Services-stay-framework-agnostic rule.

**Rationale**: this is the only mechanism that actually satisfies FR-005/SC-003 as written — without
it, a deactivated Employee's already-issued cookie would keep working for up to 30 minutes (the
default) or indefinitely (if `SecurityStampValidator` were disabled outright), neither of which
matches "revokes access within the same request cycle."

**Alternatives considered**: checking `Employee.IsActive` inside every controller action instead —
rejected, would need to be duplicated across all 9+ controllers instead of one central enforcement
point, and is exactly the kind of per-endpoint duplication the codebase's established
shared-validation-helper convention (`ValidateBusinessRulesAsync` et al.) argues against. Increasing
rather than zeroing `ValidationInterval` (e.g. 1 minute) — rejected as not actually satisfying "same
request cycle" from spec.md, only an approximation of it.

## Decision 18: Logout requires an explicit `SecurityStamp` regeneration + composing (not replacing) `SecurityStampValidator`, discovered during live testing

Live-testing US2 (`/speckit-implement`, Phase 4) with a replayed pre-logout cookie surfaced two
compounding bugs, neither anticipated in the original design:

1. **`SignInManager.SignOutAsync()` alone does not satisfy FR-004.** ASP.NET Core Identity's cookie
   auth is stateless by default — the cookie *is* the session token; there is no server-side
   revocation list. `SignOutAsync()` only clears the cookie client-side (an expired `Set-Cookie`
   response); the exact same cookie value, if replayed (captured beforehand, or simply not honored by
   a non-browser client), remains fully valid until its 8-hour expiry. This contradicts FR-004's literal
   text ("the same cookie no longer grants access afterward") and User Story 2's acceptance scenario 1.
2. **Registering a custom `options.Events.OnValidatePrincipal` (Decision 15's `EmployeeSessionValidator`)
   silently replaces, rather than composes with, the `SecurityStampValidator` that `AddIdentity()` wires
   up by default.** Even after fixing bug 1 by regenerating the user's `SecurityStamp` on logout, the
   replayed cookie still succeeded — `EmployeeSessionValidator` only checked `Employee.IsActive`/`Role`,
   never the stamp itself, so nothing ever compared the cookie's embedded stamp against the now-changed
   one in `AspNetUsers`.

**Decision**: two changes. `AuthController.Logout` now calls
`await _userManager.UpdateSecurityStampAsync(user)` before `SignOutAsync()`. `EmployeeSessionValidator.ValidateAsync`
now resolves `ISecurityStampValidator` from `context.HttpContext.RequestServices` and calls
`await stampValidator.ValidateAsync(context)` **first**, checking `context.Principal == null` afterward
(that call internally invokes `RejectPrincipal()` on a stamp mismatch or active lockout) — only if the
stamp check passes does the method proceed to the `Employee.IsActive`/role checks. Combined with
`SecurityStampValidatorOptions.ValidationInterval = TimeSpan.Zero` (Decision 15, already in place for
FR-005), the regenerated stamp is caught on the very next request after logout, satisfying FR-004
exactly as it satisfies FR-005/SC-003 for deactivation.

**Side effect, accepted**: regenerating the `SecurityStamp` invalidates *every* outstanding session for
that user, not only the one that called logout — Identity's stamp mechanism is per-user, not
per-session; there is no per-session revocation without a server-side session store, which nothing in
this codebase or spec.md calls for. This is a narrow, judged departure from the letter of the
concurrent-sessions Assumption ("logging in again does not invalidate a staff member's other,
still-active sessions" — that sentence is specifically about *login*, and remains true unchanged; it
says nothing about logout). FR-004 is a mandatory, explicitly-tested functional requirement; the
concurrent-sessions behavior is a lower-authority inferred Assumption adopted only because "nothing in
the source asked for single-session enforcement" — given a genuine conflict, the explicit FR governs.

**Rationale**: verified live via a strict cookie-replay test (capture the exact pre-logout cookie value,
call logout with it, then resend that exact header) — first without this fix (200, confirming the bug),
then with it (401, confirming the fix), and re-verified the Decision 15 deactivation path still works
correctly through the same composed validator afterward.

**Alternatives considered**: building a per-session server-side revocation store (e.g. a table of
issued session/cookie identifiers with an explicit revoked flag) — rejected as disproportionate
infrastructure for a requirement fully satisfiable via Identity's existing stamp mechanism, and contrary
to the constitution's Principle III (dependency/infrastructure minimalism) absent a concrete need for
true per-session (not per-user) revocation that nothing in spec.md asks for.

## Decision 17: Global `FallbackPolicy` requiring authentication, discovered during implementation

While implementing (Phase 5, `/speckit-implement`), applying `[Authorize(Roles=...)]` individually to
every existing action satisfies FR-010 today, but leaves a latent gap: a bare `AddAuthorization()` call
does not deny-by-default — any *future* controller action added without an explicit
`[Authorize]`/`[AllowAnonymous]` attribute would silently default to anonymous access, which is exactly
the failure mode FR-017's least-privilege default is meant to prevent, and the kind of omission this
document's Decision 6 already flagged once for the `SuperAdmin` role string.

**Decision**: `Program.cs`'s `AddAuthorization()` call sets
`options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();`. This
policy only applies to endpoints that match no authorization attribute at all — every endpoint carrying
an explicit `[Authorize(Roles=...)]` still uses that attribute's policy, not the fallback, and every
endpoint explicitly marked `[AllowAnonymous]` (`POST /api/auth/login`,
`GET /api/shipments/tracking/{trackingNumber}`) still bypasses it. This required adding an explicit
`[AllowAnonymous]` to `ShipmentEventController.GetTracking`, which previously had no attribute at all
(correctly, since no auth middleware existed anywhere before this feature) — now required, since the
fallback would otherwise deny it.

**Rationale**: defense-in-depth matching FR-010's "reject any request to any endpoint other than the
public tracking endpoint" as an invariant of the whole API surface, not just of the endpoints that
happen to exist today.

**Alternatives considered**: relying solely on per-action `[Authorize]` attributes as originally
planned — rejected once implementation surfaced the same class of silent-omission risk Decision 6 was
written to prevent for `SuperAdmin`; the fallback costs one extra line in `Program.cs` and closes the
gap permanently rather than depending on every future author remembering the attribute.

## Decision 16: `change-password` endpoint dropped — not named anywhere in spec.md

The plan input's `UsersController.POST /api/users/{employeeId}/change-password` (with
`ChangePasswordDto`) has no corresponding user story, functional requirement, or success criterion
anywhere in `spec.md`. Unlike account provisioning (explicitly US4/FR-007-FR-009a) or the lockout
policy (explicitly Clarification 3/FR-002a/SC-006), self-service or admin-driven password change was
never named as in-scope. Spec.md only addresses password-reset-via-email as explicitly out of scope —
silent on a direct current-password-confirmed change entirely.

**Decision**: this endpoint (and its `ChangePasswordDto`) is dropped from this feature's task list.
`data-model.md`'s `ChangePasswordDto` entry and `contracts/auth-api-contract.md`'s corresponding
section are left as documented *design* reference (in case a future feature adds self-service password
change) but are not implemented as part of `008`'s tasks.

**Rationale**: matches this document's own Decision 7 precedent (Invoices/Payments excluded for the
same reason — present in the brief, absent from the finalized spec) and spec.md's FR-017 least-privilege
default, which denies any capability not explicitly named for a role. Implementing an unrequested
endpoint here would be exactly the scope creep Decision 7 already rejected once in this same plan.

**Alternatives considered**: restricting it to `SuperAdmin`-only (who bypasses all checks anyway,
Decision 6) — still rejected, since it would be shipping a whole new endpoint/DTO/validator with zero
traceability to any spec requirement, which the constitution's Principle IV (small changes, no
unrelated scope) also argues against.

## Decision 14: `SuperAdmin` seed values via configuration, not hardcoded

Per the plan input and spec.md's account-provisioning requirements: a `SuperAdmin` role and one default
`SuperAdmin` user are seeded idempotently on startup from `appsettings.json`/environment variables
(`Seed:SuperAdminEmail`, `Seed:SuperAdminPassword` — environment-variable override for the password in
any non-Development environment, since `appsettings.json` is checked into source control and a
plaintext seed password there would be a credential leak). Idempotency check: skip creation if a user
with that email already exists.

**Rationale**: matches the plan's explicit instruction ("never hardcoded") and this codebase's existing
`appsettings.json`-based configuration convention (e.g. `ConnectionStrings:DefaultConnection`).
