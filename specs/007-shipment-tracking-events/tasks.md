---

description: "Task list for the Shipment Tracking Events feature implementation"
---

# Tasks: Shipment Tracking Events

**Input**: Design documents from `/specs/007-shipment-tracking-events/`

**Prerequisites**: plan.md (required), spec.md (required — 3 user stories), research.md, data-model.md, contracts/shipment-tracking-events-api-contract.md, quickstart.md

**Tests**: No automated test tasks — this project has no test project; validation is manual via Swagger/HTTP (quickstart.md), consistent with the rest of the solution.

**Organization**: Tasks are grouped by user story. Foundational work (entity/DTO/interface/infrastructure layers, the shared transition-validator extension) is a single blocking phase before any story, since `ShipmentEvent` is extended and `DeliveryAttempt` is introduced as prerequisites for all three stories. `GetEventsByShipmentAsync` (operational) and `GetTrackingAsync` (public) are both grouped under User Story 3, since spec.md's US3 acceptance scenarios explicitly contrast the two (privacy filtering is only provable by comparing them).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: US1 to US3
- Include exact file paths in descriptions

## Path Conventions

- **Solution**: existing 4-project layered solution — `ShipmentTracker.Core/`, `ShipmentTracker.Infrastructure/`, `ShipmentTracker.Services/`, `ShipmentTracker.Web/` at repo root
- **Docs**: feature docs under `specs/007-shipment-tracking-events/`

---

## Phase 1: Setup

**N/A.** No project initialization or new packages (research.md: zero new NuGet dependencies). The
solution and all four projects already exist.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Extend `ShipmentEvent`, add `DeliveryAttempt`, extend the shared `ShipmentStatus`/
`ShipmentEventType` enums and the existing `ShipmentTransitionValidator`, and wire the full
Infrastructure layer — MUST be complete before ANY user story can be implemented.

**CRITICAL**: No user story work can begin until this phase is complete.

### Core layer (`ShipmentTracker.Core/`)

- [X] T001 [P] Modify `ShipmentStatus` enum in `ShipmentTracker.Core/Enums/ShipmentStatus.cs`: add member `OutForDelivery` between `InTransit` and `Delivered`, update the XML doc comment to describe it (research.md Decision 2 — no `Returned` member; only `Delivered`/`Cancelled` stay terminal)
- [X] T002 [P] Modify `ShipmentEventType` enum in `ShipmentTracker.Core/Enums/ShipmentEventType.cs`: add members `OutForDelivery`, `DeliveryAttempted` alongside the existing `OrderConverted`, update the XML doc comment
- [X] T003 [P] Create `DeliveryFailureReason` enum in `ShipmentTracker.Core/Enums/DeliveryFailureReason.cs` with members `NoOneHome, WrongAddress, Refused, AccessDenied, Other`, XML doc comment matching existing enum style
- [X] T004 Modify `ShipmentEvent` entity in `ShipmentTracker.Core/Entities/ShipmentEvent.cs`: add `int? EmployeeId`, `Employee? Employee` (forward navigation only, no reverse collection on `Employee.cs`), `string? LocationLabel`, `string? Notes`, `DateTime CreatedAt` (required) — do NOT remove/rename `Id`, `ShipmentId`, `Shipment`, `EventType`, `StatusSnapshot`, `OccurredAt` (depends on T002 for the doc comment reference, no hard code dependency)
- [X] T005 [P] Create `DeliveryAttempt` entity in `ShipmentTracker.Core/Entities/DeliveryAttempt.cs`: `int Id`, `int ShipmentEventId`, `ShipmentEvent ShipmentEvent` (forward navigation, `null!`, no reverse collection on `ShipmentEvent.cs`), `int AttemptNumber`, `DeliveryFailureReason FailureReason`, `DateTime? NextAttemptAt` (depends on T003, T004)
- [X] T006 [P] Create `RegisterEventDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/RegisterEventDto.cs`: `ShipmentEventType? EventType` (with `[JsonConverter(typeof(JsonStringEnumConverter))]`), `int? EmployeeId`, `string? LocationLabel`, `string? Notes`, `DateTime OccurredAt` (depends on T002)
- [X] T007 [P] Create `RegisterDeliveryAttemptDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/RegisterDeliveryAttemptDto.cs`: `int? EmployeeId`, `string? LocationLabel`, `string? Notes`, `DateTime OccurredAt`, `DeliveryFailureReason? FailureReason` (with `[JsonConverter(typeof(JsonStringEnumConverter))]`), `DateTime? NextAttemptAt` — NO `EventType` property (research.md Decision 10) (depends on T003)
- [X] T008 [P] Create `DeliveryAttemptDetailDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/DeliveryAttemptDetailDto.cs`: `int AttemptNumber`, `DeliveryFailureReason FailureReason` (with `[JsonConverter(typeof(JsonStringEnumConverter))]`), `DateTime? NextAttemptAt` (depends on T003)
- [X] T009 Create `ShipmentEventDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/ShipmentEventDto.cs`: `int Id`, `int ShipmentId`, `ShipmentEventType EventType` (JSON string converter), `ShipmentStatus StatusSnapshot` (JSON string converter), `int? EmployeeId`, `string? LocationLabel`, `string? Notes`, `DateTime OccurredAt`, `DateTime CreatedAt`, `DeliveryAttemptDetailDto? DeliveryAttempt` — the OPERATIONAL shape, includes `EmployeeId` (depends on T001, T002, T008)
- [X] T010 [P] Create `TrackingEventDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/TrackingEventDto.cs`: `ShipmentEventType EventType` (JSON string converter), `ShipmentStatus StatusSnapshot` (JSON string converter), `string? LocationLabel`, `string? Notes`, `DateTime OccurredAt`, `DeliveryAttemptDetailDto? DeliveryAttempt` — the PUBLIC-SAFE shape: no `Id`, no `EmployeeId`, no `CreatedAt` (research.md Decision 8) (depends on T001, T002, T008)
- [X] T011 Create `ShipmentTrackingDto` in `ShipmentTracker.Core/DTOs/ShipmentEvents/ShipmentTrackingDto.cs`: `string TrackingNumber`, `ShipmentStatus Status` (JSON string converter), `string Recipient`, `DateTime CreatedAt`, `DateTime? DeliveredAt`, `List<TrackingEventDto> Events` (depends on T001, T010)
- [X] T012 [P] Create `IDeliveryAttemptRepository` in `ShipmentTracker.Core/Interfaces/Repositories/IDeliveryAttemptRepository.cs`: `: IBaseRepository<DeliveryAttempt>`, no extra methods (research.md Decision 11 — `AttemptNumber` computed via the generic `CountAsync` with a navigation-property filter) (depends on T005)
- [X] T013 [P] Modify `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs`: add `IDeliveryAttemptRepository DeliveryAttemptRepository { get; }` (depends on T012)
- [X] T014 Create `IShipmentEventService` in `ShipmentTracker.Core/Interfaces/Services/IShipmentEventService.cs` with exactly these methods: `Task<ShipmentEventDto?> RegisterEventAsync(int shipmentId, RegisterEventDto dto)`, `Task<ShipmentEventDto?> RegisterDeliveryAttemptAsync(int shipmentId, RegisterDeliveryAttemptDto dto)`, `Task<IEnumerable<ShipmentEventDto>?> GetEventsByShipmentAsync(int shipmentId)`, `Task<ShipmentTrackingDto?> GetTrackingAsync(string trackingNumber)` (depends on T006, T007, T009, T011)

### Shared validator extension (`ShipmentTracker.Services/`)

- [X] T015 Modify `ShipmentTransitionValidator` in `ShipmentTracker.Services/Validators/Shipments/ShipmentTransitionValidator.cs`: in `BeAValidTransition`, add `if (context.CurrentStatus == ShipmentStatus.InTransit) return newStatus == ShipmentStatus.Delivered || newStatus == ShipmentStatus.Cancelled || newStatus == ShipmentStatus.OutForDelivery;` (extending the existing `InTransit` branch — do not duplicate the branch) and a new branch `if (context.CurrentStatus == ShipmentStatus.OutForDelivery) return newStatus == ShipmentStatus.Delivered || newStatus == ShipmentStatus.Cancelled;` before the final `return false;` — do NOT modify the existing `Collected`/terminal-state/same-status branches (research.md Decision 3) (depends on T001)

### Infrastructure layer (`ShipmentTracker.Infrastructure/`)

- [X] T016 [P] Modify `ShipmentEventConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/ShipmentEventConfiguration.cs`: add `builder.Property(x => x.EmployeeId).IsRequired(false);`, `builder.Property(x => x.LocationLabel).IsRequired(false).HasMaxLength(255);`, `builder.Property(x => x.Notes).IsRequired(false).HasMaxLength(1000);`, `builder.Property(x => x.CreatedAt).IsRequired();`, `builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);` — do NOT modify any existing line in this file (additive only, per the explicit migration constraint) (depends on T004)
- [X] T017 [P] Create `DeliveryAttemptConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/DeliveryAttemptConfiguration.cs` implementing `IEntityTypeConfiguration<DeliveryAttempt>`: `ToTable("DeliveryAttempts")`, identity PK, `FailureReason` `.IsRequired().HasConversion<string>()`, `NextAttemptAt` optional, `ShipmentEventId` FK `HasOne(x => x.ShipmentEvent).WithMany().HasForeignKey(x => x.ShipmentEventId).IsRequired().OnDelete(DeleteBehavior.Restrict)` plus `builder.HasIndex(x => x.ShipmentEventId).IsUnique();` (one-to-one) (depends on T005)
- [X] T018 Modify `ShipmentTracker.Infrastructure/Data/AppDbContext.cs`: add `public DbSet<DeliveryAttempt> DeliveryAttempts { get; set; }` and register `builder.ApplyConfiguration(new DeliveryAttemptConfiguration());` in `OnModelCreating` (the `ShipmentEventConfiguration` registration already exists from module 006 — no change needed there) (depends on T016, T017)
- [X] T019 [P] Create `DeliveryAttemptRepository` in `ShipmentTracker.Infrastructure/Repositories/DeliveryAttemptRepository.cs` as `public class DeliveryAttemptRepository : BaseRepository<DeliveryAttempt>, IDeliveryAttemptRepository` with a `(AppDbContext context) : base(context)` constructor (copy `ShipmentEventRepository.cs` shape) (depends on T012, T018)
- [X] T020 Modify `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs`: add private field `_deliveryAttemptRepository` and lazy property `public IDeliveryAttemptRepository DeliveryAttemptRepository => _deliveryAttemptRepository ??= new DeliveryAttemptRepository(_context);` (depends on T013, T019)
- [X] T021 Generate the EF Core migration: `dotnet ef migrations add ExtendShipmentEventsAndAddDeliveryAttempts --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` from the repo root, then review the generated file to confirm it uses `AddColumn<>()` exclusively for `ShipmentEvents` (no `DropColumn`/`RenameColumn`/`AlterColumn` on any existing column), creates `DeliveryAttempts` with its unique FK to `ShipmentEvents`, and adds the `EmployeeId` FK to `Employees` — no other table is touched. Depends on all of T001–T020. NOTE: it is normal for `dotnet ef` to build the solution; fix any compile errors in T001–T020 before this passes
- [X] T022 [P] Create `ShipmentEventMappingProfile` in `ShipmentTracker.Services/Mappings/ShipmentEventMappingProfile.cs` as `public class ShipmentEventMappingProfile : Profile` with `CreateMap<ShipmentEvent, ShipmentEventDto>();`, `CreateMap<ShipmentEvent, TrackingEventDto>();`, and `CreateMap<Shipment, ShipmentTrackingDto>().ForMember(d => d.Events, opt => opt.Ignore());` (the `Events` list is populated by hand in the service, since it requires the per-event `DeliveryAttempt` lookup — see data-model.md's `GetTrackingAsync` flow) (depends on T004, T005, T009, T010, T011)

**Checkpoint**: `ShipmentEvent` extended, `DeliveryAttempt` created, both enums extended, the shared
transition validator extended, full Infrastructure layer wired, migration generated (not yet applied —
applied in Polish). No story has a service method or controller action yet — that starts in Phase 3.

---

## Phase 3: User Story 1 - Mark a shipment out for delivery (Priority: P1) — MVP

**Goal**: `POST /api/shipments/{id}/events` with `eventType: "OutForDelivery"` and a valid active Driver
`employeeId` transitions the shipment to `OutForDelivery` and records the event. All rejection paths
(missing/invalid/wrong-role employee, wrong event type, invalid transition) return `400`.

**Independent Test**: Submit the endpoint against an `InTransit` shipment with a valid active Driver →
`201`, shipment status becomes `OutForDelivery`; repeat with no `employeeId`, a non-Driver `employeeId`,
an inactive/nonexistent `employeeId`, `eventType: "DeliveryAttempted"` or `"OrderConverted"`, a future
`occurredAt`, or against a `Delivered`/`Cancelled` shipment → each `400`; repeat against an already
`OutForDelivery` shipment → `201` (allowed, not rejected). (spec.md US1 scenarios 1–3; quickstart.md §2
US1)

### Implementation for User Story 1

- [X] T023 [P] [US1] Create `RegisterEventDtoValidator` in `ShipmentTracker.Services/Validators/ShipmentEvents/RegisterEventDtoValidator.cs` as `AbstractValidator<RegisterEventDto>` — pure structural rules, no repository access (research.md Decision 9): `EventType` `NotNull().IsInEnum()`, plus `.Must(t => t != ShipmentEventType.DeliveryAttempted && t != ShipmentEventType.OrderConverted).WithMessage("EventType must not be DeliveryAttempted or OrderConverted — use their dedicated creation paths.")`; `OccurredAt` `.LessThanOrEqualTo(DateTime.UtcNow).WithMessage("OccurredAt must not be in the future.")` (depends on T006)
- [X] T024 [US1] Create `ShipmentEventService` in `ShipmentTracker.Services/ShipmentEventService.cs` implementing `IShipmentEventService`, constructor injecting `IUnitOfWork`, `IMapper`, `IValidator<RegisterEventDto>`, `IValidator<StatusTransitionContext>` (the EXISTING `ShipmentTransitionValidator`, reused — research.md Decision 3); implement a shared private helper `Task<List<ValidationFailure>> ValidateEmployeeAsync(int? employeeId, bool requireDriver)` that returns failures if `requireDriver` and `employeeId` is null, or if `employeeId` is provided and the employee does not exist / is not active / (`requireDriver` and `Role != EmployeeRole.Driver`); implement `RegisterEventAsync(shipmentId, dto)`: load shipment via `GetByIdAsync` (null → return null), run `_registerEventValidator.ValidateAsync(dto)`, call `ValidateEmployeeAsync(dto.EmployeeId, requireDriver: dto.EventType == ShipmentEventType.OutForDelivery)`, compute `var newStatus = dto.EventType == ShipmentEventType.OutForDelivery ? ShipmentStatus.OutForDelivery : shipment.Status;`, run `_transitionValidator.ValidateAsync(new StatusTransitionContext { CurrentStatus = shipment.Status, NewStatus = newStatus })` and merge its failure if invalid, throw `FluentValidation.ValidationException` if any failures accumulated (nothing written); otherwise `shipment.Status = newStatus;` construct `ShipmentEvent` (`StatusSnapshot = newStatus`, `CreatedAt = DateTime.UtcNow`, plus submitted fields), `Update(shipment)` + `AddAsync(shipmentEvent)` + single `CommitAsync()`, return `_mapper.Map<ShipmentEventDto>(shipmentEvent)` (data-model.md `RegisterEventAsync` flow) (depends on T014, T015, T022, T023)
- [X] T025 [US1] Register DI in `ShipmentTracker.Web/Program.cs`: add `builder.Services.AddScoped<IDeliveryAttemptRepository, DeliveryAttemptRepository>();`, `builder.Services.AddScoped<IShipmentEventService, ShipmentEventService>();`, `builder.Services.AddScoped<IValidator<RegisterEventDto>, RegisterEventDtoValidator>();` alongside the existing registrations (add `using` statements for the new namespaces as needed; `IValidator<StatusTransitionContext>` is already registered from module 001) (depends on T023, T024)
- [X] T026 [US1] Create `ShipmentEventController` in `ShipmentTracker.Web/Controllers/ShipmentEventController.cs` with `[Route("api/shipments")]`, `[ApiController]`, `[Produces("application/json")]`, constructor-injected `IShipmentEventService`, XML doc comments on the class and every action; implement `[HttpPost("{id}/events")] RegisterEvent(int id, [FromBody] RegisterEventDto dto)`: call the service, `201 Created($"/api/shipments/{id}/events", result)` on success, `404 NotFound(new { message = $"No shipment was found with id '{id}'." })` when the service returns null, catch `FluentValidation.ValidationException` → `400 BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) })` (depends on T024, T025)

**Checkpoint**: User Story 1 fully functional and independently testable — a shipment can be marked
out for delivery, with the Driver requirement and transition legality both enforced.

---

## Phase 4: User Story 2 - Log a failed delivery attempt (Priority: P2)

**Goal**: `POST /api/shipments/{id}/events/delivery-attempt` on an `OutForDelivery` shipment records a
`DeliveryAttempted` event and auto-creates its `DeliveryAttempt` row with a correctly-sequenced
`AttemptNumber`, without changing the shipment's status.

**Independent Test**: On a shipment already `OutForDelivery` (via US1), submit the endpoint with a
`failureReason` → `201` with `deliveryAttempt.attemptNumber: 1`, shipment status unchanged; submit a
second one with no new out-for-delivery event in between → `attemptNumber: 2`; submit against an
`InTransit` shipment → `400`; submit with no/invalid `failureReason`, or `nextAttemptAt` not later than
`occurredAt` → `400`; submit with no `nextAttemptAt` → `201` with `nextAttemptAt: null`. (spec.md US2
scenarios 1–6; quickstart.md §2 US2)

### Implementation for User Story 2

- [X] T027 [P] [US2] Create `RegisterDeliveryAttemptDtoValidator` in `ShipmentTracker.Services/Validators/ShipmentEvents/RegisterDeliveryAttemptDtoValidator.cs` as `AbstractValidator<RegisterDeliveryAttemptDto>`: `OccurredAt` `.LessThanOrEqualTo(DateTime.UtcNow).WithMessage("OccurredAt must not be in the future.")`; `FailureReason` `.NotNull().IsInEnum()`; `NextAttemptAt` `.GreaterThan(x => x.OccurredAt).WithMessage("NextAttemptAt must be later than OccurredAt.").When(x => x.NextAttemptAt.HasValue)` — no `Include()`/inheritance from `RegisterEventDtoValidator` (research.md Decision 10) (depends on T007)
- [X] T028 [US2] Add `RegisterDeliveryAttemptAsync` to `ShipmentTracker.Services/ShipmentEventService.cs` (same file as T024 — do not create a second service; add `IValidator<RegisterDeliveryAttemptDto>` to the constructor): load shipment via `GetByIdAsync` (null → return null), run `_registerDeliveryAttemptValidator.ValidateAsync(dto)`, call the shared `ValidateEmployeeAsync(dto.EmployeeId, requireDriver: false)` helper from T024, add a failure if `shipment.Status != ShipmentStatus.OutForDelivery` (data-model.md Decision 4 — a plain equality check, NOT routed through `_transitionValidator`), throw `ValidationException` if any failures accumulated; otherwise construct `shipmentEvent = new ShipmentEvent { EventType = ShipmentEventType.DeliveryAttempted, StatusSnapshot = shipment.Status, CreatedAt = DateTime.UtcNow, ShipmentId = shipmentId, EmployeeId = dto.EmployeeId, LocationLabel = dto.LocationLabel, Notes = dto.Notes, OccurredAt = dto.OccurredAt }`, compute `var attemptNumber = await _unitOfWork.DeliveryAttemptRepository.CountAsync(x => x.ShipmentEvent.ShipmentId == shipmentId) + 1;`, construct `deliveryAttempt = new DeliveryAttempt { ShipmentEvent = shipmentEvent, AttemptNumber = attemptNumber, FailureReason = dto.FailureReason!.Value, NextAttemptAt = dto.NextAttemptAt }` (assign the `ShipmentEvent` navigation, NOT a guessed `ShipmentEventId` — research.md Decision 12 of module 006, same pattern), `AddAsync(shipmentEvent)` + `AddAsync(deliveryAttempt)` + single `CommitAsync()` — do NOT call `Update(shipment)`, its status is unchanged; map the result with `var eventDto = _mapper.Map<ShipmentEventDto>(shipmentEvent); eventDto.DeliveryAttempt = new DeliveryAttemptDetailDto { AttemptNumber = attemptNumber, FailureReason = deliveryAttempt.FailureReason, NextAttemptAt = deliveryAttempt.NextAttemptAt }; return eventDto;` (depends on T024, T027)
- [X] T029 [US2] Register `builder.Services.AddScoped<IValidator<RegisterDeliveryAttemptDto>, RegisterDeliveryAttemptDtoValidator>();` in `ShipmentTracker.Web/Program.cs` alongside the T025 registrations (depends on T027)
- [X] T030 [US2] Add `[HttpPost("{id}/events/delivery-attempt")] RegisterDeliveryAttempt(int id, [FromBody] RegisterDeliveryAttemptDto dto)` to `ShipmentTracker.Web/Controllers/ShipmentEventController.cs`: same response shape as T026's action (`201 Created($"/api/shipments/{id}/events", result)`, `404` on null, `400` on `ValidationException`) (depends on T026, T028, T029)

**Checkpoint**: User Stories 1 and 2 both work independently — a shipment can be marked out for
delivery and have one or more failed attempts logged against it, correctly sequenced.

---

## Phase 5: User Story 3 - View a shipment's public tracking timeline (Priority: P3)

**Goal**: `GET /api/shipments/{id}/events` returns the full operational event history (including
`employeeId`) for staff use; `GET /api/shipments/tracking/{trackingNumber}` returns the shipment plus
its event timeline with `employeeId` and all other employee data structurally absent, safe for public
consumption.

**Independent Test**: With events recorded via US1/US2, `GET /api/shipments/{id}/events` → `200` with
every event including `employeeId`; `GET /api/shipments/tracking/{trackingNumber}` for the same shipment
→ `200` with the same events but no `employeeId` key anywhere in the JSON, plus `locationLabel`/`notes`/
`deliveryAttempt` detail present; a shipment with no events → `200` with `events: []` on both endpoints,
not an error; an unknown id/tracking number → `404` on the respective endpoint. (spec.md US3 scenarios
1–3; quickstart.md §2 US3)

### Implementation for User Story 3

- [X] T031 [US3] Add `GetEventsByShipmentAsync` to `ShipmentTracker.Services/ShipmentEventService.cs`: load shipment via `GetByIdAsync` (null → return null); load its events via `_unitOfWork.ShipmentEventRepository.GetAsync(x => x.ShipmentId == shipmentId, orderBy: q => q.OrderBy(x => x.OccurredAt))` (no pagination — research.md Decision 12); for each event map to `ShipmentEventDto`, and when `EventType == ShipmentEventType.DeliveryAttempted`, look up `_unitOfWork.DeliveryAttemptRepository.SingleOrDefaultAsync(x => x.ShipmentEventId == event.Id)` and attach its `DeliveryAttemptDetailDto`; return the mapped list (depends on T024)
- [X] T032 [US3] Add `GetTrackingAsync` to `ShipmentTracker.Services/ShipmentEventService.cs`: load shipment via `_unitOfWork.ShipmentRepository.SingleOrDefaultAsync(x => x.TrackingNumber == trackingNumber)` (null → return null); load and order its events the same way as T031, map each to `TrackingEventDto` with the same per-event `DeliveryAttempt` lookup/attach logic; map the shipment to `ShipmentTrackingDto` via `_mapper.Map<ShipmentTrackingDto>(shipment)` then assign `.Events` by hand to the mapped list (depends on T024, T031)
- [X] T033 [US3] Add `[HttpGet("{id}/events")] GetEventsByShipment(int id)` to `ShipmentTracker.Web/Controllers/ShipmentEventController.cs`: `200 Ok(IEnumerable<ShipmentEventDto>)` or `404 NotFound(new { message = $"No shipment was found with id '{id}'." })`; XML doc comment noting this is the operational (staff) view, includes `employeeId` (depends on T026, T031)
- [X] T034 [US3] Add `[HttpGet("tracking/{trackingNumber}")] GetTracking(string trackingNumber)` to `ShipmentTracker.Web/Controllers/ShipmentEventController.cs`: `200 Ok(ShipmentTrackingDto)` or `404 NotFound(new { message = $"No shipment was found with tracking number '{trackingNumber}'." })`; XML doc comment noting this is the PUBLIC endpoint — no `[AllowAnonymous]` attribute, no auth middleware exists in this solution (research.md Decision 6) (depends on T026, T032)

**Checkpoint**: All three user stories complete — the full mark-out-for-delivery → log-attempt(s) →
publicly-track workflow is implemented and each story is independently testable.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Apply the migration, verify the whole solution, validate against the manual scenarios, and
keep the knowledge graph current.

- [X] T035 Run `dotnet build ShipmentTracker.sln` and fix any build warnings or errors introduced by the module (verify: no warnings from unused usings, nullable annotations, or missing XML doc comments)
- [X] T036 Apply the migration to the local database: `dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` — confirm `DeliveryAttempts` exists, `ShipmentEvents` gained exactly the 4 new columns with no data loss, and any pre-existing `ShipmentEvent` rows (from module 006's Order conversions) have `EmployeeId`/`LocationLabel`/`Notes` as `NULL` and a non-null backfilled `CreatedAt`. (depends on T035)
- [X] T037 Execute every scenario in `specs/007-shipment-tracking-events/quickstart.md` against the running API (`dotnet run --project ShipmentTracker.Web`), including the §3 "verify nothing else changed" checks (`GET/POST/PATCH /api/shipment` unchanged, existing seeded/converted Shipments untouched), and confirm the pre-existing `/api/branches`, `/api/employees`, `/api/vehicles`, `/api/customers`, `/api/orders` behaviors are unchanged. (depends on T035, T036)
- [X] T038 [P] Confirm no file outside this feature's own new files, plus `ShipmentStatus.cs`, `ShipmentEventType.cs`, and `ShipmentTransitionValidator.cs`, changed (`git diff --stat` should not list `Shipment.cs`, `ShipmentController.cs`, `ShipmentService.cs`, or any file belonging to `Branch`/`Employee`/`Vehicle`/`Customer`/`Order`, aside from the usual shared `IUnitOfWork.cs`/`UnitOfWork.cs`/`Program.cs`/`AppDbContext.cs`). (depends on T035)
- [X] T039 [P] Run `graphify update .` from the repo root so the knowledge graph reflects the new module (per CLAUDE.md — AST-only, no API cost). (depends on all code tasks)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Stories (Phases 3–5)**: All depend on Foundational completion; each story depends on the ones
  before it only where it touches the same files (`ShipmentEventService.cs`,
  `ShipmentEventController.cs`, `Program.cs`)
- **Polish (Phase 6)**: Depends on all implementation phases

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2). No dependency on other stories.
  Creates `ShipmentEventService.cs`, `ShipmentEventController.cs`, and the DI registrations that all
  later stories extend — so it MUST go first in file terms.
- **User Story 2 (P2)**: Depends on US1 files (adds methods to the same `ShipmentEventService.cs`/
  `ShipmentEventController.cs`). Independently testable once US1 exists — needs a shipment already
  `OutForDelivery`, which only US1 can produce.
- **User Story 3 (P3)**: Depends on US1 files; its two read endpoints are independently useful once any
  events exist (from US1 alone), though its Independent Test scenario benefits from US2 having also run
  (to exercise the `DeliveryAttempt` nested detail).

### Within Each User Story

- Entities/enums/DTOs always precede services; services precede controller actions; implementation
  precedes manual verification
- Core → Infrastructure → Services → Web dependency flow is never inverted (Constitution Principle II)
- Structural/conditional-field validation lives in FluentValidation validators; existence/active-status/
  role checks live in the Service; transition legality is delegated to the existing
  `ShipmentTransitionValidator` (research.md Decision 3); the delivery-attempt status gate is a plain
  equality check in the Service, not a transition check (research.md Decision 4)
- No automated tests exist in this project — "test" means running the quickstart.md scenarios via
  Swagger/HTTP (per-story Independent Test above)

### Parallel Opportunities

- All [P]-marked Core tasks (T001–T003, T005–T013) can run in parallel once their listed dependencies
  are met (distinct files)
- T016 and T017 (Infrastructure configs) can run in parallel once T004/T005 exist
- T019 can run in parallel with T016–T018 being finished, since it only needs T012 (interface) —
  though in practice T018 (AppDbContext) should land first so the DbSet exists before generating T021
- T023 (US1 validator) has no sibling to parallelize with in that story alone, but can be written in
  parallel with T027 (US2 validator) once both T006/T007 exist, since they are different files
- Unlike module 004 (two independent tracks), this module's three user stories are NOT independently
  parallelizable by different developers past Foundational — all three extend the same
  `ShipmentEventService.cs`/`ShipmentEventController.cs` files, so implementation is effectively
  single-threaded per the Dependencies above (same shape as module 006, Customer/Order — a single
  service/controller shared across every story)

---

## Parallel Example: Foundational

```bash
# At the start of Phase 2, in parallel:
Task: "Modify ShipmentStatus enum in ShipmentTracker.Core/Enums/ShipmentStatus.cs"
Task: "Modify ShipmentEventType enum in ShipmentTracker.Core/Enums/ShipmentEventType.cs"
Task: "Create DeliveryFailureReason enum in ShipmentTracker.Core/Enums/DeliveryFailureReason.cs"

# Once ShipmentEvent (T004) and DeliveryFailureReason (T003) exist, in parallel:
Task: "Create DeliveryAttempt entity in ShipmentTracker.Core/Entities/DeliveryAttempt.cs"
Task: "Create RegisterEventDto in ShipmentTracker.Core/DTOs/ShipmentEvents/RegisterEventDto.cs"
Task: "Create RegisterDeliveryAttemptDto in ShipmentTracker.Core/DTOs/ShipmentEvents/RegisterDeliveryAttemptDto.cs"
```

## Parallel Example: User Story 1 + User Story 2 validators

```bash
# Once RegisterEventDto (T006) and RegisterDeliveryAttemptDto (T007) both exist, in parallel:
Task: "Create RegisterEventDtoValidator in ShipmentTracker.Services/Validators/ShipmentEvents/RegisterEventDtoValidator.cs"
Task: "Create RegisterDeliveryAttemptDtoValidator in ShipmentTracker.Services/Validators/ShipmentEvents/RegisterDeliveryAttemptDtoValidator.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories; 22 tasks)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: run the US1 quickstart scenarios independently (a shipment can be marked out
   for delivery, with the Driver requirement and transition legality both enforced)
5. Deploy/demo if ready — an operator can already mark shipments out for delivery

### Incremental Delivery

1. Setup + Foundational → foundation ready (entities, configs, migration generated)
2. User Story 1 → test independently → demo (MVP)
3. User Story 2 → test independently → failed delivery attempts can be logged and sequenced correctly
4. User Story 3 → test independently → both the operational and public tracking views are live
5. Polish → apply migration, full quickstart validation, graphify update
6. Each story adds value without breaking previous stories

### Single-Developer Strategy (recommended)

Because `ShipmentEventService.cs`, `ShipmentEventController.cs`, and `Program.cs` are each single files
extended across stories, follow strict priority order (US1 → US3), completing the per-story Independent
Test after each phase before moving on.

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable against the quickstart.md scenarios
- Zero new NuGet packages (Constitution Principle III) — everything uses already-referenced EF Core,
  AutoMapper, FluentValidation
- `ShipmentEvents` migration is strictly additive (`AddColumn<>()` only) per the explicit constraint —
  no existing column is dropped, renamed, or retyped
- `Shipment.cs`, `ShipmentController.cs`, and `ShipmentService.cs` (module 001/002) are NOT modified by
  this feature; only `ShipmentStatus.cs`, `ShipmentEventType.cs`, and `ShipmentTransitionValidator.cs`
  are touched outside this feature's own new files
- All new/extended enums are persisted as strings (`HasConversion<string>()`); every DTO carrying one of
  these enums (input AND output) uses the per-property `[JsonConverter(typeof(JsonStringEnumConverter))]`
  — check both sides, per the lesson recorded in `CLAUDE.md` after module 006
- No pagination anywhere in this feature (research.md Decision 12) — both new `GET` endpoints return
  small, bounded, per-shipment collections in full
- Commit after each task or logical group; stop at any checkpoint to validate the story independently
