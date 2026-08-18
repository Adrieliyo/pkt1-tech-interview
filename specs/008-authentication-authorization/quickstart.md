# Quickstart: Authentication & Authorization

**Feature**: `008-authentication-authorization` | **Date**: 2026-08-18

## Prerequisites

```bash
dotnet build ShipmentTracker.sln
dotnet ef migrations add AddIdentityTables --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
```

Set the SuperAdmin seed credentials (Research Decision 14) before first run — in
`ShipmentTracker.Web/appsettings.Development.json` for local testing, or as environment variables
(`Seed__SuperAdminEmail`, `Seed__SuperAdminPassword`) for anything shared:

```json
{ "Seed": { "SuperAdminEmail": "superadmin@shipmenttracker.local", "SuperAdminPassword": "ChangeMe123!" } }
```

```bash
dotnet run --project ShipmentTracker.Web
```

Confirm the seed ran once (check logs or query `AspNetUsers` for the SuperAdmin row) and confirm the
five roles (`BranchManager`, `Operator`, `Driver`, `WarehouseStaff`, `SuperAdmin`) exist in
`AspNetRoles`.

## Scenario 1 — SuperAdmin login and account provisioning (US4)

1. `POST /api/auth/login` with the seeded SuperAdmin credentials → expect `200` + `UserSessionDto`
   with `role: "SuperAdmin"`, `employeeId: null`.
2. Using the returned session cookie, create an Employee via the existing
   `POST /api/employees` endpoint (now `BranchManager`-only — see step 4) if none exists yet, note its
   `id` and `role`.
3. `POST /api/users` with `{ "employeeId": <id>, "password": "Str0ngPass!" }` → expect `201` +
   `UserSessionDto` whose `role` matches the Employee's `EmployeeRole`.
4. Attempt the same `POST /api/users` call while authenticated as a `BranchManager` account instead →
   expect `403` (spec.md Clarification 4: provisioning is SuperAdmin-only, explicitly excluded from
   BranchManager's "full access").

## Scenario 2 — Login, lockout, logout (US1, US2)

1. `POST /api/auth/login` with a valid staff account's correct credentials → expect `200` +
   `UserSessionDto` with the account's `role`/`employeeId`/`branchId` populated from its `Employee`.
2. `POST /api/auth/login` with the wrong password 5 times in a row → the 5th (or 6th, per configured
   `MaxFailedAccessAttempts`) attempt returns `423 Locked` with a message naming the lockout expiry.
3. `POST /api/auth/login` with the correct password immediately after → still `423` until
   `DefaultLockoutTimeSpan` (15 min) elapses.
4. With a valid session, `POST /api/auth/logout` → expect `204`. Immediately after,
   `GET /api/auth/me` using the same (now-invalidated) cookie → expect `401`.

## Scenario 3 — Role-based access enforcement (US3)

Using four different logged-in sessions (one per staff role) plus one anonymous client:

1. Anonymous client calls `GET /api/shipments/tracking/TRK-20260818-0001` → expect `200` (public,
   unauthenticated — spec.md FR-016).
2. Anonymous client calls `GET /api/shipment` (list) → expect `401`.
3. `Operator` session calls `POST /api/customers/individual` → expect `403` (Operator is read-only on
   Customers per Research Decision 8 — corrects the plan input's over-grant).
4. `Operator` session calls `GET /api/customers` → expect `200`.
5. `Driver` session calls `POST /api/orders` → expect `403` (Driver has no Order access per spec.md).
6. `Driver` session calls `POST /api/shipments/{id}/events` with `eventType: "OutForDelivery"` and
   their own `employeeId` → expect `200`/`201` (Research Decision 5).
7. `WarehouseStaff` session calls the same endpoint with `eventType: "OutForDelivery"` → expect `403`
   (Research Decision 8: WarehouseStaff restricted to the three warehouse event types).
8. `Driver` A registers an `OutForDelivery` event on shipment X, then calls
   `GET /api/shipments/X/events` for a *different* shipment Y they have no event on → expect `403`
   (Research Decision 4 — assignment derived from ShipmentEvent history, no `AssignedDriverId` field).
9. `SuperAdmin` session calls every endpoint above (including `POST /api/employees`,
   `DELETE /api/vehicles/{id}`) → expect success on all of them regardless of the per-controller role
   list (Research Decision 6 — bypass handler).

## Scenario 4 — Public tracking stays open (US5)

1. Restart the app with no `Authorization` header/cookie at all.
2. `GET /api/shipments/tracking/{trackingNumber}` for a known shipment → expect `200` with the
   privacy-filtered `ShipmentTrackingDto` (module 007 shape, unchanged by this feature).
3. Every other endpoint from Scenario 3 step 2 pattern → expect `401`, confirming FR-010 ("reject any
   request to any endpoint other than the public tracking endpoint when no valid authentication is
   presented").

## Expected outcomes (traceable to spec.md Success Criteria)

- SC-001/SC-002 (login success/failure paths): Scenario 2 steps 1-2.
- SC-006 (lockout): Scenario 2 steps 2-3.
- SC-003/SC-004 (role enforcement, least-privilege default): Scenario 3.
- SC-005 (public tracking unaffected): Scenario 4.
