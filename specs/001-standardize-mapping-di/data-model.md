# Data Model: Estandarizar Mapeo con AutoMapper e Inyección del Validador

Este feature no introduce entidades nuevas ni cambia el esquema de datos persistido. Documenta el
contrato de mapeo y la máquina de estados que ya existen, para que la implementación (y su
verificación manual) tengan una referencia única de lo que debe preservarse exactamente igual.

## Entidad: Shipment → ShipmentDto (mapeo, sin cambios de forma)

| Campo `Shipment` (origen) | Campo `ShipmentDto` (destino) | Tipo | Notas |
|---|---|---|---|
| `Id` | `Id` | `int` | Copia directa |
| `TrackingNumber` | `TrackingNumber` | `string` | Copia directa |
| `Recipient` | `Recipient` | `string` | Copia directa |
| `Status` | `Status` | `ShipmentStatus` (enum) | Copia directa |
| `CreatedAt` | `CreatedAt` | `DateTime` | Copia directa |
| `DeliveredAt` | `DeliveredAt` | `DateTime?` | Copia directa; DEBE preservar `null` (Edge Case en spec.md) |

El nuevo profile de AutoMapper (`ShipmentMappingProfile`) debe producir exactamente esta tabla vía
`CreateMap<Shipment, ShipmentDto>()` sin `.ForMember` adicionales, ya que los nombres de propiedad
ya coinciden 1:1 entre ambos tipos.

**Fuera de alcance**: `CreateShipmentDto → Shipment` (creación) sigue construyéndose como hoy en
`ShipmentService.CreateShipmentAsync` (asignación manual de `Recipient` + valores calculados
`TrackingNumber`, `Status`, `CreatedAt`). No es el mapeo duplicado reportado y no se toca (ver
research.md, Decisión 5).

## Máquina de estados: StatusTransitionContext (sin cambios de lógica, solo de resolución)

Transiciones válidas ya implementadas en `ShipmentTransitionValidator.BeAValidTransition` —
se documentan aquí como contrato a preservar:

| Estado actual | Transición a sí mismo | Transiciones válidas | Transiciones inválidas |
|---|---|---|---|
| `Collected` | permitido | `InTransit`, `Cancelled` | `Delivered` |
| `InTransit` | permitido | `Delivered`, `Cancelled` | `Collected` |
| `Delivered` | permitido | — | `InTransit`, `Collected`, `Cancelled` |
| `Cancelled` | permitido | — | `InTransit`, `Collected`, `Delivered` |

Efecto colateral ya existente que se preserva: al transicionar a `Delivered`, si `DeliveredAt` es
`null`, se asigna `DateTime.UtcNow`. Este comportamiento vive en `ShipmentService.UpdateShipmentStatusAsync`
y no depende de cómo se resuelve el validador, por lo que no cambia con este feature.

## Entidades de código afectadas (no de datos)

- **ShipmentMappingProfile** *(nuevo)*: `Profile` de AutoMapper en `ShipmentTracker.Services/Mappings/`.
- **ShipmentModel** *(eliminado)*: sin relación con el modelo de datos persistido; era un tipo de
  `Web` sin referencias activas.
