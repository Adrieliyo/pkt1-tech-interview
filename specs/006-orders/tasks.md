---

description: "Task list for the Orders module feature implementation"
---

# Tasks: Orders Module

**Input**: Design documents from `/specs/006-orders/`

**Prerequisites**: plan.md (required), spec.md (required — 6 user stories), research.md, data-model.md, contracts/orders-api-contract.md, quickstart.md

**Tests**: No automated test tasks — this project has no test project; validation is manual via Swagger/HTTP (quickstart.md), consistent with the rest of the solution.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story. Shared Core/Infrastructure layers are foundational and block all stories.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Solution**: existing 4-project layered solution — `ShipmentTracker.Core/`, `ShipmentTracker.Infrastructure/`, `ShipmentTracker.Services/`, `ShipmentTracker.Web/` at repo root
- **Docs**: feature docs under `specs/006-orders/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Verify the baseline and orient on existing module conventions before writing code

- [x] T001 Run `dotnet build ShipmentTracker.sln` and confirm the baseline builds cleanly with zero errors before any changes are made
- [x] T002 Orient on established conventions by reading `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs`, `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs`, `ShipmentTracker.Core/DTOs/PagedResult.cs`, `ShipmentTracker.Web/Controllers/ShipmentController.cs`, `ShipmentTracker.Web/Program.cs`, and one existing repository (e.g. `ShipmentTracker.Infrastructure/Repositories/ShipmentRepository.cs`) before starting Phase 2

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core entities/enums/DTOs/interfaces and the full Infrastructure layer — MUST be complete before ANY user story can be implemented

**CRITICAL**: No user story work can begin until this phase is complete

### Core layer (`ShipmentTracker.Core/`)

- [x] T003 [P] Create `OrderStatus` enum in `ShipmentTracker.Core/Enums/OrderStatus.cs` with members `Pending, Confirmed, Converted, Cancelled` (no attributes; persisted as string via EF conversion later)
- [x] T004 [P] Create `ServiceType` enum in `ShipmentTracker.Core/Enums/ServiceType.cs` with members `Standard, Express, Economy`
- [x] T005 [P] Create `PickupType` enum in `ShipmentTracker.Core/Enums/PickupType.cs` with members `HomePickup, DropOff`
- [x] T006 [P] Create `ShipmentEventType` enum in `ShipmentTracker.Core/Enums/ShipmentEventType.cs` with a single member `OrderConverted`
- [x] T007 [P] Create `Order` entity in `ShipmentTracker.Core/Entities/Order.cs` (namespace `ShipmentTracker.Core.Entities`) with ALL fields from `specs/006-orders/data-model.md`: `int Id`, `string OrderNumber`, `int CustomerId`, `int? OriginBranchId`, `OrderStatus Status`, `ServiceType ServiceType`, `PickupType PickupType`, `string? PickupAddress`, `DateTime? PickupScheduledAt`, `string RecipientName`, `string RecipientPhone`, `string RecipientAddress`, `string RecipientCity`, `string RecipientState`, `string RecipientZipCode`, `decimal DeclaredWeightKg`, `decimal DeclaredWidthCm`, `decimal DeclaredHeightCm`, `decimal DeclaredLengthCm`, `decimal QuotedPrice`, `string? Notes`, `DateTime CreatedAt`, `DateTime? UpdatedAt`
- [x] T008 [P] Modify `Shipment` entity in `ShipmentTracker.Core/Entities/Shipment.cs` — add exactly one new property `public int? OrderId { get; set; }` (nullable, per research.md Decision 2); no other field changes
- [x] T009 [P] Create `ShipmentEvent` entity in `ShipmentTracker.Core/Entities/ShipmentEvent.cs` (namespace `ShipmentTracker.Core.Entities`) with `int Id`, `int ShipmentId`, `Shipment Shipment` (forward navigation property, `null!`), `ShipmentEventType EventType`, `ShipmentStatus StatusSnapshot` (existing enum from module 001), `DateTime OccurredAt` — no reverse collection on `Shipment` (research.md Decision 12)
- [x] T010 [P] Create `CreateOrderDto` in `ShipmentTracker.Core/DTOs/Orders/CreateOrderDto.cs` (namespace `ShipmentTracker.Core.DTOs.Orders`) with nullable reference types as listed in data-model.md: `int CustomerId`, `int? OriginBranchId`, `ServiceType? ServiceType`, `PickupType? PickupType`, `string? PickupAddress`, `DateTime? PickupScheduledAt`, `string RecipientName`, `string RecipientPhone`, `string RecipientAddress`, `string RecipientCity`, `string RecipientState`, `string RecipientZipCode`, `decimal DeclaredWeightKg`, `decimal DeclaredWidthCm`, `decimal DeclaredHeightCm`, `decimal DeclaredLengthCm`, `decimal QuotedPrice`, `string? Notes` — NO `OrderNumber`/`Status` (system-assigned); nullable enums distinguish omitted (rejected) from invalid
- [x] T011 [P] Create `UpdateOrderDto` in `ShipmentTracker.Core/DTOs/Orders/UpdateOrderDto.cs` — identical fields to `CreateOrderDto` EXCEPT no `CustomerId` (ownership fixed at creation, research.md Decision 13)
- [x] T012 [P] Create `OrderDto` in `ShipmentTracker.Core/DTOs/Orders/OrderDto.cs` — all `Order` fields including `int Id`, `string OrderNumber`, `int CustomerId`, `int? OriginBranchId`, `OrderStatus Status`, `ServiceType ServiceType`, `PickupType PickupType`, `DateTime CreatedAt`, `DateTime? UpdatedAt`; apply `[JsonConverter(typeof(JsonStringEnumConverter))]` (from `System.Text.Json.Serialization`) per property on `Status`, `ServiceType`, and `PickupType` only — no global converter
- [x] T013 [P] Create `ConvertOrderResultDto` in `ShipmentTracker.Core/DTOs/Orders/ConvertOrderResultDto.cs` with `int ShipmentId` and `string TrackingNumber` (response for `POST /api/orders/{id}/convert`)
- [x] T014 [P] Create `IOrderRepository` in `ShipmentTracker.Core/Interfaces/Repositories/IOrderRepository.cs` as `public interface IOrderRepository : IBaseRepository<Order> { }` (empty — generic base covers everything, incl. `CountAsync` for number generation)
- [x] T015 [P] Create `IShipmentEventRepository` in `ShipmentTracker.Core/Interfaces/Repositories/IShipmentEventRepository.cs` as `public interface IShipmentEventRepository : IBaseRepository<ShipmentEvent> { }` (empty)
- [x] T016 [P] Create `IOrderService` in `ShipmentTracker.Core/Interfaces/Services/IOrderService.cs` with exactly these methods (signatures from data-model.md): `Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)`, `Task<PagedResult<OrderDto>> GetOrdersAsync(int? customerId = null, OrderStatus? status = null, int page = 1, int pageSize = 5)`, `Task<OrderDto?> GetOrderByIdAsync(int id)`, `Task<OrderDto?> GetOrderByNumberAsync(string orderNumber)`, `Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto dto)`, `Task<OrderDto?> ConfirmOrderAsync(int id)`, `Task<bool> CancelOrderAsync(int id)`, `Task<ConvertOrderResultDto?> ConvertToShipmentAsync(int id)` — add `using ShipmentTracker.Core.DTOs;` for `PagedResult<T>`
- [x] T017 [P] Modify `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs` — add two read-only properties `IOrderRepository OrderRepository { get; }` and `IShipmentEventRepository ShipmentEventRepository { get; }` (existing `ShipmentRepository`/`CustomerRepository`/`BranchRepository` are reused as-is; add `using` for the new repository interfaces if needed)

### Infrastructure layer (`ShipmentTracker.Infrastructure/`)

- [x] T018 [P] Create `OrderConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/OrderConfiguration.cs` implementing `IEntityTypeConfiguration<Order>`: `ToTable("Orders")`, identity PK; `OrderNumber` required `HasMaxLength(50)` + `HasIndex(...).IsUnique()`; `Status`/`ServiceType`/`PickupType` each `.IsRequired().HasConversion<string>()` (match `EmployeeConfiguration.cs` enum pattern); decimal columns with `HasPrecision(18, 2)` for `DeclaredWeightKg`/`DeclaredWidthCm`/`DeclaredHeightCm`/`DeclaredLengthCm`/`QuotedPrice`; `CustomerId` FK `HasOne(...).WithMany().HasForeignKey(x => x.CustomerId).IsRequired().OnDelete(DeleteBehavior.Restrict)`; `OriginBranchId` FK `HasOne(...).WithMany().HasForeignKey(x => x.OriginBranchId).IsRequired(false).OnDelete(DeleteBehavior.Restrict)` (unidirectional — no back-collection on `Customer`/`Branch`, per CLAUDE.md); `CreatedAt`/`UpdatedAt` configured
- [x] T019 [P] Create `ShipmentEventConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/ShipmentEventConfiguration.cs` implementing `IEntityTypeConfiguration<ShipmentEvent>`: `ToTable("ShipmentEvents")`, identity PK; `EventType`/`StatusSnapshot` each `.IsRequired().HasConversion<string>()`; `ShipmentId` FK `HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId).IsRequired().OnDelete(DeleteBehavior.Restrict)`; `OccurredAt` required
- [x] T020 [P] Modify `ShipmentConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/ShipmentConfiguration.cs` — add the nullable FK and its unique index: `builder.HasOne<Order>().WithMany().HasForeignKey(x => x.OrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);` plus `builder.HasIndex(x => x.OrderId).IsUnique();` (SQL Server unique index allows multiple NULLs — required for the 5 seeded Shipments and directly-created Shipments, research.md Decision 2). No other change to this file
- [x] T021 Modify `ShipmentTracker.Infrastructure/Data/AppDbContext.cs` — add `public DbSet<Order> Orders { get; set; }` and `public DbSet<ShipmentEvent> ShipmentEvents { get; set; }`, and register `builder.ApplyConfiguration(new OrderConfiguration());` + `builder.ApplyConfiguration(new ShipmentEventConfiguration());` in `OnModelCreating` (depends on T018, T019)
- [x] T022 [P] Create `OrderRepository` in `ShipmentTracker.Infrastructure/Repositories/OrderRepository.cs` as `public class OrderRepository : BaseRepository<Order>, IOrderRepository` with a `(AppDbContext context) : base(context)` constructor (copy `ShipmentRepository.cs` shape)
- [x] T023 [P] Create `ShipmentEventRepository` in `ShipmentTracker.Infrastructure/Repositories/ShipmentEventRepository.cs` as `public class ShipmentEventRepository : BaseRepository<ShipmentEvent>, IShipmentEventRepository` with a `(AppDbContext context) : base(context)` constructor
- [x] T024 Modify `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs` — add private fields `_orderRepository`/`_shipmentEventRepository` and lazy read-only properties `public IOrderRepository OrderRepository => _orderRepository ??= new OrderRepository(_context);` and `public IShipmentEventRepository ShipmentEventRepository => _shipmentEventRepository ??= new ShipmentEventRepository(_context);` (depends on T022, T023)
- [x] T025 Generate the EF Core migration by running `dotnet ef migrations add AddOrdersAndShipmentEvents --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` from the repo root, then review the generated file under `ShipmentTracker.Infrastructure/Migrations/` to confirm it creates the `Orders` and `ShipmentEvents` tables, the unique index on `Orders.OrderNumber`, the unique-allowing-null index on `Shipments.OrderId`, the nullable `Shipments.OrderId` column, and the three Restrict FKs (Orders→Customers, Orders→Branches, ShipmentEvents→Shipments) — the 5 seeded Shipment rows must remain untouched (implicit `OrderId = NULL`). Depends on all of T003–T024. NOTE: it is normal for `dotnet ef` to build the solution; fix any compile errors in T003–T024 before this passes

**Checkpoint**: Foundation ready — entities, DTOs, interfaces, configs, repositories, UnitOfWork, and migration all in place. User story implementation can now begin. Do not apply the migration to the database yet (done in the Polish phase).

---

## Phase 3: User Story 1 - Create a new order (Priority: P1) — MVP

**Goal**: A user creates an Order for an active Customer with `DropOff` (valid active origin Branch) or `HomePickup` (pickup address + future scheduled datetime), receiving a `Pending` Order with a system-generated unique `ORD-YYYYMMDD-XXXX` order number. All structural and cross-field consistency rules are enforced.

**Independent Test**: Submit `POST /api/orders` with `pickupType: "DropOff"` + a valid active `originBranchId` → `201` with `status: "Pending"` and `orderNumber` matching `ORD-{today}-XXXX`; and a second with `pickupType: "HomePickup"` + `pickupAddress` + future `pickupScheduledAt` → `201`. Then verify the rejection scenarios: missing/inactive/nonexistent origin Branch on DropOff → `400`; missing/past scheduled datetime on HomePickup → `400`; DropOff that also supplies `pickupAddress` (and HomePickup that supplies `originBranchId`) → `400`; nonexistent/inactive Customer → `400`; zero/negative weight or any dimension → `400`; invalid `serviceType`/`pickupType` → `400`. (spec.md US1 acceptance scenarios 1–8; quickstart.md §2 US1)

### Implementation for User Story 1

- [x] T026 [P] [US1] Create `CreateOrderDtoValidator` in `ShipmentTracker.Services/Validators/Orders/CreateOrderDtoValidator.cs` as `AbstractValidator<CreateOrderDto>` — pure structural rules, no repository access (research.md Decision 9): `RecipientName`/`RecipientPhone`/`RecipientAddress`/`RecipientCity`/`RecipientState`/`RecipientZipCode` `NotEmpty()`; `ServiceType` and `PickupType` `NotNull().IsInEnum()`; `DeclaredWeightKg`/`DeclaredWidthCm`/`DeclaredHeightCm`/`DeclaredLengthCm` each `GreaterThan(0)` (FR-005); `QuotedPrice` `GreaterThanOrEqualTo(0)`; conditional rules — When `PickupType == HomePickup`: `PickupAddress` not empty, `PickupScheduledAt` not null and `GreaterThan(DateTime.UtcNow)`, and `OriginBranchId` must be null; When `PickupType == DropOff`: `OriginBranchId` not null, and `PickupAddress`/`PickupScheduledAt` must both be null (FR-002/FR-003). Error messages in English matching the contract (e.g. `"PickupType HomePickup requires a pickup address and a scheduled pickup datetime."`)
- [x] T027 [P] [US1] Create `OrderMappingProfile` in `ShipmentTracker.Services/Mappings/OrderMappingProfile.cs` as `public class OrderMappingProfile : Profile` with a single output-only map `CreateMap<Order, OrderDto>();` (copy `ShipmentMappingProfile.cs` shape; never map into entity construction)
- [x] T028 [US1] Create `OrderService` in `ShipmentTracker.Services/OrderService.cs` implementing `IOrderService`, starting with `CreateOrderAsync` (constructor injects `IUnitOfWork`, `IValidator<CreateOrderDto>`, `IMapper`): (1) trim `RecipientName`/`RecipientPhone`/`RecipientAddress`/`RecipientCity`/`RecipientState`/`RecipientZipCode`/`PickupAddress`/`Notes` with the `dto.X = dto.X?.Trim() ?? string.Empty;` convention; (2) run `_createValidator.ValidateAsync(dto)` collecting structural failures; (3) DB-dependent checks via `_unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.Id == dto.CustomerId)` (reject if `null` or `!IsActive`, FR-001) and, only when `PickupType == DropOff`, `_unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == dto.OriginBranchId)` (reject if `null` or `!IsActive`, FR-002) — merge all failures into one `FluentValidation.ValidationException` and throw before any write; (4) generate order number `ORD-{yyyyMMdd}-{count:D4}` where `count = await _unitOfWork.OrderRepository.CountAsync(x => x.CreatedAt >= todayUtc && x.CreatedAt < todayUtc.AddDays(1)) + 1` (suffix grows past 4 digits rather than truncating, per spec.md Edge Cases); (5) hand-build the `Order` entity (no `_mapper.Map`): `Status = OrderStatus.Pending`, `CreatedAt = DateTime.UtcNow`, `UpdatedAt = null`; (6) `AddAsync` + single `await _unitOfWork.CommitAsync()`; (7) return `_mapper.Map<OrderDto>(order)`. Use English validation messages per the contract. (depends on T026, T027)
- [x] T029 [US1] Register DI in `ShipmentTracker.Web/Program.cs` — add `builder.Services.AddScoped<IOrderRepository, OrderRepository>();`, `builder.Services.AddScoped<IShipmentEventRepository, ShipmentEventRepository>();`, `builder.Services.AddScoped<IOrderService, OrderService>();`, and `builder.Services.AddScoped<IValidator<CreateOrderDto>, CreateOrderDtoValidator>();` alongside the existing registrations (add `using` statements as needed) (depends on T026, T028)
- [x] T030 [US1] Create `OrderController` in `ShipmentTracker.Web/Controllers/OrderController.cs` with `[Route("api/orders")]`, `[ApiController]`, `[Produces("application/json")]`, constructor-injected `IOrderService`, and XML doc comments on the class and every action (Spanish, matching existing controllers). Implement `POST /api/orders` (HTTP 201 with `CreatedAtAction` `Location` header pointing to `GET /api/orders/{id}`; catch `FluentValidation.ValidationException` → `400 BadRequest(new { errors = ex.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage }) })`). Use English `NotFound` message shape per the contract. (depends on T028, T029)

**Checkpoint**: User Story 1 fully functional and independently testable — orders can be created with both pickup types, all rejection paths return `400` with the documented shape, and order numbers are unique and correctly formatted.

---

## Phase 4: User Story 2 - Find and review orders (Priority: P2)

**Goal**: An operator lists the order queue with optional `customerId`/`status` filters (paginated, most recent first) and retrieves a single Order by id or by human-readable order number.

**Independent Test**: Create orders in multiple statuses (via US1 + US3/US5), then `GET /api/orders` with no filters (all returned, newest first), `?status=Pending` (only Pending), `?customerId={id}` (only that Customer's), `GET /api/orders/{id}` and `GET /api/orders/number/{orderNumber}` (full detail), and `GET /api/orders/999999` / unknown number (`404`). (spec.md US2 scenarios 1–5; quickstart.md §2 US2)

### Implementation for User Story 2

- [x] T031 [US2] Add `GetOrdersAsync`, `GetOrderByIdAsync`, and `GetOrderByNumberAsync` to `ShipmentTracker.Services/OrderService.cs` (same file as T028 — do not create a second service): `GetOrdersAsync` builds a `Expression<Func<Order, bool>> filter` combining optional `customerId` and `status` predicates (null when unfiltered), uses `private const int MaxPageSize = 50;` + `Math.Min(pageSize, MaxPageSize)` clamp, `long skip` with the `if (skip > int.MaxValue)` empty-result guard, `GetAsync(filter, orderBy: q => q.OrderByDescending(x => x.CreatedAt), skip, take)` + `CountAsync(filter)`, returning `PagedResult<OrderDto>` (copy `ShipmentService.GetShipmentsAsync` shape exactly); `GetOrderByIdAsync` uses `GetByIdAsync(id)` returning null when missing; `GetOrderByNumberAsync` uses `SingleOrDefaultAsync(x => x.OrderNumber == orderNumber)`; both map to `OrderDto` and return null when not found. (depends on T028)
- [x] T032 [US2] Add to `ShipmentTracker.Web/Controllers/OrderController.cs`: `GET /api/orders` with `[FromQuery] int? customerId`, `[FromQuery] OrderStatus? status`, `[FromQuery, Range(1, int.MaxValue)] int page = 1`, `[FromQuery, Range(1, int.MaxValue)] int pageSize = 5` — sets the 4 pagination headers (`X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`) and returns `200 Ok(result.Items)` (unrecognized `status` enum name → automatic `400` via model binding); `GET /api/orders/{id}` and `GET /api/orders/number/{orderNumber}` returning `200` with `OrderDto` or `404 NotFound(new { message = $"No order was found with id '{id}'." })` / `"...with number '{orderNumber}'."` per the contract. Add XML doc comments to each new action. (depends on T031)

**Checkpoint**: User Stories 1 AND 2 both work independently — the operator can create, list (with filters), and drill into orders.

---

## Phase 5: User Story 3 - Confirm a pending order (Priority: P3)

**Goal**: An operator confirms a `Pending` Order, locking it as `Confirmed` so it becomes eligible for conversion.

**Independent Test**: `POST /api/orders/{id}/confirm` on a `Pending` order → `200` with `status: "Confirmed"`; repeating the same call → `400`; `PUT` on the now-`Confirmed` order → `400`. (spec.md US3 scenarios 1–2; quickstart.md §2 US3)

### Implementation for User Story 3

- [x] T033 [US3] Add `ConfirmOrderAsync` to `ShipmentTracker.Services/OrderService.cs`: load the order by id (null → return null, caller maps to `404`); if `order.Status != OrderStatus.Pending` throw `InvalidOperationException("Only pending orders can be confirmed.")` (status-transition guard — `InvalidOperationException` → `400 { message }`, NOT a `ValidationException`, research.md Decision 10, matching `ShipmentService.UpdateShipmentStatusAsync` precedent); set `Status = OrderStatus.Confirmed` and `UpdatedAt = DateTime.UtcNow`; `Update` + single `CommitAsync`; return `_mapper.Map<OrderDto>(order)`. (depends on T028)
- [x] T034 [US3] Add `POST /api/orders/{id}/confirm` to `ShipmentTracker.Web/Controllers/OrderController.cs`: `200 Ok(OrderDto)`; when service returns null → `404 NotFound(new { message = $"No order was found with id '{id}'." })`; catch `InvalidOperationException` → `400 BadRequest(new { message = ex.Message })`. Add XML doc comments. (depends on T033)

**Checkpoint**: User Story 3 complete — orders can be confirmed and the confirm/update lock behavior matches the contract.

---

## Phase 6: User Story 4 - Convert a confirmed order to a shipment (Priority: P4) — central operation

**Goal**: An operator converts a `Confirmed` Order into an actual Shipment: generates a unique `TRK-YYYYMMDD-XXXX` tracking number, creates the Shipment (minimal existing shape + `OrderId` back-reference, `Recipient` from the Order's `RecipientName`), records its first `OrderConverted` `ShipmentEvent`, marks the Order `Converted`, and commits all three writes atomically in a single `CommitAsync()`.

**Independent Test**: `POST /api/orders/{id}/convert` on a `Confirmed` order → `200` with `{ shipmentId, trackingNumber }`, tracking number matches `TRK-{today}-XXXX`; `GET /api/shipment/{trackingNumber}` shows the new Shipment (`status: "Collected"`, `recipient` = order's `recipientName`); `GET /api/orders/{id}` shows `status: "Converted"`; a second convert → `400` with no second Shipment; convert on a `Pending`/`Cancelled` order → `400`; two conversions on the same day produce sequential, independent suffixes. (spec.md US4 scenarios 1–4; quickstart.md §2 US4)

### Implementation for User Story 4

- [x] T035 [US4] Add `ConvertToShipmentAsync` to `ShipmentTracker.Services/OrderService.cs` (plan input steps 1–7, data-model.md flow): load order by id (null → return null, caller maps to `404`); if `order.Status != OrderStatus.Confirmed` throw `InvalidOperationException("Only confirmed orders can be converted to a shipment.")`; generate tracking number via `_unitOfWork.ShipmentRepository.CountAsync(x => x.CreatedAt >= todayUtc && x.CreatedAt < todayUtc.AddDays(1)) + 1` formatted `TRK-{yyyyMMdd}-{count:D4}` (independent sequence from orders, FR-016); hand-build `shipment = new Shipment { TrackingNumber, Recipient = order.RecipientName, Status = ShipmentStatus.Collected, CreatedAt = DateTime.UtcNow, OrderId = order.Id }` (FR-017, research.md Decisions 1/3/7); build `shipmentEvent = new ShipmentEvent { Shipment = shipment, EventType = ShipmentEventType.OrderConverted, StatusSnapshot = ShipmentStatus.Collected, OccurredAt = DateTime.UtcNow }` — assign the `Shipment` navigation property, NOT a guessed `ShipmentId` (research.md Decision 12); set `order.Status = OrderStatus.Converted` and `order.UpdatedAt = DateTime.UtcNow`; `AddAsync(shipment)`, `AddAsync(shipmentEvent)`, `Update(order)`, then a SINGLE `await _unitOfWork.CommitAsync()` — atomicity from EF Core's implicit per-`SaveChanges` transaction, no explicit transaction API (research.md Decision 11, FR-019); return `new ConvertOrderResultDto { ShipmentId = shipment.Id, TrackingNumber = shipment.TrackingNumber }` (both populated post-commit). (depends on T028, T033)
- [x] T036 [US4] Add `POST /api/orders/{id}/convert` to `ShipmentTracker.Web/Controllers/OrderController.cs`: `200 Ok(ConvertOrderResultDto)`; null service result → `404 NotFound(new { message = $"No order was found with id '{id}'." })`; catch `InvalidOperationException` → `400 BadRequest(new { message = ex.Message })`. Add XML doc comments. (depends on T035)

**Checkpoint**: All of US1–US4 complete — the full create → confirm → convert workflow (SC-001) works end-to-end atomically.

---

## Phase 7: User Story 5 - Cancel a pending order (Priority: P5)

**Goal**: A user cancels a `Pending` Order; it becomes `Cancelled` (terminal), is retained (never deleted), and can no longer be edited/confirmed/converted.

**Independent Test**: `DELETE /api/orders/{id}` on a `Pending` order → `204` and the order's status becomes `Cancelled`; repeating the `DELETE` → `400` (NOT idempotent — `Cancelled` is terminal, unlike other modules' soft-delete); `GET /api/orders/{id}` still returns the cancelled order's full original detail; `DELETE` on a `Confirmed` order → `400`. (spec.md US5 scenarios 1–3; quickstart.md §2 US5)

### Implementation for User Story 5

- [x] T037 [US5] Add `CancelOrderAsync` to `ShipmentTracker.Services/OrderService.cs`: load order by id (null → return false, caller maps to `404`); if `order.Status != OrderStatus.Pending` throw `InvalidOperationException("Only pending orders can be cancelled.")`; set `Status = OrderStatus.Cancelled` and `UpdatedAt = DateTime.UtcNow`; `Update` + single `CommitAsync`; return `true`. The row is never removed (FR-007). (depends on T028)
- [x] T038 [US5] Add `DELETE /api/orders/{id}` to `ShipmentTracker.Web/Controllers/OrderController.cs`: `204 NoContent` on success; when service returns false → `404 NotFound(new { message = $"No order was found with id '{id}'." })`; catch `InvalidOperationException` → `400 BadRequest(new { message = ex.Message })`. Add XML doc comments. (depends on T037)

**Checkpoint**: User Story 5 complete — the cancellation path works and the non-idempotency contract is enforced.

---

## Phase 8: User Story 6 - Update a pending order (Priority: P6)

**Goal**: A user corrects an Order's recipient, destination, pickup information, or package dimensions/weight while it is still `Pending`; the same creation rules (including `HomePickup`/`DropOff` consistency and positive dimensions/weight) are re-validated, and updates are rejected once the Order is not `Pending`.

**Independent Test**: `PUT /api/orders/{id}` on a `Pending` order changing `declaredWeightKg` and `recipientAddress` → `200` and the changes persist; switching `pickupType` from `DropOff` to `HomePickup` (supplying `pickupAddress`/`pickupScheduledAt`, omitting `originBranchId`) → `200`; an inconsistent combination (e.g. `HomePickup` with no `pickupAddress`) → `400` with the order unchanged; `PUT` on a `Confirmed`/`Converted`/`Cancelled` order → `400` with the order unchanged. (spec.md US6 scenarios 1–3; quickstart.md §2 US6)

### Implementation for User Story 6

- [x] T039 [P] [US6] Create `UpdateOrderDtoValidator` in `ShipmentTracker.Services/Validators/Orders/UpdateOrderDtoValidator.cs` — identical structural and conditional rules to `CreateOrderDtoValidator` (T026) applied to `UpdateOrderDto` (no `CustomerId` involved); reuse the same rule set verbatim against the `UpdateOrderDto` type. (can be written in parallel with T026/T027 since it is a separate file; depends on T011)
- [x] T040 [US6] Add `UpdateOrderAsync` to `ShipmentTracker.Services/OrderService.cs` (data-model.md flow): load order by id (null → return null, caller maps to `404`); if `order.Status != OrderStatus.Pending` throw `InvalidOperationException("Only pending orders can be edited.")`; trim string fields with the `?? string.Empty` convention; run `_updateValidator.ValidateAsync(dto)`; DB-dependent checks with the order's existing `CustomerId` (customer must still exist and be `IsActive`) and, only when the new `PickupType == DropOff`, the `OriginBranchId` (branch must exist and be active) — merge all failures into one `FluentValidation.ValidationException` thrown before any write (entity left unchanged); apply all editable fields (origin/recipient/destination/pickup/dimensions/weight/quotedPrice/notes/serviceType — NOT `CustomerId`, NOT `OrderNumber`); set `UpdatedAt = DateTime.UtcNow`; `Update` + single `CommitAsync`; return `_mapper.Map<OrderDto>(order)`. (depends on T028, T039)
- [x] T041 [US6] Register `builder.Services.AddScoped<IValidator<UpdateOrderDto>, UpdateOrderDtoValidator>();` in `ShipmentTracker.Web/Program.cs` alongside the T029 registrations. (depends on T039)
- [x] T042 [US6] Add `PUT /api/orders/{id}` to `ShipmentTracker.Web/Controllers/OrderController.cs`: `200 Ok(OrderDto)`; null service result → `404 NotFound(new { message = $"No order was found with id '{id}'." })`; catch `FluentValidation.ValidationException` → `400 BadRequest(new { errors = ... })` and `InvalidOperationException` → `400 BadRequest(new { message = ex.Message })` (both shapes per the contract). Add XML doc comments. (depends on T040, T041)

**Checkpoint**: All six user stories complete — the full order lifecycle (create → review → confirm → convert, plus cancel and update paths) is implemented and each is independently testable.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Apply the migration, verify the whole solution, validate against the manual scenarios, and keep the knowledge graph current

- [X] T043 Run `dotnet build ShipmentTracker.sln` and fix any build warnings or errors introduced by the module (verify: no warnings from unused usings, nullable annotations, or missing XML doc comments)
- [X] T044 Apply the migration to the local database: `dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` — confirm `Orders`, `ShipmentEvents` tables and the `Shipments.OrderId` column exist and the 5 pre-existing seeded Shipments are untouched (`OrderId = NULL`). (depends on T043)
- [X] T045 Execute every scenario in `specs/006-orders/quickstart.md` against the running API (`dotnet run --project ShipmentTracker.Web`), including the §3 "verify nothing else changed" checks (`/api/shipment` direct create still works and returns `orderId: null`), and confirm the pre-existing `GET/POST/PATCH /api/shipment`, `/api/branches`, `/api/employees`, `/api/vehicles`, `/api/customers` behaviors are unchanged. (depends on T043, T044)
- [X] T046 Run `graphify update .` from the repo root so the knowledge graph reflects the new module (per CLAUDE.md — AST-only, no API cost). (depends on all code tasks)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **User Stories (Phases 3–8)**: All depend on Foundational completion; each story depends on the ones before it only where it touches the same files (`OrderService.cs`, `OrderController.cs`, `Program.cs`)
- **Polish (Phase 9)**: Depends on all implementation phases

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2). No dependencies on other stories. Creates `OrderService.cs`, `OrderController.cs`, and the DI registrations that all later stories extend — so it MUST go first in file terms.
- **User Story 2 (P2)**: Depends on US1 (adds methods to the same `OrderService.cs`/`OrderController.cs` files). Independently testable once US1 exists.
- **User Story 3 (P3)**: Depends on US1 files; adds `ConfirmOrderAsync` + confirm action.
- **User Story 4 (P4)**: Depends on US1 + US3 (convert requires `Confirmed` orders to test end-to-end); adds `ConvertToShipmentAsync` + convert action.
- **User Story 5 (P5)**: Depends on US1 files; adds `CancelOrderAsync` + DELETE action.
- **User Story 6 (P6)**: Depends on US1 files; adds `UpdateOrderAsync` + PUT action. Its validator (`T039`) can be written in parallel with US1's validator/mapping tasks.

### Within Each User Story

- Models/entities/DTOs always precede services; services precede controller actions; implementation precedes manual verification
- Core → Infrastructure → Services → Web dependency flow is never inverted (Constitution Principle II)
- Structural/conditional-field validation lives in FluentValidation validators; existence/active-status checks live in the Service; status-transition guards use `InvalidOperationException` → `400 { message }` (research.md Decision 10)
- No automated tests exist in this project — "test" means running the quickstart.md scenarios via Swagger/HTTP (per-story Independent Test above)

### Parallel Opportunities

- All [P]-marked foundational Core tasks (T003–T017) can run in parallel (distinct files)
- All [P]-marked Infrastructure tasks (T018–T020, T022–T023) can run in parallel, and the whole Infrastructure set can run in parallel with the Core set once the entity/enum files exist
- T021 and T024 depend on the config/repository files they reference — run them after those finish
- T026 and T027 (US1 validator + mapping profile) and T039 (US6 validator) can all be written in parallel — three distinct files
- After US1, US3/US5/US6 add methods to `OrderService.cs`/`OrderController.cs` — these are sequential on those files, so a single developer completes stories in priority order; a team can still parallelize US2's independent method additions (T031) with US3 (T033) only if they merge file edits carefully — otherwise follow priority order P1 → P6

---

## Parallel Example: User Story 1

```bash
# Launch all file-independent artifacts together:
Task: "T026 [P] [US1] CreateOrderDtoValidator in ShipmentTracker.Services/Validators/Orders/CreateOrderDtoValidator.cs"
Task: "T027 [P] [US1] OrderMappingProfile in ShipmentTracker.Services/Mappings/OrderMappingProfile.cs"

# Then the sequential chain on shared files:
Task: "T028 [US1] OrderService.CreateOrderAsync in ShipmentTracker.Services/OrderService.cs"
Task: "T029 [US1] DI registrations in ShipmentTracker.Web/Program.cs"
Task: "T030 [US1] POST /api/orders in ShipmentTracker.Web/Controllers/OrderController.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL — blocks all stories; 23 tasks)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: run the US1 quickstart scenarios independently (orders created as `Pending` with correct `ORD-YYYYMMDD-XXXX` numbers; all rejection paths return `400`)
5. Deploy/demo if ready — an operator can already register orders

### Incremental Delivery

1. Setup + Foundational → foundation ready (entities, configs, migration generated)
2. User Story 1 → test independently → demo (MVP)
3. User Story 2 → test independently → operators can review the queue
4. User Story 3 → test independently → confirm locks orders
5. User Story 4 → test independently → create → confirm → convert workflow live (SC-001, SC-003)
6. User Story 5 → test independently → cancellation path
7. User Story 6 → test independently → Pending-order corrections
8. Polish → apply migration, full quickstart validation, graphify update
9. Each story adds value without breaking previous stories

### Single-Developer Strategy (recommended)

Because `OrderService.cs`, `OrderController.cs`, and `Program.cs` are each single files extended across stories, follow strict priority order (US1 → US6), completing the per-story Independent Test after each phase before moving on.

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story is independently completable and testable against the quickstart.md scenarios
- Zero new NuGet packages (Constitution Principle III) — everything uses already-referenced EF Core, AutoMapper, FluentValidation
- `Shipment` is touched by exactly one nullable column (`OrderId`, T008/T020) and nothing else; `Branch`/`Customer`/`Employee`/`Vehicle` are not modified at all
- All new enums are persisted as strings (`HasConversion<string>()`); `OrderDto` enum fields use per-property `[JsonConverter(typeof(JsonStringEnumConverter))]` only
- Order/tracking number generation is `COUNT`-based per calendar day with an accepted, documented race condition (research.md Decision 8) — do not add locking/sequence machinery
- Orders are never deleted — `Cancelled`/`Converted` are permanent retained states (FR-007)
- Error message language for this module is ENGLISH per `contracts/orders-api-contract.md` (unlike the older Spanish-message modules)
- Commit after each task or logical group; stop at any checkpoint to validate the story independently