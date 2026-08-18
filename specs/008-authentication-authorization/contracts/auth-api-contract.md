# API Contract: Authentication & Authorization

**Feature**: `008-authentication-authorization` | **Date**: 2026-08-18

Cookie-based session auth. No JWT/bearer token — the `Set-Cookie` from `/api/auth/login` is sent
automatically by the browser/HTTP client on subsequent requests. All error bodies follow this
codebase's existing convention: FluentValidation failures → `400 {"errors": [...]}`;
guard/state failures → `400 {"message": "..."}` (module 001-007 precedent); this feature adds
`401`/`403`/`423` for authentication/authorization failures specifically.

## AuthController — `api/auth`

### `POST /api/auth/login`
`[AllowAnonymous]`

Request:
```json
{ "email": "driver1@shipmenttracker.local", "password": "Str0ngPass!" }
```

Responses:
- `200 OK` — sets session cookie, body: `UserSessionDto`
  ```json
  { "userId": "...", "email": "driver1@shipmenttracker.local", "employeeId": 12, "fullName": "Ana Perez", "role": "Driver", "branchId": 3 }
  ```
- `400` — `{"errors": [...]}` — malformed request (missing email/password, bad email format)
- `401` — `{"message": "Invalid credentials."}` — wrong email/password, or account has no linked active Employee
- `423 Locked` — `{"message": "Account locked. Try again after {lockoutEnd}."}` — too many failed attempts (spec.md FR-002a)

### `POST /api/auth/logout`
`[Authorize]` (any authenticated role)

- `204 No Content` — session cookie invalidated server-side
- `401` — no valid session presented

### `GET /api/auth/me`
`[Authorize]` (any authenticated role)

- `200 OK` — body: `UserSessionDto` for the current session
- `401` — no valid session presented

## UsersController — `api/users`

### `POST /api/users`
`[Authorize(Roles = "SuperAdmin")]` — explicitly SuperAdmin-only, `BranchManager` excluded (spec.md Clarification 4 / FR-009a)

Request: `CreateUserDto` — `{ "employeeId": 12, "password": "Str0ngPass!" }`

- `201 Created` — body: `UserSessionDto` for the newly created account
- `400` — `{"errors": [...]}` — validation failures (weak password, missing fields)
- `400` — `{"message": "..."}` — Employee not found / inactive / already has an account
- `403` — caller is `BranchManager` or any staff role (not `SuperAdmin`)

> `POST /api/users/{employeeId}/change-password` from the original technical brief is **not
> implemented** by this feature — it has no corresponding spec.md user story/FR (Research Decision 16).
> `ChangePasswordDto` remains documented in `data-model.md` for a possible future feature only.

## Authorization matrix applied to existing controllers

See `research.md` Decision 8 for the full table and rationale per row. Summary of enforcement points:

- Every controller action not listed as `[AllowAnonymous]` requires a valid session cookie; a missing
  or expired one returns `401` (via `ConfigureApplicationCookie`'s `OnRedirectToLogin` override).
- A valid session lacking the required role returns `403` (via `OnRedirectToAccessDenied`).
- `SuperAdmin` always succeeds regardless of the listed roles (Decision 6).
- Two endpoints apply an additional Service-layer, per-request data check beyond the role attribute:
  - `ShipmentEventController.RegisterEvent`: `WarehouseStaff` callers are further restricted to
    `EventType ∈ {ReceivedAtBranch, DepartedFromBranch, InTransit}` — violation returns
    `403 {"message": "WarehouseStaff can only register ReceivedAtBranch, DepartedFromBranch, or InTransit events."}`.
  - `ShipmentEventController.GetEventsByShipment`: `Driver` callers are further restricted to
    shipments they have an `OutForDelivery`/`DeliveryAttempted` event on — violation returns
    `403 {"message": "You are not assigned to this shipment."}`.
- `GET /api/shipments/tracking/{trackingNumber}` (module 007) is the sole `[AllowAnonymous]` endpoint
  besides login (spec.md FR-016).
