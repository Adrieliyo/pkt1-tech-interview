# Graph Report - ShipmentTracker  (2026-08-19)

## Corpus Check
- 299 files · ~200,330 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1838 nodes · 3085 edges · 128 communities (118 shown, 10 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 83 edges (avg confidence: 0.79)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `d5b1cf2b`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- 002 Implementation Plan
- CustomerDetailDto
- .CommitAsync
- AuthController
- ShipmentEventDto
- BranchDto
- ShipmentDto
- IBaseRepository
- ShipmentTracker Project Overview
- Implementation Plan: Customers & Accounts Module
- OrderController
- Tasks: Orders Module
- http
- ShipmentTracker.Infrastructure.Data.Configurations
- Tasks: Customers & Accounts Module
- Tasks: Shipment Tracking Events
- speckit.analyze.md
- ShipmentTracker.Core.Enums
- ShipmentTracker.Services.Mappings
- ShipmentTracker.Core.DTOs.ShipmentEvents
- ShipmentTracker.Infrastructure
- User Scenarios & Testing *(mandatory)*
- Implementation Plan: Multiple Shipments per Order
- EmployeeDto
- ShipmentTracker.Core.Interfaces.Services
- VehicleDto
- Data Model: Orders Module
- Feature Specification: Shipment Tracking Events
- Vehicle Entity
- Execution Steps
- common.ps1
- Research: Orders Module
- /graphify Skill
- Tasks: Multiple Shipments per Order
- Employee Entity
- Quickstart: Validar el módulo de Employees & Vehicles
- Research: Customers & Accounts Module
- OrderDto
- Branch
- BaseRepository
- Data Model: Customers & Accounts Module
- Data Model: Shipment Tracking Events
- Research: Shipment Tracking Events
- ShipmentTracker.Core.DTOs.Employees
- ShipmentTracker.Core.Entities
- ShipmentTracker.Core.DTOs.Auth
- AbstractValidator
- Authentication & Authorization Research
- Project Constitution Document
- CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- Specification Quality Checklist: Shipment Tracking Events
- /speckit-specify Command
- DeliveryAttempt
- speckit.plan.md
- speckit.specify.md
- speckit.tasks.md
- 003-branches-hubs Module (Branch entity)
- User Story 1: Register a new branch (P1)
- User Story 2: Find and review branches (P2)
- US2: Find drivers available at a branch (P2)
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- MakeShipmentOrderIdIndexNonUnique
- Program.cs
- Branch Entity
- UnitOfWork
- Shipment
- Core Principles
- 006-orders/research.md
- Shipment Tracking Events Research
- AppDbContext
- ShipmentTracker.Infrastructure.Migrations
- Migration
- Phase 1 Data Model: Multiple Shipments per Order
- IUnitOfWork
- Quickstart: Multiple Shipments per Order
- SeedAdditionalModules
- EmployeeDto
- US1: Register a new employee (P1)
- Implementation Plan: Orders Module
- 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)
- Employee
- graphify add & --watch Reference
- Extraction Subagent Prompt Spec
- /speckit-plan Command
- ShipmentEvent
- speckit.checklist.md
- AddBranchesAndSchedule
- AddEmployeesAndVehicles
- AddCustomers
- AddOrdersAndShipmentEvents
- ExtendShipmentEventsAndAddDeliveryAttempts
- AddIdentityTables
- Tasks: Branches & Hubs Module
- US3: Register a new vehicle (P3)
- Implementation Plan: Shipment Tracking Events
- Extra Exports & Benchmark Reference
- speckit.clarify.md
- speckit.implement.md
- Vehicle
- BranchSchedule Entity
- Query, Path, Explain Reference
- /speckit-tasks Command
- speckit.constitution.md
- UpdateBranchDto
- create-new-feature.ps1
- User Story 4: Deactivate a branch (P4)
- .RejectAsync
- AppDbContextModelSnapshot.cs
- speckit.taskstoissues.md
- IndividualCustomer
- /speckit-constitution Command
- CLAUDE.md
- HTTP Contract: Shipment Tracking Events API
- Phase 0 Research: Multiple Shipments per Order
- Constitution Check (Plan Gate)
- FR-014: New Vehicle active by default
- FR-025: Clear not-found result for nonexistent id
- SC-001: Register employee in a single submission
- SC-002: Find every active driver at a branch in a single lookup
- SC-003: 100% of Employees/Vehicles associated with a valid active Branch
- SC-004: Register vehicle and view complete active fleet at a branch
- API Contract: Multiple Shipments per Order
- ShipmentTransitionValidator.cs

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.Enums` - 62 edges
2. `ShipmentTracker.Core.Entities` - 54 edges
3. `ShipmentTracker.Core.Interfaces.Services` - 25 edges
4. `OrderDto` - 24 edges
5. `ShipmentTracker.Infrastructure.Data` - 23 edges
6. `BaseRepository` - 23 edges
7. `IUnitOfWork` - 21 edges
8. `ShipmentTracker.Core.Interfaces.Repositories` - 21 edges
9. `IBaseRepository` - 21 edges
10. `UnitOfWork` - 21 edges

## Surprising Connections (you probably didn't know these)
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `001 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/001-standardize-mapping-di/spec.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `002 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/002-paginate-shipment-list/spec.md
- `Tasks Template` --conceptually_related_to--> `001 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/001-standardize-mapping-di/tasks.md
- `Tasks Template` --conceptually_related_to--> `002 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/002-paginate-shipment-list/tasks.md
- `CreateUserDtoValidator` --references--> `CreateUserDto`  [EXTRACTED]
  ShipmentTracker.Services/Validators/Auth/CreateUserDtoValidator.cs → ShipmentTracker.Core/DTOs/Auth/CreateUserDto.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Order-to-Shipment Conversion Flow** — specs_006_orders_data_model_order, specs_006_orders_data_model_shipmentevent, specs_006_orders_data_model_shipment_orderid, specs_006_orders_data_model_converttoshipmentasync, specs_006_orders_data_model_orderservice [EXTRACTED 0.90]
- **Spec-Driven Development Command Pipeline** — opencode_commands_speckit_specify_command, opencode_commands_speckit_clarify_command, opencode_commands_speckit_plan_command, opencode_commands_speckit_tasks_command, opencode_commands_speckit_implement_command [EXTRACTED 0.95]
- **Employee and Vehicle Share Branch FK Without Direct Relation** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_data_model_vehicle, specs_003_branches_hubs [EXTRACTED 1.00]
- **Graphify AST+Semantic Extraction & Merge Pipeline** — _claude_skills_graphify_skill_ast_extraction, _claude_skills_graphify_skill_semantic_extraction, _claude_skills_graphify_skill_extraction_cache, _claude_skills_graphify_skill_merge_extraction, _claude_skills_graphify_references_extraction_spec_extraction_spec [EXTRACTED 1.00]
- **Five Clarifications Resolved for Module 008** — specs_008_authentication_authorization_research_driver_assignment_derivation, specs_008_authentication_authorization_spec_warehousestaff_event_types, specs_008_authentication_authorization_spec_account_lockout, specs_008_authentication_authorization_spec_provisioning_superadmin_only, specs_008_authentication_authorization_spec_email_sync [EXTRACTED 1.00]
- **Module 008 Corrections Discovered During Implementation** — specs_008_authentication_authorization_research_identity_core_placement, specs_008_authentication_authorization_research_securitystamp_fix, specs_008_authentication_authorization_research_fallbackpolicy [EXTRACTED 1.00]
- **Nullable Enum Omission-Detection Pattern** — specs_003_branches_hubs_research_decision1, shipmenttracker_core_dtos_createbranchdto_createbranchdto, shipmenttracker_core_dtos_scheduleentryinputdto_scheduleentryinputdto, shipmenttracker_core_enums_branchtype_branchtype, shipmenttracker_core_enums_scheduleday_scheduleday [EXTRACTED 1.00]
- **Full Weekly Schedule Replacement Flow** — shipmenttracker_core_entities_branchschedule_branchschedule, shipmenttracker_services_branchservice_branchservice, specs_003_branches_hubs_research_decision5, specs_003_branches_hubs_spec_fr009, specs_003_branches_hubs_contracts_branches_api_contract_put_branch [EXTRACTED 1.00]
- **Soft-Delete Deactivation Pattern** — shipmenttracker_core_entities_branch_branch, specs_003_branches_hubs_spec_fr011, specs_003_branches_hubs_spec_fr012, specs_003_branches_hubs_research_decision8, specs_003_branches_hubs_contracts_branches_api_contract_delete_branch [EXTRACTED 1.00]
- **Spec-Driven Development Workflow Pipeline** — _claude_skills_speckit_specify_skill_speckit_specify, _claude_skills_speckit_clarify_skill_speckit_clarify, _claude_skills_speckit_plan_skill_speckit_plan, _claude_skills_speckit_tasks_skill_speckit_tasks, _claude_skills_speckit_implement_skill_speckit_implement, _claude_skills_speckit_analyze_skill_speckit_analyze, _claude_skills_speckit_converge_skill_speckit_converge [EXTRACTED 1.00]
- **ShipmentService Constructor Dependency Evolution** — shipmenttracker_core_dtos_pagedresult_pagedresult [INFERRED 0.75]
- **Customer TPT Inheritance Hierarchy** — specs_005_customers_accounts_data_model_customer, specs_005_customers_accounts_data_model_individualcustomer, specs_005_customers_accounts_data_model_businesscustomer, claude_tpt_inheritance [INFERRED 0.85]
- **Navigation-Based Atomic Multi-Entity Write Pattern** — specs_006_orders_spec_converttoshipmentasync, specs_006_orders_research_navigation_fk_fixup, specs_007_shipment_tracking_events_data_model_deliveryattempt [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 001)** — specs_001_standardize_mapping_di_spec, specs_001_standardize_mapping_di_plan, specs_001_standardize_mapping_di_tasks, specs_001_standardize_mapping_di_research, specs_001_standardize_mapping_di_data_model [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 002)** — specs_002_paginate_shipment_list_spec, specs_002_paginate_shipment_list_plan, specs_002_paginate_shipment_list_tasks, specs_002_paginate_shipment_list_research, specs_002_paginate_shipment_list_data_model [INFERRED 0.85]
- **Employee Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_employee, specs_004_employees_vehicles_plan_employeerepository, specs_004_employees_vehicles_plan_employeeservice, specs_004_employees_vehicles_plan_employeecontroller, specs_004_employees_vehicles_data_model_employeedto [INFERRED 0.95]
- **Vehicle Full-Stack Layer Flow (Core/Infra/Services/Web)** — specs_004_employees_vehicles_data_model_vehicle, specs_004_employees_vehicles_plan_vehiclerepository, specs_004_employees_vehicles_plan_vehicleservice, specs_004_employees_vehicles_plan_vehiclecontroller, specs_004_employees_vehicles_data_model_vehicledto [INFERRED 0.95]

## Communities (128 total, 10 thin omitted)

### Community 0 - "002 Implementation Plan"
Cohesion: 0.05
Nodes (69): Spec Kit Full SDD Cycle Workflow, AllowReactApp CORS Policy, Manual (Swagger/HTTP) Verification Policy, Medium Clean Architecture Article, Shipment List Pagination Mechanism, README, Shipment Status Transition State Machine, CreateShipmentDto (+61 more)

### Community 1 - "CustomerDetailDto"
Cohesion: 0.08
Nodes (30): BusinessDetailDto, CreateBusinessCustomerDto, DateOnly, CreateIndividualCustomerDto, DateTime, CustomerDetailDto, DateOnly, IndividualDetailDto (+22 more)

### Community 2 - ".CommitAsync"
Cohesion: 0.16
Nodes (12): IsFulfilled, ShipmentsCount, DateTime, UpdateOrderDto, Task, IMapper, int, IValidator (+4 more)

### Community 3 - "AuthController"
Cohesion: 0.09
Nodes (24): CreateUserDto, UserSessionDto, Task, IUserService, IValidator, Task, UserManager, UserService (+16 more)

### Community 4 - "ShipmentEventDto"
Cohesion: 0.06
Nodes (33): DateTime, DeliveryAttemptDetailDto, DateTime, RegisterDeliveryAttemptDto, DateTime, RegisterEventDto, DateTime, ShipmentEventDto (+25 more)

### Community 5 - "BranchDto"
Cohesion: 0.08
Nodes (31): DateTime, List, BranchDto, List, CreateBranchDto, TimeOnly, ScheduleEntryDto, TimeOnly (+23 more)

### Community 6 - "ShipmentDto"
Cohesion: 0.11
Nodes (21): ControllerBase, HttpPatch, CreateShipmentDto, DateTime, ShipmentDto, ShipmentStatus, Task, IShipmentService (+13 more)

### Community 7 - "IBaseRepository"
Cohesion: 0.18
Nodes (8): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository

### Community 8 - "ShipmentTracker Project Overview"
Cohesion: 0.10
Nodes (35): AutoMapper Output-Only Convention, Enums Persisted as Strings Convention, Minimal-Touch Extension of Existing Modules, FluentValidation Structural-Only Convention, JSON Enum Serialization Gotcha, Pagination / PagedResult Convention, ShipmentTracker Project Overview, Repository + Unit of Work Pattern (+27 more)

### Community 9 - "Implementation Plan: Customers & Accounts Module"
Cohesion: 0.06
Nodes (31): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Customers & Accounts Module, Complexity Tracking, Constitution Check, Documentation (this feature) (+23 more)

### Community 10 - "OrderController"
Cohesion: 0.20
Nodes (11): ConvertOrderResultDto, ActionResult, Authorize, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult (+3 more)

### Community 11 - "Tasks: Orders Module"
Cohesion: 0.06
Nodes (31): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation for User Story 5 (+23 more)

### Community 12 - "http"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 13 - "ShipmentTracker.Infrastructure.Data.Configurations"
Cohesion: 0.12
Nodes (11): ShipmentTracker.Infrastructure.Data.Configurations, IEntityTypeConfiguration, TimeOnly, BranchSchedule, BusinessCustomer, EntityTypeBuilder, BranchScheduleConfiguration, EntityTypeBuilder (+3 more)

### Community 14 - "Tasks: Customers & Accounts Module"
Cohesion: 0.07
Nodes (26): Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation Strategy, Incremental Delivery (+18 more)

### Community 15 - "Tasks: Shipment Tracking Events"
Cohesion: 0.07
Nodes (27): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation Strategy, Incremental Delivery (+19 more)

### Community 16 - "speckit.analyze.md"
Cohesion: 0.08
Nodes (25): 1. Initialize Analysis Context, 2. Load Artifacts (Progressive Disclosure), 3. Build Semantic Models, 4. Detection Passes (Token-Efficient Analysis), 5. Severity Assignment, 6. Produce Compact Analysis Report, 7. Provide Next Actions, 8. Offer Remediation (+17 more)

### Community 17 - "ShipmentTracker.Core.Enums"
Cohesion: 0.10
Nodes (9): ShipmentTracker.Core.DTOs.Branches, ShipmentTracker.Services.Validators.Branches, ShipmentTracker.Core.DTOs.Orders, ShipmentTracker.Services.Validators.Orders, ShipmentTracker.Core.Enums, CreateBranchDtoValidator, UpdateBranchDtoValidator, CreateOrderDtoValidator (+1 more)

### Community 18 - "ShipmentTracker.Services.Mappings"
Cohesion: 0.17
Nodes (9): ShipmentTracker.Services.Mappings, Profile, BranchMappingProfile, CustomerMappingProfile, EmployeeMappingProfile, OrderMappingProfile, ShipmentEventMappingProfile, ShipmentMappingProfile (+1 more)

### Community 19 - "ShipmentTracker.Core.DTOs.ShipmentEvents"
Cohesion: 0.29
Nodes (4): ShipmentTracker.Core.DTOs.ShipmentEvents, ShipmentTracker.Services.Validators.ShipmentEvents, RegisterDeliveryAttemptDtoValidator, RegisterEventDtoValidator

### Community 20 - "ShipmentTracker.Infrastructure"
Cohesion: 0.09
Nodes (25): FluentValidation (11.9.0), Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29), Microsoft.Extensions.Identity.Core (8.0.*), Microsoft.Extensions.Identity.Stores (8.0.*) (+17 more)

### Community 21 - "User Scenarios & Testing *(mandatory)*"
Cohesion: 0.10
Nodes (21): Assumptions, Clarifications, Conversion to Shipment, Edge Cases, Editing, confirming, cancelling, Feature Specification: Orders Module, Finding and reviewing, Functional Requirements (+13 more)

### Community 22 - "Implementation Plan: Multiple Shipments per Order"
Cohesion: 0.07
Nodes (27): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Multiple Shipments per Order, Complexity Tracking, Constitution Check, Documentation (this feature) (+19 more)

### Community 23 - "EmployeeDto"
Cohesion: 0.08
Nodes (29): HashSet, DateOnly, CreateEmployeeDto, DateOnly, DateTime, EmployeeDto, DateOnly, UpdateEmployeeDto (+21 more)

### Community 24 - "ShipmentTracker.Core.Interfaces.Services"
Cohesion: 0.22
Nodes (6): ShipmentTracker.Core.DTOs, ShipmentTracker.Core.DTOs.Shipments, ShipmentTracker.Services.Validators.Shipments, ShipmentTracker.Services, ShipmentTracker.Core.Interfaces, ShipmentTracker.Core.Interfaces.Services

### Community 25 - "VehicleDto"
Cohesion: 0.09
Nodes (23): CreateVehicleDto, UpdateVehicleDto, DateTime, VehicleDto, VehicleType, Task, IVehicleService, IMapper (+15 more)

### Community 26 - "Data Model: Orders Module"
Cohesion: 0.11
Nodes (17): Data Model: Orders Module, Database-dependent (`OrderService`), DTOs, Entity: `Order`, Entity: `Shipment` (existing, module `001`/`002`) — one new nullable column, Entity: `ShipmentEvent` (new), Enums, Flow: `ConfirmOrderAsync` / `CancelOrderAsync` (+9 more)

### Community 27 - "Feature Specification: Shipment Tracking Events"
Cohesion: 0.11
Nodes (18): Assumptions, Clarifications, Delivery attempts, Edge Cases, Extending the shipment event record, Feature Specification: Shipment Tracking Events, Functional Requirements, Key Entities (+10 more)

### Community 28 - "Vehicle Entity"
Cohesion: 0.15
Nodes (17): DELETE /api/vehicles/{id}, GET /api/vehicles/{id}, POST /api/vehicles, PUT /api/vehicles/{id}, CreateVehicleDto, IVehicleService, UpdateVehicleDto, Vehicle Entity (+9 more)

### Community 29 - "Execution Steps"
Cohesion: 0.12
Nodes (15): 1. Initialize Convergence Context, 2. Load Artifacts (Progressive Disclosure), 3. Build the Intent Inventory, 4. Assess the Codebase and Classify Findings, 5. Assign Severity, 6. Present the In-Session Findings Summary, 7. Append Convergence Tasks (or report converged), 8. Provide Next Actions (Handoff) (+7 more)

### Community 30 - "common.ps1"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

### Community 31 - "Research: Orders Module"
Cohesion: 0.12
Nodes (16): Decision 10: Status-transition guards (`Confirm`, `Cancel`, `Update`-when-not-Pending, `Convert`) use `InvalidOperationException` → `400`, not `FluentValidation.ValidationException`, Decision 11: `ConvertToShipmentAsync` atomicity — a single `CommitAsync()` call, no explicit EF transaction API, Decision 12: `ShipmentEvent` gets a forward navigation to `Shipment` (for FK fixup), no reverse collection on `Shipment`, Decision 13: `CustomerId` is not part of `UpdateOrderDto` — ownership is fixed at creation, Decision 14: Pagination — reused from `002`/`003`/`004`/`005`, filtered by `customerId` and `status`, Decision 1: `Shipment` stays in its existing minimal shape — the plan input's "copying quoted_price, recipient data, branch, customer" is not implemented, Decision 2: `Shipment.OrderId` must be nullable — forced by existing seed data and the still-active direct-creation endpoint, Decision 3: Two coexisting tracking-number formats on `Shipment` — no unification attempted (+8 more)

### Community 32 - "/graphify Skill"
Cohesion: 0.17
Nodes (15): GitHub Clone & Cross-Repo Merge Reference, graphify clone, graphify merge-graphs, graphify claude install (CLAUDE.md integration), Commit Hook & CLAUDE.md Integration Reference, graphify hook install (post-commit hook), Transcribe Video/Audio Reference, Whisper Domain-Hint Prompt (+7 more)

### Community 33 - "Tasks: Multiple Shipments per Order"
Cohesion: 0.09
Nodes (21): Dentro de cada historia, Dependencies & Execution Order, Entrega incremental, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation Strategy (+13 more)

### Community 34 - "Employee Entity"
Cohesion: 0.20
Nodes (15): DELETE /api/employees/{id}, Employee Entity, EmployeeRole Enum, IEmployeeRepository, IUnitOfWork (extended), IVehicleRepository, AddEmployeesAndVehicles EF Migration, EmployeeConfiguration (+7 more)

### Community 35 - "Quickstart: Validar el módulo de Employees & Vehicles"
Cohesion: 0.13
Nodes (15): Quickstart: Validar el módulo de Employees & Vehicles, FR-006: Deactivate an active Employee, FR-007: Deactivating already-inactive Employee is a no-op, FR-008: No permanent delete of Employee, FR-015: Update Vehicle fields incl. Branch reassignment, FR-016: Deactivate an active Vehicle, FR-017: Deactivating already-inactive Vehicle is a no-op, FR-018: No permanent delete of Vehicle (+7 more)

### Community 36 - "Research: Customers & Accounts Module"
Cohesion: 0.13
Nodes (14): Decision 10: `ICustomerRepository : IBaseRepository<Customer>` — no extra methods, generic paging works unchanged against the TPT base, Decision 11: Pagination reused from `002`/`003`/`004` — identical contract, `onlyActive`/`type` filters, Decision 12: JSON enum representation — `CustomerType` gets the per-property `JsonStringEnumConverter`, Decision 1: Table Per Type (TPT) via `.ToTable()` on each derived entity, Decision 2: `Customer.Type` stays a real, persisted property — not an EF discriminator, Decision 3: Address fields as flat scalar properties on `Customer` — no owned type, Decision 4: `id_number` → `GovernmentId`; `tax_id` → `TaxId` (naming, not behavior), Decision 5: Business tax ID max length corrected from 13 to 12 — RFC-persona-moral length (+6 more)

### Community 37 - "OrderDto"
Cohesion: 0.20
Nodes (7): DateTime, OrderDto, IEnumerable, PagedResult, OrderStatus, Task, IOrderService

### Community 38 - "Branch"
Cohesion: 0.19
Nodes (9): ICollection, DateTime, Branch, Task, IBranchRepository, EntityTypeBuilder, BranchConfiguration, Task (+1 more)

### Community 39 - "BaseRepository"
Cohesion: 0.18
Nodes (9): DbSet, Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask (+1 more)

### Community 40 - "Data Model: Customers & Accounts Module"
Cohesion: 0.14
Nodes (13): Data Model: Customers & Accounts Module, Database-dependent (`CustomerService`, research.md Decisions 7–8), DTOs, Entity: `BusinessCustomer : Customer`, Entity: `Customer` (abstract base), Entity: `IndividualCustomer : Customer`, Enum, Flow: `CreateIndividualAsync` / `CreateBusinessAsync` (+5 more)

### Community 41 - "Data Model: Shipment Tracking Events"
Cohesion: 0.14
Nodes (14): Data Model: Shipment Tracking Events, Database-dependent (`ShipmentEventService`, shared private helper — research.md Decision 10), DTOs, Entity: `DeliveryAttempt` (new), Entity: `ShipmentEvent` (existing, additive changes only), Enums, Flow: `GetEventsByShipmentAsync` / `GetTrackingAsync`, Flow: `RegisterDeliveryAttemptAsync` (+6 more)

### Community 42 - "Research: Shipment Tracking Events"
Cohesion: 0.14
Nodes (14): Decision 10: No DTO inheritance for `RegisterDeliveryAttemptDto` — a flat, independent DTO; shared rules live in one Service-layer helper, Decision 11: `AttemptNumber` computed via the generic repository, filtering through the `ShipmentEvent` navigation — no new repository method, Decision 12: No pagination on any of this feature's four endpoints, Decision 1: File paths corrected from `Domain/...` to this solution's actual `Core/...` layout, Decision 2: No `Returned` shipment status — only `Delivered`/`Cancelled` are terminal, matching the existing enum and the clarified spec, Decision 3: Reuse the existing `ShipmentTransitionValidator`/`StatusTransitionContext` — no new "`ShipmentTransitionRules`" class, Decision 4: The "current status must be `OutForDelivery`" gate for delivery attempts is a plain equality check, not routed through the transition validator, Decision 5: New route prefix `/api/shipments/...` (plural) coexists with the existing `/api/shipment/...` (singular) — the existing route is not renamed (+6 more)

### Community 43 - "ShipmentTracker.Core.DTOs.Employees"
Cohesion: 0.33
Nodes (4): ShipmentTracker.Core.DTOs.Employees, ShipmentTracker.Services.Validators.Employees, CreateEmployeeDtoValidator, UpdateEmployeeDtoValidator

### Community 44 - "ShipmentTracker.Core.Entities"
Cohesion: 0.22
Nodes (4): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Core.Entities, ShipmentTracker.Infrastructure.Repositories

### Community 45 - "ShipmentTracker.Core.DTOs.Auth"
Cohesion: 0.21
Nodes (6): ShipmentTracker.Core.DTOs.Auth, ShipmentTracker.Web.Controllers, ShipmentTracker.Services.Validators.Auth, LoginDto, CreateUserDtoValidator, LoginDtoValidator

### Community 46 - "AbstractValidator"
Cohesion: 0.13
Nodes (13): AbstractValidator, ShipmentTracker.Services.Validators.Customers, ShipmentTracker.Services.Validators.Vehicles, ShipmentTracker.Core.DTOs.Customers, ShipmentTracker.Core.DTOs.Vehicles, string, CreateBusinessCustomerDtoValidator, string (+5 more)

### Community 47 - "Authentication & Authorization Research"
Cohesion: 0.21
Nodes (23): Authentication & Authorization Requirements Checklist, Authentication & Authorization API Contract, Authentication & Authorization Data Model, ApplicationRole Entity, ApplicationUser Entity, Authentication & Authorization Plan, Authentication & Authorization Quickstart, Authentication & Authorization Research (+15 more)

### Community 48 - "Project Constitution Document"
Cohesion: 0.47
Nodes (12): Project Constitution Document, /speckit.analyze Command, Extension Hooks Mechanism, /speckit.checklist Command, /speckit.clarify Command, /speckit.constitution Command, /speckit.converge Command, /speckit.implement Command (+4 more)

### Community 49 - "CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow"
Cohesion: 0.20
Nodes (12): 002-paginate-shipment-list Module (Pagination Contract), GET /api/employees, GET /api/vehicles, CreateVehicleDtoValidator, UpdateEmployeeDtoValidator, UpdateVehicleDtoValidator, CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow, Implementation Plan: Employees & Vehicles Module (+4 more)

### Community 50 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.17
Nodes (12): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Orders Module, User Story 1 — Create a new order (P1), User Story 2 — Find and review orders (P2) (+4 more)

### Community 51 - "Specification Quality Checklist: Shipment Tracking Events"
Cohesion: 0.50
Nodes (4): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Shipment Tracking Events

### Community 52 - "/speckit-specify Command"
Cohesion: 0.20
Nodes (11): /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section, /speckit-clarify Command, Checklist Status Gate, Spec Quality Checklist, /speckit-specify Command (+3 more)

### Community 53 - "DeliveryAttempt"
Cohesion: 0.28
Nodes (6): DateTime, DeliveryAttempt, IDeliveryAttemptRepository, EntityTypeBuilder, DeliveryAttemptConfiguration, DeliveryAttemptRepository

### Community 54 - "speckit.plan.md"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, Key rules, Mandatory Post-Execution Hooks, Outline, Phase 0: Outline & Research, Phase 1: Design & Contracts, Phases (+2 more)

### Community 55 - "speckit.specify.md"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, For AI Generation, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, Quick Guidelines, Section Requirements (+2 more)

### Community 56 - "speckit.tasks.md"
Cohesion: 0.18
Nodes (10): Checklist Format (REQUIRED), Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Phase Structure, Pre-Execution Checks, Task Generation Rules (+2 more)

### Community 57 - "003-branches-hubs Module (Branch entity)"
Cohesion: 0.22
Nodes (11): 003-branches-hubs Module (Branch entity), Specification Quality Checklist: Employees & Vehicles Module, POST /api/employees, CreateEmployeeDto, Decisión 10: BranchId NO se hace nullable, Decisión 11: JsonStringEnumConverter por propiedad para Role/Type, Decisión 15: Reactivación vía PUT (mismo precedente que Branch), Decisión 7: Unicidad sin filtrar por IsActive (índice único simple) (+3 more)

### Community 58 - "User Story 1: Register a new branch (P1)"
Cohesion: 0.18
Nodes (15): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, ShipmentDto, BranchType Enum, ScheduleDay Enum, POST /api/branches (+7 more)

### Community 59 - "User Story 2: Find and review branches (P2)"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 60 - "US2: Find drivers available at a branch (P2)"
Cohesion: 0.18
Nodes (11): FR-009: List Employees filtered by Branch and/or Role, FR-010: Only active Employees in listings, FR-019: List Vehicles filtered by Branch, FR-020: Only active Vehicles in listings, FR-023: Retrieve single Employee by identifier, FR-024: Retrieve single Vehicle by identifier, SC-005: Deactivated records never returned, never deleted, US2: Find drivers available at a branch (P2) (+3 more)

### Community 61 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.18
Nodes (10): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Customers & Accounts Module, User Story 1 — Register a new customer (P1), User Story 2 — Find and review customers (P2) (+2 more)

### Community 62 - "MakeShipmentOrderIdIndexNonUnique"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, MakeShipmentOrderIdIndexNonUnique

### Community 63 - "Program.cs"
Cohesion: 0.11
Nodes (14): AuthorizationHandlerContext, ShipmentTracker.Web.Authorization, ShipmentTracker.Infrastructure.Data.Seed, ShipmentTracker.Core.Identity, ShipmentTracker.Core.Constants, ShipmentTracker.Infrastructure.Identity, IAuthorizationHandler, IServiceProvider (+6 more)

### Community 64 - "Branch Entity"
Cohesion: 0.22
Nodes (10): Branch Entity, AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration, Decision 10: Latitude/Longitude as nullable double, validated only when present, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default (+2 more)

### Community 65 - "UnitOfWork"
Cohesion: 0.19
Nodes (8): DateTime, Customer, ICustomerRepository, EntityTypeBuilder, CustomerConfiguration, Task, UnitOfWork, CustomerRepository

### Community 66 - "Shipment"
Cohesion: 0.28
Nodes (6): DateTime, Shipment, IShipmentRepository, EntityTypeBuilder, ShipmentConfiguration, ShipmentRepository

### Community 67 - "Core Principles"
Cohesion: 0.20
Nodes (9): Core Principles, Flujo de Trabajo de Desarrollo, Governance, I. Framework Objetivo Único (.NET 8.0), II. Integridad de la Arquitectura en Capas, III. Minimalismo de Dependencias, IV. Cambios Pequeños y Reversibles, Restricciones Técnicas y de Arquitectura (+1 more)

### Community 68 - "006-orders/research.md"
Cohesion: 0.16
Nodes (11): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Orders Module, Navigation-Based FK Fixup Pattern, Order/Tracking Number Sequence Generation, Shipment.OrderId Nullable FK, Status-Transition InvalidOperationException Pattern (+3 more)

### Community 69 - "Shipment Tracking Events Research"
Cohesion: 0.20
Nodes (19): Structural vs DB-Dependent Validation Split, Shipment Tracking Events Requirements Checklist, Shipment Tracking Events API Contract, Shipment Tracking Events Data Model, DeliveryAttempt Entity, ShipmentStatus.OutForDelivery, ShipmentEventType Enum, Shipment Tracking Events Plan (+11 more)

### Community 70 - "AppDbContext"
Cohesion: 0.13
Nodes (14): ClaimsPrincipal, IdentityDbContext, IdentityRole, IdentityUser, ApplicationRole, ApplicationUser, DbSet, ModelBuilder (+6 more)

### Community 71 - "ShipmentTracker.Infrastructure.Migrations"
Cohesion: 0.28
Nodes (4): ShipmentTracker.Infrastructure.Migrations, MigrationBuilder, ModelBuilder, InitialModel

### Community 72 - "Migration"
Cohesion: 0.25
Nodes (4): Migration, MigrationBuilder, ModelBuilder, SeedInitialData

### Community 73 - "Phase 1 Data Model: Multiple Shipments per Order"
Cohesion: 0.22
Nodes (8): Edge case: por qué `ShipmentsCount`/`IsFulfilled` no se calculan en el listado paginado de órdenes, Nueva operación de servicio: `IOrderService.GetShipmentsByOrderAsync`, Order Entity (sin cambios estructurales), OrderDto (Core/DTOs/Orders/OrderDto.cs) — campos nuevos, PagedResult<ShipmentDto> (sin cambios de forma), Phase 1 Data Model: Multiple Shipments per Order, Shipment Entity (sin cambios en la entidad; corrección de índice), ShipmentDto (sin cambios)

### Community 74 - "IUnitOfWork"
Cohesion: 0.24
Nodes (10): IDisposable, DateTime, CreateOrderDto, DateTime, Order, PickupType, ServiceType, IUnitOfWork (+2 more)

### Community 75 - "Quickstart: Multiple Shipments per Order"
Cohesion: 0.22
Nodes (8): Escenario 1 — Generar un segundo envío sobre la misma orden, Escenario 2 — Listar los envíos de una orden, Escenario 3 — Rechazo de conversión sobre orden cancelada, Escenario 4 — Cumplimiento agregado (`isFulfilled`), Escenario 5 — Envíos cancelados no bloquean el cumplimiento, Escenario 6 — Todos los envíos cancelados, Prerrequisitos, Quickstart: Multiple Shipments per Order

### Community 76 - "SeedAdditionalModules"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, SeedAdditionalModules

### Community 77 - "EmployeeDto"
Cohesion: 0.25
Nodes (9): GET /api/employees/{id}, PUT /api/employees/{id}, EmployeeDto, IEmployeeService, UpdateEmployeeDto, EmployeeMappingProfile, FR-005: Update Employee fields incl. Branch reassignment, unrestricted role change, US5: Update employee information (P5) (+1 more)

### Community 78 - "US1: Register a new employee (P1)"
Cohesion: 0.25
Nodes (9): CreateEmployeeDtoValidator, EmployeeController, EmployeeService, FR-001: Create Employee with name, unique number/email, role, hire date, branch, FR-002: Reject Employee create/update on duplicate number/email (company-wide, incl. inactive), FR-003: Reject Employee create/update if Branch missing/inactive, FR-004: New Employee active by default, US1: Register a new employee (P1) (+1 more)

### Community 79 - "Implementation Plan: Orders Module"
Cohesion: 0.22
Nodes (8): Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Orders Module, Project Structure, Source Code (repository root), Summary, Technical Context

### Community 80 - "2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios)"
Cohesion: 0.22
Nodes (9): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating Shipment Tracking Events, User Story 1 — Mark a shipment out for delivery (P1), User Story 2 — Log a failed delivery attempt (P2) (+1 more)

### Community 81 - "Employee"
Cohesion: 0.24
Nodes (7): DateOnly, DateTime, Employee, IEmployeeRepository, EntityTypeBuilder, EmployeeConfiguration, EmployeeRepository

### Community 82 - "graphify add & --watch Reference"
Cohesion: 0.25
Nodes (6): graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, --cluster-only, --update Incremental Re-extraction, Incremental Update & Cluster-Only Reference

### Community 83 - "Extraction Subagent Prompt Spec"
Cohesion: 0.25
Nodes (8): Confidence Score Rubric, Extraction Subagent Prompt Spec, Hyperedges Rule, Node ID Format Rule, Semantic Similarity Edge Rule, Convergence Findings, Gap Type Classification (missing/partial/contradicts/unrequested), NEEDS CLARIFICATION Markers

### Community 84 - "/speckit-plan Command"
Cohesion: 0.29
Nodes (8): Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, Phase 0: Outline & Research, Phase 1: Design & Contracts, /speckit-plan Command, Constitution Check Gate, Implementation Plan Template

### Community 85 - "ShipmentEvent"
Cohesion: 0.28
Nodes (6): DateTime, ShipmentEvent, IShipmentEventRepository, EntityTypeBuilder, ShipmentEventConfiguration, ShipmentEventRepository

### Community 86 - "speckit.checklist.md"
Cohesion: 0.25
Nodes (7): Anti-Examples: What NOT To Do, Checklist Purpose: "Unit Tests for English", Example Checklist Types & Sample Items, Execution Steps, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 87 - "AddBranchesAndSchedule"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddBranchesAndSchedule

### Community 88 - "AddEmployeesAndVehicles"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddEmployeesAndVehicles

### Community 89 - "AddCustomers"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddCustomers

### Community 90 - "AddOrdersAndShipmentEvents"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddOrdersAndShipmentEvents

### Community 91 - "ExtendShipmentEventsAndAddDeliveryAttempts"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, ExtendShipmentEventsAndAddDeliveryAttempts

### Community 92 - "AddIdentityTables"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddIdentityTables

### Community 93 - "Tasks: Branches & Hubs Module"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 94 - "US3: Register a new vehicle (P3)"
Cohesion: 0.25
Nodes (8): VehicleController, VehicleService, Decisión 12: BranchId/role como filtro de query string sin conversor especial, FR-011: Create Vehicle with unique plate, type, brand, model, year, capacity, branch, FR-012: Reject Vehicle create/update on duplicate plate (company-wide, incl. inactive), FR-013: Reject Vehicle create/update if Branch missing/inactive, US3: Register a new vehicle (P3), Phase 5: User Story 3 Tasks (Register Vehicle)

### Community 95 - "Implementation Plan: Shipment Tracking Events"
Cohesion: 0.25
Nodes (8): Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Shipment Tracking Events, Project Structure, Source Code (repository root), Summary, Technical Context

### Community 96 - "Extra Exports & Benchmark Reference"
Cohesion: 0.29
Nodes (7): Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server, Neo4j Export, Build Graph & Cluster (Step 4), Community Labeling (Step 5)

### Community 97 - "speckit.clarify.md"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 98 - "speckit.implement.md"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 99 - "Vehicle"
Cohesion: 0.28
Nodes (6): DateTime, Vehicle, IVehicleRepository, EntityTypeBuilder, VehicleConfiguration, VehicleRepository

### Community 100 - "BranchSchedule Entity"
Cohesion: 0.25
Nodes (11): BranchSchedule Entity, Edge Cases Checklist: Branches & Hubs Module, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 3: Closed-day vs schedule-times consistency, FR-004: Exactly one schedule entry per day of week (7 total), FR-005: Reject duplicate day entries in schedule, FR-006: Opening time strictly earlier than closing time, FR-007: Schedule entry may be marked closed with no times (+3 more)

### Community 101 - "Query, Path, Explain Reference"
Cohesion: 0.40
Nodes (6): /graphify explain, /graphify path, Query, Path, Explain Reference, save-result Work Memory Loop, BFS/DFS Graph Traversal, Constrained Query Vocab Expansion

### Community 102 - "/speckit-tasks Command"
Cohesion: 0.40
Nodes (6): /speckit-converge Command, /speckit-implement Command, /speckit-tasks Command, Task Generation Rules, /speckit-taskstoissues Command, Task ID Dedup Regex Matching

### Community 103 - "speckit.constitution.md"
Cohesion: 0.33
Nodes (5): Outline, Post-Execution Checks, Pre-Execution Checks, Scope Guard, User Input

### Community 104 - "UpdateBranchDto"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 106 - "User Story 4: Deactivate a branch (P4)"
Cohesion: 0.47
Nodes (6): DELETE /api/branches/{id}, Decision 8: DELETE is soft-delete only, no physical delete, FR-011: Deactivating an already-inactive branch is a no-op, FR-012: No capability to permanently delete a branch, SC-004: Deactivated branches retain 100% of data with zero loss, User Story 4: Deactivate a branch (P4)

### Community 107 - ".RejectAsync"
Cohesion: 0.70
Nodes (3): CookieValidatePrincipalContext, Task, EmployeeSessionValidator

### Community 108 - "AppDbContextModelSnapshot.cs"
Cohesion: 0.40
Nodes (3): ModelSnapshot, ModelBuilder, AppDbContextModelSnapshot

### Community 109 - "speckit.taskstoissues.md"
Cohesion: 0.40
Nodes (4): Outline, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 110 - "IndividualCustomer"
Cohesion: 0.33
Nodes (4): DateOnly, IndividualCustomer, EntityTypeBuilder, IndividualCustomerConfiguration

### Community 111 - "/speckit-constitution Command"
Cohesion: 0.67
Nodes (3): /speckit-constitution Command, Sync Impact Report, Constitution Template

### Community 113 - "HTTP Contract: Shipment Tracking Events API"
Cohesion: 0.29
Nodes (7): Examples, `GET /api/shipments/{id}/events`, `GET /api/shipments/tracking/{trackingNumber}`, HTTP Contract: Shipment Tracking Events API, JSON format notes, `POST /api/shipments/{id}/events`, `POST /api/shipments/{id}/events/delivery-attempt`

### Community 114 - "Phase 0 Research: Multiple Shipments per Order"
Cohesion: 0.29
Nodes (6): Decision 1: Relajar el guard de `ConvertToShipmentAsync` en vez de reescribirlo, Decision 2: Indicador de cumplimiento calculado on-demand, sin columna nueva, Decision 3: Nuevo endpoint de listado reutiliza `ShipmentDto`/`PagedResult<T>` existentes, Decision 4: `IShipmentRepository`/`IOrderRepository` no necesitan métodos nuevos, Decision 5: Condición de carrera en `GenerateTrackingNumberAsync` — sin cambios, Phase 0 Research: Multiple Shipments per Order

### Community 126 - "API Contract: Multiple Shipments per Order"
Cohesion: 0.33
Nodes (5): API Contract: Multiple Shipments per Order, `GET /api/orders` (existente, sin cambio de comportamiento), `GET /api/orders/{id}/shipments` (NUEVO), `GET /api/orders/{id}` y `GET /api/orders/number/{orderNumber}` (existentes, DTO extendido), `POST /api/orders/{id}/convert` (existente, comportamiento modificado)

## Knowledge Gaps
- **541 isolated node(s):** `net8.0`, `Microsoft.Extensions.Identity.Stores (8.0.*)`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*)` (+536 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **10 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ShipmentTracker.Infrastructure.Data` connect `ShipmentTracker.Core.Entities` to `ShipmentTracker.Infrastructure.Migrations`, `Migration`, `SeedAdditionalModules`, `AppDbContextModelSnapshot.cs`, `AddBranchesAndSchedule`, `AddEmployeesAndVehicles`, `AddCustomers`, `AddOrdersAndShipmentEvents`, `ExtendShipmentEventsAndAddDeliveryAttempts`, `AddIdentityTables`, `MakeShipmentOrderIdIndexNonUnique`, `Program.cs`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Enums` connect `ShipmentTracker.Core.Enums` to `CustomerDetailDto`, `ShipmentEventDto`, `BranchDto`, `ShipmentDto`, `ShipmentTracker.Infrastructure.Data.Configurations`, `ShipmentTracker.Core.DTOs.ShipmentEvents`, `EmployeeDto`, `ShipmentTracker.Core.Interfaces.Services`, `VehicleDto`, `Branch`, `ShipmentTracker.Core.DTOs.Employees`, `AbstractValidator`, `DeliveryAttempt`, `UnitOfWork`, `Shipment`, `Employee`, `ShipmentEvent`, `Vehicle`, `ShipmentTransitionValidator.cs`?**
  _High betweenness centrality (0.033) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Entities` connect `ShipmentTracker.Core.Entities` to `UnitOfWork`, `Shipment`, `Vehicle`, `Branch`, `ShipmentTracker.Infrastructure.Data.Configurations`, `IndividualCustomer`, `Employee`, `ShipmentTracker.Core.Enums`, `ShipmentTracker.Services.Mappings`, `DeliveryAttempt`, `ShipmentEvent`, `ShipmentTracker.Core.Interfaces.Services`, `Program.cs`?**
  _High betweenness centrality (0.027) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.Extensions.Identity.Stores (8.0.*)`, `Microsoft.NET.Sdk` to the rest of the system?**
  _541 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `002 Implementation Plan` be split into smaller, more focused modules?**
  _Cohesion score 0.05352112676056338 - nodes in this community are weakly interconnected._
- **Should `CustomerDetailDto` be split into smaller, more focused modules?**
  _Cohesion score 0.07832080200501253 - nodes in this community are weakly interconnected._
- **Should `AuthController` be split into smaller, more focused modules?**
  _Cohesion score 0.08901515151515152 - nodes in this community are weakly interconnected._