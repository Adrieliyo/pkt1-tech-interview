# Quickstart: Validating Shipment Tracking Events

Manual verification (Swagger/HTTP), consistent with this project's policy (no automated test
project).

## Prerequisites

- .NET 8.0 SDK, local SQL Server with the database migrated (see `CLAUDE.md`).
- Apply this feature's migration before testing:
  ```
  dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
  ```
- An existing `Shipment` that is currently `InTransit` (create one via `POST /api/shipment`, then
  `PATCH /api/shipment/{trackingNumber}/status` to move it to `InTransit` — see module `001`/`002`).
- An active `Employee` with `Role: "Driver"` (module `004`), and ideally a second active Employee with
  a different role (e.g. `Operator`) to test the role-rejection path.

## 1. Build and run

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)

### User Story 1 — Mark a shipment out for delivery (P1)

1. `POST /api/shipments/{id}/events` with `eventType: "OutForDelivery"` and a valid active Driver
   `employeeId`: `201`, the shipment's status becomes `OutForDelivery` (verify via
   `GET /api/shipment/{trackingNumber}`, module `001`, or the new tracking endpoint).
2. Repeat with no `employeeId`: `400`. Repeat with an `employeeId` belonging to a non-Driver (e.g.
   `Operator`): `400`. Repeat with an inactive or nonexistent `employeeId`: `400`.
3. Repeat with `locationLabel`/`notes` supplied: `201`, both stored exactly as given.
4. `POST` targeting a shipment that is `Delivered` or `Cancelled`: `400`.
5. `POST` again on the same now-`OutForDelivery` shipment: `201` — allowed, no rejection (Edge Case).

### User Story 2 — Log a failed delivery attempt (P2)

1. On the `OutForDelivery` shipment from US1, `POST /api/shipments/{id}/events/delivery-attempt` with
   `failureReason: "NoOneHome"`: `201`, response includes `deliveryAttempt.attemptNumber: 1`, and the
   shipment's status is still `OutForDelivery` afterward.
2. `POST` a second delivery-attempt on the same shipment, with no new out-for-delivery event in
   between: `201`, `deliveryAttempt.attemptNumber: 2`.
3. `POST` a delivery-attempt on a shipment that is `InTransit` (never marked out for delivery): `400`.
4. `POST` with no `failureReason`, or an unrecognized one: `400`.
5. `POST` with `nextAttemptAt` earlier than or equal to `occurredAt`: `400`.
6. `POST` with no `nextAttemptAt`: `201`, `deliveryAttempt.nextAttemptAt: null`.

### User Story 3 — View a shipment's public tracking timeline (P3)

1. `GET /api/shipments/tracking/{trackingNumber}` for the shipment used above: `200`, `events` array
   includes the out-for-delivery and delivery-attempt events with `locationLabel`/`notes`/failure
   detail — and **no** `employeeId` anywhere in the response (inspect the raw JSON to confirm the key
   is entirely absent, not just `null`).
2. `GET` for a shipment with no events recorded: `200`, `events: []`, not an error.
3. Compare the same shipment's `GET /api/shipments/{id}/events` (operational) response: confirm it
   **does** include `employeeId` for each event, unlike the tracking response.

### Additional edge cases

- `POST /api/shipments/{id}/events` with `eventType: "DeliveryAttempted"`: `400` (must use the
  dedicated endpoint).
- `POST /api/shipments/{id}/events` with `eventType: "OrderConverted"`: `400` (internal-only, owned by
  Order conversion).
- `POST` with `occurredAt` in the future: `400`.
- `GET /api/shipments/{id}/events` for a nonexistent shipment id: `404`.
- `GET /api/shipments/tracking/{trackingNumber}` for an unknown tracking number: `404`.

## 3. Verify nothing else changed

- `GET/POST/PATCH /api/shipment` (module `001`/`002`, singular route): unchanged behavior and response
  shape — no event timeline added there.
- `GET/POST/PUT/DELETE` on `/api/branches`, `/api/employees`, `/api/vehicles`, `/api/customers`,
  `/api/orders`: no behavior change.
- The 5 pre-existing seeded Shipments and any Shipments created via `POST /api/shipment` or Order
  conversion (module `006`) remain intact and still have no events unless explicitly added.

If every scenario in section 2 and the check in section 3 pass, the implementation satisfies SC-001
through SC-005 of `spec.md`.
