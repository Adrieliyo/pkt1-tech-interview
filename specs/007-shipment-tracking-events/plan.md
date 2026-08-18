# Implementation Plan: Shipment Tracking Events

**Branch**: `007-shipment-tracking-events` | **Date**: 2026-08-18 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/007-shipment-tracking-events/spec.md`

## Summary

Additive extension of the existing `ShipmentEvent` entity (module `006`) with four new nullable/required
columns (`EmployeeId`, `LocationLabel`, `Notes`, `CreatedAt`) and one new one-to-one child entity,
`DeliveryAttempt`. No existing `ShipmentEvent` column is dropped, renamed, or retyped, and `Shipment`
(module `001`/`002`) is untouched beyond gaining one new `ShipmentStatus` enum member (`OutForDelivery`,
forced by the feature's own business rules — research.md Decision 2) with two new transition edges added
to the existing `ShipmentTransitionValidator` rather than a new validator class. A new `ShipmentEventService`
+ `ShipmentEventController` (routed under `/api/shipments/...`, coexisting with the existing singular
`/api/shipment/...` controller — research.md Decision 5) exposes: registering a generic shipment event
(currently only `OutForDelivery` is legal, since `DeliveryAttempted` and `OrderConverted` are excluded
in favor of their own dedicated/internal creation paths), registering a delivery-attempt event (which
auto-creates its `DeliveryAttempt` row with a service-computed `AttemptNumber`, never accepting one from
the caller), listing a shipment's events (operational view, includes `EmployeeId`), and a new public
tracking endpoint (privacy-filtered view, `EmployeeId` and all employee data structurally absent from
the DTO shape, not just nulled). Validation follows the established structural-vs-database-dependent
split, plus reuses the existing `ShipmentTransitionValidator`/`StatusTransitionContext` for status-change
legality — the same failure-category distinction (`ValidationException` for field errors, and the
`InvalidOperationException` pattern is not needed here since every rejection in this feature is a
field-level or transition-level validation failure, not a bare state-transition guard with no property to
attach to).

## Technical Context

**Language/Version**: C# on .NET 8.0 (`net8.0`, unchanged from the rest of the solution)

**Primary Dependencies**: ASP.NET Core 8, Entity Framework Core 8 + SQL Server (already referenced),
AutoMapper (already registered), FluentValidation (already registered) — **zero new NuGet packages**
(Principio III / Minimalismo de Dependencias).

**Storage**: SQL Server via EF Core, same `AppDbContext`. One new table (`DeliveryAttempts`), one
additively-modified existing table (`ShipmentEvents` gains 4 columns, all nullable except the required
`CreatedAt`, which needs no backfill concern since `ShipmentEvent` rows only exist from module `006`
onward in this environment — still generated as a plain `ADD COLUMN`, no data migration needed). FK
`ShipmentEvents.EmployeeId → Employees.Id` (Restrict, nullable). FK `DeliveryAttempts.ShipmentEventId →
ShipmentEvents.Id` (Restrict, unique — one-to-one). Requires one new, strictly additive migration.

**Testing**: Manual via Swagger/HTTP (see `quickstart.md`), same policy as the rest of the project —
no automated test project exists.

**Target Platform**: ASP.NET Core Web API (no hosting change; same CORS `AllowReactApp`)

**Project Type**: Web service — same existing layered solution, additive new module touching two
existing entities (`ShipmentEvent` additively, `Shipment`'s status enum) with the smallest possible
changes

**Performance Goals**: N/A — no latency/throughput target defined; per-shipment event counts are small
and bounded, so no pagination is introduced (research.md Decision 12).

**Constraints**: Reuses `IBaseRepository<T>` + `IUnitOfWork`, AutoMapper for output only (entity
construction done by hand), FluentValidation for structural/conditional rules, and — new to this
module's plumbing but not to the codebase — the existing `ShipmentTransitionValidator`/
`StatusTransitionContext` (module `001`), extended with two new transition edges rather than duplicated.
`ShipmentEvent`'s migration is additive-only per explicit constraint (`.AddColumn<>()` only, no
`AlterColumn`/`DropColumn`/`RenameColumn`). No DTO inheritance is introduced (research.md Decision 10);
`[AllowAnonymous]` is omitted since no auth middleware exists anywhere in this solution (research.md
Decision 6).

**Scale/Scope**: New module, additive across all 4 layers: `Core` (1 new entity — `DeliveryAttempt` —
plus 1 modified entity — `ShipmentEvent` — 1 new enum, 2 extended enums, 7 DTOs, 1 new repository
interface + 1 new service interface + 1 new property on `IUnitOfWork`), `Infrastructure` (1 new
repository, 1 new EF configuration + 1 modified one, 1 modified transition validator note — actually
`ShipmentTransitionValidator` lives in `Services`, see below — 1 migration, `AppDbContext`/`UnitOfWork`
extended), `Services` (1 new service, 2 new validators, 1 modified validator —
`ShipmentTransitionValidator` gains 2 rule branches, 1 new mapping profile), `Web` (1 new controller, DI
registrations in `Program.cs`). No file belonging to `Branch`, `Employee`, `Vehicle`, `Customer`, or
`Order` is modified; `Shipment.cs`/`ShipmentController.cs`/`ShipmentService.cs` are **not** modified —
only `ShipmentStatus.cs` (enum, gains one member) and `ShipmentTransitionValidator.cs` (gains two
transition edges) are touched outside this feature's own new files.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Evaluation | Result |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | No `TargetFramework` change; everything used is already referenced. | PASS |
| II. Integridad de la Arquitectura en Capas | `Core` has no outbound dependency. `Infrastructure`/`Services` depend only on `Core`. `Web` is the only project depending on all three. The feature deliberately reuses two existing mechanisms instead of introducing competing ones: `ShipmentTransitionValidator` (extended, not duplicated — research.md Decision 3) and the established Service-layer shared-helper pattern for DB-dependent validation (not DTO inheritance — research.md Decision 10). The one new cross-cutting touch (`ShipmentStatus` gains `OutForDelivery`) is the minimum necessary for the feature's own explicitly-stated business rules. | PASS |
| III. Minimalismo de Dependencias | Zero new NuGet packages. | PASS |
| IV. Cambios Pequeños y Reversibles | `ShipmentEvent`'s migration is additive-only per explicit constraint. `Shipment.cs`/`ShipmentController.cs`/`ShipmentService.cs` are untouched; only `ShipmentStatus.cs` and `ShipmentTransitionValidator.cs` receive small, explicitly-justified additions (one enum member, two transition-rule branches) rather than a restructure. `IUnitOfWork.cs`/`UnitOfWork.cs`/`Program.cs`/`AppDbContext.cs` get the same kind of small, additive edits already made for every prior module. | PASS |

No violations to record in Complexity Tracking.

**Re-check post Phase 1**: after designing `data-model.md`, `contracts/`, and `quickstart.md`, the
table holds unchanged. The two design choices with the most potential to violate Principio II — how to
enforce transition legality, and how to share validation rules between the two register-event DTOs —
were both resolved by reusing an existing, already-adopted mechanism (the transition validator; the
shared-Service-helper pattern) rather than inventing a new one.

## Project Structure

### Documentation (this feature)

```text
specs/007-shipment-tracking-events/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── shipment-tracking-events-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Entities/
│   ├── ShipmentEvent.cs                         # [MODIFY] + EmployeeId (int?), Employee (nav),
│   │                                             #   LocationLabel (string?), Notes (string?),
│   │                                             #   CreatedAt (DateTime, required)
│   └── DeliveryAttempt.cs                       # [NEW] Id, ShipmentEventId, ShipmentEvent (nav),
│                                                 #   AttemptNumber, FailureReason, NextAttemptAt
├── Enums/
│   ├── ShipmentStatus.cs                        # [MODIFY] + OutForDelivery
│   ├── ShipmentEventType.cs                     # [MODIFY] + OutForDelivery, DeliveryAttempted
│   └── DeliveryFailureReason.cs                 # [NEW] NoOneHome, WrongAddress, Refused,
│                                                 #   AccessDenied, Other
├── DTOs/
│   └── ShipmentEvents/
│       ├── RegisterEventDto.cs                  # [NEW] input for POST .../events
│       ├── RegisterDeliveryAttemptDto.cs        # [NEW] input for POST .../events/delivery-attempt
│       ├── ShipmentEventDto.cs                  # [NEW] operational output, includes EmployeeId
│       ├── DeliveryAttemptDetailDto.cs          # [NEW] nested in ShipmentEventDto/TrackingEventDto
│       ├── ShipmentTrackingDto.cs               # [NEW] public tracking response (shipment summary)
│       └── TrackingEventDto.cs                  # [NEW] nested in ShipmentTrackingDto, no EmployeeId
└── Interfaces/
    ├── IUnitOfWork.cs                           # [MODIFY] + IDeliveryAttemptRepository
    ├── Repositories/
    │   └── IDeliveryAttemptRepository.cs        # [NEW] : IBaseRepository<DeliveryAttempt> (no extra methods)
    └── Services/
        └── IShipmentEventService.cs             # [NEW]

ShipmentTracker.Infrastructure/
├── Data/
│   ├── AppDbContext.cs                          # [MODIFY] + DbSet<DeliveryAttempt>, ApplyConfiguration
│   ├── Configurations/
│   │   ├── ShipmentEventConfiguration.cs        # [MODIFY] + 4 new columns, EmployeeId FK Restrict
│   │   └── DeliveryAttemptConfiguration.cs      # [NEW] ToTable("DeliveryAttempts"), unique index on
│   │                                            #   ShipmentEventId, FK Restrict
│   └── UnitOfWork.cs                            # [MODIFY] + lazy DeliveryAttemptRepository property
├── Migrations/
│   └── <timestamp>_ExtendShipmentEventsAndAddDeliveryAttempts.cs  # [NEW] additive only
└── Repositories/
    └── DeliveryAttemptRepository.cs             # [NEW] : BaseRepository<DeliveryAttempt>, IDeliveryAttemptRepository

ShipmentTracker.Services/
├── ShipmentEventService.cs                      # [NEW]
├── Mappings/
│   └── ShipmentEventMappingProfile.cs           # [NEW] ShipmentEvent→ShipmentEventDto,
│                                                 #   Shipment→ShipmentTrackingDto (output-only)
└── Validators/
    ├── Shipments/
    │   └── ShipmentTransitionValidator.cs       # [MODIFY] + InTransit→OutForDelivery,
    │                                            #   OutForDelivery→Delivered/Cancelled
    └── ShipmentEvents/
        ├── RegisterEventDtoValidator.cs          # [NEW] structural rules incl. EventType exclusions
        └── RegisterDeliveryAttemptDtoValidator.cs # [NEW] structural rules incl. NextAttemptAt > OccurredAt

ShipmentTracker.Web/
├── Program.cs                                   # [MODIFY] + DI registrations (1 repo, 1 service,
│                                                 #   2 validators)
└── Controllers/
    └── ShipmentEventController.cs               # [NEW] POST .../events, POST .../events/delivery-attempt,
                                                   #   GET .../events, GET tracking/{trackingNumber}
```

**Structure Decision**: Same existing layered architecture, no new projects. `ShipmentEvent` and
`DeliveryAttempt` share one new service/controller (`ShipmentEventService`/`ShipmentEventController`),
matching the established one-service-per-primary-entity shape — `DeliveryAttempt` has no independent
CRUD surface of its own, only ever written internally alongside a `DeliveryAttempted` event, the same
shape `BranchSchedule` (module `003`) and (now) `DeliveryAttempt` both follow: a service-managed child
concept with no independent endpoints. The new controller's `/api/shipments/...` route prefix
deliberately coexists with the existing singular `/api/shipment/...` (research.md Decision 5) rather
than unifying them.

## Complexity Tracking

*No violations to justify — table intentionally omitted.*
