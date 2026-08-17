# Data Model: Employees & Vehicles Module

## Entidad: `Employee`

`ShipmentTracker.Core/Entities/Employee.cs`. Referencia a `Branch` (módulo 003) vía FK; sin relación
directa con `Vehicle`.

| Campo | Tipo | Reglas | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `BranchId` | `int` (FK, requerido) | Debe referenciar una sucursal existente **y activa** | FR-003 |
| `FirstName` | `string` | Requerido, no vacío | FR-001 |
| `LastName` | `string` | Requerido, no vacío | FR-001 |
| `Email` | `string` | Requerido, formato de email válido, único a nivel compañía (activos e inactivos) | FR-001, FR-002 |
| `Phone` | `string?` | Opcional (agregado en planificación, ver research.md Decisión 2) | — |
| `Role` | `EmployeeRole` (enum) | Requerido, uno de los 4 valores definidos | FR-001 |
| `EmployeeNumber` | `string` | Requerido, no vacío, único a nivel compañía (activos e inactivos) | FR-001, FR-002 |
| `HireDate` | `DateOnly` | Requerido (`!= default`) | FR-001 |
| `IsActive` | `bool` | `true` por defecto al crear; nunca se borra el registro | FR-004, FR-008 |
| `CreatedAt` | `DateTime` (UTC) | Asignado por el servicio al crear | — |
| `UpdatedAt` | `DateTime?` (UTC) | `null` hasta la primera actualización; se reasigna en cada `PUT` exitoso | — |
| `Branch` | `Branch` (nav, unidireccional) | Sin colección inversa en `Branch.cs` (research.md Decisión 14) | — |

## Entidad: `Vehicle`

`ShipmentTracker.Core/Entities/Vehicle.cs`. Referencia a `Branch` vía FK; sin relación directa con
`Employee` — un vehículo se asigna a una sucursal, nunca a un empleado específico (spec.md, Edge
Cases).

| Campo | Tipo | Reglas | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `BranchId` | `int` (FK, requerido) | Debe referenciar una sucursal existente **y activa** | FR-013 |
| `Plate` | `string` | Requerido, no vacío, único a nivel compañía (activos e inactivos) | FR-011, FR-012 |
| `Type` | `VehicleType` (enum) | Requerido, uno de los 3 valores definidos | FR-011 |
| `Brand` | `string` | Requerido, no vacío | FR-011 |
| `Model` | `string` | Requerido, no vacío | FR-011 |
| `Year` | `int` | Requerido, no puede ser un año futuro | FR-011, FR-021 |
| `MaxWeightKg` | `decimal` | Requerido, debe ser positivo (> 0) | FR-011, FR-022 |
| `IsActive` | `bool` | `true` por defecto al crear; nunca se borra el registro | FR-014, FR-018 |
| `CreatedAt` | `DateTime` (UTC) | Asignado por el servicio al crear | — |
| `Branch` | `Branch` (nav, unidireccional) | Sin colección inversa en `Branch.cs` | — |

*Nota*: `Vehicle` no tiene `UpdatedAt` — asimetría intencional respecto a `Employee`, ver
research.md Decisión 3.

## Enums

`ShipmentTracker.Core/Enums/EmployeeRole.cs`:

```
Operator, Driver, WarehouseStaff, BranchManager
```

`ShipmentTracker.Core/Enums/VehicleType.cs`:

```
Motorcycle, Van, Truck
```

Ambos se persisten como `string` (`HasConversion<string>()`), mismo patrón que `ShipmentStatus`/
`BranchType`.

## DTOs

`ShipmentTracker.Core/DTOs/`:

| DTO | Uso | Campos |
|---|---|---|
| `EmployeeDto` | Salida (`POST`, `GET`, `GET/{id}`, `PUT`) | `Id`, `BranchId`, `FirstName`, `LastName`, `Email`, `Phone?`, `Role`, `EmployeeNumber`, `HireDate`, `IsActive`, `CreatedAt`, `UpdatedAt?` |
| `CreateEmployeeDto` | Entrada de `POST /api/employees` | `BranchId`, `FirstName`, `LastName`, `Email`, `Phone?`, `Role?`, `EmployeeNumber`, `HireDate` — **sin** `IsActive` (siempre `true` al crear) |
| `UpdateEmployeeDto` | Entrada de `PUT /api/employees/{id}` | Igual que `CreateEmployeeDto` + `IsActive` (permite reactivar) |
| `VehicleDto` | Salida | `Id`, `BranchId`, `Plate`, `Type`, `Brand`, `Model`, `Year`, `MaxWeightKg`, `IsActive`, `CreatedAt` |
| `CreateVehicleDto` | Entrada de `POST /api/vehicles` | `BranchId`, `Plate`, `Type?`, `Brand`, `Model`, `Year`, `MaxWeightKg` |
| `UpdateVehicleDto` | Entrada de `PUT /api/vehicles/{id}` | Igual que `CreateVehicleDto` + `IsActive` |

`Role`/`Type` son *nullable* (`EmployeeRole?`, `VehicleType?`) en los DTOs de entrada para distinguir
"omitido" de un valor explícito (research.md Decisión 9). `BranchId` es `int` simple, no nullable
(research.md Decisión 10).

## Reglas de validación

### Estructurales (FluentValidation, `ShipmentTracker.Services/Validators/`)

| Validador | Regla | FR |
|---|---|---|
| `CreateEmployeeDtoValidator` / `UpdateEmployeeDtoValidator` | `FirstName`, `LastName`, `EmployeeNumber`: no vacíos | FR-001 |
| | `Email`: no vacío, formato de email válido | FR-001 |
| | `Role`: no nulo, `IsInEnum()` | FR-001 |
| | `HireDate`: `!= default(DateOnly)` | FR-001 |
| `CreateVehicleDtoValidator` / `UpdateVehicleDtoValidator` | `Plate`, `Brand`, `Model`: no vacíos | FR-011 |
| | `Type`: no nulo, `IsInEnum()` | FR-011 |
| | `Year`: `<= DateTime.UtcNow.Year` | FR-021 |
| | `MaxWeightKg`: `> 0` | FR-022 |

### Dependientes de base de datos (`EmployeeService`/`VehicleService`, research.md Decisión 5)

| Regla | Detalle | FR |
|---|---|---|
| Sucursal existente y activa | `_unitOfWork.BranchRepository.SingleOrDefaultAsync(x => x.Id == dto.BranchId)`; error si `null` o `!IsActive` | FR-003, FR-013 |
| Email único (Employee) | Comparado contra **todos** los empleados (activos e inactivos), excluyendo el propio id en `Update` | FR-002 |
| Número de empleado único | Igual que Email | FR-002 |
| Placa única (Vehicle) | Igual que Email, para `Vehicle.Plate` | FR-012 |

Todos los errores (estructurales + de negocio) se acumulan y se reportan juntos en un único
`FluentValidation.ValidationException` si hay al menos uno, **antes** de escribir cualquier cambio
(consistente con el "no escritura parcial" ya establecido en `Branch`).

## Interfaces nuevas

`ShipmentTracker.Core/Interfaces/`:

```csharp
public interface IEmployeeRepository : IBaseRepository<Employee> { } // sin métodos extra

public interface IVehicleRepository : IBaseRepository<Vehicle> { } // sin métodos extra

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(int? branchId = null, EmployeeRole? role = null, int page = 1, int pageSize = 5);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);           // null → 404
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto); // lanza ValidationException si es inválido
    Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto); // null → 404; lanza ValidationException si es inválido
    Task<bool> DeactivateEmployeeAsync(int id);                 // false → 404; idempotente
}

public interface IVehicleService
{
    Task<PagedResult<VehicleDto>> GetVehiclesAsync(int? branchId = null, int page = 1, int pageSize = 5);
    Task<VehicleDto?> GetVehicleByIdAsync(int id);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);
    Task<VehicleDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto);
    Task<bool> DeactivateVehicleAsync(int id);
}
```

`IUnitOfWork` gana `IEmployeeRepository EmployeeRepository { get; }` y
`IVehicleRepository VehicleRepository { get; }` (mismo patrón lazy que `ShipmentRepository`/
`BranchRepository`).

No se agrega `IBranchRepository.GetEmployeesAsync` ni nada similar en `Branch` — `Employee`/`Vehicle`
consultan `Branch` directamente vía `IUnitOfWork.BranchRepository`, sin tocar el módulo 003.

## Flujo de `CreateEmployeeAsync` / `UpdateEmployeeAsync` (y análogo para `Vehicle`)

1. Recortar (`.Trim()`) `Email`, `EmployeeNumber`, `FirstName`, `LastName` (o `Plate`, `Brand`,
   `Model` para `Vehicle`) del DTO recibido.
2. Validar estructuralmente con `IValidator<TDto>`; acumular errores.
3. Ejecutar `ValidateBusinessRulesAsync(dto, currentId)` (research.md Decisión 6): comprobar
   sucursal activa + unicidad global (excluyendo `currentId` — `0` en `Create`, el id real en
   `Update`); acumular errores.
4. Si hay algún error acumulado, lanzar `FluentValidation.ValidationException(errores)` sin tocar la
   entidad.
5. **Create**: construir la entidad a mano (`IsActive = true`, `CreatedAt = DateTime.UtcNow`,
   `UpdatedAt = null` si aplica), `AddAsync` + `CommitAsync`.
   **Update**: cargar la entidad existente (`GetByIdAsync`/`SingleOrDefaultAsync`; `null` → 404),
   actualizar todos los campos editables, `UpdatedAt = DateTime.UtcNow` si aplica, `Update()` +
   `CommitAsync`.
6. Retornar `_mapper.Map<TDto>(entidad)`.

## Migración

Se requiere una migración EF Core nueva (`AddEmployeesAndVehicles`, fase de implementación) que crea
las tablas `Employees` y `Vehicles`, sus FKs `Restrict` hacia `Branches`, y los índices únicos
(`Employees.Email`, `Employees.EmployeeNumber`, `Vehicles.Plate`). No modifica ninguna tabla
existente (`Shipments`, `Branches`, `BranchSchedules` no cambian).
