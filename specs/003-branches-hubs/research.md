# Research: Branches & Hubs Module

Todos los `NEEDS CLARIFICATION` del Technical Context ya se resolvieron: el usuario fijó
explícitamente el stack (.NET 8 Web API en capas, EF Core, FluentValidation, AutoMapper,
Repository + Unit of Work), las entidades (`Branch`, `BranchSchedule`) y las cinco rutas HTTP en
el input de `/speckit-plan`. Este documento registra las decisiones técnicas derivadas de ese
input, de `spec.md` (incluidas sus Clarifications) y de la investigación directa del código actual
(`ShipmentService`, `ShipmentController`, `BaseRepository`, `UnitOfWork`, `Program.cs`).

## Decisión 1: `Type`/`DayOfWeek` como enums *nullable* en los DTOs de entrada

- **Decision**: `CreateBranchDto.Type` y `ScheduleEntryInputDto.DayOfWeek` son `BranchType?` y
  `ScheduleDay?` (no los tipos no-nullable), validados con `.NotNull()` + `.IsInEnum()` en los
  validadores de FluentValidation.
- **Rationale**: `BranchType` y `ScheduleDay` son enums cuyo primer valor (`0`) es
  `Headquarters`/`Monday`. Si la propiedad fuera no-nullable, un cliente que omite el campo en el
  JSON recibiría silenciosamente `default(BranchType) == Headquarters` (o `Monday`) en vez de un
  error de validación — violando FR-001 ("omitir el tipo de sucursal → rechazado") y la regla de
  "no exactamente 7 días" (un `DayOfWeek` omitido en una entrada de horario podría enmascararse
  como un segundo `Monday`, detectable solo indirectamente vía la regla de duplicados). Usar `?`
  hace que "omitido" y "`Headquarters`/`Monday` explícito" sean estados distintos y verificables.
- **Alternatives considered**: Dejarlos no-nullable y confiar en que la regla de "sin
  duplicados"/"exactamente 7" capture indirectamente el caso — rechazada: no cubre el caso de un
  `Type` omitido en `CreateBranchDto` (no hay una segunda señal que lo delate), y depender de un
  efecto colateral para detectar un campo faltante es frágil y no comunica el error real al
  usuario.

## Decisión 2: Cobertura de 7 días sin una regla explícita de "todos los días presentes"

- **Decision**: `CreateBranchDtoValidator`/`UpdateBranchDtoValidator` validan el arreglo
  `Schedule` con tres reglas combinadas: (a) cada entrada es válida individualmente
  (`RuleForEach(...).SetValidator(new ScheduleEntryInputDtoValidator())`, que incluye
  `DayOfWeek.IsInEnum()`), (b) `Schedule.Count == 7`, (c)
  `Schedule.Select(x => x.DayOfWeek).Distinct().Count() == 7`.
- **Rationale**: `ScheduleDay` tiene exactamente 7 valores posibles. Si las 7 entradas tienen un
  `DayOfWeek` válido (regla a) y son 7 en total (regla b) y todas son distintas entre sí (regla c),
  matemáticamente cubren los 7 valores del enum sin faltar ninguno — no hace falta una cuarta regla
  que enumere y compare explícitamente contra `Enum.GetValues<ScheduleDay>()`. Esto implementa
  FR-004 (7 entradas siempre) y FR-005 (sin días duplicados) con el mínimo de código.
- **Alternatives considered**: Regla explícita "todos los valores del enum están presentes" —
  rechazada: es lógicamente redundante dadas (a)+(b)+(c) y añade una cuarta comprobación sin
  aportar cobertura adicional.

## Decisión 3: Consistencia de "cerrado" vs horarios en `ScheduleEntryInputDtoValidator`

- **Decision**: Un único validador por entrada (reutilizado desde Create y Update):
  - `IsClosed == true` → `OpensAt` y `ClosesAt` deben ser `null` (si alguno viene con valor, falla
    con un mensaje explícito de "entrada inconsistente").
  - `IsClosed == false` → `OpensAt` y `ClosesAt` son requeridos (`NotNull()`) y
    `OpensAt < ClosesAt` (estrictamente).
- **Rationale**: Implementa FR-006, FR-007 y FR-017 tal como quedaron fijados en la sesión de
  `/speckit-clarify` ("cerrado + horarios → rechazado, no se ignoran los horarios en silencio").
  Un único validador reutilizable evita duplicar la regla entre `CreateBranchDtoValidator` y
  `UpdateBranchDtoValidator` (ambos lo referencian vía `SetValidator`).
- **Alternatives considered**: Ignorar los horarios cuando `IsClosed == true` (dejar que el flag
  tome precedencia) — descartada explícitamente en Clarifications.

## Decisión 4: `BranchSchedule` no tiene repositorio, controlador ni servicio propio

- **Decision**: `BranchSchedule` se gestiona íntegramente como parte del agregado `Branch`, a
  través de `IBranchRepository`/`BranchService`. No existe `IBranchScheduleRepository` ni ningún
  endpoint HTTP para manipular una sola entrada de horario de forma aislada.
- **Rationale**: `spec.md` no expone ninguna operación a nivel de una sola entrada de horario — el
  horario siempre se crea o se reemplaza completo, junto con la sucursal (FR-004, FR-009, y la
  asunción de "reemplazo completo" confirmada en `spec.md`). Modelarlo como un objeto hijo del
  agregado `Branch` (en vez de una entidad de primera clase con su propio repositorio) evita una
  capa de indirección sin ningún caso de uso que la requiera (Principio IV: no construir para
  requisitos hipotéticos).
- **Alternatives considered**: `IBranchScheduleRepository : IBaseRepository<BranchSchedule>`
  separado, con endpoints `PUT /api/branches/{id}/schedule/{day}` — rechazada: no la pide `spec.md`
  ni el input del usuario; añadiría superficie sin un requisito que la justifique.

## Decisión 5: Reemplazo completo del horario en `PUT` vía *cascade delete* de EF Core

- **Decision**: `BranchScheduleConfiguration` configura la relación como requerida
  (`HasOne(x => x.Branch).WithMany(b => b.Schedule).HasForeignKey(x => x.BranchId).IsRequired()`).
  En `BranchService.UpdateBranchAsync`, la entidad `Branch` se obtiene con su `Schedule` ya cargado
  (`GetByIdWithScheduleAsync`, mismo `DbContext` con seguimiento activo), y el reemplazo se hace con
  `branch.Schedule.Clear()` seguido de agregar las 7 entradas nuevas a la misma colección, antes de
  `CommitAsync()`.
- **Rationale**: Cuando una relación es *requerida* (FK no-nullable) y la entidad hija está
  siendo *rastreada* por el `DbContext` (lo que garantiza `GetByIdWithScheduleAsync` vía
  `Include`), EF Core marca automáticamally como eliminadas las entidades hijas que quedan
  huérfanas al removerlas de la colección de navegación — comportamiento documentado de EF Core
  para relaciones requeridas, sin necesidad de borrarlas una por una a mano ni de exponer un
  repositorio de `BranchSchedule`. Esto implementa la asunción de "reemplazo completo" con el
  mínimo de código posible y sin abrir una segunda vía de acceso a `BranchSchedule` (ver Decisión
  4).
- **Alternatives considered**: Borrar manualmente las entradas existentes vía un repositorio
  genérico (`IBaseRepository<BranchSchedule>.RemoveRange(...)`) antes de agregar las nuevas —
  rechazada: exige registrar un repositorio adicional solo para esta operación cuando el
  comportamiento de *cascade delete* de EF Core ya resuelve el mismo problema sin código extra
  (Principio III).

## Decisión 6: Construcción manual de la entidad en la creación (no `_mapper.Map`)

- **Decision**: `BranchService.CreateBranchAsync` construye `new Branch { ... }` y su lista de
  `BranchSchedule` a mano a partir de `CreateBranchDto`, en vez de usar `_mapper.Map<Branch>(dto)`.
  `AutoMapper` (vía `BranchMappingProfile`) se usa exclusivamente para la salida
  (`Branch → BranchDto`, `BranchSchedule → ScheduleEntryDto`).
- **Rationale**: Es exactamente el patrón ya establecido en `ShipmentService.CreateShipmentAsync`
  (que tampoco usa `_mapper.Map` para `CreateShipmentDto → Shipment`, pese a que existe un
  `CreateMap` sin usar en `ShipmentTracker.Web/Mappers/MappingProfiles.cs`). Seguirlo evita
  introducir un segundo patrón de creación que compita con el ya adoptado (Principio II), y hace
  explícito en el código dónde se fijan los valores que el cliente no controla (`IsActive = true`
  por FR-003, `CreatedAt = DateTime.UtcNow`).
- **Alternatives considered**: Mapear con AutoMapper y sobrescribir `IsActive`/`CreatedAt` después
  — rechazada: mezclar ambos estilos en el mismo módulo es más difícil de seguir que elegir uno
  consistente con el resto de la solución, sin ninguna ganancia real.

## Decisión 7: `onlyActive` (bool, default `true`) cubre los tres casos de listado de `spec.md`

- **Decision**: `GET /api/branches` recibe `bool onlyActive = true` (no un `bool?` ni un tercer
  valor "todos"). El filtro que arma `BranchService.GetBranchesAsync` siempre incluye
  `x.IsActive == onlyActive`, más el filtro opcional de `type`.
- **Rationale**: El input del usuario fijó la firma exacta `onlyActive=bool`. Verificado contra
  `spec.md`: (1) sin query params → `onlyActive` toma su default `true` → solo activas, igual que
  FR-014; (2) `onlyActive=true` explícito → mismo resultado; (3) `onlyActive=false` → devuelve
  únicamente las inactivas (`IsActive == false`), que es exactamente el escenario 3 de la User
  Story 2 ("filtro explícito a inactivas devuelve solo inactivas"). Los tres escenarios de
  `spec.md` quedan cubiertos con un `bool` simple, sin necesitar un tercer valor "todas" que nadie
  pidió.
- **Alternatives considered**: `bool? onlyActive` con `null` = "todas" — rechazada: el input del
  usuario especifica `onlyActive=bool` (no nullable), y `spec.md` nunca pide un modo "activas +
  inactivas mezcladas en una sola respuesta"; agregarlo sería alcance no solicitado.

## Decisión 8: `DELETE /api/branches/{id}` es soft-delete — no existe borrado físico en ninguna capa

- **Decision**: `[HttpDelete("{id}")]` en `BranchController` invoca
  `BranchService.DeactivateBranchAsync(id)`, que únicamente pone `IsActive = false` (o no hace
  nada si ya estaba inactiva — idempotente, FR-011). Ningún método de `IBranchRepository` ni de
  `IBranchService` expone una eliminación real de filas.
- **Rationale**: El input del usuario pide explícitamente `DELETE → soft delete only`, y FR-012
  exige que no exista ninguna capacidad de borrado físico. Usar el verbo HTTP `DELETE` para una
  operación de desactivación es una convención REST común (el recurso deja de estar disponible
  para las operaciones normales, aunque el registro persista) — se documenta explícitamente en
  `contracts/branches-api-contract.md` para que no se confunda con un borrado real.
- **Alternatives considered**: Endpoint separado `PATCH /api/branches/{id}/deactivate` en vez de
  `DELETE` — rechazada: el input del usuario fijó `DELETE` explícitamente para esta acción.

## Decisión 9: Errores de validación vía `FluentValidation.ValidationException` con lista completa de errores

- **Decision**: `BranchService.CreateBranchAsync`/`UpdateBranchAsync` invocan
  `IValidator<T>.ValidateAsync(dto)` y, si `!result.IsValid`, lanzan
  `new FluentValidation.ValidationException(result.Errors)` (tipo ya incluido en el paquete
  FluentValidation, no requiere código nuevo). `BranchController` captura ese tipo específico y
  responde `400` con la lista completa de errores (`{ property, message }[]`).
- **Rationale**: `spec.md` pide que se reporte "el problema de validación específico" cuando hay
  varios campos inválidos a la vez (p. ej. dirección incompleta *y* horario con días duplicados en
  el mismo request) — devolver solo el primer error, como hace hoy
  `ShipmentController.UpdateStatus` con `InvalidOperationException`, perdería información en esos
  casos. `ValidationException` de FluentValidation ya trae la colección completa de
  `ValidationFailure`, así que no hace falta un tipo de excepción propio.
- **Alternatives considered**: Reutilizar `InvalidOperationException` con solo el primer mensaje
  (mismo estilo que `ShipmentController`) — rechazada: pierde errores simultáneos cuando el
  request viola varias reglas a la vez, algo mucho más probable aquí (múltiples campos de
  dirección + 7 entradas de horario) que en la transición de un solo campo de `Shipment`.

## Decisión 10: `Latitude`/`Longitude` como `double?`, validados solo cuando vienen con valor

- **Decision**: `Branch.Latitude`/`Branch.Longitude` son `double?`. `CreateBranchDtoValidator`
  aplica `RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue)` (y el
  equivalente `-180..180` para `Longitude`).
- **Rationale**: Implementa FR-002 (opcionales) y FR-018 (rango geográfico válido cuando están
  presentes) con el tipo numérico estándar de .NET para coordenadas, sin dependencias externas.
- **Alternatives considered**: `decimal?` — rechazada sin una razón de negocio que lo requiera
  (no hay cálculos financieros); `double` es el tipo convencional para latitud/longitud en .NET.

## Decisión 11: `GetByIdWithScheduleAsync` nuevo en `IBranchRepository` (FR-015)

- **Decision**: `IBranchRepository` agrega `Task<Branch?> GetByIdWithScheduleAsync(int id)`,
  implementado en `BranchRepository` con `dbSet.Include(x => x.Schedule).SingleOrDefaultAsync(x =>
  x.Id == id)`. `GET /api/branches/{id}` y el flujo de `PUT` (que necesita el horario actual
  cargado para reemplazarlo, Decisión 5) lo usan; `GET /api/branches` (listado) no lo necesita.
- **Rationale**: `IBaseRepository<T>.GetByIdAsync` usa `dbSet.FindAsync(id)`, que no soporta
  `Include` de navegaciones. FR-015 exige que la recuperación de una sola sucursal *siempre*
  incluya su horario completo — un método dedicado con `Include` es la forma mínima de cumplirlo
  sin modificar el contrato de `IBaseRepository<T>` (que es compartido con `Shipment` y no debe
  cambiar por una necesidad específica de `Branch`).
- **Alternatives considered**: Usar `GetAsync(filter: x => x.Id == id, includeProperties:
  "Schedule")` (ya existente en `IBaseRepository<T>`) en vez de un método nuevo — rechazada: exige
  luego un `.SingleOrDefault()` sobre el `IEnumerable` resultante en el `Service`, y usa cadenas
  mágicas (`"Schedule"`) para algo que ocurre en exactamente dos lugares; un método tipado en
  `IBranchRepository` es igual de simple y más explícito.

## Decisión 12: Representación JSON de `BranchType`/`ScheduleDay` — nombre del enum, por propiedad

- **Decision**: `BranchType`/`ScheduleDay` en los DTOs de Branch (`BranchDto`, `CreateBranchDto`,
  `UpdateBranchDto`, `ScheduleEntryDto`, `ScheduleEntryInputDto`) llevan
  `[JsonConverter(typeof(JsonStringEnumConverter))]` a nivel de propiedad, en vez de registrar un
  `JsonStringEnumConverter` global en `Program.cs`.
- **Rationale**: Sin ningún conversor, System.Text.Json serializa/deserializa enums como su valor
  numérico subyacente por defecto — probado en la fase de implementación: `{"type": "Hub"}` fallaba
  con un `400` de binding. FR-001 lista los tipos por nombre (`Headquarters`, `Hub`, `SalesPoint`,
  `PickupPoint`), y el contrato/quickstart de esta feature ya asumían nombres legibles en el JSON,
  no códigos numéricos — un conversor por propiedad, scopeado solo a los DTOs de `Branch`, logra
  esto sin tocar la configuración global de JSON. Un conversor **global** habría cambiado también
  la forma de `ShipmentStatus` en `ShipmentDto`/`UpdateStatus` (hoy numérico), violando el
  Principio IV y la garantía explícita de este módulo de no alterar el contrato de `Shipment` —
  verificado en la fase de implementación (respuesta de `GET /api/shipment` sigue devolviendo
  `"status": 2`, sin cambios, tras aplicar el conversor scopeado).
- **Alternatives considered**: `AddJsonOptions` global con `JsonStringEnumConverter` — rechazada
  por la razón de arriba (rompería silenciosamente el contrato de `Shipment`).

## Decisión 13: `opensAt`/`closesAt` requieren formato `HH:mm:ss` en el JSON

- **Decision**: Ninguna configuración adicional — se documenta en `contracts/` y `quickstart.md`
  que el conversor incorporado de `TimeOnly` de System.Text.Json exige `HH:mm:ss` (p. ej.
  `"08:00:00"`); `HH:mm` sin segundos (p. ej. `"08:00"`) falla la deserialización con `400`.
- **Rationale**: Comportamiento de fábrica de .NET 8 para `TimeOnly`, verificado en la fase de
  implementación. No se agregó un conversor personalizado — no está pedido por `spec.md` ni
  aporta valor suficiente frente a documentar el formato esperado.
- **Alternatives considered**: Conversor personalizado que acepte también `HH:mm` — rechazada por
  Principio III (dependencia/código nuevo sin una necesidad concreta más allá de conveniencia).

## Estado

Todos los `NEEDS CLARIFICATION` resueltos. Sin bloqueos para Phase 1. Decisiones 12-13 registradas
durante la fase de implementación (`/speckit-implement`), no en la planificación original.
