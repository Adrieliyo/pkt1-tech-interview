# Feature Specification: Multiple Shipments per Order

**Feature Branch**: `009-order-multi-shipments`

**Created**: 2026-08-18

**Status**: Draft

**Input**: User description: "Permitir que una Orden (Order) pueda generar múltiples Envíos (Shipments), en lugar de la relación actual donde ConvertToShipmentAsync produce un único Shipment por Order."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generar un envío adicional sobre una orden ya convertida (Priority: P1)

Un operador de la sucursal tiene una orden que ya generó un envío, pero el paquete físico del cliente debe salir en más de una pieza (por ejemplo, se dividió en dos bultos porque no cupo en un solo vehículo/ruta). El operador necesita poder generar un segundo envío para la misma orden sin tener que crear una orden nueva desde cero, conservando la trazabilidad de que ambos envíos pertenecen al mismo pedido del cliente.

**Why this priority**: Es la capacidad central que motiva la feature — sin esto, el sistema sigue limitado a un envío por orden y el resto de historias no tiene sentido.

**Independent Test**: Puede probarse confirmando una orden, convirtiéndola a envío una vez, y luego invocando la conversión una segunda vez sobre la misma orden — el sistema debe generar un segundo Shipment con su propio número de rastreo, en vez de rechazar la operación.

**Acceptance Scenarios**:

1. **Given** una orden en estado Confirmed que aún no ha sido convertida, **When** el operador la convierte a envío, **Then** se genera un Shipment con número de rastreo propio y la orden pasa a reflejar que tiene al menos un envío asociado.
2. **Given** una orden que ya tiene un Shipment asociado y no se encuentra en un estado terminal, **When** el operador vuelve a invocar la conversión, **Then** se genera un segundo Shipment independiente (con su propio número de rastreo y ciclo de vida de estado), y el primer Shipment no se modifica.
3. **Given** una orden en estado Cancelled, **When** el operador intenta convertirla a envío, **Then** el sistema rechaza la operación con un mensaje claro.

---

### User Story 2 - Consultar todos los envíos de una orden (Priority: P2)

Un operador o un agente de servicio al cliente necesita ver, a partir de una orden, la lista completa de envíos que se generaron a raíz de ella, para poder informar al cliente sobre el estado de cada pieza de su pedido.

**Why this priority**: Sin visibilidad de los envíos asociados, la capacidad de generar varios pierde valor operativo — es el complemento necesario e inmediato de la Historia 1.

**Independent Test**: Puede probarse creando una orden con dos o más envíos asociados y consultando el listado de envíos de esa orden, verificando que aparecen todos con su información básica y paginados.

**Acceptance Scenarios**:

1. **Given** una orden con tres Shipments asociados, **When** se consulta el listado de envíos de esa orden, **Then** el sistema devuelve los tres envíos con su número de rastreo y estado actual, paginados.
2. **Given** una orden sin ningún Shipment asociado todavía, **When** se consulta su listado de envíos, **Then** el sistema devuelve una lista vacía (no un error).

---

### User Story 3 - Conocer el estado de cumplimiento agregado de la orden (Priority: P3)

Un operador que revisa el detalle de una orden con múltiples envíos necesita saber, de un vistazo, si el pedido completo ya fue entregado o si todavía hay envíos pendientes, sin tener que abrir cada envío por separado.

**Why this priority**: Mejora la experiencia operativa pero no bloquea el valor central (generar y listar múltiples envíos) — puede entregarse después de las dos historias anteriores.

**Independent Test**: Puede probarse creando una orden con dos envíos, marcando uno como entregado y dejando el otro en tránsito, y verificando que el detalle de la orden refleja correctamente que el cumplimiento está parcial.

**Acceptance Scenarios**:

1. **Given** una orden con dos envíos, ambos en estado Delivered, **When** se consulta el detalle de la orden, **Then** el sistema indica que el pedido está completamente cumplido (`Fulfilled`), sin que el operador tenga que marcarlo manualmente.
2. **Given** una orden con dos envíos, uno Delivered y otro InTransit, **When** se consulta el detalle de la orden, **Then** el sistema indica que el pedido todavía tiene envíos pendientes (no `Fulfilled`).
3. **Given** una orden con dos envíos, uno Delivered y el otro Cancelled, **When** se consulta el detalle de la orden, **Then** el sistema indica que el pedido está completamente cumplido, ya que el envío cancelado se excluye del cálculo.

---

### Edge Cases

- ¿Qué ocurre si se intenta convertir a envío una orden que aún está en estado Pending (nunca confirmada)? Debe seguir rechazándose, igual que hoy.
- ¿Qué ocurre si se consulta el listado de envíos de una orden que no existe? Debe devolver una respuesta de "no encontrado", no una lista vacía.
- ¿Qué ocurre si todos los envíos de una orden terminan cancelados y ninguno llega a ser entregado? La orden NO se considera cumplida (`Fulfilled`) en ese caso — permanece en su estado de "tiene envíos generados" indefinidamente, ya que no existe ningún envío entregado del que derivar el cumplimiento.
- ¿Qué pasa con órdenes ya existentes que tienen exactamente un Shipment asociado bajo el comportamiento anterior? Deben seguir funcionando sin cambios y sin ninguna migración de datos: como el cumplimiento se calcula bajo demanda a partir de los envíos, cualquier orden histórica ya `Converted` cuyo único envío esté Delivered aparecerá automáticamente como `Fulfilled` en cuanto se consulte, sin necesidad de tocar su fila almacenada.
- ¿Qué ocurre si se genera un nuevo envío sobre una orden que ya estaba en `Fulfilled` (todos sus envíos previos ya entregados)? Debe permitirse (no hay límite ni bloqueo por estar en `Fulfilled`, ver FR-009) y la orden regresa a su estado de "tiene envíos pendientes" hasta que el nuevo envío también se entregue.

## Clarifications

### Session 2026-08-18

- Q: ¿Existe un máximo de envíos por orden? → A: Sin límite explícito, mientras la orden no esté en estado `Cancelled`.
- Q: ¿Cuándo se considera una orden "completamente cumplida"? → A: Solo cuando TODOS sus envíos no cancelados están Delivered (los cancelados se excluyen del cálculo).
- Q: ¿Qué pasa con el estado `Converted` (hoy terminal)? → A: Se introduce un estado nuevo y explícito (`Fulfilled`) para "completamente cumplida", distinto de "tiene envíos generados, aún no todos entregados"; la orden transiciona automáticamente entre ambos según el estado de sus envíos.
- Q: ¿El estado de cumplimiento (`Fulfilled`) se persiste en la columna `OrderStatus`, o se calcula dinámicamente? → A: `OrderStatus` no cambia — `Converted` conserva su nombre y valor persistido actual y deja de ser terminal; el cumplimiento se calcula bajo demanda consultando los envíos de la orden, sin nueva columna ni migración de datos históricos.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE permitir que una orden confirmada genere más de un envío a lo largo de su ciclo de vida, en vez de bloquear toda conversión futura después de la primera.
- **FR-002**: El sistema DEBE seguir rechazando la conversión a envío de una orden que no está en un estado que lo permita (por ejemplo, Pending sin confirmar, o Cancelled), con un mensaje de error claro.
- **FR-003**: Cada envío generado a partir de una misma orden DEBE tener su propio número de rastreo único y su propio ciclo de vida de estado, independiente de los demás envíos de esa orden.
- **FR-004**: El sistema DEBE permitir consultar, a partir de una orden, la lista paginada de todos los envíos generados a raíz de ella, incluyendo el caso de que no tenga ninguno.
- **FR-005**: El sistema DEBE exponer, en la información de una orden, un indicador de cuántos envíos tiene asociados y si su cumplimiento está completo (`Fulfilled`) o pendiente. Este indicador se calcula bajo demanda a partir de los envíos existentes en el momento de la consulta — no es un valor almacenado junto con la orden.
- **FR-006**: El sistema DEBE seguir bloqueando toda modificación de los datos propios de la orden (destinatario, dirección, dimensiones, etc.) una vez que ha generado al menos un envío, igual que hoy ocurre tras la conversión.
- **FR-007**: El sistema DEBE permitir cancelar una orden únicamente en las mismas condiciones que hoy (antes de tener envíos asociados); una orden con al menos un envío generado ya no puede cancelarse como un todo.
- **FR-008**: El sistema DEBE seguir generando cada número de rastreo con el mismo formato y mecanismo ya usado (independiente por envío, no reutilizado entre los envíos de una misma orden).
- **FR-009**: El sistema NO DEBE imponer un número máximo de envíos por orden — la conversión puede repetirse un número ilimitado de veces mientras la orden no esté en estado `Cancelled`.
- **FR-010**: El sistema DEBE considerar una orden "completamente cumplida" únicamente cuando **todos** sus envíos no cancelados alcanzan el estado Delivered; los envíos cancelados se excluyen del cálculo (no cuentan como pendientes ni impiden el cumplimiento). Si una orden llega a tener todos sus envíos cancelados (ninguno entregado), NO se considera cumplida.
- **FR-011**: El sistema DEBE distinguir, cada vez que se consulta una orden, entre "tiene envíos generados pero aún no todos entregados" y "completamente cumplida" (`Fulfilled`: todos sus envíos no cancelados en Delivered). Esta distinción se determina en el momento de la consulta a partir del estado actual de los envíos de la orden — no requiere una acción manual del operador ni un campo de estado adicional persistido junto con la orden; el campo de estado propio de la orden (el que hoy distingue Pending/Confirmed/Converted/Cancelled) NO cambia de significado ni de valores para representarla.
- **FR-012**: El estado "completamente cumplida" (`Fulfilled`) NO es una barrera para seguir generando envíos adicionales sobre esa misma orden (ver FR-009); al generarse un nuevo envío sobre una orden ya `Fulfilled`, la siguiente consulta de esa orden vuelve a reflejar que tiene envíos pendientes hasta que ese nuevo envío también se entregue.

### Key Entities

- **Order**: entidad ya existente, sin cambios en su propio campo de estado persistido. Pasa de tener una relación implícita 1:1 con Shipment a una relación 1:N explotada a nivel de negocio — puede seguir generando envíos adicionales sin límite mientras no esté `Cancelled`. Su nivel de cumplimiento (`Fulfilled` cuando todos sus envíos no cancelados llegan a Delivered, o "pendiente" en caso contrario) se deriva en el momento de la consulta a partir de sus envíos asociados, no se guarda como parte de la orden.
- **Shipment**: entidad ya existente, sin cambios estructurales. Sigue referenciando a su Order de origen mediante la FK ya existente; ahora puede haber más de un Shipment compartiendo el mismo OrderId.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Un operador puede generar un envío adicional sobre una orden ya convertida en menos de 10 segundos, sin necesidad de crear una orden nueva.
- **SC-002**: El 100% de los envíos generados a partir de la misma orden conservan números de rastreo únicos entre sí, sin colisiones.
- **SC-003**: Un operador puede identificar, consultando una sola orden, el estado de cumplimiento de todos sus envíos asociados sin tener que abrir cada envío individualmente.
- **SC-004**: Las órdenes creadas antes de esta funcionalidad, con exactamente un envío asociado, conservan su estado (`Converted`) sin ningún cambio ni migración, y automáticamente muestran el indicador de cumplimiento correcto (`Fulfilled` o pendiente) según el estado real de su envío, sin intervención manual.

## Assumptions

- Una orden no tiene línea de artículos/cantidades propias (peso y dimensiones declaradas son un valor único por orden); "múltiples envíos" se interpreta como la posibilidad de generar más de una operación de conversión sobre la misma orden a lo largo del tiempo (por ejemplo, paquetes que se despachan por separado, o una segunda conversión operativa), no como una división automática de un listado de ítems.
- Los datos propios de la orden (destinatario, dirección, dimensiones) siguen siendo inmutables una vez que existe al menos un envío asociado, igual que en el comportamiento actual tras la conversión.
- No se modifica el modelo de `DeliveryAttempt` ni de `ShipmentEvent`; cada envío sigue registrando sus propios eventos exactamente como ya lo hace hoy.
- El listado de envíos por orden sigue la misma convención de paginación (`page`/`pageSize`, headers `X-Total-Count` etc.) ya usada en el resto de listados del sistema.
- Los roles y permisos que hoy pueden confirmar/convertir/cancelar órdenes y ver envíos se mantienen sin cambios; esta funcionalidad no introduce nuevos roles.
- El campo de estado persistido de la orden (`Pending`/`Confirmed`/`Converted`/`Cancelled`) no cambia de nombre, valores ni significado; no se requiere migración de datos históricos, ya que el cumplimiento (`Fulfilled`) es un valor derivado en el momento de la consulta, no un campo almacenado.
