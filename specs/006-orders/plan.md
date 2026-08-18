# Implementation Plan: Orders Module

**Branch**: `006-orders` | **Date**: 2026-08-17 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/006-orders/spec.md`

## Summary

New `Order` entity (module-owned) referencing `Customer` (module `005`) and, optionally, `Branch`
(module `003`) by FK — plus one nullable column added to the existing `Shipment` entity (`OrderId`)
and one new `ShipmentEvent` entity, both required by this module's central operation: converting a
`Confirmed` Order into a `Shipment`. Per `spec.md`'s Clarifications, `Shipment` is **not** extended
with Order detail (destination, dimensions, quoted price) — it keeps its existing minimal shape and
gains only the `OrderId` back-reference; the Order remains the permanent system of record for that
detail. `OrderService` follows the same structural-vs-database-dependent validation split established
since module `004` (FluentValidation for shape/conditional-field rules, Service-layer checks for
Customer/Branch existence+active-status), plus a second failure category this module newly needs at
this scale — status-transition guards (confirm/cancel/update-when-locked/convert) — modeled with
`InvalidOperationException` → `400`, reusing the pattern `ShipmentService`'s own status-transition
guard already established, rather than overloading `FluentValidation.ValidationException` for a
different kind of failure. `ConvertToShipmentAsync` writes the new `Shipment`, its first
`ShipmentEvent`, and the Order's `Converted` status update through a single `IUnitOfWork.CommitAsync()`
call, which already provides atomicity via EF Core's implicit per-`SaveChanges` transaction — no new
transaction-management code needed. Order-number/tracking-number generation uses the exact
`COUNT`-based per-day sequence the user specified, implemented entirely with the existing generic
`CountAsync(filter)` — no new repository methods, no sequence table. The list endpoint reuses the
established `PagedResult<T>`/header/`onlyActive`-style filter contract, with `customerId`/`status`
filters per the user's query string.

## Technical Context

**Language/Version**: C# on .NET 8.0 (`net8.0`, unchanged from the rest of the solution)

**Primary Dependencies**: ASP.NET Core 8, Entity Framework Core 8 + SQL Server (already referenced),
AutoMapper (already registered), FluentValidation (already registered) — **zero new NuGet packages**
(Principio III / Minimalismo de Dependencias).

**Storage**: SQL Server via EF Core, same `AppDbContext`. Two new tables (`Orders`, `ShipmentEvents`)
plus one new nullable column on the existing `Shipments` table (`OrderId`, forced nullable by 5
pre-existing seeded rows and the still-active direct-Shipment-creation endpoint — research.md Decision
2). Unique index on `Orders.OrderNumber`; unique-allowing-null index on `Shipments.OrderId`. Requires
one new migration (generated during implementation).

**Testing**: Manual via Swagger/HTTP (see `quickstart.md`), same policy as the rest of the project —
no automated test project exists.

**Target Platform**: ASP.NET Core Web API (no hosting change; same CORS `AllowReactApp`, which already
exposes the `X-Total-*` headers since `002`)

**Project Type**: Web service — same existing layered solution, additive new module touching one
existing entity (`Shipment`) with the smallest possible change (one nullable column)

**Performance Goals**: N/A — no latency/throughput target defined; the plan input's own order-number
generation algorithm (a `COUNT`-based per-day sequence) carries a known, accepted race condition under
true concurrency, documented rather than engineered around, since no concurrency target exists to
justify the added complexity (research.md Decision 8).

**Constraints**: Reuses `IBaseRepository<T>` + `IUnitOfWork`, AutoMapper for output only,
FluentValidation for structural/conditional-field rules; adds one new failure-handling pattern to this
module specifically — `InvalidOperationException` for status-transition guards — reusing
`ShipmentService`'s own existing precedent rather than introducing a third pattern (research.md
Decision 10). `Shipment` (`001`/`002`) is modified by exactly one nullable column and nothing else;
`Branch`/`Customer`/`Employee`/`Vehicle` are not modified at all. Recipient/destination fields have no
`Country` column, and `ServiceType`/`QuotedPrice`/`Notes` are included on `Order` even though
`spec.md` didn't name them — both refinements of the plan input over the spec's higher-level defaults
(research.md Decisions 4-5).

**Scale/Scope**: New module, additive across all 4 layers: `Core` (2 new entities — `Order`,
`ShipmentEvent` — plus 4 new enums, 4 DTOs, 2 repository interfaces + 1 service interface + 2 new
properties on `IUnitOfWork`), `Infrastructure` (2 repositories, 2 new EF configurations + 1 modified
one for `Shipment`, 1 migration, `AppDbContext`/`UnitOfWork` extended), `Services` (1 service, 2
validators, 1 mapping profile), `Web` (1 controller, DI registrations in `Program.cs`). No file
belonging to `Branch`, `Employee`, `Vehicle`, or `Customer` is modified; `Shipment.cs`/
`ShipmentConfiguration.cs`/`AppDbContext.cs` receive the smallest possible additive change.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Evaluation | Result |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | No `TargetFramework` change; everything used is already referenced. | PASS |
| II. Integridad de la Arquitectura en Capas | `Core` has no outbound dependency. `Infrastructure`/`Services` depend only on `Core`. `Web` is the only project depending on all three. `Order` references `Customer`/`Branch` only via FK + `IUnitOfWork.CustomerRepository`/`BranchRepository` (both already exposed) — no back-collection added to either. The one cross-module touch (`Shipment` gaining `OrderId`) is the minimum necessary for the module's stated central operation and doesn't invert any dependency direction. The new `InvalidOperationException` status-guard pattern (Decision 10) reuses — rather than competes with — `ShipmentService`'s own existing precedent for the same failure category, so no second competing pattern is introduced for either failure category (field validation vs. transition guard) this module needs. | PASS |
| III. Minimalismo de Dependencias | Zero new NuGet packages. | PASS |
| IV. Cambios Pequeños y Reversibles | Additive and (almost) self-contained: the only existing-module touch is one nullable column on `Shipment`, forced by real constraints (seed data, the still-active direct-creation endpoint) rather than a design preference — documented explicitly in research.md Decision 2 rather than silently expanding scope. `IUnitOfWork.cs`/`UnitOfWork.cs`/`Program.cs`/`AppDbContext.cs` get the same kind of small, additive edits already made for every prior module. | PASS |

No violations to record in Complexity Tracking.

**Re-check post Phase 1**: after designing `data-model.md`, `contracts/`, and `quickstart.md`, the
table holds unchanged. The two design choices with the most potential to violate Principio IV — how
much of `Shipment` to touch, and whether to add a second validation-failure pattern — were both
resolved by minimizing the touch (one nullable column) and reusing an existing pattern
(`InvalidOperationException`, already used by `ShipmentService`) rather than inventing a new one.

## Project Structure

### Documentation (this feature)

```text
specs/006-orders/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── orders-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Entities/
│   ├── Order.cs                                 # [NEW] see data-model.md for full field list
│   ├── ShipmentEvent.cs                         # [NEW] ShipmentId, EventType, StatusSnapshot,
│   │                                             #   OccurredAt, Shipment (forward nav only)
│   └── Shipment.cs                              # [MODIFY] + OrderId (int?)
├── Enums/
│   ├── OrderStatus.cs                           # [NEW] Pending, Confirmed, Converted, Cancelled
│   ├── ServiceType.cs                           # [NEW] Standard, Express, Economy
│   ├── PickupType.cs                            # [NEW] HomePickup, DropOff
│   └── ShipmentEventType.cs                     # [NEW] OrderConverted
├── DTOs/
│   └── Orders/
│       ├── CreateOrderDto.cs                    # [NEW] input for POST /api/orders
│       ├── UpdateOrderDto.cs                    # [NEW] input for PUT — no CustomerId
│       ├── OrderDto.cs                          # [NEW] output for every endpoint except convert
│       └── ConvertOrderResultDto.cs             # [NEW] { ShipmentId, TrackingNumber }
└── Interfaces/
    ├── IUnitOfWork.cs                           # [MODIFY] + IOrderRepository, IShipmentEventRepository
    ├── Repositories/
    │   ├── IOrderRepository.cs                  # [NEW] : IBaseRepository<Order> (no extra methods)
    │   └── IShipmentEventRepository.cs          # [NEW] : IBaseRepository<ShipmentEvent> (no extra methods)
    └── Services/
        └── IOrderService.cs                     # [NEW]

ShipmentTracker.Infrastructure/
├── Data/
│   ├── AppDbContext.cs                          # [MODIFY] + DbSet<Order>, DbSet<ShipmentEvent>,
│   │                                             #   ApplyConfiguration x2
│   ├── Configurations/
│   │   ├── OrderConfiguration.cs                # [NEW] ToTable("Orders"), unique index on
│   │   │                                        #   OrderNumber, FKs to Customer/Branch Restrict
│   │   ├── ShipmentEventConfiguration.cs        # [NEW] ToTable("ShipmentEvents"), FK to
│   │   │                                        #   Shipment Restrict
│   │   └── ShipmentConfiguration.cs             # [MODIFY] + OrderId nullable, unique-allowing-null index
│   └── UnitOfWork.cs                            # [MODIFY] + lazy OrderRepository/ShipmentEventRepository
├── Migrations/
│   └── <timestamp>_AddOrdersAndShipmentEvents.cs # [NEW] generated during implementation
└── Repositories/
    ├── OrderRepository.cs                       # [NEW] : BaseRepository<Order>, IOrderRepository
    └── ShipmentEventRepository.cs               # [NEW] : BaseRepository<ShipmentEvent>, IShipmentEventRepository

ShipmentTracker.Services/
├── OrderService.cs                              # [NEW]
├── Mappings/
│   └── OrderMappingProfile.cs                   # [NEW] Order→OrderDto (output-only)
└── Validators/
    └── Orders/
        ├── CreateOrderDtoValidator.cs            # [NEW] structural + HomePickup/DropOff conditional rules
        └── UpdateOrderDtoValidator.cs            # [NEW] same rules, no CustomerId

ShipmentTracker.Web/
├── Program.cs                                   # [MODIFY] + DI registrations (2 repos, 1 service,
│                                                 #   2 validators)
└── Controllers/
    └── OrderController.cs                       # [NEW] POST, GET (paginated), GET/{id},
                                                   #   GET/number/{orderNumber}, PUT/{id},
                                                   #   POST/{id}/confirm, DELETE/{id}, POST/{id}/convert
```

**Structure Decision**: Same existing layered architecture, no new projects. `Order` and
`ShipmentEvent` are two new entities within one module (sharing `OrderService`/`OrderController` for
`Order`; `ShipmentEvent` has no service/controller of its own — it is only ever written internally by
`OrderService.ConvertToShipmentAsync`, matching `BranchSchedule`'s precedent of being a
service-managed child concept with no independent CRUD surface). `Shipment` receives the smallest
possible additive touch (one nullable column) rather than being restructured, per `spec.md`'s
Clarifications.

## Complexity Tracking

*No violations to justify — table intentionally omitted.*
