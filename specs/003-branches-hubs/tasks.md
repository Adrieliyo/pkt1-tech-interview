---
description: "Task list template for feature implementation"
---

# Tasks: Branches & Hubs Module

**Input**: Design documents from `/specs/003-branches-hubs/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/branches-api-contract.md, quickstart.md

**Tests**: Sin tareas de pruebas automatizadas — el proyecto no tiene proyecto de pruebas (ver
constitución); verificación manual vía `quickstart.md`, misma política que
`001-standardize-mapping-di` y `002-paginate-shipment-list`.

**Organization**: Agrupadas por historia de usuario, con una fase Foundational previa (a
diferencia de `002`, aquí sí aplica: `Branch`/`BranchSchedule` son entidades completamente nuevas
que necesitan persistencia, repositorio y DI antes de que cualquier historia pueda ejecutarse).
`IBranchService`, `BranchService.cs` y `BranchController.cs` se **crean en la Historia 1 y se
amplían en cada historia siguiente** (un método/acción nuevo por historia, mismo patrón de
"mismo archivo, tarea distinta" que ya usó `002` con `ShipmentController.cs`) — no se generan
stubs `NotImplementedException`; cada método se agrega solo cuando la historia que lo necesita
llega a esa tarea.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivo distinto, sin dependencia de tareas incompletas)
- **[Story]**: US1, US2, US3 o US4
- Cada tarea incluye la ruta exacta del archivo

## Path Conventions

Solución .NET en capas existente (`ShipmentTracker.Core` / `.Infrastructure` / `.Services` /
`.Web`, ver `plan.md`). Rutas relativas a la raíz del repositorio.

---

## Phase 1: Setup

**N/A.** Sin inicialización de proyecto ni paquetes nuevos (research.md: cero dependencias NuGet
nuevas). La solución y los cuatro proyectos ya existen.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Persistencia, repositorio y wiring de DI compartidos por las 4 historias. Ninguna
historia puede empezar hasta que esta fase esté completa.

- [X] T001 [P] Crear enum `BranchType` en `ShipmentTracker.Core/Enums/BranchType.cs`: valores `Headquarters`, `Hub`, `SalesPoint`, `PickupPoint`, con comentario XML en español describiendo cada valor (mismo estilo que `ShipmentStatus.cs`)
- [X] T002 [P] Crear enum `ScheduleDay` en `ShipmentTracker.Core/Enums/ScheduleDay.cs`: valores `Monday`, `Tuesday`, `Wednesday`, `Thursday`, `Friday`, `Saturday`, `Sunday`, con comentario XML
- [X] T003 Crear entidad `BranchSchedule` en `ShipmentTracker.Core/Entities/BranchSchedule.cs`: `Id` (int), `BranchId` (int), `DayOfWeek` (`ScheduleDay`), `OpensAt` (`TimeOnly?`), `ClosesAt` (`TimeOnly?`), `IsClosed` (bool), `Branch` (navegación `Branch`) — ver tabla de campos en `data-model.md` (depende de T002)
- [X] T004 Crear entidad `Branch` en `ShipmentTracker.Core/Entities/Branch.cs`: `Id`, `Name`, `Type` (`BranchType`), `Address`, `City`, `State`, `ZipCode`, `Latitude` (`double?`), `Longitude` (`double?`), `Phone` (`string?`), `IsActive` (bool), `CreatedAt` (`DateTime`), `Schedule` (`ICollection<BranchSchedule>`) — ver tabla de campos en `data-model.md` (depende de T001, T003)
- [X] T005 [P] Crear `ScheduleEntryDto` en `ShipmentTracker.Core/DTOs/ScheduleEntryDto.cs`: `Id`, `DayOfWeek` (`ScheduleDay`), `IsClosed` (bool), `OpensAt` (`TimeOnly?`), `ClosesAt` (`TimeOnly?`) — DTO de salida (depende de T002)
- [X] T006 [P] Crear `ScheduleEntryInputDto` en `ShipmentTracker.Core/DTOs/ScheduleEntryInputDto.cs`: `DayOfWeek` (`ScheduleDay?`, **nullable** para distinguir "omitido" — ver research.md Decisión 1), `IsClosed` (bool), `OpensAt` (`TimeOnly?`), `ClosesAt` (`TimeOnly?`) — DTO de entrada, sin `Id`, compartido por `CreateBranchDto` y `UpdateBranchDto` (depende de T002)
- [X] T007 Crear `BranchDto` en `ShipmentTracker.Core/DTOs/BranchDto.cs`: `Id`, `Name`, `Type` (`BranchType`), `Address`, `City`, `State`, `ZipCode`, `Latitude?`, `Longitude?`, `Phone?`, `IsActive`, `CreatedAt`, `Schedule` (`List<ScheduleEntryDto>`) — DTO de salida usado por las 4 historias (depende de T001, T005)
- [X] T008 [P] Crear `IBranchRepository` en `ShipmentTracker.Core/Interfaces/Repositories/IBranchRepository.cs`: `: IBaseRepository<Branch>` + `Task<Branch?> GetByIdWithScheduleAsync(int id);` (depende de T004)
- [X] T009 En `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs`: agregar `IBranchRepository BranchRepository { get; }` (mismo patrón que `ShipmentRepository`) (depende de T008)
- [X] T010 [P] Crear `BranchConfiguration` en `ShipmentTracker.Infrastructure/Data/Configurations/BranchConfiguration.cs`: `ToTable("Branches")`, `HasKey(Id)` con `UseIdentityColumn()`, `Name`/`Address`/`City`/`State`/`ZipCode` requeridos con `HasMaxLength` razonable, `Type` con `HasConversion<string>()`, `Latitude`/`Longitude`/`Phone` opcionales, `IsActive`/`CreatedAt` requeridos (mismo estilo que `ShipmentConfiguration`) (depende de T004)
- [X] T011 [P] Crear `BranchScheduleConfiguration` en `ShipmentTracker.Infrastructure/Data/Configurations/BranchScheduleConfiguration.cs`: `ToTable("BranchSchedules")`, `HasKey(Id)`, `DayOfWeek` con `HasConversion<string>()`, `HasOne(x => x.Branch).WithMany(b => b.Schedule).HasForeignKey(x => x.BranchId).IsRequired()`, **índice único compuesto** `HasIndex(x => new { x.BranchId, x.DayOfWeek }).IsUnique()` (defensa en profundidad de FR-005, ver data-model.md) (depende de T003)
- [X] T012 En `ShipmentTracker.Infrastructure/Data/AppDbContext.cs`: agregar `DbSet<Branch> Branches` y `DbSet<BranchSchedule> BranchSchedules`, y en `OnModelCreating` agregar `builder.ApplyConfiguration(new BranchConfiguration())` y `builder.ApplyConfiguration(new BranchScheduleConfiguration())` (depende de T010, T011)
- [X] T013 [P] Crear `BranchRepository` en `ShipmentTracker.Infrastructure/Repositories/BranchRepository.cs`: `: BaseRepository<Branch>, IBranchRepository`, implementar `GetByIdWithScheduleAsync(int id)` con `Context.Set<Branch>().Include(x => x.Schedule).SingleOrDefaultAsync(x => x.Id == id)` (depende de T008, T012)
- [X] T014 En `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs`: agregar campo privado `_branchRepository` y propiedad lazy `IBranchRepository BranchRepository => _branchRepository ??= new BranchRepository(_context);` (mismo patrón que `ShipmentRepository`) (depende de T009, T013)
- [X] T015 Generar la migración de EF Core: `dotnet ef migrations add AddBranchesAndSchedule --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` — crea `ShipmentTracker.Infrastructure/Migrations/<timestamp>_AddBranchesAndSchedule.cs` con las tablas `Branches`/`BranchSchedules`, la FK y el índice único; no modifica ninguna migración existente (depende de T012)
- [X] T016 [P] Crear `BranchMappingProfile` en `ShipmentTracker.Services/Mappings/BranchMappingProfile.cs`: `CreateMap<Branch, BranchDto>()` y `CreateMap<BranchSchedule, ScheduleEntryDto>()` (solo salida — la creación construye la entidad a mano, ver research.md Decisión 6) (depende de T004, T007; puede hacerse en paralelo con T008-T014)
- [X] T017 En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IBranchRepository, BranchRepository>();` (mismo patrón que el registro ya existente de `IShipmentRepository`, aunque `UnitOfWork` construye el repositorio directamente) (depende de T013)

**Checkpoint**: Persistencia, repositorio y `IUnitOfWork.BranchRepository` listos. Ninguna
historia tiene aún servicio, controlador ni endpoint — eso empieza en la Fase 3.

---

## Phase 3: User Story 1 - Registrar una nueva sucursal (Priority: P1) 🎯 MVP

**Goal**: `POST /api/branches` crea una sucursal activa con dirección completa y horario de 7 días,
rechazando con `400` cualquier dato inválido (dirección incompleta, tipo faltante, horario con
días faltantes/duplicados/inconsistentes, coordenadas fuera de rango).

**Independent Test**: `POST /api/branches` con un cuerpo válido (tipo `Hub`, dirección completa,
7 entradas de horario) devuelve `201` con `isActive: true` y el horario guardado tal cual;
`POST` con cada tipo de dato inválido por separado devuelve `400` (escenarios 1-4 de
`quickstart.md`, User Story 1).

### Implementation for User Story 1

- [X] T018 [US1] Crear `CreateBranchDto` en `ShipmentTracker.Core/DTOs/CreateBranchDto.cs`: `Name`, `Type` (`BranchType?`), `Address`, `City`, `State`, `ZipCode`, `Latitude?`, `Longitude?`, `Phone?`, `Schedule` (`List<ScheduleEntryInputDto>`) — **sin** `IsActive` (siempre `true` al crear, FR-003) (depende de T006)
- [X] T019 [US1] Crear `ScheduleEntryInputDtoValidator` en `ShipmentTracker.Services/Validators/ScheduleEntryInputDtoValidator.cs` (`AbstractValidator<ScheduleEntryInputDto>`): `DayOfWeek` no nulo y `IsInEnum()`; si `IsClosed == false`, `OpensAt`/`ClosesAt` requeridos y `OpensAt < ClosesAt`; si `IsClosed == true`, `OpensAt`/`ClosesAt` deben ser `null` — ver reglas en data-model.md (reutilizado sin cambios por la Historia 3) (depende de T006)
- [X] T020 [US1] Crear `CreateBranchDtoValidator` en `ShipmentTracker.Services/Validators/CreateBranchDtoValidator.cs` (`AbstractValidator<CreateBranchDto>`): `Name`/`Address`/`City`/`State`/`ZipCode` no vacíos; `Type` no nulo y `IsInEnum()`; `Latitude` en `[-90, 90]` si tiene valor; `Longitude` en `[-180, 180]` si tiene valor; `Schedule.Count == 7`; sin `DayOfWeek` repetidos; `RuleForEach(x => x.Schedule).SetValidator(new ScheduleEntryInputDtoValidator())` (depende de T018, T019)
- [X] T021 [US1] Crear `IBranchService` en `ShipmentTracker.Core/Interfaces/Services/IBranchService.cs` con el primer método: `Task<BranchDto> CreateBranchAsync(CreateBranchDto dto);` (depende de T018)
- [X] T022 [US1] Crear `BranchService` en `ShipmentTracker.Services/BranchService.cs`, `: IBranchService`, constructor `(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateBranchDto> createValidator)`; implementar `CreateBranchAsync`: validar con `createValidator.ValidateAsync`, si inválido lanzar `new FluentValidation.ValidationException(result.Errors)` (research.md Decisión 9); si válido, construir `Branch`+`List<BranchSchedule>` a mano (`IsActive = true`, `CreatedAt = DateTime.UtcNow`, ver research.md Decisión 6), `AddAsync`+`CommitAsync` vía `_unitOfWork.BranchRepository`, retornar `_mapper.Map<BranchDto>(nuevo)` (depende de T020, T021, T014, T016)
- [X] T023 [US1] Crear `BranchController` en `ShipmentTracker.Web/Controllers/BranchController.cs`, ruta `[Route("api/branches")]`, acción `[HttpPost] CreateBranch([FromBody] CreateBranchDto dto)`: llama al servicio, retorna `Created($"/api/branches/{result.Id}", result)` (**sin** `CreatedAtAction`/`nameof` — evita depender de la acción `GetBranchById`, que aún no existe hasta la Historia 2), captura `FluentValidation.ValidationException` y retorna `400` con `{ errors: [{ property, message }] }`; agregar comentarios XML (mismo estilo que `ShipmentController`) (depende de T022)
- [X] T024 [US1] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IBranchService, BranchService>();` y `builder.Services.AddScoped<IValidator<CreateBranchDto>, CreateBranchDtoValidator>();` (depende de T022, T020, T017 — mismo archivo)

**Checkpoint**: `POST /api/branches` funciona de punta a punta. Historia 1 verificable de forma
independiente (escenarios US1 de `quickstart.md`).

---

## Phase 4: User Story 2 - Buscar y revisar sucursales (Priority: P2)

**Goal**: `GET /api/branches` lista sucursales (por defecto solo activas; filtros opcionales
`onlyActive`/`type`) y `GET /api/branches/{id}` devuelve una sucursal con su horario completo
siempre incluido, o `404` si no existe.

**Independent Test**: Con sucursales de distinto tipo/estado ya creadas (vía US1), `GET
/api/branches` sin parámetros devuelve solo activas; `GET /api/branches?onlyActive=false`
devuelve solo inactivas; `GET /api/branches?type=Hub` combina ambos filtros; `GET
/api/branches/{id}` incluye `schedule` completo; `GET /api/branches/999999` devuelve `404`
(escenarios 1-6 de `quickstart.md`, User Story 2).

### Implementation for User Story 2

- [X] T025 [US2] En `ShipmentTracker.Core/Interfaces/Services/IBranchService.cs`: agregar `Task<IEnumerable<BranchDto>> GetBranchesAsync(bool onlyActive = true, BranchType? type = null);` y `Task<BranchDto?> GetBranchByIdAsync(int id);` (depende de T021 — mismo archivo)
- [X] T026 [US2] En `ShipmentTracker.Services/BranchService.cs`: implementar `GetBranchesAsync` (arma `Expression<Func<Branch,bool>>` combinando `x.IsActive == onlyActive` con `x.Type == type.Value` si `type.HasValue`, llama a `_unitOfWork.BranchRepository.GetAsync(filter)`, mapea a `IEnumerable<BranchDto>` — ver research.md Decisión 7) e implementar `GetBranchByIdAsync` (usa `GetByIdWithScheduleAsync(id)`, retorna `null` si no existe, si no `_mapper.Map<BranchDto>`) (depende de T022, T025 — mismo archivo)
- [X] T027 [US2] En `ShipmentTracker.Web/Controllers/BranchController.cs`: agregar `[HttpGet] GetBranches([FromQuery] bool onlyActive = true, [FromQuery] BranchType? type = null)` → `200` con `IEnumerable<BranchDto>`, y `[HttpGet("{id}")] GetBranchById(int id)` → `200` con `BranchDto` o `404` con `{ "message": "No se encontró una sucursal con el id '{id}'." }`; comentarios XML (depende de T023, T026 — mismo archivo)

**Checkpoint**: Listado con filtros y recuperación individual con horario funcionan. Historias 1 y
2 verificables juntas.

---

## Phase 5: User Story 3 - Actualizar información de una sucursal (Priority: P3)

**Goal**: `PUT /api/branches/{id}` reemplaza por completo los datos editables de una sucursal
(incluido el horario de 7 días), re-validando todas las reglas antes de escribir cualquier cambio,
y permite reactivar una sucursal inactiva vía `isActive: true`.

**Independent Test**: `PUT` sobre una sucursal existente con dirección/horario válidos distintos a
los originales devuelve `200` y un `GET` posterior refleja el reemplazo completo; `PUT` con un
horario inválido devuelve `400` y la sucursal queda intacta; `PUT` con `isActive: true` sobre una
sucursal previamente desactivada la reactiva (escenarios 1-4 de `quickstart.md`, User Story 3).

### Implementation for User Story 3

- [X] T028 [US3] Crear `UpdateBranchDto` en `ShipmentTracker.Core/DTOs/UpdateBranchDto.cs`: mismos campos que `CreateBranchDto` (`Name`, `Type?`, `Address`, `City`, `State`, `ZipCode`, `Latitude?`, `Longitude?`, `Phone?`, `Schedule`) más `IsActive` (bool) — permite reactivar (depende de T006)
- [X] T029 [US3] Crear `UpdateBranchDtoValidator` en `ShipmentTracker.Services/Validators/UpdateBranchDtoValidator.cs` (`AbstractValidator<UpdateBranchDto>`): mismas reglas que `CreateBranchDtoValidator` (T020), reutilizando `ScheduleEntryInputDtoValidator` (T019) vía `SetValidator` — sin regla especial para `IsActive` (cualquier valor booleano es aceptable) (depende de T028, T019)
- [X] T030 [US3] En `ShipmentTracker.Core/Interfaces/Services/IBranchService.cs`: agregar `Task<BranchDto?> UpdateBranchAsync(int id, UpdateBranchDto dto);` (depende de T028, T025 — mismo archivo)
- [X] T031 [US3] En `ShipmentTracker.Services/BranchService.cs`: agregar `IValidator<UpdateBranchDto> updateValidator` al constructor; implementar `UpdateBranchAsync`: cargar con `GetByIdWithScheduleAsync(id)` (si `null`, retornar `null`); validar con `updateValidator` **antes** de mutar nada (si inválido, lanzar `ValidationException` sin tocar la entidad, FR-009); actualizar campos escalares (`Name`, `Type`, `Address`, `City`, `State`, `ZipCode`, `Latitude`, `Longitude`, `Phone`, `IsActive`); `branch.Schedule.Clear()` y agregar las 7 `BranchSchedule` nuevas desde `dto.Schedule`; `_unitOfWork.BranchRepository.Update(branch)` + `CommitAsync()` (EF Core genera los `DELETE`/`INSERT` del horario, research.md Decisión 5); retornar `_mapper.Map<BranchDto>(branch)` (depende de T026, T029, T030)
- [X] T032 [US3] En `ShipmentTracker.Web/Controllers/BranchController.cs`: agregar `[HttpPut("{id}")] UpdateBranch(int id, [FromBody] UpdateBranchDto dto)` → `200` con `BranchDto`, `404` si el servicio retorna `null`, `400` (mismo formato que `POST`) si `ValidationException` (depende de T027, T031 — mismo archivo)
- [X] T033 [US3] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IValidator<UpdateBranchDto>, UpdateBranchDtoValidator>();` (depende de T029, T024 — mismo archivo)

**Checkpoint**: Reemplazo completo vía `PUT`, incluida la reactivación, funciona. Historias 1-3
verificables juntas.

---

## Phase 6: User Story 4 - Desactivar una sucursal (Priority: P4)

**Goal**: `DELETE /api/branches/{id}` desactiva una sucursal (soft-delete, `IsActive = false`) de
forma idempotente; ninguna capa expone un borrado físico.

**Independent Test**: `DELETE` sobre una sucursal activa devuelve `204` y dicha sucursal deja de
aparecer en `GET /api/branches` sin filtros, pero sigue siendo recuperable completa vía `GET
/api/branches/{id}`; repetir el mismo `DELETE` devuelve `204` de nuevo sin error; `DELETE` sobre
un id inexistente devuelve `404` (escenarios 1-4 de `quickstart.md`, User Story 4).

### Implementation for User Story 4

- [X] T034 [US4] En `ShipmentTracker.Core/Interfaces/Services/IBranchService.cs`: agregar `Task<bool> DeactivateBranchAsync(int id);` (depende de T030 — mismo archivo)
- [X] T035 [US4] En `ShipmentTracker.Services/BranchService.cs`: implementar `DeactivateBranchAsync`: buscar con `SingleOrDefaultAsync(x => x.Id == id)`, si `null` retornar `false`; si `IsActive == true`, ponerlo en `false`, `Update()` + `CommitAsync()`; si ya estaba en `false`, no escribir nada (idempotente, FR-011); retornar `true` en ambos casos de éxito (depende de T031, T034)
- [X] T036 [US4] En `ShipmentTracker.Web/Controllers/BranchController.cs`: agregar `[HttpDelete("{id}")] DeactivateBranch(int id)` → `204` si el servicio retorna `true`, `404` si retorna `false`; comentario XML aclarando que es soft-delete (research.md Decisión 8) (depende de T032, T035 — mismo archivo)

**Checkpoint**: Las 4 historias funcionan juntas — CRUD completo del módulo de Branches & Hubs.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T037 [P] Compilar la solución (`dotnet build ShipmentTracker.sln`) y confirmar cero errores y cero advertencias nuevas (depende de T001-T036)
- [X] T038 Aplicar la migración (`dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web`) y ejecutar de punta a punta todos los escenarios de `specs/003-branches-hubs/quickstart.md` (las 4 historias + edge cases adicionales) (depende de T037)
- [X] T039 [P] Confirmar que ningún archivo de `Shipment` cambió (`git diff --stat` no debe listar rutas de `Shipment*`) y que los 4 endpoints de `ShipmentController` no cambiaron de comportamiento (depende de T037)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Fase 1)**: N/A.
- **Foundational (Fase 2)**: sin dependencias de historias — BLOQUEA las 4 historias.
- **User Story 1 (Fase 3)**: depende de Foundational. Sin dependencias de otras historias.
- **User Story 2 (Fase 4)**: depende de Foundational y de que T021/T022/T023 (US1) ya existan,
  porque T025/T026/T027 modifican esos mismos archivos (`IBranchService.cs`, `BranchService.cs`,
  `BranchController.cs`) agregando métodos nuevos. No depende de que US1 esté "perfecta", solo de
  que esos archivos ya tengan la forma que US2 extiende.
- **User Story 3 (Fase 5)**: depende de que T025/T026/T027 (US2) ya existan, mismo motivo (edita
  los mismos 3 archivos otra vez).
- **User Story 4 (Fase 6)**: depende de que T030/T031/T032 (US3) ya existan, mismo motivo.
- **Polish (Fase 7)**: depende de que las 4 historias estén completas.

### Dentro de Foundational

- T001, T002 → T003 → T004
- T002 → T005, T006 (en paralelo entre sí y con T003/T004)
- T001, T005 → T007
- T004 → T008 → T009
- T004 → T010, T011 (en paralelo entre sí) → T012
- T008, T012 → T013 → T014
- T012 → T015
- T004, T007 → T016 (en paralelo con T008-T014)
- T013 → T017

### Dentro de cada historia

- **US1**: T006 → T018; T018, T019 → T020; T018 → T021; T020, T021, T014, T016 → T022; T022 → T023; T022, T020, T017 → T024
- **US2**: T021 → T025; T022, T025 → T026; T023, T026 → T027
- **US3**: T006 → T028; T028, T019 → T029; T028, T025 → T030; T026, T029, T030 → T031; T027, T031 → T032; T029, T024 → T033
- **US4**: T030 → T034; T031, T034 → T035; T032, T035 → T036

### Parallel Opportunities

- Al iniciar la Fase 2: **T001 y T002** en paralelo. Tras T002: **T005 y T006** en paralelo entre
  sí y con la cadena T003→T004. **T010 y T011** en paralelo entre sí una vez existe T004. **T016**
  puede hacerse en paralelo con toda la cadena T008-T014 una vez existen T004 y T007.
- Cada historia (Fase 3-6), una vez cumplida su dependencia de la historia anterior, edita
  secuencialmente los mismos 3 archivos compartidos (`IBranchService.cs`, `BranchService.cs`,
  `BranchController.cs`) — dentro de una misma historia no hay paralelismo entre esas tareas, pero
  las tareas de DTO/validador nuevas (T018+T019 en US1; T028+T029 en US3) sí pueden hacerse en
  paralelo entre sí antes de tocar los archivos compartidos.

---

## Parallel Example: Foundational

```bash
# Al inicio de la Fase 2, en paralelo:
Task: "Crear enum BranchType en ShipmentTracker.Core/Enums/BranchType.cs"
Task: "Crear enum ScheduleDay en ShipmentTracker.Core/Enums/ScheduleDay.cs"

# Tras completar ScheduleDay, en paralelo:
Task: "Crear ScheduleEntryDto en ShipmentTracker.Core/DTOs/ScheduleEntryDto.cs"
Task: "Crear ScheduleEntryInputDto en ShipmentTracker.Core/DTOs/ScheduleEntryInputDto.cs"
Task: "Crear entidad BranchSchedule en ShipmentTracker.Core/Entities/BranchSchedule.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 solamente)

1. Completar Fase 2: Foundational (T001-T017).
2. Completar Fase 3: User Story 1 (T018-T024).
3. Ejecutar los escenarios 1-4 de User Story 1 en `quickstart.md` para validar de forma aislada.
4. `POST /api/branches` ya es un incremento entregable — permite empezar a poblar la red de
   sucursales, aunque todavía no se puedan listar/editar/desactivar por API.

### Incremental Delivery

1. Foundational → persistencia lista, sin endpoints todavía.
2. US1 → validar → alta de sucursales funcionando (MVP).
3. US2 → validar con escenarios 1-6 → búsqueda y revisión disponibles.
4. US3 → validar con escenarios 1-4 → edición y reactivación disponibles.
5. US4 → validar con escenarios 1-4 → desactivación disponible; CRUD completo.
6. Polish → build limpio + validación manual completa + confirmación de que `Shipment` no cambió.

### Parallel Team Strategy

Dado que US2/US3/US4 modifican los mismos 3 archivos que crea US1 (`IBranchService.cs`,
`BranchService.cs`, `BranchController.cs`), este módulo **no** se presta a repartir las historias
en paralelo entre desarrolladores distintos sin coordinación — se recomienda avanzarlas en
secuencia (P1 → P2 → P3 → P4), aunque cada una sea independientemente verificable al completarse.

---

## Notes

- No hay tareas de test automatizado — el proyecto no tiene proyecto de pruebas (misma política
  que `001` y `002`); la validación es manual vía `quickstart.md`.
- `BranchSchedule` no tiene repositorio, servicio ni controlador propio a propósito — se gestiona
  íntegramente como hijo del agregado `Branch` (research.md, Decisión 4). No generar tareas para
  crearlos.
- El `POST` de la Historia 1 usa `Created(uri, result)` en vez de `CreatedAtAction(nameof(...))`
  para no depender de que la acción `GetBranchById` (Historia 2) ya exista — ver T023.
- Cada tarea toca 1-3 archivos como máximo, consistente con el Principio IV de la constitución.
