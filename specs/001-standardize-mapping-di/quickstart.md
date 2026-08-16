# Quickstart: Validar el fix de mapeo/DI de forma manual

Sigue esta guía después de implementar `tasks.md` para confirmar que no hay regresión. La
verificación es manual (Swagger/HTTP), según la decisión confirmada en `spec.md` (Clarifications,
Q3) — no se crea proyecto de pruebas automatizadas para este cambio.

## Prerrequisitos

- .NET 8.0 SDK instalado.
- SQL Server accesible con la cadena de conexión de `ShipmentTracker.Web/appsettings.Development.json`.
- Base de datos migrada (`dotnet ef database update` desde `ShipmentTracker.Web` si hace falta).

## 1. Compilar

```
dotnet build ShipmentTracker.sln
```

Debe compilar sin errores ni advertencias nuevas. Confirma en particular que no quedan referencias
rotas a `ShipmentModel` (debe estar eliminado, ver `research.md` Decisión 4).

## 2. Levantar la API

```
dotnet run --project ShipmentTracker.Web
```

Abre Swagger UI (`/swagger`) en el entorno de desarrollo.

## 3. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de spec.md)

1. **Listar envíos** — `GET /api/shipment` (con y sin `status`): confirma que cada elemento trae
   `id`, `trackingNumber`, `recipient`, `status`, `createdAt`, `deliveredAt` (ver contrato en
   [`contracts/shipment-api-contract.md`](./contracts/shipment-api-contract.md)).
2. **Obtener por guía (existente)** — `GET /api/shipment/{trackingNumber}` con una guía real:
   200 OK con el DTO completo.
3. **Obtener por guía (inexistente)** — `GET /api/shipment/no-existe`: 404 con el mensaje de
   "No se encontró...".
4. **Crear envío** — `POST /api/shipment` con `{ "recipient": "Juan Pérez" }`: 201 Created, header
   `Location`, DTO con `trackingNumber` generado (`TRK-XXXXXXXX`), `status: "Collected"`,
   `deliveredAt: null`.
5. **Transición válida** — `PATCH /api/shipment/{trackingNumber}/status` con `"InTransit"` sobre un
   envío en `Collected`: 204 No Content.
6. **Transición inválida** — mismo endpoint con `"Delivered"` sobre un envío recién creado
   (`Collected`): 400 Bad Request con el mensaje de transición inválida (mismo texto que antes del
   cambio).
7. **Entrega** — transicionar un envío a `Delivered`: al consultar de nuevo con `GET`, `deliveredAt`
   ya no es `null`.

## 4. Verificación de código (no runtime)

- `ShipmentService`: sin ningún `new ShipmentDto { ... }` ni `new ShipmentTransitionValidator()`.
- `ShipmentController`: sin campo `IMapper` sin uso.
- `ShipmentTracker.Web/Models/ShipmentModel.cs`: ya no existe.
- `MappingProfiles.cs` (Web): ya no contiene `CreateMap<Shipment, ShipmentModel>` ni
  `CreateMap<ShipmentModel, Shipment>`.

Si los 7 escenarios manuales y las 4 verificaciones de código pasan, el fix cumple SC-001 a SC-005
de `spec.md`.
