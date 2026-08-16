# Feature Specification: Estandarizar Mapeo con AutoMapper e Inyección del Validador

**Feature Branch**: `001-standardize-mapping-di`

**Created**: 2026-08-16

**Status**: Draft

**Input**: User description: "Se debe de arreglar el mapeo duplicado entre el AutoMapper, ya que inyecto IMapper en ShipmentController pero en ShipmentService se mapean las entidades a DTOs a mano, se debe de estandarizar una sola estrategia de mapeo que será mediante el AutoMapper. El validador debe de seguir el patron de Inyeccion de dependencias del resto del proyecto."

## Clarifications

### Session 2026-08-16

- Q: ¿En qué proyecto de la solución debería vivir el nuevo mapeo de AutoMapper (`Shipment → ShipmentDto`)? → A: Nuevo profile en `ShipmentTracker.Services`, registrado escaneando también ese ensamblado en `Program.cs`.
- Q: ¿El alcance debe limitarse al mapeo `Shipment → ShipmentDto` y la inyección del validador, o ampliarse a limpiar `ShipmentModel` y sus mapeos si están muertos? → A: Alcance ampliado; se confirmó que `ShipmentModel` solo se referencia en `MappingProfiles.cs` y en su propia definición (`ShipmentTracker.Web\Models\ShipmentModel.cs`), sin uso en controladores ni servicios — se elimina junto con sus `CreateMap` asociados.
- Q: ¿La verificación de que este cambio no rompe nada debe ser manual, o debe incluir crear un proyecto de pruebas automatizadas mínimo? → A: Verificación manual únicamente (Swagger/HTTP); no se crea proyecto de pruebas como parte de este cambio.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Unificar el mapeo de Shipment a DTO mediante AutoMapper (Priority: P1)

Como responsable de mantener ShipmentTracker, quiero que exista una única forma de convertir un
`Shipment` en su DTO correspondiente, para no tener que actualizar dos lugares distintos (mapeo
manual y AutoMapper) cada vez que cambie la forma de representar un envío.

**Why this priority**: Es la inconsistencia de mayor impacto: afecta a los tres endpoints de
lectura/creación de envíos y es la causa raíz señalada en la revisión de arquitectura previa. Sin
esto, cualquier cambio futuro en `ShipmentDto` requiere recordar actualizar dos estrategias de
mapeo distintas.

**Independent Test**: Se verifica llamando a los tres endpoints afectados (`GET /api/shipment`,
`GET /api/shipment/{trackingNumber}`, `POST /api/shipment`) y comprobando que las respuestas no
cambian frente al comportamiento actual, mientras se confirma por revisión de código que
`ShipmentService` ya no construye `ShipmentDto` asignando sus propiedades una por una.

**Acceptance Scenarios**:

1. **Given** un envío existente en la base de datos, **When** se consulta
   `GET /api/shipment/{trackingNumber}`, **Then** la respuesta contiene los mismos campos y
   valores que antes del cambio (Id, TrackingNumber, Recipient, Status, CreatedAt, DeliveredAt).
2. **Given** una solicitud válida de creación de envío, **When** se invoca `POST /api/shipment`,
   **Then** el envío creado se devuelve con el mismo formato de DTO que antes, generado a través
   de AutoMapper y no mediante asignación manual de campos.
3. **Given** el código fuente de `ShipmentService`, **When** se inspecciona cualquiera de sus
   métodos, **Then** ninguno construye una instancia de `ShipmentDto` asignando sus propiedades
   una por una.

---

### User Story 2 - Inyectar el validador de transición de estado (Priority: P2)

Como responsable de mantener ShipmentTracker, quiero que `ShipmentTransitionValidator` se
resuelva por inyección de dependencias igual que el resto de las dependencias del servicio, para
poder sustituirlo o probarlo de forma aislada sin depender de una instancia creada internamente.

**Why this priority**: Corrige una inconsistencia de alcance menor (una sola clase, un solo
método) que no cambia la respuesta observable de la API, pero sí la capacidad de mantener y
probar `ShipmentService` de forma aislada. Es independiente de la User Story 1 y puede entregarse
por separado.

**Independent Test**: Se verifica revisando que `ShipmentService` recibe el validador como
parámetro inyectado por constructor, que dicho tipo está registrado en el contenedor de
dependencias, y confirmando con una llamada a `PATCH /api/shipment/{trackingNumber}/status` que
las transiciones válidas e inválidas se comportan igual que antes.

**Acceptance Scenarios**:

1. **Given** el constructor de `ShipmentService`, **When** se inspecciona su firma, **Then** el
   validador de transición de estado se recibe como parámetro inyectado, y no se instancia con
   `new` dentro de un método.
2. **Given** una transición de estado inválida (p. ej. de `Delivered` a `InTransit`), **When** se
   invoca `PATCH /api/shipment/{trackingNumber}/status`, **Then** la API responde `400 Bad Request`
   con el mismo mensaje de error que antes del cambio.
3. **Given** una transición de estado válida, **When** se invoca
   `PATCH /api/shipment/{trackingNumber}/status`, **Then** la API responde `204 No Content` igual
   que antes del cambio.

---

### Edge Cases

- ¿Qué pasa si el perfil de AutoMapper no incluye un mapeo explícito de `Shipment` a
  `ShipmentDto`? Hoy no existe (`MappingProfiles` solo mapea `Shipment↔ShipmentModel` y
  `Shipment↔CreateShipmentDto`); debe agregarse, o el mapeo fallaría en tiempo de ejecución o
  devolvería un DTO vacío.
- ¿Qué pasa con el campo `DeliveredAt` cuando es `null` en la entidad? El mapeo automático debe
  preservar el valor `null` igual que la asignación manual actual, sin sustituirlo por una fecha
  por defecto.
- ¿Qué pasa con la dependencia `IMapper` ya inyectada (pero sin uso) en `ShipmentController`? Al
  moverse el mapeo al servicio, esa dependencia queda sin uso en el controlador y debe retirarse
  para no dejar una dependencia inyectada que nadie invoca.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE mapear `Shipment` a `ShipmentDto` exclusivamente mediante AutoMapper
  en las tres operaciones que hoy construyen el DTO manualmente (listar envíos, obtener por número
  de guía, crear envío).
- **FR-002**: `ShipmentService` NO DEBE contener construcción manual campo por campo de
  `ShipmentDto`.
- **FR-003**: El perfil de mapeo de AutoMapper para `Shipment → ShipmentDto` DEBE cubrir
  exactamente los mismos campos que hoy se asignan a mano (Id, TrackingNumber, Recipient, Status,
  CreatedAt, DeliveredAt), sin pérdida ni adición de datos.
- **FR-004**: El mecanismo de mapeo (`IMapper`) DEBE estar disponible en `ShipmentService` por
  inyección de dependencias, siguiendo el mismo patrón de inyección por constructor ya usado para
  `IUnitOfWork`. El profile de AutoMapper que define `Shipment → ShipmentDto` DEBE residir en
  `ShipmentTracker.Services`, junto al código que lo consume.
- **FR-005**: `ShipmentTransitionValidator` DEBE inyectarse en `ShipmentService` vía constructor y
  estar registrado en el contenedor de dependencias, reemplazando la instanciación directa con
  `new` que existe hoy.
- **FR-006**: La dependencia `IMapper` inyectada y no utilizada en `ShipmentController` DEBE
  retirarse una vez el mapeo se resuelva en el servicio, para no dejar código muerto.
- **FR-007**: El comportamiento observable de los endpoints existentes de envíos (`GET`, `POST`,
  `PATCH`) DEBE permanecer sin cambios para los consumidores de la API: mismos campos, mismos
  valores, mismos códigos de estado HTTP y mismos mensajes de error.
- **FR-008**: La clase `ShipmentModel` (`ShipmentTracker.Web/Models/ShipmentModel.cs`) y sus
  mapeos asociados (`Shipment↔ShipmentModel`) DEBEN eliminarse, dado que no se referencian desde
  ningún controlador ni servicio de la solución.

### Key Entities *(include if feature involves data)*

- **Shipment**: entidad de dominio que representa un envío; origen del mapeo.
- **ShipmentDto / CreateShipmentDto**: contratos de datos expuestos por la API; destino/origen del
  mapeo.
- **StatusTransitionContext / ShipmentTransitionValidator**: encapsula la regla de negocio de
  transición de estado válida; pasa de instanciarse manualmente a resolverse por inyección de
  dependencias.
- **MappingProfiles**: perfil de AutoMapper existente en `Web`; pierde los mapeos `Shipment↔ShipmentModel`
  (retirados junto con `ShipmentModel`) y gana un nuevo profile hermano en `Services` para
  `Shipment → ShipmentDto`.
- **ShipmentModel**: clase sin referencias fuera del propio perfil de mapeo; se elimina de la
  solución como parte de este cambio.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Las respuestas de los tres endpoints de envíos (listar, obtener por guía, crear) son
  idénticas en estructura y valores, antes y después del cambio, para los mismos datos de entrada.
- **SC-002**: Existe exactamente un mecanismo de conversión de `Shipment` a `ShipmentDto` en toda
  la solución (cero ocurrencias de construcción manual del DTO).
- **SC-003**: `ShipmentService` puede instanciarse proporcionando un doble/mock del validador de
  transición, sin que el validador real necesite ejecutarse.
- **SC-004**: El comportamiento de las transiciones de estado válidas e inválidas (aceptación o
  rechazo con mensaje) permanece idéntico al comportamiento previo al cambio.
- **SC-005**: `ShipmentModel` y sus mapeos asociados ya no existen en la solución, y el build
  compila sin advertencias de tipos o mapeos huérfanos.

## Assumptions

- AutoMapper ya es una dependencia adoptada en la solución (usada en `Web`); extender su uso a
  `Services` no se considera una dependencia nueva bajo el Principio III de la constitución, pero
  sí implica agregar la referencia al paquete NuGet AutoMapper en el proyecto `Services` (hoy solo
  referencia FluentValidation).
- El perfil de mapeo `Shipment → ShipmentDto` vive en `ShipmentTracker.Services` (ver
  Clarifications); esto requiere que `Program.cs` registre AutoMapper escaneando también el
  ensamblado de `Services`, además del de `Web`.
- El validador se registra en el contenedor de dependencias como `Scoped`, consistente con el
  resto de los registros en `Program.cs`, dado que no mantiene estado entre solicitudes.
- No existe actualmente un proyecto de pruebas automatizadas. Confirmado (ver Clarifications): la
  verificación de "sin regresión" de este cambio se realiza mediante pruebas manuales de los
  endpoints (Swagger/HTTP); no se crea un proyecto de pruebas como parte de este fix.
- La dependencia `IMapper` hoy inyectada y sin uso en `ShipmentController` se retira como parte de
  este cambio, al quedar redundante una vez el mapeo se resuelve en el servicio.
