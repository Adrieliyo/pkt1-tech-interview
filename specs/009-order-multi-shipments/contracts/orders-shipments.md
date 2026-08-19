# API Contract: Multiple Shipments per Order

Todos los endpoints ya existen bajo `/api/orders`, salvo el nuevo listado de envíos. Autenticación cookie-based ya vigente (módulo `008`); esta feature no cambia roles.

## `POST /api/orders/{id}/convert` (existente, comportamiento modificado)

**Autorización**: `[Authorize(Roles = Roles.BranchManager)]` (sin cambios).

**Cambio de comportamiento**:
- Antes: 400 si `order.Status != Confirmed` (incluye el caso de una orden ya `Converted`).
- Ahora: 400 solo si `order.Status` NO es `Confirmed` **ni** `Converted` (es decir, sigue rechazando `Pending` y `Cancelled`; ahora acepta repetir sobre `Converted`).

**Respuesta 200** (`ConvertOrderResultDto`, sin cambios de forma):
```json
{ "shipmentId": 42, "trackingNumber": "TRK-20260818-0007" }
```
Cada llamada exitosa subsiguiente sobre la misma orden devuelve un `shipmentId`/`trackingNumber` distintos.

**Respuesta 400** (`InvalidOperationException`, sin cambios de forma):
```json
{ "message": "Only confirmed orders can be converted to a shipment." }
```

**Respuesta 404**: orden inexistente, sin cambios.

## `GET /api/orders/{id}` y `GET /api/orders/number/{orderNumber}` (existentes, DTO extendido)

**Cambio de forma en `OrderDto`** — dos campos nuevos al final del objeto (ver `data-model.md`):

```json
{
  "id": 12,
  "orderNumber": "ORD-20260818-0003",
  "customerId": 5,
  "originBranchId": null,
  "status": "Converted",
  "...": "...campos existentes sin cambios...",
  "shipmentsCount": 2,
  "isFulfilled": false
}
```

`status` sigue siendo el mismo valor persistido de siempre (`"Pending"`/`"Confirmed"`/`"Converted"`/`"Cancelled"`) — `shipmentsCount`/`isFulfilled` son los campos que ahora comunican el cumplimiento real, no `status`.

## `GET /api/orders` (existente, sin cambio de comportamiento)

Sigue devolviendo `OrderDto`, pero por diseño (ver Edge Case en `data-model.md`) `shipmentsCount` e `isFulfilled` quedan en `0`/`false` en cada fila del listado — no reflejan el estado real. Un consumidor que necesite el dato preciso debe consultar `GET /api/orders/{id}`.

## `GET /api/orders/{id}/shipments` (NUEVO)

**Autorización**: `[Authorize(Roles = Roles.BranchManager + "," + Roles.Operator)]` (mismo nivel que el resto de `OrderController`; no requiere el rol exclusivo de conversión).

**Query params**: `page` (default 1, `[Range(1, int.MaxValue)]`), `pageSize` (default 5, clamp a `MaxPageSize = 50`) — misma convención que `GET /api/orders`.

**Respuesta 200** (`PagedResult<ShipmentDto>`, headers `X-Total-Count`/`X-Page`/`X-Page-Size`/`X-Total-Pages`):
```json
[
  {
    "id": 43,
    "trackingNumber": "TRK-20260818-0008",
    "recipient": "Jane Doe",
    "status": 1,
    "createdAt": "2026-08-18T14:02:00Z",
    "deliveredAt": null
  },
  {
    "id": 42,
    "trackingNumber": "TRK-20260818-0007",
    "recipient": "Jane Doe",
    "status": 3,
    "createdAt": "2026-08-18T13:40:00Z",
    "deliveredAt": "2026-08-18T15:10:00Z"
  }
]
```
(`status` se serializa como número — `ShipmentDto.Status` no lleva `JsonStringEnumConverter`, igual que en el resto de endpoints de `Shipment`; ver el gotcha de serialización de enums documentado en CLAUDE.md.)

**Respuesta 404**: `{ "message": "No order was found with id '{id}'." }` si la orden no existe.

**Respuesta 200 con lista vacía**: orden existente sin envíos generados (`items: []`, `totalCount: 0`).
