---
description: "Task list template for feature implementation"
---

# Tasks: Employees & Vehicles Module

**Input**: Design documents from `/specs/004-employees-vehicles/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/employees-vehicles-api-contract.md, quickstart.md

**Tests**: Sin tareas de pruebas automatizadas — el proyecto no tiene proyecto de pruebas (ver
constitución); verificación manual vía `quickstart.md`, misma política que `001`, `002` y `003`.

**Organization**: Agrupadas por historia de usuario, con una fase Foundational previa (`Employee` y
`Vehicle` son entidades completamente nuevas que comparten una sola migración y wiring de DI).
`Employee` y `Vehicle` son agregados **independientes entre sí** — solo comparten la referencia a
`Branch` — por lo que sus historias avanzan en dos pistas paralelas: `IEmployeeService`/
`EmployeeService`/`EmployeeController` se crean en la Historia 1 y se amplían en las Historias 2, 5 y
7; `IVehicleService`/`VehicleService`/`VehicleController` se crean en la Historia 3 y se amplían en
las Historias 4, 6 y 8 — mismo patrón de "mismo archivo, tarea distinta" que ya usaron `002` y `003`.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivo distinto, sin dependencia de tareas incompletas)
- **[Story]**: US1 a US8
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

**Purpose**: Persistencia, repositorios y wiring de DI compartidos por las 8 historias — incluida
una única migración que crea `Employees` y `Vehicles` juntas.

- [X] T001 [P] Crear enum `EmployeeRole` en `ShipmentTracker.Core/Enums/EmployeeRole.cs`: valores `Operator`, `Driver`, `WarehouseStaff`, `BranchManager`, con comentario XML en español (mismo estilo que `BranchType.cs`)
- [X] T002 [P] Crear enum `VehicleType` en `ShipmentTracker.Core/Enums/VehicleType.cs`: valores `Motorcycle`, `Van`, `Truck`, con comentario XML
- [X] T003 Crear entidad `Employee` en `ShipmentTracker.Core/Entities/Employee.cs`: `Id` (int), `BranchId` (int), `FirstName`, `LastName`, `Email`, `Phone` (`string?`), `Role` (`EmployeeRole`), `EmployeeNumber`, `HireDate` (`DateOnly`), `IsActive` (bool), `CreatedAt` (`DateTime`), `UpdatedAt` (`DateTime?`), `Branch` (navegación `Branch`, unidireccional — ver data-model.md) (depende de T001; puede hacerse en paralelo con T004)
- [X] T004 Crear entidad `Vehicle` en `ShipmentTracker.Core/Entities/Vehicle.cs`: `Id` (int), `BranchId` (int), `Plate`, `Type` (`VehicleType`), `Brand`, `Model`, `Year` (int), `MaxWeightKg` (decimal), `IsActive` (bool), `CreatedAt` (`DateTime`), `Branch` (navegación, unidireccional) — **sin** `UpdatedAt` (asimetría intencional, research.md Decisión 3) (depende de T002; puede hacerse en paralelo con T003)
- [X] T005 [P] Crear `EmployeeDto` en `ShipmentTracker.Core/DTOs/EmployeeDto.cs`: `Id`, `BranchId`, `FirstName`, `LastName`, `Email`, `Phone?`, `Role` (con `[JsonConverter(typeof(JsonStringEnumConverter))]`, research.md Decisión 11), `EmployeeNumber`, `HireDate`, `IsActive`, `CreatedAt`, `UpdatedAt?` — DTO de salida (depende de T001)
- [X] T006 [P] Crear `VehicleDto` en `ShipmentTracker.Core/DTOs/VehicleDto.cs`: `Id`, `BranchId`, `Plate`, `Type` (con `[JsonConverter(typeof(JsonStringEnumConverter))]`), `Brand`, `Model`, `Year`, `MaxWeightKg`, `IsActive`, `CreatedAt` — DTO de salida (depende de T002)
- [X] T007 [P] Crear `IEmployeeRepository` en `ShipmentTracker.Core/Interfaces/Repositories/IEmployeeRepository.cs`: `: IBaseRepository<Employee>`, sin métodos adicionales (data-model.md) (depende de T003)
- [X] T008 [P] Crear `IVehicleRepository` en `ShipmentTracker.Core/Interfaces/Repositories/IVehicleRepository.cs`: `: IBaseRepository<Vehicle>`, sin métodos adicionales (depende de T004)
- [X] T009 En `ShipmentTracker.Core/Interfaces/IUnitOfWork.cs`: agregar `IEmployeeRepository EmployeeRepository { get; }` y `IVehicleRepository VehicleRepository { get; }` (mismo patrón que `BranchRepository`) (depende de T007, T008)
- [X] T010 [P] Crear `EmployeeConfiguration` en `ShipmentTracker.Infrastructure/Data/Configurations/EmployeeConfiguration.cs`: `ToTable("Employees")`, `HasKey(Id)` con `UseIdentityColumn()`, `FirstName`/`LastName` requeridos (`HasMaxLength(100)`), `Email` requerido (`HasMaxLength(255)`) + `HasIndex(x => x.Email).IsUnique()` (sin filtro por `IsActive`, research.md Decisión 7), `Phone` opcional (`HasMaxLength(30)`), `Role` con `HasConversion<string>()`, `EmployeeNumber` requerido (`HasMaxLength(50)`) + `HasIndex(...).IsUnique()`, `HireDate` requerido, `IsActive`/`CreatedAt` requeridos, `UpdatedAt` opcional, `HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).IsRequired().OnDelete(DeleteBehavior.Restrict)` (depende de T003)
- [X] T011 [P] Crear `VehicleConfiguration` en `ShipmentTracker.Infrastructure/Data/Configurations/VehicleConfiguration.cs`: `ToTable("Vehicles")`, `HasKey(Id)` con `UseIdentityColumn()`, `Plate` requerido (`HasMaxLength(20)`) + `HasIndex(...).IsUnique()` sin filtro, `Type` con `HasConversion<string>()`, `Brand`/`Model` requeridos (`HasMaxLength(100)`), `Year` requerido, `MaxWeightKg` requerido (`HasColumnType("decimal(10,2)")`), `IsActive`/`CreatedAt` requeridos, `HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).IsRequired().OnDelete(DeleteBehavior.Restrict)` (depende de T004)
- [X] T012 En `ShipmentTracker.Infrastructure/Data/AppDbContext.cs`: agregar `DbSet<Employee> Employees` y `DbSet<Vehicle> Vehicles`, y en `OnModelCreating` agregar `builder.ApplyConfiguration(new EmployeeConfiguration())` y `builder.ApplyConfiguration(new VehicleConfiguration())` (depende de T010, T011)
- [X] T013 [P] Crear `EmployeeRepository` en `ShipmentTracker.Infrastructure/Repositories/EmployeeRepository.cs`: `: BaseRepository<Employee>, IEmployeeRepository`, constructor que reenvía a la base (mismo patrón que `BranchRepository`, sin métodos extra) (depende de T007, T012)
- [X] T014 [P] Crear `VehicleRepository` en `ShipmentTracker.Infrastructure/Repositories/VehicleRepository.cs`: `: BaseRepository<Vehicle>, IVehicleRepository` (depende de T008, T012)
- [X] T015 En `ShipmentTracker.Infrastructure/Data/UnitOfWork.cs`: agregar campos privados `_employeeRepository`/`_vehicleRepository` y propiedades lazy `EmployeeRepository`/`VehicleRepository` (mismo patrón que `BranchRepository`) (depende de T009, T013, T014)
- [X] T016 Generar la migración de EF Core: `dotnet ef migrations add AddEmployeesAndVehicles --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web` — crea las tablas `Employees`/`Vehicles`, sus FKs `Restrict` hacia `Branches`, y los índices únicos de `Email`/`EmployeeNumber`/`Plate`; no modifica ninguna migración existente (depende de T012)
- [X] T017 [P] Crear `EmployeeMappingProfile` en `ShipmentTracker.Services/Mappings/EmployeeMappingProfile.cs`: `CreateMap<Employee, EmployeeDto>()` (solo salida — la creación construye la entidad a mano, mismo patrón que `Branch`) (depende de T003, T005)
- [X] T018 [P] Crear `VehicleMappingProfile` en `ShipmentTracker.Services/Mappings/VehicleMappingProfile.cs`: `CreateMap<Vehicle, VehicleDto>()` (depende de T004, T006)
- [X] T019 En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();` y `builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();` (mismo patrón que el registro ya existente de `IBranchRepository`) (depende de T013, T014)

**Checkpoint**: Persistencia, repositorios y `IUnitOfWork.EmployeeRepository`/`VehicleRepository`
listos. Ninguna historia tiene aún servicio, controlador ni endpoint — eso empieza en la Fase 3.

---

## Phase 3: User Story 1 - Registrar un nuevo empleado (Priority: P1) 🎯 MVP

**Goal**: `POST /api/employees` crea un empleado activo con sucursal (activa), rol, número de
empleado y email únicos, y fecha de contratación, rechazando con `400` cualquier dato inválido.

**Independent Test**: `POST /api/employees` con un cuerpo válido devuelve `201` con `isActive:
true`; `POST` con cada tipo de dato inválido por separado (número/email duplicado, sucursal
inactiva o inexistente, rol inválido) devuelve `400` (escenarios 1-4 de `quickstart.md`, User
Story 1).

### Implementation for User Story 1

- [X] T020 [US1] Crear `CreateEmployeeDto` en `ShipmentTracker.Core/DTOs/CreateEmployeeDto.cs`: `BranchId` (int), `FirstName`, `LastName`, `Email`, `Phone?`, `Role` (`EmployeeRole?`), `EmployeeNumber`, `HireDate` (`DateOnly`) — **sin** `IsActive` (siempre `true` al crear) (depende de T001)
- [X] T021 [US1] Crear `CreateEmployeeDtoValidator` en `ShipmentTracker.Services/Validators/CreateEmployeeDtoValidator.cs` (`AbstractValidator<CreateEmployeeDto>`): `FirstName`/`LastName`/`EmployeeNumber` no vacíos; `Email` no vacío + `EmailAddress()`; `Role` no nulo + `IsInEnum()`; `HireDate` distinta de `default(DateOnly)` — solo reglas estructurales, sin acceso a base de datos (research.md Decisión 5) (depende de T020)
- [X] T022 [US1] Crear `IEmployeeService` en `ShipmentTracker.Core/Interfaces/Services/IEmployeeService.cs` con el primer método: `Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);` (depende de T020)
- [X] T023 [US1] Crear `EmployeeService` en `ShipmentTracker.Services/EmployeeService.cs`, `: IEmployeeService`, constructor `(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateEmployeeDto> createValidator)`; implementar método privado `ValidateBusinessRulesAsync(BranchId, Email, EmployeeNumber, currentId)` que verifica sucursal existente+activa y unicidad de `Email`/`EmployeeNumber` contra **todos** los empleados (activos e inactivos, excluyendo `currentId`), devolviendo una lista de `ValidationFailure` (research.md Decisión 6); implementar `CreateEmployeeAsync`: recortar (`Trim()`) `FirstName`/`LastName`/`Email`/`EmployeeNumber`, validar con `createValidator`, combinar con `ValidateBusinessRulesAsync(..., currentId: 0)`, lanzar `FluentValidation.ValidationException` si hay errores; si no, construir `Employee` a mano (`IsActive = true`, `CreatedAt = DateTime.UtcNow`, `UpdatedAt = null`), `AddAsync` + `CommitAsync`, retornar `_mapper.Map<EmployeeDto>` (depende de T021, T022, T015, T017)
- [X] T024 [US1] Crear `EmployeeController` en `ShipmentTracker.Web/Controllers/EmployeeController.cs`, ruta `[Route("api/employees")]`, acción `[HttpPost] CreateEmployee([FromBody] CreateEmployeeDto dto)`: llama al servicio, retorna `Created($"/api/employees/{result.Id}", result)` (sin `CreatedAtAction`/`nameof`, evita depender de `GetEmployeeById` que se agrega en la Historia 2), captura `FluentValidation.ValidationException` → `400` con `{ errors: [{ property, message }] }`; comentarios XML (depende de T023)
- [X] T025 [US1] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IEmployeeService, EmployeeService>();` y `builder.Services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeDtoValidator>();` (depende de T023, T021, T019)

**Checkpoint**: `POST /api/employees` funciona de punta a punta. Historia 1 verificable de forma
independiente.

---

## Phase 4: User Story 2 - Buscar choferes disponibles en una sucursal (Priority: P2)

**Goal**: `GET /api/employees` lista empleados activos de forma paginada, filtrando opcionalmente
por `branchId` y `role` (combinados o por separado), y `GET /api/employees/{id}` recupera un
empleado individual.

**Independent Test**: Con empleados de distintos roles/sucursales ya creados (vía US1), `GET
/api/employees?branchId={id}&role=Driver` devuelve solo choferes activos de esa sucursal; `GET
/api/employees/{id}` incluye todos los detalles; `GET /api/employees/999999` devuelve `404`
(escenarios 1-6 de `quickstart.md`, User Story 2).

### Implementation for User Story 2

- [X] T026 [US2] En `ShipmentTracker.Core/Interfaces/Services/IEmployeeService.cs`: agregar `Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int? branchId = null, EmployeeRole? role = null, int page = 1, int pageSize = 5);` y `Task<EmployeeDto?> GetEmployeeByIdAsync(int id);` (depende de T022 — mismo archivo)
- [X] T027 [US2] En `ShipmentTracker.Services/EmployeeService.cs`: agregar `private const int MaxPageSize = 50;`; implementar `GetEmployeesAsync` (arma `Expression<Func<Employee,bool>>` combinando `x.IsActive == true` con `x.BranchId == branchId.Value` y/o `x.Role == role.Value` según los filtros presentes — igual patrón que `ShipmentService.GetShipmentsAsync`/`BranchService.GetBranchesAsync`; calcula `skip` como `long`, aplica `Math.Min(pageSize, MaxPageSize)`, llama a `_unitOfWork.EmployeeRepository.GetAsync(filter, orderBy: q => q.OrderByDescending(x => x.CreatedAt), skip, take)` + `CountAsync(filter)`, retorna `PagedResult<EmployeeDto>`); implementar `GetEmployeeByIdAsync` (usa `GetByIdAsync(id)`, retorna `null` si no existe, si no `_mapper.Map<EmployeeDto>` — sin filtrar por `IsActive`, la recuperación individual siempre funciona) (depende de T023, T026 — mismo archivo)
- [X] T028 [US2] En `ShipmentTracker.Web/Controllers/EmployeeController.cs`: agregar `[HttpGet] GetEmployees([FromQuery] int? branchId, [FromQuery] EmployeeRole? role, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)` → seteando headers `X-Total-Count`/`X-Page`/`X-Page-Size`/`X-Total-Pages` y `Ok(result.Items)` (mismo patrón que `ShipmentController.GetShipments`), y `[HttpGet("{id}")] GetEmployeeById(int id)` → `200` con `EmployeeDto` o `404` con `{ "message": "No se encontró un empleado con el id '{id}'." }`; comentarios XML (depende de T024, T027 — mismo archivo)

**Checkpoint**: Búsqueda de choferes por sucursal y recuperación individual funcionan. Historias 1
y 2 verificables juntas.

---

## Phase 5: User Story 3 - Registrar un nuevo vehículo (Priority: P3)

**Goal**: `POST /api/vehicles` crea un vehículo activo con sucursal (activa), tipo, placa única,
marca, modelo, año y capacidad de carga, rechazando con `400` cualquier dato inválido.

**Independent Test**: `POST /api/vehicles` con un cuerpo válido devuelve `201` con `isActive:
true`; `POST` con cada tipo de dato inválido por separado (placa duplicada, sucursal inactiva o
inexistente, tipo inválido, año futuro, capacidad `<= 0`) devuelve `400` (escenarios 1-4 de
`quickstart.md`, User Story 3).

### Implementation for User Story 3

- [X] T029 [US3] Crear `CreateVehicleDto` en `ShipmentTracker.Core/DTOs/CreateVehicleDto.cs`: `BranchId` (int), `Plate`, `Type` (`VehicleType?`), `Brand`, `Model`, `Year` (int), `MaxWeightKg` (decimal) — **sin** `IsActive` (depende de T002)
- [X] T030 [US3] Crear `CreateVehicleDtoValidator` en `ShipmentTracker.Services/Validators/CreateVehicleDtoValidator.cs` (`AbstractValidator<CreateVehicleDto>`): `Plate`/`Brand`/`Model` no vacíos; `Type` no nulo + `IsInEnum()`; `Year` `<= DateTime.UtcNow.Year`; `MaxWeightKg` `> 0` — solo reglas estructurales (depende de T029)
- [X] T031 [US3] Crear `IVehicleService` en `ShipmentTracker.Core/Interfaces/Services/IVehicleService.cs` con el primer método: `Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);` (depende de T029)
- [X] T032 [US3] Crear `VehicleService` en `ShipmentTracker.Services/VehicleService.cs`, `: IVehicleService`, constructor `(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateVehicleDto> createValidator)`; implementar método privado `ValidateBusinessRulesAsync(BranchId, Plate, currentId)` (sucursal activa + unicidad de `Plate` contra todos los vehículos, activos e inactivos, excluyendo `currentId`); implementar `CreateVehicleAsync`: recortar `Plate`/`Brand`/`Model`, validar estructural + de negocio, lanzar `ValidationException` si hay errores, si no construir `Vehicle` a mano (`IsActive = true`, `CreatedAt = DateTime.UtcNow`), `AddAsync` + `CommitAsync`, retornar `_mapper.Map<VehicleDto>` (depende de T030, T031, T015, T018)
- [X] T033 [US3] Crear `VehicleController` en `ShipmentTracker.Web/Controllers/VehicleController.cs`, ruta `[Route("api/vehicles")]`, acción `[HttpPost] CreateVehicle([FromBody] CreateVehicleDto dto)`: llama al servicio, retorna `Created($"/api/vehicles/{result.Id}", result)` (sin `nameof`, evita depender de `GetVehicleById` que se agrega en la Historia 4), captura `ValidationException` → `400`; comentarios XML (depende de T032)
- [X] T034 [US3] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IVehicleService, VehicleService>();` y `builder.Services.AddScoped<IValidator<CreateVehicleDto>, CreateVehicleDtoValidator>();` (depende de T032, T030, T019)

**Checkpoint**: `POST /api/vehicles` funciona de punta a punta. Historias 1-3 verificables juntas.

---

## Phase 6: User Story 4 - Ver la flota de una sucursal (Priority: P4)

**Goal**: `GET /api/vehicles` lista vehículos activos de forma paginada, filtrando opcionalmente
por `branchId`, y `GET /api/vehicles/{id}` recupera un vehículo individual.

**Independent Test**: Con vehículos en distintas sucursales ya creados (vía US3), `GET
/api/vehicles?branchId={id}` devuelve solo vehículos activos de esa sucursal; `GET
/api/vehicles/{id}` incluye todos los detalles; `GET /api/vehicles/999999` devuelve `404`
(escenarios 1-5 de `quickstart.md`, User Story 4).

### Implementation for User Story 4

- [X] T035 [US4] En `ShipmentTracker.Core/Interfaces/Services/IVehicleService.cs`: agregar `Task<PagedResult<VehicleDto>> GetVehiclesAsync(int? branchId = null, int page = 1, int pageSize = 5);` y `Task<VehicleDto?> GetVehicleByIdAsync(int id);` (depende de T031 — mismo archivo)
- [X] T036 [US4] En `ShipmentTracker.Services/VehicleService.cs`: agregar `private const int MaxPageSize = 50;`; implementar `GetVehiclesAsync` (filtro `x.IsActive == true` combinado opcionalmente con `x.BranchId == branchId.Value`, paginación igual que `EmployeeService.GetEmployeesAsync`, orden `CreatedAt` descendente); implementar `GetVehicleByIdAsync` (usa `GetByIdAsync(id)`, retorna `null` si no existe) (depende de T032, T035 — mismo archivo)
- [X] T037 [US4] En `ShipmentTracker.Web/Controllers/VehicleController.cs`: agregar `[HttpGet] GetVehicles([FromQuery] int? branchId, [FromQuery, Range(1, int.MaxValue)] int page = 1, [FromQuery, Range(1, int.MaxValue)] int pageSize = 5)` con headers de paginación, y `[HttpGet("{id}")] GetVehicleById(int id)` → `200`/`404`; comentarios XML (depende de T033, T036 — mismo archivo)

**Checkpoint**: Listado de flota por sucursal y recuperación individual funcionan. Historias 1-4
verificables juntas.

---

## Phase 7: User Story 5 - Actualizar información de un empleado (Priority: P5)

**Goal**: `PUT /api/employees/{id}` reemplaza los datos editables de un empleado (incluida la
reasignación de sucursal), re-validando todas las reglas antes de escribir cualquier cambio.

**Independent Test**: `PUT` sobre un empleado existente con datos válidos y no conflictivos
devuelve `200` y refleja los cambios; `PUT` con `employeeNumber`/`email` duplicados o sucursal
inactiva/inexistente devuelve `400` y el empleado queda intacto (escenarios 1-4 de
`quickstart.md`, User Story 5).

### Implementation for User Story 5

- [X] T038 [US5] Crear `UpdateEmployeeDto` en `ShipmentTracker.Core/DTOs/UpdateEmployeeDto.cs`: mismos campos que `CreateEmployeeDto` más `IsActive` (bool) — permite reactivar (research.md Decisión 15) (depende de T001)
- [X] T039 [US5] Crear `UpdateEmployeeDtoValidator` en `ShipmentTracker.Services/Validators/UpdateEmployeeDtoValidator.cs` (`AbstractValidator<UpdateEmployeeDto>`): mismas reglas estructurales que `CreateEmployeeDtoValidator` — sin regla especial para `IsActive` (depende de T038)
- [X] T040 [US5] En `ShipmentTracker.Core/Interfaces/Services/IEmployeeService.cs`: agregar `Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);` (depende de T038, T026 — mismo archivo)
- [X] T041 [US5] En `ShipmentTracker.Services/EmployeeService.cs`: agregar `IValidator<UpdateEmployeeDto> updateValidator` al constructor; implementar `UpdateEmployeeAsync`: cargar con `GetByIdAsync(id)` (`null` → `null`); recortar campos de texto; validar estructural + `ValidateBusinessRulesAsync(..., currentId: id)` **antes** de mutar nada; si hay errores, lanzar `ValidationException` sin tocar la entidad; si no, actualizar todos los campos editables (`FirstName`, `LastName`, `Email`, `Phone`, `Role`, `EmployeeNumber`, `HireDate`, `BranchId`, `IsActive`), `UpdatedAt = DateTime.UtcNow`, `Update()` + `CommitAsync()`, retornar `_mapper.Map<EmployeeDto>` (depende de T027, T039, T040)
- [X] T042 [US5] En `ShipmentTracker.Web/Controllers/EmployeeController.cs`: agregar `[HttpPut("{id}")] UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)` → `200`/`404`/`400` (mismo patrón que `BranchController.UpdateBranch`) (depende de T028, T041 — mismo archivo)
- [X] T043 [US5] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeDtoValidator>();` (depende de T039, T025 — mismo archivo)

**Checkpoint**: Actualización y reasignación de sucursal de empleados funciona. Historias 1-5
verificables juntas.

---

## Phase 8: User Story 6 - Actualizar información de un vehículo (Priority: P6)

**Goal**: `PUT /api/vehicles/{id}` reemplaza los datos editables de un vehículo (incluida la
reasignación de sucursal), re-validando todas las reglas antes de escribir cualquier cambio.

**Independent Test**: `PUT` sobre un vehículo existente con datos válidos devuelve `200` y refleja
los cambios; `PUT` con placa duplicada o sucursal inactiva/inexistente devuelve `400` y el
vehículo queda intacto (escenarios 1-4 de `quickstart.md`, User Story 6).

### Implementation for User Story 6

- [X] T044 [US6] Crear `UpdateVehicleDto` en `ShipmentTracker.Core/DTOs/UpdateVehicleDto.cs`: mismos campos que `CreateVehicleDto` más `IsActive` (depende de T002)
- [X] T045 [US6] Crear `UpdateVehicleDtoValidator` en `ShipmentTracker.Services/Validators/UpdateVehicleDtoValidator.cs` (`AbstractValidator<UpdateVehicleDto>`): mismas reglas estructurales que `CreateVehicleDtoValidator` (depende de T044)
- [X] T046 [US6] En `ShipmentTracker.Core/Interfaces/Services/IVehicleService.cs`: agregar `Task<VehicleDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);` (depende de T044, T035 — mismo archivo)
- [X] T047 [US6] En `ShipmentTracker.Services/VehicleService.cs`: agregar `IValidator<UpdateVehicleDto> updateValidator` al constructor; implementar `UpdateVehicleAsync`: cargar con `GetByIdAsync(id)`; recortar `Plate`/`Brand`/`Model`; validar estructural + `ValidateBusinessRulesAsync(..., currentId: id)` antes de mutar; actualizar todos los campos editables (`Plate`, `Type`, `Brand`, `Model`, `Year`, `MaxWeightKg`, `BranchId`, `IsActive`), `Update()` + `CommitAsync()` (depende de T036, T045, T046)
- [X] T048 [US6] En `ShipmentTracker.Web/Controllers/VehicleController.cs`: agregar `[HttpPut("{id}")] UpdateVehicle(int id, [FromBody] UpdateVehicleDto dto)` → `200`/`404`/`400` (depende de T037, T047 — mismo archivo)
- [X] T049 [US6] En `ShipmentTracker.Web/Program.cs`: agregar `builder.Services.AddScoped<IValidator<UpdateVehicleDto>, UpdateVehicleDtoValidator>();` (depende de T045, T034 — mismo archivo)

**Checkpoint**: Actualización y reasignación de sucursal de vehículos funciona. Historias 1-6
verificables juntas.

---

## Phase 9: User Story 7 - Desactivar un empleado (Priority: P7)

**Goal**: `DELETE /api/employees/{id}` desactiva un empleado (soft-delete, `IsActive = false`) de
forma idempotente; ninguna capa expone un borrado físico.

**Independent Test**: `DELETE` sobre un empleado activo devuelve `204` y deja de aparecer en
cualquier listado (incluida la búsqueda de choferes por sucursal); repetir el mismo `DELETE`
devuelve `204` de nuevo sin error; `DELETE` sobre un id inexistente devuelve `404` (escenarios 1-4
de `quickstart.md`, User Story 7).

### Implementation for User Story 7

- [X] T050 [US7] En `ShipmentTracker.Core/Interfaces/Services/IEmployeeService.cs`: agregar `Task<bool> DeactivateEmployeeAsync(int id);` (depende de T040 — mismo archivo)
- [X] T051 [US7] En `ShipmentTracker.Services/EmployeeService.cs`: implementar `DeactivateEmployeeAsync`: buscar con `GetByIdAsync(id)`, `false` si no existe; si `IsActive == true`, ponerlo en `false` (sin tocar `UpdatedAt` — es una desactivación, no una edición de datos), `Update()` + `CommitAsync()`; si ya estaba inactivo, no escribir nada (idempotente); retornar `true` en ambos casos de éxito (depende de T041, T050)
- [X] T052 [US7] En `ShipmentTracker.Web/Controllers/EmployeeController.cs`: agregar `[HttpDelete("{id}")] DeactivateEmployee(int id)` → `204` si el servicio retorna `true`, `404` si retorna `false`; comentario XML aclarando que es soft-delete (depende de T042, T051 — mismo archivo)

**Checkpoint**: Desactivación de empleados funciona. Historias 1-2, 5, 7 (pista de `Employee`)
completas.

---

## Phase 10: User Story 8 - Desactivar un vehículo (Priority: P8)

**Goal**: `DELETE /api/vehicles/{id}` desactiva un vehículo (soft-delete) de forma idempotente;
ninguna capa expone un borrado físico.

**Independent Test**: `DELETE` sobre un vehículo activo devuelve `204` y deja de aparecer en
cualquier listado; repetir el mismo `DELETE` devuelve `204` de nuevo sin error; `DELETE` sobre un
id inexistente devuelve `404` (escenarios 1-3 de `quickstart.md`, User Story 8).

### Implementation for User Story 8

- [X] T053 [US8] En `ShipmentTracker.Core/Interfaces/Services/IVehicleService.cs`: agregar `Task<bool> DeactivateVehicleAsync(int id);` (depende de T046 — mismo archivo)
- [X] T054 [US8] En `ShipmentTracker.Services/VehicleService.cs`: implementar `DeactivateVehicleAsync`: mismo patrón que `EmployeeService.DeactivateEmployeeAsync` (depende de T047, T053)
- [X] T055 [US8] En `ShipmentTracker.Web/Controllers/VehicleController.cs`: agregar `[HttpDelete("{id}")] DeactivateVehicle(int id)` → `204`/`404` (depende de T048, T054 — mismo archivo)

**Checkpoint**: Las 8 historias funcionan juntas — CRUD completo de Employees & Vehicles.

---

## Phase 11: Polish & Cross-Cutting Concerns

- [X] T056 [P] Compilar la solución (`dotnet build ShipmentTracker.sln`) y confirmar cero errores y cero advertencias nuevas (depende de T001-T055)
- [X] T057 Aplicar la migración (`dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web`) y ejecutar de punta a punta todos los escenarios de `specs/004-employees-vehicles/quickstart.md` (las 8 historias + edge cases) (depende de T056)
- [X] T058 [P] Confirmar que ningún archivo de `Shipment` o `Branch` cambió (`git diff --stat` no debe listar rutas de `Shipment*`/`Branch*`, salvo `IUnitOfWork.cs`/`UnitOfWork.cs`/`Program.cs`, que son compartidos y ya se tocaron en el módulo 003 también) y que los endpoints existentes no cambiaron de comportamiento (depende de T056)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Fase 1)**: N/A.
- **Foundational (Fase 2)**: sin dependencias de historias — BLOQUEA las 8 historias.
- **User Story 1 (Fase 3)**: depende de Foundational. Sin dependencias de otras historias.
- **User Story 2 (Fase 4)**: depende de que T022/T023/T024 (US1) ya existan (mismos 3 archivos).
- **User Story 3 (Fase 5)**: depende de Foundational únicamente — **no** depende de US1/US2 (pista
  de `Vehicle`, completamente independiente de la pista de `Employee`).
- **User Story 4 (Fase 6)**: depende de que T031/T032/T033 (US3) ya existan (mismos 3 archivos).
- **User Story 5 (Fase 7)**: depende de que T026/T027/T028 (US2) ya existan (misma pista de
  `Employee`).
- **User Story 6 (Fase 8)**: depende de que T035/T036/T037 (US4) ya existan (misma pista de
  `Vehicle`) — no depende de US5.
- **User Story 7 (Fase 9)**: depende de que T040/T041/T042 (US5) ya existan.
- **User Story 8 (Fase 10)**: depende de que T046/T047/T048 (US6) ya existan — no depende de US7.
- **Polish (Fase 11)**: depende de que las 8 historias estén completas.

### Dentro de Foundational

- T001, T002 → T003, T004 (T003 solo depende de T001; T004 solo depende de T002 — en paralelo)
- T001 → T005; T002 → T006 (en paralelo entre sí y con T003/T004)
- T003 → T007 → T009 (junto con T008); T004 → T008 → T009
- T003 → T010; T004 → T011 (en paralelo entre sí) → T012
- T007, T012 → T013; T008, T012 → T014 (en paralelo entre sí)
- T009, T013, T014 → T015
- T012 → T016
- T003, T005 → T017; T004, T006 → T018 (en paralelo con T007-T015)
- T013, T014 → T019

### Dentro de cada historia

- **US1**: T001 → T020 → T021; T020 → T022; T021, T022, T015, T017 → T023 → T024; T023, T021, T019 → T025
- **US2**: T022 → T026; T023, T026 → T027; T024, T027 → T028
- **US3**: T002 → T029 → T030; T029 → T031; T030, T031, T015, T018 → T032 → T033; T032, T030, T019 → T034
- **US4**: T031 → T035; T032, T035 → T036; T033, T036 → T037
- **US5**: T001 → T038 → T039; T038, T026 → T040; T027, T039, T040 → T041; T028, T041 → T042; T039, T025 → T043
- **US6**: T002 → T044 → T045; T044, T035 → T046; T036, T045, T046 → T047; T037, T047 → T048; T045, T034 → T049
- **US7**: T040 → T050; T041, T050 → T051; T042, T051 → T052
- **US8**: T046 → T053; T047, T053 → T054; T048, T054 → T055

### Parallel Opportunities

- Al iniciar la Fase 2: **T001 y T002** en paralelo; una vez listos, **T003+T004**, **T005+T006**
  en paralelo entre sí (dos pistas independientes, `Employee` vs `Vehicle`). **T010+T011** en
  paralelo una vez existen T003/T004. **T013+T014** en paralelo una vez existe T012. **T017+T018**
  pueden hacerse en paralelo con toda la cadena T007-T015.
- **US3 (Vehicle) puede empezar en paralelo con US1/US2 (Employee)** apenas termina Foundational —
  son pistas completamente independientes que no comparten ningún archivo de historia. Un equipo de
  dos desarrolladores podría avanzar `Employee` (US1→US2→US5→US7) y `Vehicle` (US3→US4→US6→US8) en
  simultáneo.
- Dentro de cada historia, las tareas de DTO/validador nuevas (p. ej. T020+T021 en US1) pueden
  hacerse en paralelo con la interfaz de servicio (T022) antes de tocar los archivos compartidos
  (`EmployeeService.cs`, `EmployeeController.cs`).

---

## Parallel Example: Foundational

```bash
# Al inicio de la Fase 2, en paralelo:
Task: "Crear enum EmployeeRole en ShipmentTracker.Core/Enums/EmployeeRole.cs"
Task: "Crear enum VehicleType en ShipmentTracker.Core/Enums/VehicleType.cs"

# Tras completar ambos enums, en paralelo (dos pistas independientes):
Task: "Crear entidad Employee en ShipmentTracker.Core/Entities/Employee.cs"
Task: "Crear entidad Vehicle en ShipmentTracker.Core/Entities/Vehicle.cs"
Task: "Crear EmployeeDto en ShipmentTracker.Core/DTOs/EmployeeDto.cs"
Task: "Crear VehicleDto en ShipmentTracker.Core/DTOs/VehicleDto.cs"
```

## Parallel Example: Employee vs Vehicle tracks

```bash
# Una vez completada la Fase 2 (Foundational), en paralelo:
Task: "Ejecutar Fase 3 (US1) -> Fase 4 (US2) -> Fase 7 (US5) -> Fase 9 (US7): pista Employee"
Task: "Ejecutar Fase 5 (US3) -> Fase 6 (US4) -> Fase 8 (US6) -> Fase 10 (US8): pista Vehicle"
```

---

## Implementation Strategy

### MVP First (User Story 1 solamente)

1. Completar Fase 2: Foundational (T001-T019).
2. Completar Fase 3: User Story 1 (T020-T025).
3. Ejecutar los escenarios 1-4 de User Story 1 en `quickstart.md`.
4. `POST /api/employees` ya es un incremento entregable — permite empezar a registrar personal,
   aunque todavía no se pueda buscar/editar/desactivar por API ni exista gestión de vehículos.

### Incremental Delivery

1. Foundational → persistencia lista para ambas entidades, sin endpoints todavía.
2. US1 → validar → alta de empleados funcionando (MVP).
3. US2 → validar → búsqueda de choferes por sucursal disponible (el objetivo principal del módulo,
   según spec.md).
4. US3 → validar → alta de vehículos funcionando (pista independiente, puede hacerse en paralelo
   con US1/US2).
5. US4 → validar → visibilidad de flota por sucursal disponible.
6. US5 → validar → edición y reasignación de empleados disponible.
7. US6 → validar → edición y reasignación de vehículos disponible.
8. US7 → validar → desactivación de empleados disponible.
9. US8 → validar → desactivación de vehículos disponible; CRUD completo de ambas entidades.
10. Polish → build limpio + validación manual completa + confirmación de que `Shipment`/`Branch`
    no cambiaron.

### Parallel Team Strategy

A diferencia del módulo 003 (una sola entidad), este módulo tiene **dos pistas verdaderamente
independientes** (`Employee`: US1→US2→US5→US7; `Vehicle`: US3→US4→US6→US8) que no comparten ningún
archivo de historia — solo comparten la Fase 2 (Foundational) y los archivos compartidos
`Program.cs`/`IUnitOfWork.cs`/`UnitOfWork.cs` (ya resueltos en Foundational). Dos desarrolladores
pueden avanzar cada pista en paralelo sin coordinación adicional una vez completada la Fase 2.

---

## Notes

- No hay tareas de test automatizado — el proyecto no tiene proyecto de pruebas; la validación es
  manual vía `quickstart.md`.
- `Employee` y `Vehicle` no tienen relación directa entre sí — un vehículo se asigna a una
  sucursal, nunca a un empleado (spec.md, Edge Cases). No generar tareas para modelar esa relación.
- `Branch.cs` (módulo 003) no se modifica — la FK es unidireccional, sin colección inversa
  (research.md Decisión 14). No generar una tarea para agregarla.
- El `POST` de las Historias 1 y 3 usa `Created(uri, result)` en vez de
  `CreatedAtAction(nameof(...))` para no depender de las acciones `GetEmployeeById`/
  `GetVehicleById`, que se agregan recién en las Historias 2 y 4 — ver T024, T033.
- Cada tarea toca 1-3 archivos como máximo, consistente con el Principio IV de la constitución.
