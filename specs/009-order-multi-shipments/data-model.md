# Phase 1 Data Model: Multiple Shipments per Order

Ninguna entidad nueva ni columna nueva. Este documento describe únicamente los cambios de forma en los DTOs y el comportamiento de los campos ya existentes.

## Order Entity (sin cambios estructurales)

Sin cambios en `ShipmentTracker.Core/Entities/Order.cs`. `Status` conserva sus 4 valores (`Pending`, `Confirmed`, `Converted`, `Cancelled`) sin añadir ni renombrar ninguno.

**Comportamiento actualizado** (solo en `OrderService`, no en la entidad):
- `Confirmed → Converted`: primera conversión (sin cambios respecto a hoy).
- `Converted → Converted`: conversiones subsiguientes ya no se rechazan (antes lanzaban `InvalidOperationException`); el valor persistido no cambia.
- `Pending`/`Cancelled → (rechazado)`: sin cambios.

## Shipment Entity (sin cambios en la entidad; corrección de índice)

Sin cambios en `ShipmentTracker.Core/Entities/Shipment.cs`. `OrderId` (`int?`, ya existente) ahora puede repetirse entre varias filas de `Shipment`.

**Corrección post-implementación**: el índice `IX_Shipments_OrderId` (creado en el módulo `006-orders`, filtrado `WHERE OrderId IS NOT NULL`) era **único** — asumía la relación 1:1 original y no se detectó durante la investigación de esta feature. Provocaba un `500` (violación de restricción única en SQL Server) al intentar generar un segundo `Shipment` para la misma orden. Corregido en `ShipmentConfiguration.cs` (`builder.HasIndex(x => x.OrderId)`, sin `.IsUnique()`) más la migración `MakeShipmentOrderIdIndexNonUnique` (aditiva: solo reemplaza el índice, sin `DropColumn`/pérdida de datos). El índice se conserva sin la restricción porque `GetShipmentsByOrderAsync`/`ComputeFulfillmentAsync` siguen filtrando por esta columna.

## OrderDto (Core/DTOs/Orders/OrderDto.cs) — campos nuevos

Dos propiedades nuevas, calculadas en `OrderService` (no mapeadas 1:1 por AutoMapper desde `Order`, ya que no existen como columnas en la entidad):

| Campo | Tipo | Descripción |
|---|---|---|
| `ShipmentsCount` | `int` | Total de `Shipment` cuyo `OrderId` es esta orden (incluye cancelados). `0` si nunca se convirtió. |
| `IsFulfilled` | `bool` | `true` únicamente si `ShipmentsCount > 0` Y todos los envíos con `Status != Cancelled` están en `Status == Delivered` Y existe al menos un envío no cancelado. `false` en cualquier otro caso (incluyendo `ShipmentsCount == 0` y el caso "todos cancelados"). |

Ambos campos se calculan en cada método de `OrderService` que devuelve un `OrderDto` individual: `GetOrderByIdAsync`, `GetOrderByNumberAsync`, `ConfirmOrderAsync`, `UpdateOrderAsync` (y `CreateOrderAsync`, donde el atajo trivial es devolver `0`/`false` sin consultar, ya que una orden recién creada en `Pending` nunca tiene envíos). `ConvertToShipmentAsync` sigue devolviendo `ConvertOrderResultDto` (sin cambios de forma, ver contracts) — no expone estos campos directamente; un cliente que los necesite tras convertir hace un `GET /api/orders/{id}` de seguimiento. **No** se calculan para el listado paginado `GetOrdersAsync` (evita N+1 — ver Edge Case abajo), que sigue devolviendo `OrderDto` con estos dos campos en su valor por defecto (`0`/`false`); esta limitación queda documentada explícitamente en el comentario XML de `ShipmentsCount`/`IsFulfilled` y en `contracts/orders-shipments.md`.

## ShipmentDto (sin cambios)

Reutilizado tal cual en el nuevo endpoint de listado — ya expone `Id`, `TrackingNumber`, `Recipient`, `Status`, `CreatedAt`, `DeliveredAt`.

## PagedResult<ShipmentDto> (sin cambios de forma)

Reutilizado tal cual (genérico ya existente) como envoltorio de respuesta del nuevo endpoint.

## Nueva operación de servicio: `IOrderService.GetShipmentsByOrderAsync`

```
Task<PagedResult<ShipmentDto>?> GetShipmentsByOrderAsync(int orderId, int page, int pageSize);
```

- Devuelve `null` si la orden no existe (el controller traduce a 404).
- `page`/`pageSize` siguen la misma convención de clamp (`MaxPageSize = 50`) ya usada en `GetOrdersAsync`/`GetShipmentsAsync`.
- Orden: `OrderByDescending(s => s.CreatedAt)` (más recientes primero), igual que el resto de listados.

## Edge case: por qué `ShipmentsCount`/`IsFulfilled` no se calculan en el listado paginado de órdenes

Calcular el cumplimiento de N órdenes en una sola página implicaría N consultas adicionales (una por orden) o una consulta de agregación más compleja sobre todos los `Shipment` de la página. Dado que el spec (User Story 3, FR-005) solo pide que el operador pueda "consultar una orden" para ver su cumplimiento — no que el listado completo lo muestre columna por columna — se limita el cálculo a las operaciones de detalle (`GetOrderById`, `GetOrderByNumber`) y a las mutaciones que devuelven la orden actualizada (`Confirm`, `Convert`, `Update`). Si en el futuro se requiere en el listado, se resolvería con una consulta de agregación dedicada — fuera de alcance de este cambio (Cambios Pequeños y Reversibles).
