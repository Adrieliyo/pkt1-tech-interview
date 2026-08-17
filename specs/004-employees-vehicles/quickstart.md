# Quickstart: Validar el módulo de Employees & Vehicles

Verificación manual (Swagger/HTTP), consistente con la política del proyecto (sin proyecto de
pruebas automatizadas).

## Prerrequisitos

- .NET 8.0 SDK, SQL Server local con la base migrada (ver `CLAUDE.md`).
- Al menos una sucursal **activa** existente (vía `POST /api/branches` del módulo 003) — todas las
  pruebas de este módulo dependen de tener un `branchId` activo a mano. Se recomienda también tener
  una sucursal **inactiva** disponible (desactivada vía `DELETE /api/branches/{id}`) para probar el
  rechazo por sucursal inactiva.
- Aplicar la migración de esta feature (`AddEmployeesAndVehicles`) antes de probar:
  ```
  dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
  ```

## 1. Compilar y levantar

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de `spec.md`)

### User Story 1 — Registrar un empleado (P1)

1. `POST /api/employees` con una sucursal activa, `role: "Driver"`, `employeeNumber`/`email` únicos
   y `hireDate`: `201`, `isActive: true`.
2. Repetir el `POST` anterior con el mismo `employeeNumber` (u otro `email` ya usado): `400`.
3. `POST` con `branchId` de una sucursal inactiva (o un id inexistente): `400`.
4. `POST` con `role: "NoExiste"`: `400`.

### User Story 2 — Buscar choferes disponibles en una sucursal (P2)

1. Crear empleados de distintos roles en la misma sucursal (vía US1). `GET
   /api/employees?branchId={id}&role=Driver`: solo choferes activos de esa sucursal.
2. `GET /api/employees?branchId={id}`: todos los roles activos de esa sucursal.
3. `GET /api/employees?role=Driver`: choferes activos de cualquier sucursal.
4. `GET /api/employees?branchId={id}&role=Driver` sobre una sucursal sin choferes: `200` con
   arreglo vacío, no error.
5. `GET /api/employees/{id}` de un empleado existente: `200` con todos sus detalles.
6. `GET /api/employees/999999` (id inexistente): `404`.

### User Story 3 — Registrar un vehículo (P3)

1. `POST /api/vehicles` con una sucursal activa, `type: "Van"`, `plate` única, `year` válido y
   `maxWeightKg` positivo: `201`, `isActive: true`.
2. Repetir con la misma `plate`: `400`.
3. `POST` con sucursal inactiva o inexistente: `400`.
4. `POST` con `type: "NoExiste"`: `400`.

### User Story 4 — Ver la flota de una sucursal (P4)

1. Crear vehículos en distintas sucursales (vía US3). `GET /api/vehicles?branchId={id}`: solo
   vehículos activos de esa sucursal.
2. `GET /api/vehicles?branchId={id}` sobre una sucursal sin vehículos: `200` con arreglo vacío.
3. `GET /api/vehicles` sin filtro: todos los vehículos activos de todas las sucursales.
4. `GET /api/vehicles/{id}` de un vehículo existente: `200` con todos sus detalles.
5. `GET /api/vehicles/999999`: `404`.

### User Story 5 — Actualizar un empleado (P5)

1. `PUT /api/employees/{id}` cambiando `role`, `hireDate`, `employeeNumber` o `email` a valores
   válidos y no conflictivos: `200`, refleja los nuevos valores.
2. `PUT` reasignando `branchId` a otra sucursal activa: `200`; el empleado deja de aparecer en
   `GET /api/employees?branchId={sucursalVieja}` y aparece en la nueva.
3. `PUT` con `employeeNumber`/`email` ya usados por otro empleado: `400`, el empleado queda
   intacto.
4. `PUT` reasignando a una sucursal inactiva o inexistente: `400`, el empleado queda intacto.

### User Story 6 — Actualizar un vehículo (P6)

1. `PUT /api/vehicles/{id}` cambiando `brand`, `model`, `year` o `maxWeightKg` a valores válidos:
   `200`.
2. `PUT` reasignando `branchId` a otra sucursal activa: `200`; el vehículo cambia de sucursal en los
   listados filtrados.
3. `PUT` con `plate` ya usada por otro vehículo: `400`, el vehículo queda intacto.
4. `PUT` reasignando a una sucursal inactiva o inexistente: `400`, el vehículo queda intacto.

### User Story 7 — Desactivar un empleado (P7)

1. `DELETE /api/employees/{id}` de un empleado activo: `204`. Deja de aparecer en cualquier
   listado, incluida la búsqueda de choferes por sucursal.
2. Repetir el mismo `DELETE`: `204` de nuevo, sin error (idempotente).
3. `DELETE /api/employees/999999`: `404`.
4. Confirmar que no existe ningún endpoint de borrado físico — `DELETE` es la única forma de
   retirar un empleado.

### User Story 8 — Desactivar un vehículo (P8)

1. `DELETE /api/vehicles/{id}` de un vehículo activo: `204`. Deja de aparecer en cualquier listado.
2. Repetir el mismo `DELETE`: `204` de nuevo, sin error.
3. `DELETE /api/vehicles/999999`: `404`.

### Edge cases adicionales

- `POST /api/employees` con `email`/`employeeNumber` que solo difieren en mayúsculas o espacios de
  un registro existente: `400` (unicidad case-insensitive y sin espacios — research.md Decisión 8).
- Desactivar un empleado y luego intentar crear uno nuevo con su mismo `employeeNumber`/`email`:
  `400` — la unicidad aplica incluso contra registros inactivos.
- `POST /api/vehicles` con `year` en el futuro: `400`. Con `maxWeightKg: 0` o negativo: `400`.
- `GET /api/employees?role=NoExiste` o `GET /api/employees?branchId=abc`: `400`, no una lista
  vacía.
- `GET /api/employees?pageSize=1000`: no falla; `X-Page-Size: 50` en la respuesta.

## 3. Verificación de que nada más cambió

- `GET /api/shipment`, `POST /api/shipment`, etc.: sin ningún cambio de comportamiento.
- `POST/GET/PUT/DELETE /api/branches`: sin ningún cambio de comportamiento — este módulo solo lee
  `Branch` vía `IUnitOfWork.BranchRepository`, nunca lo modifica.

Si todos los escenarios de la sección 2 y las verificaciones de la sección 3 pasan, la
implementación cumple SC-001 a SC-005 de `spec.md`.
