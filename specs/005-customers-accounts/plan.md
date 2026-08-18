# Implementation Plan: Customers & Accounts Module

**Branch**: `005-customers-accounts` | **Date**: 2026-08-17 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/005-customers-accounts/spec.md`

## Summary

New, self-contained module (no changes to `Shipment`, `Branch`, or `Employee`/`Vehicle`) that adds a
type-discriminated `Customer` concept to the existing four layers, using EF Core Table Per Type (TPT)
inheritance: an abstract `Customer` base entity (shared contact/address/status fields, plus a
persisted `Type` discriminator property) with two concrete subtypes, `IndividualCustomer` and
`BusinessCustomer`, each adding its own required identifying fields. Structural validation
(FluentValidation) covers required fields, email format, and — new for this module — CURP/RFC format
regexes; database-dependent rules (global email uniqueness across both types, government-ID/tax-ID
uniqueness within each type, all enforced even against inactive records) are resolved in
`CustomerService`, following the same split already established by `EmployeeService`/`VehicleService`.
AutoMapper stays output-only, using `Include`/`AfterMap` to populate a discriminated-union response
DTO (`CustomerDetailDto` with nested `Individual`/`Business` detail) based on the entity's runtime
type. Repository + Unit of Work: a single `ICustomerRepository : IBaseRepository<Customer>` with no
extra methods, since EF Core 8 resolves TPT hierarchy queries natively through the generic
`GetAsync`/`SingleOrDefaultAsync`. The six routes fixed by the user are exposed through one
`CustomerController`; the paginated list reuses the exact `PagedResult<T>`/header contract already
established by `002-paginate-shipment-list`, using `Branch`'s `onlyActive`/`type` filter shape (not
`Employee`/`Vehicle`'s always-active-only shape) since the user's query string needs to support
listing inactive-only customers too.

## Technical Context

**Language/Version**: C# on .NET 8.0 (`net8.0`, unchanged from the rest of the solution)

**Primary Dependencies**: ASP.NET Core 8, Entity Framework Core 8 + SQL Server (already referenced),
AutoMapper (already registered), FluentValidation (already registered) — **zero new NuGet packages**
(Principio III / Minimalismo de Dependencias).

**Storage**: SQL Server via EF Core, same `AppDbContext`. Three new tables via TPT: `Customers`
(base — unique index on `Email`), `IndividualCustomers` (PK/FK to `Customers.Id` — unique index on
`GovernmentId`), `BusinessCustomers` (PK/FK to `Customers.Id` — unique index on `TaxId`). None of the
three unique indexes filters on `IsActive` — uniqueness applies even against inactive records
(confirmed in Clarifications, same precedent as module 004). Requires one new migration (generated
during implementation).

**Testing**: Manual via Swagger/HTTP (see `quickstart.md`), same policy as the rest of the project —
no automated test project exists.

**Target Platform**: ASP.NET Core Web API (no hosting change; same CORS `AllowReactApp`, which already
exposes the `X-Total-*` headers since `002`)

**Project Type**: Web service — same existing layered solution, additive new module

**Performance Goals**: N/A — no latency target defined; expected volume (a parcel company's customer
roster) is comfortably covered by the existing pagination mechanism.

**Constraints**: Reuses the patterns already adopted — `IBaseRepository<T>` + `IUnitOfWork`, AutoMapper
for output only (entity construction from input DTOs done by hand in the Service), FluentValidation
invoked manually for **structural** rules only (required fields, email format, CURP/RFC regex shape,
`CreditLimit >= 0`), while **database-dependent** rules (global/scoped uniqueness, type-immutability
enforcement, cross-type field rejection, type-appropriate completeness on update) are resolved in
`CustomerService` — the first module needing that DB-dependent check to also depend on which of two
possible entity subtypes is being updated (see research.md, Decision 8). Business tax-ID max length
corrected from the plan input's 13 to 12 characters, matching the RFC-persona-moral format already
confirmed in `spec.md`'s Clarifications (see research.md, Decision 5).

**Scale/Scope**: New module, additive across all 4 layers: `Core` (3 entities, 1 enum, 6 DTOs, 1
repository interface + 1 service interface + 1 new property on `IUnitOfWork`), `Infrastructure` (1
repository, 3 EF configurations, 1 migration, `AppDbContext`/`UnitOfWork` extended), `Services` (1
service, 3 validators, 1 mapping profile), `Web` (1 controller, DI registrations in `Program.cs`). No
file belonging to `Shipment`, `Branch`, `Employee`, or `Vehicle` is modified.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Evaluation | Result |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | No `TargetFramework` change; everything used is already referenced. | PASS |
| II. Integridad de la Arquitectura en Capas | `Core` has no outbound dependency. `Infrastructure`/`Services` depend only on `Core`. `Web` is the only project depending on all three. `Customer`/`IndividualCustomer`/`BusinessCustomer` are fully self-contained — no FK to any other module's entity, so no risk of an inverted or competing dependency. TPT inheritance is a new *entity-modeling* shape for this solution, but it is expressed entirely through the already-adopted `IEntityTypeConfiguration<T>` + `IBaseRepository<T>`/`IUnitOfWork` patterns — no second repository or persistence pattern is introduced alongside the existing one. AutoMapper's `Include`/`AfterMap` conditional mapping (research.md Decision 9) extends the existing "AutoMapper is output-only" convention rather than competing with it. | PASS |
| III. Minimalismo de Dependencias | Zero new NuGet packages. | PASS |
| IV. Cambios Pequeños y Reversibles | Additive, self-contained change: no file belonging to any other module is touched. The only modifications to existing files (`IUnitOfWork`/`UnitOfWork.cs` gain one property; `Program.cs` gains DI registrations) are the same kind of small, additive edits already made for modules 003/004. | PASS |

No violations to record in Complexity Tracking.

**Re-check post Phase 1**: after designing `data-model.md`, `contracts/`, and `quickstart.md`, the
table holds unchanged — the TPT modeling decision (Decision 1) and the AutoMapper conditional-mapping
decision (Decision 9) were the two design choices with the most potential to introduce a competing
pattern, and both were resolved by extending existing, already-adopted mechanisms rather than adding
new ones.

## Project Structure

### Documentation (this feature)

```text
specs/005-customers-accounts/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── customers-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Entities/
│   ├── Customer.cs                              # [NEW] abstract: Id, Type, Email, Phone, Address,
│   │                                             #   City, State, ZipCode, Country, IsActive,
│   │                                             #   CreatedAt, UpdatedAt (DateTime?)
│   ├── IndividualCustomer.cs                    # [NEW] : Customer — FirstName, LastName,
│   │                                             #   BirthDate (DateOnly?), GovernmentId
│   └── BusinessCustomer.cs                      # [NEW] : Customer — BusinessName, TaxId,
│                                                 #   LegalRepresentative, Industry?, CreditLimit?
├── Enums/
│   └── CustomerType.cs                          # [NEW] Individual, Business
├── DTOs/
│   └── Customers/
│       ├── CreateIndividualCustomerDto.cs       # [NEW] input for POST .../individual
│       ├── CreateBusinessCustomerDto.cs         # [NEW] input for POST .../business
│       ├── UpdateCustomerDto.cs                 # [NEW] input for PUT (shared required + all
│       │                                        #   type-specific fields nullable)
│       ├── CustomerDetailDto.cs                 # [NEW] output — shared + Type + Individual?/Business?
│       ├── IndividualDetailDto.cs               # [NEW] nested Individual-only output fields
│       └── BusinessDetailDto.cs                 # [NEW] nested Business-only output fields
└── Interfaces/
    ├── IUnitOfWork.cs                           # [MODIFY] + ICustomerRepository
    ├── Repositories/
    │   └── ICustomerRepository.cs               # [NEW] : IBaseRepository<Customer> (no extra methods)
    └── Services/
        └── ICustomerService.cs                  # [NEW]

ShipmentTracker.Infrastructure/
├── Data/
│   ├── AppDbContext.cs                          # [MODIFY] + DbSet<Customer>, DbSet<IndividualCustomer>,
│   │                                             #   DbSet<BusinessCustomer>, ApplyConfiguration x3
│   ├── Configurations/
│   │   ├── CustomerConfiguration.cs             # [NEW] ToTable("Customers"), unique index on Email
│   │   ├── IndividualCustomerConfiguration.cs   # [NEW] ToTable("IndividualCustomers"),
│   │   │                                        #   GovernmentId required/MaxLength(18)/unique
│   │   └── BusinessCustomerConfiguration.cs     # [NEW] ToTable("BusinessCustomers"),
│   │                                            #   TaxId required/MaxLength(12)/unique,
│   │                                            #   CreditLimit decimal(18,2) nullable
│   └── UnitOfWork.cs                            # [MODIFY] + lazy CustomerRepository property
├── Migrations/
│   └── <timestamp>_AddCustomers.cs              # [NEW] generated during implementation
└── Repositories/
    └── CustomerRepository.cs                    # [NEW] : BaseRepository<Customer>, ICustomerRepository

ShipmentTracker.Services/
├── CustomerService.cs                           # [NEW]
├── Mappings/
│   └── CustomerMappingProfile.cs                # [NEW] Customer→CustomerDetailDto via
│                                                 #   Include<IndividualCustomer>/Include<BusinessCustomer>
│                                                 #   + AfterMap (research.md Decision 9)
└── Validators/
    └── Customers/
        ├── CreateIndividualCustomerDtoValidator.cs  # [NEW] structural rules incl. CURP regex
        ├── CreateBusinessCustomerDtoValidator.cs    # [NEW] structural rules incl. RFC regex
        └── UpdateCustomerDtoValidator.cs            # [NEW] shape-only rules for present fields

ShipmentTracker.Web/
├── Program.cs                                   # [MODIFY] + DI registrations (1 repo, 1 service,
│                                                 #   3 validators)
└── Controllers/
    └── CustomerController.cs                    # [NEW] POST .../individual, POST .../business,
                                                   #   GET (paginated), GET/{id}, PUT/{id}, DELETE/{id}
```

**Structure Decision**: Same existing layered architecture, no new projects. `Customer` is modeled as
one module spanning three related entities (one abstract base + two concrete subtypes via TPT) rather
than two independent aggregates like `Employee`/`Vehicle` in module 004 — the base/subtype
relationship is intrinsic to the domain (`spec.md`'s own Key Entities section already describes
`Individual Customer`/`Business Customer` as "Customer where type = X"), so it is implemented as true
inheritance instead of two parallel, unrelated entities.

## Complexity Tracking

*No violations to justify — table intentionally omitted.*
