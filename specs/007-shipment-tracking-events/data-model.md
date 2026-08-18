# Data Model: Shipment Tracking Events

Extends `ShipmentEvent` (module `006`) additively and adds `DeliveryAttempt` (new). Also extends
`ShipmentStatus` (module `001`) with one new value and `ShipmentEventType` (module `006`) with two —
both are open, string-persisted enums designed for exactly this kind of extension.

## Entity: `ShipmentEvent` (existing, additive changes only)

`ShipmentTracker.Core/Entities/ShipmentEvent.cs`, table `ShipmentEvents`.

| Field | Type | Rules | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | *(unchanged)* | — |
| `ShipmentId` | `int` (FK, required) | *(unchanged)* | — |
| `Shipment` | `Shipment` (nav) | *(unchanged)* | — |
| `EventType` | `ShipmentEventType` (enum) | *(unchanged column; enum gains `OutForDelivery`, `DeliveryAttempted`)* | FR-011 |
| `StatusSnapshot` | `ShipmentStatus` (enum) | *(unchanged column; enum gains `OutForDelivery`)* | FR-011 |
| `OccurredAt` | `DateTime` (UTC) | *(unchanged)*; new rule: must not be in the future | — |
| `EmployeeId` | `int?` (FK, nullable, **new**) | Required and must reference an active Employee with `Role == Driver` when `EventType == OutForDelivery`; optional (active-only check, no role restriction) for every other event type | FR-003, FR-004 |
| `Employee` | `Employee?` (nav, **new**) | Forward-only, no reverse collection on `Employee.cs` | — |
| `LocationLabel` | `string?` (**new**) | Optional free text | FR-002 |
| `Notes` | `string?` (**new**) | Optional free text | FR-002 |
| `CreatedAt` | `DateTime` (UTC, **new**, required) | System insert timestamp, set by the service; distinct from `OccurredAt` | FR-002 |

## Entity: `DeliveryAttempt` (new)

`ShipmentTracker.Core/Entities/DeliveryAttempt.cs`, table `DeliveryAttempts`. Created only alongside a
`DeliveryAttempted`-type `ShipmentEvent`, never independently.

| Field | Type | Rules | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `ShipmentEventId` | `int` (FK, required, **unique**) | One-to-one with `ShipmentEvent`; only ever set for a `DeliveryAttempted` event | FR-006 |
| `ShipmentEvent` | `ShipmentEvent` (nav) | Forward-only, no reverse collection on `ShipmentEvent.cs` | — |
| `AttemptNumber` | `int` (required) | Computed by the service as `(count of prior DeliveryAttempt rows for the same Shipment) + 1`; never supplied by the caller | FR-007 |
| `FailureReason` | `DeliveryFailureReason` (enum, required) | One of `NoOneHome`, `WrongAddress`, `Refused`, `AccessDenied`, `Other` | FR-008 |
| `NextAttemptAt` | `DateTime?` (UTC) | When provided, must be strictly later than its `ShipmentEvent.OccurredAt` | FR-009 |

## Enums

`ShipmentTracker.Core/Enums/ShipmentStatus.cs` (existing, module `001`) gains one member:

```
Collected, InTransit, OutForDelivery, Delivered, Cancelled
```

`ShipmentTracker.Core/Enums/ShipmentEventType.cs` (existing, module `006`) gains two members:

```
OrderConverted, OutForDelivery, DeliveryAttempted
```

`ShipmentTracker.Core/Enums/DeliveryFailureReason.cs` (new):

```
NoOneHome, WrongAddress, Refused, AccessDenied, Other
```

All persisted as `string` (`HasConversion<string>()`), same convention as every other enum.

## Shipment status transitions (extends the existing `ShipmentTransitionValidator`)

```
Collected --------------> InTransit -------> Delivered   [terminal]
Collected --------------> Cancelled          [terminal]   (existing)
InTransit --------------> Delivered   [terminal]           (existing)
InTransit --------------> Cancelled   [terminal]           (existing)
InTransit --------------> OutForDelivery                   (new)
OutForDelivery ---------> Delivered   [terminal]            (new)
OutForDelivery ---------> Cancelled   [terminal]            (new)
OutForDelivery ---------> OutForDelivery (no-op, already legal via the existing same-status shortcut)
```

A `DeliveryAttempted` event never changes `Shipment.Status` — it requires the shipment's *current*
status to already equal `OutForDelivery` (a plain equality gate, not a transition — research.md
Decision 4) and leaves it unchanged.

## DTOs

`ShipmentTracker.Core/DTOs/ShipmentEvents/`:

| DTO | Used by | Fields |
|---|---|---|
| `RegisterEventDto` | `POST /api/shipments/{id}/events` | `EventType` (required; rejected if `DeliveryAttempted` or `OrderConverted`), `EmployeeId?`, `LocationLabel?`, `Notes?`, `OccurredAt` (required, not in the future) |
| `RegisterDeliveryAttemptDto` | `POST /api/shipments/{id}/events/delivery-attempt` | `EmployeeId?`, `LocationLabel?`, `Notes?`, `OccurredAt` (required, not in the future), `FailureReason` (required), `NextAttemptAt?` — no `EventType` (forced to `DeliveryAttempted` server-side, research.md Decision 10) |
| `ShipmentEventDto` | Response for both `POST` endpoints and `GET .../events` | `Id`, `ShipmentId`, `EventType`, `StatusSnapshot`, `EmployeeId?`, `LocationLabel?`, `Notes?`, `OccurredAt`, `CreatedAt`, `DeliveryAttempt?` (nested `DeliveryAttemptDetailDto`, populated only when `EventType == DeliveryAttempted`) — the **operational** shape, includes `EmployeeId` |
| `DeliveryAttemptDetailDto` | Nested inside `ShipmentEventDto` | `AttemptNumber`, `FailureReason`, `NextAttemptAt?` |
| `ShipmentTrackingDto` | Response for `GET /api/shipments/tracking/{trackingNumber}` | `TrackingNumber`, `Status`, `Recipient`, `CreatedAt`, `DeliveredAt?`, `Events` (`List<TrackingEventDto>`) |
| `TrackingEventDto` | Nested inside `ShipmentTrackingDto` | `EventType`, `StatusSnapshot`, `LocationLabel?`, `Notes?`, `OccurredAt`, `DeliveryAttempt?` (nested `DeliveryAttemptDetailDto`) — the **public-safe** shape; no `Id`, no `EmployeeId`, no `CreatedAt` (system-internal, not customer-relevant) |

`EventType`/`StatusSnapshot`/`FailureReason` carry `[JsonConverter(typeof(JsonStringEnumConverter))]`
per property, on both input and output DTOs (per the `006` lesson recorded in `CLAUDE.md` — check both
sides, not just the response shape).

## Validation rules

### Structural (FluentValidation, `ShipmentTracker.Services/Validators/ShipmentEvents/`)

| Validator | Rule | FR |
|---|---|---|
| `RegisterEventDtoValidator` | `EventType`: not null, `IsInEnum()`, and not equal to `DeliveryAttempted` or `OrderConverted` (research.md Decision 9) | FR-011 |
| | `OccurredAt`: not default, not later than `DateTime.UtcNow` | — |
| `RegisterDeliveryAttemptDtoValidator` | `OccurredAt`: not default, not later than `DateTime.UtcNow` | — |
| | `FailureReason`: not null, `IsInEnum()` | FR-008 |
| | `NextAttemptAt`, when provided: must be later than `OccurredAt` | FR-009 |

### Database-dependent (`ShipmentEventService`, shared private helper — research.md Decision 10)

| Rule | Detail | FR |
|---|---|---|
| Shipment exists | `_unitOfWork.ShipmentRepository.GetByIdAsync(shipmentId)`; `null` → 404 | — |
| Employee exists + active (when `EmployeeId` provided) | `_unitOfWork.EmployeeRepository.SingleOrDefaultAsync(x => x.Id == dto.EmployeeId)`; error if `null` or `!IsActive` | FR-004 |
| Employee required + must be Driver (`OutForDelivery` only) | Same lookup; additionally error if `EmployeeId` is `null`, or the employee's `Role != Driver` | FR-003 |
| Transition allowed (`RegisterEventAsync` only) | `IValidator<StatusTransitionContext>` (existing `ShipmentTransitionValidator`) with `CurrentStatus = shipment.Status`, `NewStatus` derived from `EventType` (`OutForDelivery` → `ShipmentStatus.OutForDelivery`) | FR-011 |
| Current status must be `OutForDelivery` (`RegisterDeliveryAttemptAsync` only) | Plain equality check, not routed through the transition validator (research.md Decision 4) | FR-005 |

All errors accumulate into one `FluentValidation.ValidationException` if any exist, thrown before any
write — same "no partial write" guarantee as every other module.

## New interfaces

```csharp
public interface IDeliveryAttemptRepository : IBaseRepository<DeliveryAttempt> { } // no extra methods

public interface IShipmentEventService
{
    Task<ShipmentEventDto?> RegisterEventAsync(int shipmentId, RegisterEventDto dto);           // null -> 404; throws ValidationException
    Task<ShipmentEventDto?> RegisterDeliveryAttemptAsync(int shipmentId, RegisterDeliveryAttemptDto dto); // null -> 404; throws ValidationException
    Task<IEnumerable<ShipmentEventDto>?> GetEventsByShipmentAsync(int shipmentId);               // null -> 404 (shipment not found); empty list if found but no events
    Task<ShipmentTrackingDto?> GetTrackingAsync(string trackingNumber);                          // null -> 404
}
```

`IUnitOfWork` gains `IDeliveryAttemptRepository DeliveryAttemptRepository { get; }`. `IShipmentRepository`,
`IShipmentEventRepository`, `IEmployeeRepository` (all pre-existing) are reused as-is.

## Flow: `RegisterEventAsync`

1. Load `shipment` by id; `null` → caller returns 404.
2. Run `RegisterEventDtoValidator` (structural); accumulate errors.
3. If `dto.EmployeeId` provided (required when `EventType == OutForDelivery`), look up the Employee;
   accumulate errors per the rules above.
4. Compute `newStatus`: `OutForDelivery` → `ShipmentStatus.OutForDelivery` (the only currently-legal
   value once `DeliveryAttempted`/`OrderConverted` are excluded).
5. Run the existing `ShipmentTransitionValidator` with `{ CurrentStatus: shipment.Status, NewStatus:
   newStatus }`; accumulate its error if invalid.
6. If any error accumulated, throw `ValidationException` — nothing written.
7. `shipment.Status = newStatus`; construct `ShipmentEvent` (`StatusSnapshot = newStatus`, `CreatedAt =
   DateTime.UtcNow`, plus the submitted fields).
8. `Update(shipment)`, `AddAsync(shipmentEvent)`, single `CommitAsync()` — atomic (same pattern as
   module `006`'s `ConvertToShipmentAsync`).
9. Return `_mapper.Map<ShipmentEventDto>(shipmentEvent)`.

## Flow: `RegisterDeliveryAttemptAsync`

1. Load `shipment` by id; `null` → caller returns 404.
2. Run `RegisterDeliveryAttemptDtoValidator` (structural); accumulate errors.
3. If `dto.EmployeeId` provided, look up and validate active (no Driver-role requirement here);
   accumulate errors.
4. If `shipment.Status != ShipmentStatus.OutForDelivery`, add a failure (FR-005).
5. If any error accumulated, throw `ValidationException` — nothing written.
6. Construct `shipmentEvent` (`EventType = DeliveryAttempted`, `StatusSnapshot = shipment.Status`
   (unchanged), `CreatedAt = DateTime.UtcNow`, plus submitted fields).
7. Compute `attemptNumber = await _unitOfWork.DeliveryAttemptRepository.CountAsync(x =>
   x.ShipmentEvent.ShipmentId == shipmentId) + 1`.
8. Construct `deliveryAttempt` (`ShipmentEvent = shipmentEvent` — navigation assignment, not a guessed
   FK — `AttemptNumber = attemptNumber`, `FailureReason`, `NextAttemptAt`).
9. `AddAsync(shipmentEvent)`, `AddAsync(deliveryAttempt)`, single `CommitAsync()` — `shipment.Status` is
   **not** modified, so no `Update(shipment)` call is needed here.
10. Return `_mapper.Map<ShipmentEventDto>(shipmentEvent)` with its nested `DeliveryAttempt` populated by
    hand from `deliveryAttempt`.

## Flow: `GetEventsByShipmentAsync` / `GetTrackingAsync`

- `GetEventsByShipmentAsync`: load shipment (404 if missing), load all its events
  (`_unitOfWork.ShipmentEventRepository.GetAsync(x => x.ShipmentId == shipmentId, orderBy: q =>
  q.OrderBy(x => x.OccurredAt))`), for each event whose `EventType == DeliveryAttempted` look up its
  `DeliveryAttempt` by `ShipmentEventId` and attach it, map to `ShipmentEventDto`. No pagination
  (research.md Decision 12).
- `GetTrackingAsync`: load shipment by `TrackingNumber` (404 if missing), same event-loading logic,
  mapped to the public-safe `TrackingEventDto` shape (no `EmployeeId`).

## Migration

A single new EF Core migration (`ExtendShipmentEventsAndAddDeliveryAttempts`, generated during
implementation), strictly additive:
- `ALTER TABLE ShipmentEvents ADD EmployeeId (nullable), LocationLabel (nullable), Notes (nullable),
  CreatedAt (required)` — no column dropped, renamed, or retyped.
- `CREATE TABLE DeliveryAttempts` with its FK (unique) to `ShipmentEvents` and its columns.
- FK `ShipmentEvents.EmployeeId → Employees.Id`, `Restrict`, nullable.
- No change to any other existing table.
