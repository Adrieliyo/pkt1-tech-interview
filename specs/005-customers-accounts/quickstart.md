# Quickstart: Validating the Customers & Accounts Module

Manual verification (Swagger/HTTP), consistent with this project's policy (no automated test
project).

## Prerequisites

- .NET 8.0 SDK, local SQL Server with the database migrated (see `CLAUDE.md`).
- Apply this feature's migration before testing:
  ```
  dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
  ```
- No dependency on any other module (`Branch`, `Employee`, `Vehicle`, `Shipment`) — `Customer` is
  fully self-contained, so no prerequisite records from other modules are needed.

## 1. Build and run

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)

### User Story 1 — Register a new customer (P1)

1. `POST /api/customers/individual` with a unique `email`/`governmentId` (valid CURP shape, 18 chars)
   and all required fields: `201`, `isActive: true`, `type: "Individual"`.
2. `POST /api/customers/business` with a unique `email`/`taxId` (valid RFC shape, 12 chars): `201`,
   `type: "Business"`.
3. Repeat either `POST` with an `email` already used by the other type's customer: `400`.
4. Repeat with a `governmentId`/`taxId` already used by another customer of the *same* type: `400`.
5. `POST /api/customers/individual` omitting `lastName`: `400`. `POST /api/customers/business`
   omitting `taxId`: `400`.
6. `POST /api/customers/individual` with a `governmentId` that doesn't match the CURP shape (wrong
   length/characters): `400`, distinct message from the duplicate-value case.

### User Story 2 — Find and review customers (P2)

1. Create both an Individual and a Business customer, plus deactivate a third one (via `DELETE`).
   `GET /api/customers` with no filters: only active customers.
2. `GET /api/customers?type=Business`: only active Business customers.
3. `GET /api/customers?onlyActive=false`: only inactive customers.
4. `GET /api/customers?onlyActive=false&type=Individual`: only inactive Individual customers
   (combined filter).
5. `GET /api/customers/{id}` of the Individual customer: response includes `individual` populated,
   `business: null`.
6. `GET /api/customers/{id}` of the Business customer: response includes `business` populated,
   `individual: null`.
7. `GET /api/customers/999999`: `404`.

### User Story 3 — Update customer information (P3)

1. `PUT /api/customers/{id}` on the Individual customer, changing `address` and `birthDate` to valid
   values: `200`, reflects the new values.
2. `PUT /api/customers/{id}` on the Business customer, changing `creditLimit` to a valid non-negative
   value: `200`.
3. `PUT` on the Individual customer including a Business-only field (e.g. `taxId` non-null): `400`
   (FR-013), customer unchanged.
4. `PUT` omitting a required Individual field (e.g. `lastName`) on the Individual customer: `400`
   (FR-005), customer unchanged.
5. `PUT` changing `email` to a value already used by another customer: `400`, customer unchanged.
6. `PUT` changing `governmentId`/`taxId` to a value already used by another customer of the same
   type: `400`, customer unchanged.
7. `PUT` on an inactive customer with `isActive: true`: `200`, customer becomes active again and
   reappears in default (`onlyActive=true`) listings.

### User Story 4 — Deactivate a customer (P4)

1. `DELETE /api/customers/{id}` of an active customer: `204`. No longer appears in
   `GET /api/customers` with default filters.
2. `GET /api/customers/{id}` of that now-inactive customer: `200`, all fields — including
   type-specific ones — still returned intact.
3. Repeat the same `DELETE`: `204` again, no error (idempotent).
4. Confirm there is no endpoint that physically removes a customer — `DELETE` is the only retirement
   path.

### Additional edge cases

- `POST /api/customers/individual` with an `email`/`governmentId` that only differs by case or
  leading/trailing whitespace from an existing one: `400` (case-insensitive, trimmed uniqueness).
- Deactivate an Individual customer, then attempt to create a new one with the same `governmentId`:
  `400` — uniqueness applies even against inactive records.
- `POST /api/customers/business` with `creditLimit: -100`: `400`.
- `GET /api/customers?type=NotARealType`: `400`, not an empty list.
- `GET /api/customers?pageSize=1000`: does not fail; response has `X-Page-Size: 50`.

## 3. Verify nothing else changed

- `GET/POST/PUT/DELETE` on `/api/shipment`, `/api/branches`, `/api/employees`, `/api/vehicles`: no
  behavior change — this module adds a new `Customers`/`IndividualCustomers`/`BusinessCustomers`
  table set and touches no existing table or endpoint.

If every scenario in section 2 and the check in section 3 pass, the implementation satisfies SC-001
through SC-005 of `spec.md`.
