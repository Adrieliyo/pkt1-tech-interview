---

description: "Task list for Multiple Shipments per Order"
---

# Tasks: Multiple Shipments per Order

**Input**: Design documents from `/specs/009-order-multi-shipments/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/orders-shipments.md, quickstart.md

**Tests**: No se generan tareas de test automatizado — la solución no tiene proyecto de pruebas (ver `plan.md` § Technical Context); la validación es manual vía `quickstart.md`.

**Organization**: Tareas agrupadas por historia de usuario (spec.md). Las 3 historias tocan archivos disjuntos (guard de conversión / nuevo endpoint de listado / campos agregados en `OrderDto`) y son independientemente implementables y probables.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencia de una tarea incompleta)
- **[Story]**: US1, US2 o US3 (spec.md)

## Path Conventions

Proyecto único en capas (`ShipmentTracker.Core`, `ShipmentTracker.Infrastructure`, `ShipmentTracker.Services`, `ShipmentTracker.Web`) — sin `Infrastructure` en esta feature (ver plan.md § Structure Decision, `IShipmentRepository`/`IOrderRepository` no requieren métodos nuevos).

---

## Phase 1: Setup

**Purpose**: Confirmar una línea base limpia antes de empezar. No hay proyecto nuevo que inicializar ni dependencias que añadir (Constitution III — Minimalismo de Dependencias).

- [X] T001 Ejecutar `dotnet build ShipmentTracker.sln` y confirmar que compila sin errores antes de iniciar cualquier cambio.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Prerrequisitos bloqueantes compartidos por las 3 historias de usuario.

**No aplica ninguna tarea foundational**: las 3 historias tocan superficies de código disjuntas (US1: guard en `ConvertToShipmentAsync`; US2: método + endpoint nuevos de solo lectura; US3: campos agregados en `OrderDto` + helper privado nuevo) y ninguna depende de que otra se complete primero. `IShipmentRepository`/`IOrderRepository` ya exponen todo lo necesario (`GetAsync`/`CountAsync` genéricos heredados de `IBaseRepository<T>`, confirmado en `research.md` Decision 4) — no hay migración, entidad ni interfaz de repositorio nueva que crear.

**Checkpoint**: Tras Phase 1, las 3 historias pueden implementarse en cualquier orden o en paralelo.

---

## Phase 3: User Story 1 - Generar un envío adicional sobre una orden ya convertida (Priority: P1) 🎯 MVP

**Goal**: Que `POST /api/orders/{id}/convert` pueda invocarse más de una vez sobre la misma orden mientras esta no esté `Cancelled`/`Pending`, generando un `Shipment` nuevo e independiente cada vez.

**Independent Test**: Confirmar una orden, convertirla dos veces seguidas — la segunda llamada debe devolver 200 con un `shipmentId`/`trackingNumber` distintos al de la primera, en vez del 400 actual (Escenario 1 y 3 de `quickstart.md`).

### Implementation for User Story 1

- [X] T002 [US1] Relajar el guard en `ConvertToShipmentAsync` en `ShipmentTracker.Services/OrderService.cs`: cambiar `if (order.Status != OrderStatus.Confirmed)` por `if (order.Status != OrderStatus.Confirmed && order.Status != OrderStatus.Converted)`, conservando el mismo `InvalidOperationException("Only confirmed orders can be converted to a shipment.")` para el resto de estados (`Pending`, `Cancelled`).
- [X] T003 [P] [US1] Actualizar el comentario XML de `ConvertToShipmentAsync` en `ShipmentTracker.Core/Interfaces/Services/IOrderService.cs` para reflejar que puede invocarse repetidamente sobre una orden ya `Converted` (deja de ser una operación de una sola vez).
- [X] T004 [P] [US1] Actualizar el comentario XML del método `ConvertOrder` en `ShipmentTracker.Web/Controllers/OrderController.cs` con el mismo matiz (conversión repetible, cada llamada crea un `Shipment` independiente).

**Checkpoint**: User Story 1 completamente funcional — validar con Escenarios 1 y 3 de `quickstart.md`.

---

## Phase 4: User Story 2 - Consultar todos los envíos de una orden (Priority: P2)

**Goal**: Exponer `GET /api/orders/{id}/shipments`, paginado, reutilizando `ShipmentDto`/`PagedResult<T>`.

**Independent Test**: Sobre una orden con 2+ envíos (o 0), listar sus envíos y verificar contenido, paginación y headers (Escenario 2 de `quickstart.md`); y 404 sobre una orden inexistente.

### Implementation for User Story 2

- [X] T005 [US2] Añadir la firma `Task<PagedResult<ShipmentDto>?> GetShipmentsByOrderAsync(int orderId, int page = 1, int pageSize = 5);` (con comentario XML) a `ShipmentTracker.Core/Interfaces/Services/IOrderService.cs`.
- [X] T006 [P] [US2] Implementar `GetShipmentsByOrderAsync` en `ShipmentTracker.Services/OrderService.cs`: verificar que la orden exista (`OrderRepository.GetByIdAsync`, devolver `null` si no), calcular `effectivePageSize = Math.Min(pageSize, MaxPageSize)`, consultar `IUnitOfWork.ShipmentRepository.GetAsync(filter: s => s.OrderId == orderId, orderBy: q => q.OrderByDescending(s => s.CreatedAt), skip: (page-1)*effectivePageSize, take: effectivePageSize)` + `CountAsync` con el mismo filtro, mapear cada `Shipment` a `ShipmentDto` vía `_mapper` y devolver un `PagedResult<ShipmentDto>` (mismo patrón que `GetOrdersAsync`). Depende de T005.
- [X] T007 [P] [US2] Añadir el endpoint `[HttpGet("{id}/shipments")]` a `ShipmentTracker.Web/Controllers/OrderController.cs`: parámetros `[FromQuery, Range(1, int.MaxValue)] int page = 1` y `pageSize = 5`, invocar `_orderService.GetShipmentsByOrderAsync`, devolver `NotFound` si el resultado es `null`, o `Ok(result.Items)` con los headers `X-Total-Count`/`X-Page`/`X-Page-Size`/`X-Total-Pages` (mismo patrón que `GetOrders`), con comentario XML documentando el nuevo endpoint. Depende de T005.

**Checkpoint**: User Story 2 completamente funcional de forma independiente — validar con Escenario 2 de `quickstart.md`.

---

## Phase 5: User Story 3 - Conocer el estado de cumplimiento agregado de la orden (Priority: P3)

**Goal**: Exponer en `OrderDto` un conteo de envíos (`ShipmentsCount`) y un indicador de cumplimiento (`IsFulfilled`), calculados bajo demanda sin persistir ninguna columna nueva.

**Independent Test**: Con una orden de 2 envíos, marcar uno `Delivered` y dejar el otro pendiente → `isFulfilled: false`; marcar ambos `Delivered` → `isFulfilled: true`; con un envío cancelado y otro `Delivered` → `isFulfilled: true` (Escenarios 4, 5 y 6 de `quickstart.md`).

### Implementation for User Story 3

- [X] T008 [P] [US3] Añadir las propiedades `public int ShipmentsCount { get; set; }` y `public bool IsFulfilled { get; set; }` (con comentarios XML describiendo que son calculadas, no persistidas, y el criterio de `IsFulfilled` — ver `data-model.md`) a `ShipmentTracker.Core/DTOs/Orders/OrderDto.cs`.
- [X] T009 [US3] Añadir un método privado `ComputeFulfillmentAsync(int orderId)` a `ShipmentTracker.Services/OrderService.cs` que consulte `IUnitOfWork.ShipmentRepository.GetAsync(filter: s => s.OrderId == orderId)`, calcule `shipmentsCount` (total, incluye cancelados) y `isFulfilled` (`true` solo si existe al menos un envío con `Status != ShipmentStatus.Cancelled` y todos los envíos con `Status != ShipmentStatus.Cancelled` tienen `Status == ShipmentStatus.Delivered`), y devuelva ambos valores (p. ej. tupla `(int ShipmentsCount, bool IsFulfilled)`). Depende de T008.
- [X] T010 [US3] Invocar `ComputeFulfillmentAsync` y asignar `ShipmentsCount`/`IsFulfilled` sobre el `OrderDto` resultante en `GetOrderByIdAsync`, `GetOrderByNumberAsync`, `ConfirmOrderAsync` y `UpdateOrderAsync` en `ShipmentTracker.Services/OrderService.cs`; en `CreateOrderAsync`, asignar `0`/`false` directamente sin consultar (una orden recién creada en `Pending` nunca tiene envíos). Depende de T009.

**Checkpoint**: User Story 3 completamente funcional de forma independiente — validar con Escenarios 4, 5 y 6 de `quickstart.md`.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Ajustes finales que abarcan más de una historia.

- [X] T011 [P] Revisar y, si es necesario, aclarar los comentarios XML de `Converted` en `ShipmentTracker.Core/Enums/OrderStatus.cs` y `ShipmentTracker.Core/Entities/Order.cs` para reflejar que ya no bloquea conversiones adicionales (aunque sigue siendo el único valor desde el que se puede volver a convertir, y `Cancelled` sigue siendo el único estado verdaderamente terminal para ediciones).
- [ ] T012 Ejecutar manualmente los 6 escenarios de `specs/009-order-multi-shipments/quickstart.md` contra la API en ejecución (`dotnet run --project ShipmentTracker.Web`, Swagger UI) y confirmar que cada uno produce el resultado esperado.
- [X] T013 Ejecutar `dotnet build ShipmentTracker.sln` una vez más para confirmar que la solución completa compila sin errores ni advertencias nuevas tras todos los cambios.
- [X] T014 (encontrada durante validación manual, fuera del plan original) Corregir el índice único preexistente `IX_Shipments_OrderId` (módulo `006-orders`, asumía 1:1) que causaba un 500 al generar un segundo Shipment para la misma orden: `ShipmentTracker.Infrastructure/Data/Configurations/ShipmentConfiguration.cs` (quitar `.IsUnique()`) + migración `MakeShipmentOrderIdIndexNonUnique` (aplicada con `dotnet ef database update`). Ver `data-model.md` § Shipment Entity.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sin dependencias — T001 primero.
- **Foundational (Phase 2)**: Vacía — no bloquea nada.
- **User Stories (Phase 3-5)**: Todas pueden empezar inmediatamente después de T001. Son mutuamente independientes (tocan archivos y métodos disjuntos) y pueden implementarse en cualquier orden o en paralelo.
- **Polish (Phase 6)**: Depende de que las historias que se vayan a entregar estén completas (T012/T013 ejercitan el resultado final).

### Dentro de cada historia

- **US1**: T002 antes que T003/T004 (los comentarios documentan el comportamiento ya implementado, aunque no dependen técnicamente del código para compilar).
- **US2**: T005 (firma en la interfaz) antes que T006 y T007; T006 y T007 son paralelos entre sí (archivos distintos, ambos solo dependen de la firma de T005).
- **US3**: T008 → T009 → T010, estrictamente secuencial (mismo archivo en T009/T010, y T009 necesita las propiedades de T008 para poder asignarlas más adelante en T010).

### Parallel Opportunities

- T003 y T004 (US1) en paralelo entre sí.
- T006 y T007 (US2) en paralelo entre sí, una vez completado T005.
- T002 (US1), T005 (US2) y T008 (US3) pueden iniciarse en paralelo entre sí (tres historias independientes) inmediatamente después de T001.
- T011 (Polish) es paralelizable respecto a cualquier tarea de implementación restante, ya que solo toca comentarios XML en archivos que ninguna otra tarea de esta lista edita.

---

## Parallel Example: Arranque de las 3 historias tras Setup

```bash
# Tras T001, tres desarrolladores podrían tomar cada historia en paralelo:
Task: "T002 [US1] Relajar el guard en ConvertToShipmentAsync en ShipmentTracker.Services/OrderService.cs"
Task: "T005 [US2] Añadir GetShipmentsByOrderAsync a ShipmentTracker.Core/Interfaces/Services/IOrderService.cs"
Task: "T008 [US3] Añadir ShipmentsCount/IsFulfilled a ShipmentTracker.Core/DTOs/Orders/OrderDto.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 únicamente)

1. Completar Phase 1 (T001).
2. Completar Phase 3 — User Story 1 (T002-T004).
3. **Detener y validar**: ejecutar Escenarios 1 y 3 de `quickstart.md`.
4. Esto ya entrega el valor central: una orden puede generar más de un envío.

### Entrega incremental

1. Setup (T001) → línea base lista.
2. + User Story 1 (T002-T004) → validar → esto ya es el MVP.
3. + User Story 2 (T005-T007) → validar → operadores pueden listar los envíos de una orden.
4. + User Story 3 (T008-T010) → validar → visibilidad del cumplimiento agregado sin abrir cada envío.
5. Polish (T011-T013) → confirmación final de build + quickstart completo.

### Nota sobre `OrderService.cs`

`OrderService.cs` es tocado por las 3 historias (T002 para US1; T006 para US2; T009/T010 para US3) en métodos distintos del mismo archivo. Si se trabaja en paralelo con más de una persona sobre este archivo, coordinar para evitar conflictos de merge triviales (no hay solapamiento de líneas, pero sí del mismo archivo).
