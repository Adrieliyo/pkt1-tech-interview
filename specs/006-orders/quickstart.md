# Quickstart: Validating the Orders Module

Manual verification (Swagger/HTTP), consistent with this project's policy (no automated test
project).

## Prerequisites

- .NET 8.0 SDK, local SQL Server with the database migrated (see `CLAUDE.md`).
- Apply this feature's migration before testing:
  ```
  dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
  ```
- At least one **active** Customer (`POST /api/customers/individual` or `/business`, module `005`).
- At least one **active** Branch (`POST /api/branches`, module `003`) for `DropOff` scenarios.

## 1. Build and run

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)

### User Story 1 — Create a new order (P1)

1. `POST /api/orders` with `pickupType: "DropOff"` and a valid active `originBranchId`: `201`,
   `status: "Pending"`, `orderNumber` matches `ORD-{today}-0001` (or the next sequential suffix).
2. `POST /api/orders` with `pickupType: "HomePickup"`, a `pickupAddress`, and a future
   `pickupScheduledAt`: `201`.
3. Create two orders on the same day; confirm their `orderNumber` suffixes are sequential.
4. `POST` with `pickupType: "DropOff"` and no `originBranchId`, or an inactive/nonexistent one: `400`.
5. `POST` with `pickupType: "HomePickup"` omitting `pickupAddress`/`pickupScheduledAt`, or with a
   `pickupScheduledAt` in the past: `400`.
6. `POST` with `pickupType: "DropOff"` that also supplies `pickupAddress`: `400`.
7. `POST` with a `customerId` that doesn't exist, or belongs to an inactive Customer: `400`.
8. `POST` with `declaredWeightKg: 0` or a negative dimension: `400`.

### User Story 2 — Find and review orders (P2)

1. Create several orders in different statuses (via US1 + US3/US5 below). `GET /api/orders` with no
   filters: all orders, most recent first.
2. `GET /api/orders?status=Pending`: only `Pending` orders.
3. `GET /api/orders?customerId={id}`: only that Customer's orders.
4. `GET /api/orders/{id}`: full detail including pickup/recipient/dimensions.
5. `GET /api/orders/number/{orderNumber}`: same detail, looked up by the human-readable number.
6. `GET /api/orders/999999`: `404`.

### User Story 3 — Confirm a pending order (P3)

1. `POST /api/orders/{id}/confirm` on a `Pending` order: `200`, `status: "Confirmed"`.
2. Repeat the same confirm call: `400` (no longer `Pending`).
3. `PUT /api/orders/{id}` on the now-`Confirmed` order: `400` (no longer editable).

### User Story 4 — Convert a confirmed order to a shipment (P4) — the central operation

1. `POST /api/orders/{id}/convert` on a `Confirmed` order: `200` with `{ shipmentId, trackingNumber }`,
   `trackingNumber` matches `TRK-{today}-XXXX`.
2. `GET /api/orders/{id}`: `status` is now `Converted`.
3. `GET /api/shipment/{trackingNumber}` (module `001`/`002` endpoint): the new Shipment exists,
   `status: "Collected"`, `recipient` matches the order's `recipientName`.
4. `POST /api/orders/{id}/convert` again on the same (now `Converted`) order: `400`, no second
   Shipment created.
5. `POST /api/orders/{otherId}/convert` on a `Pending` or `Cancelled` order: `400`.
6. Convert two different Confirmed orders on the same day; confirm their tracking-number suffixes are
   sequential and independent of the order-number sequence.

### User Story 5 — Cancel a pending order (P5)

1. `DELETE /api/orders/{id}` on a `Pending` order: `204`, `status` becomes `Cancelled`.
2. `DELETE /api/orders/{id}` again on the now-`Cancelled` order: `400` (not idempotent — `Cancelled`
   is terminal, unlike other modules' soft-delete `DELETE`).
3. `GET /api/orders/{id}`: the cancelled order's original detail is still fully returned.
4. `DELETE /api/orders/{confirmedId}` on a `Confirmed` order: `400`.

### User Story 6 — Update a pending order (P6)

1. `PUT /api/orders/{id}` on a `Pending` order, changing `declaredWeightKg` and `recipientAddress` to
   valid values: `200`, reflects the changes.
2. `PUT /api/orders/{id}` switching `pickupType` from `DropOff` to `HomePickup` (supplying
   `pickupAddress`/`pickupScheduledAt`, omitting `originBranchId`): `200`.
3. `PUT` with an inconsistent combination (e.g. `HomePickup` with no `pickupAddress`): `400`, order
   unchanged.
4. `PUT` on a `Confirmed`/`Converted`/`Cancelled` order: `400`, order unchanged.

### Additional edge cases

- `POST /api/orders` with `serviceType` omitted or an unrecognized value: `400`.
- `GET /api/orders?status=NotARealStatus`: `400`, not an empty list.
- `GET /api/orders?pageSize=1000`: does not fail; response has `X-Page-Size: 50`.
- Deactivate the Customer used by an existing order (`DELETE /api/customers/{id}`), then `GET
  /api/orders/{id}`: the order is still fully retrievable — active status is only checked at order
  creation/update time (spec.md Edge Cases), not on every read.

## 3. Verify nothing else changed

- `GET/POST/PATCH` on `/api/shipment`: unchanged behavior; a Shipment created directly via `POST
  /api/shipment` still works and has `orderId: null`.
- `GET/POST/PUT/DELETE` on `/api/branches`, `/api/employees`, `/api/vehicles`, `/api/customers`: no
  behavior change.
- The 5 pre-existing seeded Shipments (`TRK-90001`...`TRK-90005`) are still present and unaffected.

If every scenario in section 2 and the check in section 3 pass, the implementation satisfies SC-001
through SC-005 of `spec.md`.
