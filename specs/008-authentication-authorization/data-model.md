# Data Model: Authentication & Authorization

**Feature**: `008-authentication-authorization` | **Date**: 2026-08-18

## Entities

### ApplicationUser (new — `ShipmentTracker.Infrastructure/Identity/ApplicationUser.cs`)

Inherits `IdentityUser` (provides `Id` (`string`, GUID), `UserName`, `Email`, `PasswordHash`,
`LockoutEnd`, `AccessFailedCount`, etc. — all standard Identity columns, unchanged).

| Field | Type | Notes |
|---|---|---|
| `EmployeeId` | `int?` | FK to `Employee.Id`. Nullable — see Research Decision 2. Non-null for `Operator`/`Driver`/`WarehouseStaff`/`BranchManager` accounts; null for `SuperAdmin`. |
| `Employee` | `Employee?` | Forward navigation, `.Restrict` delete behavior (matches `ShipmentEvent.Employee`/`Vehicle.Branch` convention). |

Validation / business rules (enforced in `UserService`, not the DB):
- A non-`SuperAdmin` account MUST have `EmployeeId` set, and that `Employee` MUST be `IsActive`.
- One `ApplicationUser` per `Employee` — `EmployeeId` is unique among non-null values (enforced via a
  unique filtered index in `ApplicationUserConfiguration`, mirroring the `DeliveryAttempt.ShipmentEventId`
  unique-index precedent from module 007).
- `Email` is synced from `Employee.Email` for the four staff roles (spec.md FR-007) — `UserService`
  sets `ApplicationUser.Email`/`UserName` = `Employee.Email` at creation and whenever `Employee.Email`
  changes (via `EmployeeService.UpdateAsync`, see Research Decision — Integration section below).
  `SuperAdmin` is the sole exception with an independently entered `Email`.
- The account's role (a single `AspNetUserRoles` row) mirrors `Employee.Role` 1:1 at creation time
  (one-role-per-account, no multi-role accounts — matches spec.md Assumptions).

### ApplicationRole (new — `ShipmentTracker.Infrastructure/Identity/ApplicationRole.cs`)

Inherits `IdentityRole` with no additional fields. Five seeded rows: `BranchManager`, `Operator`,
`Driver`, `WarehouseStaff` (string-identical to the four `EmployeeRole` enum members — see
`ShipmentTracker.Core/Enums/EmployeeRole.cs`), plus `SuperAdmin` (not tied to any `EmployeeRole`).

### Employee (existing, module 004 — unchanged)

No new columns. Referenced by `ApplicationUser.EmployeeId`. `Employee.Role` (`EmployeeRole` enum:
`Operator`, `Driver`, `WarehouseStaff`, `BranchManager`) is the source of truth an `ApplicationUser`'s
Identity role is derived from at account-creation time.

## Relationships

```text
Employee (1) ────────< (0..1) ApplicationUser
   Branch (1) ──< (0..*) Employee            [existing, unchanged]

ApplicationUser (1) ──< (0..*) AspNetUserRoles >── (1) ApplicationRole   [standard Identity many-to-many, constrained to exactly one row per user by this feature's business rules]
```

## New Core DTOs (`ShipmentTracker.Core/DTOs/Auth/`)

### LoginDto (input)
| Field | Type | Rules |
|---|---|---|
| `Email` | `string` | Required, valid email format |
| `Password` | `string` | Required |

### UserSessionDto (output)
| Field | Type | Notes |
|---|---|---|
| `UserId` | `string` | `ApplicationUser.Id` |
| `Email` | `string` | |
| `EmployeeId` | `int?` | Null for `SuperAdmin` |
| `FullName` | `string?` | `Employee.FirstName + " " + Employee.LastName`, null for `SuperAdmin` |
| `Role` | `string` | The single Identity role name |
| `BranchId` | `int?` | `Employee.BranchId`, null for `SuperAdmin` |

### ChangePasswordDto (input) — **design reference only, not implemented in this feature's tasks (Research Decision 16)**
| Field | Type | Rules |
|---|---|---|
| `CurrentPassword` | `string` | Required |
| `NewPassword` | `string` | Required, same policy as account creation (8+ chars, 1 digit, 1 uppercase) |
| `ConfirmPassword` | `string` | Required, must equal `NewPassword` |

### CreateUserDto (input, for `SuperAdmin`-only provisioning, FR-002/FR-009a)
| Field | Type | Rules |
|---|---|---|
| `EmployeeId` | `int` | Required, must reference an existing, active `Employee` with no `ApplicationUser` yet |
| `Password` | `string` | Required, same policy as above |

No output-only inheritance is introduced (per this codebase's no-DTO-inheritance convention) — each
DTO above is flat and independent, matching every prior module.

## State / Lifecycle Notes

- **Account lockout** (spec.md FR-002a): `ApplicationUser.LockoutEnd`/`AccessFailedCount`, entirely
  managed by `SignInManager`/`UserManager`'s built-in lockout mechanics (Research Decision 11) — no
  custom state machine needed.
- **Account deactivation** (mirrors `Employee.IsActive = false` on `EmployeeService.DeactivateAsync`):
  `UserService.DeactivateUserAsync` sets `LockoutEnd = DateTimeOffset.MaxValue` (permanent lockout,
  distinct from the temporary failed-login lockout above — same field, unbounded value).
- **No `Employee` back-collection to `ApplicationUser`** — matches the established
  FK-to-never-hard-deleted-entity, unidirectional-navigation convention (`CLAUDE.md`); querying "does
  this Employee have an account" goes through `IUserService`/a filtered `ApplicationUser` query, not a
  new `Employee.ApplicationUser` navigation property.
