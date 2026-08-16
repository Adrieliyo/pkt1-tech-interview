# Research: Paginación del Listado de Envíos

Todos los `NEEDS CLARIFICATION` del Technical Context ya se resolvieron en `spec.md` (Clarifications,
incluida la sesión de `/speckit-clarify` sobre el orden determinista). Este documento registra las
decisiones técnicas derivadas de esas respuestas y de la investigación directa del código actual.

## Decisión 1: Cómo extender el repositorio genérico para soportar paginación

- **Decision**: Agregar parámetros opcionales `int? skip` y `int? take` al método existente
  `IBaseRepository<T>.GetAsync(...)`, y un nuevo método `Task<int> CountAsync(filter)`.
- **Rationale**: `GetAsync` ya acepta `filter` y `orderBy`; skip/take son la extensión natural para
  paginar sin duplicar la lógica de filtrado/orden/includes ya existente. Como son parámetros
  opcionales con default `null`, el único llamador actual (`ShipmentService`, confirmado por
  búsqueda de código — `GetAsync`/`GetAllAsync` no se usan en ningún otro lugar de la solución) no
  se rompe por el cambio de firma.
- **Alternatives considered**:
  - Método nuevo `GetPagedAsync(...)` separado — rechazado: duplicaría la lógica de `filter`/
    `orderBy`/`includeProperties` que `GetAsync` ya tiene, violando el Principio IV (cambio más
    grande de lo necesario) sin beneficio real.
  - Exponer `IQueryable<T>` desde el repositorio para que `Services` arme el `Skip/Take` — rechazado
    explícitamente: la constitución exige que ningún proyecto fuera de `Infrastructure` acceda a
    detalles de EF Core / `DbSet` (Restricciones Técnicas y de Arquitectura).

## Decisión 2: Orden determinista y su relación con `Skip`/`Take`

- **Decision**: `ShipmentService.GetShipmentsAsync` siempre pasa
  `orderBy: q => q.OrderByDescending(x => x.CreatedAt)` a `GetAsync`, tanto con paginación como sin
  ella.
- **Rationale**: Confirmado en la sesión de `/speckit-clarify` (FR-011). Además, SQL Server exige
  `ORDER BY` para poder usar `OFFSET`/`FETCH` (lo que EF Core genera a partir de `Skip().Take()`);
  sin un orden explícito, `Skip`/`Take` no tiene una base estable y el resultado entre páginas no
  está garantizado — exactamente el problema que motivó la pregunta de clarificación.
- **Alternatives considered**: Ninguna — la clarificación ya fijó `CreatedAt` descendente como el
  criterio; no hay una alternativa técnica razonable que evite necesitar algún `ORDER BY`.

## Decisión 3: Cómo transportar ítems + metadata de paginación entre capas

- **Decision**: Nuevo tipo `PagedResult<T>` en `ShipmentTracker.Core/DTOs/PagedResult.cs`
  (`Items`, `Page`, `PageSize`, `TotalCount`, `TotalPages` calculado). `IShipmentService.GetShipmentsAsync`
  pasa de devolver `Task<IEnumerable<ShipmentDto>>` a `Task<PagedResult<ShipmentDto>>`.
- **Rationale**: Es un POCO simple, sin dependencias externas (coherente con que `Core` no tenga
  paquetes NuGet), análogo a los DTOs que ya viven ahí. Cambiar el tipo de retorno de una interfaz
  interna (`IShipmentService`) no afecta el contrato HTTP — el controlador sigue devolviendo
  `ShipmentDto[]` en el cuerpo (FR-007); `PagedResult<T>` nunca se serializa directamente.
- **Alternatives considered**:
  - Tupla con nombres (`(IEnumerable<ShipmentDto> Items, int TotalCount)`) — rechazada: menos
    autodescriptiva en la firma pública de una interfaz, y no calcula `TotalPages` por sí sola.
  - Parámetro `out`/`ref` — rechazada: no es idiomático con métodos `async`.

## Decisión 4: Dónde y cómo exponer la metadata de paginación en HTTP

- **Decision**: `ShipmentController.GetShipments` setea 4 encabezados de respuesta:
  `X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`, vía `Response.Headers`. El cuerpo sigue
  siendo `result.Items` (el arreglo de `ShipmentDto`).
- **Rationale**: Confirmado en Clarifications (FR-009) — el cuerpo no se envuelve. Los encabezados
  son la forma estándar de exponer metadata sin alterar el contrato del cuerpo.
- **Alternatives considered**: Objeto envolvente en el cuerpo — descartado explícitamente en la
  sesión de `/speckit-specify`.
- **Hallazgo importante**: el CORS configurado hoy en `Program.cs` (`AllowReactApp`) no incluye
  `.WithExposedHeaders(...)`. Por especificación del CORS del navegador, un cliente cross-origin
  (el frontend en `localhost:3000`/`5173`) **no puede leer encabezados de respuesta personalizados**
  a menos que el servidor los exponga explícitamente — aunque el encabezado exista en la respuesta
  HTTP cruda. Sin este ajuste, FR-009 quedaría técnicamente implementado pero inútil para el
  frontend. Se agrega `.WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages")`
  a la política `AllowReactApp` como parte de este cambio.

## Decisión 5: Validación de `page`/`pageSize` inválidos (FR-006) sin código nuevo de validación

- **Decision**: Decorar los parámetros de acción con `[FromQuery, Range(1, int.MaxValue)]` para
  `page` y `pageSize`. `[ApiController]` ya dispara automáticamente una respuesta `400` con
  `ValidationProblemDetails` cuando el `ModelState` es inválido — incluyendo valores no numéricos
  (falla de binding) y valores fuera de rango (falla de `[Range]`) — sin necesidad de un `try/catch`
  ni de un validador nuevo.
- **Rationale**: Es la validación más simple posible para "negativo, cero o no numérico" (FR-006),
  usando una capacidad ya incluida en ASP.NET Core (`System.ComponentModel.DataAnnotations`, BCL) —
  cero dependencias nuevas (Principio III) y cero código de validación a mantener.
- **Nota de consistencia**: la respuesta de error resultante (`ValidationProblemDetails`, formato
  RFC 7807) tiene una forma distinta al `{ "message": "..." }` ad-hoc que ya usan otros errores 400
  de este controlador (p. ej. transición de estado inválida). FR-006 solo exige "un error claro",
  no una forma específica; se acepta esta inconsistencia menor como tradeoff documentado en vez de
  escribir código de validación manual solo para igualar el formato.
- **Alternatives considered**:
  - Validar a mano en el controlador o en `ShipmentService` y lanzar una excepción capturada por un
    `try/catch` (como ya existe para `UpdateStatus`) — rechazada: duplica una validación que
    `[Range]` ya cubre completamente, sin beneficio salvo uniformar el formato del error.

## Decisión 6: Tope máximo de `pageSize` (FR-010) — clamping, no rechazo

- **Decision**: El límite superior (50, definido en `spec.md` → Assumptions) se aplica como
  `const int MaxPageSize = 50;` dentro de `ShipmentService`, haciendo
  `var effectivePageSize = Math.Min(pageSize, MaxPageSize);` antes de paginar. **No** se usa el
  segundo argumento de `[Range]` para este límite, porque `[Range]` rechaza (400) en vez de
  recortar, contradiciendo FR-010 ("limitarlo al máximo permitido en vez de fallar").
- **Rationale**: Mantiene el límite de negocio ("cuánto es demasiado") en `Services`, junto a las
  demás reglas de negocio del proyecto (p. ej. `ShipmentTransitionValidator`), separado de la
  validación puramente estructural ("¿es un número positivo?") que vive en el `Web` vía `[Range]`.
- **Alternatives considered**: Clamping en el controlador — rechazada: es una regla de negocio (el
  valor 50 específicamente), no un detalle de HTTP; vive mejor en `Services` (Principio II).

## Decisión 7: `GetAllAsync()` se deja intacto

- **Decision**: `IBaseRepository<T>.GetAllAsync()` / `BaseRepository<T>.GetAllAsync()` no se tocan;
  `ShipmentService.GetShipmentsAsync` deja de llamarlos (ambas ramas se consolidan en una sola
  llamada a `GetAsync` con `filter` opcionalmente `null`), pero el método en sí no se elimina.
- **Rationale**: Eliminarlo sería una limpieza de código no relacionada con la paginación en sí
  (Principio IV: cambios acotados a la inconsistencia/feature puntual). Ninguna otra parte de la
  solución lo usa hoy, pero removerlo no es necesario para cumplir ningún FR de esta feature.
- **Alternatives considered**: Eliminarlo ya que queda sin uso — descartado por alcance; puede
  proponerse como limpieza aparte si se desea en el futuro.

## Decisión 8: `skip` a prueba de overflow para valores de `page` extremos

- **Decision**: Calcular `skip` como `long` (`(long)(page - 1) * effectivePageSize`). Si ese valor
  excede `int.MaxValue`, tratar la solicitud como página fuera de rango (lista vacía, `TotalCount`
  real) en vez de pasarlo a `Skip()` de EF Core.
- **Rationale**: `[Range(1, int.MaxValue)]` (Decisión 5) solo acota `page` por abajo. Un `page`
  suficientemente grande desborda la multiplicación en `int`, produciendo un valor negativo que EF
  Core rechaza con `ArgumentOutOfRangeException` (500), contradiciendo FR-005. Detectarlo con `long`
  antes de tocar el repositorio evita el desbordamiento sin necesitar un tope artificial a `page`.
- **Alternatives considered**: Poner un `[Range(1, N)]` superior a `page` — rechazada: requeriría
  inventar un tope arbitrario no pedido por el usuario; la comprobación con `long` resuelve la causa
  raíz sin restringir el rango legítimo de `page`.

## Estado

Todos los `NEEDS CLARIFICATION` resueltos. Sin bloqueos para Phase 1.
