# Phase 0 Research: Multiple Shipments per Order

No quedan `NEEDS CLARIFICATION` en el Technical Context del plan (todas las decisiones de negocio ya se resolvieron en `/speckit-clarify`, ver `spec.md` § Clarifications). Este documento consolida las decisiones técnicas derivadas de esas clarificaciones y del enfoque técnico aportado por el usuario.

## Decision 1: Relajar el guard de `ConvertToShipmentAsync` en vez de reescribirlo

**Decision**: El guard actual (`if (order.Status != OrderStatus.Confirmed) throw new InvalidOperationException(...)`) se relaja a `if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.Converted)`. Tras la primera conversión, `order.Status` sigue asignándose a `Converted` (ya no cambia de `Confirmed` a un valor distinto en la segunda llamada en adelante — simplemente permanece en `Converted`).

**Rationale**: `Converted` deja de comportarse como estado terminal para el propósito de "bloquear conversiones futuras" (resuelto en Clarifications), pero conserva su nombre y su valor persistido — ninguna migración de datos, ningún consumidor externo que dependa del valor string `"Converted"` se rompe. `Cancelled` y `Pending` siguen rechazando la conversión exactamente igual que hoy.

**Alternatives considered**:
- Añadir un nuevo valor de enum (`PartiallyConverted`) — rechazado explícitamente en Clarifications (Question 2/B) para evitar tocar el contrato persistido de `OrderStatus`.
- Permitir conversión desde cualquier estado no-`Cancelled` (incluyendo `Pending`) — rechazado: el spec (FR-002, edge case) exige que `Pending` sin confirmar siga rechazándose; solo `Confirmed`/`Converted` habilitan la conversión.

## Decision 2: Indicador de cumplimiento calculado on-demand, sin columna nueva

**Decision**: `OrderService` expone un método privado (p. ej. `ComputeFulfillmentAsync(int orderId)`) que consulta `IUnitOfWork.ShipmentRepository.GetAsync(filter: s => s.OrderId == orderId)`, cuenta el total y evalúa si todos los envíos con `Status != ShipmentStatus.Cancelled` están en `Status == ShipmentStatus.Delivered` (y hay al menos uno no cancelado). El resultado (`ShipmentsCount: int`, `IsFulfilled: bool`) se adjunta manualmente al `OrderDto` después del `_mapper.Map<OrderDto>(order)` — igual que cualquier campo agregado no mapeado por AutoMapper (patrón ya usado: AutoMapper es output-only para los campos 1:1 de la entidad; los campos calculados se asignan a mano en el `Service`).

**Rationale**: Ya decidido en Clarifications (Question 3/B). Reutiliza el patrón "estado agregado derivado en el Service consultando registros hijos, no persistido" documentado en CLAUDE.md para casos análogos (aunque no hay un precedente idéntico de "agregado sobre hijos" en este repo, el principio de "no persistir lo que se puede derivar" es coherente con Minimalismo de Dependencias/Cambios Pequeños de la constitución — evita migración y evita introducir un mecanismo de sincronización nuevo).

**Alternatives considered**:
- Persistir `ShipmentsCount`/`IsFulfilled` como columnas en `Order`, actualizadas en cada evento de envío — rechazado: requiere tocar `ShipmentEventService` (fuera del módulo `Order`) para mantener la columna sincronizada cada vez que un `Shipment` cambia de estado, mucho mayor superficie de cambio para un valor trivialmente derivable en lectura.
- Exponerlo como un endpoint separado (`GET /api/orders/{id}/fulfillment`) en vez de campos en `OrderDto` — rechazado: el spec (FR-005) pide que la información esté disponible "en la información de una orden", y añadir un endpoint adicional por este único propósito es una superficie mayor sin beneficio (`GetOrderById` ya se consulta junto con este dato en el mismo flujo de UI típico).

## Decision 3: Nuevo endpoint de listado reutiliza `ShipmentDto`/`PagedResult<T>` existentes

**Decision**: `GET /api/orders/{id}/shipments` en `OrderController`, delegando a un nuevo método `IOrderService.GetShipmentsByOrderAsync(int orderId, int page, int pageSize)` que:
1. Verifica que la orden exista (404 si no).
2. Llama a `IUnitOfWork.ShipmentRepository.GetAsync(filter: s => s.OrderId == orderId, orderBy: q => q.OrderByDescending(s => s.CreatedAt), skip, take)` + `CountAsync` con el mismo filtro.
3. Mapea cada `Shipment` a `ShipmentDto` (mapeo ya existente, sin cambios) y arma un `PagedResult<ShipmentDto>`.

**Rationale**: Sigue exactamente la convención de paginación ya establecida (`page`/`pageSize` con `[Range(1, int.MaxValue)]`, `MaxPageSize = 50` clamp, headers `X-Total-Count`/`X-Page`/`X-Page-Size`/`X-Total-Pages`, `OrderByDescending(CreatedAt)`) usada por `Shipment`, `Branch`, `Employee`, `Vehicle`, `Order`. No se crea un DTO nuevo — `ShipmentDto` ya expone exactamente lo que un listado necesita (TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt).

**Alternatives considered**:
- Añadir un parámetro `orderId` opcional al endpoint `GET /api/shipment` existente (módulo `001`/`002`, ruta singular `/api/shipment/...`) en vez de un endpoint nuevo bajo `/api/orders/{id}/shipments` — rechazado: mezclaría la responsabilidad de un controlador ya estable de otro módulo; el patrón ya aceptado en el repo (`ConvertToShipmentAsync` accediendo a `ShipmentRepository` vía `IUnitOfWork` desde `OrderService`) es que la *consulta* relacionada con `Order` vive en `OrderController`/`OrderService`, no que se modifique `ShipmentController`.
- Devolver la lista de envíos embebida dentro de `OrderDto` en vez de un endpoint separado — rechazado: rompe la paginación consistente ya usada en toda la solución (una lista embebida sin paginación propia no expone los headers estándar `X-Total-Count`, etc.) y haría `GetOrderById` potencialmente costoso para órdenes con muchos envíos.

## Decision 4: `IShipmentRepository`/`IOrderRepository` no necesitan métodos nuevos

**Decision**: No se añade ningún método a `IShipmentRepository` ni a `IOrderRepository`. El filtro genérico `GetAsync(filter, orderBy, includeProperties, skip, take)` de `IBaseRepository<T>` ya soporta `filter: s => s.OrderId == orderId` sin necesidad de una consulta especializada.

**Rationale**: Confirmado leyendo `IBaseRepository<TEntity>` — ya acepta una `Expression<Func<TEntity, bool>> filter` arbitraria. Añadir un método `GetByOrderIdAsync` sería una superficie de repositorio especulativa que la convención del repo explícitamente evita ("si un repositorio no necesita ninguna consulta más allá de la base genérica, se deja vacío").

**Alternatives considered**: Ninguna — la convención ya documentada en CLAUDE.md resuelve esto sin ambigüedad.

## Decision 5: Condición de carrera en `GenerateTrackingNumberAsync` — sin cambios

**Decision**: `OrderService.GenerateTrackingNumberAsync` (conteo de shipments del día + 1) se reutiliza sin modificación para cada conversión, incluidas las conversiones repetidas sobre la misma orden. No se introduce locking ni una secuencia dedicada.

**Rationale**: Ya es una condición de carrera aceptada y documentada (`specs/006-orders/research.md` Decision 8) para el caso de una conversión por orden; permitir múltiples conversiones por orden no cambia la naturaleza del riesgo (dos requests concurrentes cualesquiera, sean de la misma orden o de órdenes distintas, ya podían colisionar en teoría bajo el mismo mecanismo). No se justifica una excepción a "Cambios Pequeños y Reversibles" para introducir locking solo por este feature.

**Alternatives considered**: Introducir una tabla de secuencia o un `SELECT ... WITH (UPDLOCK)` — rechazado, fuera de alcance y no solicitado por el spec ni por ningún requisito de esta feature.
