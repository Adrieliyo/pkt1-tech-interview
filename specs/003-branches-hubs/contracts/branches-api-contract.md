# Contrato HTTP: API de Branches & Hubs

Módulo nuevo — no hay contrato previo que preservar. Las cinco rutas y sus verbos están fijados
por el input del usuario en `/speckit-plan`.

## `POST /api/branches`

- **Body**: `CreateBranchDto`
- **201 Created**: `BranchDto` (con `IsActive: true`, `Schedule` con las 7 entradas guardadas).
  Header `Location` apuntando a `GET /api/branches/{id}`.
- **400 Bad Request**: cuando la validación falla (dirección incompleta, tipo faltante, horario
  con menos/más de 7 días, día duplicado, horario abierto con `OpensAt >= ClosesAt`, día cerrado
  con horarios, o coordenadas fuera de rango). Cuerpo:

  ```json
  { "errors": [ { "property": "Schedule", "message": "..." }, { "property": "State", "message": "..." } ] }
  ```

  Puede incluir más de un error simultáneamente (research.md, Decisión 9).

## `GET /api/branches`

### Query params (todos opcionales)

| Param | Tipo | Default | Reglas |
|---|---|---|---|
| `onlyActive` | `bool` | `true` | Sin filtro adicional posible más allá de activas/inactivas (research.md, Decisión 7) |
| `type` | `BranchType` (string del enum) | — (sin filtro) | Valor no reconocido → `400` (binding de `[FromQuery]` sobre un enum) |

### Respuesta `200 OK`

- **Cuerpo**: `BranchDto[]`, sin paginar (fuera de alcance de esta feature).
- Sin parámetros: solo sucursales activas (FR-014).
- `onlyActive=true`: mismo resultado que sin parámetros.
- `onlyActive=false`: solo sucursales inactivas.
- `type=Hub` (u otro valor válido del enum): combinado con `onlyActive` (AND lógico).

## `GET /api/branches/{id}`

- **200 OK**: `BranchDto` — **siempre** incluye `Schedule` completo (FR-015), sin importar
  `IsActive`.
- **404 Not Found**: `{ "message": "No se encontró una sucursal con el id '{id}'." }`

## `PUT /api/branches/{id}`

- **Body**: `UpdateBranchDto` (reemplazo completo — incluye las 7 entradas de `Schedule`, no un
  patch parcial; ver Assumptions de `spec.md`).
- **200 OK**: `BranchDto` actualizado.
- **404 Not Found**: mismo formato que `GET /api/branches/{id}`.
- **400 Bad Request**: mismas reglas y mismo formato que `POST /api/branches`. La sucursal
  **no** se modifica si la validación falla (FR-009).
- Reactivar una sucursal inactiva: enviar `IsActive: true` en el body — no existe un endpoint
  `PATCH .../activate` separado (research.md, Decisión 4 aplicada por analogía; ver Assumptions
  de `spec.md`, "Reactivación vía update").

## `DELETE /api/branches/{id}`

- **Efecto**: soft-delete — pone `IsActive = false`. **Nunca** elimina la fila ni su horario
  (FR-012). Ver research.md, Decisión 8.
- **204 No Content**: éxito, incluida una sucursal ya inactiva (idempotente, FR-011).
- **404 Not Found**: mismo formato que `GET /api/branches/{id}`.

## Forma de `BranchDto`

Ver tabla de mapeo en [`../data-model.md`](../data-model.md). Campos: `id`, `name`, `type`,
`address`, `city`, `state`, `zipCode`, `latitude`, `longitude`, `phone`, `isActive`, `createdAt`,
`schedule` (arreglo de `ScheduleEntryDto`: `id`, `dayOfWeek`, `isClosed`, `opensAt`, `closesAt`).

## Notas de formato JSON

- `type` y `dayOfWeek` se serializan y deben enviarse como el **nombre** del valor del enum (p.
  ej. `"Hub"`, `"Monday"`), no como el número subyacente — los DTOs de Branch aplican
  `JsonStringEnumConverter` por propiedad para esto, sin afectar la serialización de
  `ShipmentStatus` en el resto de la API (que permanece numérica, sin cambios).
- `opensAt`/`closesAt` deben enviarse en formato `HH:mm:ss` (p. ej. `"08:00:00"`) — el conversor
  de `TimeOnly` de System.Text.Json no acepta `HH:mm` sin segundos.

## Ejemplo

```
POST /api/branches
{
  "name": "Sucursal Centro",
  "type": "Hub",
  "address": "Av. Siempre Viva 123",
  "city": "Springfield",
  "state": "IL",
  "zipCode": "62704",
  "phone": "+1-555-0100",
  "schedule": [
    { "dayOfWeek": "Monday", "isClosed": false, "opensAt": "08:00:00", "closesAt": "18:00:00" },
    { "dayOfWeek": "Tuesday", "isClosed": false, "opensAt": "08:00:00", "closesAt": "18:00:00" },
    { "dayOfWeek": "Wednesday", "isClosed": false, "opensAt": "08:00:00", "closesAt": "18:00:00" },
    { "dayOfWeek": "Thursday", "isClosed": false, "opensAt": "08:00:00", "closesAt": "18:00:00" },
    { "dayOfWeek": "Friday", "isClosed": false, "opensAt": "08:00:00", "closesAt": "18:00:00" },
    { "dayOfWeek": "Saturday", "isClosed": false, "opensAt": "09:00:00", "closesAt": "13:00:00" },
    { "dayOfWeek": "Sunday", "isClosed": true }
  ]
}

201 Created
Location: /api/branches/1

{ "id": 1, "name": "Sucursal Centro", "type": "Hub", ..., "isActive": true, "schedule": [ ... 7 entradas ... ] }
```
