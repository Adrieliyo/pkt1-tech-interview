# HTTP Contract: Customers & Accounts API

New module — no prior contract to preserve. The six routes and their verbs are fixed by the user's
`/speckit-plan` input; list pagination reuses the contract already established by
`002-paginate-shipment-list` (see
[`../../002-paginate-shipment-list/contracts/shipment-list-contract.md`](../../002-paginate-shipment-list/contracts/shipment-list-contract.md)),
following the same `onlyActive`/`type` filter shape already used by `GET /api/branches`.

## `POST /api/customers/individual`

- **Body**: `CreateIndividualCustomerDto`
- **201 Created**: `CustomerDetailDto` (`type: "Individual"`, `isActive: true`, `individual` populated,
  `business: null`). Header `Location` pointing to `GET /api/customers/{id}`.
- **400 Bad Request**: missing/empty shared or Individual-only field, invalid `email` format,
  `governmentId` not matching the CURP structure, `email` or `governmentId` already in use (active or
  inactive). Body:

  ```json
  { "errors": [ { "property": "Email", "message": "..." }, { "property": "GovernmentId", "message": "..." } ] }
  ```

## `POST /api/customers/business`

- **Body**: `CreateBusinessCustomerDto`
- **201 Created**: `CustomerDetailDto` (`type: "Business"`, `isActive: true`, `business` populated,
  `individual: null`). Header `Location` pointing to `GET /api/customers/{id}`.
- **400 Bad Request**: missing/empty shared or Business-only required field, invalid `email` format,
  `taxId` not matching the RFC-persona-moral structure, negative `creditLimit`, `email` or `taxId`
  already in use (active or inactive). Same error body shape as above.

## `GET /api/customers`

Paginated — same header contract as `GET /api/branches`, `GET /api/employees`, `GET /api/vehicles`.

| Param | Type | Default | Rules |
|---|---|---|---|
| `onlyActive` | `bool` | `true` | `true` → only active customers; `false` → only inactive customers (never "all", matching `BranchController.GetBranches`) |
| `type` | `CustomerType` (enum name) | — (no filter) | Optional; unrecognized value → `400` |
| `page` | `int` | `1` | `>= 1`; non-numeric or `<= 0` → `400` |
| `pageSize` | `int` | `5` | `>= 1`; `> 50` is clamped to `50` (never rejected) |

- **200 OK**: `CustomerDetailDto[]` — the page matching the filters, ordered by `createdAt` descending.
  Headers `X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`.
- `onlyActive=false&type=Business`: all inactive Business customers — the combined-filter case from
  User Story 2's acceptance scenario 4.

## `GET /api/customers/{id}`

- **200 OK**: `CustomerDetailDto` (active or inactive — single-record retrieval never filters by
  status), always including the type-specific nested detail (FR-011).
- **404 Not Found**: `{ "message": "No customer was found with id '{id}'." }`

## `PUT /api/customers/{id}`

- **Body**: `UpdateCustomerDto` (shared fields + `isActive`, required; type-specific fields from both
  subtypes present but only the ones matching the customer's existing type may be non-null — see
  research.md Decision 8). Also usable to reactivate an inactive customer by sending `isActive: true` —
  no separate "activate" action exists.
- **200 OK**: updated `CustomerDetailDto`.
- **404 Not Found**: same shape as `GET /api/customers/{id}`.
- **400 Bad Request**: same structural rules as the matching `POST`, plus: any field belonging to the
  customer's *other* type is non-null (FR-013), a required field for the customer's actual type is
  missing (FR-005), or `email`/`governmentId`/`taxId` collides with a different customer (FR-002,
  FR-015, FR-017). The customer is **not** modified if validation fails.
- Attempting to submit a value implying a type change is impossible by construction — `UpdateCustomerDto`
  has no `type` property at all (FR-004).

## `DELETE /api/customers/{id}`

- **Effect**: soft-delete — `isActive = false`. The row is never removed; all type-specific fields
  remain intact and retrievable via `GET /api/customers/{id}`.
- **204 No Content**: success, including for an already-inactive customer (idempotent).
- **404 Not Found**: same shape as `GET /api/customers/{id}`.

## JSON format notes

- `type` (on `CustomerDetailDto`) is serialized and must be sent as the enum's **name**
  (`"Individual"` / `"Business"`), same convention as `Branch.type`/`Employee.role`/`Vehicle.type`.
- `birthDate` is `DateOnly` — format `YYYY-MM-DD` (e.g. `"1990-05-14"`), no time component, same as
  `Employee.hireDate`.
- `creditLimit`, when present, is a plain decimal number with no currency unit assumed (Assumptions,
  `spec.md`).

## Examples

```
POST /api/customers/individual
{
  "email": "maria.lopez@example.com",
  "phone": "+52-55-0101-0101",
  "address": "Av. Reforma 123",
  "city": "Ciudad de México",
  "state": "CDMX",
  "zipCode": "06600",
  "country": "México",
  "firstName": "María",
  "lastName": "López",
  "birthDate": "1990-05-14",
  "governmentId": "LOMA900514MDFPRR08"
}

201 Created
Location: /api/customers/1

{ "id": 1, "type": "Individual", "email": "maria.lopez@example.com", "phone": "+52-55-0101-0101",
  "address": "Av. Reforma 123", "city": "Ciudad de México", "state": "CDMX", "zipCode": "06600",
  "country": "México", "isActive": true, "createdAt": "2026-08-17T...", "updatedAt": null,
  "individual": { "firstName": "María", "lastName": "López", "birthDate": "1990-05-14",
    "governmentId": "LOMA900514MDFPRR08" },
  "business": null }
```

```
GET /api/customers?type=Business&onlyActive=true

200 OK
X-Total-Count: 2
X-Page: 1
X-Page-Size: 5
X-Total-Pages: 1

[ { "id": 2, "type": "Business", "business": { "businessName": "Acme SA de CV", "taxId": "ACM120101AB1", ... }, "individual": null, ... }, ... ]
```
