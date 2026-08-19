# Implementation Plan: Multiple Shipments per Order

**Branch**: `009-order-multi-shipments` | **Date**: 2026-08-18 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/009-order-multi-shipments/spec.md`

## Summary

Permitir que `OrderService.ConvertToShipmentAsync` se invoque más de una vez sobre la misma orden, generando un `Shipment` independiente cada vez, en lugar de bloquear la conversión tras la primera (estado `Converted` deja de ser terminal para ese propósito). Se añade un indicador de cumplimiento (`Fulfilled`/pendiente + conteo de envíos) calculado bajo demanda en `OrderService` consultando los `Shipment` hijos vía `IUnitOfWork.ShipmentRepository`, expuesto en `OrderDto`, y un nuevo endpoint paginado `GET /api/orders/{id}/shipments` que reutiliza `ShipmentDto`. No se persiste ninguna columna nueva, no se toca `OrderStatus` como enum, y no se modifica `Shipment`, `ShipmentEvent` ni `DeliveryAttempt`.

## Technical Context

**Language/Version**: C# 12 / .NET 8.0 (`net8.0`, ya fijado en los 4 `.csproj`)

**Primary Dependencies**: EF Core 8 (`ShipmentTracker.Infrastructure`), AutoMapper, FluentValidation, Swashbuckle/Swagger — todas ya presentes; no se añade ninguna dependencia nueva.

**Storage**: SQL Server vía EF Core, encapsulado en `AppDbContext`/repositorios. Ningún cambio de esquema (sin migración) — el indicador de cumplimiento es un valor calculado, no una columna.

**Testing**: No hay proyecto de pruebas automatizadas en la solución (validación manual vía Swagger, documentada en `quickstart.md`, igual que el resto de módulos).

**Target Platform**: API REST ASP.NET Core (Kestrel), self-hosted / IIS según despliegue existente — sin cambios.

**Project Type**: Web service (arquitectura en 4 capas ya establecida: `Core`, `Infrastructure`, `Services`, `Web`).

**Performance Goals**: Sin objetivos nuevos más allá de los ya implícitos en el resto de la API (respuesta interactiva típica de un CRUD paginado); no aplica un target cuantitativo nuevo.

**Constraints**: El cálculo de cumplimiento se resuelve con una sola consulta adicional por orden consultada (vía `GetAsync`/`CountAsync` filtrado por `OrderId`), sin N+1 al listar múltiples órdenes salvo que el listado de órdenes decida exponer el mismo indicador por fila (ver Fase 1 / data-model.md para el alcance exacto).

**Scale/Scope**: Mismo volumen de datos que el resto de la solución (una PyME de logística); no introduce un patrón de escala distinto.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **I. Framework Objetivo Único (.NET 8.0)**: Sin cambios de `TargetFramework`. ✅ PASS
- **II. Integridad de la Arquitectura en Capas**: `OrderService` (capa `Services`) sigue consultando `Shipment` solo a través de `IUnitOfWork.ShipmentRepository` (patrón ya sancionado explícitamente para `Order` en `ConvertToShipmentAsync`, ver CLAUDE.md sección "Extending an existing, already-shipped module"). Ningún proyecto nuevo, ninguna flecha de dependencia nueva. ✅ PASS
- **III. Minimalismo de Dependencias**: No se añade ningún paquete NuGet. ✅ PASS
- **IV. Cambios Pequeños y Reversibles**: El cambio se limita a `OrderService.ConvertToShipmentAsync` (relajar el guard), un método de solo lectura nuevo para el indicador de cumplimiento, un endpoint de listado nuevo en `OrderController`, y los DTOs asociados — sin tocar `Shipment`, `ShipmentEvent`, `DeliveryAttempt` ni sus controladores. ✅ PASS

No hay violaciones que requieran `Complexity Tracking`.

## Project Structure

### Documentation (this feature)

```text
specs/009-order-multi-shipments/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
│   └── orders-shipments.md
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
ShipmentTracker.Core/
├── DTOs/Orders/OrderDto.cs                          # + ShipmentsCount, IsFulfilled (calculados por el Service, no mapeados por AutoMapper)
└── Interfaces/Services/IOrderService.cs             # + GetShipmentsByOrderAsync(...)

ShipmentTracker.Services/
├── OrderService.cs                                  # ConvertToShipmentAsync: relaja el guard; + método privado de cumplimiento; + GetShipmentsByOrderAsync
└── Mappings/ (sin cambios — AutoMapper sigue mapeando solo Entity → Dto 1:1 para los campos existentes)

ShipmentTracker.Web/
└── Controllers/OrderController.cs                   # + GET /api/orders/{id}/shipments
```

**Structure Decision**: Se extiende el módulo `Order` ya existente (mismo patrón que `006-orders`), sin nuevos proyectos ni nuevas entidades. `Shipment`/`ShipmentRepository`/`ShipmentDto` se reutilizan sin modificación. Todo el cambio vive en `Core` (contrato), `Services` (lógica), `Web` (endpoint) — `Infrastructure` no requiere cambios porque el filtro genérico `GetAsync(filter: x => x.OrderId == id, ...)` ya cubre la consulta necesaria (confirmado: `IShipmentRepository`/`IOrderRepository` no exponen métodos propios, solo heredan `IBaseRepository<T>`).

## Complexity Tracking

*No aplica — sin violaciones de la Constitution Check.*
