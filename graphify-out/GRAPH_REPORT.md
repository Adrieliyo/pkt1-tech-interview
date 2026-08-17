# Graph Report - ShipmentTracker  (2026-08-17)

## Corpus Check
- 122 files · ~73,328 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 538 nodes · 972 edges · 20 communities (19 shown, 1 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 32 edges (avg confidence: 0.78)
- Token cost: 411,792 input · 45,753 output

## Community Hubs (Navigation)
- SDD Workflow & Conventions
- Entities & EF Configuration
- Shipment Services & Validators
- Branch DTOs & Entities
- Branch Controller Actions
- graphify Add & Export Tools
- Speckit Analyze & Checklist
- Shipment Pagination & DTOs
- EF Core Migrations
- Launch Settings Config
- NuGet Dependencies & Solution
- Base Repository Generics
- LINQ Expression Generics
- Branches & Hubs Spec Docs
- Spec Kit PowerShell Helpers
- Feature-Branch Creation Script

## God Nodes (most connected - your core abstractions)
1. `ShipmentTracker.Core.DTOs` - 21 edges
2. `ShipmentTracker.Core.Enums` - 19 edges
3. `BranchDto` - 17 edges
4. `BaseRepository` - 17 edges
5. `001 Implementation Plan` - 17 edges
6. `ShipmentTracker.Core.Entities` - 16 edges
7. `ShipmentTracker Constitution` - 16 edges
8. `002 Implementation Plan` - 16 edges
9. `IBaseRepository` - 15 edges
10. `/graphify Skill` - 14 edges

## Surprising Connections (you probably didn't know these)
- `Shipment List Pagination Mechanism` --semantically_similar_to--> `Per-Property JSON Enum Serialization Gotcha`  [INFERRED] [semantically similar]
  specs/002-paginate-shipment-list/spec.md → CLAUDE.md
- `CreateBranchDtoValidator` --references--> `CreateBranchDto`  [EXTRACTED]
  ShipmentTracker.Services/Validators/CreateBranchDtoValidator.cs → ShipmentTracker.Core/DTOs/CreateBranchDto.cs
- `UpdateBranchDtoValidator` --references--> `UpdateBranchDto`  [EXTRACTED]
  ShipmentTracker.Services/Validators/UpdateBranchDtoValidator.cs → ShipmentTracker.Core/DTOs/UpdateBranchDto.cs
- `StatusTransitionContext` --references--> `ShipmentStatus`  [EXTRACTED]
  ShipmentTracker.Services/Validators/ShipmentTransitionValidator.cs → ShipmentTracker.Core/Enums/ShipmentStatus.cs
- `BranchService` --references--> `IUnitOfWork`  [EXTRACTED]
  ShipmentTracker.Services/BranchService.cs → ShipmentTracker.Core/Interfaces/IUnitOfWork.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Spec-Driven Development Workflow Pipeline** — _claude_skills_speckit_specify_skill_speckit_specify, _claude_skills_speckit_clarify_skill_speckit_clarify, _claude_skills_speckit_plan_skill_speckit_plan, _claude_skills_speckit_tasks_skill_speckit_tasks, _claude_skills_speckit_implement_skill_speckit_implement, _claude_skills_speckit_analyze_skill_speckit_analyze, _claude_skills_speckit_converge_skill_speckit_converge [EXTRACTED 1.00]
- **Graphify AST+Semantic Extraction & Merge Pipeline** — _claude_skills_graphify_skill_ast_extraction, _claude_skills_graphify_skill_semantic_extraction, _claude_skills_graphify_skill_extraction_cache, _claude_skills_graphify_skill_merge_extraction, _claude_skills_graphify_references_extraction_spec_extraction_spec [EXTRACTED 1.00]
- **ShipmentTracker Constitution Core Principles** — _specify_memory_constitution_principle_i_framework_unico, _specify_memory_constitution_principle_ii_arquitectura_capas, _specify_memory_constitution_principle_iii_minimalismo_dependencias, _specify_memory_constitution_principle_iv_cambios_pequenos [EXTRACTED 1.00]
- **Spec-Driven Development Artifact Set (Feature 001)** — specs_001_standardize_mapping_di_spec, specs_001_standardize_mapping_di_plan, specs_001_standardize_mapping_di_tasks, specs_001_standardize_mapping_di_research, specs_001_standardize_mapping_di_data_model [INFERRED 0.85]
- **Spec-Driven Development Artifact Set (Feature 002)** — specs_002_paginate_shipment_list_spec, specs_002_paginate_shipment_list_plan, specs_002_paginate_shipment_list_tasks, specs_002_paginate_shipment_list_research, specs_002_paginate_shipment_list_data_model [INFERRED 0.85]
- **ShipmentService Constructor Dependency Evolution** — shipmenttracker_services_shipmentservice_shipmentservice, automapper, fluentvalidation, shipmenttracker_core_dtos_pagedresult_pagedresult [INFERRED 0.75]
- **Soft-Delete Deactivation Pattern** — shipmenttracker_core_entities_branch_branch, specs_003_branches_hubs_spec_fr011, specs_003_branches_hubs_spec_fr012, specs_003_branches_hubs_research_decision8, specs_003_branches_hubs_contracts_branches_api_contract_delete_branch [EXTRACTED 1.00]
- **Full Weekly Schedule Replacement Flow** — shipmenttracker_core_entities_branchschedule_branchschedule, shipmenttracker_services_branchservice_branchservice, specs_003_branches_hubs_research_decision5, specs_003_branches_hubs_spec_fr009, specs_003_branches_hubs_contracts_branches_api_contract_put_branch [EXTRACTED 1.00]
- **Nullable Enum Omission-Detection Pattern** — specs_003_branches_hubs_research_decision1, shipmenttracker_core_dtos_createbranchdto_createbranchdto, shipmenttracker_core_dtos_scheduleentryinputdto_scheduleentryinputdto, shipmenttracker_core_enums_branchtype_branchtype, shipmenttracker_core_enums_scheduleday_scheduleday [EXTRACTED 1.00]

## Communities (20 total, 1 thin omitted)

### Community 0 - "SDD Workflow & Conventions"
Cohesion: 0.08
Nodes (61): Spec Kit Full SDD Cycle Workflow, AutoMapper, Clean Architecture Layering, Project Constitution (.specify/memory/constitution.md), AllowReactApp CORS Policy, Constructor Dependency Injection Convention, .NET 8, graphify Knowledge Graph (+53 more)

### Community 1 - "Entities & EF Configuration"
Cohesion: 0.06
Nodes (32): ShipmentTracker.Core.Interfaces.Repositories, ShipmentTracker.Infrastructure.Data.Configurations, ShipmentTracker.Core.Entities, ShipmentTracker.Infrastructure.Repositories, DbContext, ICollection, IDisposable, IEntityTypeConfiguration (+24 more)

### Community 2 - "Shipment Services & Validators"
Cohesion: 0.08
Nodes (25): AbstractValidator, ShipmentTracker.Core.DTOs, ShipmentTracker.Services.Validators, ShipmentTracker.Web.Mappers, ShipmentTracker.Services, ShipmentTracker.Services.Mappings, ShipmentTracker.Web.Controllers, ShipmentTracker.Core.Interfaces (+17 more)

### Community 3 - "Branch DTOs & Entities"
Cohesion: 0.07
Nodes (47): BranchDto, CreateBranchDto, ScheduleEntryDto, ScheduleEntryInputDto, UpdateBranchDto, Branch Entity, BranchSchedule Entity, BranchType Enum (+39 more)

### Community 4 - "Branch Controller Actions"
Cohesion: 0.09
Nodes (27): ControllerBase, HttpDelete, HttpPut, DateTime, List, BranchDto, List, CreateBranchDto (+19 more)

### Community 5 - "graphify Add & Export Tools"
Cohesion: 0.06
Nodes (39): CLAUDE.md Graphify Directive, graphify add & --watch Reference, /graphify add Command, --watch Background Watcher, Token Reduction Benchmark, Extra Exports & Benchmark Reference, FalkorDB Export, graphify.serve MCP Server (+31 more)

### Community 6 - "Speckit Analyze & Checklist"
Cohesion: 0.08
Nodes (38): Confidence Score Rubric, Honesty Rules, Analyze Severity Assignment Heuristic, /speckit-analyze Command, /speckit-checklist Command, "Unit Tests for English" Concept, Ambiguity & Coverage Taxonomy, Clarifications Section (+30 more)

### Community 7 - "Shipment Pagination & DTOs"
Cohesion: 0.11
Nodes (21): HttpPatch, int, CreateShipmentDto, IEnumerable, PagedResult, DateTime, ShipmentDto, ShipmentStatus (+13 more)

### Community 8 - "EF Core Migrations"
Cohesion: 0.08
Nodes (15): ShipmentTracker.Infrastructure.Migrations, ShipmentTracker.Infrastructure.Data, Migration, ModelSnapshot, MigrationBuilder, ModelBuilder, InitialModel, MigrationBuilder (+7 more)

### Community 9 - "Launch Settings Config"
Cohesion: 0.07
Nodes (28): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, launchUrl, applicationUrl (+20 more)

### Community 10 - "NuGet Dependencies & Solution"
Cohesion: 0.11
Nodes (22): FluentValidation (11.9.0), Microsoft.AspNetCore.OpenApi (8.0.19), Microsoft.EntityFrameworkCore (8.0.*), Microsoft.EntityFrameworkCore.SqlServer (8.0.*), Microsoft.EntityFrameworkCore.Tools (8.0.29), Swashbuckle.AspNetCore (6.6.2), Microsoft.NET.Sdk.Web, ShipmentTracker.Core (+14 more)

### Community 11 - "Base Repository Generics"
Cohesion: 0.18
Nodes (9): DbSet, Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask (+1 more)

### Community 12 - "LINQ Expression Generics"
Cohesion: 0.19
Nodes (8): Expression, Func, IEnumerable, IOrderedQueryable, IQueryable, Task, ValueTask, IBaseRepository

### Community 13 - "Branches & Hubs Spec Docs"
Cohesion: 0.18
Nodes (19): Specification Quality Checklist: Branches & Hubs Module, Contrato HTTP: API de Branches & Hubs, GET /api/branches/{id}, GET /api/branches, Data Model: Branches & Hubs Module, Implementation Plan: Branches & Hubs Module, Quickstart: Validar el módulo de Branches & Hubs, Research: Branches & Hubs Module (+11 more)

### Community 14 - "Spec Kit PowerShell Helpers"
Cohesion: 0.23
Nodes (13): Find-SpecifyRoot(), Format-SpecKitCommand(), Get-CurrentBranch(), Get-FeaturePathsEnv(), Get-InvokeSeparator(), Get-NormalizedPriority(), Get-Python3Command(), Get-RepoRoot() (+5 more)

## Knowledge Gaps
- **82 isolated node(s):** `net8.0`, `Microsoft.NET.Sdk`, `net8.0`, `Microsoft.EntityFrameworkCore (8.0.*)`, `Microsoft.EntityFrameworkCore.Design (8.0.*)` (+77 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **1 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `FluentValidation` connect `Shipment Services & Validators` to `SDD Workflow & Conventions`?**
  _High betweenness centrality (0.165) - this node is a cross-community bridge._
- **Why does `Decision 12: JSON representation of BranchType/ScheduleDay as enum name, scoped converter` connect `SDD Workflow & Conventions` to `Branch DTOs & Entities`?**
  _High betweenness centrality (0.085) - this node is a cross-community bridge._
- **What connects `net8.0`, `Microsoft.NET.Sdk`, `net8.0` to the rest of the system?**
  _82 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `SDD Workflow & Conventions` be split into smaller, more focused modules?**
  _Cohesion score 0.08302485457429931 - nodes in this community are weakly interconnected._
- **Should `Entities & EF Configuration` be split into smaller, more focused modules?**
  _Cohesion score 0.06127946127946128 - nodes in this community are weakly interconnected._
- **Should `Shipment Services & Validators` be split into smaller, more focused modules?**
  _Cohesion score 0.0841813135985199 - nodes in this community are weakly interconnected._
- **Should `Branch DTOs & Entities` be split into smaller, more focused modules?**
  _Cohesion score 0.0666049953746531 - nodes in this community are weakly interconnected._