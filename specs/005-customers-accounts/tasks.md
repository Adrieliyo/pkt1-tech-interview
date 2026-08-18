---

description: "Task list template for feature implementation"
---

# Tasks: Customers & Accounts Module

**Input**: Design documents from `/specs/005-customers-accounts/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/customers-api-contract.md, quickstart.md

**Tests**: No automated test tasks — this project has no test project (see the constitution); manual
verification via `quickstart.md`, same policy as `001`, `002`, `003`, and `004`.

**Organization**: Grouped by user story, with a Foundational phase up front. Unlike module 004
(`Employee`/`Vehicle` — two independent aggregates), `Customer` is a single track: `Individual` and
`Business` are two concrete subtypes of one TPT hierarchy sharing `ICustomerService`/`CustomerService`/
`CustomerController`, so every user story after Foundational extends those same three files with a
new method/action rather than creating a parallel set. User Story 1 (Register) covers *both* creation
endpoints together, matching `spec.md`'s own User Story 1, which tests Individual and Business
creation as one story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different file, no dependency on an incomplete task)
- **[Story]**: US1 to US4
- Each task includes the exact file path

## Path Conventions

Existing layered .NET solution (`ShipmentTracker.Core` / `.Infrastructure` / `.Services` / `.Web`, see
`plan.md`). Paths are relative to the repository root.

---

## Phase 1: Setup

**N/A.** No project initialization or new packages (research.md: zero new NuGet dependencies). The
solution and all four projects already exist.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Persistence, repository, mapping, and DI wiring shared by all 4 user stories — including
the single migration that creates `Customers`/`IndividualCustomers`/`BusinessCustomers` together.

- [X] T001 [P] Create enum `CustomerType` in `ShipmentTracker.Core/Enums/CustomerType.cs`: values `Individual`, `Business`, with an XML doc comment (same style as `BranchType.cs`)
- [X] T002 Create abstract entity `Customer` in `ShipmentTracker.Core/Entities/Customer.cs`: `Id` (int), `Type` (`CustomerType`), `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `IsActive` (bool), `CreatedAt` (`DateTime`), `UpdatedAt` (`DateTime?`) (depends on T001)
- [X] T003 [P] Create entity `IndividualCustomer` in `ShipmentTracker.Core/Entities/IndividualCustomer.cs`: `: Customer` — `FirstName`, `LastName`, `BirthDate` (`DateOnly?`), `GovernmentId` (depends on T002; can be done in parallel with T004)
- [X] T004 [P] Create entity `BusinessCustomer` in `ShipmentTracker.Core/Entities/BusinessCustomer.cs`: `: Customer` — `BusinessName`, `TaxId`, `LegalRepresentative`, `Industry` (`string?`), `CreditLimit` (`decimal?`) (depends on T002; can be done in parallel with T003)
- [X] T005 [P] Create `IndividualDetailDto` in `ShipmentTracker.Core/DTOs/Customers/IndividualDetailDto.cs`: `FirstName`, `LastName`, `BirthDate?`, `GovernmentId` — nested output shape (data-model.md)
- [X] T006 [P] Create `BusinessDetailDto` in `ShipmentTracker.Core/DTOs/Customers/BusinessDetailDto.cs`: `BusinessName`, `TaxId`, `LegalRepresentative`, `Industry?`, `CreditLimit?` — nested output shape
- [X] T007 Create `CustomerDetailDto` in `ShipmentTracker.Core/DTOs/Customers/CustomerDetailDto.cs`: `Id`, `Type` (with `[JsonConverter(typeof(JsonStringEnumConverter))]`, research.md Decision 12), `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `IsActive`, `CreatedAt`, `UpdatedAt?`, `Individual` (`IndividualDetailDto?`), `Business` (`BusinessDetailDto?`) — discriminated-union output DTO reused for every endpoint (depends on T001, T005, T006)
- [X] T008 [P] Create `ICustomerRepository` in `ShipmentTracker.Core/Interfaces/Repositories/ICustomerRepository.cs`: `: IBaseRepository<Customer>`, no extra methods (data-model.md, research.md Decision 10) (depends on T002)
- [X] T009 [P] In `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs`: add `ICustomerRepository CustomerRepository { get; }` (same pattern as `BranchRepository`/`EmployeeRepository`) (depends on T008)
- [X] T010 [P] Create `CustomerConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/CustomerConfiguration.cs`: `ToTable("Customers")`, `HasKey(Id)` with `UseIdentityColumn()`, `Type` with `HasConversion<string>()`, `Email` required (`HasMaxLength(255)`) + `HasIndex(x => x.Email).IsUnique()` (no `IsActive` filter, research.md Decision 7), `Phone`/`Address`/`City`/`State`/`ZipCode`/`Country` required, `IsActive`/`CreatedAt` required, `UpdatedAt` optional (depends on T002)
- [X] T011 [P] Create `IndividualCustomerConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/IndividualCustomerConfiguration.cs`: `ToTable("IndividualCustomers")`, `FirstName`/`LastName` required, `BirthDate` optional, `GovernmentId` required (`HasMaxLength(18)`) + `HasIndex(...).IsUnique()` without filter (depends on T003)
- [X] T012 [P] Create `BusinessCustomerConfiguration` in `ShipmentTracker.Infrastructure/Data/Configurations/BusinessCustomerConfiguration.cs`: `ToTable("BusinessCustomers")`, `BusinessName`/`LegalRepresentative` required, `TaxId` required (`HasMaxLength(12)` — corrected from the plan input's 13, research.md Decision 5) + `HasIndex(...).IsUnique()` without filter, `Industry` optional, `CreditLimit` optional (`HasColumnType("decimal(18,2)")`) (depends on T004)
- [X] T013 In `ShipmentTracker.Infrastructure/Data/AppDbContext.cs`: add `DbSet<Customer> Customers`, `DbSet<IndividualCustomer> IndividualCustomers`, `DbSet<BusinessCustomer> BusinessCustomers`, and in `OnModelCreating` add `builder.ApplyConfiguration(new CustomerConfiguration())`, `new IndividualCustomerConfiguration()`, `new BusinessCustomerConfiguration()` (depends on T010, T011, T012)
- [X] T014 Create `CustomerRepository` in `ShipmentTracker.Infrastructure/Repositories/CustomerRepository.cs`: `: BaseRepository<Customer>, ICustomerRepository`, constructor forwarding to the base (same pattern as `BranchRepository`, no extra methods) (depends on T008, T013)
- [X] T015 In `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs`: add private field `_customerRepository` and lazy property `CustomerRepository` (same pattern as `EmployeeRepository`) (depends on T009, T014)
- [X] T016 Generate the EF Core migration: `dotnet ef migrations add AddCustomers --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` — creates `Customers`/`IndividualCustomers`/`BusinessCustomers` with their TPT PK/FK relationship and the three unique indexes; does not modify any existing migration (depends on T013)
- [X] T017 [P] Create `CustomerMappingProfile` in `ShipmentTracker.Services/Mappings/CustomerMappingProfile.cs`: `CreateMap<Customer, CustomerDetailDto>().Include<IndividualCustomer, CustomerDetailDto>().Include<BusinessCustomer, CustomerDetailDto>()`; `CreateMap<IndividualCustomer, CustomerDetailDto>().AfterMap((src, dest) => dest.Individual = new IndividualDetailDto { ... })`; `CreateMap<BusinessCustomer, CustomerDetailDto>().AfterMap((src, dest) => dest.Business = new BusinessDetailDto { ... })` (research.md Decision 9 — output-only, no reverse mapping) (depends on T002, T003, T004, T007)
- [X] T018 In `ShipmentTracker.Web/Program.cs`: add `builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();` (same pattern as the existing `IBranchRepository`/`IEmployeeRepository` registrations) (depends on T014)

**Checkpoint**: Persistence, repository, `IUnitOfWork.CustomerRepository`, and output mapping are
ready. No story has a service, validator, or controller action yet — that starts in Phase 3.

---

## Phase 3: User Story 1 - Register a new customer (Priority: P1) 🎯 MVP

**Goal**: `POST /api/customers/individual` and `POST /api/customers/business` each create an active
customer of their respective type, with a globally-unique `email` and a type-scoped-unique
`governmentId`/`taxId` matching its official format, rejecting any invalid or duplicate data with
`400`.

**Independent Test**: `POST /api/customers/individual` with a valid body returns `201` with
`isActive: true` and `individual` populated; `POST /api/customers/business` likewise with `business`
populated. Each `POST` repeated with a duplicate `email`, a duplicate type-scoped identifier, a
malformed `governmentId`/`taxId`, or a missing required field returns `400` (scenarios 1-6 of
`quickstart.md`, User Story 1).

### Implementation for User Story 1

- [X] T019 [P] [US1] Create `CreateIndividualCustomerDto` in `ShipmentTracker.Core/DTOs/Customers/CreateIndividualCustomerDto.cs`: `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `FirstName`, `LastName`, `BirthDate?`, `GovernmentId` — no `Type` (implied by the endpoint), no `IsActive` (always `true` at creation) (depends on T001)
- [X] T020 [P] [US1] Create `CreateIndividualCustomerDtoValidator` in `ShipmentTracker.Services/Validators/Customers/CreateIndividualCustomerDtoValidator.cs` (`AbstractValidator<CreateIndividualCustomerDto>`): shared fields not empty, `Email` not empty + `EmailAddress()`, `FirstName`/`LastName` not empty, `GovernmentId` not empty + `Matches(curpPattern)` (18-character CURP structure, research.md Decision 6) — structural rules only (depends on T019)
- [X] T021 [P] [US1] Create `CreateBusinessCustomerDto` in `ShipmentTracker.Core/DTOs/Customers/CreateBusinessCustomerDto.cs`: `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `BusinessName`, `TaxId`, `LegalRepresentative`, `Industry?`, `CreditLimit?` (depends on T001)
- [X] T022 [P] [US1] Create `CreateBusinessCustomerDtoValidator` in `ShipmentTracker.Services/Validators/Customers/CreateBusinessCustomerDtoValidator.cs` (`AbstractValidator<CreateBusinessCustomerDto>`): shared fields not empty, `Email` not empty + `EmailAddress()`, `BusinessName`/`LegalRepresentative` not empty, `TaxId` not empty + `Matches(rfcPattern)` (12-character RFC-persona-moral structure, research.md Decisions 5-6), `CreditLimit`: when provided, `>= 0` — structural rules only (depends on T021)
- [X] T023 [US1] Create `ICustomerService` in `ShipmentTracker.Core/Interfaces/Services/ICustomerService.cs` with the first two methods: `Task<CustomerDetailDto> CreateIndividualAsync(CreateIndividualCustomerDto dto);` and `Task<CustomerDetailDto> CreateBusinessAsync(CreateBusinessCustomerDto dto);` (depends on T019, T021, T007)
- [X] T024 [US1] Create `CustomerService` in `ShipmentTracker.Services/CustomerService.cs`, `: ICustomerService`, constructor `(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateIndividualCustomerDto> createIndividualValidator, IValidator<CreateBusinessCustomerDto> createBusinessValidator)`; implement private helpers `ValidateEmailUniquenessAsync(email, currentId)` (checks against **all** customers, both types, active and inactive), `ValidateGovernmentIdUniquenessAsync(governmentId, currentId)` (checks `x is IndividualCustomer ic && ic.GovernmentId == governmentId`, active and inactive), `ValidateTaxIdUniquenessAsync(taxId, currentId)` (same pattern for `BusinessCustomer`/`TaxId`) — each returns a list of `ValidationFailure` (research.md Decision 7); implement `CreateIndividualAsync`: trim `Email`/`GovernmentId`/`FirstName`/`LastName`, run the structural validator, combine with email + governmentId uniqueness checks (`currentId: 0`), throw `FluentValidation.ValidationException` if any errors, else construct `IndividualCustomer` by hand (`Type = CustomerType.Individual`, `IsActive = true`, `CreatedAt = DateTime.UtcNow`, `UpdatedAt = null`), `AddAsync` + `CommitAsync`, return `_mapper.Map<CustomerDetailDto>`; implement `CreateBusinessAsync` analogously with `TaxId` uniqueness instead of `GovernmentId` (depends on T015, T017, T020, T022, T023)
- [X] T025 [US1] Create `CustomerController` in `ShipmentTracker.Web/Controllers/CustomerController.cs`, route `[Route("api/customers")]`; action `[HttpPost("individual")] CreateIndividualCustomer([FromBody] CreateIndividualCustomerDto dto)` and action `[HttpPost("business")] CreateBusinessCustomer([FromBody] CreateBusinessCustomerDto dto)`: each calls the matching service method and returns `Created($"/api/customers/{result.Id}", result)` (no `CreatedAtAction`/`nameof`, avoids depending on `GetCustomerById`, added in Story 2), catching `FluentValidation.ValidationException` → `400` with `{ errors: [{ property, message }] }`; XML doc comments (depends on T024)
- [X] T026 [US1] In `ShipmentTracker.Web/Program.cs`: add `builder.Services.AddScoped<ICustomerService, CustomerService>();`, `builder.Services.AddScoped<IValidator<CreateIndividualCustomerDto>, CreateIndividualCustomerDtoValidator>();`, `builder.Services.AddScoped<IValidator<CreateBusinessCustomerDto>, CreateBusinessCustomerDtoValidator>();` (depends on T024, T020, T022, T018)

**Checkpoint**: Both `POST /api/customers/individual` and `POST /api/customers/business` work
end-to-end. User Story 1 independently verifiable.

---

## Phase 4: User Story 2 - Find and review customers (Priority: P2)

**Goal**: `GET /api/customers` lists customers paginated, optionally filtered by `onlyActive` and/or
`type`; `GET /api/customers/{id}` retrieves a single customer with full type-specific detail
regardless of status.

**Independent Test**: With Individual and Business customers already created (via US1), `GET
/api/customers` returns only active customers by default; `GET /api/customers?type=Business` returns
only active Business customers; `GET /api/customers/{id}` includes `individual`/`business` correctly
populated; `GET /api/customers/999999` returns `404` (scenarios 1-7 of `quickstart.md`, User Story 2).

### Implementation for User Story 2

- [X] T027 [US2] In `ShipmentTracker.Core/Interfaces/Services/ICustomerService.cs`: add `Task<PagedResult<CustomerDetailDto>> GetCustomersAsync(bool onlyActive = true, CustomerType? type = null, int page = 1, int pageSize = 5);` and `Task<CustomerDetailDto?> GetCustomerByIdAsync(int id);` (depends on T023 — same file)
- [X] T028 [US2] In `ShipmentTracker.Services/CustomerService.cs`: add `private const int MaxPageSize = 50;`; implement `GetCustomersAsync` (build `Expression<Func<Customer, bool>>` combining `x.IsActive == onlyActive` with an optional `x.Type == type.Value` — same pattern as `BranchService.GetBranchesAsync`; compute `skip` as `long`, apply `Math.Min(pageSize, MaxPageSize)`, call `_unitOfWork.CustomerRepository.GetAsync(filter, orderBy: q => q.OrderByDescending(x => x.CreatedAt), skip, take)` + `CountAsync(filter)`, return `PagedResult<CustomerDetailDto>`); implement `GetCustomerByIdAsync` (`GetByIdAsync(id)`, `null` if not found, else `_mapper.Map<CustomerDetailDto>` — no `IsActive` filter, single-record retrieval always works) (depends on T024, T027 — same file)
- [X] T029 [US2] In `ShipmentTracker.Web/Controllers/CustomerController.cs`: add `[HttpGet] GetCustomers([FromQuery] bool onlyActive = true, [FromQuery] CustomerType? type = null, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)` → sets headers `X-Total-Count`/`X-Page`/`X-Page-Size`/`X-Total-Pages` and `Ok(result.Items)` (same pattern as `BranchController.GetBranches`), and `[HttpGet("{id}")] GetCustomerById(int id)` → `200` with `CustomerDetailDto` or `404` with `{ "message": "No customer was found with id '{id}'." }`; XML doc comments (depends on T025, T028 — same file)

**Checkpoint**: Listing and single-record lookup work. User Stories 1 and 2 independently verifiable
together.

---

## Phase 5: User Story 3 - Update customer information (Priority: P3)

**Goal**: `PUT /api/customers/{id}` replaces a customer's shared fields and its type-appropriate
fields, re-validating structural rules, cross-type field rejection, type-appropriate completeness, and
uniqueness before writing any change.

**Independent Test**: `PUT` on an existing customer with valid, non-conflicting values returns `200`
and reflects the changes; `PUT` including a field belonging to the customer's other type, omitting a
required field for its actual type, or reusing a duplicate `email`/`governmentId`/`taxId` returns
`400` and leaves the customer unchanged (scenarios 1-7 of `quickstart.md`, User Story 3).

### Implementation for User Story 3

- [X] T030 [P] [US3] Create `UpdateCustomerDto` in `ShipmentTracker.Core/DTOs/Customers/UpdateCustomerDto.cs`: shared fields required (`Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `IsActive`) plus **all** type-specific fields from both subtypes as nullable (`FirstName?`, `LastName?`, `BirthDate?`, `GovernmentId?`, `BusinessName?`, `TaxId?`, `LegalRepresentative?`, `Industry?`, `CreditLimit?`) — no `Type` field at all (immutable, FR-004; data-model.md, research.md Decision 8) (depends on T001)
- [X] T031 [US3] Create `UpdateCustomerDtoValidator` in `ShipmentTracker.Services/Validators/Customers/UpdateCustomerDtoValidator.cs` (`AbstractValidator<UpdateCustomerDto>`): shared fields not empty + `Email` format (always required — every customer has them); `GovernmentId`, when non-null: `Matches(curpPattern)`; `TaxId`, when non-null: `Matches(rfcPattern)`; `CreditLimit`, when non-null: `>= 0` — shape-only, does **not** decide which type-specific fields are required or forbidden (research.md Decision 8) (depends on T030)
- [X] T032 [US3] In `ShipmentTracker.Core/Interfaces/Services/ICustomerService.cs`: add `Task<CustomerDetailDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto);` (depends on T030, T027 — same file)
- [X] T033 [US3] In `ShipmentTracker.Services/CustomerService.cs`: add `IValidator<UpdateCustomerDto> updateValidator` to the constructor; implement `UpdateCustomerAsync`: load with `GetByIdAsync(id)` (`null` → `null`); trim string fields; run `updateValidator` (shape only), accumulate errors; branch on the loaded entity's runtime type (`is IndividualCustomer`/`is BusinessCustomer`) to (a) reject the request if any field belonging to the *other* type is non-null in the DTO (FR-013), and (b) require that type's completeness rule be satisfied by the DTO's present fields (FR-005); run `ValidateEmailUniquenessAsync`/`ValidateGovernmentIdUniquenessAsync`-or-`ValidateTaxIdUniquenessAsync` (whichever applies to the loaded type) with `currentId: id`; if any error accumulated, throw `ValidationException` without mutating the entity; else apply the shared fields plus the type-appropriate fields onto the loaded entity, set `UpdatedAt = DateTime.UtcNow`, `Update()` + `CommitAsync()`, return `_mapper.Map<CustomerDetailDto>` (depends on T028, T031, T032)
- [X] T034 [US3] In `ShipmentTracker.Web/Controllers/CustomerController.cs`: add `[HttpPut("{id}")] UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)` → `200`/`404`/`400` (same pattern as `BranchController.UpdateBranch`) (depends on T029, T033 — same file)
- [X] T035 [US3] In `ShipmentTracker.Web/Program.cs`: add `builder.Services.AddScoped<IValidator<UpdateCustomerDto>, UpdateCustomerDtoValidator>();` (depends on T031, T026 — same file)

**Checkpoint**: Updating shared and type-specific fields, including reactivation via `isActive: true`,
works. User Stories 1-3 independently verifiable together.

---

## Phase 6: User Story 4 - Deactivate a customer (Priority: P4)

**Goal**: `DELETE /api/customers/{id}` deactivates a customer (soft-delete, `IsActive = false`)
idempotently; no layer exposes a physical delete.

**Independent Test**: `DELETE` on an active customer returns `204` and the customer no longer appears
in default listings, while `GET /api/customers/{id}` still returns it with all fields intact;
repeating the same `DELETE` returns `204` again with no error; `DELETE` on a nonexistent id returns
`404` (scenarios 1-4 of `quickstart.md`, User Story 4).

### Implementation for User Story 4

- [X] T036 [US4] In `ShipmentTracker.Core/Interfaces/Services/ICustomerService.cs`: add `Task<bool> DeactivateCustomerAsync(int id);` (depends on T032 — same file)
- [X] T037 [US4] In `ShipmentTracker.Services/CustomerService.cs`: implement `DeactivateCustomerAsync`: look up with `GetByIdAsync(id)`, return `false` if not found; if `IsActive == true`, set it to `false` (leaving `UpdatedAt` untouched — this is a deactivation, not a data edit), `Update()` + `CommitAsync()`; if already inactive, write nothing (idempotent); return `true` in both success cases (depends on T033, T036)
- [X] T038 [US4] In `ShipmentTracker.Web/Controllers/CustomerController.cs`: add `[HttpDelete("{id}")] DeactivateCustomer(int id)` → `204` if the service returns `true`, `404` if it returns `false`; XML doc comment clarifying this is a soft-delete (depends on T034, T037 — same file)

**Checkpoint**: All 4 user stories work together — full CRUD (create both subtypes, list/filter/get,
update, deactivate) for Customers & Accounts.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T039 [P] Build the solution (`dotnet build ShipmentTracker.sln`) and confirm zero errors and zero new warnings (depends on T001-T038)
- [X] T040 Apply the migration (`dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web`) and run every scenario in `specs/005-customers-accounts/quickstart.md` end to end (all 4 stories + edge cases) (depends on T039)
- [X] T041 [P] Confirm no file belonging to `Shipment`, `Branch`, `Employee`, or `Vehicle` changed (`git diff --stat` should list no `Shipment*`/`Branch*`/`Employee*`/`Vehicle*` paths, except the shared `IUnitOfWork.cs`/`UnitOfWork.cs`/`Program.cs`, already touched by prior modules too) and that none of their existing endpoints changed behavior (depends on T039)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: N/A.
- **Foundational (Phase 2)**: no dependency on any story — BLOCKS all 4 stories.
- **User Story 1 (Phase 3)**: depends on Foundational. No dependency on other stories.
- **User Story 2 (Phase 4)**: depends on T023/T024/T025 (US1) already existing (same 3 files extended).
- **User Story 3 (Phase 5)**: depends on T027/T028/T029 (US2) already existing (same 3 files extended).
- **User Story 4 (Phase 6)**: depends on T032/T033/T034 (US3) already existing (same 3 files extended).
- **Polish (Phase 7)**: depends on all 4 stories being complete.

Unlike module 004, there is only one track here — `Individual` and `Business` share every service/
controller file from US1 onward, so stories 2-4 are strictly sequential extensions of the same files,
not independent parallel tracks.

### Within Foundational

- T001 → T002 → T003, T004 (in parallel with each other)
- T005, T006 (independent of T001-T004) → T007 (also needs T001)
- T002 → T008 → T009
- T002 → T010; T003 → T011; T004 → T012 (all three in parallel) → T013
- T008, T013 → T014 → T015 (together with T009)
- T013 → T016
- T002, T003, T004, T007 → T017 (in parallel with T010-T016)
- T014 → T018

### Within each story

- **US1**: T001 → T019 → T020; T001 → T021 → T022; T019, T021, T007 → T023; T020, T022, T023, T015, T017 → T024 → T025; T024, T020, T022, T018 → T026
- **US2**: T023 → T027; T024, T027 → T028; T025, T028 → T029
- **US3**: T001 → T030 → T031; T030, T027 → T032; T028, T031, T032 → T033; T029, T033 → T034; T031, T026 → T035
- **US4**: T032 → T036; T033, T036 → T037; T034, T037 → T038

### Parallel Opportunities

- Foundational: **T003+T004** in parallel once T002 exists; **T005+T006** can start immediately,
  in parallel with T001-T004; **T010+T011+T012** in parallel once T002/T003/T004 exist; **T017** can
  be done in parallel with the whole T008-T016 Infrastructure chain (different project layer).
- US1: **T019+T021** (Create DTOs) in parallel; **T020+T022** (their validators) in parallel once
  their respective DTOs exist.
- US3: **T030** has no sibling to parallelize with in that story, but can be started as soon as
  Foundational (T001) is done, ahead of finishing US1/US2, since it depends only on T001 — however
  T032/T033/T034 still require US2's files to exist first, so implementing it early only saves the
  DTO-authoring step, not the full story.
- Unlike module 004, US2/US3/US4 cannot run in parallel with each other or with US1 — all four extend
  the same `ICustomerService.cs`/`CustomerService.cs`/`CustomerController.cs` files, so they are
  inherently sequential per file (this is expected and consistent with `spec.md`'s own priority
  ordering, P1 → P2 → P3 → P4).

---

## Parallel Example: Foundational

```bash
# At the start of Phase 2, in parallel:
Task: "Create enum CustomerType in ShipmentTracker.Core/Enums/CustomerType.cs"
Task: "Create IndividualDetailDto in ShipmentTracker.Core/DTOs/Customers/IndividualDetailDto.cs"
Task: "Create BusinessDetailDto in ShipmentTracker.Core/DTOs/Customers/BusinessDetailDto.cs"

# Once the Customer entity exists, in parallel:
Task: "Create entity IndividualCustomer in ShipmentTracker.Core/Entities/IndividualCustomer.cs"
Task: "Create entity BusinessCustomer in ShipmentTracker.Core/Entities/BusinessCustomer.cs"
```

## Parallel Example: User Story 1 creation DTOs

```bash
# Once CustomerType exists, in parallel:
Task: "Create CreateIndividualCustomerDto in ShipmentTracker.Core/DTOs/Customers/CreateIndividualCustomerDto.cs"
Task: "Create CreateBusinessCustomerDto in ShipmentTracker.Core/DTOs/Customers/CreateBusinessCustomerDto.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 only)

1. Complete Phase 2: Foundational (T001-T018).
2. Complete Phase 3: User Story 1 (T019-T026).
3. Run scenarios 1-6 of User Story 1 in `quickstart.md`.
4. Both `POST` endpoints are already a deliverable increment — customers of either type can be
   registered, even though they cannot yet be listed, updated, or deactivated via the API.

### Incremental Delivery

1. Foundational → persistence ready for both subtypes, no endpoints yet.
2. US1 → validate → registering Individual and Business customers works (MVP).
3. US2 → validate → finding and reviewing customers by status/type is available.
4. US3 → validate → correcting customer data (including reactivation) is available.
5. US4 → validate → retiring a customer is available; full CRUD complete.
6. Polish → clean build + full manual validation + confirmation that no other module changed.

### Parallel Team Strategy

Unlike module 004's two independent tracks, this module has a **single track**: `CustomerService`/
`CustomerController` are shared by both subtypes from US1 onward, so stories cannot be split across
developers the way `Employee`/`Vehicle` could. Within Foundational, one developer can work the
`Individual`-related files (T003, T011) while another works the `Business`-related files (T004, T012)
in parallel; from Phase 3 onward, work is effectively single-threaded per the Dependencies above.

---

## Notes

- No automated test tasks — this project has no test project; validation is manual via
  `quickstart.md`.
- `Customer`/`IndividualCustomer`/`BusinessCustomer` have no relationship to `Shipment`, `Branch`, or
  `Employee`/`Vehicle` (spec.md, Assumptions) — no task models such a relationship.
- The `POST` actions in Story 1 use `Created(uri, result)` instead of
  `CreatedAtAction(nameof(...))` to avoid depending on `GetCustomerById`, which is only added in
  Story 2 — see T025.
- `UpdateCustomerDto` has no `Type` property at all, by construction — there is no runtime "reject a
  type change" check to write, because the field to attempt it with does not exist in the DTO (FR-004).
- Each task touches 1-3 files at most, consistent with Principio IV of the constitution.
