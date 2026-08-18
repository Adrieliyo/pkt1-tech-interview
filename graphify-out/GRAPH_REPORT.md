# Graph Report - ShipmentTracker  (2026-08-18)

## Corpus Check
- 155 files · ~189,130 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 1768 nodes · 2915 edges · 126 communities (115 shown, 11 thin omitted)
- Extraction: 98% EXTRACTED · 2% INFERRED · 0% AMBIGUOUS · INFERRED: 63 edges (avg confidence: 0.79)
- Token cost: 370,082 input · 41,120 output

## Community Hubs (Navigation)
- Project Conventions & Docs
- Customer DTOs & Validators
- Order Conversion DTOs
- Identity Core (User/Role/Claims)
- Delivery Attempt DTOs
- Branch Validators & DTOs
- Shipment Controller & DTOs
- Base Repository Interface
- Architecture Conventions List
- Customers Spec Quality Checklist
- Repository Implementations
- Orders Tasks Breakdown
- Launch Settings Config
- EF Entity Configurations
- Customers Module Tasks
- Orders Tasks (US1-3)
- speckit-analyze Workflow
- Core Entities & Order DTOs
- AutoMapper Mapping Profiles
- ShipmentEvent DTOs & Validators
- NuGet Package Dependencies
- Orders Spec Clarifications
- Branch DTOs & Service
- Employee DTOs & Service
- Shipment Validators & Interfaces
- Vehicle DTOs & Service
- Orders Data Model
- ShipmentEvent Spec Clarifications
- Vehicle CRUD Endpoints
- speckit-converge Workflow
- Spec Kit PowerShell Helpers
- Orders Research Decisions (10-14)
- graphify Clone & Merge Reference
- PagedResult DTO
- Employee Entity & Repository
- Employees/Vehicles Quickstart FRs
- Customers Research Decisions
- Vehicle DTOs & Validators
- Branch Entity Relations
- Common Service Plumbing Types
- Customers Data Model
- ShipmentEvent Data Model
- ShipmentEvent Research Decisions (1-2, 10-12)
- Employee DTOs (Create)
- Infrastructure Data Layer Namespaces
- Roles Constants & Web Controllers
- EmployeeRole Enum Usage
- Auth Plan: ApplicationUser/Role Entities
- Spec Kit Constitution & Hooks
- Pagination Contract Reuse
- Customers Quickstart Scenarios
- ShipmentEvent Requirements & Status
- speckit-clarify Ambiguity Taxonomy
- Identity Infra Namespaces
- speckit-plan Command Spec
- speckit-specify Command Spec
- speckit-tasks Command Spec
- Employee Quickstart & POST
- Branch Requirements & Decisions
- Branch GET Endpoints
- Employee/Vehicle List Filters (FRs)
- Customers Quickstart Steps
- ShipmentEvent API Contract
- Auth DTOs & Validators (Login/User)
- Branch Entity & Migrations
- Employee Repository Interface
- ShipmentEvent Repository Interface
- Project Constitution Principles
- Orders Spec Quality Checklist
- Orders Research: Sequencing & FK
- IdentityDbContext & Core Entities
- Initial Migration
- Seed Initial Data Migration
- Vehicle Service Async Ops
- Order Repository & Config
- Shipment Repository & Config
- Vehicle Repository & Config
- Employee Update Endpoint
- Employee Create Requirements
- Orders Implementation Plan
- Orders Quickstart Scenarios
- Auth Spec: Checklist & Driver Derivation
- graphify Add/Watch Reference
- graphify Extraction Prompt Spec
- speckit-analyze Severity & Phases
- Auth Authorization Handlers
- speckit-checklist Guidance
- Branches/Schedule Migration
- Employees/Vehicles Migration
- Customers Migration
- Orders/ShipmentEvents Migration
- DeliveryAttempts Migration
- Identity Tables Migration
- Branches Module Docs
- Vehicle Create Requirements
- ShipmentEvent Implementation Plan
- graphify Export & Benchmark Reference
- speckit-clarify Command Spec
- speckit-implement Command Spec
- Branch Schedule DTOs
- BranchSchedule Entity & Rules
- graphify Query/Path/Explain Reference
- Spec Kit Command Index
- speckit-constitution Command Spec
- Branch Update Endpoint
- New Feature Branch Script
- Branch Deactivation Requirements
- Employee Session Validator Logic
- EF Model Snapshot
- speckit-taskstoissues Command Spec
- ShipmentEvent Spec Quality Checklist
- Constitution Template & Sync
- Root CLAUDE.md
- DbContext Base Type
- DbSet Type
- Plan Constitution Gate
- Vehicle Default-Active FR
- Employee Not-Found FR
- Employee Registration Success Criterion
- Active Driver Lookup Criterion
- Employee/Vehicle-Branch Association Criterion
- Fleet View Success Criterion

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.Enums` - 62 edges
2. `ShipmentTracker.Core.Entities` - 55 edges
3. `ShipmentTracker.Core.Interfaces.Services` - 25 edges
4. `OrderDto` - 23 edges
5. `ShipmentTracker.Core.Interfaces.Repositories` - 21 edges
6. `IUnitOfWork` - 21 edges
7. `ShipmentTracker.Infrastructure.Data` - 21 edges
8. `UnitOfWork` - 21 edges
9. `CustomerDetailDto` - 20 edges
10. `BaseRepository` - 19 edges

## Surprising Connections (you probably didn't know these)
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `001 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/001-standardize-mapping-di/spec.md
- `Spec Kit Full SDD Cycle Workflow` --conceptually_related_to--> `002 Feature Specification`  [INFERRED]
  .specify/workflows/speckit/workflow.yml → specs/002-paginate-shipment-list/spec.md
- `Tasks Template` --conceptually_related_to--> `001 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/001-standardize-mapping-di/tasks.md
- `Tasks Template` --conceptually_related_to--> `002 Tasks`  [INFERRED]
  .specify/templates/tasks-template.md → specs/002-paginate-shipment-list/tasks.md
- `EmployeeService` --implements--> `IEmployeeService`  [EXTRACTED]
  ShipmentTracker.Services/EmployeeService.cs → ShipmentTracker.Core/Interfaces/Services/IEmployeeService.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Customer TPT Inheritance Hierarchy** — specs_005_customers_accounts_data_model_customer, specs_005_customers_accounts_data_model_individualcustomer, specs_005_customers_accounts_data_model_businesscustomer, claude_tpt_inheritance [INFERRED 0.85]
- **Order-to-Shipment Conversion Flow** — specs_006_orders_data_model_order, specs_006_orders_data_model_shipmentevent, specs_006_orders_data_model_shipment_orderid, specs_006_orders_data_model_converttoshipmentasync, specs_006_orders_data_model_orderservice [EXTRACTED 0.90]
- **Spec-Driven Development Command Pipeline** — opencode_commands_speckit_specify_command, opencode_commands_speckit_clarify_command, opencode_commands_speckit_plan_command, opencode_commands_speckit_tasks_command, opencode_commands_speckit_implement_command [EXTRACTED 0.95]
- **Five Clarifications Resolved for Module 008** — specs_008_authentication_authorization_research_driver_assignment_derivation, specs_008_authentication_authorization_spec_warehousestaff_event_types, specs_008_authentication_authorization_spec_account_lockout, specs_008_authentication_authorization_spec_provisioning_superadmin_only, specs_008_authentication_authorization_spec_email_sync [EXTRACTED 1.00]
- **Module 008 Corrections Discovered During Implementation** — specs_008_authentication_authorization_research_identity_core_placement, specs_008_authentication_authorization_research_securitystamp_fix, specs_008_authentication_authorization_research_fallbackpolicy [EXTRACTED 1.00]
- **Navigation-Based Atomic Multi-Entity Write Pattern** — specs_006_orders_spec_converttoshipmentasync, specs_006_orders_research_navigation_fk_fixup, specs_007_shipment_tracking_events_data_model_deliveryattempt [INFERRED 0.85]
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

## Communities (126 total, 11 thin omitted)

### Community 0 - "Project Conventions & Docs"
Cohesion: 0.05
Nodes (70): Spec Kit Full SDD Cycle Workflow, AllowReactApp CORS Policy, Manual (Swagger/HTTP) Verification Policy, Medium Clean Architecture Article, Shipment List Pagination Mechanism, README, Shipment Status Transition State Machine, CreateShipmentDto (+62 more)

### Community 1 - "Customer DTOs & Validators"
Cohesion: 0.06
Nodes (38): ShipmentTracker.Services.Validators.Customers, ShipmentTracker.Core.DTOs.Customers, BusinessDetailDto, CreateBusinessCustomerDto, DateOnly, CreateIndividualCustomerDto, DateTime, CustomerDetailDto (+30 more)

### Community 2 - "Order Conversion DTOs"
Cohesion: 0.07
Nodes (33): ConvertOrderResultDto, DateTime, CreateOrderDto, DateTime, OrderDto, DateTime, UpdateOrderDto, OrderStatus (+25 more)

### Community 3 - "Identity Core (User/Role/Claims)"
Cohesion: 0.06
Nodes (35): ClaimsPrincipal, IdentityRole, IdentityUser, CreateUserDto, UserSessionDto, ApplicationRole, Employee, ApplicationUser (+27 more)

### Community 4 - "Delivery Attempt DTOs"
Cohesion: 0.09
Nodes (26): HashSet, DateTime, RegisterDeliveryAttemptDto, DateTime, ShipmentEventDto, DateTime, List, ShipmentTrackingDto (+18 more)

### Community 5 - "Branch Validators & DTOs"
Cohesion: 0.08
Nodes (27): AbstractValidator, ShipmentTracker.Services.Validators.Branches, DateTime, List, BranchDto, List, CreateBranchDto, TimeOnly (+19 more)

### Community 6 - "Shipment Controller & DTOs"
Cohesion: 0.08
Nodes (26): ControllerBase, CreateShipmentDto, HttpPatch, IShipmentService, ShipmentDto, CreateShipmentDto, DateTime, ShipmentDto (+18 more)

### Community 7 - "Base Repository Interface"
Cohesion: 0.09
Nodes (17): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository (+9 more)

### Community 8 - "Architecture Conventions List"
Cohesion: 0.10
Nodes (35): AutoMapper Output-Only Convention, Enums Persisted as Strings Convention, Minimal-Touch Extension of Existing Modules, FluentValidation Structural-Only Convention, JSON Enum Serialization Gotcha, Pagination / PagedResult Convention, ShipmentTracker Project Overview, Repository + Unit of Work Pattern (+27 more)

### Community 9 - "Customers Spec Quality Checklist"
Cohesion: 0.06
Nodes (31): Content Quality, Feature Readiness, Notes, Requirement Completeness, Specification Quality Checklist: Customers & Accounts Module, Complexity Tracking, Constitution Check, Documentation (this feature) (+23 more)

### Community 10 - "Repository Implementations"
Cohesion: 0.08
Nodes (26): BaseRepository, BranchRepository, EmployeeRepository, IBaseRepository, IDisposable, ShipmentRepository, DateTime, DeliveryAttempt (+18 more)

### Community 11 - "Orders Tasks Breakdown"
Cohesion: 0.06
Nodes (31): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation for User Story 5 (+23 more)

### Community 12 - "Launch Settings Config"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 13 - "EF Entity Configurations"
Cohesion: 0.10
Nodes (17): ShipmentTracker.Infrastructure.Data.Configurations, IEntityTypeConfiguration, TimeOnly, BranchSchedule, BusinessCustomer, DateTime, Customer, DateOnly (+9 more)

### Community 14 - "Customers Module Tasks"
Cohesion: 0.07
Nodes (26): Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation for User Story 4, Implementation Strategy, Incremental Delivery (+18 more)

### Community 15 - "Orders Tasks (US1-3)"
Cohesion: 0.07
Nodes (27): Core layer (`ShipmentTracker.Core/`), Dependencies & Execution Order, Format: `[ID] [P?] [Story] Description`, Implementation for User Story 1, Implementation for User Story 2, Implementation for User Story 3, Implementation Strategy, Incremental Delivery (+19 more)

### Community 16 - "speckit-analyze Workflow"
Cohesion: 0.08
Nodes (25): 1. Initialize Analysis Context, 2. Load Artifacts (Progressive Disclosure), 3. Build Semantic Models, 4. Detection Passes (Token-Efficient Analysis), 5. Severity Assignment, 6. Produce Compact Analysis Report, 7. Provide Next Actions, 8. Offer Remediation (+17 more)

### Community 17 - "Core Entities & Order DTOs"
Cohesion: 0.14
Nodes (5): ShipmentTracker.Core.Entities, ShipmentTracker.Core.DTOs.Branches, ShipmentTracker.Core.DTOs.Orders, ShipmentTracker.Services.Validators.Orders, ShipmentTracker.Core.Enums

### Community 18 - "AutoMapper Mapping Profiles"
Cohesion: 0.13
Nodes (12): ShipmentTracker.Core.DTOs.Shipments, ShipmentTracker.Web.Mappers, ShipmentTracker.Services.Mappings, Profile, BranchMappingProfile, CustomerMappingProfile, EmployeeMappingProfile, OrderMappingProfile (+4 more)

### Community 19 - "ShipmentEvent DTOs & Validators"
Cohesion: 0.11
Nodes (12): ShipmentTracker.Core.DTOs.ShipmentEvents, ShipmentTracker.Services.Validators.ShipmentEvents, DateTime, DeliveryAttemptDetailDto, DateTime, RegisterEventDto, DateTime, TrackingEventDto (+4 more)

### Community 20 - "NuGet Package Dependencies"
Cohesion: 0.14
Nodes (17): AutoMapper (16.2.0), FluentValidation (11.9.0), Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.Design (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29) (+9 more)

### Community 21 - "Orders Spec Clarifications"
Cohesion: 0.10
Nodes (21): Assumptions, Clarifications, Conversion to Shipment, Edge Cases, Editing, confirming, cancelling, Feature Specification: Orders Module, Finding and reviewing, Functional Requirements (+13 more)

### Community 22 - "Branch DTOs & Service"
Cohesion: 0.18
Nodes (15): BranchDto, BranchType, CreateBranchDto, IBranchService, ActionResult, Authorize, HttpDelete, HttpGet (+7 more)

### Community 23 - "Employee DTOs & Service"
Cohesion: 0.17
Nodes (14): CreateEmployeeDto, EmployeeDto, EmployeeRole, IEmployeeService, ActionResult, HttpDelete, HttpGet, HttpPost (+6 more)

### Community 24 - "Shipment Validators & Interfaces"
Cohesion: 0.25
Nodes (5): ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators.Shipments, ShipmentTracker.Services, ShipmentTracker.Core.Interfaces, ShipmentTracker.Core.Interfaces.Services

### Community 25 - "Vehicle DTOs & Service"
Cohesion: 0.18
Nodes (13): CreateVehicleDto, IVehicleService, ActionResult, HttpDelete, HttpGet, HttpPost, HttpPut, IActionResult (+5 more)

### Community 26 - "Orders Data Model"
Cohesion: 0.11
Nodes (17): Data Model: Orders Module, Database-dependent (`OrderService`), DTOs, Entity: `Order`, Entity: `Shipment` (existing, module `001`/`002`) — one new nullable column, Entity: `ShipmentEvent` (new), Enums, Flow: `ConfirmOrderAsync` / `CancelOrderAsync` (+9 more)

### Community 27 - "ShipmentEvent Spec Clarifications"
Cohesion: 0.11
Nodes (18): Assumptions, Clarifications, Delivery attempts, Edge Cases, Extending the shipment event record, Feature Specification: Shipment Tracking Events, Functional Requirements, Key Entities (+10 more)

### Community 28 - "Vehicle CRUD Endpoints"
Cohesion: 0.15
Nodes (17): DELETE /api/vehicles/{id}, GET /api/vehicles/{id}, POST /api/vehicles, PUT /api/vehicles/{id}, CreateVehicleDto, IVehicleService, UpdateVehicleDto, Vehicle Entity (+9 more)

### Community 29 - "speckit-converge Workflow"
Cohesion: 0.12
Nodes (15): 1. Initialize Convergence Context, 2. Load Artifacts (Progressive Disclosure), 3. Build the Intent Inventory, 4. Assess the Codebase and Classify Findings, 5. Assign Severity, 6. Present the In-Session Findings Summary, 7. Append Convergence Tasks (or report converged), 8. Provide Next Actions (Handoff) (+7 more)

### Community 30 - "Spec Kit PowerShell Helpers"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

### Community 31 - "Orders Research Decisions (10-14)"
Cohesion: 0.12
Nodes (16): Decision 10: Status-transition guards (`Confirm`, `Cancel`, `Update`-when-not-Pending, `Convert`) use `InvalidOperationException` → `400`, not `FluentValidation.ValidationException`, Decision 11: `ConvertToShipmentAsync` atomicity — a single `CommitAsync()` call, no explicit EF transaction API, Decision 12: `ShipmentEvent` gets a forward navigation to `Shipment` (for FK fixup), no reverse collection on `Shipment`, Decision 13: `CustomerId` is not part of `UpdateOrderDto` — ownership is fixed at creation, Decision 14: Pagination — reused from `002`/`003`/`004`/`005`, filtered by `customerId` and `status`, Decision 1: `Shipment` stays in its existing minimal shape — the plan input's "copying quoted_price, recipient data, branch, customer" is not implemented, Decision 2: `Shipment.OrderId` must be nullable — forced by existing seed data and the still-active direct-creation endpoint, Decision 3: Two coexisting tracking-number formats on `Shipment` — no unification attempted (+8 more)

### Community 32 - "graphify Clone & Merge Reference"
Cohesion: 0.17
Nodes (15): GitHub Clone & Cross-Repo Merge Reference, graphify clone, graphify merge-graphs, graphify claude install (CLAUDE.md integration), Commit Hook & CLAUDE.md Integration Reference, graphify hook install (post-commit hook), Transcribe Video/Audio Reference, Whisper Domain-Hint Prompt (+7 more)

### Community 33 - "PagedResult DTO"
Cohesion: 0.18
Nodes (9): IEnumerable, PagedResult, IMapper, int, IValidator, List, Task, ValidationFailure (+1 more)

### Community 34 - "Employee Entity & Repository"
Cohesion: 0.20
Nodes (15): DELETE /api/employees/{id}, Employee Entity, EmployeeRole Enum, IEmployeeRepository, IUnitOfWork (extended), IVehicleRepository, AddEmployeesAndVehicles EF Migration, EmployeeConfiguration (+7 more)

### Community 35 - "Employees/Vehicles Quickstart FRs"
Cohesion: 0.13
Nodes (15): Quickstart: Validar el módulo de Employees & Vehicles, FR-006: Deactivate an active Employee, FR-007: Deactivating already-inactive Employee is a no-op, FR-008: No permanent delete of Employee, FR-015: Update Vehicle fields incl. Branch reassignment, FR-016: Deactivate an active Vehicle, FR-017: Deactivating already-inactive Vehicle is a no-op, FR-018: No permanent delete of Vehicle (+7 more)

### Community 36 - "Customers Research Decisions"
Cohesion: 0.13
Nodes (14): Decision 10: `ICustomerRepository : IBaseRepository<Customer>` — no extra methods, generic paging works unchanged against the TPT base, Decision 11: Pagination reused from `002`/`003`/`004` — identical contract, `onlyActive`/`type` filters, Decision 12: JSON enum representation — `CustomerType` gets the per-property `JsonStringEnumConverter`, Decision 1: Table Per Type (TPT) via `.ToTable()` on each derived entity, Decision 2: `Customer.Type` stays a real, persisted property — not an EF discriminator, Decision 3: Address fields as flat scalar properties on `Customer` — no owned type, Decision 4: `id_number` → `GovernmentId`; `tax_id` → `TaxId` (naming, not behavior), Decision 5: Business tax ID max length corrected from 13 to 12 — RFC-persona-moral length (+6 more)

### Community 37 - "Vehicle DTOs & Validators"
Cohesion: 0.19
Nodes (7): ShipmentTracker.Services.Validators.Vehicles, ShipmentTracker.Core.DTOs.Vehicles, CreateVehicleDto, UpdateVehicleDto, VehicleType, CreateVehicleDtoValidator, UpdateVehicleDtoValidator

### Community 38 - "Branch Entity Relations"
Cohesion: 0.19
Nodes (9): ICollection, DateTime, Branch, Task, IBranchRepository, EntityTypeBuilder, BranchConfiguration, Task (+1 more)

### Community 39 - "Common Service Plumbing Types"
Cohesion: 0.23
Nodes (8): Task, IMapper, int, IValidator, List, Task, ValidationFailure, EmployeeService

### Community 40 - "Customers Data Model"
Cohesion: 0.14
Nodes (13): Data Model: Customers & Accounts Module, Database-dependent (`CustomerService`, research.md Decisions 7–8), DTOs, Entity: `BusinessCustomer : Customer`, Entity: `Customer` (abstract base), Entity: `IndividualCustomer : Customer`, Enum, Flow: `CreateIndividualAsync` / `CreateBusinessAsync` (+5 more)

### Community 41 - "ShipmentEvent Data Model"
Cohesion: 0.14
Nodes (14): Data Model: Shipment Tracking Events, Database-dependent (`ShipmentEventService`, shared private helper — research.md Decision 10), DTOs, Entity: `DeliveryAttempt` (new), Entity: `ShipmentEvent` (existing, additive changes only), Enums, Flow: `GetEventsByShipmentAsync` / `GetTrackingAsync`, Flow: `RegisterDeliveryAttemptAsync` (+6 more)

### Community 42 - "ShipmentEvent Research Decisions (1-2, 10-12)"
Cohesion: 0.14
Nodes (14): Decision 10: No DTO inheritance for `RegisterDeliveryAttemptDto` — a flat, independent DTO; shared rules live in one Service-layer helper, Decision 11: `AttemptNumber` computed via the generic repository, filtering through the `ShipmentEvent` navigation — no new repository method, Decision 12: No pagination on any of this feature's four endpoints, Decision 1: File paths corrected from `Domain/...` to this solution's actual `Core/...` layout, Decision 2: No `Returned` shipment status — only `Delivered`/`Cancelled` are terminal, matching the existing enum and the clarified spec, Decision 3: Reuse the existing `ShipmentTransitionValidator`/`StatusTransitionContext` — no new "`ShipmentTransitionRules`" class, Decision 4: The "current status must be `OutForDelivery`" gate for delivery attempts is a plain equality check, not routed through the transition validator, Decision 5: New route prefix `/api/shipments/...` (plural) coexists with the existing `/api/shipment/...` (singular) — the existing route is not renamed (+6 more)

### Community 43 - "Employee DTOs (Create)"
Cohesion: 0.19
Nodes (8): ShipmentTracker.Core.DTOs.Employees, ShipmentTracker.Services.Validators.Employees, DateOnly, CreateEmployeeDto, DateOnly, UpdateEmployeeDto, CreateEmployeeDtoValidator, UpdateEmployeeDtoValidator

### Community 44 - "Infrastructure Data Layer Namespaces"
Cohesion: 0.38
Nodes (3): ShipmentTracker.Infrastructure.Data, ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Infrastructure.Repositories

### Community 45 - "Roles Constants & Web Controllers"
Cohesion: 0.24
Nodes (4): ShipmentTracker.Core.Constants, ShipmentTracker.Web.Controllers, string, Roles

### Community 46 - "EmployeeRole Enum Usage"
Cohesion: 0.27
Nodes (6): DateOnly, DateTime, EmployeeDto, EmployeeRole, Task, IEmployeeService

### Community 47 - "Auth Plan: ApplicationUser/Role Entities"
Cohesion: 0.36
Nodes (13): ApplicationRole Entity, ApplicationUser Entity, Authentication & Authorization Plan, Authentication & Authorization Research, Endpoint-to-Role Authorization Matrix, Caller-Identity Context via Parameters, EmployeeId Custom Claim, Global FallbackPolicy Deny-by-Default (+5 more)

### Community 48 - "Spec Kit Constitution & Hooks"
Cohesion: 0.47
Nodes (12): Project Constitution Document, /speckit.analyze Command, Extension Hooks Mechanism, /speckit.checklist Command, /speckit.clarify Command, /speckit.constitution Command, /speckit.converge Command, /speckit.implement Command (+4 more)

### Community 49 - "Pagination Contract Reuse"
Cohesion: 0.20
Nodes (12): 002-paginate-shipment-list Module (Pagination Contract), GET /api/employees, GET /api/vehicles, CreateVehicleDtoValidator, UpdateEmployeeDtoValidator, UpdateVehicleDtoValidator, CreateEmployeeAsync/UpdateEmployeeAsync Validation Flow, Implementation Plan: Employees & Vehicles Module (+4 more)

### Community 50 - "Customers Quickstart Scenarios"
Cohesion: 0.17
Nodes (12): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Orders Module, User Story 1 — Create a new order (P1), User Story 2 — Find and review orders (P2) (+4 more)

### Community 51 - "ShipmentEvent Requirements & Status"
Cohesion: 0.32
Nodes (12): Status-Transition InvalidOperationException Pattern, Shipment Tracking Events Requirements Checklist, Shipment Tracking Events Data Model, DeliveryAttempt Entity, ShipmentStatus.OutForDelivery, ShipmentEventType Enum, Shipment Tracking Events Plan, Shipment Tracking Events Quickstart (+4 more)

### Community 52 - "speckit-clarify Ambiguity Taxonomy"
Cohesion: 0.20
Nodes (11): /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section, /speckit-clarify Command, Checklist Status Gate, Spec Quality Checklist, /speckit-specify Command (+3 more)

### Community 53 - "Identity Infra Namespaces"
Cohesion: 0.18
Nodes (6): ShipmentTracker.Infrastructure.Data.Seed, ShipmentTracker.Core.Identity, ShipmentTracker.Infrastructure.Identity, IServiceProvider, Task, IdentitySeeder

### Community 54 - "speckit-plan Command Spec"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, Key rules, Mandatory Post-Execution Hooks, Outline, Phase 0: Outline & Research, Phase 1: Design & Contracts, Phases (+2 more)

### Community 55 - "speckit-specify Command Spec"
Cohesion: 0.18
Nodes (10): Completion Report, Done When, For AI Generation, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, Quick Guidelines, Section Requirements (+2 more)

### Community 56 - "speckit-tasks Command Spec"
Cohesion: 0.18
Nodes (10): Checklist Format (REQUIRED), Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Phase Structure, Pre-Execution Checks, Task Generation Rules (+2 more)

### Community 57 - "Employee Quickstart & POST"
Cohesion: 0.22
Nodes (11): 003-branches-hubs Module (Branch entity), Specification Quality Checklist: Employees & Vehicles Module, POST /api/employees, CreateEmployeeDto, Decisión 10: BranchId NO se hace nullable, Decisión 11: JsonStringEnumConverter por propiedad para Role/Type, Decisión 15: Reactivación vía PUT (mismo precedente que Branch), Decisión 7: Unicidad sin filtrar por IsActive (índice único simple) (+3 more)

### Community 58 - "Branch Requirements & Decisions"
Cohesion: 0.25
Nodes (11): Edge Cases Checklist: Branches & Hubs Module, POST /api/branches, Decision 13: opensAt/closesAt require HH:mm:ss format, Decision 2: 7-day coverage without explicit 'all days present' rule, Decision 9: Validation errors via FluentValidation.ValidationException with full error list, FR-001: Create branch with name, type, full address, FR-004: Exactly one schedule entry per day of week (7 total), FR-005: Reject duplicate day entries in schedule (+3 more)

### Community 59 - "Branch GET Endpoints"
Cohesion: 0.25
Nodes (11): GET /api/branches/{id}, GET /api/branches, Decision 11: GetByIdWithScheduleAsync new repository method, Decision 7: onlyActive bool default true covers listing cases, FR-013: List branches with optional status/type filters, FR-014: Default listing returns only active branches, FR-015: Single branch retrieval always includes complete schedule, FR-016: Clear not-found result for missing branch id (+3 more)

### Community 60 - "Employee/Vehicle List Filters (FRs)"
Cohesion: 0.18
Nodes (11): FR-009: List Employees filtered by Branch and/or Role, FR-010: Only active Employees in listings, FR-019: List Vehicles filtered by Branch, FR-020: Only active Vehicles in listings, FR-023: Retrieve single Employee by identifier, FR-024: Retrieve single Vehicle by identifier, SC-005: Deactivated records never returned, never deleted, US2: Find drivers available at a branch (P2) (+3 more)

### Community 61 - "Customers Quickstart Steps"
Cohesion: 0.18
Nodes (10): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating the Customers & Accounts Module, User Story 1 — Register a new customer (P1), User Story 2 — Find and review customers (P2) (+2 more)

### Community 62 - "ShipmentEvent API Contract"
Cohesion: 0.18
Nodes (11): Shipment Tracking Events API Contract, Examples, `GET /api/shipments/{id}/events`, `GET /api/shipments/tracking/{trackingNumber}`, HTTP Contract: Shipment Tracking Events API, JSON format notes, `POST /api/shipments/{id}/events`, `POST /api/shipments/{id}/events/delivery-attempt` (+3 more)

### Community 63 - "Auth DTOs & Validators (Login/User)"
Cohesion: 0.24
Nodes (5): ShipmentTracker.Core.DTOs.Auth, ShipmentTracker.Services.Validators.Auth, LoginDto, CreateUserDtoValidator, LoginDtoValidator

### Community 64 - "Branch Entity & Migrations"
Cohesion: 0.22
Nodes (10): Branch Entity, AppDbContext (extended), BranchConfiguration, BranchScheduleConfiguration, AddBranchesAndSchedule Migration, Decision 10: Latitude/Longitude as nullable double, validated only when present, FR-002: Optional coordinates and phone, FR-003: New branch marked active by default (+2 more)

### Community 65 - "Employee Repository Interface"
Cohesion: 0.24
Nodes (7): DateOnly, DateTime, Employee, IEmployeeRepository, EntityTypeBuilder, EmployeeConfiguration, EmployeeRepository

### Community 66 - "ShipmentEvent Repository Interface"
Cohesion: 0.24
Nodes (7): DateTime, Employee, ShipmentEvent, IShipmentEventRepository, EntityTypeBuilder, ShipmentEventConfiguration, ShipmentEventRepository

### Community 67 - "Project Constitution Principles"
Cohesion: 0.20
Nodes (9): Core Principles, Flujo de Trabajo de Desarrollo, Governance, I. Framework Objetivo Único (.NET 8.0), II. Integridad de la Arquitectura en Capas, III. Minimalismo de Dependencias, IV. Cambios Pequeños y Reversibles, Restricciones Técnicas y de Arquitectura (+1 more)

### Community 68 - "Orders Spec Quality Checklist"
Cohesion: 0.24
Nodes (6): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Orders Module, ConvertToShipmentAsync Operation, Order Entity

### Community 69 - "Orders Research: Sequencing & FK"
Cohesion: 0.24
Nodes (9): Navigation-Based FK Fixup Pattern, Order/Tracking Number Sequence Generation, Shipment.OrderId Nullable FK, Structural vs DB-Dependent Validation Split, Shipment Tracking Events Research, AttemptNumber Sequencing Pattern, No DTO Inheritance Convention, No Pagination for Bounded Child Collections (+1 more)

### Community 70 - "IdentityDbContext & Core Entities"
Cohesion: 0.22
Nodes (8): Branch, BranchSchedule, DbSet, IdentityDbContext, Employee, ModelBuilder, AppDbContext, Vehicle

### Community 71 - "Initial Migration"
Cohesion: 0.28
Nodes (4): ShipmentTracker.Infrastructure.Migrations, MigrationBuilder, ModelBuilder, InitialModel

### Community 72 - "Seed Initial Data Migration"
Cohesion: 0.25
Nodes (4): Migration, MigrationBuilder, ModelBuilder, SeedInitialData

### Community 73 - "Vehicle Service Async Ops"
Cohesion: 0.42
Nodes (4): DateTime, VehicleDto, Task, IVehicleService

### Community 74 - "Order Repository & Config"
Cohesion: 0.28
Nodes (6): DateTime, Order, IOrderRepository, EntityTypeBuilder, OrderConfiguration, OrderRepository

### Community 75 - "Shipment Repository & Config"
Cohesion: 0.28
Nodes (6): DateTime, Shipment, IShipmentRepository, EntityTypeBuilder, ShipmentConfiguration, ShipmentRepository

### Community 76 - "Vehicle Repository & Config"
Cohesion: 0.28
Nodes (6): DateTime, Vehicle, IVehicleRepository, EntityTypeBuilder, VehicleConfiguration, VehicleRepository

### Community 77 - "Employee Update Endpoint"
Cohesion: 0.25
Nodes (9): GET /api/employees/{id}, PUT /api/employees/{id}, EmployeeDto, IEmployeeService, UpdateEmployeeDto, EmployeeMappingProfile, FR-005: Update Employee fields incl. Branch reassignment, unrestricted role change, US5: Update employee information (P5) (+1 more)

### Community 78 - "Employee Create Requirements"
Cohesion: 0.25
Nodes (9): CreateEmployeeDtoValidator, EmployeeController, EmployeeService, FR-001: Create Employee with name, unique number/email, role, hire date, branch, FR-002: Reject Employee create/update on duplicate number/email (company-wide, incl. inactive), FR-003: Reject Employee create/update if Branch missing/inactive, FR-004: New Employee active by default, US1: Register a new employee (P1) (+1 more)

### Community 79 - "Orders Implementation Plan"
Cohesion: 0.22
Nodes (8): Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Orders Module, Project Structure, Source Code (repository root), Summary, Technical Context

### Community 80 - "Orders Quickstart Scenarios"
Cohesion: 0.22
Nodes (9): 1. Build and run, 2. Scenarios to validate (map 1:1 to `spec.md`'s Acceptance Scenarios), 3. Verify nothing else changed, Additional edge cases, Prerequisites, Quickstart: Validating Shipment Tracking Events, User Story 1 — Mark a shipment out for delivery (P1), User Story 2 — Log a failed delivery attempt (P2) (+1 more)

### Community 81 - "Auth Spec: Checklist & Driver Derivation"
Cohesion: 0.42
Nodes (9): Authentication & Authorization Requirements Checklist, Authentication & Authorization Data Model, Authentication & Authorization Quickstart, Driver Assigned-Shipment Derivation, Authentication & Authorization Spec, Account Lockout Policy, ApplicationUser Email Sync, SuperAdmin-Only Account Provisioning (+1 more)

### Community 82 - "graphify Add/Watch Reference"
Cohesion: 0.25
Nodes (6): graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, --cluster-only, --update Incremental Re-extraction, Incremental Update & Cluster-Only Reference

### Community 83 - "graphify Extraction Prompt Spec"
Cohesion: 0.25
Nodes (8): Confidence Score Rubric, Extraction Subagent Prompt Spec, Hyperedges Rule, Node ID Format Rule, Semantic Similarity Edge Rule, Convergence Findings, Gap Type Classification (missing/partial/contradicts/unrequested), NEEDS CLARIFICATION Markers

### Community 84 - "speckit-analyze Severity & Phases"
Cohesion: 0.29
Nodes (8): Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, Phase 0: Outline & Research, Phase 1: Design & Contracts, /speckit-plan Command, Constitution Check Gate, Implementation Plan Template

### Community 85 - "Auth Authorization Handlers"
Cohesion: 0.25
Nodes (5): AuthorizationHandlerContext, ShipmentTracker.Web.Authorization, IAuthorizationHandler, Task, SuperAdminAuthorizationHandler

### Community 86 - "speckit-checklist Guidance"
Cohesion: 0.25
Nodes (7): Anti-Examples: What NOT To Do, Checklist Purpose: "Unit Tests for English", Example Checklist Types & Sample Items, Execution Steps, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 87 - "Branches/Schedule Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddBranchesAndSchedule

### Community 88 - "Employees/Vehicles Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddEmployeesAndVehicles

### Community 89 - "Customers Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddCustomers

### Community 90 - "Orders/ShipmentEvents Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddOrdersAndShipmentEvents

### Community 91 - "DeliveryAttempts Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, ExtendShipmentEventsAndAddDeliveryAttempts

### Community 92 - "Identity Tables Migration"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddIdentityTables

### Community 93 - "Branches Module Docs"
Cohesion: 0.57
Nodes (8): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module, Feature Specification: Branches & Hubs Module, Tasks: Branches & Hubs Module

### Community 94 - "Vehicle Create Requirements"
Cohesion: 0.25
Nodes (8): VehicleController, VehicleService, Decisión 12: BranchId/role como filtro de query string sin conversor especial, FR-011: Create Vehicle with unique plate, type, brand, model, year, capacity, branch, FR-012: Reject Vehicle create/update on duplicate plate (company-wide, incl. inactive), FR-013: Reject Vehicle create/update if Branch missing/inactive, US3: Register a new vehicle (P3), Phase 5: User Story 3 Tasks (Register Vehicle)

### Community 95 - "ShipmentEvent Implementation Plan"
Cohesion: 0.25
Nodes (8): Complexity Tracking, Constitution Check, Documentation (this feature), Implementation Plan: Shipment Tracking Events, Project Structure, Source Code (repository root), Summary, Technical Context

### Community 96 - "graphify Export & Benchmark Reference"
Cohesion: 0.29
Nodes (7): Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server, Neo4j Export, Build Graph & Cluster (Step 4), Community Labeling (Step 5)

### Community 97 - "speckit-clarify Command Spec"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 98 - "speckit-implement Command Spec"
Cohesion: 0.29
Nodes (6): Completion Report, Done When, Mandatory Post-Execution Hooks, Outline, Pre-Execution Checks, User Input

### Community 99 - "Branch Schedule DTOs"
Cohesion: 0.38
Nodes (7): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, BranchType Enum, ScheduleDay Enum, Decision 1: Nullable Type/DayOfWeek enums in input DTOs

### Community 100 - "BranchSchedule Entity & Rules"
Cohesion: 0.33
Nodes (7): BranchSchedule Entity, Decision 3: Closed-day vs schedule-times consistency, Decision 5: Full schedule replacement via EF Core cascade delete, FR-006: Opening time strictly earlier than closing time, FR-007: Schedule entry may be marked closed with no times, FR-017: Reject inconsistent closed+times schedule submission, FR-019: Schedule times are branch-local time-of-day, no time zone

### Community 101 - "graphify Query/Path/Explain Reference"
Cohesion: 0.40
Nodes (6): /graphify explain, /graphify path, Query, Path, Explain Reference, save-result Work Memory Loop, BFS/DFS Graph Traversal, Constrained Query Vocab Expansion

### Community 102 - "Spec Kit Command Index"
Cohesion: 0.40
Nodes (6): /speckit-converge Command, /speckit-implement Command, /speckit-tasks Command, Task Generation Rules, /speckit-taskstoissues Command, Task ID Dedup Regex Matching

### Community 103 - "speckit-constitution Command Spec"
Cohesion: 0.33
Nodes (5): Outline, Post-Execution Checks, Pre-Execution Checks, Scope Guard, User Input

### Community 104 - "Branch Update Endpoint"
Cohesion: 0.47
Nodes (6): UpdateBranchDto, PUT /api/branches/{id}, Decision 4: BranchSchedule has no own repository/controller/service, FR-008: Update allowed on any field regardless of active status, FR-009: Re-validate all rules on update; reject leaving prior data unchanged, User Story 3: Update branch information (P3)

### Community 106 - "Branch Deactivation Requirements"
Cohesion: 0.47
Nodes (6): DELETE /api/branches/{id}, Decision 8: DELETE is soft-delete only, no physical delete, FR-011: Deactivating an already-inactive branch is a no-op, FR-012: No capability to permanently delete a branch, SC-004: Deactivated branches retain 100% of data with zero loss, User Story 4: Deactivate a branch (P4)

### Community 107 - "Employee Session Validator Logic"
Cohesion: 0.70
Nodes (3): CookieValidatePrincipalContext, Task, EmployeeSessionValidator

### Community 108 - "EF Model Snapshot"
Cohesion: 0.40
Nodes (3): ModelSnapshot, ModelBuilder, AppDbContextModelSnapshot

### Community 109 - "speckit-taskstoissues Command Spec"
Cohesion: 0.40
Nodes (4): Outline, Post-Execution Checks, Pre-Execution Checks, User Input

### Community 110 - "ShipmentEvent Spec Quality Checklist"
Cohesion: 0.50
Nodes (4): Content Quality, Feature Readiness, Requirement Completeness, Specification Quality Checklist: Shipment Tracking Events

### Community 111 - "Constitution Template & Sync"
Cohesion: 0.67
Nodes (3): /speckit-constitution Command, Sync Impact Report, Constitution Template

## Knowledge Gaps
- **478 isolated node(s):** `graphify merge-graphs`, `graphify claude install (CLAUDE.md integration)`, `Whisper Domain-Hint Prompt`, `Cumulative Cost Tracker (Step 9)`, `Extraction Cache (Step B0)` (+473 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **11 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ShipmentTracker.Core.Entities` connect `Core Entities & Order DTOs` to `Employee Repository Interface`, `ShipmentEvent Repository Interface`, `Branch Entity Relations`, `Repository Implementations`, `Order Repository & Config`, `Shipment Repository & Config`, `EF Entity Configurations`, `Vehicle Repository & Config`, `Infrastructure Data Layer Namespaces`, `AutoMapper Mapping Profiles`, `Identity Infra Namespaces`, `Shipment Validators & Interfaces`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Core.Enums` connect `Core Entities & Order DTOs` to `Customer DTOs & Validators`, `Order Conversion DTOs`, `Delivery Attempt DTOs`, `Vehicle DTOs & Validators`, `Branch Validators & DTOs`, `Shipment Controller & DTOs`, `Employee DTOs (Create)`, `Roles Constants & Web Controllers`, `EmployeeRole Enum Usage`, `AutoMapper Mapping Profiles`, `ShipmentEvent DTOs & Validators`, `Shipment Validators & Interfaces`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **Why does `ShipmentTracker.Infrastructure.Data` connect `Infrastructure Data Layer Namespaces` to `Initial Migration`, `Seed Initial Data Migration`, `EF Model Snapshot`, `Identity Infra Namespaces`, `Branches/Schedule Migration`, `Employees/Vehicles Migration`, `Customers Migration`, `Orders/ShipmentEvents Migration`, `DeliveryAttempts Migration`, `Identity Tables Migration`, `Shipment Validators & Interfaces`?**
  _High betweenness centrality (0.034) - this node is a cross-community bridge._
- **What connects `graphify merge-graphs`, `graphify claude install (CLAUDE.md integration)`, `Whisper Domain-Hint Prompt` to the rest of the system?**
  _478 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Project Conventions & Docs` be split into smaller, more focused modules?**
  _Cohesion score 0.0528169014084507 - nodes in this community are weakly interconnected._
- **Should `Customer DTOs & Validators` be split into smaller, more focused modules?**
  _Cohesion score 0.06223358908780904 - nodes in this community are weakly interconnected._
- **Should `Order Conversion DTOs` be split into smaller, more focused modules?**
  _Cohesion score 0.06971153846153846 - nodes in this community are weakly interconnected._