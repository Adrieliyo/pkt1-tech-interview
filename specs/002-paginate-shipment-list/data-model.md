# Data Model: Paginación del Listado de Envíos

Esta feature no cambia el esquema persistido (`Shipment` en SQL Server no se modifica). Introduce
un tipo de transporte nuevo (`PagedResult<T>`) y fija el comportamiento de consulta (orden, filtro,
paginación) para `GET /api/shipment`.

## Nuevo tipo: `PagedResult<T>`

Vive en `ShipmentTracker.Core/DTOs/PagedResult.cs`. Genérico, sin dependencias externas.

| Campo | Tipo | Descripción |
|---|---|---|
| `Items` | `IEnumerable<T>` | Los registros de la página actual (para esta feature: `ShipmentDto`) |
| `Page` | `int` | Número de página solicitado (1-based) |
| `PageSize` | `int` | Tamaño de página efectivamente usado (ya con el tope de 50 aplicado, ver research.md Decisión 6) |
| `TotalCount` | `int` | Total de registros que cumplen el filtro (`status`), sin paginar |
| `TotalPages` | `int` (calculado) | `Ceiling(TotalCount / PageSize)`; `0` si `TotalCount` es `0` |

`PagedResult<T>` nunca se serializa directamente en una respuesta HTTP — el controlador extrae
`Items` para el cuerpo y el resto de los campos para los encabezados (ver `contracts/`).

## Consulta de `Shipment` para el listado paginado

| Aspecto | Valor |
|---|---|
| Filtro | `x => x.Status == status.Value` si se especifica `status`; sin filtro (todos) en caso contrario — mismo comportamiento de filtro que ya existía |
| Orden | `OrderByDescending(x => x.CreatedAt)` — fijado por FR-011, aplica siempre, con o sin paginación |
| Paginación | `Skip((page - 1) * effectivePageSize).Take(effectivePageSize)` |
| Conteo total | `CountAsync` con el mismo filtro que la consulta paginada (para que `TotalCount`/`TotalPages` reflejen el conjunto ya filtrado, FR-004) |

## Reglas de validación (sin cambios en `Shipment`, nuevas en los parámetros de consulta)

| Parámetro | Regla | Dónde se aplica |
|---|---|---|
| `page` | Entero, `>= 1`; no numérico o `<= 0` → `400` | `[Range(1, int.MaxValue)]` en `ShipmentController` (Web) |
| `pageSize` | Entero, `>= 1`; no numérico o `<= 0` → `400`; si excede 50, se recorta a 50 (no falla) | Límite inferior: `[Range]` (Web). Tope superior (50): `ShipmentService` (Services) |

## Sin cambios

- `Shipment` (entidad), `ShipmentDto`, `CreateShipmentDto`: forma idéntica a la de `001-standardize-mapping-di`.
- `GET /api/shipment/{trackingNumber}`, `POST /api/shipment`, `PATCH /api/shipment/{trackingNumber}/status`:
  sin cambios de ningún tipo (FR-008).
