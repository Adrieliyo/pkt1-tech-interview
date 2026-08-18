# Research: Orders Module

Most `NEEDS CLARIFICATION` risk in the Technical Context is already resolved by the user's explicit
`/speckit-plan` input (stack, entity fields, endpoints, and the exact `ConvertToShipmentAsync` step
sequence) and by `spec.md`'s Clarifications (the Order→Shipment data-model decision). This document
records the technical decisions derived from that input, from direct inspection of the existing
`Shipment` module (`001`/`002`) and `Customer`/`Branch`/`Employee` modules, and — critically — from
one direct conflict between the plan input and the already-agreed spec that this document resolves in
favor of the spec, plus one conflict forced by the project's own seed data.

## Decision 1: `Shipment` stays in its existing minimal shape — the plan input's "copying quoted_price, recipient data, branch, customer" is not implemented

- **Decision**: Converting an Order does **not** copy `quoted_price`, `customerId`, `originBranchId`,
  or a recipient-field breakdown onto `Shipment`. `Shipment` gains exactly one new column,
  `OrderId` (see Decision 2 for nullability), and its pre-existing `Recipient` field is populated from
  `Order.RecipientName` — reusing an already-existing required field, not adding a new one.
- **Rationale**: This directly contradicts the plan input's literal wording ("Create Shipment entity
  (copying quoted_price, recipient data, branch, customer)"), but `spec.md`'s Clarifications session —
  answered by the user in this same conversation, immediately before this planning step — explicitly
  chose "keep `Shipment` in its current minimal shape... add only a required `OrderId` back-reference...
  full detail is retrieved by following that reference to the Order" over the alternative of extending
  `Shipment`. The plan input's phrasing reads as carried over from a generic/earlier draft that predates
  that clarification decision, rather than a deliberate reversal of an answer the user gave moments
  earlier in the same session. Per the precedent set in modules `004` (Decision 4) and `005` (Decision 5)
  — an already-agreed, freshly-clarified spec decision wins over a conflicting planning-input detail —
  this plan follows the spec's explicit answer.
- **Alternatives considered**: Implementing the plan input literally (extend `Shipment` with
  `CustomerId`, `OriginBranchId`, `QuotedPrice`, and a recipient-field breakdown) — rejected: it would
  silently reverse a decision the user just made, and would turn this module into a large, cross-cutting
  change to the already-shipped `Shipment` entity (modules `001`/`002`), which is exactly what Option B
  was chosen to avoid.

## Decision 2: `Shipment.OrderId` must be nullable — forced by existing seed data and the still-active direct-creation endpoint

- **Decision**: `Shipment.OrderId` is `int?` (nullable FK to `Order`), with a unique index that allows
  multiple `NULL`s (SQL Server's default unique-index behavior). It is populated only when a Shipment
  is created via `OrderService.ConvertToShipmentAsync`; it stays `null` for Shipments created through
  the pre-existing `POST /api/shipment` endpoint (module `001`/`002`, untouched by this module).
- **Rationale**: This is not a preference — it is forced by two concrete facts already in the codebase:
  (1) migration `SeedInitialData` (`20260728103816`) inserted 5 `Shipment` rows (`TRK-90001`...
  `TRK-90005`) with no concept of an `Order` at all, and a `NOT NULL` `OrderId` column cannot be added
  to a table with existing rows without a default value that would fabricate a fake Order reference;
  (2) `ShipmentController.CreateShipment` / `IShipmentService.CreateShipmentAsync` remain fully
  functional and unmodified by this module (nothing in `spec.md` or the plan input asks to remove or
  gate that endpoint), so new Shipments with no Order will keep being created going forward, too.
- **Alternatives considered**: Required (`NOT NULL`) `OrderId`, with a data migration backfilling the 5
  seeded rows against synthetic placeholder Orders — rejected: fabricating Orders for pre-existing,
  unrelated legacy Shipments would misrepresent real data and is exactly the kind of out-of-scope,
  non-additive change Principio IV warns against.

## Decision 3: Two coexisting tracking-number formats on `Shipment` — no unification attempted

- **Decision**: `Shipment.TrackingNumber` values generated via `POST /api/shipment` keep their existing
  format (`TRK-` + 8 hex chars from a GUID, per `ShipmentService.CreateShipmentAsync`, itself already
  inconsistent with the even older seeded values like `TRK-90001`). Shipments generated via
  `OrderService.ConvertToShipmentAsync` use the new `TRK-YYYYMMDD-XXXX` format specified in the plan
  input. Both are enforced unique by the same pre-existing unique index on `TrackingNumber`.
- **Rationale**: Unifying the formats would mean modifying `ShipmentService.CreateShipmentAsync` (an
  existing, working module) with no request to do so — out of scope for an additive module. The
  existing unique index already guarantees no collision between the two formats regardless.
- **Alternatives considered**: Changing `ShipmentService.CreateShipmentAsync` to also use the
  `TRK-YYYYMMDD-XXXX` pattern for consistency — rejected as unrequested scope creep into module
  `001`/`002`.

## Decision 4: Recipient/destination address has no `Country` field — plan input is more specific than the spec's Assumption

- **Decision**: `Order`'s recipient/destination fields are exactly the five the plan input lists:
  `RecipientName`, `RecipientPhone`, `RecipientAddress`, `RecipientCity`, `RecipientState`,
  `RecipientZipCode` — no `RecipientCountry`.
- **Rationale**: `spec.md`'s Assumptions section defaulted to the full Branch/Customer address shape
  (including country) in the absence of more detail at specify-time. The plan input now gives an
  explicit, field-by-field list without a country column — a more specific technical decision made
  with more context than the earlier high-level assumption, the same category of refinement as module
  `004`'s Decision 1 (splitting `name` into `FirstName`/`LastName` because the plan input was more
  specific than `spec.md`'s placeholder assumption). This does not contradict any FR — it only
  concretizes how the "destination" field is implemented.
- **Alternatives considered**: Keeping `RecipientCountry` per the spec's original Assumption — rejected
  in favor of the more detailed, more recent plan input, consistent with precedent.

## Decision 5: `ServiceType`, `QuotedPrice`, and `Notes` — additive fields beyond the original spec, accepted as-is

- **Decision**: `Order` includes `ServiceType` (enum: `Standard`/`Express`/`Economy`), `QuotedPrice`
  (`decimal`, required, must be `>= 0`), and `Notes` (`string?`, optional free text) exactly as listed
  in the plan input, even though none of the three appears in `spec.md`.
- **Rationale**: Same pattern as module `004`'s Decision 2 (`Employee.Phone` added at planning time
  despite not being in `spec.md`): additive, low-risk fields introduced with more technical context at
  planning time, none of which conflicts with any functional requirement already agreed. `QuotedPrice`
  gets a non-negative structural rule for the same reason `BusinessCustomer.CreditLimit` does (module
  `005`) — a monetary field with no stated reason to ever be negative.
- **Alternatives considered**: Omitting the three fields for being outside `spec.md`'s original scope —
  rejected; they are additive, non-contradictory, and the user supplied them with explicit technical
  detail in this planning step.

## Decision 6: `ShipmentEvent.EventType` — a string-persisted C# enum, starting with one member

- **Decision**: `ShipmentEvent.EventType` is a new `ShipmentEventType` enum (`Core/Enums`), persisted as
  `string` (`HasConversion<string>()`, same convention as every other enum in this solution), with a
  single member for this module: `OrderConverted`.
- **Rationale**: `spec.md`'s Key Entities section says `ShipmentEvent`'s type "is modeled openly so
  future modules can register further event types... without a structural change" — read together with
  this project's established, unbroken convention that every status/type concept is a C# enum
  persisted as a plain string column (never a free-text field), the right reading is "no *database*
  schema change is needed to add a new value" (true here: the column is already a plain `nvarchar`) —
  not "avoid enums altogether," which would be the first exception to that convention in this solution
  and would compete with the already-adopted pattern (`CLAUDE.md`'s architecture guidance: don't
  introduce a competing pattern for an already-solved concern). A future module adding a new event type
  extends this enum in code — a normal, expected step, not a database migration.
- **Alternatives considered**: A free-text `string EventType` column with no enum — rejected as the
  first departure from this project's otherwise-universal typed-enum-for-status-fields convention, for
  a benefit (avoiding a future one-line enum addition) that does not outweigh the consistency cost.

## Decision 7: `ShipmentEvent.StatusSnapshot` reuses the existing `ShipmentStatus` enum — no new enum

- **Decision**: `ShipmentEvent.StatusSnapshot` is typed as the already-existing `ShipmentStatus` enum
  (`Collected`/`InTransit`/`Delivered`/`Cancelled` — module `001`), persisted as `string`, same
  convention. For this module's only writer (`OrderConverted`), it is always set to `Collected`,
  matching the plan input's step 4 (`status_snapshot: Collected`) and the value the new `Shipment`
  itself is created with.
- **Rationale**: `ShipmentStatus` already exists and already represents exactly this concept (a
  snapshot of a shipment's status); introducing a second, parallel status enum for the same domain idea
  would be a direct violation of Principio II (no competing pattern for an already-solved concern).
- **Alternatives considered**: A new, event-specific status enum — rejected as unnecessary duplication
  of `ShipmentStatus`.

## Decision 8: Order-number / tracking-number generation — simple `COUNT`-based sequence, exactly as specified, race condition accepted and documented

- **Decision**: `OrderService.CreateAsync` generates `ORD-{yyyyMMdd}-{XXXX}` by counting `Order` rows
  whose `CreatedAt` falls within `[today 00:00:00, tomorrow 00:00:00)` (UTC) and adding 1, formatted
  with `.ToString("D4")` (never truncated — a 5th-digit day is allowed to grow past 4 characters rather
  than erroring or colliding, per spec.md's Edge Cases). `OrderService.ConvertToShipmentAsync` generates
  `TRK-{yyyyMMdd}-{XXXX}` identically, scoped to `Shipment.CreatedAt` instead.
- **Rationale**: This is exactly the algorithm the plan input specifies ("XXXX = count of orders created
  today + 1"). It is implemented via the existing generic `CountAsync(filter)` already available on
  `IBaseRepository<T>` — no new repository method needed. It carries a known, accepted race condition
  under truly concurrent creates on the same calendar day (two simultaneous requests could both count
  the same N and generate the same suffix, which the unique index on `OrderNumber`/`TrackingNumber`
  would then reject as a duplicate-key error surfaced to the caller as an unhandled 500). No queue,
  advisory lock, or dedicated sequence table is introduced to close this gap, since none was requested
  and this system has no stated concurrency/throughput target (`plan.md`'s Performance Goals) that would
  justify the added complexity — consistent with Principio IV (small, scoped changes) and this
  project's existing tolerance for the same category of simplification elsewhere.
- **Alternatives considered**: A dedicated per-day sequence table or `SELECT ... WITH (UPDLOCK,
  HOLDLOCK)` pattern to make the counter race-free — rejected as unrequested complexity with no
  concrete need demonstrated (Principio III/IV); documented here instead as an accepted, known
  limitation so it is not mistaken for an oversight.

## Decision 9: Structural vs. database-dependent validation split — `HomePickup`/`DropOff` conditional rules are fully structural

- **Decision**: `CreateOrderDtoValidator`/`UpdateOrderDtoValidator` (FluentValidation) enforce, purely
  from the submitted DTO with no repository access: required shared fields, positive dimensions/weight,
  non-negative `QuotedPrice`, valid enum values, and the full `HomePickup`/`DropOff` conditional rule —
  `PickupType == HomePickup` requires `PickupAddress` + `PickupScheduledAt` (future datetime) and
  forbids `OriginBranchId`; `PickupType == DropOff` requires `OriginBranchId` and forbids
  `PickupAddress`/`PickupScheduledAt`. `OrderService` then separately checks, after structural
  validation passes: the referenced `Customer` exists and is active, and — only when `DropOff` — the
  referenced `Branch` exists and is active.
- **Rationale**: Unlike `Customer`'s `UpdateCustomerDto` (module `005`, Decision 8), `Order`'s
  `PickupType` is a normal, always-present field on both `CreateOrderDto` and `UpdateOrderDto` (nothing
  makes it immutable — `spec.md`'s User Story 6 explicitly allows switching pickup type on update), so
  the validator always has direct access to the value needed to decide which companion fields are
  required, with no DB round-trip needed. Only the *existence and active status* of the referenced
  `Customer`/`Branch` genuinely requires a repository call, so only those two checks move to the
  Service, following the same structural-vs-DB-dependent split established since module `004`.
- **Alternatives considered**: Moving the `HomePickup`/`DropOff` conditional rule to the Service
  alongside the DB-dependent checks — rejected as unnecessary: it doesn't need `IUnitOfWork`, so keeping
  it in FluentValidation (where every other pure-shape rule already lives) is the smaller, more
  consistent change.

## Decision 10: Status-transition guards (`Confirm`, `Cancel`, `Update`-when-not-Pending, `Convert`) use `InvalidOperationException` → `400`, not `FluentValidation.ValidationException`

- **Decision**: `ConfirmOrderAsync`, `CancelOrderAsync`, `UpdateOrderAsync` (when the order is not
  `Pending`), and `ConvertToShipmentAsync` (when the order is not `Confirmed`) throw
  `InvalidOperationException` with a clear message when the order's current status doesn't allow the
  requested transition. `OrderController` catches this exception type and returns `400` with
  `{ "message": "..." }` — the same shape `ShipmentController.UpdateStatus` already uses for
  `ShipmentService.UpdateShipmentStatusAsync`'s own transition guard.
- **Rationale**: A status-transition guard ("this order isn't in the right state for this operation") is
  a different kind of failure than a data-shape problem ("this field is missing or malformed") — this
  project already has a precedent for exactly this distinction: `ShipmentService` uses
  `ShipmentTransitionValidator` + `InvalidOperationException` for status-transition guards, while
  `Branch`/`Employee`/`Vehicle`/`Customer` use `FluentValidation.ValidationException` for field-level
  create/update errors. `Order` needs both kinds (field validation on create/update, transition guards
  on confirm/cancel/update-when-locked/convert) and reuses each existing pattern for the failure
  category it actually matches, rather than forcing every error through one shared shape.
- **Alternatives considered**: Modeling status-transition failures as `FluentValidation.ValidationException`
  too (a single failure list entry like `{ property: "Status", message: "..." }`) — rejected: it would
  blur the same field-vs-transition distinction this codebase already keeps separate for `Shipment`,
  and status-transition errors have no natural "property" to attach to.

## Decision 11: `ConvertToShipmentAsync` atomicity — a single `CommitAsync()` call, no explicit EF transaction API

- **Decision**: `ConvertToShipmentAsync` calls `AddAsync` for the new `Shipment`, `AddAsync` for the new
  `ShipmentEvent` (wired to the Shipment via a navigation property, not a manually-assigned FK — see
  Decision 12), and `Update` for the modified `Order`, then a single `await
  _unitOfWork.CommitAsync()` at the end. No `BeginTransactionAsync`/`CommitTransactionAsync` call is
  added.
- **Rationale**: `DbContext.SaveChangesAsync()` (what `UnitOfWork.CommitAsync()` already calls) wraps
  every pending tracked change in one implicit database transaction by default — this is standard EF
  Core behavior already relied on elsewhere in this solution (e.g. `BranchService.CreateBranchAsync`
  writes a `Branch` and its full `BranchSchedule` collection in one `CommitAsync()` call). The plan
  input's requirement ("use `IUnitOfWork` to wrap the multi-entity transaction... single atomic
  transaction") is satisfied by this existing mechanism without adding a new one.
- **Alternatives considered**: An explicit `IDbContextTransaction` via `BeginTransactionAsync` — rejected
  as redundant: `SaveChangesAsync`'s implicit transaction already covers all three writes in this method
  because they share the same `AppDbContext` instance and are all flushed by the same `CommitAsync()`
  call; introducing explicit transaction management would be a new pattern for a need already met.

## Decision 12: `ShipmentEvent` gets a forward navigation to `Shipment` (for FK fixup), no reverse collection on `Shipment`

- **Decision**: `ShipmentEvent.Shipment` (forward navigation, required) exists so that
  `ConvertToShipmentAsync` can set `shipmentEvent.Shipment = shipment` before the first `Add`/`Commit`
  call and let EF Core resolve `ShipmentEvent.ShipmentId` automatically once the new `Shipment.Id` is
  generated by the same `SaveChangesAsync` call — the standard EF Core pattern for inserting a parent
  and a dependent together without knowing the parent's identity value up front. `Shipment.cs` gets **no**
  `ICollection<ShipmentEvent>` back-reference.
- **Rationale**: Matches the unidirectional-FK convention already established for `Employee.Branch`/
  `Vehicle.Branch` (forward navigation only, no reverse collection added to `Branch.cs`) — this module
  never needs to load "all events for a Shipment" through `Shipment` itself; it only ever writes one
  event per conversion.
- **Alternatives considered**: Manually querying the new `Shipment.Id` after an intermediate
  `CommitAsync()` call, then setting `ShipmentEvent.ShipmentId` directly and committing again — rejected
  as two round-trips (and two implicit transactions) where the navigation-based single-commit approach
  already achieves true atomicity in one.

## Decision 13: `CustomerId` is not part of `UpdateOrderDto` — ownership is fixed at creation

- **Decision**: `UpdateOrderDto` omits `CustomerId` entirely. `OriginBranchId`, `PickupType`, and every
  other field the plan input lists as part of `Order` remain editable.
- **Rationale**: Neither `spec.md`'s User Story 6 nor the plan input mentions reassigning an Order to a
  different Customer via update — only pickup/recipient/destination/dimension fields are described as
  editable. An Order changing owners mid-lifecycle has no stated use case and no default in this
  system's precedent (Employee/Vehicle reassign `Branch` via update because that's an explicit,
  stated capability — module 004's User Story 5/6 — but nothing analogous is stated here for `Customer`).
- **Alternatives considered**: Including `CustomerId` in `UpdateOrderDto` for parallelism with
  `OriginBranchId` — rejected as unrequested scope with no stated use case.

## Decision 14: Pagination — reused from `002`/`003`/`004`/`005`, filtered by `customerId` and `status`

- **Decision**: `GetOrdersAsync(int? customerId, OrderStatus? status, int page = 1, int pageSize = 5)`
  returns `PagedResult<OrderDto>`, same `MaxPageSize = 50` clamp, `OrderByDescending(x => x.CreatedAt)`,
  same 4 response headers already exposed via CORS in `Program.cs`.
- **Rationale**: Explicit user instruction ("Keep my pagination rules for global GET endpoints... check
  Shipment module"), and the query string the user specified (`?customerId=&status=`) maps directly onto
  this shape, following the exact `ShipmentService.GetShipmentsAsync`/`BranchService.GetBranchesAsync`
  pattern.
- **Alternatives considered**: N/A — directly specified by the user.

## Status

All `NEEDS CLARIFICATION` markers resolved (the one from `spec.md`'s Clarifications was resolved in
that session; Decision 1 above reconciles it against the plan input). No blockers for Phase 1.
