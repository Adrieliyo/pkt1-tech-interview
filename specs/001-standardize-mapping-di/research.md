# Research: Estandarizar Mapeo con AutoMapper e Inyección del Validador

Todos los `NEEDS CLARIFICATION` del Technical Context ya se resolvieron en `spec.md` (sección
Clarifications) antes de llegar a esta fase. Este documento registra las decisiones técnicas de
implementación derivadas de esas respuestas y de la investigación directa del código actual.

## Decisión 1: Ubicación del profile de AutoMapper

- **Decision**: Nuevo archivo `ShipmentTracker.Services/Mappings/ShipmentMappingProfile.cs` con
  `CreateMap<Shipment, ShipmentDto>()`.
- **Rationale**: Confirmado en Clarifications. `Services` es quien consume el mapeo; colocar el
  profile ahí evita que `Web` conozca detalles de mapeo que ya no usa (una vez retirado el
  `IMapper` sin uso del controlador). No infringe el Principio II: la dependencia nueva es hacia el
  paquete NuGet AutoMapper, no hacia el proyecto `Web`.
- **Alternatives considered**:
  - Extender el `MappingProfiles` existente en `Web` — rechazado: dejaría el profile en la capa
    equivocada (Web ya no tendría motivo para mapear Shipment una vez se retira su `IMapper`).
  - Nuevo profile en `Core` — rechazado: `Core` no tiene ninguna dependencia externa hoy (ni
    siquiera EF Core); añadir AutoMapper ahí rompería su característica de "cero dependencias" y
    no aporta nada, ya que `Core` no ejecuta lógica de mapeo.

## Decisión 2: Registro de AutoMapper para múltiples ensamblados

- **Decision**: En `ShipmentTracker.Web/Program.cs`, cambiar
  `builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);` para escanear también el
  ensamblado de `Services`:
  `builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly, typeof(ShipmentService).Assembly);`
- **Rationale**: `Program.cs` (composition root, `Web`) es el único lugar donde ya se configura
  AutoMapper; añadir un segundo ensamblado a escanear es un cambio de una línea, no requiere
  paquete nuevo y preserva el patrón existente (Principio IV: cambio pequeño y reversible).
- **Alternatives considered**:
  - Registrar un segundo `AddAutoMapper` para el ensamblado de `Services` — rechazado: AutoMapper
    permite pasar múltiples ensamblados en una sola llamada; dos llamadas separadas es redundante
    y menos idiomático.

## Decisión 3: Mecanismo de inyección del validador

- **Decision**: Registrar `builder.Services.AddScoped<IValidator<StatusTransitionContext>, ShipmentTransitionValidator>();`
  en `Program.cs`, e inyectar `IValidator<StatusTransitionContext>` en el constructor de
  `ShipmentService`, reemplazando `new ShipmentTransitionValidator()`.
- **Rationale**: `IValidator<T>` es parte del paquete base `FluentValidation` (ya referenciado por
  `Services`) — **no requiere el paquete adicional `FluentValidation.DependencyInjectionExtensions`**
  porque se registra manualmente, igual que el resto de las dependencias en `Program.cs`
  (`AddScoped<IUnitOfWork, UnitOfWork>()`, etc.). Cumple el Principio III (cero paquetes nuevos) y
  el Principio II (inyectar por interfaz, como ya se hace con `IUnitOfWork`, `IShipmentService`).
- **Alternatives considered**:
  - Inyectar la clase concreta `ShipmentTransitionValidator` directamente — rechazado: rompe la
    convención de inyectar por interfaz que ya sigue el resto del proyecto, y dificulta sustituir
    el validador en pruebas futuras (SC-003 del spec exige poder inyectar un doble).
  - Añadir el paquete `FluentValidation.DependencyInjectionExtensions` y usar
    `AddValidatorsFromAssemblyContaining<T>()` — rechazado: introduce un paquete nuevo para
    registrar un único validador; el registro manual de una línea cubre la misma necesidad sin
    dependencias adicionales (Principio III).
- **Note**: `StatusTransitionContext` y `ShipmentTransitionValidator` no cambian su lógica interna
  de validación de transiciones; solo cambia cómo se obtiene la instancia.

## Decisión 4: Eliminación de `ShipmentModel` y sus mapeos

- **Decision**: Eliminar `ShipmentTracker.Web/Models/ShipmentModel.cs` y las entradas
  `CreateMap<Shipment, ShipmentModel>()` / `CreateMap<ShipmentModel, Shipment>()` de
  `MappingProfiles.cs`.
- **Rationale**: Confirmado en Clarifications (Q2). Búsqueda de referencias
  (`grep -r "ShipmentModel"`) muestra que `ShipmentModel` solo aparece en su propia definición y en
  el archivo de perfiles — ningún controlador, servicio o repositorio lo usa.
- **Alternatives considered**: Dejarlo intacto — descartado explícitamente por el usuario al elegir
  el alcance ampliado en la clarificación.

## Decisión 5: No-objetivo — mapeos de `CreateShipmentDto`

- **Decision**: `CreateMap<Shipment, CreateShipmentDto>()` y `CreateMap<CreateShipmentDto, Shipment>()`
  en `MappingProfiles.cs` **no se tocan** en este cambio.
- **Rationale**: La investigación confirma que, al igual que los mapeos de `ShipmentModel`, estas
  dos entradas tampoco se usan hoy (no hay ninguna llamada a `IMapper.Map` en toda la solución).
  Sin embargo, el alcance confirmado en Clarifications (Q2) fue específicamente "`ShipmentModel` y
  sus mapeos", no una auditoría general de todo `MappingProfiles`. Tocarlas ahora sería expandir el
  cambio más allá de lo confirmado, en contra del Principio IV. Se documenta aquí para que quede
  registrado y no se pierda como hallazgo, pero queda fuera de `tasks.md`.
- **Alternatives considered**: Incluirlas en el mismo cambio — descartado por disciplina de
  alcance; puede proponerse como un fix separado si se desea.

## Decisión 6: Retiro del `IMapper` sin uso en `ShipmentController`

- **Decision**: Retirar el campo `_mapper` y el parámetro `IMapper mapper` del constructor de
  `ShipmentController`.
- **Rationale**: Confirmado por búsqueda de código: `_mapper` se asigna en el constructor pero
  nunca se invoca en ningún método del controlador. Una vez el mapeo vive en `Services`, esta
  dependencia queda sin ningún propósito (FR-006).
- **Alternatives considered**: Dejarlo por si se usa en el futuro — rechazado: es exactamente el
  tipo de código muerto/engañoso que el Principio II busca evitar (una dependencia inyectada que
  aparenta cumplir un propósito que ya no cumple).

## Estado

Todos los `NEEDS CLARIFICATION` resueltos. Sin bloqueos para Phase 1.
