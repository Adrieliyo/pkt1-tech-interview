# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## Project overview

.NET 8 REST API (`ShipmentTracker.sln`) managing shipments for a parcel delivery company, built in layers following Clean Architecture. Domain modules so far: **Shipment** (tracking, status transitions), **Branch/BranchSchedule** ("Branches & Hubs" — physical locations and their weekly operating hours), and **Employee/Vehicle** ("Employees & Vehicles" — operational staff and fleet, each referencing `Branch` by FK).

## Commands

```bash
# Build the whole solution
dotnet build ShipmentTracker.sln

# Run the API (Swagger UI at /swagger in Development)
dotnet run --project ShipmentTracker.Web

# Create the database (once), then apply migrations
# (connection string: ShipmentTracker.Web/appsettings.json -> ConnectionStrings:DefaultConnection)
dotnet ef database update --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web

# Add a new migration after changing an entity/EF configuration
dotnet ef migrations add <Name> --project ShipmentTracker.Infrastructure --startup-project ShipmentTracker.Web
```

There is no automated test project in this solution — validation is done manually via Swagger/HTTP. Each feature under `specs/` has a `quickstart.md` with the manual scenarios to run against the live API.

## Architecture

Four projects, one-directional dependency flow — never invert these:

- **`ShipmentTracker.Core`**: entities, DTOs, enums, interfaces (`Interfaces/Repositories`, `Interfaces/Services`). No dependencies on any other project in the solution.
- **`ShipmentTracker.Infrastructure`**: EF Core `AppDbContext`, entity configurations (`Data/Configurations`), migrations, repository implementations. Depends only on `Core`.
- **`ShipmentTracker.Services`**: business logic (`*Service` classes), FluentValidation validators (`Validators/`), AutoMapper profiles (`Mappings/`). Depends only on `Core`.
- **`ShipmentTracker.Web`**: composition root — controllers, `Program.cs` (all DI registration and CORS/Swagger setup). The only project allowed to depend on all three of the above.

Each domain module (e.g. `Shipment`, `Branch`, `Employee`, `Vehicle`) repeats the same shape across the four layers: entity + DTOs + `I<X>Repository`/`I<X>Service` in Core, an `<X>Repository : BaseRepository<X>` in Infrastructure, an `<X>Service` + validators + a mapping profile in Services, and an `<X>Controller` in Web. A feature can bundle more than one such module (e.g. "Employees & Vehicles" is `Employee` + `Vehicle` as two independent tracks that only share a FK to `Branch` — each still gets its own full per-layer file set, and can be built/tested separately).

Established conventions — follow them rather than introducing a competing pattern for the same concern:

- **Repository + Unit of Work**: generic `IBaseRepository<T>`/`BaseRepository<T>` (supports filter/orderBy/paging via `GetAsync`). Each entity gets its own `I<Entity>Repository : IBaseRepository<Entity>`, exposed as a lazily-instantiated property on `IUnitOfWork`/`UnitOfWork` (e.g. `unitOfWork.BranchRepository`). No project other than `Infrastructure` touches `AppDbContext`/`DbSet` directly. If a repository needs no query beyond what the generic base already offers, leave its interface empty (`IEmployeeRepository : IBaseRepository<Employee> { }`) rather than adding methods speculatively.
- **AutoMapper is output-only**: `CreateMap<Entity, Dto>()` profiles map entity → response DTO. Creating an entity from an input DTO is done by hand in the `Service` method (see `ShipmentService.CreateShipmentAsync`, `BranchService.CreateBranchAsync`) — don't mix `_mapper.Map` into entity construction.
- **FluentValidation covers structural rules only** (required fields, formats, enum validity, numeric ranges) and is invoked manually inside `Service` methods (not wired into the MVC pipeline) — never as an async validator with a repository injected. Rules that need a database round-trip (a referenced entity must exist/be active, a field must be globally unique) are checked directly in the `Service`, *after* structural validation, with both sets of failures merged into one list. On failure the service throws (`FluentValidation.ValidationException` carrying the full merged error list for multi-field DTOs; a plain exception for single-rule checks like status transitions); the controller catches it and returns `400` with the error list. See `EmployeeService`/`VehicleService`'s shared private `ValidateBusinessRulesAsync(..., currentId)` helper — reused by both `Create` (`currentId: 0`, which never matches a real identity-generated id) and `Update` (`currentId: id`, to exclude the record being edited from its own uniqueness check) instead of duplicating the checks.
- **Enums are persisted as strings** (`HasConversion<string>()` in the entity configuration) so the database stays human-readable regardless of enum member order.
- **Foreign keys to entities that are never hard-deleted** (e.g. `BranchId` on `Employee`/`Vehicle`) use `.OnDelete(DeleteBehavior.Restrict)` and stay **unidirectional** — don't add a back-reference collection (`ICollection<Employee>`) to the referenced entity (`Branch`) unless a real use case needs to load "all children of X" through X itself; query the child's own repository filtered by the FK instead.
- **List endpoints are paginated** (`Shipment`, `Branch`, `Employee`, `Vehicle` all follow this): `PagedResult<T>` (`Core/DTOs/PagedResult.cs`), `page`/`pageSize` query params (`[Range(1, int.MaxValue)]`), a per-service `MaxPageSize = 50` clamp (not a `[Range]` upper bound — clamping, not rejecting), ordered `OrderByDescending(x => x.CreatedAt)`, and the same 4 response headers (`X-Total-Count`, `X-Page`, `X-Page-Size`, `X-Total-Pages`) already exposed once via CORS in `Program.cs`. Apply this to any new list endpoint by default.
- **Trim input strings used in uniqueness checks** (email, employee/plate numbers, etc.) at the top of the `Service` method, before validation — `dto.Email = dto.Email?.Trim() ?? string.Empty;` (the `?? string.Empty` avoids a nullable-warning on the non-nullable DTO property while still letting `NotEmpty()` correctly reject a null/omitted value instead of throwing).
- **CORS** in `Program.cs` is an explicit origin allowlist (`AllowReactApp`) — never replace with `AllowAnyOrigin`.

### JSON enum serialization gotcha

There is **no global** `JsonStringEnumConverter`. `ShipmentStatus` therefore serializes as a plain number in `Shipment` endpoints (existing, unchanged behavior). The `Branch`/`BranchSchedule`/`Employee`/`Vehicle` DTOs instead apply `[JsonConverter(typeof(JsonStringEnumConverter))]` **per property** on their enum fields (`Type`, `DayOfWeek`, `Role`, ...), so those specific fields accept/return enum names (e.g. `"Hub"`, `"Driver"`) without affecting `Shipment`'s contract. Do the same (per-property, not global) if a new enum needs string JSON representation. Also note: `TimeOnly` fields (`opensAt`/`closesAt`) require `HH:mm:ss` in JSON — `HH:mm` without seconds fails deserialization.

## Spec-driven development

Features are built via Spec Kit (`.specify/`, `/speckit-*` slash commands): each feature gets a `specs/<NNN-feature-name>/` directory with `spec.md` → `plan.md` → `tasks.md` (plus `research.md`, `data-model.md`, `contracts/`, `quickstart.md`, `checklists/`). `.specify/memory/constitution.md` is the authoritative governance document for this repo (framework/dependency/architecture/change-size rules) — consult it before making structural decisions; the summary above reflects it but isn't a substitute for reading it directly when in doubt.
