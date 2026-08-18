# Data Model: Customers & Accounts Module

TPT (Table Per Type) inheritance — see `research.md` Decision 1. `Customer` is abstract; every real
row lives in `Customers` joined to exactly one of `IndividualCustomers`/`BusinessCustomers`. No
relationship to `Shipment`, `Branch`, or `Employee`/`Vehicle` — fully self-contained module.

## Entity: `Customer` (abstract base)

`ShipmentTracker.Core/Entities/Customer.cs`, table `Customers`.

| Field | Type | Rules | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `Type` | `CustomerType` (enum) | Required; set once at creation, never changed afterward | FR-004 |
| `Email` | `string` | Required, valid email format, globally unique across both types (active and inactive) | FR-001, FR-002 |
| `Phone` | `string` | Required, free text | FR-001 |
| `Address` | `string` | Required (street line) | FR-001 |
| `City` | `string` | Required | FR-001 |
| `State` | `string` | Required, free text (no fixed code list — Assumptions) | FR-001 |
| `ZipCode` | `string` | Required | FR-001 |
| `Country` | `string` | Required, free text | FR-001 |
| `IsActive` | `bool` | `true` by default at creation; record is never deleted | FR-003, FR-008 |
| `CreatedAt` | `DateTime` (UTC) | Set by the service on create | — |
| `UpdatedAt` | `DateTime?` (UTC) | `null` until the first successful `PUT`; reassigned on every subsequent one | — |

## Entity: `IndividualCustomer : Customer`

`ShipmentTracker.Core/Entities/IndividualCustomer.cs`, table `IndividualCustomers` (TPT, `Id` is both
PK and FK back to `Customers.Id`).

| Field | Type | Rules | FR |
|---|---|---|---|
| `FirstName` | `string` | Required | FR-014 |
| `LastName` | `string` | Required | FR-014 |
| `BirthDate` | `DateOnly?` | Optional | FR-014 |
| `GovernmentId` | `string` | Required, matches the official CURP structure (18 alphanumeric characters), unique among Individual customers (active and inactive) | FR-014, FR-015 |

## Entity: `BusinessCustomer : Customer`

`ShipmentTracker.Core/Entities/BusinessCustomer.cs`, table `BusinessCustomers` (TPT, same `Id`
PK/FK pattern).

| Field | Type | Rules | FR |
|---|---|---|---|
| `BusinessName` | `string` | Required (legal business name) | FR-016 |
| `TaxId` | `string` | Required, matches the official RFC structure for legal entities (12 alphanumeric characters — corrected from the plan input's 13, see research.md Decision 5), unique among Business customers (active and inactive) | FR-016, FR-017 |
| `LegalRepresentative` | `string` | Required | FR-016 |
| `Industry` | `string?` | Optional, free text (Assumptions) | FR-016 |
| `CreditLimit` | `decimal?` | Optional; when provided, must not be negative | FR-016, FR-018 |

## Enum

`ShipmentTracker.Core/Enums/CustomerType.cs`:

```
Individual, Business
```

Persisted as `string` (`HasConversion<string>()`), same convention as `ShipmentStatus`/`BranchType`/
`EmployeeRole`/`VehicleType`.

## DTOs

`ShipmentTracker.Core/DTOs/Customers/`:

| DTO | Used by | Fields |
|---|---|---|
| `CreateIndividualCustomerDto` | `POST /api/customers/individual` | `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `FirstName`, `LastName`, `BirthDate?`, `GovernmentId` — no `Type` (implied by the endpoint), no `IsActive` (always `true` at creation) |
| `CreateBusinessCustomerDto` | `POST /api/customers/business` | `Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `BusinessName`, `TaxId`, `LegalRepresentative`, `Industry?`, `CreditLimit?` |
| `UpdateCustomerDto` | `PUT /api/customers/{id}` | Shared fields (`Email`, `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`, `IsActive`) required; **all** type-specific fields from both subtypes present as nullable/optional properties (see research.md Decision 8) — no `Type` field at all (immutable, FR-004) |
| `CustomerDetailDto` | Response for every endpoint (`POST` x2, `GET` list, `GET/{id}`, `PUT`) | Shared fields + `Type` + exactly one of `Individual` (`IndividualDetailDto?`) / `Business` (`BusinessDetailDto?`), populated by AutoMapper based on the entity's runtime type (research.md Decision 9) |
| `IndividualDetailDto` | Nested inside `CustomerDetailDto` | `FirstName`, `LastName`, `BirthDate?`, `GovernmentId` |
| `BusinessDetailDto` | Nested inside `CustomerDetailDto` | `BusinessName`, `TaxId`, `LegalRepresentative`, `Industry?`, `CreditLimit?` |

`CustomerDetailDto.Type` carries `[JsonConverter(typeof(JsonStringEnumConverter))]` (research.md
Decision 12). The same `CustomerDetailDto` shape is reused for both the paginated list and the
single-record `GET`, consistent with how `EmployeeDto`/`VehicleDto`/`BranchDto` are each reused across
list and detail today — `spec.md`'s "always returns full detail" requirement (FR-011) is satisfied
trivially since there is no separate summary shape.

## Validation rules

### Structural (FluentValidation, `ShipmentTracker.Services/Validators/Customers/`)

| Validator | Rule | FR |
|---|---|---|
| `CreateIndividualCustomerDtoValidator` | `Email`: required, valid email format | FR-001 |
| | `Phone`, `Address`, `City`, `State`, `ZipCode`, `Country`: required, not empty | FR-001 |
| | `FirstName`, `LastName`: required, not empty | FR-014 |
| | `GovernmentId`: required, matches the CURP regex (18 chars) | FR-014 |
| `CreateBusinessCustomerDtoValidator` | Same shared-field rules as above | FR-001 |
| | `BusinessName`, `LegalRepresentative`: required, not empty | FR-016 |
| | `TaxId`: required, matches the RFC-persona-moral regex (12 chars) | FR-016 |
| | `CreditLimit`: when provided, `>= 0` | FR-018 |
| `UpdateCustomerDtoValidator` | Same shared-field rules as Create (always required — shared fields exist on every customer type) | FR-001 |
| | `GovernmentId`, when provided: matches the CURP regex | FR-014 |
| | `TaxId`, when provided: matches the RFC-persona-moral regex | FR-016 |
| | `CreditLimit`, when provided: `>= 0` | FR-018 |
| | Does **not** enforce which type-specific fields are required/forbidden — that depends on the target's persisted `Type`, resolved only in the Service (research.md Decision 8) | FR-005, FR-013 |

### Database-dependent (`CustomerService`, research.md Decisions 7–8)

| Rule | Detail | FR |
|---|---|---|
| Email uniqueness | Compared against **all** customers, both types, active and inactive, excluding `currentId` on update | FR-002 |
| `GovernmentId` uniqueness | Compared against **all** `IndividualCustomer` rows only, active and inactive, excluding `currentId` | FR-015 |
| `TaxId` uniqueness | Compared against **all** `BusinessCustomer` rows only, active and inactive, excluding `currentId` | FR-017 |
| Type immutability | Update never accepts a `Type` field — cannot even attempt a change; enforced by the DTO shape itself, not a runtime check | FR-004 |
| Cross-type field rejection | On update, the service loads the existing customer, determines its concrete type, and rejects the request if any field belonging to the *other* type is non-null in `UpdateCustomerDto` | FR-013 |
| Type-appropriate completeness on update | After determining the target's type, the service re-applies that type's required-field rule (e.g. an Individual update must still carry `FirstName`/`LastName`/`GovernmentId`) | FR-005 |

All errors (structural + business-rule) are accumulated and reported together in a single
`FluentValidation.ValidationException`, thrown before any change is written — same "no partial write"
guarantee already established for `Branch`/`Employee`/`Vehicle`.

## New interfaces

`ShipmentTracker.Core/Interfaces/`:

```csharp
public interface ICustomerRepository : IBaseRepository<Customer> { } // no extra methods (research.md Decision 10)

public interface ICustomerService
{
    Task<CustomerDetailDto> CreateIndividualAsync(CreateIndividualCustomerDto dto); // throws ValidationException
    Task<CustomerDetailDto> CreateBusinessAsync(CreateBusinessCustomerDto dto);     // throws ValidationException
    Task<PagedResult<CustomerDetailDto>> GetCustomersAsync(bool onlyActive = true, CustomerType? type = null, int page = 1, int pageSize = 5);
    Task<CustomerDetailDto?> GetCustomerByIdAsync(int id);                          // null -> 404
    Task<CustomerDetailDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto);    // null -> 404; throws ValidationException
    Task<bool> DeactivateCustomerAsync(int id);                                     // false -> 404; idempotent
}
```

`IUnitOfWork` gains `ICustomerRepository CustomerRepository { get; }`, same lazy-property pattern as
every other repository.

## Flow: `CreateIndividualAsync` / `CreateBusinessAsync`

1. Trim `Email`, `GovernmentId`/`TaxId`, and the other identifying string fields on the DTO.
2. Run the type-appropriate structural validator; accumulate errors.
3. Check business rules: `Email` uniqueness (against all customers) + `GovernmentId`/`TaxId`
   uniqueness (against same-type customers only); accumulate errors.
4. If any error accumulated, throw `ValidationException` — no entity constructed, nothing written.
5. Construct `IndividualCustomer`/`BusinessCustomer` by hand (`Type` set explicitly by which method was
   called, `IsActive = true`, `CreatedAt = DateTime.UtcNow`, `UpdatedAt = null`), `AddAsync` +
   `CommitAsync`.
6. Return `_mapper.Map<CustomerDetailDto>(entity)`.

## Flow: `UpdateCustomerAsync`

1. Load the existing customer by id (`GetByIdAsync`); `null` → caller returns 404.
2. Trim the same string fields as create.
3. Run `UpdateCustomerDtoValidator` (shape-only, see Validation rules above); accumulate errors.
4. Using the loaded entity's actual runtime type, re-run that type's completeness rule (all required
   fields for that type must be present in the DTO) and reject any field that belongs to the *other*
   type (FR-013); accumulate errors.
5. Check `Email`/`GovernmentId`/`TaxId` uniqueness excluding `currentId = id`; accumulate errors.
6. If any error accumulated, throw `ValidationException` — entity left unchanged.
7. Apply the shared fields plus the type-appropriate fields onto the loaded entity, set `UpdatedAt =
   DateTime.UtcNow`, `Update()` + `CommitAsync`.
8. Return `_mapper.Map<CustomerDetailDto>(entity)`.

## Migration

A single new EF Core migration (`AddCustomers`, generated during the implementation phase) creates
three tables: `Customers` (base, unique index on `Email`), `IndividualCustomers` (PK/FK to
`Customers.Id`, unique index on `GovernmentId`), `BusinessCustomers` (PK/FK to `Customers.Id`, unique
index on `TaxId`). No existing table (`Shipments`, `Branches`, `BranchSchedules`, `Employees`,
`Vehicles`) is touched.
