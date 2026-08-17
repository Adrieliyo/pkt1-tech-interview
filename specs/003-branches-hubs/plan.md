# Implementation Plan: Branches & Hubs Module

**Branch**: `003-branches-hubs` | **Date**: 2026-08-17 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/003-branches-hubs/spec.md`

## Summary

Módulo nuevo y autocontenido (sin tocar código de `Shipment`) que agrega gestión de sucursales
(`Branch`) y su horario semanal (`BranchSchedule`) a las cuatro capas ya existentes. Enfoque
técnico: dos entidades nuevas (`Branch` 1—N `BranchSchedule`) persistidas vía EF Core/SQL Server,
expuestas mediante `IBranchRepository`/`IUnitOfWork.BranchRepository` (mismo patrón que
`Shipment`), un `BranchService` que construye/reemplaza el horario completo en cada creación o
actualización (PUT reemplaza siempre las 7 entradas), validado con FluentValidation antes de
tocar la base de datos, mapeado a DTOs con AutoMapper, y expuesto en `BranchController` con las
cinco rutas pedidas por el usuario (`POST/GET/GET{id}/PUT/DELETE` bajo `/api/branches`), donde
`DELETE` es exclusivamente el soft-delete (`IsActive = false`) — nunca hay una ruta de borrado
físico en ninguna capa.

## Technical Context

**Language/Version**: C# sobre .NET 8.0 (`net8.0`, sin cambios respecto al resto de la solución)

**Primary Dependencies**: ASP.NET Core 8, Entity Framework Core 8 + SQL Server (ya referenciado en
`Infrastructure`), AutoMapper (ya registrado), FluentValidation (ya registrado, mismo patrón que
`ShipmentTransitionValidator`), Swashbuckle/Swagger (XML docs ya habilitados) — **cero paquetes
NuGet nuevos** (Principio III).

**Storage**: SQL Server vía EF Core, mismo `AppDbContext`. Dos tablas nuevas: `Branches` y
`BranchSchedules` (FK `BranchId`, índice único compuesto `(BranchId, DayOfWeek)` como defensa en
profundidad de FR-005). Requiere una migración nueva (se genera en la fase de implementación, no
en esta planificación).

**Testing**: Manual vía Swagger/HTTP (ver `quickstart.md`), consistente con la política ya
establecida del proyecto — no existe proyecto de pruebas automatizadas (constitución, Flujo de
Trabajo de Desarrollo).

**Target Platform**: ASP.NET Core Web API (sin cambios de hosting; mismo CORS `AllowReactApp`)

**Project Type**: Web service — misma solución en capas existente, módulo nuevo aditivo

**Performance Goals**: N/A — no se define una meta de latencia en `spec.md`; volumen esperado
(sucursales de una red logística) es bajo (decenas a cientos de filas), sin necesidad de
paginación ni índices adicionales más allá del único de `(BranchId, DayOfWeek)`.

**Constraints**: Debe reutilizar los patrones ya adoptados — `IBaseRepository<T>` +
`IUnitOfWork`, AutoMapper para la salida (`Entity → Dto`), FluentValidation invocado manualmente
desde el `Service` (mismo estilo que `ShipmentTransitionValidator`), construcción manual de la
entidad en la creación (mismo estilo que `ShipmentService.CreateShipmentAsync`, no
`_mapper.Map` para el alta) — sin introducir un segundo patrón que compita con estos (Principio
II). Rutas HTTP exactas fijadas por el usuario: `POST/GET/GET{id}/PUT/DELETE /api/branches`.

**Scale/Scope**: Módulo nuevo, aditivo en las 4 capas: `Core` (2 entidades, 2 enums, 5 DTOs, 2
interfaces nuevas + 1 propiedad nueva en `IUnitOfWork`), `Infrastructure` (1 repositorio, 2
configuraciones EF, 1 migración, `AppDbContext`/`UnitOfWork` ampliados), `Services` (1 servicio, 3
validadores, 1 perfil de mapeo), `Web` (1 controlador, registros de DI en `Program.cs`). Ningún
archivo de `Shipment` se modifica.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principio | Evaluación | Resultado |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | Ningún cambio de `TargetFramework`; todo lo usado (EF Core, AutoMapper, FluentValidation) ya está referenciado en los proyectos correspondientes. | PASS |
| II. Integridad de la Arquitectura en Capas | `Core` no depende de ningún otro proyecto (entidades/DTOs/enums/interfaces puros). `Infrastructure` y `Services` dependen únicamente de `Core`. `Web` es el único que depende de los tres. Ninguna flecha se invierte. Se reutilizan los patrones ya adoptados (Repository + Unit of Work, AutoMapper, FluentValidation, inyección por constructor) sin introducir uno alternativo que compita para el mismo propósito — en particular, la creación construye la entidad a mano (igual que `ShipmentService.CreateShipmentAsync`) en vez de mezclar `_mapper.Map` con lógica manual. | PASS |
| III. Minimalismo de Dependencias | Cero paquetes NuGet nuevos — EF Core, AutoMapper, FluentValidation y Swashbuckle ya cubren persistencia, mapeo, validación y documentación de API para este módulo (ver research.md). | PASS |
| IV. Cambios Pequeños y Reversibles | Cambio aditivo y autocontenido: ningún archivo de `Shipment` se toca. Las dos únicas modificaciones a archivos existentes (`IUnitOfWork`/`UnitOfWork.cs` ganan una propiedad `BranchRepository`; `Program.cs` gana registros de DI) son adiciones puntuales, análogas a como `ShipmentRepository` ya está registrado — no hay refactor de código no relacionado. Tocar las 4 capas es el tamaño mínimo necesario para un módulo de dominio nuevo completo (entidad → persistencia → regla de negocio → HTTP), no una fuga de responsabilidades. | PASS |

No hay violaciones que registrar en Complexity Tracking.

**Re-check post Phase 1**: tras diseñar `data-model.md`, `contracts/` y `quickstart.md`, la tabla
se sostiene sin cambios — ningún hallazgo de diseño introdujo una dependencia nueva, invirtió una
flecha de dependencia, ni infló el alcance más allá de lo descrito arriba.

## Project Structure

### Documentation (this feature)

```text
specs/003-branches-hubs/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── branches-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── Entities/
│   ├── Branch.cs                               # [NUEVO] Id, Name, Type, Address, City, State,
│   │                                            #   ZipCode, Latitude?, Longitude?, Phone?,
│   │                                            #   IsActive, CreatedAt, Schedule (ICollection)
│   └── BranchSchedule.cs                       # [NUEVO] Id, BranchId FK, DayOfWeek, OpensAt?,
│                                                #   ClosesAt?, IsClosed, Branch (nav)
├── Enums/
│   ├── BranchType.cs                           # [NUEVO] Headquarters, Hub, SalesPoint, PickupPoint
│   └── ScheduleDay.cs                          # [NUEVO] Monday..Sunday
├── DTOs/
│   ├── BranchDto.cs                            # [NUEVO] salida, incluye Schedule
│   ├── CreateBranchDto.cs                      # [NUEVO] alta
│   ├── UpdateBranchDto.cs                      # [NUEVO] reemplazo completo (PUT)
│   ├── ScheduleEntryDto.cs                     # [NUEVO] salida de una entrada de horario
│   └── ScheduleEntryInputDto.cs                # [NUEVO] entrada para Create/Update
└── Interfaces/
    ├── IUnitOfWork.cs                          # [MODIFICAR] + IBranchRepository BranchRepository
    ├── Repositories/
    │   └── IBranchRepository.cs                # [NUEVO] : IBaseRepository<Branch> +
    │                                            #   GetByIdWithScheduleAsync(int)
    └── Services/
        └── IBranchService.cs                   # [NUEVO]

ShipmentTracker.Infrastructure/
├── Data/
│   ├── AppDbContext.cs                         # [MODIFICAR] + DbSet<Branch>, DbSet<BranchSchedule>,
│   │                                            #   ApplyConfiguration de ambas
│   ├── Configurations/
│   │   ├── BranchConfiguration.cs              # [NUEVO]
│   │   └── BranchScheduleConfiguration.cs      # [NUEVO] FK requerida + índice único (BranchId, DayOfWeek)
│   └── UnitOfWork.cs                           # [MODIFICAR] + propiedad lazy BranchRepository
├── Migrations/
│   └── <timestamp>_AddBranchesAndSchedule.cs   # [NUEVO] generada en fase de implementación
└── Repositories/
    └── BranchRepository.cs                     # [NUEVO] : BaseRepository<Branch>, IBranchRepository

ShipmentTracker.Services/
├── BranchService.cs                            # [NUEVO]
├── Mappings/
│   └── BranchMappingProfile.cs                 # [NUEVO] Branch→BranchDto, BranchSchedule→ScheduleEntryDto
└── Validators/
    ├── ScheduleEntryInputDtoValidator.cs       # [NUEVO] reglas por día (FR-006, FR-007, FR-017)
    ├── CreateBranchDtoValidator.cs              # [NUEVO] dirección + tipo + 7 días sin duplicados (FR-001..FR-005)
    └── UpdateBranchDtoValidator.cs              # [NUEVO] mismas reglas que Create

ShipmentTracker.Web/
├── Program.cs                                  # [MODIFICAR] + registros DI (repositorio, servicio, 2 validadores)
└── Controllers/
    └── BranchController.cs                     # [NUEVO] POST/GET/GET{id}/PUT/DELETE /api/branches
```

**Structure Decision**: Misma arquitectura en capas existente, sin proyectos nuevos. `Branch` y
`BranchSchedule` siguen exactamente el mismo patrón por capa que `Shipment`: entidad + DTOs +
interfaces en `Core`, repositorio + configuración EF en `Infrastructure`, servicio + validadores +
perfil de mapeo en `Services`, controlador + DI en `Web`. No se crea ningún repositorio ni
controlador para `BranchSchedule` por separado — es un objeto hijo del agregado `Branch`, se
gestiona íntegramente a través de `IBranchRepository`/`BranchService` (ver research.md, Decisión 4).

## Complexity Tracking

*Sin violaciones que justificar — tabla omitida intencionalmente.*
