# Implementation Plan: Paginación del Listado de Envíos

**Branch**: `002-paginate-shipment-list` | **Date**: 2026-08-16 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/002-paginate-shipment-list/spec.md`

## Summary

`GET /api/shipment` hoy devuelve todos los envíos (filtrados por `status` si se pide) sin límite ni
orden garantizado. El enfoque técnico: extender el repositorio genérico (`IBaseRepository<T>`) con
soporte de `skip`/`take` y un `CountAsync`, hacer que `ShipmentService` siempre ordene por
`CreatedAt` descendente y aplique paginación (con un tope de 50 en `pageSize`), y exponer la
metadata de paginación vía encabezados HTTP en el controlador — sin envolver el cuerpo de la
respuesta, que sigue siendo `ShipmentDto[]`. `GET /api/shipment/{trackingNumber}` y el resto de los
endpoints no cambian.

## Technical Context

**Language/Version**: C# sobre .NET 8.0 (`net8.0`, sin cambios respecto al resto de la solución)

**Primary Dependencies**: ASP.NET Core 8 (`System.ComponentModel.DataAnnotations`, ya incluido en
el BCL, para `[Range]`), Entity Framework Core 8 (`Skip`/`Take`/`CountAsync`, ya referenciado en
`Infrastructure`) — **cero paquetes NuGet nuevos**.

**Storage**: SQL Server vía EF Core; la paginación se resuelve a nivel de base de datos
(`OFFSET`/`FETCH` generado por `Skip().Take()`), no cargando la tabla completa en memoria.

**Testing**: Manual vía Swagger/HTTP, consistente con la política ya establecida del proyecto (no
existe proyecto de pruebas automatizadas; ver constitución).

**Target Platform**: ASP.NET Core Web API (sin cambios de hosting)

**Project Type**: Web service — misma solución en capas existente

**Performance Goals**: N/A — no se define una meta de latencia; el cambio a `Skip/Take` a nivel de
base de datos es estrictamente igual o más eficiente que el `ToListAsync()` sin límite que existe
hoy, para el volumen actual de la aplicación.

**Constraints**: Los endpoints existentes se mantienen — no se crea ninguna ruta nueva; la
paginación se agrega como parámetros de consulta opcionales sobre `GET /api/shipment` (restricción
explícita del usuario para esta planificación). El cuerpo de la respuesta de cada endpoint no
cambia de forma (FR-007); solo se agregan encabezados HTTP nuevos.

**Scale/Scope**: Cambio pequeño que toca las 4 capas de forma aditiva: `Core` (nuevo tipo
`PagedResult<T>`), `Infrastructure` (parámetros nuevos en el repositorio genérico), `Services`
(orden + clamping + paginación), `Web` (parámetros de consulta + encabezados + exposición CORS).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principio | Evaluación | Resultado |
|---|---|---|
| I. Framework Objetivo Único (.NET 8.0) | Ningún cambio de `TargetFramework` ni de versión de paquete; todo lo usado (`[Range]`, `Skip/Take/CountAsync`) ya está disponible en el BCL o en `Microsoft.EntityFrameworkCore` (ya referenciado). | PASS |
| II. Integridad de la Arquitectura en Capas | Cada capa cambia solo lo que le corresponde: `Core` gana un POCO sin dependencias externas; `Infrastructure` implementa la paginación dentro del repositorio (ningún otro proyecto toca `DbSet`/`AppDbContext` directamente, consistente con Restricciones Técnicas de la constitución); `Services` decide orden/tope (regla de negocio); `Web` traduce eso a query params/encabezados HTTP (su responsabilidad como composition root + capa HTTP). No se invierte ninguna flecha de dependencia. El cambio toca las 4 capas porque la paginación es, por naturaleza, una preocupación transversal — cada capa aporta exactamente la pieza mínima que ya le correspondía (contrato, implementación, regla de negocio, HTTP), no una fuga de responsabilidades entre capas. | PASS |
| III. Minimalismo de Dependencias | Cero paquetes NuGet nuevos — se investigó explícitamente (ver research.md) y todo lo necesario ya está disponible en dependencias existentes o en el BCL. | PASS |
| IV. Cambios Pequeños y Reversibles | Cambios aditivos: parámetros opcionales nuevos en interfaces existentes, un método nuevo (`CountAsync`), un tipo nuevo pequeño (`PagedResult<T>`). Ningún endpoint se elimina ni se reescribe; `GetAllAsync()` se deja intacto aunque quede sin uso, por no ser parte de esta inconsistencia puntual. | PASS |

No hay violaciones que registrar en Complexity Tracking.

**Re-check post Phase 1**: tras diseñar `data-model.md`, `contracts/` y `quickstart.md`, la tabla se
sostiene sin cambios.

## Project Structure

### Documentation (this feature)

```text
specs/002-paginate-shipment-list/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md         # Phase 1 output (/speckit-plan command)
├── quickstart.md         # Phase 1 output (/speckit-plan command)
├── contracts/            # Phase 1 output (/speckit-plan command)
│   └── shipment-list-contract.md
├── checklists/
│   └── requirements.md
└── tasks.md               # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── DTOs/
│   └── PagedResult.cs                          # [NUEVO] Items, Page, PageSize, TotalCount, TotalPages
└── Interfaces/
    ├── Repositories/
    │   └── IBaseRepository.cs                  # [MODIFICAR] GetAsync gana skip/take opcionales; + CountAsync
    └── Services/
        └── IShipmentService.cs                 # [MODIFICAR] GetShipmentsAsync retorna PagedResult<ShipmentDto>

ShipmentTracker.Infrastructure/
└── Repositories/
    └── BaseRepository.cs                       # [MODIFICAR] implementa skip/take (EF Core Skip/Take) y CountAsync

ShipmentTracker.Services/
└── ShipmentService.cs                          # [MODIFICAR] GetShipmentsAsync: ordena por CreatedAt desc,
                                                 #   aplica tope de pageSize (50), arma PagedResult<ShipmentDto>

ShipmentTracker.Web/
├── Program.cs                                  # [MODIFICAR] CORS: WithExposedHeaders para los encabezados
│                                                #   de paginación (si no, el frontend no puede leerlos)
└── Controllers/
    └── ShipmentController.cs                   # [MODIFICAR] GetShipments: + page/pageSize [FromQuery] con
                                                 #   [Range], setea encabezados de paginación, XML docs
```

**Structure Decision**: Misma arquitectura en capas existente, sin proyectos nuevos. `Core` gana un
tipo de datos (`PagedResult<T>`), análogo a los DTOs ya existentes, para transportar
ítems+metadata entre `Services` y `Web` sin que `Web` conozca detalles de repositorio ni `Services`
conozca detalles de HTTP.

## Complexity Tracking

*Sin violaciones que justificar — tabla omitida intencionalmente.*
