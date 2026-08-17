# Contrato HTTP: API de Employees & Vehicles

Módulo nuevo — no hay contrato previo que preservar. Las diez rutas y sus verbos están fijados por
el input del usuario en `/speckit-plan`; la paginación de los listados reutiliza el contrato ya
establecido por `002-paginate-shipment-list` (ver
[`../../002-paginate-shipment-list/contracts/shipment-list-contract.md`](../../002-paginate-shipment-list/contracts/shipment-list-contract.md)).

## Empleados

### `POST /api/employees`

- **Body**: `CreateEmployeeDto`
- **201 Created**: `EmployeeDto` (`isActive: true`). Header `Location` apuntando a
  `GET /api/employees/{id}`.
- **400 Bad Request**: dirección/campos incompletos, `role` inválido u omitido, `hireDate` omitida,
  sucursal inexistente o inactiva, `email`/`employeeNumber` ya usados (activo o inactivo). Cuerpo:

  ```json
  { "errors": [ { "property": "Email", "message": "..." }, { "property": "BranchId", "message": "..." } ] }
  ```

### `GET /api/employees`

Paginado — mismo contrato de headers que `GET /api/shipment` (ver `002`).

| Param | Tipo | Default | Reglas |
|---|---|---|---|
| `branchId` | `int` | — (sin filtro) | Opcional |
| `role` | `EmployeeRole` (string del enum) | — (sin filtro) | Opcional; valor no reconocido → `400` |
| `page` | `int` | `1` | `>= 1`; no numérico o `<= 0` → `400` |
| `pageSize` | `int` | `5` | `>= 1`; `> 50` se recorta a `50` (no falla) |

- **200 OK**: `EmployeeDto[]` — solo empleados activos, sin importar los filtros aplicados
  (FR-010). Headers `X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`.
- `branchId` + `role=Driver` combinados: caso de uso principal del módulo (US2 de spec.md) — solo
  choferes activos de esa sucursal.

### `GET /api/employees/{id}`

- **200 OK**: `EmployeeDto` (activo o inactivo — la recuperación individual no filtra por estado).
- **404 Not Found**: `{ "message": "No se encontró un empleado con el id '{id}'." }`

### `PUT /api/employees/{id}`

- **Body**: `UpdateEmployeeDto` (reemplazo completo, incluye `isActive` para permitir reactivar).
- **200 OK**: `EmployeeDto` actualizado.
- **404 Not Found**: mismo formato que `GET /api/employees/{id}`.
- **400 Bad Request**: mismas reglas que `POST`. El empleado **no** se modifica si la validación
  falla.

### `DELETE /api/employees/{id}`

- **Efecto**: soft-delete — `isActive = false`. Nunca elimina la fila.
- **204 No Content**: éxito, incluido un empleado ya inactivo (idempotente).
- **404 Not Found**: mismo formato que `GET /api/employees/{id}`.

## Vehículos

### `POST /api/vehicles`

- **Body**: `CreateVehicleDto`
- **201 Created**: `VehicleDto` (`isActive: true`). Header `Location` apuntando a
  `GET /api/vehicles/{id}`.
- **400 Bad Request**: campos incompletos, `type` inválido u omitido, año futuro, capacidad `<= 0`,
  sucursal inexistente o inactiva, `plate` ya usada (activa o inactiva).

### `GET /api/vehicles`

Paginado, mismo contrato que `GET /api/employees`.

| Param | Tipo | Default | Reglas |
|---|---|---|---|
| `branchId` | `int` | — (sin filtro) | Opcional — **no** `guid`, ver research.md Decisión 4 |
| `page` | `int` | `1` | `>= 1` |
| `pageSize` | `int` | `5` | `>= 1`; `> 50` se recorta a `50` |

- **200 OK**: `VehicleDto[]` — solo vehículos activos. Headers `X-Total-Count`, `X-Page`,
  `X-Page-Size`, `X-Total-Pages`.

### `GET /api/vehicles/{id}`

- **200 OK**: `VehicleDto` (activo o inactivo).
- **404 Not Found**: `{ "message": "No se encontró un vehículo con el id '{id}'." }`

### `PUT /api/vehicles/{id}`

- **Body**: `UpdateVehicleDto` (reemplazo completo, incluye `isActive`).
- **200 OK**: `VehicleDto` actualizado.
- **404 Not Found** / **400 Bad Request**: mismas reglas que Empleados.

### `DELETE /api/vehicles/{id}`

- **Efecto**: soft-delete — `isActive = false`. Nunca elimina la fila.
- **204 No Content**: éxito, incluido un vehículo ya inactivo (idempotente).
- **404 Not Found**: mismo formato que `GET /api/vehicles/{id}`.

## Notas de formato JSON

- `role`/`type` se serializan y deben enviarse como el **nombre** del valor del enum (p. ej.
  `"Driver"`, `"Van"`), igual que `Branch.type` — ver research.md Decisión 11.
- `hireDate` es `DateOnly` — formato `YYYY-MM-DD` (p. ej. `"2026-08-17"`), sin componente de hora.

## Ejemplo

```
POST /api/employees
{
  "branchId": 1,
  "firstName": "Ana",
  "lastName": "García",
  "email": "ana.garcia@example.com",
  "phone": "+1-555-0199",
  "role": "Driver",
  "employeeNumber": "E-1001",
  "hireDate": "2026-08-17"
}

201 Created
Location: /api/employees/1

{ "id": 1, "branchId": 1, "firstName": "Ana", "lastName": "García", "email": "ana.garcia@example.com",
  "phone": "+1-555-0199", "role": "Driver", "employeeNumber": "E-1001", "hireDate": "2026-08-17",
  "isActive": true, "createdAt": "2026-08-17T...", "updatedAt": null }
```

```
GET /api/employees?branchId=1&role=Driver

200 OK
X-Total-Count: 3
X-Page: 1
X-Page-Size: 5
X-Total-Pages: 1

[ { "id": 1, "firstName": "Ana", "role": "Driver", ... }, ... ]
```
