# Quickstart: Validar el módulo de Branches & Hubs

Verificación manual (Swagger/HTTP), consistente con la política del proyecto (sin proyecto de
pruebas automatizadas).

## Prerrequisitos

- .NET 8.0 SDK, SQL Server local con la base migrada (ver `CLAUDE.md`).
- Aplicar la migración de esta feature (`AddBranchesAndSchedule`, ya generada) antes de probar:
  ```
  dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
  ```
- `opensAt`/`closesAt` en los ejemplos de esta guía usan formato `HH:mm:ss` (p. ej. `"08:00:00"`)
  — el conversor de `TimeOnly` de System.Text.Json no acepta `HH:mm` sin segundos.

## 1. Compilar y levantar

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de `spec.md`)

### User Story 1 — Registrar una sucursal (P1)

1. `POST /api/branches` con tipo `Hub`, dirección completa y un horario de 7 días (algunos con
   horario, `Sunday` cerrado): `201 Created`, `isActive: true`, `schedule` con las 7 entradas
   exactamente como se enviaron.
2. Repetir el `POST` anterior omitiendo `latitude`, `longitude` y `phone`: `201 Created` sin esos
   valores.
3. `POST` con un horario de 6 entradas: `400`. Repetir con 8 entradas: `400`. Repetir con dos
   entradas `Monday`: `400`. Repetir con una entrada abierta donde `opensAt >= closesAt`: `400`.
4. `POST` omitiendo `state` (o cualquier campo de dirección) y otro omitiendo `type`: ambos `400`.

### User Story 2 — Buscar y revisar sucursales (P2)

1. Crear sucursales de distinto `type` y con al menos una desactivada (ver US4). `GET
   /api/branches` sin parámetros: solo las activas.
2. `GET /api/branches?type=Hub`: solo activas de tipo `Hub`.
3. `GET /api/branches?onlyActive=false`: solo las inactivas.
4. `GET /api/branches?onlyActive=false&type=Hub`: solo inactivas de tipo `Hub`.
5. `GET /api/branches/{id}` de una sucursal existente: `200`, incluye `schedule` completo (7
   entradas) sin importar `isActive`.
6. `GET /api/branches/999999` (id inexistente): `404`.

### User Story 3 — Actualizar una sucursal (P3)

1. `PUT /api/branches/{id}` cambiando `address`, `phone` y `latitude`/`longitude`: `200`, `GET`
   posterior refleja los nuevos valores.
2. `PUT` con un `schedule` válido pero distinto al original (p. ej. horarios de fin de semana
   distintos): `200`, `GET` posterior devuelve el horario nuevo completo, no una mezcla con el
   anterior.
3. `PUT` con un `schedule` inválido (día duplicado, o un día cerrado con `opensAt` presente):
   `400`, y un `GET` posterior confirma que la sucursal **no cambió** (ni los campos escalares ni
   el horario).
4. Desactivar una sucursal (ver US4), luego `PUT` esa misma sucursal con `isActive: true`: `200`,
   la sucursal vuelve a aparecer en `GET /api/branches` sin filtros.

### User Story 4 — Desactivar una sucursal (P4)

1. `DELETE /api/branches/{id}` de una sucursal activa: `204`. `GET /api/branches` (sin filtros) ya
   no la incluye; `GET /api/branches/{id}` directo sigue devolviéndola con todos sus datos y
   `isActive: false`.
2. Repetir `DELETE /api/branches/{id}` sobre la misma sucursal ya inactiva: `204` de nuevo, sin
   error (idempotente).
3. `DELETE /api/branches/999999` (id inexistente): `404`.
4. Confirmar que no existe ningún endpoint de borrado físico — la única forma de "quitar" una
   sucursal en toda la API es `DELETE` (soft-delete).

### Edge cases adicionales

- `POST` con `latitude: 200` (fuera de rango): `400`.
- `POST` con un día `isClosed: true` que además trae `opensAt`/`closesAt`: `400` (no se ignoran en
  silencio los horarios — ver Clarifications de `spec.md`).
- `GET /api/branches?type=NoExiste`: `400` (valor de enum no reconocido), no una lista vacía.

## 3. Verificación de que nada más cambió

- `GET /api/shipment`, `GET /api/shipment/{trackingNumber}`, `POST /api/shipment`,
  `PATCH /api/shipment/{trackingNumber}/status`: sin ningún cambio de comportamiento — este módulo
  no toca ningún archivo de `Shipment`.

Si los escenarios de la sección 2 y la verificación de la sección 3 pasan, la implementación
cumple SC-001 a SC-005 de `spec.md`.
