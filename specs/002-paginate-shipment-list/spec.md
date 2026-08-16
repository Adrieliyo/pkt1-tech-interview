# Feature Specification: Paginación del Listado de Envíos

**Feature Branch**: `002-paginate-shipment-list`

**Created**: 2026-08-16

**Status**: Draft

**Input**: User description: "Se debe de implementar la paginación dentro de los endpoints GET de /api/shipment sin alterar lo que retorna cada uno. La paginación por default debe estar limitada a solo 5 registros, pero debe de tener la capaciadad de poderse aumentar la cantidad de los registros dependiendo de las necesidades del usuario."

## Clarifications

### Session 2026-08-16

- Q: ¿Dónde debe ir la información de paginación (total de registros, página actual, si hay más páginas) en la respuesta de `GET /api/shipment`? → A: En encabezados HTTP; el cuerpo de la respuesta se mantiene como el mismo arreglo de envíos que hoy.
- Q: ¿Debe existir un límite máximo al tamaño de página que un cliente puede solicitar? → A: Sí, con un tope fijo — si se pide más, se limita al máximo en vez de fallar.
- Q: ¿Qué criterio de orden determinista deben usar los envíos al paginar, dado que hoy el listado no tiene ningún `ORDER BY` explícito? → A: Por fecha de creación (`CreatedAt`) descendente — los envíos más recientes primero.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Ver envíos en grupos manejables por defecto (Priority: P1)

Como consumidor de la API (la aplicación frontend u otro cliente), quiero que al listar envíos
reciba un grupo pequeño y manejable (5 por defecto) en vez de todos a la vez, para que el listado
cargue rápido y no se sature con cientos de registros a medida que crece el volumen de envíos.

**Why this priority**: Es el comportamiento por defecto — afecta a todo consumidor del endpoint
desde el primer momento, sin que tenga que hacer nada especial para beneficiarse.

**Independent Test**: Llamar `GET /api/shipment` sin ningún parámetro de paginación, con más de 5
envíos existentes, y confirmar que se reciben exactamente 5.

**Acceptance Scenarios**:

1. **Given** hay más de 5 envíos registrados, **When** se llama `GET /api/shipment` sin
   parámetros de paginación, **Then** se reciben exactamente 5 envíos, junto con una indicación de
   que existen más disponibles.
2. **Given** hay 5 o menos envíos registrados en total, **When** se llama `GET /api/shipment` sin
   parámetros, **Then** se reciben todos los envíos existentes.

---

### User Story 2 - Pedir más registros o navegar a otras páginas (Priority: P2)

Como usuario con una necesidad puntual (por ejemplo, revisar o exportar más envíos de los que
caben en una página), quiero poder pedir más de 5 registros por página, o pedir una página
distinta a la primera, para ver más envíos sin depender únicamente del límite por defecto.

**Why this priority**: Complementa la Historia 1 — habilita el caso "necesito ver más" sin ser el
comportamiento por defecto, y puede entregarse justo después de que la Historia 1 esté funcionando.

**Independent Test**: Llamar `GET /api/shipment` solicitando un tamaño de página mayor a 5 y
confirmar que se reciben esa cantidad de registros; llamar solicitando una página distinta a la
primera y confirmar que se reciben los registros correspondientes a esa página, sin repetir los ya
vistos.

**Acceptance Scenarios**:

1. **Given** hay 12 envíos registrados, **When** se llama `GET /api/shipment` solicitando un
   tamaño de página de 10, **Then** se reciben 10 envíos.
2. **Given** hay 12 envíos registrados y un tamaño de página de 5, **When** se solicita la
   segunda página, **Then** se reciben los envíos 6 a 10 (los siguientes 5 tras la primera
   página), sin repetir ninguno de la primera página.
3. **Given** un tamaño de página o número de página inválido (negativo, cero, o no numérico),
   **When** se llama al endpoint, **Then** el sistema responde con un error claro en vez de
   fallar de forma inesperada o ignorar silenciosamente el parámetro.

---

### Edge Cases

- ¿Qué pasa si se solicita una página que no tiene registros (p. ej. página 100 cuando solo hay 3
  páginas de resultados)? Debe devolver una lista vacía, no un error.
- ¿Qué pasa si se combina la paginación con el filtro existente por `status`? La paginación se
  aplica sobre el conjunto de envíos ya filtrado por estado, no sobre el total sin filtrar.
- ¿Qué pasa con `GET /api/shipment/{trackingNumber}` (consulta de un único envío)? No aplica
  paginación — sigue devolviendo un solo objeto o `404`, sin cambios; queda fuera de alcance
  porque no devuelve una colección.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `GET /api/shipment` DEBE devolver como máximo 5 envíos cuando el cliente no
  especifica un tamaño de página.
- **FR-002**: `GET /api/shipment` DEBE permitir al cliente solicitar un tamaño de página distinto
  al valor por defecto (5) mediante un parámetro explícito.
- **FR-003**: `GET /api/shipment` DEBE permitir al cliente solicitar una página específica del
  listado, no solo la primera.
- **FR-004**: La paginación DEBE aplicarse sobre el conjunto de envíos ya filtrado por `status`
  cuando dicho filtro se use junto con los parámetros de paginación.
- **FR-005**: Cuando se solicita una página sin registros disponibles más allá del total, el
  sistema DEBE devolver una lista vacía en vez de un error.
- **FR-006**: Cuando el tamaño de página o el número de página solicitado es inválido (negativo,
  cero, o no numérico), el sistema DEBE responder con un error claro (`400 Bad Request`) en vez de
  ignorar el parámetro silenciosamente o fallar de forma inesperada.
- **FR-007**: La forma de cada envío individual dentro de la respuesta (los campos de cada
  elemento) DEBE permanecer sin cambios respecto al comportamiento actual.
- **FR-008**: `GET /api/shipment/{trackingNumber}` (consulta de un único envío) NO se ve afectado
  por esta funcionalidad — sigue devolviendo un solo objeto o `404` sin paginación.
- **FR-009**: El sistema DEBE comunicar al cliente, mediante encabezados HTTP en la respuesta,
  cuántos envíos hay en total, en qué página está, el tamaño de página usado y cuántas páginas
  existen en total. El cuerpo de la respuesta DEBE mantenerse como el arreglo de envíos que ya
  devuelve hoy, sin envolverlo en un objeto adicional (ver FR-007).
- **FR-010**: El tamaño de página que un cliente puede solicitar DEBE tener un límite superior
  fijo. Si el cliente solicita un tamaño mayor al límite, el sistema DEBE limitarlo al máximo
  permitido en vez de fallar o de devolver una cantidad ilimitada de registros.
- **FR-011**: `GET /api/shipment` DEBE devolver los envíos ordenados de forma determinista por
  fecha de creación (`CreatedAt`) descendente (los más recientes primero), tanto con como sin
  paginación, para que el mismo registro no aparezca en dos páginas distintas ni se omita entre
  una página y la siguiente (ver SC-003).

### Key Entities *(include if feature involves data)*

- **Shipment / envío individual**: sin cambios en su forma (ver FR-007).
- **Página de resultados**: subconjunto ordenado por `CreatedAt` descendente (FR-011),
  identificado por un número de página y un tamaño de página, acompañado de metadatos de
  paginación (total de registros, página actual, tamaño de página, total de páginas) expuestos
  vía encabezados HTTP (FR-009).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Al listar envíos sin especificar paginación, el usuario recibe como máximo 5
  registros, sin importar cuántos envíos existan en total.
- **SC-002**: Un usuario que necesita ver más de 5 envíos puede obtenerlos ajustando el tamaño de
  página solicitado, sin que cambie la forma en que identifica cada envío individual.
- **SC-003**: Un usuario puede recorrer la totalidad de los envíos registrados navegando por
  páginas sucesivas, sin perder ni duplicar registros entre una página y la siguiente.
- **SC-004**: Solicitar parámetros de paginación inválidos produce una respuesta de error clara en
  el 100% de los casos, en vez de un comportamiento inesperado.

## Assumptions

- Los parámetros de paginación se combinan con el filtro `status` ya existente en
  `GET /api/shipment`; la paginación se aplica sobre el resultado ya filtrado (ver Edge Cases).
- Si no se especifica número de página, se asume la primera.
- `GET /api/shipment/{trackingNumber}` queda fuera de alcance de esta feature, porque no devuelve
  una colección.
- El orden por `CreatedAt` descendente (FR-011) es un cambio observable respecto al
  comportamiento actual: hoy el listado no tiene `ORDER BY` explícito, por lo que su orden no
  está garantizado ni documentado como parte del contrato existente; introducir un orden
  determinista no se considera una ruptura de FR-007 (forma de cada envío), solo fija la
  secuencia en la que aparecen.
- El límite superior del tamaño de página (FR-010) se fija en 50 registros por página. Es un valor
  de referencia razonable para el volumen actual de la aplicación; puede ajustarse en la fase de
  planificación si se identifica una necesidad concreta de un tope distinto.
- Los nombres exactos de los encabezados HTTP de paginación (FR-009) y los nombres de los
  parámetros de consulta (para tamaño y número de página) se definen en `/speckit-plan`, siguiendo
  convenciones estándar de ASP.NET Core.
