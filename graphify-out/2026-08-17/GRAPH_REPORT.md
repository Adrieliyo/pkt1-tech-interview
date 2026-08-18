# Graph Report - ShipmentTracker  (2026-08-17)

## Corpus Check
- 191 files · ~111,846 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1023 nodes · 1776 edges · 72 communities (60 shown, 12 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 56 edges (avg confidence: 0.79)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `20fbd4c8`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- ShipmentTracker.Core.Enums
- 002 Implementation Plan
- EmployeeDto
- /graphify Skill
- VehicleDto
- PagedResult
- BranchDto
- ShipmentTracker.Infrastructure.Migrations
- BaseRepository
- /speckit-specify Command
- Quickstart: Validar el módulo de Employees & Vehicles
- http
- ShipmentTracker.Web
- Vehicle Entity
- Spec Kit PowerShell Helpers
- EmployeeDto
- Edge Cases Checklist: Branches & Hubs Module
- Branch
- AppDbContext
- 003-branches-hubs Module (Branch entity)
- User Story 2: Find and review branches (P2)
- ShipmentTracker.Core.Entities
- UnitOfWork
- Branch Entity
- US3: Register a new vehicle (P3)
- Shipment
- Vehicle
- Employee Entity
- CustomerDetailDto
- Tasks: Branches & Hubs Module
- CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow
- Implementation Plan: Customers & Accounts Module
- BranchDto
- BranchSchedule Entity
- Employee
- UpdateBranchDto
- create-new-feature.ps1
- User Story 4: Deactivate a branch (P4)
- Employee/Vehicle Module (Employees & Vehicles)
- Tasks: Customers & Accounts Module
- ShipmentTracker.Core
- AutoMapper Output-Only Convention
- CORS Explicit Origin Allowlist
- Enums Persisted as Strings
- graphify Knowledge Graph Tool
- FR-014: New Vehicle active by default
- FR-025: Clear not-found result for nonexistent id
- SC-001: Register employee in a single submission
- SC-002: Find every active driver at a branch in a single lookup
- SC-003: 100% of Employees/Vehicles associated with a valid active Branch
- SC-004: Register vehicle and view complete active fleet at a branch
- IBaseRepository
- Customer
- Research: Customers & Accounts Module
- Data Model: Customers & Accounts Module
- US2: Find drivers available at a branch (P2)
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- Core Principles
- graphify add & --watch Reference
- Extraction Subagent Prompt Spec
- /speckit-plan Command
- Extra Exports & Benchmark Reference
- IndividualCustomer
- Query, Path, Explain Reference
- /speckit-tasks Command
- US7: Deactivate an employee (P7)
- /speckit-constitution Command
- CLAUDE.md

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.Entities` - 38 edges
2. `ShipmentTracker.Core.Enums` - 38 edges
3. `CustomerDetailDto` - 20 edges
4. `BaseRepository` - 20 edges
5. `IBaseRepository` - 18 edges
6. `Vehicle Entity` - 18 edges
7. `BranchDto` - 17 edges
8. `Employee Entity` - 17 edges
9. `EmployeeDto` - 16 edges
10. `ShipmentTracker.Core.Interfaces.Services` - 16 edges

## Surprising Connections (you probably didn't know these)
- `Per-Property JsonStringEnumConverter Gotcha` --semantically_similar_to--> `Decisión 12: BranchId/role como filtro de query string sin conversor especial`  [INFERRED] [semantically similar]
  CLAUDE.md → specs/004-employees-vehicles/research.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `001 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/001-standardize-mapping-di/spec.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `002 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/002-paginate-shipment-list/spec.md
- `Tasks Template` --conceptually_related_to--> `001 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/001-standardize-mapping-di/tasks.md
- `Tasks Template` --conceptually_related_to--> `002 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/002-paginate-shipment-list/tasks.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Employee and Vehicle Share Branch FK Without Direct Relation** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_data_model_vehicle, specs_003_branches_hubs [EXTRACTED 1.00]
- **Graphify AST+Semantic Extraction & Merge Pipeline** — _claude_skills_graphify_skill_ast_extraction, _claude_skills_graphify_skill_semantic_extraction, _claude_skills_graphify_skill_extraction_cache, _claude_skills_graphify_skill_merge_extraction, _claude_skills_graphify_references_extraction_spec_extraction_spec [EXTRACTED 1.00]
- **Nullable Enum Omission-Detection Pattern** — specs_003_branches_hubs_research_decision1, shipmenttracker_core_dtos_createbranchdto_createbranchdto, shipmenttracker_core_dtos_scheduleentryinputdto_scheduleentryinputdto, shipmenttracker_core_enums_branchtype_branchtype, shipmenttracker_core_enums_scheduleday_scheduleday [EXTRACTED 1.00]
- **Full Weekly Schedule Replacement Flow** — shipmenttracker_core_entities_branchschedule_branchschedule, shipmenttracker_services_branchservice_branchservice, specs_003_branches_hubs_research_decision5, specs_003_branches_hubs_spec_fr009, specs_003_branches_hubs_contracts_branches_api_contract_put_branch [EXTRACTED 1.00]
- **Soft-Delete Deactivation Pattern** — shipmenttracker_core_entities_branch_branch, specs_003_branches_hubs_spec_fr011, specs_003_branches_hubs_spec_fr012, specs_003_branches_hubs_research_decision8, specs_003_branches_hubs_contracts_branches_api_contract_delete_branch [EXTRACTED 1.00]
- **Spec-Driven Development Workflow Pipeline** — _claude_skills_speckit_specify_skill_speckit_specify, _claude_skills_speckit_clarify_skill_speckit_clarify, _claude_skills_speckit_plan_skill_speckit_plan, _claude_skills_speckit_tasks_skill_speckit_tasks, _claude_skills_speckit_implement_skill_speckit_implement, _claude_skills_speckit_analyze_skill_speckit_analyze, _claude_skills_speckit_converge_skill_speckit_converge [EXTRACTED 1.00]
- **ShipmentService Constructor Dependency Evolution** — shipmenttracker_core_dtos_pagedresult_pagedresult [INFERRED 0.75]
- **Spec-Driven Development Artifact Set (Feature 001)** — specs_001_standardize_mapping_di_spec, specs_001_standardize_mapping_di_plan, specs_001_standardize_mapping_di_tasks, specs_001_standardize_mapping_di_research, specs_001_standardize_mapping_di_data_model [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 002)** — specs_002_paginate_shipment_list_spec, specs_002_paginate_shipment_list_plan, specs_002_paginate_shipment_list_tasks, specs_002_paginate_shipment_list_research, specs_002_paginate_shipment_list_data_model [INFERRED 0.85]
- **Employee Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_plan_employeerepository, specs_004_employees_vehicles_plan_employeeservice, specs_004_employees_vehicles_plan_employeecontroller, specs_004_employees_vehicles_data_model_employeedto [INFERRED 0.95]
- **Vehicle Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_vehicle, specs_004_employees_vehicles_plan_vehiclerepository, specs_004_employees_vehicles_plan_vehicleservice, specs_004_employees_vehicles_plan_vehiclecontroller, specs_004_employees_vehicles_data_model_vehicledto [INFERRED 0.95]

## Communities (72 total, 12 thin omitted)

### Community 0 - "ShipmentTracker.Core.Enums"
Cohesion: 0.05
Nodes (39): AbstractValidator, ShipmentTracker.Core.DTOs.Employees, ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators.Customers, ShipmentTracker.Services.Validators.Vehicles, ShipmentTracker.Core.DTOs.Customers, ShipmentTracker.Core.DTOs.Shipments, ShipmentTracker.Core.DTOs.Branches (+31 more)

### Community 1 - "002 Implementation Plan"
Cohesion: 0.07
Nodes (58): Spec Kit Full SDD Cycle Workflow, AllowReactApp CORS Policy, Manual (Swagger/HTTP) Verification Policy, Medium Clean Architecture Article, Shipment List Pagination Mechanism, README, Shipment Status Transition State Machine, CreateShipmentDto (+50 more)

### Community 2 - "EmployeeDto"
Cohesion: 0.08
Nodes (29): ShipmentTracker.Services.Validators.Employees, DateOnly, CreateEmployeeDto, DateOnly, DateTime, EmployeeDto, DateOnly, UpdateEmployeeDto (+21 more)

### Community 3 - "/graphify Skill"
Cohesion: 0.17
Nodes (15): GitHub Clone & Cross-Repo Merge Reference, graphify clone, graphify merge-graphs, graphify claude install (CLAUDE.md integration), Commit Hook & CLAUDE.md Integration Reference, graphify hook install (post-commit hook), Transcribe Video/Audio Reference, Whisper Domain-Hint Prompt (+7 more)

### Community 4 - "VehicleDto"
Cohesion: 0.10
Nodes (25): ControllerBase, CreateVehicleDto, UpdateVehicleDto, DateTime, VehicleDto, VehicleType, Task, IVehicleService (+17 more)

### Community 5 - "PagedResult"
Cohesion: 0.10
Nodes (23): HttpPatch, IEnumerable, PagedResult, CreateShipmentDto, DateTime, ShipmentDto, ShipmentStatus, Task (+15 more)

### Community 6 - "BranchDto"
Cohesion: 0.09
Nodes (26): DateTime, List, BranchDto, List, CreateBranchDto, List, UpdateBranchDto, BranchType (+18 more)

### Community 7 - "ShipmentTracker.Infrastructure.Migrations"
Cohesion: 0.06
Nodes (20): ShipmentTracker.Infrastructure.Migrations, Migration, ModelSnapshot, MigrationBuilder, ModelBuilder, InitialModel, MigrationBuilder, ModelBuilder (+12 more)

### Community 8 - "BaseRepository"
Cohesion: 0.18
Nodes (9): DbSet, Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask (+1 more)

### Community 9 - "/speckit-specify Command"
Cohesion: 0.20
Nodes (11): /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section, /speckit-clarify Command, Checklist Status Gate, Spec Quality Checklist, /speckit-specify Command (+3 more)

### Community 10 - "Quickstart: Validar el módulo de Employees & Vehicles"
Cohesion: 0.11
Nodes (18): .specify/memory/constitution.md, Spec-Driven Development (Spec Kit), Constitution Check (Plan Gate), Quickstart: Validar el módulo de Employees & Vehicles, Decisión 8: Trim de campos usados en unicidad antes de validar/guardar, Edge Cases (spec.md), FR-005: Update Employee fields incl. Branch reassignment, unrestricted role change, FR-015: Update Vehicle fields incl. Branch reassignment (+10 more)

### Community 11 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 12 - "ShipmentTracker.Web"
Cohesion: 0.11
Nodes (22): FluentValidation (11.9.0), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web, ShipmentTracker.Core (+14 more)

### Community 13 - "Vehicle Entity"
Cohesion: 0.19
Nodes (14): DELETE /api/vehicles/{id}, GET /api/vehicles/{id}, PUT /api/vehicles/{id}, CreateVehicleDto, IVehicleService, UpdateVehicleDto, Vehicle Entity, VehicleDto (+6 more)

### Community 14 - "Spec Kit PowerShell Helpers"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

### Community 15 - "EmployeeDto"
Cohesion: 0.14
Nodes (17): Per-Property JsonStringEnumConverter Gotcha, GET /api/employees/{id}, PUT /api/employees/{id}, CreateEmployeeDtoValidator, EmployeeDto, IEmployeeService, UpdateEmployeeDto, EmployeeController (+9 more)

### Community 16 - "Edge Cases Checklist: Branches & Hubs Module"
Cohesion: 0.21
Nodes (13): ShipmentDto, Edge Cases Checklist: Branches & Hubs Module, POST /api/branches, Decision 12: JSON representation of BranchType/ScheduleDay as enum name, scoped converter, Decision 13: opensAt/closesAt require HH:mm:ss format, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 9: Validation errors via FluentValidation.ValidationException with full error list, FR-001: Create branch with name, type, full address (+5 more)

### Community 17 - "Branch"
Cohesion: 0.19
Nodes (9): ICollection, DateTime, Branch, Task, IBranchRepository, EntityTypeBuilder, BranchConfiguration, Task (+1 more)

### Community 18 - "AppDbContext"
Cohesion: 0.18
Nodes (8): DbContext, TimeOnly, BranchSchedule, DbSet, ModelBuilder, AppDbContext, EntityTypeBuilder, BranchScheduleConfiguration

### Community 19 - "003-branches-hubs Module (Branch entity)"
Cohesion: 0.20
Nodes (12): 003-branches-hubs Module (Branch entity), Specification Quality Checklist: Employees & Vehicles Module, POST /api/employees, POST /api/vehicles, CreateEmployeeDto, Decisión 10: BranchId NO se hace nullable, Decisión 11: JsonStringEnumConverter por propiedad para Role/Type, Decisión 15: Reactivación vía PUT (mismo precedente que Branch) (+4 more)

### Community 20 - "User Story 2: Find and review branches (P2)"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 21 - "ShipmentTracker.Core.Entities"
Cohesion: 0.28
Nodes (4): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Core.Entities, ShipmentTracker.Infrastructure.Repositories

### Community 22 - "UnitOfWork"
Cohesion: 0.21
Nodes (12): IDisposable, IUnitOfWork, ICustomerRepository, IEmployeeRepository, IShipmentRepository, IVehicleRepository, Task, UnitOfWork (+4 more)

### Community 23 - "Branch Entity"
Cohesion: 0.22
Nodes (10): Branch Entity, AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration, Decision 10: Latitude/Longitude as nullable double, validated only when present, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default (+2 more)

### Community 24 - "US3: Register a new vehicle (P3)"
Cohesion: 0.29
Nodes (7): VehicleController, VehicleService, FR-011: Create Vehicle with unique plate, type, brand, model, year, capacity, branch, FR-012: Reject Vehicle create/update on duplicate plate (company-wide, incl. inactive), FR-013: Reject Vehicle create/update if Branch missing/inactive, US3: Register a new vehicle (P3), Phase 5: User Story 3 Tasks (Register Vehicle)

### Community 25 - "Shipment"
Cohesion: 0.33
Nodes (4): DateTime, Shipment, EntityTypeBuilder, ShipmentConfiguration

### Community 26 - "Vehicle"
Cohesion: 0.33
Nodes (4): DateTime, Vehicle, EntityTypeBuilder, VehicleConfiguration

### Community 27 - "Employee Entity"
Cohesion: 0.18
Nodes (17): FK Restrict + Unidirectional Navigation Convention, Repository + Unit of Work Pattern, DELETE /api/employees/{id}, Employee Entity, EmployeeRole Enum, IEmployeeRepository, IUnitOfWork (extended), IVehicleRepository (+9 more)

### Community 28 - "CustomerDetailDto"
Cohesion: 0.08
Nodes (29): BusinessDetailDto, CreateBusinessCustomerDto, DateOnly, CreateIndividualCustomerDto, DateTime, CustomerDetailDto, DateOnly, IndividualDetailDto (+21 more)

### Community 29 - "Tasks: Branches & Hubs Module"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 30 - "CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow"
Cohesion: 0.20
Nodes (12): FluentValidation Structural-Only + Service DB Rules, 002-paginate-shipment-list Module (Pagination Contract), GET /api/employees, GET /api/vehicles, CreateVehicleDtoValidator, UpdateEmployeeDtoValidator, UpdateVehicleDtoValidator, CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow (+4 more)

### Community 31 - "Implementation Plan: Customers & Accounts Module"
Cohesion: 0.06
Nodes (31): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Customers & Accounts Module, Complexity Tracking, Constitution Check, Documentation (this feature) (+23 more)

### Community 32 - "BranchDto"
Cohesion: 0.38
Nodes (7): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, BranchType Enum, ScheduleDay Enum, Decision 1: Nullable Type/DayOfWeek enums in input DTOs

### Community 33 - "BranchSchedule Entity"
Cohesion: 0.33
Nodes (7): BranchSchedule Entity, Decision 3: Closed-day vs schedule-times consistency, Decision 5: Full schedule replacement via EF Core cascade delete, FR-006: Opening time strictly earlier than closing time, FR-007: Schedule entry may be marked closed with no times, FR-017: Reject inconsistent closed+times schedule submission, FR-019: Schedule times are branch-local time-of-day, no time zone

### Community 34 - "Employee"
Cohesion: 0.29
Nodes (6): IEntityTypeConfiguration, DateOnly, DateTime, Employee, EntityTypeBuilder, EmployeeConfiguration

### Community 35 - "UpdateBranchDto"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 37 - "User Story 4: Deactivate a branch (P4)"
Cohesion: 0.47
Nodes (6): DELETE /api/branches/{id}, Decision 8: DELETE is soft-delete only, no physical delete, FR-011: Deactivating an already-inactive branch is a no-op, FR-012: No capability to permanently delete a branch, SC-004: Deactivated branches retain 100% of data with zero loss, User Story 4: Deactivate a branch (P4)

### Community 38 - "Employee/Vehicle Module (Employees & Vehicles)"
Cohesion: 0.50
Nodes (5): Branch/BranchSchedule Module (Branches & Hubs), Employee/Vehicle Module (Employees & Vehicles), List Endpoint Pagination Convention, Shipment Module, Decisión 13: Paginación reutilizada del módulo 002 para ambos listados

### Community 39 - "Tasks: Customers & Accounts Module"
Cohesion: 0.07
Nodes (26): Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation Strategy, Incremental Delivery (+18 more)

### Community 40 - "ShipmentTracker.Core"
Cohesion: 0.83
Nodes (4): ShipmentTracker.Core, ShipmentTracker.Infrastructure, ShipmentTracker.Services, ShipmentTracker.Web

### Community 55 - "IBaseRepository"
Cohesion: 0.18
Nodes (8): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository

### Community 56 - "Customer"
Cohesion: 0.16
Nodes (8): ShipmentTracker.Infrastructure.Data.Configurations, BusinessCustomer, DateTime, Customer, EntityTypeBuilder, BusinessCustomerConfiguration, EntityTypeBuilder, CustomerConfiguration

### Community 57 - "Research: Customers & Accounts Module"
Cohesion: 0.13
Nodes (14): Decision 10: `ICustomerRepository : IBaseRepository<Customer>` — no extra methods, generic paging works unchanged against the TPT base, Decision 11: Pagination reused from `002`/`003`/`004` — identical contract, `onlyActive`/`type` filters, Decision 12: JSON enum representation — `CustomerType` gets the per-property `JsonStringEnumConverter`, Decision 1: Table Per Type (TPT) via `.ToTable()` on each derived entity, Decision 2: `Customer.Type` stays a real, persisted property — not an EF discriminator, Decision 3: Address fields as flat scalar properties on `Customer` — no owned type, Decision 4: `id_number` → `GovernmentId`; `tax_id` → `TaxId` (naming, not behavior), Decision 5: Business tax ID max length corrected from 13 to 12 — RFC-persona-moral length (+6 more)

### Community 58 - "Data Model: Customers & Accounts Module"
Cohesion: 0.14
Nodes (13): Data Model: Customers & Accounts Module, Database-dependent (`CustomerService`, research.md Decisions 7–8), DTOs, Entity: `BusinessCustomer : Customer`, Entity: `Customer` (abstract base), Entity: `IndividualCustomer : Customer`, Enum, Flow: `CreateIndividualAsync` / `CreateBusinessAsync` (+5 more)

### Community 59 - "US2: Find drivers available at a branch (P2)"
Cohesion: 0.18
Nodes (11): FR-009: List Employees filtered by Branch and/or Role, FR-010: Only active Employees in listings, FR-019: List Vehicles filtered by Branch, FR-020: Only active Vehicles in listings, FR-023: Retrieve single Employee by identifier, FR-024: Retrieve single Vehicle by identifier, SC-005: Deactivated records never returned, never deleted, US2: Find drivers available at a branch (P2) (+3 more)

### Community 60 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.18
Nodes (10): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Customers & Accounts Module, User Story 1 — Register a new customer (P1), User Story 2 — Find and review customers (P2) (+2 more)

### Community 61 - "Core Principles"
Cohesion: 0.20
Nodes (9): Core Principles, Flujo de Trabajo de Desarrollo, Governance, I. Framework Objetivo Único (.NET 8.0), II. Integridad de la Arquitectura en Capas, III. Minimalismo de Dependencias, IV. Cambios Pequeños y Reversibles, Restricciones Técnicas y de Arquitectura (+1 more)

### Community 62 - "graphify add & --watch Reference"
Cohesion: 0.25
Nodes (6): graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, --cluster-only, --update Incremental Re-extraction, Incremental Update & Cluster-Only Reference

### Community 63 - "Extraction Subagent Prompt Spec"
Cohesion: 0.25
Nodes (8): Confidence Score Rubric, Extraction Subagent Prompt Spec, Hyperedges Rule, Node ID Format Rule, Semantic Similarity Edge Rule, Convergence Findings, Gap Type Classification (missing/partial/contradicts/unrequested), NEEDS CLARIFICATION Markers

### Community 64 - "/speckit-plan Command"
Cohesion: 0.29
Nodes (8): Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, Phase 0: Outline & Research, Phase 1: Design & Contracts, /speckit-plan Command, Constitution Check Gate, Implementation Plan Template

### Community 65 - "Extra Exports & Benchmark Reference"
Cohesion: 0.29
Nodes (7): Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server, Neo4j Export, Build Graph & Cluster (Step 4), Community Labeling (Step 5)

### Community 66 - "IndividualCustomer"
Cohesion: 0.33
Nodes (4): DateOnly, IndividualCustomer, EntityTypeBuilder, IndividualCustomerConfiguration

### Community 67 - "Query, Path, Explain Reference"
Cohesion: 0.40
Nodes (6): /graphify explain, /graphify path, Query, Path, Explain Reference, save-result Work Memory Loop, BFS/DFS Graph Traversal, Constrained Query Vocab Expansion

### Community 68 - "/speckit-tasks Command"
Cohesion: 0.40
Nodes (6): /speckit-converge Command, /speckit-implement Command, /speckit-tasks Command, Task Generation Rules, /speckit-taskstoissues Command, Task ID Dedup Regex Matching

### Community 69 - "US7: Deactivate an employee (P7)"
Cohesion: 0.40
Nodes (5): FR-006: Deactivate an active Employee, FR-007: Deactivating already-inactive Employee is a no-op, FR-008: No permanent delete of Employee, US7: Deactivate an employee (P7), Phase 9: User Story 7 Tasks (Deactivate Employee)

### Community 70 - "/speckit-constitution Command"
Cohesion: 0.67
Nodes (3): /speckit-constitution Command, Sync Impact Report, Constitution Template

## Knowledge Gaps
- **219 isolated node(s):** `net8.0`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.EntityFrameworkCore (8.0.*)`, `Microsoft.EntityFrameworkCore.Design (8.0.*)` (+214 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **12 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ShipmentTracker.Infrastructure.Data` connect `ShipmentTracker.Core.Entities` to `ShipmentTracker.Core.Enums`, `ShipmentTracker.Infrastructure.Migrations`?**
  _High betweenness centrality (0.044) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Entities` connect `ShipmentTracker.Core.Entities` to `ShipmentTracker.Core.Enums`, `Employee`, `IndividualCustomer`, `Branch`, `AppDbContext`, `Customer`, `Shipment`, `Vehicle`?**
  _High betweenness centrality (0.042) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Enums` connect `ShipmentTracker.Core.Enums` to `Employee`, `EmployeeDto`, `VehicleDto`, `PagedResult`, `BranchDto`, `Branch`, `AppDbContext`, `Customer`, `Shipment`, `Vehicle`, `CustomerDetailDto`?**
  _High betweenness centrality (0.034) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Sdk`, `net8.0` to the rest of the system?**
  _219 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `ShipmentTracker.Core.Enums` be split into smaller, more focused modules?**
  _Cohesion score 0.050286058416139714 - nodes in this community are weakly interconnected._
- **Should `002 Implementation Plan` be split into smaller, more focused modules?**
  _Cohesion score 0.07071887784921099 - nodes in this community are weakly interconnected._
- **Should `EmployeeDto` be split into smaller, more focused modules?**
  _Cohesion score 0.08156028368794327 - nodes in this community are weakly interconnected._