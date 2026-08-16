# Contrato HTTP: API de Shipment (a preservar sin cambios)

Este feature no modifica ningún contrato HTTP. Este documento fija el contrato actual como
línea base de regresión — la implementación se considera correcta solo si estos cuatro
endpoints se comportan exactamente igual antes y después del cambio (FR-007, SC-001 de spec.md).

## `GET /api/shipment`

- **Query params**: `status` (opcional, `ShipmentStatus` como string del enum)
- **200 OK**: `ShipmentDto[]`

## `GET /api/shipment/{trackingNumber}`

- **200 OK**: `ShipmentDto`
- **404 Not Found**: `{ "message": "No se encontró un envío con la guía '{trackingNumber}'." }`

## `POST /api/shipment`

- **Body**: `CreateShipmentDto` (`{ "recipient": string }`)
- **201 Created**: `ShipmentDto`, header `Location` apuntando a `GET /api/shipment/{trackingNumber}`

## `PATCH /api/shipment/{trackingNumber}/status`

- **Body**: `ShipmentStatus` (nuevo estado)
- **204 No Content**: transición aceptada
- **404 Not Found**: `{ "message": "No se encontró un envío con la guía '{trackingNumber}'." }`
- **400 Bad Request**: `{ "message": "Transición de estado inválida. No se puede pasar de '{actual}' a '{nuevo}'." }`

## Forma de `ShipmentDto`

Ver tabla de mapeo en [`../data-model.md`](../data-model.md). Campos: `id`, `trackingNumber`,
`recipient`, `status`, `createdAt`, `deliveredAt`.

## Qué NO cambia con este feature

- Rutas, verbos HTTP, códigos de estado y forma de los payloads de los cuatro endpoints de arriba.
- Los mensajes de error de validación de transición y de "no encontrado".

## Qué SÍ cambia (invisible para el consumidor de la API)

- Cómo se construye internamente `ShipmentDto` (AutoMapper en vez de asignación manual).
- Cómo `ShipmentService` obtiene su instancia del validador de transición (inyección de
  dependencias en vez de `new`).
