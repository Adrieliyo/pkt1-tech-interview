---
description: "Task list template for feature implementation"
---

# Tasks: Estandarizar Mapeo con AutoMapper e Inyección del Validador

**Input**: Design documents from `/specs/001-standardize-mapping-di/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/shipment-api-contract.md, quickstart.md

**Tests**: Sin tareas de pruebas automatizadas — decisión confirmada en spec.md (Clarifications,
Q3): verificación manual vía `quickstart.md`, sin crear proyecto de pruebas.

**Organization**: Las tareas se agrupan por historia de usuario para poder implementar y validar
cada una de forma independiente, tal como exige el spec (Independent Test de cada historia).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivo distinto, sin dependencia de tareas incompletas)
- **[Story]**: Historia de usuario a la que pertenece (US1, US2)
- Cada tarea incluye la ruta exacta del archivo

## Path Conventions

Solución .NET existente en capas (`ShipmentTracker.Core` / `.Infrastructure` / `.Services` /
`.Web`, ver `plan.md`). Todas las rutas son relativas a la raíz del repositorio.

---

## Phase 1: Setup

**N/A para este feature.** No hay inicialización de proyecto nueva: la solución, sus 4 proyectos y
sus paquetes base ya existen. La única dependencia nueva (paquete AutoMapper en `Services`) es
específica de la Historia 1 y se declara ahí (T001).

---

## Phase 2: Foundational (Blocking Prerequisites)

**N/A para este feature.** US1 y US2 no comparten infraestructura nueva que deba prepararse antes:
el contenedor de DI, el punto de registro de AutoMapper (`Program.cs`) y EF Core ya existen y
soportan ambas historias sin cambios previos compartidos. Cada historia declara sus propias
dependencias dentro de su fase.

**Checkpoint**: No aplica — se puede iniciar directamente la Fase 3.

---

## Phase 3: User Story 1 - Unificar el mapeo de Shipment a DTO mediante AutoMapper (Priority: P1) 🎯 MVP

**Goal**: Que `ShipmentDto` se construya en un único lugar (AutoMapper), eliminando la asignación
manual campo por campo en `ShipmentService` y el código muerto relacionado (`IMapper` sin uso en el
controlador, `ShipmentModel` sin referencias).

**Independent Test**: Llamar a `GET /api/shipment`, `GET /api/shipment/{trackingNumber}` y
`POST /api/shipment` (ver escenarios 1, 2 y 4 de `quickstart.md`) y confirmar que las respuestas no
cambian, mientras se verifica por código que `ShipmentService` ya no construye `ShipmentDto` a mano.

### Implementation for User Story 1

- [X] T001 [P] [US1] Agregar `<PackageReference Include="AutoMapper" Version="16.2.0" />` a `ShipmentTracker.Services/ShipmentTracker.Services.csproj`
- [X] T002 [P] [US1] Crear `ShipmentTracker.Services/Mappings/ShipmentMappingProfile.cs`: clase `ShipmentMappingProfile : Profile` con `CreateMap<Shipment, ShipmentDto>();` en el constructor (depende de T001; ver tabla de campos en `data-model.md`)
- [X] T003 [P] [US1] En `ShipmentTracker.Services/ShipmentService.cs`: inyectar `IMapper` por constructor (campo `_mapper`, junto al ya existente `_unitOfWork`) y reemplazar la construcción manual de `ShipmentDto` en `GetShipmentsAsync`, `GetShipmentByTrackingNumberAsync` y `CreateShipmentAsync` por `_mapper.Map<ShipmentDto>(shipment)` (depende de T001)
- [X] T004 [US1] En `ShipmentTracker.Web/Program.cs`: cambiar `builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);` para escanear también el ensamblado de Services: `..., typeof(Program).Assembly, typeof(ShipmentTracker.Services.ShipmentService).Assembly);` (depende de T002)
- [X] T005 [P] [US1] En `ShipmentTracker.Web/Controllers/ShipmentController.cs`: retirar el campo `IMapper _mapper` y el parámetro `IMapper mapper` del constructor (sin uso actual), y el `using AutoMapper;` que queda sin uso
- [X] T006 [P] [US1] Eliminar el archivo `ShipmentTracker.Web/Models/ShipmentModel.cs` (confirmado sin referencias fuera del perfil de mapeo)
- [X] T007 [US1] En `ShipmentTracker.Web/Mappers/MappingProfiles.cs`: retirar las líneas `CreateMap<Shipment, ShipmentModel>();` y `CreateMap<ShipmentModel, Shipment>();`, y el `using ShipmentTracker.Web.Models;` que queda sin uso (depende de T006)

**Checkpoint**: `ShipmentDto` se genera exclusivamente vía AutoMapper; `ShipmentModel` y el `IMapper`
sin uso del controlador ya no existen. Historia 1 verificable de forma independiente.

---

## Phase 4: User Story 2 - Inyectar el validador de transición de estado (Priority: P2)

**Goal**: Que `ShipmentTransitionValidator` se resuelva por inyección de dependencias, igual que el
resto de las dependencias de `ShipmentService`, en vez de instanciarse con `new` dentro de un
método.

**Independent Test**: Inspeccionar el constructor de `ShipmentService` (recibe el validador
inyectado) y ejecutar los escenarios 5 y 6 de `quickstart.md` (`PATCH .../status` con transición
válida e inválida) confirmando que el comportamiento no cambia.

### Implementation for User Story 2

- [X] T008 [US2] En `ShipmentTracker.Web/Program.cs`: registrar `builder.Services.AddScoped<IValidator<StatusTransitionContext>, ShipmentTransitionValidator>();` (usa `FluentValidation.IValidator<T>`, ya incluido en el paquete `FluentValidation` referenciado por `Services`; añadir el `using` correspondiente) (depende de T004, mismo archivo)
- [X] T009 [US2] En `ShipmentTracker.Services/ShipmentService.cs`: inyectar `IValidator<StatusTransitionContext>` por constructor (campo `_transitionValidator`) y en `UpdateShipmentStatusAsync` reemplazar `var validator = new ShipmentTransitionValidator();` por el uso directo de `_transitionValidator` (depende de T003, mismo archivo)

**Checkpoint**: `ShipmentTransitionValidator` se resuelve por DI; ninguna clase de `Services` o
`Web` instancia dependencias con `new`, salvo la propia entidad `Shipment` en `CreateShipmentAsync`
(fuera de alcance, ver `research.md` Decisión 5).

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Confirmar que el cambio compila y que el comportamiento observable no cambió, cerrando
FR-007/SC-001 a SC-005 de `spec.md`.

- [X] T010 [P] Compilar la solución (`dotnet build ShipmentTracker.sln`) y confirmar cero errores y cero advertencias nuevas (depende de T001-T009)
- [X] T011 Ejecutar de punta a punta los 7 escenarios manuales y las 4 verificaciones de código de `specs/001-standardize-mapping-di/quickstart.md` (depende de T010)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup / Foundational**: N/A — se puede empezar directo en la Fase 3.
- **User Story 1 (Fase 3)**: sin dependencias de otras historias.
- **User Story 2 (Fase 4)**: depende de que T003 y T004 (US1) ya existan, porque T008 y T009
  editan los mismos archivos (`Program.cs`, `ShipmentService.cs`) que US1 tocó primero. No depende
  conceptualmente de la lógica de US1 (son cambios independientes en el mismo archivo).
- **Polish (Fase 5)**: depende de que Fases 3 y 4 estén completas.

### Dentro de cada historia

- T001 → T002, T003 (el paquete debe existir antes de usar `IMapper`/`Profile`)
- T002 → T004 (el ensamblado a escanear debe contener ya el profile)
- T006 → T007 (la clase debe eliminarse antes de quitar sus `CreateMap`, o viceversa; se referencia
  como par para evitar dejar el build roto entre tareas)
- T004 → T008, T003 → T009 (mismo archivo, ver arriba)
- T001–T009 → T010 → T011

### Parallel Opportunities

- Al iniciar la Fase 3: **T001, T005, T006** son independientes entre sí (archivos distintos, sin
  dependencias) y pueden hacerse en paralelo.
- Tras T001: **T002 y T003** son independientes entre sí (archivos distintos) y pueden hacerse en
  paralelo.
- US1 y US2 tienen una dependencia de archivo (no conceptual) — en la práctica, completar US1 antes
  de tocar los mismos archivos en US2 evita conflictos de edición.

---

## Parallel Example: User Story 1

```bash
# Al inicio de la Fase 3, en paralelo:
Task: "Agregar PackageReference AutoMapper a ShipmentTracker.Services.csproj"
Task: "Retirar IMapper sin uso de ShipmentController.cs"
Task: "Eliminar ShipmentTracker.Web/Models/ShipmentModel.cs"

# Tras completar T001, en paralelo:
Task: "Crear ShipmentMappingProfile.cs en ShipmentTracker.Services/Mappings/"
Task: "Inyectar IMapper en ShipmentService.cs y usar _mapper.Map<ShipmentDto>(...)"
```

---

## Implementation Strategy

### MVP First (User Story 1 solamente)

1. Completar Fase 3 (T001-T007).
2. Ejecutar los escenarios 1, 2 y 4 de `quickstart.md` para validar US1 de forma aislada.
3. US1 por sí sola ya resuelve la inconsistencia de mapeo reportada originalmente — es un
   incremento entregable por separado de US2.

### Incremental Delivery

1. Fase 3 (US1) → validar → esta es la corrección de mayor impacto, lista para revisar/mergear.
2. Fase 4 (US2) → validar con los escenarios 5 y 6 → corrige la inyección del validador.
3. Fase 5 (Polish) → build limpio + validación manual completa de los 4 endpoints.

---

## Notes

- No hay tareas de test automatizado por decisión explícita del usuario (spec.md, Clarifications
  Q3); la validación es manual vía `quickstart.md`.
- `CreateMap<Shipment, CreateShipmentDto>()` y `CreateMap<CreateShipmentDto, Shipment>()` en
  `MappingProfiles.cs` (Web) se dejan intactos a propósito — no forman parte del alcance confirmado
  (ver `research.md`, Decisión 5). No generar una tarea para tocarlos.
- Cada tarea toca como máximo 1-2 archivos, consistente con el Principio IV de la constitución
  (cambios pequeños y reversibles).
