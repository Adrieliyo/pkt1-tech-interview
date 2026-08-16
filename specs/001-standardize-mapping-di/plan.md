# Implementation Plan: Estandarizar Mapeo con AutoMapper e Inyección del Validador

**Branch**: `001-standardize-mapping-di` | **Date**: 2026-08-16 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-standardize-mapping-di/spec.md`

## Summary

`ShipmentService` mapea `Shipment → ShipmentDto` a mano en tres métodos mientras `AutoMapper` ya
está registrado y sin usar (`IMapper` inyectado en `ShipmentController` pero nunca invocado). El
enfoque técnico: mover el mapeo a un nuevo `Profile` de AutoMapper dentro de `ShipmentTracker.Services`
(capa donde vive el consumidor), inyectar `IMapper` en `ShipmentService`, registrar
`ShipmentTransitionValidator` en el contenedor de DI como `IValidator<StatusTransitionContext>` en
vez de instanciarlo con `new`, y eliminar el código muerto que la investigación confirmó
(`ShipmentModel` + sus mapeos, y el `IMapper` sin uso del controlador). Ningún endpoint cambia su
firma, ruta, código de estado o forma de respuesta.

## Technical Context

**Language/Version**: C# sobre .NET 8.0 (`net8.0`, ya fijado en los 4 `.csproj` de la solución)

**Primary Dependencies**: ASP.NET Core 8 (Web API), Entity Framework Core 8 + SQL Server
(persistencia, sin cambios), AutoMapper 16.2.0 (ya usado en `Web`, se extiende a `Services`),
FluentValidation 11.9.0 (ya usado en `Services`, se usa su interfaz `IValidator<T>` para DI)

**Storage**: SQL Server vía EF Core, encapsulado en `Infrastructure`; sin cambios en este feature

**Testing**: Verificación manual vía Swagger/HTTP (decisión confirmada en Clarifications de
spec.md); no se crea proyecto de pruebas automatizadas como parte de este cambio

**Target Platform**: ASP.NET Core Web API (multiplataforma, mismo hosting actual)

**Project Type**: Web service — solución en capas existente (`Core` / `Infrastructure` /
`Services` / `Web`)

**Performance Goals**: N/A — no se persigue ni se espera cambio de rendimiento; el mapeo pasa de
asignación manual a `IMapper.Map`, con overhead despreciable para el volumen actual

**Constraints**: El comportamiento observable de los endpoints en `Controller` y `Services`
(rutas, payloads, códigos HTTP, mensajes de error) NO debe cambiar (restricción explícita del
usuario para esta planificación, y ya exigida por FR-007/SC-001 del spec); no se agregan paquetes
NuGet nuevos más allá de extender AutoMapper (ya adoptado) al proyecto `Services`

**Scale/Scope**: Cambio pequeño acotado a 2 historias de usuario + limpieza de código muerto
confirmada; ~6 archivos existentes tocados, 1 archivo nuevo (profile de mapeo)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principio | Evaluación | Resultado |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | Todo el código y dependencias nuevas (AutoMapper 16.2.0) ya son compatibles con `net8.0`; no se toca ningún `TargetFramework`. | PASS |
| II. Integridad de la Arquitectura en Capas | El profile de mapeo se añade en `Services` (que ya solo depende de `Core`); `Services` gana una dependencia de paquete NuGet (no de proyecto) hacia AutoMapper, sin invertir el flujo `Infrastructure/Services → Core`. `Web` sigue como composition root (registra el escaneo del nuevo ensamblado). Se retira la competencia entre mapeo manual y AutoMapper (la inconsistencia que el principio prohíbe explícitamente). | PASS |
| III. Minimalismo de Dependencias | No se agrega ningún paquete NuGet nuevo a la solución: AutoMapper ya está adoptado (solo se referencia también desde `Services`); el registro del validador usa `IValidator<T>`, ya incluido en el paquete `FluentValidation` que `Services` ya referencia — cero paquetes nuevos. | PASS |
| IV. Cambios Pequeños y Reversibles | Alcance limitado a lo confirmado en Clarifications: mapeo + DI del validador + `ShipmentModel` muerto. No se tocan `CreateShipmentDto↔Shipment` (aunque también están sin uso hoy) por no haber sido parte de lo confirmado — se documenta como no-objetivo en research.md. | PASS |

No hay violaciones que registrar en Complexity Tracking.

**Re-check post Phase 1**: tras diseñar `data-model.md`, `contracts/` y `quickstart.md`, la tabla
anterior se sostiene sin cambios — ningún artefacto de diseño introdujo un proyecto, paquete o
inversión de dependencia no contemplada arriba.

## Project Structure

### Documentation (this feature)

```text
specs/001-standardize-mapping-di/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── shipment-api-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/                          # sin cambios
├── DTOs/ (ShipmentDto.cs, CreateShipmentDto.cs)
├── Entities/Shipment.cs
└── Interfaces/...

ShipmentTracker.Infrastructure/                # sin cambios

ShipmentTracker.Services/
├── ShipmentTracker.Services.csproj            # [MODIFICAR] + PackageReference AutoMapper 16.2.0
├── ShipmentService.cs                         # [MODIFICAR] inyectar IMapper e IValidator<StatusTransitionContext>;
│                                               #   reemplazar construcción manual de ShipmentDto por _mapper.Map
├── Mappings/
│   └── ShipmentMappingProfile.cs              # [NUEVO] CreateMap<Shipment, ShipmentDto>()
└── Validators/
    └── ShipmentTransitionValidator.cs          # sin cambios en su lógica interna

ShipmentTracker.Web/
├── Program.cs                                 # [MODIFICAR] AddAutoMapper escanea también el ensamblado de Services;
│                                               #   AddScoped<IValidator<StatusTransitionContext>, ShipmentTransitionValidator>()
├── Controllers/
│   └── ShipmentController.cs                  # [MODIFICAR] retirar el campo/parámetro IMapper sin uso
├── Mappers/
│   └── MappingProfiles.cs                     # [MODIFICAR] retirar CreateMap<Shipment, ShipmentModel> y
│                                               #   CreateMap<ShipmentModel, Shipment>
└── Models/
    └── ShipmentModel.cs                       # [ELIMINAR] confirmado sin uso fuera del propio mapeo
```

**Structure Decision**: Se mantiene la arquitectura en capas ya existente (Principio II de la
constitución) sin crear proyectos nuevos. Todos los cambios son ediciones puntuales dentro de
`Services` y `Web`; `Core` e `Infrastructure` no se tocan porque el problema reportado nunca vivió
ahí.

## Complexity Tracking

*Sin violaciones que justificar — tabla omitida intencionalmente.*
