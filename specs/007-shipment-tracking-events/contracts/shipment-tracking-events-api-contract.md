# HTTP Contract: Shipment Tracking Events API

New module — extends `ShipmentEvent` (module `006`) additively and adds `DeliveryAttempt`. All four
routes live on a new `ShipmentEventController` (`/api/shipments/...`), coexisting with the existing
`ShipmentController` (`/api/shipment/...`, singular, untouched — research.md Decision 5). No
pagination on any endpoint here (research.md Decision 12).

## `POST /api/shipments/{id}/events`

- **Body**: `RegisterEventDto`
- **201 Created**: `ShipmentEventDto`. Header `Location` pointing to `GET /api/shipments/{id}/events`.
- **404 Not Found**: `{ "message": "No shipment was found with id '{id}'." }`
- **400 Bad Request**: `eventType` missing/invalid/is `DeliveryAttempted` or `OrderConverted`,
  `occurredAt` missing or in the future, referenced `employeeId` doesn't exist or isn't active,
  `employeeId` missing or not a Driver when `eventType` is `OutForDelivery`, or the resulting status
  transition isn't legal from the shipment's current status. Body:

  ```json
  { "errors": [ { "property": "EmployeeId", "message": "..." } ] }
  ```

## `POST /api/shipments/{id}/events/delivery-attempt`

- **Body**: `RegisterDeliveryAttemptDto`
- **201 Created**: `ShipmentEventDto` with its nested `deliveryAttempt` populated. Header `Location`
  pointing to `GET /api/shipments/{id}/events`.
- **404 Not Found**: same shape as above.
- **400 Bad Request**: `failureReason` missing/invalid, `nextAttemptAt` not later than `occurredAt`,
  `occurredAt` missing or in the future, referenced `employeeId` (if any) doesn't exist or isn't
  active, or — most commonly — the shipment's current status is not `OutForDelivery`. Same error body
  shape as above.

## `GET /api/shipments/{id}/events`

- **200 OK**: `ShipmentEventDto[]`, ordered by `occurredAt` ascending, every field including
  `employeeId` (operational/staff view — not the public one). Empty array if the shipment exists but
  has no events yet.
- **404 Not Found**: same shape as above.

## `GET /api/shipments/tracking/{trackingNumber}`

The public tracking endpoint — no `employeeId` or other employee data anywhere in the response.

- **200 OK**: `ShipmentTrackingDto` — shipment summary plus `events: TrackingEventDto[]` (empty array
  if none recorded yet, not an error).
- **404 Not Found**: `{ "message": "No shipment was found with tracking number '{trackingNumber}'." }`

## JSON format notes

- `eventType`, `statusSnapshot`, `failureReason` are serialized as the enum's **name**
  (`"OutForDelivery"`, `"DeliveryAttempted"`, `"NoOneHome"`, ...), same convention as every other typed
  field in this system — applied to **both** the request and response DTOs (see `CLAUDE.md`'s
  "apply the converter to every DTO that carries the enum" note, added after the `006` bug).
- `occurredAt`, `createdAt`, `nextAttemptAt` are full `DateTime` values (ISO 8601 with time), not
  date-only.

## Examples

```
POST /api/shipments/6/events
{
  "eventType": "OutForDelivery",
  "employeeId": 3,
  "locationLabel": "Hub CDMX Norte",
  "occurredAt": "2026-08-18T08:00:00Z"
}

201 Created
Location: /api/shipments/6/events

{ "id": 1, "shipmentId": 6, "eventType": "OutForDelivery", "statusSnapshot": "OutForDelivery",
  "employeeId": 3, "locationLabel": "Hub CDMX Norte", "notes": null,
  "occurredAt": "2026-08-18T08:00:00Z", "createdAt": "2026-08-18T08:00:01.234Z",
  "deliveryAttempt": null }
```

```
POST /api/shipments/6/events/delivery-attempt
{
  "employeeId": 3,
  "locationLabel": "Colonia Doctores",
  "notes": "Gate locked, no answer",
  "occurredAt": "2026-08-18T14:30:00Z",
  "failureReason": "NoOneHome",
  "nextAttemptAt": "2026-08-19T10:00:00Z"
}

201 Created
{ "id": 2, "shipmentId": 6, "eventType": "DeliveryAttempted", "statusSnapshot": "OutForDelivery",
  "employeeId": 3, "locationLabel": "Colonia Doctores", "notes": "Gate locked, no answer",
  "occurredAt": "2026-08-18T14:30:00Z", "createdAt": "2026-08-18T14:30:01.001Z",
  "deliveryAttempt": { "attemptNumber": 1, "failureReason": "NoOneHome", "nextAttemptAt": "2026-08-19T10:00:00Z" } }
```

```
GET /api/shipments/tracking/TRK-20260818-0001

200 OK
{ "trackingNumber": "TRK-20260818-0001", "status": "OutForDelivery", "recipient": "Carlos Ruiz",
  "createdAt": "2026-08-18T02:52:43.13", "deliveredAt": null,
  "events": [
    { "eventType": "OutForDelivery", "statusSnapshot": "OutForDelivery", "locationLabel": "Hub CDMX Norte",
      "notes": null, "occurredAt": "2026-08-18T08:00:00Z", "deliveryAttempt": null },
    { "eventType": "DeliveryAttempted", "statusSnapshot": "OutForDelivery", "locationLabel": "Colonia Doctores",
      "notes": "Gate locked, no answer", "occurredAt": "2026-08-18T14:30:00Z",
      "deliveryAttempt": { "attemptNumber": 1, "failureReason": "NoOneHome", "nextAttemptAt": "2026-08-19T10:00:00Z" } }
  ]
}
```
