# Quickstart: Multiple Shipments per Order

Validación manual vía Swagger UI (`/swagger` en Development), igual que el resto de módulos — no hay proyecto de pruebas automatizadas en la solución.

## Prerrequisitos

- API corriendo (`dotnet run --project ShipmentTracker.Web`) con base de datos migrada.
- Sesión autenticada como `BranchManager` (necesario para `POST /convert`; `Operator` alcanza para el resto).
- Un `Customer` activo existente (reutilizar uno de los escenarios de `specs/006-orders/quickstart.md` si aplica).

## Escenario 1 — Generar un segundo envío sobre la misma orden

1. `POST /api/orders` con datos válidos → orden en `Pending`.
2. `POST /api/orders/{id}/confirm` → orden pasa a `Confirmed`.
3. `POST /api/orders/{id}/convert` → 200, devuelve `{ shipmentId: A, trackingNumber: "TRK-..." }`. Orden pasa a `Converted`.
4. `GET /api/orders/{id}` → verificar `status: "Converted"`, `shipmentsCount: 1`, `isFulfilled: false` (el envío recién creado nace en `Collected`, no `Delivered`).
5. `POST /api/orders/{id}/convert` de nuevo, sobre la misma orden → **esperado: 200** (antes de esta feature, esperaba 400). Devuelve `{ shipmentId: B, trackingNumber: "TRK-..." }` con `B != A` y un `trackingNumber` distinto del paso 3.
6. `GET /api/orders/{id}` → `shipmentsCount: 2`, `isFulfilled: false`.

## Escenario 2 — Listar los envíos de una orden

1. Sobre la orden del Escenario 1 (con 2 envíos ya generados): `GET /api/orders/{id}/shipments?page=1&pageSize=5`.
2. Verificar respuesta 200 con 2 elementos (`ShipmentDto`), headers `X-Total-Count: 2`, `X-Page: 1`, `X-Page-Size: 5`, `X-Total-Pages: 1`.
3. Crear una tercera orden nueva (Pending, sin confirmar ni convertir) y llamar `GET /api/orders/{id}/shipments` sobre ella → 200 con lista vacía, `X-Total-Count: 0`.
4. `GET /api/orders/999999/shipments` (id inexistente) → 404.

## Escenario 3 — Rechazo de conversión sobre orden cancelada

1. Crear una orden nueva, dejarla en `Pending`, cancelarla: `DELETE /api/orders/{id}`.
2. `POST /api/orders/{id}/convert` → 400 `{ message: "Only confirmed orders can be converted to a shipment." }`.

## Escenario 4 — Cumplimiento agregado (`isFulfilled`)

1. Tomar la orden del Escenario 1 (2 envíos, A y B).
2. Usando los endpoints existentes de `ShipmentEvent`/`Shipment` (fuera de esta feature, sin cambios), avanzar el envío A hasta `Delivered`. Dejar B en `Collected`.
3. `GET /api/orders/{id}` → `isFulfilled: false` (B no está `Delivered`).
4. Avanzar B también hasta `Delivered`.
5. `GET /api/orders/{id}` → `isFulfilled: true`.
6. Generar un tercer envío sobre esa misma orden (`POST /convert` de nuevo) → `GET /api/orders/{id}` debe volver a mostrar `isFulfilled: false` hasta que el tercer envío también se entregue.

## Escenario 5 — Envíos cancelados no bloquean el cumplimiento

1. Orden con 2 envíos: A en `Delivered`, B cancelado (vía el flujo existente de cancelación de `Shipment`, fuera de esta feature).
2. `GET /api/orders/{id}` → `isFulfilled: true` (B se excluye del cálculo por estar `Cancelled`).

## Escenario 6 — Todos los envíos cancelados

1. Orden con 1 solo envío, cancelado.
2. `GET /api/orders/{id}` → `isFulfilled: false` (no hay ningún envío no cancelado que esté `Delivered`).

Ver `contracts/orders-shipments.md` para la forma exacta de cada respuesta.
