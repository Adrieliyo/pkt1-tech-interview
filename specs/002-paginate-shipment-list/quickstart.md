# Quickstart: Validar la paginación de `GET /api/shipment`

Verificación manual (Swagger/HTTP), consistente con la política del proyecto (sin proyecto de
pruebas automatizadas).

## Prerrequisitos

- .NET 8.0 SDK, SQL Server local con la base `pkt1` migrada (ver `CLAUDE.md`).
- Al menos 12 envíos existentes para poder probar una segunda página completa (la seed inicial trae
  5; puedes crear más vía `POST /api/shipment` o reutilizar los que ya hayas creado en features
  anteriores).

## 1. Compilar y levantar

```
dotnet build ShipmentTracker.sln
dotnet run --project ShipmentTracker.Web
```

## 2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de spec.md)

1. **Default (US1, escenario 1)** — `GET /api/shipment` sin parámetros, con más de 5 envíos
   existentes: cuerpo con exactamente 5 elementos, ordenados por `createdAt` descendente (el más
   reciente primero). Encabezados `X-Total-Count` (total real), `X-Page: 1`, `X-Page-Size: 5`,
   `X-Total-Pages` coherente.
2. **Pocos registros (US1, escenario 2)** — si en algún momento hay 5 o menos envíos totales:
   `GET /api/shipment` sin parámetros devuelve todos, `X-Total-Pages: 1`.
3. **Tamaño de página mayor (US2, escenario 1)** — `GET /api/shipment?pageSize=10` con 12+ envíos:
   10 elementos en el cuerpo.
4. **Segunda página (US2, escenario 2)** — `GET /api/shipment?pageSize=5&page=2` con 12+ envíos:
   los envíos 6–10 (por orden de `createdAt` descendente), ninguno repetido respecto a
   `GET /api/shipment?pageSize=5&page=1`.
5. **Parámetro inválido (US2, escenario 3)** — `GET /api/shipment?page=0`,
   `GET /api/shipment?pageSize=-1`, `GET /api/shipment?page=abc`: los tres devuelven `400`.
6. **Página fuera de rango (Edge Case)** — `GET /api/shipment?page=9999`: `200 OK`, cuerpo `[]`,
   `X-Total-Count` sigue reflejando el total real.
7. **Combinado con `status` (Edge Case / FR-004)** — `GET /api/shipment?status=1&pageSize=2`:
   `X-Total-Count` refleja solo los envíos con `status=1` (InTransit), no el total general.
8. **Tope de `pageSize` (FR-010)** — `GET /api/shipment?pageSize=1000`: no falla; `X-Page-Size: 50`
   en la respuesta, y el cuerpo trae como máximo 50 elementos.
9. **CORS de los encabezados (research.md, Decisión 4)** — desde el navegador (o `curl -i` para
   inspeccionar), confirmar que la respuesta incluye
   `Access-Control-Expose-Headers: X-Total-Count, X-Page, X-Page-Size, X-Total-Pages` cuando la
   solicitud lleva `Origin: http://localhost:5173` (o 3000).
10. **`page` extremo (research.md, Decisión 8)** — `GET /api/shipment?page=2000000000`: `200 OK`,
    cuerpo `[]`, sin error `500`; `X-Total-Count` sigue reflejando el total real.

## 3. Verificación de que nada más cambió

- `GET /api/shipment/{trackingNumber}`, `POST /api/shipment`,
  `PATCH /api/shipment/{trackingNumber}/status`: mismos escenarios de
  [`../001-standardize-mapping-di/quickstart.md`](../001-standardize-mapping-di/quickstart.md),
  sin ninguna diferencia (FR-008).
- La forma de cada elemento dentro del arreglo de `GET /api/shipment` (campos de `ShipmentDto`) es
  idéntica a la de antes (FR-007).

Si los 10 escenarios de la sección 2 y las verificaciones de la sección 3 pasan, el fix cumple
SC-001 a SC-004 de `spec.md`.
