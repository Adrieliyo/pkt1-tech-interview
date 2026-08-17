# Research: Employees & Vehicles Module

Todos los `NEEDS CLARIFICATION` del Technical Context ya se resolvieron: el usuario fijó
explícitamente el stack, las entidades, las restricciones de unicidad/FK, la instrucción de
reutilizar la paginación de `002`, y las diez rutas HTTP en el input de `/speckit-plan`. Este
documento registra las decisiones técnicas derivadas de ese input, de `spec.md` (incluidas sus
Clarifications) y de la investigación directa del código actual (`ShipmentService`,
`ShipmentController`, `BranchService`, `IBaseRepository`, `Branch.cs`).

## Decisión 1: `FirstName`/`LastName` en vez del `name` genérico de spec.md

- **Decision**: `Employee` tiene `FirstName` y `LastName` como campos separados, tal como los
  especificó el usuario en el input de planificación, reemplazando la asunción genérica de un solo
  campo `name` que se había documentado en `spec.md` (agregado ahí porque el input original de
  `/speckit-specify` no mencionaba ningún campo de nombre).
- **Rationale**: El input de planificación es una decisión técnica explícita y más específica que la
  asunción de alto nivel de `spec.md`; dividir el nombre es además el estándar para registros de
  personal (permite ordenar/buscar por apellido). No contradice ningún FR de `spec.md` — solo
  concreta cómo se implementa el campo "nombre".
- **Alternatives considered**: Mantener un solo campo `Name` como decía la asunción de `spec.md` —
  rechazada porque el usuario, con más contexto técnico en esta fase, pidió explícitamente el split.

## Decisión 2: `Phone` en `Employee` — campo agregado en la fase de planificación

- **Decision**: `Employee.Phone` (`string?`, opcional) se agrega tal como lo especificó el usuario en
  el input de planificación, aunque `spec.md` no lo menciona en absoluto.
- **Rationale**: Es una adición razonable y de bajo riesgo (opcional, sin reglas de negocio) hecha
  explícitamente por el usuario con contexto técnico adicional al de la especificación original —
  mismo patrón que `Branch.Phone`, ya opcional en el módulo 003.
- **Alternatives considered**: Omitirlo por no estar en `spec.md` — rechazada; el usuario lo pidió
  explícitamente en esta sesión y es aditivo, sin conflicto con ningún FR existente.

## Decisión 3: `CreatedAt`/`UpdatedAt` en `Employee`, solo `CreatedAt` en `Vehicle` — asimetría intencional

- **Decision**: Se implementa exactamente como especificó el usuario: `Employee` tiene tanto
  `CreatedAt` como `UpdatedAt` (`DateTime?`, `null` hasta la primera actualización); `Vehicle` tiene
  únicamente `CreatedAt`. `UpdatedAt` se asigna a `DateTime.UtcNow` en cada `UpdateEmployeeAsync`
  exitoso.
- **Rationale**: El input de planificación lista estos campos de forma asimétrica de manera explícita
  para cada entidad — se respeta tal cual, sin "corregir" la asimetría agregando `UpdatedAt` a
  `Vehicle` por consistencia, ya que eso excedería lo pedido (Principio IV).
- **Alternatives considered**: Agregar `UpdatedAt` también a `Vehicle` por paralelismo — rechazada,
  no fue solicitada y ampliaría el alcance sin una necesidad concreta.

## Decisión 4: `BranchId` como `int` en ambas entidades — corrección del `guid` del input

- **Decision**: `Employee.BranchId` y `Vehicle.BranchId` son `int` en ambas entidades. El query
  string de `GET /api/vehicles` se documenta como `?branchId={int}`, no `?branchId={guid}` como
  aparecía literalmente en el input del usuario.
- **Rationale**: `Branch.Id` (módulo 003, ya implementado) es `int` (identity autoincremental), no
  `Guid` — no existe una clave `Guid` en ningún lado de este sistema. El input del usuario fija
  `branchId=id` para `Employee` pero `branchId=guid` para `Vehicle`, una inconsistencia interna del
  propio input que muy probablemente es un artefacto de copiar una plantilla genérica. Usar `Guid`
  real rompería la FK contra `Branch.Id` (tipos incompatibles) — se corrige a `int` en ambas rutas
  para que el módulo compile y funcione contra el `Branch` real.
- **Alternatives considered**: Implementar `Guid` tal cual decía el input para `Vehicle` — rechazada
  de plano: sería un error de tipos que impide siquiera declarar la FK contra `Branches.Id` (`int`).

## Decisión 5: Validación estructural (FluentValidation) + validación dependiente de BD (Service) — primera vez combinadas en este módulo

- **Decision**: Los validadores de FluentValidation (`CreateEmployeeDtoValidator`,
  `UpdateEmployeeDtoValidator`, `CreateVehicleDtoValidator`, `UpdateVehicleDtoValidator`) cubren
  **únicamente** reglas estructurales/sincrónicas: campos requeridos, formato de email, enum válido,
  año no futuro, capacidad positiva. Las reglas que requieren consultar la base de datos — sucursal
  existente y activa (FR-003/FR-013), unicidad global de email/número de empleado/placa incluso
  contra registros inactivos (FR-002/FR-012) — se resuelven directamente en `EmployeeService`/
  `VehicleService`, **después** de que la validación estructural pasa, acumulando los errores en la
  misma lista y lanzando un único `FluentValidation.ValidationException` con todos los problemas
  encontrados (estructurales + de negocio) si hay alguno, antes de tocar la entidad.
- **Rationale**: Ninguna consulta a `IUnitOfWork` puede vivir dentro de un `AbstractValidator<T>`
  sincrónico sin convertirlo en asíncrono con inyección de repositorio — un patrón que este proyecto
  nunca necesitó hasta ahora porque ni `Shipment` ni `Branch` tienen restricciones de unicidad
  dependientes de BD. Mantener las reglas de BD en el `Service` (en vez de introducir validadores
  `MustAsync` con `IUnitOfWork` inyectado) es la extensión más consistente con el patrón ya
  establecido: el `Service` ya es responsable de las reglas de negocio (ver
  `ShipmentTransitionValidator` invocado desde `ShipmentService`, o la re-validación completa en
  `BranchService.UpdateBranchAsync`). Se evita así introducir un segundo estilo de validación que
  compita con el ya adoptado (Principio II).
- **Alternatives considered**: Validadores FluentValidation asíncronos con `IUnitOfWork` inyectado
  (`MustAsync`) — rechazada: es una capacidad válida de FluentValidation, pero sería la primera vez
  que un validador de este proyecto toca la base de datos, mezclando dos responsabilidades
  (estructura vs. negocio) que hoy están claramente separadas entre `Validators/` y `Service`.

## Decisión 6: Método privado compartido para las reglas de negocio de Create/Update

- **Decision**: `EmployeeService` y `VehicleService` cada uno implementan un método privado
  (`ValidateBusinessRulesAsync(dto, currentId)`) que arma la lista de `ValidationFailure` para
  sucursal activa + unicidad, reutilizado tanto por `CreateXAsync` (`currentId = 0`) como por
  `UpdateXAsync` (`currentId = id` del registro que se está editando, para excluirlo de su propia
  comprobación de unicidad).
- **Rationale**: Como los IDs reales empiezan en 1 (identity), pasar `currentId = 0` en la creación
  hace que la condición `x.Id != currentId` nunca excluya nada — permite un solo método para ambos
  flujos sin ramas especiales, evitando duplicar la lógica de validación entre `CreateXAsync` y
  `UpdateXAsync` (igual que `CreateBranchDtoValidator`/`UpdateBranchDtoValidator` comparten
  `ScheduleEntryInputDtoValidator` en el módulo 003, pero aquí a nivel de método de servicio en vez
  de validador, por ser lógica dependiente de BD).
- **Alternatives considered**: Duplicar la lógica de validación de negocio por separado en cada
  método `CreateXAsync`/`UpdateXAsync` — rechazada: violaría el mismo principio de no-duplicación que
  ya se siguió en el módulo 003.

## Decisión 7: Unicidad sin filtrar por `IsActive` — índice único simple, sin filtro parcial

- **Decision**: Los índices únicos de `Employees.Email`, `Employees.EmployeeNumber` y `Vehicles.Plate`
  son índices únicos simples (`HasIndex(...).IsUnique()`), sin ninguna cláusula de filtro sobre
  `IsActive`. La comprobación de unicidad en el `Service` tampoco filtra por `IsActive` — compara
  contra **todos** los registros, activos e inactivos.
- **Rationale**: Confirmado en Clarifications de `spec.md` — un email/número de empleado/placa usado
  una vez queda reservado para siempre, incluso si el registro que lo usa se desactiva. Un índice
  único simple (sin filtro) es exactamente esa semántica a nivel de base de datos — defensa en
  profundidad adicional a la comprobación del `Service`, mismo patrón que el índice único de
  `(BranchId, DayOfWeek)` en el módulo 003.
- **Alternatives considered**: Índice único filtrado (`WHERE IsActive = 1`), que permitiría reutilizar
  el valor tras desactivar el registro — rechazada explícitamente por la clarificación de `spec.md`.

## Decisión 8: Normalización (trim) de los campos usados en unicidad, antes de validar y guardar

- **Decision**: `EmployeeService`/`VehicleService` aplican `.Trim()` a `Email`, `EmployeeNumber` y
  `Plate` (y, por consistencia, a `FirstName`/`LastName`/`Brand`/`Model`) al recibir el DTO, antes de
  ejecutar la validación estructural y las comprobaciones de unicidad.
- **Rationale**: El edge case de `spec.md` exige que la unicidad ignore espacios al inicio/final. La
  parte de "case-insensitive" del mismo edge case se resuelve gratis por la collation por defecto de
  SQL Server (`SQL_Latin1_General_CP1_CI_AS`, ya en uso implícito por el índice único existente de
  `Shipment.TrackingNumber`, que nunca necesitó código especial para eso) — no se agrega lógica de
  comparación case-insensitive a mano.
- **Alternatives considered**: Normalizar también a mayúsculas/minúsculas en código — rechazada como
  innecesaria dado que la collation por defecto ya cubre ese caso (Principio III, minimalismo).

## Decisión 9: Enums nullable (`EmployeeRole?`, `VehicleType?`) en los DTOs de entrada

- **Decision**: `CreateEmployeeDto.Role`, `UpdateEmployeeDto.Role`, `CreateVehicleDto.Type` y
  `UpdateVehicleDto.Type` son nullable, validados con `.NotNull().IsInEnum()` — mismo patrón que
  `BranchType?`/`ScheduleDay?` en el módulo 003 (research.md de 003, Decisión 1).
- **Rationale**: Evita que un campo omitido en el JSON se interprete silenciosamente como el primer
  valor del enum (`Operator`/`Motorcycle`) en vez de fallar la validación — mismo razonamiento que ya
  se aplicó en el módulo 003.
- **Alternatives considered**: Dejarlos no-nullable — rechazada por la misma razón que en 003.

## Decisión 10: `BranchId` NO se hace nullable (a diferencia de los enums)

- **Decision**: `CreateEmployeeDto.BranchId`/`CreateVehicleDto.BranchId` son `int` simple, no `int?`.
- **Rationale**: A diferencia de un enum (donde `0` es un valor de negocio legítimo), un `BranchId`
  omitido llega como `0` por default de JSON, y `0` nunca es un id real de sucursal (identity
  autoincremental empieza en 1) — la comprobación de "la sucursal debe existir y estar activa"
  (Decisión 5) ya rechaza `0` con un mensaje claro, sin necesitar el envoltorio nullable.
- **Alternatives considered**: `int?` por paralelismo con los enums — rechazada como complejidad
  innecesaria; el caso ya está cubierto sin él.

## Decisión 11: Representación JSON de `EmployeeRole`/`VehicleType` — mismo patrón scopeado que `Branch`

- **Decision**: `Role`/`Type` en los DTOs de `Employee`/`Vehicle` llevan
  `[JsonConverter(typeof(JsonStringEnumConverter))]` por propiedad, igual que `BranchType`/
  `ScheduleDay` en el módulo 003 (research.md de 003, Decisión 12).
- **Rationale**: Mismo problema y misma solución que en 003 — sin esto, `{"role": "Driver"}` fallaría
  la deserialización (System.Text.Json usa el valor numérico por defecto). Un conversor por propiedad
  mantiene sin cambios la serialización numérica de `ShipmentStatus`.
- **Alternatives considered**: Conversor global — rechazada por la misma razón que en 003 (rompería
  el contrato existente de `Shipment`).

## Decisión 12: `BranchId` como filtro de query string — sin conversor especial

- **Decision**: `[FromQuery] int? branchId` en ambos controladores no necesita ningún conversor
  adicional — el model binding de query string de ASP.NET Core ya convierte `?branchId=5` a `int`
  de forma nativa, y `[FromQuery] EmployeeRole? role` acepta el nombre del enum (`?role=Driver`) sin
  `JsonStringEnumConverter`, porque el binding de query string usa un binder de enums distinto al
  serializador JSON del body (mismo comportamiento ya verificado para `?type=Hub` en el módulo 003).
- **Rationale**: Documentar explícitamente que esta asimetría (JSON body necesita el conversor
  scopeado, query string no) es intencional y ya está probada en producción por el módulo 003, para
  que no se intente "arreglar" agregando un conversor donde no hace falta.
- **Alternatives considered**: N/A — comportamiento nativo de ASP.NET Core ya suficiente.

## Decisión 13: Paginación reutilizada de `002` para ambos listados

- **Decision**: `GetEmployeesAsync`/`GetVehiclesAsync` devuelven `PagedResult<EmployeeDto>`/
  `PagedResult<VehicleDto>` (tipo ya existente en `Core/DTOs/PagedResult.cs`), con los mismos
  parámetros y defaults que `ShipmentService.GetShipmentsAsync`: `page = 1`, `pageSize = 5`,
  `MaxPageSize = 50` (clamping, no rechazo), orden `OrderByDescending(x => x.CreatedAt)`. Los
  controladores exponen los mismos 4 headers (`X-Total-Count`, `X-Page`, `X-Page-Size`,
  `X-Total-Pages`) que `ShipmentController.GetShipments`, ya expuestos vía CORS
  (`WithExposedHeaders`, configurado una sola vez en `Program.cs` para toda la API — no requiere
  cambio adicional aquí).
- **Rationale**: Instrucción explícita del usuario ("Keep my pagination rules for global GET
  endpoints, you can check for Shipment module"). Reutilizar `PagedResult<T>` y el mismo
  `skip`/`take` de `IBaseRepository<T>.GetAsync` (ya soporta ambos desde el módulo 002) evita
  duplicar el mecanismo de paginación.
- **Alternatives considered**: Paginación con página inicial más grande dado que el listado de
  "buscar choferes en una sucursal" (US2 de spec.md) idealmente cabe en una sola llamada — rechazada:
  el usuario pidió explícitamente mantener las mismas reglas que `Shipment`; si una sucursal tiene
  más de 5 choferes activos, el llamador puede pedir `pageSize` mayor (hasta 50), igual que ya
  funciona para envíos.

## Decisión 14: Sin colección inversa `Employee`/`Vehicle` en `Branch.cs`

- **Decision**: `Branch.cs` (módulo 003) no se modifica — no se le agrega
  `ICollection<Employee> Employees` ni `ICollection<Vehicle> Vehicles`. La relación se configura de
  forma unidireccional: `HasOne(x => x.Branch).WithMany()` (sin argumento) en
  `EmployeeConfiguration`/`VehicleConfiguration`.
- **Rationale**: EF Core soporta relaciones unidireccionales sin necesidad de una colección de
  navegación en el lado principal. Agregar esas colecciones a `Branch.cs` sería modificar el módulo
  003 sin una necesidad concreta de este módulo (que nunca necesita cargar "todos los empleados de
  una sucursal" a través de `Branch` — siempre consulta `Employee`/`Vehicle` directamente vía
  `IUnitOfWork.EmployeeRepository`/`VehicleRepository` con un filtro `BranchId`) — mantiene el
  cambio aditivo y aislado (Principio IV).
- **Alternatives considered**: Agregar las colecciones inversas a `Branch` por completitud del
  modelo — rechazada como alcance no solicitado y sin caso de uso real en este módulo.

## Decisión 15: Reactivación vía `PUT` — mismo precedente que `Branch`, sin nueva clarificación

- **Decision**: `IsActive` es un campo editable más en `UpdateEmployeeDto`/`UpdateVehicleDto`,
  permitiendo reactivar un registro inactivo por la misma vía que cualquier otra actualización — no
  existe una acción `PATCH .../activate` separada.
- **Rationale**: `spec.md` no aclaró explícitamente este punto para este módulo (a diferencia de
  `Branch`, donde sí hubo una clarificación dedicada), pero no hay ninguna señal en contra y el
  precedente ya establecido en el módulo 003 (`IsActive` editable vía `PUT`) es la opción de menor
  riesgo y más consistente — se documenta aquí como decisión de planificación en vez de bloquear con
  una clarificación de bajo impacto adicional.
- **Alternatives considered**: Bloquear la reactivación hasta una clarificación nueva — rechazada
  como fricción innecesaria dado que el precedente del propio sistema ya resuelve la pregunta de
  forma razonable.

## Estado

Todos los `NEEDS CLARIFICATION` resueltos. Sin bloqueos para Phase 1.
