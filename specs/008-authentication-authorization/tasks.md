---

description: "Task list for Authentication & Authorization"
---

# Tasks: Authentication & Authorization

**Input**: Design documents from `/specs/008-authentication-authorization/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md (all present)

**Tests**: Not requested — this solution has no automated test project (established convention);
validation is manual via `quickstart.md` against the live API, same as every prior module.

**Organization**: Tasks are grouped by user story (US1-US5, priorities P1-P5 per spec.md) to enable
independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Maps to spec.md's US1 (Login), US2 (Logout), US3 (Role enforcement), US4 (Provisioning), US5 (Public tracking unaffected)
- File paths are exact, per `plan.md`'s Project Structure section

## Phase 1: Setup

**Purpose**: Add the one new dependency and the shared, story-independent building blocks (constants, DTOs) every later phase needs.

- [X] T001 Add `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (Version 8.0.*) package reference to `ShipmentTracker.Infrastructure/ShipmentTracker.Infrastructure.csproj` (Research Decision 13)
- [X] T002 [P] Create `ShipmentTracker.Core/Constants/Roles.cs` with `BranchManager`, `Operator`, `Driver`, `WarehouseStaff`, `SuperAdmin` string constants (Research Decision 1)
- [X] T003 [P] Create `ShipmentTracker.Core/DTOs/Auth/LoginDto.cs` (`Email`, `Password`)
- [X] T004 [P] Create `ShipmentTracker.Core/DTOs/Auth/UserSessionDto.cs` (`UserId`, `Email`, `EmployeeId`, `FullName`, `Role`, `BranchId`)
- [X] T005 [P] Create `ShipmentTracker.Core/DTOs/Auth/CreateUserDto.cs` (`EmployeeId`, `Password`)

**Checkpoint**: Package restored, shared constants/DTOs exist. No behavior yet.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Identity infrastructure, DbContext migration, cookie/authorization pipeline, and the
per-request session-validity mechanism. **No user story is testable until this phase is complete** —
every story requires the ability to authenticate at all.

- [X] T006 Create `ShipmentTracker.Infrastructure/Identity/ApplicationUser.cs` — `: IdentityUser`, `EmployeeId` (`int?`), `Employee` (`Employee?`) navigation (Research Decision 2)
- [X] T007 [P] Create `ShipmentTracker.Infrastructure/Identity/ApplicationRole.cs` — `: IdentityRole`, no extra fields
- [X] T008 Create `ShipmentTracker.Infrastructure/Identity/ApplicationUserConfiguration.cs` — `IEntityTypeConfiguration<ApplicationUser>`: unique filtered index on `EmployeeId` (`WHERE EmployeeId IS NOT NULL`), `HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict)` (depends on T006)
- [X] T009 Modify `ShipmentTracker.Infrastructure/Data/AppDbContext.cs`: change base class to `IdentityDbContext<ApplicationUser, ApplicationRole, string>`, apply `ApplicationUserConfiguration` in `OnModelCreating` (depends on T006, T007, T008)
- [X] T010 Create `ShipmentTracker.Infrastructure/Identity/ApplicationUserClaimsPrincipalFactory.cs` — overrides `GenerateClaimsAsync` to append an `"EmployeeId"` claim when `user.EmployeeId` is non-null (Research Decision 10) (depends on T006)
- [X] T011 Generate migration: `dotnet ef migrations add AddIdentityTables --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web`, then verify the generated `Up()` only adds new Identity tables + the `EmployeeId` FK column on `AspNetUsers` — no `DropColumn`/`AlterColumn`/`RenameColumn` against any existing table (Research Decision 12) (depends on T009)
- [X] T012 [P] Create `ShipmentTracker.Infrastructure/Data/Seed/IdentitySeeder.cs` — idempotent: creates the 5 roles (`Roles.cs` constants) if missing, creates one `SuperAdmin` user from `Seed:SuperAdminEmail`/`Seed:SuperAdminPassword` config if no user with that email exists yet (Research Decision 14)
- [X] T013 [P] Add `Seed` section placeholders to `ShipmentTracker.Web/appsettings.json` (`SuperAdminEmail`, `SuperAdminPassword` left empty — real values via `appsettings.Development.json`/environment variables per quickstart.md)
- [X] T014 Create `ShipmentTracker.Web/Authorization/SuperAdminAuthorizationHandler.cs` — `IAuthorizationHandler` that calls `context.Succeed(requirement)` for any `RolesAuthorizationRequirement` when the user is in the `SuperAdmin` role (Research Decision 6)
- [X] T015 Create `ShipmentTracker.Web/Authorization/EmployeeSessionValidator.cs` — static method usable as `CookieAuthenticationEvents.OnValidatePrincipal`: resolves `IUnitOfWork` from the request's `IServiceProvider`, for non-`SuperAdmin` principals loads the linked `Employee`, rejects the principal (`context.RejectPrincipal()` + `SignOutAsync`) if inactive, replaces the role claim via `context.ReplaceIssuedPrincipal(...)` if `Employee.Role` no longer matches the principal's current role claim (Research Decision 15) (depends on T006)
- [X] T016 Modify `ShipmentTracker.Web/Program.cs`: add `AddIdentity<ApplicationUser, ApplicationRole>(...)` (password/lockout options per Research Decision 11) `.AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders()`; register `ApplicationUserClaimsPrincipalFactory` via `.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()`; `Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.Zero)`; `ConfigureApplicationCookie(...)` (HttpOnly/Secure/SameSite/8h sliding expiration, `OnRedirectToLogin`→401, `OnRedirectToAccessDenied`→403, `OnValidatePrincipal = EmployeeSessionValidator.ValidateAsync`); `AddAuthorization()`; `AddSingleton<IAuthorizationHandler, SuperAdminAuthorizationHandler>()`; `app.UseAuthentication()` before `app.UseAuthorization()`; call `IdentitySeeder` once at startup (depends on T006-T015)
- [X] T017 Run `dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` and verify the app starts, the 5 roles exist in `AspNetRoles`, and the seeded `SuperAdmin` row exists in `AspNetUsers` (depends on T011, T016)

**Checkpoint**: A `SuperAdmin` can log in (mechanically) and the cookie pipeline is fully wired.
Foundation ready — user stories can now proceed.

---

## Phase 3: User Story 1 - Staff member logs in (Priority: P1) 🎯 MVP

**Goal**: A staff member (or, until US4 ships, the seeded SuperAdmin) can authenticate with email +
password and receive a session cookie recognized on subsequent requests.

**Independent Test**: `POST /api/auth/login` with the seeded SuperAdmin's credentials (Phase 2) →
`200` + `UserSessionDto`; wrong password → `401` generic message; 5 consecutive wrong attempts →
`423`; full staff-account coverage (Employee-linked accounts) becomes testable once US4 (Phase 6) adds
provisioning — noted explicitly in spec.md's own Independent Test description for this story.

### Implementation for User Story 1

- [X] T018 [US1] Create `ShipmentTracker.Services/Validators/Auth/LoginDtoValidator.cs` — `Email` required + valid format, `Password` required
- [X] T019 [US1] Create `ShipmentTracker.Web/Controllers/AuthController.cs` with `[Route("api/auth")]` and `POST login` action: validates `LoginDto` via `LoginDtoValidator` (400 on failure), calls `SignInManager<ApplicationUser>.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: true)`, maps result to `200 + UserSessionDto` / `401 {"message":"Invalid credentials."}` / `423 {"message":"Account locked..."}` (depends on T018)
- [X] T020 [US1] In `AuthController`, build `UserSessionDto` by hand (not AutoMapper, per output-only-mapping convention) from the signed-in `ApplicationUser` + its role (`UserManager.GetRolesAsync`) + linked `Employee` (`FullName`, `BranchId`, null for `SuperAdmin`) (depends on T019)
- [X] T021 [US1] `[AllowAnonymous]` on `AuthController.Login`; confirm every other action in the controller requires authentication by default (no attribute needed once `[Authorize]` is applied at a broader scope in T028)

**Checkpoint**: Login works end-to-end for the seeded SuperAdmin account. Test per quickstart.md
Scenario 2 steps 1-3.

---

## Phase 4: User Story 2 - Staff member logs out (Priority: P2)

**Goal**: An authenticated session can be ended server-side; the same cookie stops working afterward.

**Independent Test**: Log in (US1), call logout, then confirm a follow-up request with the same cookie
is treated as unauthenticated.

### Implementation for User Story 2

- [X] T022 [US2] Add `POST logout` action to `ShipmentTracker.Web/Controllers/AuthController.cs`: `[Authorize]`, calls `SignInManager.SignOutAsync()`, returns `204 No Content` (depends on T019) — **live-testing correction (research.md Decision 18)**: `SignOutAsync()` alone does not satisfy FR-004 with stateless Identity cookies (the same cookie replays successfully until its 8h expiry); added `UserManager.UpdateSecurityStampAsync(user)` before sign-out, and fixed `EmployeeSessionValidator` (T015) to compose with `ISecurityStampValidator` instead of silently replacing it — verified via a strict cookie-replay test (401 after logout, confirmed against the exact pre-logout cookie value)
- [X] T023 [US2] Add `GET me` action to `AuthController`: `[Authorize]`, returns `200 + UserSessionDto` for the current principal using the same mapping helper as T020 (depends on T020)

**Checkpoint**: Login → logout → `GET /api/auth/me` returns `401` on the stale cookie. Test per
quickstart.md Scenario 2 step 4.

---

## Phase 5: User Story 3 - Access is restricted by role (Priority: P3)

**Goal**: Every existing endpoint enforces the per-role matrix from `research.md` Decision 8; `SuperAdmin`
bypasses all of it; unauthenticated requests are rejected everywhere except the one public endpoint.

**Independent Test**: Authenticate as each of the 5 roles (once accounts exist — see note below) and
confirm, per endpoint, permitted actions succeed and everything outside the role's matrix is rejected;
confirm fully unauthenticated requests are rejected everywhere but the public tracking endpoint.
**Note**: full 4-staff-role coverage requires US4 (Phase 6) to provision non-SuperAdmin test accounts;
the `SuperAdmin`-bypass and unauthenticated-rejection halves of this story are independently testable
using only the Phase 2 seeded account.

### Implementation for User Story 3

- [X] T024 [P] [US3] Add `ShipmentTracker.Core/Enums/ShipmentEventType.cs` members `ReceivedAtBranch`, `DepartedFromBranch`, `InTransit` (spec.md Clarification 2 / FR-015 — no new business rules beyond role restriction, per spec.md Assumptions)
- [X] T025 [US3] Add `[Authorize(Roles = Roles.BranchManager)]` to all write actions and `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]` to read actions in `ShipmentTracker.Web/Controllers/CustomerController.cs` (research.md Decision 8 — corrects the plan input's Operator write over-grant)
- [X] T026 [P] [US3] Add `[Authorize(Roles = Roles.BranchManager)]` to write actions and `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]` to read actions in `ShipmentTracker.Web/Controllers/BranchController.cs`
- [X] T027 [P] [US3] Add `[Authorize(Roles = Roles.BranchManager)]` to every action in `ShipmentTracker.Web/Controllers/EmployeeController.cs`
- [X] T028 [P] [US3] Add `[Authorize(Roles = Roles.BranchManager)]` to every action in `ShipmentTracker.Web/Controllers/VehicleController.cs`
- [X] T029 [US3] In `ShipmentTracker.Web/Controllers/OrderController.cs`: `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]` on `CreateOrder`, `UpdateOrder`, `ConfirmOrder`, `GetOrders`, `GetOrderById`, `GetOrderByNumber`, `CancelOrder`; `[Authorize(Roles = Roles.BranchManager)]` on `ConvertOrder` (research.md Decision 8)
- [X] T030 [US3] In `ShipmentTracker.Web/Controllers/ShipmentController.cs`: `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.WarehouseStaff)]` on `GetShipments`, `GetShipmentByTrackingNumber`; `[Authorize(Roles = Roles.BranchManager)]` on `CreateShipment`, `UpdateStatus` (research.md Decision 8 — least-privilege default for the two endpoints unnamed in spec.md)
- [X] T031 [US3] Modify `ShipmentTracker.Core/Interfaces/Services/IShipmentEventService.cs`: add `string callerRole, int? callerEmployeeId` parameters to `RegisterEventAsync` and `GetEventsByShipmentAsync` signatures (research.md Decision 9)
- [X] T032 [US3] Modify `ShipmentTracker.Services/ShipmentEventService.cs`: `RegisterEventAsync` rejects (`InvalidOperationException`, matching the existing status-guard exception convention) when `callerRole == Roles.WarehouseStaff` and `dto.EventType` is not one of `ReceivedAtBranch`/`DepartedFromBranch`/`InTransit` (research.md Decisions 8/9) (depends on T024, T031)
- [X] T033 [US3] Modify `ShipmentTracker.Services/ShipmentEventService.cs`: `GetEventsByShipmentAsync` rejects (`InvalidOperationException`) when `callerRole == Roles.Driver` and the shipment has no `ShipmentEvent` with `EmployeeId == callerEmployeeId` and `EventType` in `{OutForDelivery, DeliveryAttempted}` (research.md Decision 4) (depends on T031)
- [X] T034 [US3] Modify `ShipmentTracker.Web/Controllers/ShipmentEventController.cs`: add `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator + "," + Roles.Driver + "," + Roles.WarehouseStaff)]` on `RegisterEvent` and `GetEventsByShipment`; `[Authorize(Roles = Roles.BranchManager + "," + Roles.Driver)]` on `RegisterDeliveryAttempt`; extract `User.FindFirstValue(ClaimTypes.Role)` and `User.FindFirstValue("EmployeeId")` and pass them into the two modified Service calls; catch the new `InvalidOperationException` cases and return `403 {"message": ...}` (depends on T032, T033)
- [X] T035 [US3] Confirm `[AllowAnonymous]` remains on `ShipmentEventController.GetTracking` — now REQUIRED explicitly (added a `FallbackPolicy` requiring authentication in Program.cs as a defense-in-depth correction beyond the original task scope, so any endpoint without an explicit `[Authorize]`/`[AllowAnonymous]` now denies by default instead of defaulting open; documented in research.md)
- [X] T036 [US3] Register `builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminAuthorizationHandler>()` is already covered by T016 — verify here by testing a `SuperAdmin` session against one endpoint from each controller touched in T025-T030, T034 (verification task, no new file) — deferred to the Phase 8 full quickstart run (T046), once US4 provides non-seed accounts too

**Checkpoint**: Role enforcement live across every existing controller. Test per quickstart.md Scenario 3 (steps 1-2, 4-9 fully; steps 3, 5-6 need US4 accounts).

---

## Phase 6: User Story 4 - SuperAdmin provisions a staff account (Priority: P4)

**Goal**: A `SuperAdmin` can create a working login for an existing, active Employee; no one else can.

**Independent Test**: As `SuperAdmin`, provision an account for an active Employee with no login yet;
confirm that Employee can then complete US1's login flow. Confirm a second provision attempt for the
same Employee is rejected, as is any attempt by a non-`SuperAdmin`.

### Implementation for User Story 4

- [X] T037 [P] [US4] Create `ShipmentTracker.Core/Interfaces/Services/IUserService.cs`: `Task<UserSessionDto> CreateUserForEmployeeAsync(CreateUserDto dto)`
- [X] T038 [US4] Create `ShipmentTracker.Services/Validators/Auth/CreateUserDtoValidator.cs`: `EmployeeId` required (`>0`), `Password` required + matches Identity's configured complexity (8+ chars, digit, uppercase)
- [X] T039 [US4] Create `ShipmentTracker.Services/UserService.cs`: `CreateUserForEmployeeAsync` — run `CreateUserDtoValidator` (structural), then DB-dependent checks: Employee exists and `IsActive` (`InvalidOperationException`), Employee has no existing `ApplicationUser` (`InvalidOperationException`); on success, create `ApplicationUser` with `Email`/`UserName` = `Employee.Email`, `EmployeeId` set, assign the Identity role matching `Employee.Role.ToString()`, return `UserSessionDto` (depends on T006, T037, T038) — **mid-implementation correction**: `ApplicationUser`/`ApplicationRole` moved from `Infrastructure` to `Core/Identity/` (research.md Decision 1 amendment) because `UserService` needs `UserManager<ApplicationUser>`, which `Services` cannot reference from `Infrastructure`; `Core.csproj`/`Services.csproj` each gained one lightweight Identity package (no EF Core, no ASP.NET Core web dependency); the `AddIdentityTables` migration was rolled back, removed, and regenerated cleanly against the corrected namespace
- [X] T040 [US4] Create `ShipmentTracker.Web/Controllers/UsersController.cs` with `[Route("api/users")]`, `POST` action: `[Authorize(Roles = Roles.SuperAdmin)]` explicitly (not relying on the bypass handler here, since this is the one endpoint that must reject `BranchManager` too — Research Decision 6's bypass only ever *adds* SuperAdmin access, it never narrows another role out, so an explicit, SuperAdmin-only role string is required for this specific endpoint per spec.md Clarification 4/FR-009a), validates via `IUserService.CreateUserForEmployeeAsync`, returns `201 + UserSessionDto` / `400` on validation or guard failure (depends on T039)
- [X] T041 [US4] Register `IUserService`/`UserService`, `IValidator<CreateUserDto>`/`CreateUserDtoValidator` in `ShipmentTracker.Web/Program.cs`'s DI section (depends on T039, T038)

**Checkpoint**: Full US1-US4 flow testable end-to-end: SuperAdmin provisions → new staff member logs in
→ role-restricted endpoints behave per their EmployeeRole. Test per quickstart.md Scenario 1, then
re-run Scenario 3's remaining steps with the newly provisioned accounts.

---

## Phase 7: User Story 5 - Public tracking stays open (Priority: P5)

**Goal**: Confirm the one pre-existing public endpoint is genuinely unaffected by every change above.

**Independent Test**: Call `GET /api/shipments/tracking/{trackingNumber}` with zero authentication and
confirm identical behavior to before this feature.

### Implementation for User Story 5

- [X] T042 [US5] Verification only: ran live against a real tracking number with zero auth headers at all → `200` with the unchanged `ShipmentTrackingDto` shape (no `employeeId` leaked); confirmed elsewhere in this phase that every other tested endpoint returns `401` with no cookie (`FallbackPolicy`, Decision 17)

**Checkpoint**: All 5 user stories independently verified. Full quickstart.md passes end-to-end.

---

## Phase 8: Polish & Cross-Cutting Concerns

- [X] T043 [P] Add/verify XML doc comments on `AuthController`, `UsersController` actions (constitution's HTTP-contract requirement — Swagger/OpenAPI doc comments on every public endpoint) — present by construction, written alongside each action in T019/T022/T023/T040
- [X] T044 [P] Update root `CLAUDE.md` with this module's new conventions (Identity project placement in `Core` not `Infrastructure`, `FallbackPolicy` deny-by-default, `SuperAdmin`-bypass-handler pattern, caller-context-into-Service pattern, `SecurityStampValidator` composition gotcha) — preserving all existing content
- [X] T045 Run `graphify update .` — 155 changed files detected (112 code, 43 docs); AST extraction completed (602 nodes/1398 edges from changed code files); semantic extraction dispatched to 2 parallel subagents for the 43 doc files
- [X] T046 Full run-through of `quickstart.md` Scenarios 1-4 against the live app — all confirmed live via curl: SuperAdmin login/bypass, staff login (lockout after 5 failed attempts, 423, correct password still rejected mid-lockout), logout (with a live-testing fix, Decision 18), `/me`, US4 provisioning (success, duplicate rejection, `BranchManager` correctly rejected with 403), role enforcement (Operator write-blocked on Customers, Driver blocked from Orders/Shipment-list, Driver `OutForDelivery` on the generic endpoint, Driver assignment-scoped event reads — 403 on an unassigned shipment, WarehouseStaff blocked from `OutForDelivery` but allowed `InTransit`), FR-005/SC-003 (deactivating an Employee mid-session revokes access on the very next request), public tracking endpoint fully open with zero auth headers, unauthenticated requests rejected everywhere else via the new `FallbackPolicy`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup. **Blocks every user story** — Identity pipeline, DbContext, migration, and the cookie-validation mechanism must exist before any authenticated request can succeed.
- **User Story 1 (Phase 3)**: Depends on Foundational only.
- **User Story 2 (Phase 4)**: Depends on Foundational + US1 (`AuthController` must exist — T022/T023 extend the same file T019 creates).
- **User Story 3 (Phase 5)**: Depends on Foundational only for its `SuperAdmin`/unauthenticated-rejection half; depends on US1 to be *tested* as any non-SuperAdmin role (needs a way to log in as one, which strictly needs US4 for a real Employee-linked account — but the code changes in Phase 5 have no compile-time dependency on US1/US2/US4 code, only a testing-order dependency).
- **User Story 4 (Phase 6)**: Depends on Foundational only for its code; needed before Phase 5's non-SuperAdmin scenarios can be *tested* end-to-end.
- **User Story 5 (Phase 7)**: Depends on Foundational + Phase 5 (verifies Phase 5 didn't regress the one endpoint that must stay untouched).
- **Polish (Phase 8)**: Depends on all prior phases.

### Suggested implementation order (differs slightly from strict priority order due to the testing dependency above)

Foundational → US1 → US4 → US2 → US3 → US5 → Polish. Building US4 before US3 means every role's test
account exists before Phase 5's authorization changes are verified, avoiding a re-test pass. Priority
order (P1-P5) remains the correct order for scoping an MVP cut, per spec.md.

### Parallel Opportunities

- T002-T005 (Phase 1) — different files, no dependencies.
- T007, T012, T013 (Phase 2) — different files.
- T024, T026, T027, T028 (Phase 5) — different controllers/files, no shared dependency.
- T037 (Phase 6) — independent of the Phase 5 tasks it's listed after.
- T043, T044 (Phase 8) — different files.

---

## Parallel Example: Phase 5 (User Story 3)

```bash
# Launch independent controller-authorization tasks together:
Task: "Add [Authorize] to BranchController.cs"
Task: "Add [Authorize] to EmployeeController.cs"
Task: "Add [Authorize] to VehicleController.cs"
# T024 (new ShipmentEventType members) can also run alongside these — different file, no shared dependency
```

---

## Implementation Strategy

### MVP First (User Story 1 only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational (blocks everything — includes the migration, so this phase alone
   already changes the database schema).
3. Complete Phase 3: User Story 1.
4. **STOP and VALIDATE**: quickstart.md Scenario 2, steps 1-3, using the seeded SuperAdmin account.

### Incremental Delivery

1. Setup + Foundational → SuperAdmin can authenticate mechanically, nothing else changed yet.
2. Add US1 → login works generically (MVP, testable with the seeded account).
3. Add US4 → real staff accounts can be provisioned; US1 becomes fully testable per spec.md's own
   Independent Test wording.
4. Add US2 → logout completes the session lifecycle.
5. Add US3 → the actual authorization enforcement across every controller; test with the accounts from
   step 3.
6. Add US5 → regression-verify the one public endpoint.
7. Polish.

## Notes

- [P] tasks touch different files with no dependency on an incomplete task.
- Every controller-authorization task (T025-T030, T034) cites the exact `research.md` Decision it
  implements — re-check that decision's rationale before deviating.
- No test project exists in this solution; validation is the `quickstart.md` scenarios run manually
  against the live API, matching every prior module in this codebase.
- Commit after each task or logical group, per repository convention.
