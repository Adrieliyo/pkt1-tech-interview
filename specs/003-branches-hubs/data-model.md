# Data Model: Branches & Hubs Module

## Entidad: `Branch`

`ShipmentTracker.Core/Entities/Branch.cs`. Raíz del agregado; contiene su horario semanal completo
(`Schedule`).

| Campo | Tipo | Reglas | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `Name` | `string` | Requerido, no vacío | FR-001 |
| `Type` | `BranchType` (enum) | Requerido | FR-001 |
| `Address` | `string` | Requerido, no vacío (línea de calle) | FR-001 |
| `City` | `string` | Requerido, no vacío | FR-001 |
| `State` | `string` | Requerido, no vacío, **texto libre** (confirmado en Clarifications — no restringido a una lista fija de códigos) | FR-001 |
| `ZipCode` | `string` | Requerido, no vacío | FR-001 |
| `Latitude` | `double?` | Opcional; si tiene valor, `-90 <= x <= 90` | FR-002, FR-018 |
| `Longitude` | `double?` | Opcional; si tiene valor, `-180 <= x <= 180` | FR-002, FR-018 |
| `Phone` | `string?` | Opcional | FR-002 |
| `IsActive` | `bool` | `true` por defecto al crear; nunca se borra el registro | FR-003, FR-012 |
| `CreatedAt` | `DateTime` (UTC) | Asignado por el servicio al crear, no editable | — |
| `Schedule` | `ICollection<BranchSchedule>` | Exactamente 7 entradas, una por día, sin duplicados | FR-004, FR-005 |

## Entidad: `BranchSchedule`

`ShipmentTracker.Core/Entities/BranchSchedule.cs`. Hija del agregado `Branch` — no tiene
repositorio ni endpoint propio (research.md, Decisión 4).

| Campo | Tipo | Reglas | FR |
|---|---|---|---|
| `Id` | `int` (PK, identity) | — | — |
| `BranchId` | `int` (FK, requerido) | `NOT NULL`; relación requerida hacia `Branch` | — |
| `DayOfWeek` | `ScheduleDay` (enum) | Requerido; único por sucursal (índice compuesto) | FR-005 |
| `OpensAt` | `TimeOnly?` | `NULL` si `IsClosed`; si no, requerido y `< ClosesAt` | FR-006, FR-007, FR-017 |
| `ClosesAt` | `TimeOnly?` | `NULL` si `IsClosed`; si no, requerido y `> OpensAt` | FR-006, FR-007, FR-017 |
| `IsClosed` | `bool` | Si `true`, `OpensAt`/`ClosesAt` deben ser `NULL` | FR-006, FR-007, FR-017 |
| `Branch` | `Branch` (nav) | — | — |

**Relación**: `Branch` 1 — N `BranchSchedule`, requerida (`BranchId` no-nullable). Índice único
compuesto `(BranchId, DayOfWeek)` en `BranchScheduleConfiguration` — defensa en profundidad de
FR-005 a nivel de base de datos, además de la validación de FluentValidation (research.md,
Decisión 2).

## Enums

`ShipmentTracker.Core/Enums/BranchType.cs`:

```
Headquarters, Hub, SalesPoint, PickupPoint
```

`ShipmentTracker.Core/Enums/ScheduleDay.cs`:

```
Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday
```

Ambos se persisten como `string` (`HasConversion<string>()`), mismo patrón que `ShipmentStatus` en
`ShipmentConfiguration` — legible directamente en la base de datos, sin depender del orden
numérico de los valores del enum.

## DTOs

`ShipmentTracker.Core/DTOs/`:

| DTO | Uso | Campos |
|---|---|---|
| `BranchDto` | Salida (`POST`, `GET`, `GET/{id}`, `PUT`) | `Id`, `Name`, `Type`, `Address`, `City`, `State`, `ZipCode`, `Latitude?`, `Longitude?`, `Phone?`, `IsActive`, `CreatedAt`, `Schedule` (`List<ScheduleEntryDto>`) |
| `ScheduleEntryDto` | Salida, dentro de `BranchDto.Schedule` | `Id`, `DayOfWeek`, `IsClosed`, `OpensAt?`, `ClosesAt?` |
| `CreateBranchDto` | Entrada de `POST /api/branches` | `Name`, `Type?`, `Address`, `City`, `State`, `ZipCode`, `Latitude?`, `Longitude?`, `Phone?`, `Schedule` (`List<ScheduleEntryInputDto>`) — **sin** `IsActive` (siempre `true` al crear, FR-003) |
| `UpdateBranchDto` | Entrada de `PUT /api/branches/{id}` | Igual que `CreateBranchDto` + `IsActive` (permite reactivar, US3 escenario 4) |
| `ScheduleEntryInputDto` | Entrada, dentro de `Schedule` de Create/Update | `DayOfWeek?`, `IsClosed`, `OpensAt?`, `ClosesAt?` — **sin** `Id` (el horario siempre se reemplaza completo, no se referencia por id) |

`Type` en `CreateBranchDto`/`UpdateBranchDto` y `DayOfWeek` en `ScheduleEntryInputDto` son
*nullable* (`BranchType?`, `ScheduleDay?`) para distinguir "omitido" de "`Headquarters`/`Monday`
explícito" (research.md, Decisión 1).

## Reglas de validación (FluentValidation, invocadas a mano desde `BranchService`)

`ShipmentTracker.Services/Validators/`:

| Validador | Regla | FR |
|---|---|---|
| `ScheduleEntryInputDtoValidator` | `DayOfWeek` no nulo y válido (`IsInEnum`) | FR-005 (base) |
| | Si `IsClosed == false`: `OpensAt`/`ClosesAt` requeridos y `OpensAt < ClosesAt` | FR-006 |
| | Si `IsClosed == true`: `OpensAt`/`ClosesAt` deben ser `null` (si no, error de inconsistencia) | FR-007, FR-017 |
| `CreateBranchDtoValidator` / `UpdateBranchDtoValidator` | `Name`, `Address`, `City`, `State`, `ZipCode`: no vacíos | FR-001 |
| | `Type` no nulo y válido (`IsInEnum`) | FR-001 |
| | `Latitude`: si tiene valor, `[-90, 90]` | FR-002, FR-018 |
| | `Longitude`: si tiene valor, `[-180, 180]` | FR-002, FR-018 |
| | `Schedule.Count == 7` | FR-004 |
| | `Schedule` sin `DayOfWeek` repetidos | FR-005 |
| | Cada entrada de `Schedule` válida vía `ScheduleEntryInputDtoValidator` (`RuleForEach(...).SetValidator(...)`) | FR-006, FR-007, FR-017 |

Si la validación falla, el servicio lanza `FluentValidation.ValidationException` con **todos** los
errores encontrados (research.md, Decisión 9) — el controlador la traduce a `400` con la lista
completa, no solo el primer error.

## Interfaces nuevas

`ShipmentTracker.Core/Interfaces/`:

```csharp
public interface IBranchRepository : IBaseRepository<Branch>
{
    Task<Branch?> GetByIdWithScheduleAsync(int id); // Include(x => x.Schedule)
}

public interface IBranchService
{
    Task<IEnumerable<BranchDto>> GetBranchesAsync(bool onlyActive = true, BranchType? type = null);
    Task<BranchDto?> GetBranchByIdAsync(int id);           // null → 404
    Task<BranchDto> CreateBranchAsync(CreateBranchDto dto); // lanza ValidationException si es inválido
    Task<BranchDto?> UpdateBranchAsync(int id, UpdateBranchDto dto); // null → 404; lanza ValidationException si es inválido
    Task<bool> DeactivateBranchAsync(int id);               // false → 404; idempotente si ya estaba inactiva
}
```

`IUnitOfWork` gana `IBranchRepository BranchRepository { get; }` (mismo patrón que
`ShipmentRepository`).

## Flujo de reemplazo de horario en `UpdateBranchAsync`

1. `branch = await _unitOfWork.BranchRepository.GetByIdWithScheduleAsync(id)` — si `null`, 404.
2. Validar `UpdateBranchDto` completo (incluye las 7 entradas de `Schedule`); si inválido, lanzar
   `ValidationException` **antes** de tocar `branch` (ninguna escritura parcial, FR-009).
3. Actualizar los campos escalares (`Name`, `Type`, `Address`, `City`, `State`, `ZipCode`,
   `Latitude`, `Longitude`, `Phone`, `IsActive`) directamente sobre la entidad rastreada.
4. `branch.Schedule.Clear()`, luego agregar 7 `BranchSchedule` nuevas construidas desde
   `dto.Schedule`.
5. `_unitOfWork.BranchRepository.Update(branch)` (mismo patrón que `ShipmentService`, aunque la
   entidad ya está rastreada) + `CommitAsync()` — EF Core genera los `DELETE` de las 7 filas
   huérfanas y los `INSERT` de las 7 nuevas en la misma transacción (research.md, Decisión 5).

## Migración

Se requiere una migración EF Core nueva (`dotnet ef migrations add AddBranchesAndSchedule`, fase
de implementación) que crea las tablas `Branches` y `BranchSchedules` con la FK y el índice único
compuesto descritos arriba. No modifica ninguna tabla existente (`Shipments` no cambia).
