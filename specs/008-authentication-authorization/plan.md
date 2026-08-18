# Implementation Plan: Authentication & Authorization

**Branch**: `008-authentication-authorization` | **Date**: 2026-08-18 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/008-authentication-authorization/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Add cookie-based authentication (ASP.NET Core Identity) and role-based authorization to every existing
endpoint in the API, tying each account to the existing `Employee`/`EmployeeRole` model plus a new,
Employee-independent `SuperAdmin` role. The technical brief supplied with this plan described a 5-6
project Clean Architecture layout, a non-nullable `SuperAdmin` Employee link, a nonexistent
`ShipmentsController`, an `AssignedDriverId` field, and inconsistent `SuperAdmin` role listing — all
four contradict either this solution's real 4-project structure, its real controller routes, or
spec.md's already-clarified decisions. `research.md` resolves each conflict (14 numbered decisions);
this plan reflects the corrected design, not the literal brief.

## Technical Context

**Language/Version**: C# / .NET 8.0 (unchanged — matches every existing project in the solution)

**Primary Dependencies**: ASP.NET Core Identity (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`,
new — Research Decision 13), EF Core 8 (existing), AutoMapper (existing, output-only per convention —
not used for `ApplicationUser`, which has no DTO mapping needs beyond `UserSessionDto` built by hand in
`UserService`), FluentValidation (existing, for `LoginDto`/`ChangePasswordDto`/`CreateUserDto`
structural rules)

**Storage**: SQL Server via EF Core (existing `AppDbContext`, now `IdentityDbContext`-derived — one new
additive migration, `AddIdentityTables`)

**Testing**: No automated test project in this solution (existing convention) — validated manually via
`quickstart.md` scenarios against the live API, same as every prior module

**Target Platform**: ASP.NET Core Web API, self-hosted / IIS (unchanged)

**Project Type**: Web service (existing 4-project layered solution — no new project added)

**Performance Goals**: No new performance targets beyond the existing API's implicit expectations;
Identity's default password hashing (PBKDF2) and claims-based role checks add negligible per-request
overhead

**Constraints**: Cookie-based sessions only — no JWT, no OAuth2/external providers, no password-reset
email flow, no 2FA (all explicitly out of scope per spec.md); `Services` project must remain free of
ASP.NET Core package dependencies (Research Decision 9)

**Scale/Scope**: Touches all 7 existing controllers' authorization attributes plus 2 new controllers
(`AuthController`, `UsersController`); one new Identity-backed entity pair
(`ApplicationUser`/`ApplicationRole`); no changes to any existing entity, DTO, or business-logic method
signature beyond the two `ShipmentEventService` methods gaining caller-context parameters (Research
Decision 9)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I (single target framework)**: PASS. No new/changed `TargetFramework`; the one new NuGet
  package (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`) is `net8.0`-compatible.
- **Principle II (layer integrity)**: PASS. `ApplicationUser`/`ApplicationRole`/EF configuration live in
  `Infrastructure`; DTOs/interfaces/role constants live in `Core`; business logic (`UserService`,
  extended `ShipmentEventService`) lives in `Services`; controllers and the new
  `SuperAdminAuthorizationHandler` (an ASP.NET Core-specific concern) live in `Web`. No project gains a
  new disallowed dependency edge. `Services` explicitly stays free of `Microsoft.AspNetCore.*`
  references (Research Decision 9) — the one place the supplied brief would have violated this gate
  (an `IHttpContextAccessor` in a Service) was corrected before implementation.
- **Principle III (dependency minimalism)**: PASS, justified. One new package added
  (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`) — no BCL/already-present dependency can provide
  cookie auth + password hashing + lockout tracking + EF-backed role storage (Research Decision 13).
- **Principle IV (small, reversible changes)**: PASS. No existing entity's columns are dropped/altered;
  the new migration is strictly additive (Research Decision 12); the `Invoices`/`Payments` scope
  creep present in the supplied brief was explicitly excluded (Research Decision 7) to keep this change
  bounded to what spec.md actually asks for; no new project added despite the brief's differently-named
  folders implying one.

No violations requiring the Complexity Tracking table below.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Constants/
│   └── Roles.cs                          # NEW — BranchManager/Operator/Driver/WarehouseStaff/SuperAdmin
├── DTOs/Auth/                             # NEW
│   ├── LoginDto.cs
│   ├── UserSessionDto.cs
│   ├── ChangePasswordDto.cs               # design reference only, not implemented (Decision 16)
│   └── CreateUserDto.cs
├── Identity/                              # NEW — moved here from Infrastructure during implementation (Decision 1 correction)
│   ├── ApplicationUser.cs                 # : IdentityUser, EmployeeId (int?), Employee (Employee?)
│   └── ApplicationRole.cs                 # : IdentityRole
├── Interfaces/Services/
│   └── IUserService.cs                    # NEW
└── Entities/ (unchanged)

ShipmentTracker.Infrastructure/
├── Identity/                              # NEW
│   └── ApplicationUserClaimsPrincipalFactory.cs   # appends "EmployeeId" claim
├── Data/
│   ├── AppDbContext.cs                    # MODIFIED — now : IdentityDbContext<ApplicationUser, ApplicationRole, string>
│   ├── Configurations/
│   │   └── ApplicationUserConfiguration.cs  # NEW — unique filtered index on EmployeeId, FK Restrict
│   └── Seed/
│       └── IdentitySeeder.cs              # NEW — idempotent SuperAdmin role + user seed
└── Migrations/
    └── <timestamp>_AddIdentityTables.cs   # NEW — additive only

ShipmentTracker.Services/
├── UserService.cs                         # NEW
├── ShipmentEventService.cs                # MODIFIED — RegisterEventAsync/GetEventsByShipmentAsync gain callerRole/callerEmployeeId params
└── Validators/Auth/                       # NEW
    ├── LoginDtoValidator.cs
    ├── ChangePasswordDtoValidator.cs
    └── CreateUserDtoValidator.cs

ShipmentTracker.Web/
├── Authorization/
│   ├── SuperAdminAuthorizationHandler.cs  # NEW — IAuthorizationHandler bypass (Research Decision 6)
│   └── EmployeeSessionValidator.cs        # NEW — OnValidatePrincipal: per-request Employee active/role re-check (Research Decision 15)
├── Controllers/
│   ├── AuthController.cs                  # NEW — login/logout/me
│   ├── UsersController.cs                 # NEW — SuperAdmin-only account provisioning (change-password dropped, Research Decision 16)
│   ├── CustomerController.cs              # MODIFIED — [Authorize(Roles=...)] added per Decision 8
│   ├── BranchController.cs                # MODIFIED — same
│   ├── EmployeeController.cs              # MODIFIED — same
│   ├── VehicleController.cs               # MODIFIED — same
│   ├── OrderController.cs                 # MODIFIED — same
│   ├── ShipmentController.cs              # MODIFIED — same
│   └── ShipmentEventController.cs         # MODIFIED — [Authorize] + caller-context extraction for the 2 methods in Decision 9
└── Program.cs                             # MODIFIED — AddIdentity, ConfigureApplicationCookie, AddAuthorization, UseAuthentication/UseAuthorization, IdentitySeeder call, SuperAdminAuthorizationHandler DI registration
```

**Structure Decision**: reuses the existing 4-project layered structure unchanged
(`Core → Infrastructure/Services → Web`, per constitution Principle II) — no new project. Identity
infrastructure classes live in `Infrastructure` (co-located with `AppDbContext`/migrations, which they
extend); role constants, auth DTOs, and the `IUserService` interface live in `Core` (consumed by both
`Services` and `Web`); business logic and the new caller-context parameters live in `Services`;
controllers, `Program.cs` wiring, and the ASP.NET Core-specific `SuperAdminAuthorizationHandler` live in
`Web`, the sole project allowed to depend on all three others.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

No violations — table intentionally empty (see Constitution Check above, both pre- and post-design
passes).
