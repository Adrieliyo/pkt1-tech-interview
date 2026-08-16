# Contrato HTTP: `GET /api/shipment` con paginación

Este documento fija el contrato actualizado de `GET /api/shipment` tras esta feature. Los otros
tres endpoints (`GET /api/shipment/{trackingNumber}`, `POST /api/shipment`,
`PATCH /api/shipment/{trackingNumber}/status`) **no cambian** — ver
[`../../001-standardize-mapping-di/contracts/shipment-api-contract.md`](../../001-standardize-mapping-di/contracts/shipment-api-contract.md)
para su contrato vigente.

## `GET /api/shipment`

### Query params (todos opcionales)

| Param | Tipo | Default | Reglas |
|---|---|---|---|
| `status` | `ShipmentStatus` (int) | — (sin filtro) | Sin cambios respecto al contrato existente |
| `page` | `int` | `1` | `>= 1`; no numérico o `<= 0` → `400` |
| `pageSize` | `int` | `5` | `>= 1`; no numérico o `<= 0` → `400`; valores `> 50` se recortan a `50` (no fallan) |

### Respuesta `200 OK`

- **Cuerpo**: `ShipmentDto[]` — **idéntico en forma** al que devuelve hoy (FR-007); ahora contiene
  como máximo `pageSize` elementos, ordenados por `createdAt` descendente.
- **Encabezados nuevos**:

  | Header | Valor |
  |---|---|
  | `X-Total-Count` | Total de envíos que cumplen el filtro `status` (sin paginar) |
  | `X-Page` | Página devuelta (igual al `page` solicitado) |
  | `X-Page-Size` | Tamaño de página efectivamente usado (ya recortado al tope de 50 si aplica) |
  | `X-Total-Pages` | `Ceiling(X-Total-Count / X-Page-Size)` |

  Estos 4 encabezados están expuestos vía CORS (`Access-Control-Expose-Headers`) para el frontend
  en `localhost:3000`/`5173` — ver research.md Decisión 4.

- **Página fuera de rango** (p. ej. `page=100` con solo 3 páginas de resultados): `200 OK` con
  cuerpo `[]` (arreglo vacío) y los mismos encabezados (`X-Total-Count` refleja el total real).

### Respuesta `400 Bad Request`

Cuando `page` o `pageSize` son inválidos (no numérico, `<= 0`): `ValidationProblemDetails` estándar
de ASP.NET Core (formato RFC 7807), generado automáticamente por `[ApiController]` — no requiere
código de manejo de errores adicional.

## Ejemplo

```
GET /api/shipment?status=1&page=2&pageSize=10

200 OK
X-Total-Count: 23
X-Page: 2
X-Page-Size: 10
X-Total-Pages: 3

[ { "id": 15, "trackingNumber": "TRK-...", ... }, ... ]  // hasta 10 elementos
```
