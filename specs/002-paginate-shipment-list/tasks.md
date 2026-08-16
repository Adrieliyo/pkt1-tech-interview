---
description: "Task list template for feature implementation"
---

# Tasks: Paginación del Listado de Envíos

**Input**: Design documents from `/specs/002-paginate-shipment-list/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/shipment-list-contract.md, quickstart.md

**Tests**: Sin tareas de pruebas automatizadas — el proyecto no tiene proyecto de pruebas (ver
constitución); verificación manual vía `quickstart.md`, consistente con `001-standardize-mapping-di`.

**Organization**: Agrupadas por historia de usuario. No hay fase de Setup/Foundational separada:
el mecanismo de paginación (repositorio, `PagedResult<T>`, servicio, controlador, headers) *es* la
implementación de la Historia 1 (P1) — la Historia 2 (P2) es aditiva sobre los mismos archivos, no
un bloque de infraestructura compartida previa.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivo distinto, sin dependencia de tareas incompletas)
- **[Story]**: US1 o US2
- Cada tarea incluye la ruta exacta del archivo

## Path Conventions

Solución .NET en capas existente (`ShipmentTracker.Core` / `.Infrastructure` / `.Services` /
`.Web`, ver `plan.md`). Rutas relativas a la raíz del repositorio.

---

## Phase 1: Setup

**N/A.** Sin inicialización de proyecto ni paquetes nuevos (research.md: cero dependencias NuGet
nuevas).

---

## Phase 2: Foundational

**N/A.** Ver nota en "Organization" arriba — el mecanismo base se implementa directamente como
parte de la Historia 1.

---

## Phase 3: User Story 1 - Ver envíos en grupos manejables por defecto (Priority: P1) 🎯 MVP

**Goal**: `GET /api/shipment` sin parámetros devuelve como máximo 5 envíos, ordenados por
`CreatedAt` descendente, con la metadata de paginación disponible vía encabezados HTTP expuestos a
CORS.

**Independent Test**: Llamar `GET /api/shipment` sin parámetros de paginación (con más de 5 envíos
existentes) y confirmar exactamente 5 elementos en el cuerpo, ordenados por `createdAt`
descendente, y los 4 encabezados de paginación presentes con valores coherentes (escenarios 1-2 de
`quickstart.md`).

### Implementation for User Story 1

- [X] T001 [P] [US1] Crear `ShipmentTracker.Core/DTOs/PagedResult.cs`: clase genérica `PagedResult<T>` con `Items` (`IEnumerable<T>`), `Page`, `PageSize`, `TotalCount` (todos `int`), y `TotalPages` calculada (`Ceiling(TotalCount / (double)PageSize)`, `0` si `TotalCount` es `0`) — ver tabla de campos en `data-model.md`
- [X] T002 [P] [US1] En `ShipmentTracker.Core/Interfaces/Repositories/IBaseRepository.cs`: agregar parámetros opcionales `int? skip = null, int? take = null` a `GetAsync(...)`, y agregar `Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null);`
- [X] T003 [US1] En `ShipmentTracker.Infrastructure/Repositories/BaseRepository.cs`: implementar `skip`/`take` en `GetAsync` (aplicar `orderBy` primero si existe, luego `.Skip(skip.Value)`/`.Take(take.Value)` si se proporcionan) e implementar `CountAsync` (mismo `filter` que `GetAsync`, `await query.CountAsync()`) (depende de T002)
- [X] T004 [US1] En `ShipmentTracker.Core/Interfaces/Services/IShipmentService.cs`: cambiar la firma de `GetShipmentsAsync` a `Task<PagedResult<ShipmentDto>> GetShipmentsAsync(ShipmentStatus? status = null, int page = 1, int pageSize = 5)` (depende de T001)
- [X] T005 [US1] En `ShipmentTracker.Services/ShipmentService.cs`: reescribir `GetShipmentsAsync` calculando `skip` como `long` (`(long)(page - 1) * pageSize`, ver research.md Decisión 8); si excede `int.MaxValue`, usar una lista vacía en vez de llamar a `GetAsync`; de lo contrario llamar a `_unitOfWork.ShipmentRepository.GetAsync(filter, orderBy: q => q.OrderByDescending(x => x.CreatedAt), skip: (int)skip, take: pageSize)`. Llamar siempre a `CountAsync(filter)` con el mismo `filter` (nulo si no hay `status`) para `TotalCount`, mapear los ítems con `_mapper` y devolver un `PagedResult<ShipmentDto>` con `Page`, `PageSize` y `TotalCount` (depende de T003, T004)
- [X] T006 [US1] En `ShipmentTracker.Web/Controllers/ShipmentController.cs`: agregar parámetros `[FromQuery] int page = 1` y `[FromQuery] int pageSize = 5` a `GetShipments`, llamar al servicio con ellos, setear `Response.Headers["X-Total-Count"]`, `["X-Page"]`, `["X-Page-Size"]`, `["X-Total-Pages"]` a partir del `PagedResult`, devolver `Ok(result.Items)`, y actualizar los comentarios XML del método para documentar `page`/`pageSize` (depende de T005)
- [X] T007 [P] [US1] En `ShipmentTracker.Web/Program.cs`: agregar `.WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages")` a la política CORS `AllowReactApp`, para que el frontend cross-origin pueda leer estos encabezados (ver research.md, Decisión 4)

**Checkpoint**: `GET /api/shipment` sin parámetros ya pagina por defecto (5, ordenado, con headers
expuestos). Historia 1 verificable de forma independiente.

---

## Phase 4: User Story 2 - Pedir más registros o navegar a otras páginas (Priority: P2)

**Goal**: El cliente puede pedir un `pageSize` mayor a 5 o una página distinta a la primera, con un
tope de 50 registros por página, y recibe `400` ante parámetros inválidos.

**Independent Test**: Llamar `GET /api/shipment?pageSize=10` y `GET /api/shipment?pageSize=5&page=2`
con 12+ envíos y confirmar los conteos/registros esperados; llamar con `page=0`, `pageSize=-1` y
`page=abc` y confirmar `400` en los tres casos; llamar con `pageSize=1000` y confirmar que se
recorta a 50 en vez de fallar (escenarios 3-5 y 8 de `quickstart.md`).

### Implementation for User Story 2

- [X] T008 [P] [US2] En `ShipmentTracker.Web/Controllers/ShipmentController.cs`: agregar `[Range(1, int.MaxValue)]` a los parámetros `page` y `pageSize` de `GetShipments` (más el `using System.ComponentModel.DataAnnotations;` correspondiente), para que `[ApiController]` devuelva `400` automáticamente ante valores no numéricos, negativos o cero (depende de T006, mismo archivo)
- [X] T009 [P] [US2] En `ShipmentTracker.Services/ShipmentService.cs`: agregar `private const int MaxPageSize = 50;` y aplicar `var effectivePageSize = Math.Min(pageSize, MaxPageSize);` en `GetShipmentsAsync` antes de calcular `skip`/`take`, usando `effectivePageSize` también como `PageSize` en el `PagedResult` devuelto (depende de T005, mismo archivo)

**Checkpoint**: El cliente puede aumentar el tamaño de página o navegar páginas, con validación de
entrada y tope máximo aplicados. Historias 1 y 2 funcionan juntas.

---

## Phase 5: Polish & Cross-Cutting Concerns

- [X] T010 [P] Compilar la solución (`dotnet build ShipmentTracker.sln`) y confirmar cero errores y cero advertencias nuevas (depende de T001-T009)
- [X] T011 Ejecutar de punta a punta los 10 escenarios manuales y la verificación de "nada más cambió" de `specs/002-paginate-shipment-list/quickstart.md` (depende de T010)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup / Foundational**: N/A — se empieza directo en la Fase 3.
- **User Story 1 (Fase 3)**: sin dependencias de otras historias.
- **User Story 2 (Fase 4)**: depende de que T005 y T006 (US1) ya existan, porque T008/T009 editan
  los mismos archivos que US1 creó/modificó primero. No depende conceptualmente de que US1 "esté
  perfecto", solo de que esos archivos ya tengan la forma que US2 extiende.
- **Polish (Fase 5)**: depende de que Fases 3 y 4 estén completas.

### Dentro de cada historia

- T001, T002 → T003 (necesita el paquete de tipos/firma antes de implementar)
- T001 → T004 (necesita `PagedResult<T>` para la firma de la interfaz)
- T003, T004 → T005
- T005 → T006
- T006 → T008 (mismo archivo, `ShipmentController.cs`)
- T005 → T009 (mismo archivo, `ShipmentService.cs`)
- T001–T009 → T010 → T011

### Parallel Opportunities

- Al iniciar la Fase 3: **T001 y T002** son independientes entre sí (archivos distintos en `Core`)
  y pueden hacerse en paralelo; **T007** (Program.cs) no depende de ninguna otra tarea de la fase y
  puede hacerse en cualquier momento en paralelo con el resto.
- En la Fase 4: **T008 y T009** son independientes entre sí (archivos distintos) y pueden hacerse en
  paralelo, una vez completadas T005/T006 respectivamente.

---

## Parallel Example: User Story 1

```bash
# Al inicio de la Fase 3, en paralelo:
Task: "Crear PagedResult<T> en ShipmentTracker.Core/DTOs/PagedResult.cs"
Task: "Extender IBaseRepository<T> con skip/take y CountAsync"
Task: "Agregar WithExposedHeaders a la política CORS en Program.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 solamente)

1. Completar Fase 3 (T001-T007).
2. Ejecutar los escenarios 1, 2, 6, 7 y 9 de `quickstart.md` para validar US1 de forma aislada
   (default, pocos registros, página fuera de rango, combinado con `status`, CORS).
3. US1 por sí sola ya resuelve el problema original (evitar listados sin límite) — es un incremento
   entregable por separado de US2.

### Incremental Delivery

1. Fase 3 (US1) → validar → paginación por defecto funcionando, lista para revisar/mergear.
2. Fase 4 (US2) → validar con los escenarios 3, 4, 5 y 8 → cliente puede pedir más/otra página, con
   validación y tope aplicados.
3. Fase 5 (Polish) → build limpio + validación manual completa de los 10 escenarios.

---

## Notes

- No hay tareas de test automatizado — el proyecto no tiene proyecto de pruebas (misma política que
  `001-standardize-mapping-di`); la validación es manual vía `quickstart.md`.
- `GetAllAsync()` en `IBaseRepository<T>`/`BaseRepository<T>` se deja intacto a propósito — no
  forma parte del alcance (ver `research.md`, Decisión 7). No generar una tarea para tocarlo.
- Cada tarea toca 1-2 archivos como máximo, consistente con el Principio IV de la constitución.
