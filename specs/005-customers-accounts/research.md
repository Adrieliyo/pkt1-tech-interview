# Research: Customers & Accounts Module

All `NEEDS CLARIFICATION` markers in the Technical Context are resolved — the user supplied an
explicit stack, entity shapes, EF Core TPT configuration notes, and the six HTTP routes directly in
the `/speckit-plan` input. This document records the technical decisions derived from that input,
from `spec.md` (including its `## Clarifications`), and from direct inspection of the current
codebase (`ShipmentService`/`BranchService`/`EmployeeService`/`VehicleService`, `IBaseRepository`,
`Branch.cs`, `Employee.cs`).

## Decision 1: Table Per Type (TPT) via `.ToTable()` on each derived entity

- **Decision**: `Customer` is an abstract base entity mapped to table `Customers`; `IndividualCustomer`
  and `BusinessCustomer` derive from it via standard C# inheritance and are each mapped with
  `.ToTable("IndividualCustomers")` / `.ToTable("BusinessCustomers")` in their own
  `IEntityTypeConfiguration<T>`. EF Core resolves the concrete type per row via an inner join between
  `Customers` and whichever derived table has a matching `Id` — no discriminator column is added.
- **Rationale**: Explicit user instruction. TPT also maps cleanly onto the "Customer as Individual or
  Business" domain language already used throughout `spec.md`'s Key Entities section (`Customer`,
  `Individual Customer`, `Business Customer` as three related concepts, not one flat table with a pile
  of nullable columns) — this is the first module in the solution with a type-discriminated entity
  (already flagged as a new pattern in `checklists/requirements.md`'s Notes).
- **Alternatives considered**: Single table with nullable type-specific columns (TPH-style, no real
  discriminator needed either since `Type` is already a persisted property, see Decision 2) —
  explicitly left open at spec level for this planning phase to resolve, and rejected here because it
  would mix Individual-only and Business-only nullable columns in one table, which is exactly what the
  spec-quality checklist flagged as needing a plan-level decision. TPT keeps each subtype's required
  columns (`NOT NULL` at the database level) genuinely required, instead of nullable-by-necessity.

## Decision 2: `Customer.Type` stays a real, persisted property — not an EF discriminator

- **Decision**: `Customer` keeps a mapped `Type` (`CustomerType`) property, persisted as a plain column
  on the `Customers` table (`HasConversion<string>()`, same convention as every other enum in this
  solution). This is unrelated to — and not replaced by — the fact that EF Core's TPT mechanism itself
  needs no hidden discriminator column to resolve the runtime type from the join.
- **Rationale**: The user's own entity outline lists `type CustomerType enum` as an explicit field
  inside the abstract `Customer` base, and separately notes "discriminator not needed" only in the EF
  configuration section — read together, this is "no EF-managed shadow discriminator column," not "no
  `Type` property at all." Keeping `Type` as a real column also lets `GET /api/customers?type=...`
  filter directly on the base `Customers` table (`x.Type == type.Value`) without touching either derived
  table, matching how `GetBranchesAsync`/`GetEmployeesAsync` filter on plain base-table columns today —
  filtering via `x is BusinessCustomer` instead would force a join on every list call even when the
  caller only wants to filter by type, with no additional benefit.
- **Alternatives considered**: Drop `Type` entirely and rely on `is IndividualCustomer` / `is
  BusinessCustomer` pattern matching for both filtering and the AutoMapper conditional-mapping switch
  (Decision 9) — rejected: it works in LINQ-to-Entities (EF Core 8 translates `is` checks against a TPT
  hierarchy to an existence join) but is strictly more expensive for the common list-filter path and
  contradicts the user's explicit field list for `Customer`.

## Decision 3: Address fields as flat scalar properties on `Customer` — no owned type

- **Decision**: `Address` (street line), `City`, `State`, `ZipCode`, `Country` are plain `string`
  properties directly on `Customer` (not an EF Core owned entity / value object).
- **Rationale**: Matches the existing `Branch` entity's address shape exactly (`Branch.Address`,
  `City`, `State`, `ZipCode` — module 003), which is the only other address-holding entity in this
  solution. Introducing an EF Core owned type here would be a new pattern for the same concern
  (address storage) that `Branch` already solved with flat columns — the layered-architecture
  convention in `CLAUDE.md` says not to introduce a competing pattern without retiring the old one, and
  retiring `Branch`'s flat address columns is out of scope for this module.
- **Alternatives considered**: `OwnedEntity`/complex-type address value object — rejected as an
  unjustified second pattern for a solved problem (Principio III/Minimalismo de Dependencias applies
  by extension: no new modeling pattern without a concrete need `Branch`'s existing shape doesn't
  already cover).

## Decision 4: `id_number` → `GovernmentId`; `tax_id` → `TaxId` (naming, not behavior)

- **Decision**: The Individual-only field is named `GovernmentId` (not the plan input's literal
  `id_number`); the Business-only field is named `TaxId`, matching the plan input directly.
- **Rationale**: `spec.md`'s FR-014/FR-015 and Clarifications consistently say "government ID number,"
  never "ID number" alone — `GovernmentId` keeps the C# property name aligned with the spec's own
  vocabulary (same reasoning `CLAUDE.md`'s Terminology guidance already applies elsewhere in this
  project), while `snake_case` field names from the plan input are converted to C# `PascalCase`
  throughout, consistent with every existing entity in the solution.
- **Alternatives considered**: Keep `IdNumber` verbatim — rejected as a needless vocabulary mismatch
  against the just-clarified spec language, for a change with the same one-line implementation cost
  either way.

## Decision 5: Business tax ID max length corrected from 13 to 12 — RFC-persona-moral length

- **Decision**: `BusinessCustomer.TaxId` is configured with `HasMaxLength(12)`, not the plan input's
  `max 13 chars (RFC length)`.
- **Rationale**: `spec.md`'s Clarifications (this session, prior to `/speckit-plan`) explicitly fixed
  the Business tax ID's official format at "12 alphanumeric characters" (RFC for a legal entity /
  persona moral — the 13-character RFC format applies only to an individual acting as a taxpayer,
  which is not this field; `BusinessCustomer.TaxId` is exclusively the legal-entity RFC). The plan
  input's "13 chars" comment predates that clarification and conflicts with it. Per the same
  precedent as module 004's Decision 4 (resolving a `Guid`/`int` conflict between plan input and
  real code), the already-clarified spec is the source of truth over a stale inline comment in the
  planning request.
- **Alternatives considered**: Follow the plan input literally (13 chars) — rejected: it would silently
  contradict the FR-016/FR-018-equivalent format rule just agreed with the user in `/speckit-clarify`,
  and a 13-character cap would incorrectly accept an individual-taxpayer RFC as a valid business tax ID.

## Decision 6: CURP/RFC format validation lives in FluentValidation as a `Matches()` regex — structural, not DB-dependent

- **Decision**: `GovernmentId` (Individual) and `TaxId` (Business) get a `Matches(pattern)` rule in
  their respective Create validators (and the shared Update validator, Decision 8), in addition to
  `HasMaxLength` at the EF Core configuration level. CURP pattern: 18 uppercase alphanumeric characters
  in the standard government layout (4 letters, 6-digit birth date, 1 sex letter, 5 state/consonant
  block, 2 check characters — expressed as a single regex, exact expression is an implementation
  detail of the validator, not repeated here). RFC (persona moral) pattern: 12 alphanumeric characters
  (3 letters + 6-digit date + 3-character homoclave).
- **Rationale**: Format validity (does this string have the right shape) is a pure function of the
  string itself — no repository call needed — so it belongs in FluentValidation next to the existing
  `EmailAddress()` rule, exactly like every other structural rule in this solution (`CLAUDE.md`:
  "FluentValidation covers structural rules only"). This is different from *uniqueness*, which does
  need a repository call and stays in the Service (Decision 7).
- **Alternatives considered**: Format-check inside the Service alongside the uniqueness check —
  rejected: it doesn't need `IUnitOfWork` at all, so moving it out of FluentValidation would only
  duplicate the "no DB call, still checked in the Service" carve-out this project has never needed
  before; keeping it in FluentValidation is the more consistent, smaller change.

## Decision 7: Global uniqueness (`Email`, `GovernmentId` among Individuals, `TaxId` among Businesses) — Service-level, no `IsActive` filter, trimmed input

- **Decision**: `CustomerService` checks `Email` uniqueness across **all** customers (both concrete
  types, active or inactive) via `_unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.Email ==
  email && x.Id != currentId)` against the base `Customer` type; `GovernmentId` uniqueness is checked
  only among `IndividualCustomer` rows (`x is IndividualCustomer ic && ic.GovernmentId == govId && ic.Id
  != currentId`), `TaxId` only among `BusinessCustomer` rows, same pattern. All three checks run
  regardless of the compared record's `IsActive` value. `Email`/`GovernmentId`/`TaxId` are `.Trim()`-ed
  on the DTO before any validation or comparison.
- **Rationale**: Directly confirmed in `spec.md`'s Clarifications (both the type-immutability session
  and the earlier Employees & Vehicles module's precedent, explicitly cited in this module's own
  Assumptions) — an identifier used once is reserved forever, even if its owner is later deactivated.
  Trimming before comparison mirrors the Edge Cases entry on whitespace/case-insensitive duplicates;
  case-insensitivity itself is free from SQL Server's default collation, same as every other unique
  string index in this solution (no code-level `.ToLower()` needed, per Decision 8 of module 004's
  research.md).
- **Alternatives considered**: Filtered unique index (`WHERE IsActive = 1`) allowing reuse after
  deactivation — rejected outright by the spec's own Clarifications.

## Decision 8: One `UpdateCustomerDto` for both types — DB-dependent completeness check moves to the Service

- **Decision**: `PUT /api/customers/{id}` takes a single `UpdateCustomerDto` with the shared fields
  required, plus **all** type-specific fields as nullable/optional properties (`FirstName?`,
  `LastName?`, `BirthDate?`, `GovernmentId?` for Individual; `BusinessName?`, `TaxId?`,
  `LegalRepresentative?`, `Industry?`, `CreditLimit?` for Business). `UpdateCustomerDtoValidator`
  (FluentValidation) only validates the *shape* of whichever fields are present (email format, CURP/RFC
  regex if non-null, `CreditLimit >= 0` if non-null) — it cannot know which fields are *required*,
  because that depends on the target customer's already-persisted `Type`, a DB fact. `CustomerService`
  loads the existing customer first, and — now knowing its concrete type — re-runs the same
  completeness rules Create uses for that type (FR-005: "re-validating all applicable creation rules on
  every update") and rejects the request (FR-013) if any field belonging to the *other* type is
  non-null in the submitted DTO.
- **Rationale**: There is no `Type` field in the update contract at all (Decision 2's `Type` is
  create-time-only and immutable per FR-004 — an update request has no legal way to even attempt
  setting it), so the validator genuinely cannot resolve "which type's required-fields rule applies"
  without a repository call — the same category of DB-dependent rule this project already keeps out of
  FluentValidation (`CLAUDE.md`'s structural-vs-service split). This is a new wrinkle relative to
  `Employee`/`Vehicle` (module 004), whose single Update DTO always had exactly one required shape; it
  is documented here explicitly for that reason, the same way module 004's research.md flagged its own
  first-time combination of structural + DB-dependent validation (that module's Decision 5).
- **Alternatives considered**: Two update endpoints (`PUT /api/customers/individual/{id}`,
  `PUT /api/customers/business/{id}`) mirroring the two create endpoints — rejected: the user's
  endpoint list fixes a single `PUT /api/customers/{id}`, and a single endpoint is also simpler for a
  caller who already knows the customer's id but not necessarily its type ahead of the call.

## Decision 9: AutoMapper conditional mapping via `Include` + `AfterMap` — no custom `ITypeConverter` class

- **Decision**: `CustomerMappingProfile` declares:
  `CreateMap<Customer, CustomerDetailDto>().Include<IndividualCustomer, CustomerDetailDto>().Include<BusinessCustomer, CustomerDetailDto>()`,
  then `CreateMap<IndividualCustomer, CustomerDetailDto>().AfterMap((src, dest) => dest.Individual = new
  IndividualDetailDto { ... })` and the equivalent for `BusinessCustomer` → `dest.Business`. Calling
  `_mapper.Map<CustomerDetailDto>(customer)` with `customer`'s runtime type being `IndividualCustomer`
  or `BusinessCustomer` (never the abstract `Customer` itself, since it can't be instantiated) resolves
  to the correct derived map automatically via AutoMapper's runtime-type polymorphic mapping, populating
  exactly one of `Individual`/`Business` on the shared `CustomerDetailDto` — the discriminated-union
  shape the user asked for.
- **Rationale**: The user's own instruction offered a choice ("custom `ITypeConverter` or `AfterMap` in
  the profile"); `AfterMap` on `Include`d derived maps is the smaller change — it stays entirely inside
  the existing `Mappings/CustomerMappingProfile.cs` file, with no new class, consistent with
  Minimalismo de Dependencias and with this project's "AutoMapper is output-only" convention (entity →
  DTO only, never used for the reverse direction).
- **Alternatives considered**: A standalone `ITypeConverter<Customer, CustomerDetailDto>` class —
  rejected as the same outcome for more code; reserved as a fallback only if `AfterMap` proves
  insufficient during implementation (unlikely — this is a standard, documented AutoMapper pattern for
  inheritance hierarchies).

## Decision 10: `ICustomerRepository : IBaseRepository<Customer>` — no extra methods, generic paging works unchanged against the TPT base

- **Decision**: `ICustomerRepository` adds nothing beyond `IBaseRepository<Customer>`, same as
  `IEmployeeRepository`/`IVehicleRepository`. `GetAsync`/`CountAsync` (already `skip`/`take`-aware)
  operate directly against `Customer` — EF Core 8 translates a base-type `DbSet<Customer>` LINQ query
  over a TPT hierarchy into the necessary joins/union automatically; no `OfType<T>()` or custom SQL is
  needed for straightforward list/count/get-by-id/single-or-default calls.
- **Rationale**: Matches the established convention verbatim ("If a repository needs no query beyond
  what the generic base already offers, leave its interface empty") — every filter this module needs
  (`IsActive`, `Type`, plus the `is IndividualCustomer`/`is BusinessCustomer` uniqueness predicates from
  Decision 7) is expressible as an `Expression<Func<Customer, bool>>` passed into the existing generic
  methods.
- **Alternatives considered**: Add `GetByIdWithDetailAsync` mirroring `BranchRepository`'s
  `GetByIdWithScheduleAsync` — rejected as unnecessary: TPT already returns the fully-typed derived
  instance (with all Individual/Business-only columns populated) from a plain `GetByIdAsync`/
  `SingleOrDefaultAsync` — there is no separate related collection to eagerly `Include()`, unlike
  `Branch.Schedule`.

## Decision 11: Pagination reused from `002`/`003`/`004` — identical contract, `onlyActive`/`type` filters

- **Decision**: `GetCustomersAsync(bool onlyActive = true, CustomerType? type = null, int page = 1, int
  pageSize = 5)` returns `PagedResult<CustomerDetailDto>`, same `MaxPageSize = 50` clamp,
  `OrderByDescending(x => x.CreatedAt)`, same 4 response headers (`X-Total-Count`, `X-Page`,
  `X-Page-Size`, `X-Total-Pages`) already exposed once for the whole API via CORS in `Program.cs`. The
  filter expression mirrors `BranchService.GetBranchesAsync` exactly: `x.IsActive == onlyActive` (not
  "only active, ignore the flag") combined with an optional `x.Type == type.Value`.
- **Rationale**: Explicit user instruction ("Keep my pagination rules for global GET endpoints... check
  Shipment module"); `Branch`'s `onlyActive` boolean (rather than `Employee`'s always-active-only
  listing) is the closer precedent here because the user's query string is literally
  `?onlyActive=bool&type=CustomerType`, matching `BranchController.GetBranches`'s exact parameter
  names and semantics (a caller can request only-inactive customers by passing `onlyActive=false`,
  needed for the spec's User Story 2 acceptance scenario on filtering to inactive-only).
- **Alternatives considered**: Employee/Vehicle-style always-active-only default with no way to list
  inactive records — rejected: it cannot satisfy US2's "filtered explicitly to inactive status" scenario,
  and the user's own query string names an `onlyActive` boolean rather than an implicit always-active
  filter.

## Decision 12: JSON enum representation — `CustomerType` gets the per-property `JsonStringEnumConverter`

- **Decision**: `CustomerDetailDto.Type` (and any other `CustomerType` DTO property) carries
  `[JsonConverter(typeof(JsonStringEnumConverter))]`, same per-property pattern as `Branch.Type`,
  `Employee.Role`, `Vehicle.Type`. The `?type=Business` query-string filter on `GET /api/customers`
  needs no such converter — ASP.NET Core's query-string enum binder already accepts the enum's name
  natively, independent of the JSON body serializer (same asymmetry already documented and relied on
  in module 004's research.md, Decision 12).
- **Rationale**: `CLAUDE.md`'s documented gotcha — no global `JsonStringEnumConverter` exists in this
  solution (would silently change `ShipmentStatus`'s existing numeric wire contract) — applies
  identically here.
- **Alternatives considered**: N/A — this is a fixed, already-documented project convention with no
  new decision to make.

## Status

All `NEEDS CLARIFICATION` markers resolved. No blockers for Phase 1.
