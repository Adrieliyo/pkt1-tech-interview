# HTTP Contract: Orders API

New module — no prior contract to preserve. The eight routes and their verbs are fixed by the user's
`/speckit-plan` input. List pagination reuses the contract established by
`002-paginate-shipment-list` (see
[`../../002-paginate-shipment-list/contracts/shipment-list-contract.md`](../../002-paginate-shipment-list/contracts/shipment-list-contract.md)).

## `POST /api/orders`

- **Body**: `CreateOrderDto`
- **201 Created**: `OrderDto` (`status: "Pending"`). Header `Location` pointing to `GET
  /api/orders/{id}`.
- **400 Bad Request**: missing/empty required field, invalid `serviceType`/`pickupType`, non-positive
  dimension/weight, negative `quotedPrice`, `HomePickup`/`DropOff` field-consistency violation
  (FR-002/FR-003), nonexistent/inactive Customer, nonexistent/inactive origin Branch. Body:

  ```json
  { "errors": [ { "property": "CustomerId", "message": "..." }, { "property": "PickupAddress", "message": "..." } ] }
  ```

## `GET /api/orders`

Paginated — same header contract as `GET /api/shipment`, `GET /api/branches`, `GET /api/employees`,
`GET /api/customers`.

| Param | Type | Default | Rules |
|---|---|---|---|
| `customerId` | `int` | — (no filter) | Optional |
| `status` | `OrderStatus` (enum name) | — (no filter) | Optional; unrecognized value → `400` |
| `page` | `int` | `1` | `>= 1`; non-numeric or `<= 0` → `400` |
| `pageSize` | `int` | `5` | `>= 1`; `> 50` is clamped to `50` (never rejected) |

- **200 OK**: `OrderDto[]` — the page matching the filters, ordered by `createdAt` descending. Headers
  `X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`.

## `GET /api/orders/{id}`

- **200 OK**: `OrderDto`, any status.
- **404 Not Found**: `{ "message": "No order was found with id '{id}'." }`

## `GET /api/orders/number/{orderNumber}`

- **200 OK**: `OrderDto`, same shape as `GET /api/orders/{id}`.
- **404 Not Found**: `{ "message": "No order was found with number '{orderNumber}'." }`

## `PUT /api/orders/{id}`

- **Body**: `UpdateOrderDto` (no `customerId` — ownership is fixed at creation, research.md Decision 13).
- **200 OK**: updated `OrderDto`.
- **404 Not Found**: same shape as `GET /api/orders/{id}`.
- **400 Bad Request**: same structural rules as `POST`, plus the order-not-found-inactive-reference
  checks. Body shape `{ errors: [...] }`.
- **400 Bad Request** (different failure category — status guard, not field validation): the order is
  not `Pending`. Body: `{ "message": "Only pending orders can be edited." }`

## `POST /api/orders/{id}/confirm`

- **200 OK**: updated `OrderDto` (`status: "Confirmed"`).
- **404 Not Found**: same shape as `GET /api/orders/{id}`.
- **400 Bad Request**: the order is not `Pending`. Body: `{ "message": "Only pending orders can be confirmed." }`

## `DELETE /api/orders/{id}`

- **Effect**: cancels the order — `status = Cancelled`. The row is never removed.
- **204 No Content**: success.
- **404 Not Found**: same shape as `GET /api/orders/{id}`.
- **400 Bad Request**: the order is not `Pending`. Body: `{ "message": "Only pending orders can be cancelled." }`
- Unlike other modules' `DELETE` (soft-deactivate, idempotent from any prior state), this `DELETE` is
  **not** idempotent from a non-`Pending` state — repeating it against an already-`Cancelled` order
  also returns `400`, since `Cancelled` is terminal and re-cancelling isn't a defined no-op (spec.md
  FR-013/FR-014).

## `POST /api/orders/{id}/convert`

The module's central operation.

- **200 OK**: `ConvertOrderResultDto`:

  ```json
  { "shipmentId": 6, "trackingNumber": "TRK-20260817-0001" }
  ```

- **404 Not Found**: same shape as `GET /api/orders/{id}`.
- **400 Bad Request**: the order is not `Confirmed`. Body: `{ "message": "Only confirmed orders can be converted to a shipment." }`
- On success: a new `Shipment` is created (`status: Collected`, `orderId` set), a `ShipmentEvent`
  (`eventType: OrderConverted`, `statusSnapshot: Collected`) is recorded for it, and the Order's
  `status` becomes `Converted` — all in one atomic write (research.md Decision 11). No partial state is
  ever observable: either all three changes exist, or none do.

## JSON format notes

- `serviceType`, `pickupType`, `status` on `OrderDto` are serialized and must be sent as the enum's
  **name** (e.g. `"Express"`, `"HomePickup"`, `"Pending"`), same convention as every other typed field
  in this system.
- `pickupScheduledAt` is a full `DateTime` (ISO 8601, e.g. `"2026-08-20T09:00:00Z"`), not a date-only
  value — unlike `Employee.hireDate`.
- `quotedPrice`, `declaredWeightKg`, `declaredWidthCm`, `declaredHeightCm`, `declaredLengthCm` are
  plain decimal numbers with no unit/currency embedded in the JSON (kilograms/centimeters/no stated
  currency, per spec.md Assumptions and research.md Decision 5).

## Examples

```
POST /api/orders
{
  "customerId": 1,
  "originBranchId": null,
  "serviceType": "Express",
  "pickupType": "HomePickup",
  "pickupAddress": "Av. Insurgentes 500",
  "pickupScheduledAt": "2026-08-20T09:00:00Z",
  "recipientName": "Carlos Ruiz",
  "recipientPhone": "+52-55-0707-0707",
  "recipientAddress": "Calle Norte 45",
  "recipientCity": "Guadalajara",
  "recipientState": "JAL",
  "recipientZipCode": "44100",
  "declaredWeightKg": 3.5,
  "declaredWidthCm": 20,
  "declaredHeightCm": 15,
  "declaredLengthCm": 30,
  "quotedPrice": 250.00,
  "notes": "Fragile"
}

201 Created
Location: /api/orders/1

{ "id": 1, "orderNumber": "ORD-20260817-0001", "customerId": 1, "originBranchId": null,
  "status": "Pending", "serviceType": "Express", "pickupType": "HomePickup", ... }
```

```
POST /api/orders/1/confirm
200 OK
{ "id": 1, "orderNumber": "ORD-20260817-0001", "status": "Confirmed", ... }

POST /api/orders/1/convert
200 OK
{ "shipmentId": 6, "trackingNumber": "TRK-20260817-0001" }

GET /api/orders/1
200 OK
{ "id": 1, "orderNumber": "ORD-20260817-0001", "status": "Converted", ... }
```
