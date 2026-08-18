# Data Model: Orders Module

`Order` is a new, independent entity referencing `Customer` (module `005`, TPT root) and optionally
`Branch` (module `003`) by FK. `Shipment` (module `001`/`002`) gains one nullable column (`OrderId`,
research.md Decision 2). `ShipmentEvent` is a new entity, written only by this module's conversion
operation.

## Entity: `Order`

`ShipmentTracker.Core/Entities/Order.cs`, table `Orders`.

| Field | Type | Rules | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `OrderNumber` | `string` | System-generated at creation (`ORD-YYYYMMDD-XXXX`), unique, immutable | FR-004 |
| `CustomerId` | `int` (FK, required) | Must reference an existing, **active** Customer at creation time; not editable via update (research.md Decision 13) | FR-001 |
| `OriginBranchId` | `int?` (FK, nullable) | Required and must be an existing, active Branch when `PickupType == DropOff`; must be `null` when `PickupType == HomePickup` | FR-002 |
| `Status` | `OrderStatus` (enum) | `Pending` at creation; see State Transitions below | FR-006, FR-010–FR-018 |
| `ServiceType` | `ServiceType` (enum) | Required (`Standard`/`Express`/`Economy`, research.md Decision 5) | — |
| `PickupType` | `PickupType` (enum) | Required (`HomePickup`/`DropOff`) | FR-002, FR-003 |
| `PickupAddress` | `string?` | Required when `PickupType == HomePickup`; must be `null` when `DropOff` | FR-003 |
| `PickupScheduledAt` | `DateTime?` | Required and in the future when `PickupType == HomePickup`; must be `null` when `DropOff` | FR-003 |
| `RecipientName` | `string` | Required | FR-001 |
| `RecipientPhone` | `string` | Required | FR-001 |
| `RecipientAddress` | `string` | Required (street line) | FR-001 |
| `RecipientCity` | `string` | Required | FR-001 |
| `RecipientState` | `string` | Required, free text | FR-001 |
| `RecipientZipCode` | `string` | Required | FR-001 |
| `DeclaredWeightKg` | `decimal` | Required, `> 0` | FR-005 |
| `DeclaredWidthCm` | `decimal` | Required, `> 0` | FR-005 |
| `DeclaredHeightCm` | `decimal` | Required, `> 0` | FR-005 |
| `DeclaredLengthCm` | `decimal` | Required, `> 0` | FR-005 |
| `QuotedPrice` | `decimal` | Required, `>= 0` (research.md Decision 5) | — |
| `Notes` | `string?` | Optional free text | — |
| `CreatedAt` | `DateTime` (UTC) | Set by the service on create | — |
| `UpdatedAt` | `DateTime?` (UTC) | `null` until the first successful `PUT`; also set on `Confirm`/`Cancel`/`Convert` | — |

### State transitions (`OrderStatus`)

```
Pending --(confirm)--> Confirmed --(convert)--> Converted   [terminal]
Pending --(cancel)--> Cancelled                              [terminal]
```

`Pending` is the only status from which `update`, `confirm`, or `cancel` are allowed. `Confirmed` is
the only status from which `convert` is allowed. `Converted`/`Cancelled` accept no further transition
(FR-014).

## Entity: `Shipment` (existing, module `001`/`002`) — one new nullable column

| Field | Type | Rules | Note |
|---|---|---|---|
| `OrderId` | `int?` (FK, nullable) | `null` for Shipments created via the pre-existing `POST /api/shipment`; set to the originating Order's id for Shipments created via `ConvertToShipmentAsync`. Unique when non-null. | research.md Decision 2 |

No other `Shipment` field changes. `Recipient` (existing, required `string`) is populated from
`Order.RecipientName` during conversion — not a new field, an existing one being written by a new
caller.

## Entity: `ShipmentEvent` (new)

`ShipmentTracker.Core/Entities/ShipmentEvent.cs`, table `ShipmentEvents`. Written only by this
module's `ConvertToShipmentAsync`, for now.

| Field | Type | Rules | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `ShipmentId` | `int` (FK, required) | The Shipment this event belongs to; forward navigation only, no reverse collection on `Shipment.cs` (research.md Decision 12) | FR-017 |
| `EventType` | `ShipmentEventType` (enum) | `OrderConverted` for this module; modeled to extend in code without a DB schema change (research.md Decision 6) | FR-017 |
| `StatusSnapshot` | `ShipmentStatus` (existing enum, module `001`) | `Collected` for this module's only writer (research.md Decision 7) | FR-017 |
| `OccurredAt` | `DateTime` (UTC) | Set by the service at the moment of conversion | — |

## Enums

`ShipmentTracker.Core/Enums/OrderStatus.cs`: `Pending, Confirmed, Converted, Cancelled`

`ShipmentTracker.Core/Enums/ServiceType.cs`: `Standard, Express, Economy`

`ShipmentTracker.Core/Enums/PickupType.cs`: `HomePickup, DropOff`

`ShipmentTracker.Core/Enums/ShipmentEventType.cs`: `OrderConverted` (single member for now)

All four persisted as `string` (`HasConversion<string>()`), same convention as every existing enum.

## DTOs

`ShipmentTracker.Core/DTOs/Orders/`:

| DTO | Used by | Fields |
|---|---|---|
| `CreateOrderDto` | `POST /api/orders` | `CustomerId`, `OriginBranchId?`, `ServiceType?`, `PickupType?`, `PickupAddress?`, `PickupScheduledAt?`, `RecipientName`, `RecipientPhone`, `RecipientAddress`, `RecipientCity`, `RecipientState`, `RecipientZipCode`, `DeclaredWeightKg`, `DeclaredWidthCm`, `DeclaredHeightCm`, `DeclaredLengthCm`, `QuotedPrice`, `Notes?` — no `OrderNumber`/`Status` (system-assigned) |
| `UpdateOrderDto` | `PUT /api/orders/{id}` | Same as `CreateOrderDto` minus `CustomerId` (research.md Decision 13) |
| `OrderDto` | Response for every endpoint except `convert` | All `Order` fields including `Id`, `OrderNumber`, `Status`, `CreatedAt`, `UpdatedAt?` |
| `ConvertOrderResultDto` | Response for `POST /api/orders/{id}/convert` | `ShipmentId` (`int`), `TrackingNumber` (`string`) — per the plan input's step 7, not the full `OrderDto`/`ShipmentDto` |

`ServiceType`/`PickupType`/`Status` on `OrderDto` carry `[JsonConverter(typeof(JsonStringEnumConverter))]`
(per-property, same gotcha as every other module). `PickupScheduledAt` is a full `DateTime`, not
`DateOnly` — unlike `Employee.HireDate`, a pickup needs a time component, so it follows ISO 8601
`DateTime` JSON serialization (System.Text.Json default, no converter needed).

## Validation rules

### Structural (FluentValidation, `ShipmentTracker.Services/Validators/Orders/`)

| Validator | Rule | FR |
|---|---|---|
| `CreateOrderDtoValidator` / `UpdateOrderDtoValidator` | `RecipientName`, `RecipientPhone`, `RecipientAddress`, `RecipientCity`, `RecipientState`, `RecipientZipCode`: required | FR-001 |
| | `ServiceType`: not null, `IsInEnum()` | — |
| | `PickupType`: not null, `IsInEnum()` | FR-002, FR-003 |
| | `DeclaredWeightKg`, `DeclaredWidthCm`, `DeclaredHeightCm`, `DeclaredLengthCm`: each `> 0` | FR-005 |
| | `QuotedPrice`: `>= 0` | — |
| | When `PickupType == HomePickup`: `PickupAddress` not empty, `PickupScheduledAt` not null and in the future, `OriginBranchId` must be null | FR-003 |
| | When `PickupType == DropOff`: `OriginBranchId` not null, `PickupAddress` and `PickupScheduledAt` must both be null | FR-002 |

### Database-dependent (`OrderService`)

| Rule | Detail | FR |
|---|---|---|
| Customer exists and is active | `_unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.Id == dto.CustomerId)`; error if `null` or `!IsActive` | FR-001 |
| Branch exists and is active (`DropOff` only) | `_unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == dto.OriginBranchId)`; error if `null` or `!IsActive` | FR-002 |

Both checked on **create and update** (spec.md Assumptions), not on confirm/cancel/convert (Edge
Cases). Errors from both categories accumulate into one `FluentValidation.ValidationException` if any
exist, thrown before any write — same "no partial write" guarantee as every other module.

### Status-transition guards (`OrderService`, research.md Decision 10)

| Operation | Guard | Failure |
|---|---|---|
| `UpdateOrderAsync` | `order.Status == Pending` | `InvalidOperationException` → `400` |
| `ConfirmOrderAsync` | `order.Status == Pending` | `InvalidOperationException` → `400` |
| `CancelOrderAsync` | `order.Status == Pending` | `InvalidOperationException` → `400` |
| `ConvertToShipmentAsync` | `order.Status == Confirmed` | `InvalidOperationException` → `400` |

## New interfaces

`ShipmentTracker.Core/Interfaces/`:

```csharp
public interface IOrderRepository : IBaseRepository<Order> { }          // no extra methods
public interface IShipmentEventRepository : IBaseRepository<ShipmentEvent> { } // no extra methods

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);                       // throws ValidationException
    Task<PagedResult<OrderDto>> GetOrdersAsync(int? customerId = null, OrderStatus? status = null, int page = 1, int pageSize = 5);
    Task<OrderDto?> GetOrderByIdAsync(int id);                                 // null -> 404
    Task<OrderDto?> GetOrderByNumberAsync(string orderNumber);                 // null -> 404
    Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto);              // null -> 404; throws ValidationException / InvalidOperationException
    Task<OrderDto?> ConfirmOrderAsync(int id);                                 // null -> 404; throws InvalidOperationException
    Task<bool> CancelOrderAsync(int id);                                       // false -> 404; throws InvalidOperationException
    Task<ConvertOrderResultDto?> ConvertToShipmentAsync(int id);               // null -> 404; throws InvalidOperationException
}
```

`IUnitOfWork` gains `IOrderRepository OrderRepository { get; }` and `IShipmentEventRepository
ShipmentEventRepository { get; }`, same lazy-property pattern as every other repository.
`IShipmentRepository`/`ShipmentRepository` (already existing) are reused as-is — no changes needed
there beyond what `AppDbContext`'s configuration change already covers.

## Flow: `CreateOrderAsync`

1. Trim recipient string fields.
2. Run `CreateOrderDtoValidator` (structural, incl. `HomePickup`/`DropOff` conditional rules).
3. Check Customer active + (if `DropOff`) Branch active; accumulate errors with step 2's.
4. If any error, throw `ValidationException` — nothing written.
5. Generate `OrderNumber`: count `Order` rows with `CreatedAt` in `[todayUtc, tomorrowUtc)`, `+1`,
   format `ORD-{yyyyMMdd}-{count:D4}`.
6. Construct `Order` by hand (`Status = Pending`, `CreatedAt = DateTime.UtcNow`, `UpdatedAt = null`),
   `AddAsync` + `CommitAsync`.
7. Return `_mapper.Map<OrderDto>(order)`.

## Flow: `UpdateOrderAsync`

1. Load order by id; `null` → caller returns 404.
2. If `order.Status != Pending`, throw `InvalidOperationException` ("Only pending orders can be
   edited.").
3. Trim fields, run `UpdateOrderDtoValidator`, check Customer/Branch active (same as create, excluding
   `CustomerId` which isn't part of the DTO).
4. If any error, throw `ValidationException` — entity left unchanged.
5. Apply all editable fields, `UpdatedAt = DateTime.UtcNow`, `Update()` + `CommitAsync()`.
6. Return `_mapper.Map<OrderDto>(order)`.

## Flow: `ConfirmOrderAsync` / `CancelOrderAsync`

1. Load order by id; `null`/`false` → caller returns 404.
2. If `order.Status != Pending`, throw `InvalidOperationException`.
3. Set `Status = Confirmed` (or `Cancelled`), `UpdatedAt = DateTime.UtcNow`, `Update()` +
   `CommitAsync()`.
4. Return the mapped `OrderDto` (confirm) / `true` (cancel).

## Flow: `ConvertToShipmentAsync` (the central operation — plan input steps 1-7)

1. Load `order` by id; `null` → caller returns 404.
2. If `order.Status != Confirmed`, throw `InvalidOperationException` — nothing written.
3. Generate tracking number: count `Shipment` rows with `CreatedAt` in `[todayUtc, tomorrowUtc)`,
   `+1`, format `TRK-{yyyyMMdd}-{count:D4}`.
4. Construct `shipment = new Shipment { TrackingNumber, Recipient = order.RecipientName, Status =
   ShipmentStatus.Collected, CreatedAt = DateTime.UtcNow, OrderId = order.Id }`.
5. Construct `shipmentEvent = new ShipmentEvent { Shipment = shipment, EventType =
   ShipmentEventType.OrderConverted, StatusSnapshot = ShipmentStatus.Collected, OccurredAt =
   DateTime.UtcNow }` — assigning the `Shipment` navigation, not a manually-guessed `ShipmentId`
   (research.md Decision 12).
6. `order.Status = Converted`, `order.UpdatedAt = DateTime.UtcNow`.
7. `AddAsync(shipment)`, `AddAsync(shipmentEvent)`, `Update(order)`, then a single
   `await _unitOfWork.CommitAsync()` — atomic across all three writes (research.md Decision 11).
8. Return `new ConvertOrderResultDto { ShipmentId = shipment.Id, TrackingNumber =
   shipment.TrackingNumber }` (both now populated post-commit).

## Migration

A single new EF Core migration (`AddOrdersAndShipmentEvents`, generated during implementation)
creates two new tables (`Orders`, `ShipmentEvents`), their FKs (`Orders.CustomerId` → `Customers.Id`
Restrict; `Orders.OriginBranchId` → `Branches.Id` Restrict, nullable; `ShipmentEvents.ShipmentId` →
`Shipments.Id` Restrict), the unique index on `Orders.OrderNumber`, and adds the nullable
`Shipments.OrderId` column with its own unique-allowing-null index. No existing table's data is
altered; the 5 seeded `Shipment` rows get `OrderId = NULL` implicitly.
