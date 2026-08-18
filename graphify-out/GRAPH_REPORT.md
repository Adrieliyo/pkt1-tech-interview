# Graph Report - ShipmentTracker  (2026-08-17)

## Corpus Check
- 261 files · ~169,045 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1555 nodes · 2568 edges · 109 communities (96 shown, 13 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 75 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `9a2a844d`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- ShipmentTracker.Core.DTOs
- 002 Implementation Plan
- EmployeeDto
- /graphify Skill
- VehicleDto
- ShipmentStatus
- .CommitAsync
- AddEmployeesAndVehicles
- BaseRepository
- /speckit-specify Command
- Quickstart: Validar el módulo de Employees & Vehicles
- http
- ShipmentTracker.Web
- Vehicle Entity
- Spec Kit PowerShell Helpers
- CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow
- User Story 1: Register a new branch (P1)
- Branch
- BranchSchedule
- 003-branches-hubs Module (Branch entity)
- User Story 2: Find and review branches (P2)
- ShipmentTracker.Core.Entities
- OrderDto
- Branch Entity
- US3: Register a new vehicle (P3)
- Shipment
- UnitOfWork
- Employee Entity
- CustomerDetailDto
- Tasks: Branches & Hubs Module
- Implementation Plan: Employees & Vehicles Module
- Implementation Plan: Customers & Accounts Module
- ShipmentEventDto
- BranchSchedule Entity
- ShipmentTracker.Infrastructure.Data.Configurations
- UpdateBranchDto
- create-new-feature.ps1
- User Story 4: Deactivate a branch (P4)
- User Scenarios & Testing *(mandatory)*
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
- AppDbContext
- Research: Customers & Accounts Module
- Data Model: Customers & Accounts Module
- US2: Find drivers available at a branch (P2)
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- Core Principles
- graphify add & --watch Reference
- Extraction Subagent Prompt Spec
- /speckit-plan Command
- Extra Exports & Benchmark Reference
- Tasks: Orders Module
- Query, Path, Explain Reference
- /speckit-tasks Command
- US7: Deactivate an employee (P7)
- /speckit-constitution Command
- CLAUDE.md
- ShipmentTracker.Core.DTOs.Orders
- speckit.analyze.md
- AbstractValidator
- Data Model: Orders Module
- Profile
- Research: Orders Module
- Execution Steps
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- HTTP Contract: Orders API
- speckit.plan.md
- speckit.specify.md
- speckit.tasks.md
- Implementation Plan: Shipment Tracking Events
- AddOrdersAndShipmentEvents
- EmployeeDto
- speckit.checklist.md
- Migration
- SeedInitialData
- AddBranchesAndSchedule
- ShipmentTracker.Infrastructure.Migrations
- speckit.clarify.md
- speckit.implement.md
- speckit.constitution.md
- AppDbContextModelSnapshot.cs
- speckit.taskstoissues.md
- ShipmentTracker.Core.Enums
- Tasks: Shipment Tracking Events
- Data Model: Shipment Tracking Events
- Research: Shipment Tracking Events
- Program.cs
- ShipmentTracker.Core.DTOs.Employees
- ShipmentTracker.Core.Interfaces.Services
- IEntityTypeConfiguration
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- ShipmentEvent
- ExtendShipmentEventsAndAddDeliveryAttempts
- HTTP Contract: Shipment Tracking Events API

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.Enums` - 62 edges
2. `ShipmentTracker.Core.Entities` - 54 edges
3. `OrderDto` - 23 edges
4. `BaseRepository` - 23 edges
5. `ShipmentTracker.Core.Interfaces.Services` - 22 edges
6. `ShipmentTracker.Core.Interfaces.Repositories` - 21 edges
7. `IBaseRepository` - 21 edges
8. `UnitOfWork` - 21 edges
9. `CustomerDetailDto` - 20 edges
10. `ShipmentTracker.Infrastructure.Data` - 20 edges

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

## Communities (109 total, 13 thin omitted)

### Community 0 - "ShipmentTracker.Core.DTOs"
Cohesion: 0.40
Nodes (4): ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators.Shipments, ShipmentTracker.Services, ShipmentTracker.Core.Interfaces

### Community 1 - "002 Implementation Plan"
Cohesion: 0.07
Nodes (59): Spec Kit Full SDD Cycle Workflow, AllowReactApp CORS Policy, Manual (Swagger/HTTP) Verification Policy, Medium Clean Architecture Article, Shipment List Pagination Mechanism, README, Shipment Status Transition State Machine, CreateShipmentDto (+51 more)

### Community 2 - "EmployeeDto"
Cohesion: 0.10
Nodes (26): DateOnly, CreateEmployeeDto, DateOnly, DateTime, EmployeeDto, DateOnly, UpdateEmployeeDto, EmployeeRole (+18 more)

### Community 3 - "/graphify Skill"
Cohesion: 0.17
Nodes (15): GitHub Clone & Cross-Repo Merge Reference, graphify clone, graphify merge-graphs, graphify claude install (CLAUDE.md integration), Commit Hook & CLAUDE.md Integration Reference, graphify hook install (post-commit hook), Transcribe Video/Audio Reference, Whisper Domain-Hint Prompt (+7 more)

### Community 4 - "VehicleDto"
Cohesion: 0.10
Nodes (23): CreateVehicleDto, UpdateVehicleDto, DateTime, VehicleDto, VehicleType, Task, IVehicleService, IMapper (+15 more)

### Community 5 - "ShipmentStatus"
Cohesion: 0.11
Nodes (21): HttpPatch, CreateShipmentDto, DateTime, ShipmentDto, ShipmentStatus, Task, IShipmentService, IMapper (+13 more)

### Community 6 - ".CommitAsync"
Cohesion: 0.08
Nodes (32): IDisposable, DateTime, List, BranchDto, List, CreateBranchDto, TimeOnly, ScheduleEntryDto (+24 more)

### Community 7 - "AddEmployeesAndVehicles"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddEmployeesAndVehicles

### Community 8 - "BaseRepository"
Cohesion: 0.18
Nodes (9): DbSet, Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask (+1 more)

### Community 9 - "/speckit-specify Command"
Cohesion: 0.20
Nodes (11): /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section, /speckit-clarify Command, Checklist Status Gate, Spec Quality Checklist, /speckit-specify Command (+3 more)

### Community 10 - "Quickstart: Validar el módulo de Employees & Vehicles"
Cohesion: 0.13
Nodes (15): .specify/memory/constitution.md, Spec-Driven Development (Spec Kit), Constitution Check (Plan Gate), Quickstart: Validar el módulo de Employees & Vehicles, Decisión 8: Trim de campos usados en unicidad antes de validar/guardar, Edge Cases (spec.md), FR-015: Update Vehicle fields incl. Branch reassignment, FR-016: Deactivate an active Vehicle (+7 more)

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

### Community 15 - "CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow"
Cohesion: 0.16
Nodes (14): CreateEmployeeDtoValidator, CreateVehicleDtoValidator, UpdateEmployeeDtoValidator, UpdateVehicleDtoValidator, CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow, EmployeeController, EmployeeService, Decisión 6: Método privado compartido ValidateBusinessRulesAsync para Create/Update (+6 more)

### Community 16 - "User Story 1: Register a new branch (P1)"
Cohesion: 0.18
Nodes (15): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, ShipmentDto, BranchType Enum, ScheduleDay Enum, POST /api/branches (+7 more)

### Community 17 - "Branch"
Cohesion: 0.19
Nodes (9): ICollection, DateTime, Branch, Task, IBranchRepository, EntityTypeBuilder, BranchConfiguration, Task (+1 more)

### Community 18 - "BranchSchedule"
Cohesion: 0.33
Nodes (4): TimeOnly, BranchSchedule, EntityTypeBuilder, BranchScheduleConfiguration

### Community 19 - "003-branches-hubs Module (Branch entity)"
Cohesion: 0.20
Nodes (12): 003-branches-hubs Module (Branch entity), Specification Quality Checklist: Employees & Vehicles Module, POST /api/employees, POST /api/vehicles, CreateEmployeeDto, Decisión 10: BranchId NO se hace nullable, Decisión 11: JsonStringEnumConverter por propiedad para Role/Type, Decisión 15: Reactivación vía PUT (mismo precedente que Branch) (+4 more)

### Community 20 - "User Story 2: Find and review branches (P2)"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 21 - "ShipmentTracker.Core.Entities"
Cohesion: 0.19
Nodes (4): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Core.Entities, ShipmentTracker.Infrastructure.Repositories

### Community 22 - "OrderDto"
Cohesion: 0.07
Nodes (32): ConvertOrderResultDto, DateTime, CreateOrderDto, DateTime, OrderDto, DateTime, UpdateOrderDto, DateTime (+24 more)

### Community 23 - "Branch Entity"
Cohesion: 0.22
Nodes (10): Branch Entity, AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration, Decision 10: Latitude/Longitude as nullable double, validated only when present, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default (+2 more)

### Community 24 - "US3: Register a new vehicle (P3)"
Cohesion: 0.22
Nodes (9): Per-Property JsonStringEnumConverter Gotcha, VehicleController, VehicleService, Decisión 12: BranchId/role como filtro de query string sin conversor especial, FR-011: Create Vehicle with unique plate, type, brand, model, year, capacity, branch, FR-012: Reject Vehicle create/update on duplicate plate (company-wide, incl. inactive), FR-013: Reject Vehicle create/update if Branch missing/inactive, US3: Register a new vehicle (P3) (+1 more)

### Community 25 - "Shipment"
Cohesion: 0.28
Nodes (6): DateTime, Shipment, IShipmentRepository, EntityTypeBuilder, ShipmentConfiguration, ShipmentRepository

### Community 26 - "UnitOfWork"
Cohesion: 0.17
Nodes (10): DateTime, Vehicle, IOrderRepository, IVehicleRepository, EntityTypeBuilder, VehicleConfiguration, Task, UnitOfWork (+2 more)

### Community 27 - "Employee Entity"
Cohesion: 0.18
Nodes (17): FK Restrict + Unidirectional Navigation Convention, Repository + Unit of Work Pattern, DELETE /api/employees/{id}, Employee Entity, EmployeeRole Enum, IEmployeeRepository, IUnitOfWork (extended), IVehicleRepository (+9 more)

### Community 28 - "CustomerDetailDto"
Cohesion: 0.07
Nodes (32): ControllerBase, BusinessDetailDto, CreateBusinessCustomerDto, DateOnly, CreateIndividualCustomerDto, DateTime, CustomerDetailDto, DateOnly (+24 more)

### Community 29 - "Tasks: Branches & Hubs Module"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 30 - "Implementation Plan: Employees & Vehicles Module"
Cohesion: 0.20
Nodes (12): Branch/BranchSchedule Module (Branches & Hubs), Employee/Vehicle Module (Employees & Vehicles), FluentValidation Structural-Only + Service DB Rules, List Endpoint Pagination Convention, Shipment Module, 002-paginate-shipment-list Module (Pagination Contract), GET /api/employees, GET /api/vehicles (+4 more)

### Community 31 - "Implementation Plan: Customers & Accounts Module"
Cohesion: 0.06
Nodes (31): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Customers & Accounts Module, Complexity Tracking, Constitution Check, Documentation (this feature) (+23 more)

### Community 32 - "ShipmentEventDto"
Cohesion: 0.08
Nodes (33): DateTime, DeliveryAttemptDetailDto, DateTime, RegisterDeliveryAttemptDto, DateTime, RegisterEventDto, DateTime, ShipmentEventDto (+25 more)

### Community 33 - "BranchSchedule Entity"
Cohesion: 0.25
Nodes (11): BranchSchedule Entity, Edge Cases Checklist: Branches & Hubs Module, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 3: Closed-day vs schedule-times consistency, FR-004: Exactly one schedule entry per day of week (7 total), FR-005: Reject duplicate day entries in schedule, FR-006: Opening time strictly earlier than closing time, FR-007: Schedule entry may be marked closed with no times (+3 more)

### Community 34 - "ShipmentTracker.Infrastructure.Data.Configurations"
Cohesion: 0.18
Nodes (8): ShipmentTracker.Infrastructure.Data.Configurations, DateOnly, DateTime, Employee, IEmployeeRepository, EntityTypeBuilder, EmployeeConfiguration, EmployeeRepository

### Community 35 - "UpdateBranchDto"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 37 - "User Story 4: Deactivate a branch (P4)"
Cohesion: 0.47
Nodes (6): DELETE /api/branches/{id}, Decision 8: DELETE is soft-delete only, no physical delete, FR-011: Deactivating an already-inactive branch is a no-op, FR-012: No capability to permanently delete a branch, SC-004: Deactivated branches retain 100% of data with zero loss, User Story 4: Deactivate a branch (P4)

### Community 38 - "User Scenarios & Testing *(mandatory)*"
Cohesion: 0.06
Nodes (33): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Orders Module, Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Orders Module (+25 more)

### Community 39 - "Tasks: Customers & Accounts Module"
Cohesion: 0.07
Nodes (26): Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation Strategy, Incremental Delivery (+18 more)

### Community 40 - "ShipmentTracker.Core"
Cohesion: 0.83
Nodes (4): ShipmentTracker.Core, ShipmentTracker.Infrastructure, ShipmentTracker.Services, ShipmentTracker.Web

### Community 55 - "IBaseRepository"
Cohesion: 0.18
Nodes (8): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository

### Community 56 - "AppDbContext"
Cohesion: 0.10
Nodes (17): DbContext, BusinessCustomer, DateTime, Customer, DateOnly, IndividualCustomer, ICustomerRepository, DbSet (+9 more)

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

### Community 66 - "Tasks: Orders Module"
Cohesion: 0.06
Nodes (31): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation for User Story 5 (+23 more)

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

### Community 72 - "ShipmentTracker.Core.DTOs.Orders"
Cohesion: 0.22
Nodes (4): ShipmentTracker.Core.DTOs.Orders, ShipmentTracker.Services.Validators.Orders, CreateOrderDtoValidator, UpdateOrderDtoValidator

### Community 73 - "speckit.analyze.md"
Cohesion: 0.08
Nodes (25): 1. Initialize Analysis Context, 2. Load Artifacts (Progressive Disclosure), 3. Build Semantic Models, 4. Detection Passes (Token-Efficient Analysis), 5. Severity Assignment, 6. Produce Compact Analysis Report, 7. Provide Next Actions, 8. Offer Remediation (+17 more)

### Community 74 - "AbstractValidator"
Cohesion: 0.12
Nodes (13): AbstractValidator, ShipmentTracker.Services.Validators.Customers, ShipmentTracker.Core.DTOs.Customers, ShipmentTracker.Services.Validators.Branches, CreateBranchDtoValidator, ScheduleEntryInputDtoValidator, UpdateBranchDtoValidator, string (+5 more)

### Community 75 - "Data Model: Orders Module"
Cohesion: 0.11
Nodes (17): Data Model: Orders Module, Database-dependent (`OrderService`), DTOs, Entity: `Order`, Entity: `Shipment` (existing, module `001`/`002`) — one new nullable column, Entity: `ShipmentEvent` (new), Enums, Flow: `ConfirmOrderAsync` / `CancelOrderAsync` (+9 more)

### Community 76 - "Profile"
Cohesion: 0.12
Nodes (12): ShipmentTracker.Core.DTOs.Shipments, ShipmentTracker.Web.Mappers, ShipmentTracker.Services.Mappings, Profile, BranchMappingProfile, CustomerMappingProfile, EmployeeMappingProfile, OrderMappingProfile (+4 more)

### Community 77 - "Research: Orders Module"
Cohesion: 0.12
Nodes (16): Decision 10: Status-transition guards (`Confirm`, `Cancel`, `Update`-when-not-Pending, `Convert`) use `InvalidOperationException` → `400`, not `FluentValidation.ValidationException`, Decision 11: `ConvertToShipmentAsync` atomicity — a single `CommitAsync()` call, no explicit EF transaction API, Decision 12: `ShipmentEvent` gets a forward navigation to `Shipment` (for FK fixup), no reverse collection on `Shipment`, Decision 13: `CustomerId` is not part of `UpdateOrderDto` — ownership is fixed at creation, Decision 14: Pagination — reused from `002`/`003`/`004`/`005`, filtered by `customerId` and `status`, Decision 1: `Shipment` stays in its existing minimal shape — the plan input's "copying quoted_price, recipient data, branch, customer" is not implemented, Decision 2: `Shipment.OrderId` must be nullable — forced by existing seed data and the still-active direct-creation endpoint, Decision 3: Two coexisting tracking-number formats on `Shipment` — no unification attempted (+8 more)

### Community 78 - "Execution Steps"
Cohesion: 0.12
Nodes (15): 1. Initialize Convergence Context, 2. Load Artifacts (Progressive Disclosure), 3. Build the Intent Inventory, 4. Assess the Codebase and Classify Findings, 5. Assign Severity, 6. Present the In-Session Findings Summary, 7. Append Convergence Tasks (or report converged), 8. Provide Next Actions (Handoff) (+7 more)

### Community 79 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.15
Nodes (12): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Orders Module, User Story 1 — Create a new order (P1), User Story 2 — Find and review orders (P2) (+4 more)

### Community 80 - "HTTP Contract: Orders API"
Cohesion: 0.17
Nodes (11): `DELETE /api/orders/{id}`, Examples, `GET /api/orders`, `GET /api/orders/{id}`, `GET /api/orders/number/{orderNumber}`, HTTP Contract: Orders API, JSON format notes, `POST /api/orders` (+3 more)

### Community 81 - "speckit.plan.md"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, Key rules, Mandatory Post-Execution Hooks, Outline, Phase 0: Outline & Research, Phase 1: Design & Contracts, Phases (+2 more)

### Community 82 - "speckit.specify.md"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, For AI Generation, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, Quick Guidelines, Section Requirements (+2 more)

### Community 83 - "speckit.tasks.md"
Cohesion: 0.18
Nodes (10): Checklist Format (REQUIRED), Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Phase Structure, Pre-Execution Checks, Task Generation Rules (+2 more)

### Community 84 - "Implementation Plan: Shipment Tracking Events"
Cohesion: 0.06
Nodes (30): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Shipment Tracking Events, Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Shipment Tracking Events (+22 more)

### Community 85 - "AddOrdersAndShipmentEvents"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddOrdersAndShipmentEvents

### Community 86 - "EmployeeDto"
Cohesion: 0.25
Nodes (9): GET /api/employees/{id}, PUT /api/employees/{id}, EmployeeDto, IEmployeeService, UpdateEmployeeDto, EmployeeMappingProfile, FR-005: Update Employee fields incl. Branch reassignment, unrestricted role change, US5: Update employee information (P5) (+1 more)

### Community 87 - "speckit.checklist.md"
Cohesion: 0.25
Nodes (7): Anti-Examples: What NOT To Do, Checklist Purpose: "Unit Tests for English", Example Checklist Types & Sample Items, Execution Steps, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 88 - "Migration"
Cohesion: 0.25
Nodes (4): Migration, MigrationBuilder, ModelBuilder, InitialModel

### Community 89 - "SeedInitialData"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, SeedInitialData

### Community 90 - "AddBranchesAndSchedule"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddBranchesAndSchedule

### Community 91 - "ShipmentTracker.Infrastructure.Migrations"
Cohesion: 0.28
Nodes (4): ShipmentTracker.Infrastructure.Migrations, MigrationBuilder, ModelBuilder, AddCustomers

### Community 92 - "speckit.clarify.md"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 93 - "speckit.implement.md"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 94 - "speckit.constitution.md"
Cohesion: 0.33
Nodes (5): Outline, Post-Execution Checks, Pre-Execution Checks, Scope Guard, User Input

### Community 95 - "AppDbContextModelSnapshot.cs"
Cohesion: 0.40
Nodes (3): ModelSnapshot, ModelBuilder, AppDbContextModelSnapshot

### Community 96 - "speckit.taskstoissues.md"
Cohesion: 0.40
Nodes (4): Outline, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 97 - "ShipmentTracker.Core.Enums"
Cohesion: 0.22
Nodes (3): ShipmentTracker.Core.DTOs.Branches, ShipmentTracker.Core.DTOs.ShipmentEvents, ShipmentTracker.Core.Enums

### Community 98 - "Tasks: Shipment Tracking Events"
Cohesion: 0.07
Nodes (27): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation Strategy, Incremental Delivery (+19 more)

### Community 99 - "Data Model: Shipment Tracking Events"
Cohesion: 0.13
Nodes (14): Data Model: Shipment Tracking Events, Database-dependent (`ShipmentEventService`, shared private helper — research.md Decision 10), DTOs, Entity: `DeliveryAttempt` (new), Entity: `ShipmentEvent` (existing, additive changes only), Enums, Flow: `GetEventsByShipmentAsync` / `GetTrackingAsync`, Flow: `RegisterDeliveryAttemptAsync` (+6 more)

### Community 100 - "Research: Shipment Tracking Events"
Cohesion: 0.13
Nodes (14): Decision 10: No DTO inheritance for `RegisterDeliveryAttemptDto` — a flat, independent DTO; shared rules live in one Service-layer helper, Decision 11: `AttemptNumber` computed via the generic repository, filtering through the `ShipmentEvent` navigation — no new repository method, Decision 12: No pagination on any of this feature's four endpoints, Decision 1: File paths corrected from `Domain/...` to this solution's actual `Core/...` layout, Decision 2: No `Returned` shipment status — only `Delivered`/`Cancelled` are terminal, matching the existing enum and the clarified spec, Decision 3: Reuse the existing `ShipmentTransitionValidator`/`StatusTransitionContext` — no new "`ShipmentTransitionRules`" class, Decision 4: The "current status must be `OutForDelivery`" gate for delivery attempts is a plain equality check, not routed through the transition validator, Decision 5: New route prefix `/api/shipments/...` (plural) coexists with the existing `/api/shipment/...` (singular) — the existing route is not renamed (+6 more)

### Community 101 - "Program.cs"
Cohesion: 0.18
Nodes (5): ShipmentTracker.Services.Validators.Vehicles, ShipmentTracker.Core.DTOs.Vehicles, ShipmentTracker.Services.Validators.ShipmentEvents, CreateVehicleDtoValidator, UpdateVehicleDtoValidator

### Community 102 - "ShipmentTracker.Core.DTOs.Employees"
Cohesion: 0.22
Nodes (4): ShipmentTracker.Core.DTOs.Employees, ShipmentTracker.Services.Validators.Employees, CreateEmployeeDtoValidator, UpdateEmployeeDtoValidator

### Community 104 - "IEntityTypeConfiguration"
Cohesion: 0.24
Nodes (7): IEntityTypeConfiguration, DateTime, DeliveryAttempt, IDeliveryAttemptRepository, EntityTypeBuilder, DeliveryAttemptConfiguration, DeliveryAttemptRepository

### Community 105 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.20
Nodes (9): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating Shipment Tracking Events, User Story 1 — Mark a shipment out for delivery (P1), User Story 2 — Log a failed delivery attempt (P2) (+1 more)

### Community 106 - "ShipmentEvent"
Cohesion: 0.28
Nodes (6): DateTime, ShipmentEvent, IShipmentEventRepository, EntityTypeBuilder, ShipmentEventConfiguration, ShipmentEventRepository

### Community 107 - "ExtendShipmentEventsAndAddDeliveryAttempts"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, ExtendShipmentEventsAndAddDeliveryAttempts

### Community 108 - "HTTP Contract: Shipment Tracking Events API"
Cohesion: 0.25
Nodes (7): Examples, `GET /api/shipments/{id}/events`, `GET /api/shipments/tracking/{trackingNumber}`, HTTP Contract: Shipment Tracking Events API, JSON format notes, `POST /api/shipments/{id}/events`, `POST /api/shipments/{id}/events/delivery-attempt`

## Knowledge Gaps
- **481 isolated node(s):** `net8.0`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.EntityFrameworkCore (8.0.*)`, `Microsoft.EntityFrameworkCore.Design (8.0.*)` (+476 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **13 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ShipmentTracker.Core.Enums` connect `ShipmentTracker.Core.Enums` to `ShipmentTracker.Core.DTOs`, `EmployeeDto`, `VehicleDto`, `ShipmentStatus`, `.CommitAsync`, `Branch`, `BranchSchedule`, `ShipmentTracker.Core.Entities`, `OrderDto`, `Shipment`, `UnitOfWork`, `CustomerDetailDto`, `ShipmentEventDto`, `ShipmentTracker.Infrastructure.Data.Configurations`, `AppDbContext`, `ShipmentTracker.Core.DTOs.Orders`, `AbstractValidator`, `Profile`, `Program.cs`, `ShipmentTracker.Core.DTOs.Employees`, `ShipmentTracker.Core.Interfaces.Services`, `IEntityTypeConfiguration`, `ShipmentEvent`?**
  _High betweenness centrality (0.036) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Infrastructure.Data` connect `ShipmentTracker.Core.Entities` to `Program.cs`, `AddEmployeesAndVehicles`, `ExtendShipmentEventsAndAddDeliveryAttempts`, `AddOrdersAndShipmentEvents`, `Migration`, `SeedInitialData`, `AddBranchesAndSchedule`, `ShipmentTracker.Infrastructure.Migrations`, `AppDbContextModelSnapshot.cs`?**
  _High betweenness centrality (0.030) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Entities` connect `ShipmentTracker.Core.Entities` to `ShipmentTracker.Core.DTOs`, `ShipmentTracker.Infrastructure.Data.Configurations`, `IEntityTypeConfiguration`, `ShipmentEvent`, `Profile`, `Branch`, `BranchSchedule`, `AppDbContext`, `Shipment`, `UnitOfWork`?**
  _High betweenness centrality (0.029) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Sdk`, `net8.0` to the rest of the system?**
  _481 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `002 Implementation Plan` be split into smaller, more focused modules?**
  _Cohesion score 0.06892655367231638 - nodes in this community are weakly interconnected._
- **Should `EmployeeDto` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._
- **Should `VehicleDto` be split into smaller, more focused modules?**
  _Cohesion score 0.10256410256410256 - nodes in this community are weakly interconnected._