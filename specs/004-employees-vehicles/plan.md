# Implementation Plan: Employees & Vehicles Module

**Branch**: `004-employees-vehicles` | **Date**: 2026-08-17 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/004-employees-vehicles/spec.md`

## Summary

Módulo nuevo y autocontenido (sin tocar código de `Shipment` ni de `Branch`) que agrega gestión de
personal (`Employee`) y flota (`Vehicle`) a las cuatro capas ya existentes, ambos referenciando a
`Branch` (módulo 003) por FK sin modificarlo. Enfoque técnico: dos entidades independientes entre sí
(`Employee` y `Vehicle` — sin relación directa entre ambas, solo comparten la referencia a `Branch`),
persistidas vía EF Core/SQL Server, expuestas mediante `IEmployeeRepository`/`IVehicleRepository` +
`IUnitOfWork` (mismo patrón que `Shipment`/`Branch`), servicios que aplican las reglas de negocio
dependientes de base de datos (sucursal activa, unicidad global de email/número de empleado/placa,
incluso contra registros inactivos) antes de tocar la base, validados estructuralmente con
FluentValidation, mapeados a DTOs con AutoMapper, y expuestos en `EmployeeController`/
`VehicleController` con las diez rutas pedidas por el usuario. Los listados (`GET` plural) reutilizan
la paginación ya existente de `002-paginate-shipment-list` (`PagedResult<T>`, headers
`X-Total-*`, tope de 50), igual que `ShipmentController`.

## Technical Context

**Language/Version**: C# sobre .NET 8.0 (`net8.0`, sin cambios respecto al resto de la solución)

**Primary Dependencies**: ASP.NET Core 8, Entity Framework Core 8 + SQL Server (ya referenciado),
AutoMapper (ya registrado), FluentValidation (ya registrado) — **cero paquetes NuGet nuevos**
(Principio III).

**Storage**: SQL Server vía EF Core, mismo `AppDbContext`. Dos tablas nuevas: `Employees` y
`Vehicles`, cada una con FK requerida (`BranchId`) hacia `Branches` (`OnDelete(Restrict)`, ya que
`Branch` nunca se borra físicamente) e índices únicos (`Employees.Email`, `Employees.EmployeeNumber`,
`Vehicles.Plate`) sin filtro por `IsActive` — la unicidad aplica siempre, incluso contra registros
inactivos (confirmado en Clarifications). Requiere una migración nueva (se genera en la fase de
implementación).

**Testing**: Manual vía Swagger/HTTP (ver `quickstart.md`), misma política que el resto del proyecto
— no existe proyecto de pruebas automatizadas.

**Target Platform**: ASP.NET Core Web API (sin cambios de hosting; mismo CORS `AllowReactApp`, que ya
expone los headers `X-Total-*` desde `002`)

**Project Type**: Web service — misma solución en capas existente, módulo nuevo aditivo

**Performance Goals**: N/A — no se define una meta de latencia; volumen esperado (personal y flota de
una red logística) es bajo, cubierto sin problema por la paginación ya existente.

**Constraints**: Reutiliza los patrones ya adoptados — `IBaseRepository<T>` + `IUnitOfWork`,
AutoMapper solo para la salida, FluentValidation invocado manualmente para reglas **estructurales**
(campos requeridos, formato, enums válidos), mientras que las reglas **dependientes de base de datos**
(sucursal activa, unicidad global) se resuelven en el `Service` (primer caso en esta solución donde
un módulo necesita ambas clases de validación combinadas — ver research.md, Decisión 5). Rutas HTTP
exactas fijadas por el usuario. `BranchId` corregido a `int` para `Vehicle` (el input decía `guid`,
pero `Branch.Id` es `int` en el código real — ver research.md, Decisión 4).

**Scale/Scope**: Módulo nuevo, aditivo en las 4 capas: `Core` (2 entidades, 2 enums, 6 DTOs, 2
interfaces de repositorio + 2 de servicio + 2 propiedades nuevas en `IUnitOfWork`), `Infrastructure`
(2 repositorios, 2 configuraciones EF, 1 migración, `AppDbContext`/`UnitOfWork` ampliados),
`Services` (2 servicios, 4 validadores, 2 perfiles de mapeo), `Web` (2 controladores, registros de DI
en `Program.cs`). Ningún archivo de `Shipment` ni de `Branch` se modifica.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principio | Evaluación | Resultado |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | Ningún cambio de `TargetFramework`; todo lo usado ya está referenciado. | PASS |
| II. Integridad de la Arquitectura en Capas | `Core` sin dependencias externas. `Infrastructure`/`Services` dependen únicamente de `Core`. `Web` es el único que depende de los tres. `Employee`/`Vehicle` referencian `Branch` únicamente vía FK (`BranchId` + `_unitOfWork.BranchRepository`, ya expuesto por `IUnitOfWork` desde el módulo 003) — no se agrega una colección inversa en `Branch.cs` ni se modifica ese módulo. Se extiende FluentValidation a un caso de uso nuevo (validación dependiente de base de datos en el `Service`) sin introducir un segundo mecanismo de validación que compita con el ya adoptado — ver research.md Decisión 5. | PASS |
| III. Minimalismo de Dependencias | Cero paquetes NuGet nuevos. | PASS |
| IV. Cambios Pequeños y Reversibles | Cambio aditivo y autocontenido: ningún archivo de `Shipment` o `Branch` se toca. Las únicas modificaciones a archivos existentes (`IUnitOfWork`/`UnitOfWork.cs` ganan 2 propiedades; `Program.cs` gana registros de DI) son adiciones puntuales, análogas a como ya se hizo en el módulo 003. | PASS |

No hay violaciones que registrar en Complexity Tracking.

**Re-check post Phase 1**: tras diseñar `data-model.md`, `contracts/` y `quickstart.md`, la tabla se
sostiene sin cambios.

## Project Structure

### Documentation (this feature)

```text
specs/004-employees-vehicles/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── employees-vehicles-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Entities/
│   ├── Employee.cs                             # [NUEVO] Id, BranchId, FirstName, LastName, Email,
│   │                                            #   Phone?, Role, EmployeeNumber, HireDate (DateOnly),
│   │                                            #   IsActive, CreatedAt, UpdatedAt (DateTime?), Branch (nav)
│   └── Vehicle.cs                               # [NUEVO] Id, BranchId, Plate, Type, Brand, Model,
│                                                 #   Year, MaxWeightKg (decimal), IsActive, CreatedAt, Branch (nav)
├── Enums/
│   ├── EmployeeRole.cs                          # [NUEVO] Operator, Driver, WarehouseStaff, BranchManager
│   └── VehicleType.cs                           # [NUEVO] Motorcycle, Van, Truck
├── DTOs/
│   ├── EmployeeDto.cs                           # [NUEVO] salida
│   ├── CreateEmployeeDto.cs                     # [NUEVO] alta
│   ├── UpdateEmployeeDto.cs                     # [NUEVO] reemplazo (PUT)
│   ├── VehicleDto.cs                            # [NUEVO] salida
│   ├── CreateVehicleDto.cs                      # [NUEVO] alta
│   └── UpdateVehicleDto.cs                      # [NUEVO] reemplazo (PUT)
└── Interfaces/
    ├── IUnitOfWork.cs                           # [MODIFICAR] + IEmployeeRepository, IVehicleRepository
    ├── Repositories/
    │   ├── IEmployeeRepository.cs               # [NUEVO] : IBaseRepository<Employee> (sin métodos extra)
    │   └── IVehicleRepository.cs                # [NUEVO] : IBaseRepository<Vehicle> (sin métodos extra)
    └── Services/
        ├── IEmployeeService.cs                  # [NUEVO]
        └── IVehicleService.cs                   # [NUEVO]

ShipmentTracker.Infrastructure/
├── Data/
│   ├── AppDbContext.cs                          # [MODIFICAR] + DbSet<Employee>, DbSet<Vehicle>,
│   │                                             #   ApplyConfiguration de ambas
│   ├── Configurations/
│   │   ├── EmployeeConfiguration.cs             # [NUEVO] índices únicos Email/EmployeeNumber, FK Restrict
│   │   └── VehicleConfiguration.cs              # [NUEVO] índice único Plate, FK Restrict
│   └── UnitOfWork.cs                            # [MODIFICAR] + propiedades lazy EmployeeRepository/VehicleRepository
├── Migrations/
│   └── <timestamp>_AddEmployeesAndVehicles.cs   # [NUEVO] generada en fase de implementación
└── Repositories/
    ├── EmployeeRepository.cs                    # [NUEVO] : BaseRepository<Employee>, IEmployeeRepository
    └── VehicleRepository.cs                     # [NUEVO] : BaseRepository<Vehicle>, IVehicleRepository

ShipmentTracker.Services/
├── EmployeeService.cs                           # [NUEVO]
├── VehicleService.cs                            # [NUEVO]
├── Mappings/
│   ├── EmployeeMappingProfile.cs                # [NUEVO] Employee→EmployeeDto
│   └── VehicleMappingProfile.cs                 # [NUEVO] Vehicle→VehicleDto
└── Validators/
    ├── CreateEmployeeDtoValidator.cs             # [NUEVO] reglas estructurales
    ├── UpdateEmployeeDtoValidator.cs             # [NUEVO] mismas reglas que Create
    ├── CreateVehicleDtoValidator.cs               # [NUEVO] reglas estructurales
    └── UpdateVehicleDtoValidator.cs               # [NUEVO] mismas reglas que Create

ShipmentTracker.Web/
├── Program.cs                                   # [MODIFICAR] + registros DI (2 repos, 2 servicios, 4 validadores)
└── Controllers/
    ├── EmployeeController.cs                    # [NUEVO] POST/GET(paginado)/GET{id}/PUT/DELETE /api/employees
    └── VehicleController.cs                     # [NUEVO] POST/GET(paginado)/GET{id}/PUT/DELETE /api/vehicles
```

**Structure Decision**: Misma arquitectura en capas existente, sin proyectos nuevos. `Employee` y
`Vehicle` son dos agregados independientes (no hay relación directa entre ellos, per spec.md) que
solo comparten la referencia a `Branch` — se modelan como dos módulos paralelos completos dentro del
mismo feature, cada uno con su propio repositorio/servicio/controlador, siguiendo exactamente el
patrón por capa ya usado por `Shipment` y `Branch`.

## Complexity Tracking

*Sin violaciones que justificar — tabla omitida intencionalmente.*
