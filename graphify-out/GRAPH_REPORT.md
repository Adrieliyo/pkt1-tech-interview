# Graph Report - ShipmentTracker  (2026-08-17)

## Corpus Check
- 160 files · ~93,753 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 802 nodes · 1432 edges · 55 communities (44 shown, 11 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 47 edges (avg confidence: 0.79)
- Token cost: 148,297 input · 16,477 output

## Community Hubs (Navigation)
- Core Validators & DTOs Namespaces
- SDD Workflow & Conventions
- Employee DTO Date Fields
- graphify Add & Export Tools
- Vehicle DTOs & Controller
- Shipment Pagination & DTOs
- Branch DTOs & Lists
- EF Core Migrations
- LINQ Expression Generics
- Speckit Analyze & Checklist
- Constitution & SDD Governance
- Launch Settings Config
- NuGet Dependencies & Solution
- Vehicle Endpoints & FK Rules
- Spec Kit PowerShell Helpers
- Employee/Vehicle Create Validators
- Branch JSON & Time Decisions
- Branch Repository Interface
- Branch EF Configurations
- Employee Create Decisions
- Branch Listing & Retrieval
- Base Repository Infrastructure
- Employee Repository Interfaces
- Branch Entity & Migration
- Vehicle Service & JSON Gotcha
- Shipment Entity & Repository
- Vehicle Entity & Repository
- Employee Entity & Migration
- Employee Update & Retrieval
- Branches & Hubs Spec Docs
- Employees/Vehicles List Endpoints
- Employee/Vehicle Repo + UoW
- Branch DTOs & Enums
- Branch Schedule Consistency Rules
- Employee Entity Configuration
- Branch Update Flow
- Feature-Branch Creation Script
- Branch Soft-Delete Rules
- Cross-Module Pagination Summary
- EF Core DbContext Basics
- Solution Project Layers
- AutoMapper Output-Only Convention
- CORS Origin Allowlist
- String-Persisted Enums
- graphify Knowledge Graph Tool
- Vehicle Active-By-Default Rule
- Not-Found Error Handling
- Employee Registration Success Criterion
- Driver Lookup Success Criterion
- Branch Validity Success Criterion
- Fleet Visibility Success Criterion

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.Enums` - 32 edges
2. `ShipmentTracker.Core.Entities` - 28 edges
3. `BaseRepository` - 19 edges
4. `Vehicle Entity` - 18 edges
5. `BranchDto` - 17 edges
6. `IBaseRepository` - 17 edges
7. `Employee Entity` - 17 edges
8. `EmployeeDto` - 16 edges
9. `ShipmentTracker Constitution` - 16 edges
10. `VehicleDto` - 15 edges

## Surprising Connections (you probably didn't know these)
- `Per-Property JsonStringEnumConverter Gotcha` --semantically_similar_to--> `Decisión 12: BranchId/role como filtro de query string sin conversor especial`  [INFERRED] [semantically similar]
  CLAUDE.md → specs/004-employees-vehicles/research.md
- `Tasks Template` --conceptually_related_to--> `001 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/001-standardize-mapping-di/tasks.md
- `Tasks Template` --conceptually_related_to--> `002 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/002-paginate-shipment-list/tasks.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `001 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/001-standardize-mapping-di/spec.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `002 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/002-paginate-shipment-list/spec.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Graphify AST+Semantic Extraction & Merge Pipeline** — _claude_skills_graphify_skill_ast_extraction, _claude_skills_graphify_skill_semantic_extraction, _claude_skills_graphify_skill_extraction_cache, _claude_skills_graphify_skill_merge_extraction, _claude_skills_graphify_references_extraction_spec_extraction_spec [EXTRACTED 1.00]
- **Spec-Driven Development Workflow Pipeline** — _claude_skills_speckit_specify_skill_speckit_specify, _claude_skills_speckit_clarify_skill_speckit_clarify, _claude_skills_speckit_plan_skill_speckit_plan, _claude_skills_speckit_tasks_skill_speckit_tasks, _claude_skills_speckit_implement_skill_speckit_implement, _claude_skills_speckit_analyze_skill_speckit_analyze, _claude_skills_speckit_converge_skill_speckit_converge [EXTRACTED 1.00]
- **ShipmentTracker Constitution Core Principles** — _specify_memory_constitution_principle_i_framework_unico, _specify_memory_constitution_principle_ii_arquitectura_capas, _specify_memory_constitution_principle_iii_minimalismo_dependencias, _specify_memory_constitution_principle_iv_cambios_pequenos [EXTRACTED 1.00]
- **Spec-Driven Development Artifact Set (Feature 001)** — specs_001_standardize_mapping_di_spec, specs_001_standardize_mapping_di_plan, specs_001_standardize_mapping_di_tasks, specs_001_standardize_mapping_di_research, specs_001_standardize_mapping_di_data_model [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 002)** — specs_002_paginate_shipment_list_spec, specs_002_paginate_shipment_list_plan, specs_002_paginate_shipment_list_tasks, specs_002_paginate_shipment_list_research, specs_002_paginate_shipment_list_data_model [INFERRED 0.85]
- **ShipmentService Constructor Dependency Evolution** — shipmenttracker_core_dtos_pagedresult_pagedresult [INFERRED 0.75]
- **Full Weekly Schedule Replacement Flow** — shipmenttracker_core_entities_branchschedule_branchschedule, shipmenttracker_services_branchservice_branchservice, specs_003_branches_hubs_research_decision5, specs_003_branches_hubs_spec_fr009, specs_003_branches_hubs_contracts_branches_api_contract_put_branch [EXTRACTED 1.00]
- **Soft-Delete Deactivation Pattern** — shipmenttracker_core_entities_branch_branch, specs_003_branches_hubs_spec_fr011, specs_003_branches_hubs_spec_fr012, specs_003_branches_hubs_research_decision8, specs_003_branches_hubs_contracts_branches_api_contract_delete_branch [EXTRACTED 1.00]
- **Nullable Enum Omission-Detection Pattern** — specs_003_branches_hubs_research_decision1, shipmenttracker_core_dtos_createbranchdto_createbranchdto, shipmenttracker_core_dtos_scheduleentryinputdto_scheduleentryinputdto, shipmenttracker_core_enums_branchtype_branchtype, shipmenttracker_core_enums_scheduleday_scheduleday [EXTRACTED 1.00]
- **Employee Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_plan_employeerepository, specs_004_employees_vehicles_plan_employeeservice, specs_004_employees_vehicles_plan_employeecontroller, specs_004_employees_vehicles_data_model_employeedto [INFERRED 0.95]
- **Vehicle Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_vehicle, specs_004_employees_vehicles_plan_vehiclerepository, specs_004_employees_vehicles_plan_vehicleservice, specs_004_employees_vehicles_plan_vehiclecontroller, specs_004_employees_vehicles_data_model_vehicledto [INFERRED 0.95]
- **Employee and Vehicle Share Branch FK Without Direct Relation** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_data_model_vehicle, specs_003_branches_hubs [EXTRACTED 1.00]

## Communities (55 total, 11 thin omitted)

### Community 0 - "Core Validators & DTOs Namespaces"
Cohesion: 0.05
Nodes (36): AbstractValidator, ShipmentTracker.Core.DTOs.Employees, ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators.Vehicles, ShipmentTracker.Core.Entities, ShipmentTracker.Core.DTOs.Shipments, ShipmentTracker.Core.DTOs.Branches, ShipmentTracker.Services.Validators.Employees (+28 more)

### Community 1 - "SDD Workflow & Conventions"
Cohesion: 0.09
Nodes (49): Spec Kit Full SDD Cycle Workflow, AllowReactApp CORS Policy, Manual (Swagger/HTTP) Verification Policy, Medium Clean Architecture Article, Shipment List Pagination Mechanism, README, Shipment Status Transition State Machine, CreateShipmentDto (+41 more)

### Community 2 - "Employee DTO Date Fields"
Cohesion: 0.09
Nodes (27): DateOnly, CreateEmployeeDto, DateOnly, DateTime, EmployeeDto, DateOnly, UpdateEmployeeDto, EmployeeRole (+19 more)

### Community 3 - "graphify Add & Export Tools"
Cohesion: 0.06
Nodes (39): CLAUDE.md Graphify Directive, graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server (+31 more)

### Community 4 - "Vehicle DTOs & Controller"
Cohesion: 0.10
Nodes (24): ControllerBase, CreateVehicleDto, UpdateVehicleDto, DateTime, VehicleDto, VehicleType, Task, IVehicleService (+16 more)

### Community 5 - "Shipment Pagination & DTOs"
Cohesion: 0.10
Nodes (23): HttpPatch, IEnumerable, PagedResult, CreateShipmentDto, DateTime, ShipmentDto, ShipmentStatus, Task (+15 more)

### Community 6 - "Branch DTOs & Lists"
Cohesion: 0.10
Nodes (24): DateTime, List, BranchDto, List, CreateBranchDto, List, UpdateBranchDto, BranchType (+16 more)

### Community 7 - "EF Core Migrations"
Cohesion: 0.07
Nodes (17): ShipmentTracker.Infrastructure.Migrations, Migration, ModelSnapshot, MigrationBuilder, ModelBuilder, InitialModel, MigrationBuilder, ModelBuilder (+9 more)

### Community 8 - "LINQ Expression Generics"
Cohesion: 0.09
Nodes (17): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository (+9 more)

### Community 9 - "Speckit Analyze & Checklist"
Cohesion: 0.08
Nodes (38): Confidence Score Rubric, Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section (+30 more)

### Community 10 - "Constitution & SDD Governance"
Cohesion: 0.07
Nodes (31): .specify/memory/constitution.md, Spec-Driven Development (Spec Kit), Constitution Check (Plan Gate), Quickstart: Validar el módulo de Employees & Vehicles, Decisión 8: Trim de campos usados en unicidad antes de validar/guardar, Edge Cases (spec.md), FR-006: Deactivate an active Employee, FR-007: Deactivating already-inactive Employee is a no-op (+23 more)

### Community 11 - "Launch Settings Config"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 12 - "NuGet Dependencies & Solution"
Cohesion: 0.11
Nodes (22): FluentValidation (11.9.0), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web, ShipmentTracker.Core (+14 more)

### Community 13 - "Vehicle Endpoints & FK Rules"
Cohesion: 0.17
Nodes (16): FK Restrict + Unidirectional Navigation Convention, DELETE /api/vehicles/{id}, GET /api/vehicles/{id}, POST /api/vehicles, PUT /api/vehicles/{id}, CreateVehicleDto, IVehicleService, UpdateVehicleDto (+8 more)

### Community 14 - "Spec Kit PowerShell Helpers"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

### Community 15 - "Employee/Vehicle Create Validators"
Cohesion: 0.16
Nodes (14): CreateEmployeeDtoValidator, CreateVehicleDtoValidator, UpdateEmployeeDtoValidator, UpdateVehicleDtoValidator, CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow, EmployeeController, EmployeeService, Decisión 6: Método privado compartido ValidateBusinessRulesAsync para Create/Update (+6 more)

### Community 16 - "Branch JSON & Time Decisions"
Cohesion: 0.21
Nodes (13): ShipmentDto, Edge Cases Checklist: Branches & Hubs Module, POST /api/branches, Decision 12: JSON representation of BranchType/ScheduleDay as enum name, scoped converter, Decision 13: opensAt/closesAt require HH:mm:ss format, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 9: Validation errors via FluentValidation.ValidationException with full error list, FR-001: Create branch with name, type, full address (+5 more)

### Community 17 - "Branch Repository Interface"
Cohesion: 0.21
Nodes (8): ICollection, DateTime, Branch, Task, IBranchRepository, EntityTypeBuilder, Task, BranchRepository

### Community 18 - "Branch EF Configurations"
Cohesion: 0.22
Nodes (7): ShipmentTracker.Infrastructure.Data.Configurations, IEntityTypeConfiguration, TimeOnly, BranchSchedule, BranchConfiguration, EntityTypeBuilder, BranchScheduleConfiguration

### Community 19 - "Employee Create Decisions"
Cohesion: 0.22
Nodes (11): 003-branches-hubs Module (Branch entity), Specification Quality Checklist: Employees & Vehicles Module, POST /api/employees, CreateEmployeeDto, Decisión 10: BranchId NO se hace nullable, Decisión 11: JsonStringEnumConverter por propiedad para Role/Type, Decisión 15: Reactivación vía PUT (mismo precedente que Branch), Decisión 7: Unicidad sin filtrar por IsActive (índice único simple) (+3 more)

### Community 20 - "Branch Listing & Retrieval"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 21 - "Base Repository Infrastructure"
Cohesion: 0.42
Nodes (3): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Infrastructure.Repositories

### Community 22 - "Employee Repository Interfaces"
Cohesion: 0.24
Nodes (6): IDisposable, IUnitOfWork, IEmployeeRepository, Task, UnitOfWork, EmployeeRepository

### Community 23 - "Branch Entity & Migration"
Cohesion: 0.22
Nodes (10): Branch Entity, AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration, Decision 10: Latitude/Longitude as nullable double, validated only when present, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default (+2 more)

### Community 24 - "Vehicle Service & JSON Gotcha"
Cohesion: 0.22
Nodes (9): Per-Property JsonStringEnumConverter Gotcha, VehicleController, VehicleService, Decisión 12: BranchId/role como filtro de query string sin conversor especial, FR-011: Create Vehicle with unique plate, type, brand, model, year, capacity, branch, FR-012: Reject Vehicle create/update on duplicate plate (company-wide, incl. inactive), FR-013: Reject Vehicle create/update if Branch missing/inactive, US3: Register a new vehicle (P3) (+1 more)

### Community 25 - "Shipment Entity & Repository"
Cohesion: 0.28
Nodes (6): DateTime, Shipment, IShipmentRepository, EntityTypeBuilder, ShipmentConfiguration, ShipmentRepository

### Community 26 - "Vehicle Entity & Repository"
Cohesion: 0.28
Nodes (6): DateTime, Vehicle, IVehicleRepository, EntityTypeBuilder, VehicleConfiguration, VehicleRepository

### Community 27 - "Employee Entity & Migration"
Cohesion: 0.31
Nodes (9): DELETE /api/employees/{id}, Employee Entity, EmployeeRole Enum, AddEmployeesAndVehicles EF Migration, EmployeeConfiguration, Decisión 1: FirstName/LastName en vez de name genérico, Decisión 14: Sin colección inversa Employee/Vehicle en Branch.cs, Decisión 2: Phone agregado en Employee en fase de planificación (+1 more)

### Community 28 - "Employee Update & Retrieval"
Cohesion: 0.25
Nodes (9): GET /api/employees/{id}, PUT /api/employees/{id}, EmployeeDto, IEmployeeService, UpdateEmployeeDto, EmployeeMappingProfile, FR-005: Update Employee fields incl. Branch reassignment, unrestricted role change, US5: Update employee information (P5) (+1 more)

### Community 29 - "Branches & Hubs Spec Docs"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 30 - "Employees/Vehicles List Endpoints"
Cohesion: 0.33
Nodes (7): FluentValidation Structural-Only + Service DB Rules, 002-paginate-shipment-list Module (Pagination Contract), GET /api/employees, GET /api/vehicles, Implementation Plan: Employees & Vehicles Module, Decisión 4: BranchId como int en ambas entidades (corrección de guid), Decisión 5: Validación estructural (FluentValidation) + validación dependiente de BD (Service)

### Community 31 - "Employee/Vehicle Repo + UoW"
Cohesion: 0.43
Nodes (7): Repository + Unit of Work Pattern, IEmployeeRepository, IUnitOfWork (extended), IVehicleRepository, EmployeeRepository, UnitOfWork (class, extended), VehicleRepository

### Community 32 - "Branch DTOs & Enums"
Cohesion: 0.38
Nodes (7): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, BranchType Enum, ScheduleDay Enum, Decision 1: Nullable Type/DayOfWeek enums in input DTOs

### Community 33 - "Branch Schedule Consistency Rules"
Cohesion: 0.33
Nodes (7): BranchSchedule Entity, Decision 3: Closed-day vs schedule-times consistency, Decision 5: Full schedule replacement via EF Core cascade delete, FR-006: Opening time strictly earlier than closing time, FR-007: Schedule entry may be marked closed with no times, FR-017: Reject inconsistent closed+times schedule submission, FR-019: Schedule times are branch-local time-of-day, no time zone

### Community 34 - "Employee Entity Configuration"
Cohesion: 0.33
Nodes (5): DateOnly, DateTime, Employee, EntityTypeBuilder, EmployeeConfiguration

### Community 35 - "Branch Update Flow"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 37 - "Branch Soft-Delete Rules"
Cohesion: 0.47
Nodes (6): DELETE /api/branches/{id}, Decision 8: DELETE is soft-delete only, no physical delete, FR-011: Deactivating an already-inactive branch is a no-op, FR-012: No capability to permanently delete a branch, SC-004: Deactivated branches retain 100% of data with zero loss, User Story 4: Deactivate a branch (P4)

### Community 38 - "Cross-Module Pagination Summary"
Cohesion: 0.50
Nodes (5): Branch/BranchSchedule Module (Branches & Hubs), Employee/Vehicle Module (Employees & Vehicles), List Endpoint Pagination Convention, Shipment Module, Decisión 13: Paginación reutilizada del módulo 002 para ambos listados

### Community 39 - "EF Core DbContext Basics"
Cohesion: 0.40
Nodes (4): DbContext, DbSet, ModelBuilder, AppDbContext

### Community 40 - "Solution Project Layers"
Cohesion: 0.83
Nodes (4): ShipmentTracker.Core, ShipmentTracker.Infrastructure, ShipmentTracker.Services, ShipmentTracker.Web

## Knowledge Gaps
- **128 isolated node(s):** `net8.0`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.EntityFrameworkCore (8.0.*)`, `Microsoft.EntityFrameworkCore.Design (8.0.*)` (+123 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **11 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ShipmentTracker.Infrastructure.Data` connect `Base Repository Infrastructure` to `Core Validators & DTOs Namespaces`, `Branch EF Configurations`, `EF Core Migrations`?**
  _High betweenness centrality (0.045) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Enums` connect `Core Validators & DTOs Namespaces` to `Employee DTO Date Fields`, `Vehicle DTOs & Controller`, `Shipment Pagination & DTOs`, `Branch DTOs & Lists`?**
  _High betweenness centrality (0.032) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Entities` connect `Core Validators & DTOs Namespaces` to `Employee Entity Configuration`, `Branch Repository Interface`, `Branch EF Configurations`, `Base Repository Infrastructure`, `Employee Repository Interfaces`, `Shipment Entity & Repository`, `Vehicle Entity & Repository`?**
  _High betweenness centrality (0.031) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Sdk`, `net8.0` to the rest of the system?**
  _128 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Core Validators & DTOs Namespaces` be split into smaller, more focused modules?**
  _Cohesion score 0.05322947095098994 - nodes in this community are weakly interconnected._
- **Should `SDD Workflow & Conventions` be split into smaller, more focused modules?**
  _Cohesion score 0.09438775510204081 - nodes in this community are weakly interconnected._
- **Should `Employee DTO Date Fields` be split into smaller, more focused modules?**
  _Cohesion score 0.09090909090909091 - nodes in this community are weakly interconnected._