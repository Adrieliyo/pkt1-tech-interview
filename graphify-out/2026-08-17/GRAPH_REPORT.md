# Graph Report - ShipmentTracker  (2026-08-17)

## Corpus Check
- 159 files · ~93,727 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 815 nodes · 1439 edges · 43 communities (41 shown, 2 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 47 edges (avg confidence: 0.79)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `402911a5`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- CLAUDE.md
- BranchSchedule
- ShipmentTracker.Core.DTOs
- User Story 1: Register a new branch (P1)
- BranchDto
- /graphify Skill
- /speckit-specify Command
- ShipmentDto
- ShipmentTracker.Infrastructure.Migrations
- http
- ShipmentTracker.Web
- BaseRepository
- IBaseRepository
- User Story 2: Find and review branches (P2)
- common.ps1
- create-new-feature.ps1
- EmployeeDto
- VehicleDto
- User Scenarios & Testing *(mandatory)*
- Tasks: Employees & Vehicles Module
- Research: Employees & Vehicles Module
- Empleados
- 2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de `spec.md`)
- UnitOfWork
- Branch Entity
- BranchSchedule Entity
- Data Model: Employees & Vehicles Module
- ShipmentTracker.Core.Entities
- Branch
- Shipment
- Core Principles
- Employee
- Tasks: Branches & Hubs Module
- Vehicle
- UpdateBranchDto
- AppDbContext
- BranchScheduleConfiguration
- /speckit-constitution Command
- .claude/CLAUDE.md

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.DTOs` - 39 edges
2. `ShipmentTracker.Core.Enums` - 32 edges
3. `ShipmentTracker.Core.Entities` - 28 edges
4. `BaseRepository` - 19 edges
5. `Tasks: Employees & Vehicles Module` - 19 edges
6. `BranchDto` - 17 edges
7. `IBaseRepository` - 17 edges
8. `Research: Employees & Vehicles Module` - 17 edges
9. `001 Implementation Plan` - 17 edges
10. `FluentValidation` - 17 edges

## Surprising Connections (you probably didn't know these)
- `Shipment List Pagination Mechanism` --semantically_similar_to--> `Per-Property JSON Enum Serialization Gotcha`  [INFERRED] [semantically similar]
  specs/002-paginate-shipment-list/spec.md → CLAUDE.md
- `CreateVehicleDtoValidator` --references--> `CreateVehicleDto`  [EXTRACTED]
  ShipmentTracker.Services/Validators/CreateVehicleDtoValidator.cs → ShipmentTracker.Core/DTOs/CreateVehicleDto.cs
- `UpdateVehicleDtoValidator` --references--> `UpdateVehicleDto`  [EXTRACTED]
  ShipmentTracker.Services/Validators/UpdateVehicleDtoValidator.cs → ShipmentTracker.Core/DTOs/UpdateVehicleDto.cs
- `AppDbContext` --references--> `Branch`  [EXTRACTED]
  ShipmentTracker.Infrastructure/Data/AppDbContext.cs → ShipmentTracker.Core/Entities/Branch.cs
- `BranchConfiguration` --references--> `Branch`  [EXTRACTED]
  ShipmentTracker.Infrastructure/Data/Configurations/BranchConfiguration.cs → ShipmentTracker.Core/Entities/Branch.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Graphify AST+Semantic Extraction & Merge Pipeline** — _claude_skills_graphify_skill_ast_extraction, _claude_skills_graphify_skill_semantic_extraction, _claude_skills_graphify_skill_extraction_cache, _claude_skills_graphify_skill_merge_extraction, _claude_skills_graphify_references_extraction_spec_extraction_spec [EXTRACTED 1.00]
- **Nullable Enum Omission-Detection Pattern** — specs_003_branches_hubs_research_decision1, shipmenttracker_core_dtos_createbranchdto_createbranchdto, shipmenttracker_core_dtos_scheduleentryinputdto_scheduleentryinputdto, shipmenttracker_core_enums_branchtype_branchtype, shipmenttracker_core_enums_scheduleday_scheduleday [EXTRACTED 1.00]
- **Full Weekly Schedule Replacement Flow** — shipmenttracker_core_entities_branchschedule_branchschedule, shipmenttracker_services_branchservice_branchservice, specs_003_branches_hubs_research_decision5, specs_003_branches_hubs_spec_fr009, specs_003_branches_hubs_contracts_branches_api_contract_put_branch [EXTRACTED 1.00]
- **Soft-Delete Deactivation Pattern** — shipmenttracker_core_entities_branch_branch, specs_003_branches_hubs_spec_fr011, specs_003_branches_hubs_spec_fr012, specs_003_branches_hubs_research_decision8, specs_003_branches_hubs_contracts_branches_api_contract_delete_branch [EXTRACTED 1.00]
- **Spec-Driven Development Workflow Pipeline** — _claude_skills_speckit_specify_skill_speckit_specify, _claude_skills_speckit_clarify_skill_speckit_clarify, _claude_skills_speckit_plan_skill_speckit_plan, _claude_skills_speckit_tasks_skill_speckit_tasks, _claude_skills_speckit_implement_skill_speckit_implement, _claude_skills_speckit_analyze_skill_speckit_analyze, _claude_skills_speckit_converge_skill_speckit_converge [EXTRACTED 1.00]
- **ShipmentService Constructor Dependency Evolution** — shipmenttracker_services_shipmentservice_shipmentservice, automapper, fluentvalidation, shipmenttracker_core_dtos_pagedresult_pagedresult [INFERRED 0.75]
- **Spec-Driven Development Artifact Set (Feature 001)** — specs_001_standardize_mapping_di_spec, specs_001_standardize_mapping_di_plan, specs_001_standardize_mapping_di_tasks, specs_001_standardize_mapping_di_research, specs_001_standardize_mapping_di_data_model [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 002)** — specs_002_paginate_shipment_list_spec, specs_002_paginate_shipment_list_plan, specs_002_paginate_shipment_list_tasks, specs_002_paginate_shipment_list_research, specs_002_paginate_shipment_list_data_model [INFERRED 0.85]

## Communities (43 total, 2 thin omitted)

### Community 0 - "CLAUDE.md"
Cohesion: 0.08
Nodes (60): Spec Kit Full SDD Cycle Workflow, Clean Architecture Layering, Project Constitution (.specify/memory/constitution.md), AllowReactApp CORS Policy, Constructor Dependency Injection Convention, .NET 8, graphify Knowledge Graph, Per-Property JSON Enum Serialization Gotcha (+52 more)

### Community 1 - "BranchSchedule"
Cohesion: 0.16
Nodes (8): ShipmentTracker.Infrastructure.Data.Configurations, IEntityTypeConfiguration, TimeOnly, BranchSchedule, EntityTypeBuilder, BranchConfiguration, EntityTypeBuilder, BranchScheduleConfiguration

### Community 2 - "ShipmentTracker.Core.DTOs"
Cohesion: 0.08
Nodes (19): AutoMapper, ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators, ShipmentTracker.Web.Mappers, ShipmentTracker.Services, ShipmentTracker.Services.Mappings, ShipmentTracker.Web.Controllers, ShipmentTracker.Core.Interfaces (+11 more)

### Community 3 - "User Story 1: Register a new branch (P1)"
Cohesion: 0.22
Nodes (13): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, BranchType Enum, ScheduleDay Enum, POST /api/branches, Decision 1: Nullable Type/DayOfWeek enums in input DTOs (+5 more)

### Community 4 - "BranchDto"
Cohesion: 0.07
Nodes (35): AbstractValidator, DateTime, List, BranchDto, List, CreateBranchDto, TimeOnly, ScheduleEntryDto (+27 more)

### Community 5 - "/graphify Skill"
Cohesion: 0.06
Nodes (38): graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server, Neo4j Export (+30 more)

### Community 6 - "/speckit-specify Command"
Cohesion: 0.09
Nodes (29): Confidence Score Rubric, Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section (+21 more)

### Community 7 - "ShipmentDto"
Cohesion: 0.11
Nodes (21): HttpPatch, CreateShipmentDto, IEnumerable, PagedResult, DateTime, ShipmentDto, ShipmentStatus, Task (+13 more)

### Community 8 - "ShipmentTracker.Infrastructure.Migrations"
Cohesion: 0.07
Nodes (17): ShipmentTracker.Infrastructure.Migrations, Migration, ModelSnapshot, MigrationBuilder, ModelBuilder, InitialModel, MigrationBuilder, ModelBuilder (+9 more)

### Community 9 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 10 - "ShipmentTracker.Web"
Cohesion: 0.11
Nodes (22): FluentValidation (11.9.0), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web, ShipmentTracker.Core (+14 more)

### Community 11 - "BaseRepository"
Cohesion: 0.18
Nodes (9): DbSet, Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask (+1 more)

### Community 12 - "IBaseRepository"
Cohesion: 0.18
Nodes (8): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository

### Community 13 - "User Story 2: Find and review branches (P2)"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 14 - "common.ps1"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

### Community 20 - "EmployeeDto"
Cohesion: 0.08
Nodes (30): ControllerBase, DateOnly, CreateEmployeeDto, DateOnly, DateTime, EmployeeDto, DateOnly, UpdateEmployeeDto (+22 more)

### Community 21 - "VehicleDto"
Cohesion: 0.10
Nodes (23): CreateVehicleDto, UpdateVehicleDto, DateTime, VehicleDto, VehicleType, Task, IVehicleService, IMapper (+15 more)

### Community 22 - "User Scenarios & Testing *(mandatory)*"
Cohesion: 0.05
Nodes (36): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Employees & Vehicles Module, Complexity Tracking, Constitution Check, Documentation (this feature) (+28 more)

### Community 23 - "Tasks: Employees & Vehicles Module"
Cohesion: 0.06
Nodes (34): Dentro de cada historia, Dentro de Foundational, Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4 (+26 more)

### Community 24 - "Research: Employees & Vehicles Module"
Cohesion: 0.11
Nodes (17): Decisión 10: `BranchId` NO se hace nullable (a diferencia de los enums), Decisión 11: Representación JSON de `EmployeeRole`/`VehicleType` — mismo patrón scopeado que `Branch`, Decisión 12: `BranchId` como filtro de query string — sin conversor especial, Decisión 13: Paginación reutilizada de `002` para ambos listados, Decisión 14: Sin colección inversa `Employee`/`Vehicle` en `Branch.cs`, Decisión 15: Reactivación vía `PUT` — mismo precedente que `Branch`, sin nueva clarificación, Decisión 1: `FirstName`/`LastName` en vez del `name` genérico de spec.md, Decisión 2: `Phone` en `Employee` — campo agregado en la fase de planificación (+9 more)

### Community 25 - "Empleados"
Cohesion: 0.12
Nodes (15): Contrato HTTP: API de Employees & Vehicles, `DELETE /api/employees/{id}`, `DELETE /api/vehicles/{id}`, Ejemplo, Empleados, `GET /api/employees`, `GET /api/employees/{id}`, `GET /api/vehicles` (+7 more)

### Community 26 - "2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de `spec.md`)"
Cohesion: 0.13
Nodes (14): 1. Compilar y levantar, 2. Escenarios a validar (mapean 1:1 con los Acceptance Scenarios de `spec.md`), 3. Verificación de que nada más cambió, Edge cases adicionales, Prerrequisitos, Quickstart: Validar el módulo de Employees & Vehicles, User Story 1 — Registrar un empleado (P1), User Story 2 — Buscar choferes disponibles en una sucursal (P2) (+6 more)

### Community 27 - "UnitOfWork"
Cohesion: 0.23
Nodes (8): IDisposable, IUnitOfWork, IEmployeeRepository, IVehicleRepository, Task, UnitOfWork, EmployeeRepository, VehicleRepository

### Community 28 - "Branch Entity"
Cohesion: 0.20
Nodes (12): Branch Entity, DELETE /api/branches/{id}, Decision 10: Latitude/Longitude as nullable double, validated only when present, Decision 8: DELETE is soft-delete only, no physical delete, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default, FR-010: Allow deactivating an active branch, FR-011: Deactivating an already-inactive branch is a no-op (+4 more)

### Community 29 - "BranchSchedule Entity"
Cohesion: 0.23
Nodes (12): BranchSchedule Entity, Edge Cases Checklist: Branches & Hubs Module, Decision 13: opensAt/closesAt require HH:mm:ss format, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 3: Closed-day vs schedule-times consistency, FR-004: Exactly one schedule entry per day of week (7 total), FR-005: Reject duplicate day entries in schedule, FR-006: Opening time strictly earlier than closing time (+4 more)

### Community 30 - "Data Model: Employees & Vehicles Module"
Cohesion: 0.17
Nodes (11): Data Model: Employees & Vehicles Module, Dependientes de base de datos (`EmployeeService`/`VehicleService`, research.md Decisión 5), DTOs, Entidad: `Employee`, Entidad: `Vehicle`, Enums, Estructurales (FluentValidation, `ShipmentTracker.Services/Validators/`), Flujo de `CreateEmployeeAsync` / `UpdateEmployeeAsync` (y análogo para `Vehicle`) (+3 more)

### Community 31 - "ShipmentTracker.Core.Entities"
Cohesion: 0.44
Nodes (4): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Core.Entities, ShipmentTracker.Infrastructure.Repositories

### Community 32 - "Branch"
Cohesion: 0.24
Nodes (7): ICollection, DateTime, Branch, Task, IBranchRepository, Task, BranchRepository

### Community 33 - "Shipment"
Cohesion: 0.24
Nodes (6): DateTime, Shipment, IShipmentRepository, EntityTypeBuilder, ShipmentConfiguration, ShipmentRepository

### Community 34 - "Core Principles"
Cohesion: 0.20
Nodes (9): Core Principles, Flujo de Trabajo de Desarrollo, Governance, I. Framework Objetivo Único (.NET 8.0), II. Integridad de la Arquitectura en Capas, III. Minimalismo de Dependencias, IV. Cambios Pequeños y Reversibles, Restricciones Técnicas y de Arquitectura (+1 more)

### Community 35 - "Employee"
Cohesion: 0.29
Nodes (5): DateOnly, DateTime, Employee, EntityTypeBuilder, EmployeeConfiguration

### Community 36 - "Tasks: Branches & Hubs Module"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 37 - "Vehicle"
Cohesion: 0.33
Nodes (4): DateTime, Vehicle, EntityTypeBuilder, VehicleConfiguration

### Community 38 - "UpdateBranchDto"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 39 - "AppDbContext"
Cohesion: 0.40
Nodes (4): DbContext, DbSet, ModelBuilder, AppDbContext

### Community 40 - "BranchScheduleConfiguration"
Cohesion: 0.67
Nodes (4): AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration

### Community 41 - "/speckit-constitution Command"
Cohesion: 0.67
Nodes (3): /speckit-constitution Command, Sync Impact Report, Constitution Template

## Knowledge Gaps
- **191 isolated node(s):** `net8.0`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.EntityFrameworkCore (8.0.*)`, `Microsoft.EntityFrameworkCore.Design (8.0.*)` (+186 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **2 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `FluentValidation` connect `ShipmentTracker.Core.DTOs` to `CLAUDE.md`?**
  _High betweenness centrality (0.118) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Infrastructure.Data` connect `ShipmentTracker.Core.Entities` to `ShipmentTracker.Infrastructure.Migrations`, `BranchSchedule`, `ShipmentTracker.Core.DTOs`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Why does `001 Implementation Plan` connect `CLAUDE.md` to `ShipmentTracker.Core.DTOs`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Sdk`, `net8.0` to the rest of the system?**
  _191 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `CLAUDE.md` be split into smaller, more focused modules?**
  _Cohesion score 0.08306010928961749 - nodes in this community are weakly interconnected._
- **Should `ShipmentTracker.Core.DTOs` be split into smaller, more focused modules?**
  _Cohesion score 0.08408953418027829 - nodes in this community are weakly interconnected._
- **Should `BranchDto` be split into smaller, more focused modules?**
  _Cohesion score 0.07239819004524888 - nodes in this community are weakly interconnected._