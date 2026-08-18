# Research: Shipment Tracking Events

The plan input is unusually detailed (exact entity fields, validator rule lists, endpoint routes),
but it was evidently drafted without checking this repository's actual structure and existing code in
several places — it reuses naming conventions from a different project template and invents a status
value that doesn't exist here. This document resolves each such conflict in favor of what already
exists in this codebase (same precedent as modules `004`–`006`: an already-established, concrete fact
in the code wins over a generic or stale detail in the planning input), plus records the genuine new
design decisions this feature requires.

## Decision 1: File paths corrected from `Domain/...` to this solution's actual `Core/...` layout

- **Decision**: `DeliveryAttempt.cs` goes in `ShipmentTracker.Core/Entities/`, `DeliveryFailureReason.cs`
  in `ShipmentTracker.Core/Enums/` — not the `Domain/Entities/`, `Domain/Enums/` paths the plan input
  literally specifies.
- **Rationale**: This solution has no `Domain` project or folder anywhere — its four projects are
  `ShipmentTracker.Core/Infrastructure/Services/Web`, as documented in `CLAUDE.md` and used by every
  prior module. `Domain/Entities/...` is a naming convention from a different project skeleton that
  doesn't apply here; the plan input's own instruction ("Follow the existing Project structure") points
  the same direction. Same category of correction as module `006`'s Decision 1 (folder/path convention
  corrected to match this repo, not a stale template).
- **Alternatives considered**: Creating an actual `Domain/` folder to match the literal path — rejected
  outright: it would introduce a second, competing project-structure convention alongside `Core`,
  directly contradicting the plan's own "follow existing structure" instruction.

## Decision 2: No `Returned` shipment status — only `Delivered`/`Cancelled` are terminal, matching the existing enum and the clarified spec

- **Decision**: The terminal-state check for `RegisterEventAsync` only excludes `ShipmentStatus.Delivered`
  and `ShipmentStatus.Cancelled`. No `Returned` status is added anywhere.
- **Rationale**: The plan input's validator description says "Shipment must not be in terminal state
  (Delivered, Returned, Cancelled)," but `ShipmentStatus` (module `001`) has exactly four values —
  `Collected, InTransit, Delivered, Cancelled` — no `Returned`. `spec.md` (already clarified this
  session) never mentions a "returned to sender" concept either; its Edge Cases and FR-011 only ever
  discuss delivered/cancelled as terminal. Adding a whole new "returned" lifecycle phase would be a
  significant, unrequested scope expansion with no basis in the spec — `Returned` in the plan input
  reads as a stray reference from a different domain's status model, not a deliberate ask.
- **Alternatives considered**: Adding `Returned` to `ShipmentStatus` "to be safe" — rejected: it's not
  in the spec, not exercised by any acceptance scenario, and would need its own transition rules and
  business meaning this feature never defines.

## Decision 3: Reuse the existing `ShipmentTransitionValidator`/`StatusTransitionContext` — no new "`ShipmentTransitionRules`" class

- **Decision**: `ShipmentEventService` injects the already-registered `IValidator<StatusTransitionContext>`
  (`ShipmentTransitionValidator`, `ShipmentTracker.Services/Validators/Shipments/ShipmentTransitionValidator.cs`)
  — the same one `ShipmentService.UpdateShipmentStatusAsync` already uses — rather than a new class. Its
  `BeAValidTransition` method gains exactly two new edges: `InTransit → OutForDelivery`, and
  `OutForDelivery → Delivered` / `OutForDelivery → Cancelled`. Its existing `if (context.CurrentStatus ==
  newStatus) return true;` line already covers the confirmed "already out for delivery" Edge Case
  (re-marking a shipment that's already out for delivery is a same-status no-op, not a new code path)
  — no change needed for that case. Its existing terminal-state check (`Delivered`/`Cancelled` → reject
  any transition) already covers "must not be in terminal state" — that plan-input rule is not a
  separate check, it's the same one, since a call to `BeAValidTransition` for a terminal `CurrentStatus`
  already returns `false` for any `NewStatus`.
- **Rationale**: The plan input names a `ShipmentTransitionRules` class that doesn't exist in this
  codebase; the actual, already-shipped transition-guard mechanism is `ShipmentTransitionValidator`.
  Reusing it (rather than a parallel new validator) is the direct application of Principio II — don't
  introduce a competing pattern for an already-solved concern (status-transition legality).
- **Alternatives considered**: A separate `ShipmentEventTransitionValidator` scoped to this feature —
  rejected: `Shipment.Status` is one concept with one legality ruleset regardless of which feature is
  requesting the change; splitting it would let the two validators drift out of sync over time.

## Decision 4: The "current status must be `OutForDelivery`" gate for delivery attempts is a plain equality check, not routed through the transition validator

- **Decision**: `RegisterDeliveryAttemptAsync` does **not** call `BeAValidTransition` to enforce its
  "shipment must currently be out for delivery" precondition — it asserts `shipment.Status ==
  ShipmentStatus.OutForDelivery` directly, because a delivery-attempt event never changes the shipment's
  status (confirmed in `spec.md`'s Clarifications: it stays "out for delivery" across multiple attempts).
- **Rationale**: Routing this through `BeAValidTransition(CurrentStatus, NewStatus: OutForDelivery)`
  would be wrong: for a shipment currently `InTransit` (never yet marked out for delivery), that call
  would incorrectly **pass** — `InTransit → OutForDelivery` is a legal transition *for an out-for-delivery
  event*, but a delivery-attempt event must never be accepted for a shipment that hasn't actually been
  marked out for delivery yet. The plan input itself lists this as a separate rule from "transition must
  be allowed," which matches this distinction exactly.
- **Alternatives considered**: Reusing the transition validator with `NewStatus == CurrentStatus` forced
  — rejected as needlessly indirect for what is fundamentally a straight equality check with no status
  change involved.

## Decision 5: New route prefix `/api/shipments/...` (plural) coexists with the existing `/api/shipment/...` (singular) — the existing route is not renamed

- **Decision**: The four new endpoints live under `/api/shipments/...` exactly as the plan input
  specifies, in a new `ShipmentEventController`. The existing `ShipmentController` and its
  `/api/shipment/...` routes (module `001`/`002`) are untouched.
- **Rationale**: The plan input is explicit and consistent across all four routes about the plural
  prefix. Renaming the existing singular route to match would be a breaking change to an already-shipped
  contract, entirely unrequested — the same reasoning module `006` applied when it left `Shipment`'s
  existing shape alone rather than "fixing" it. The resulting singular/plural asymmetry between the two
  controllers is accepted and documented here, not silently fixed.
- **Alternatives considered**: Renaming `/api/shipment` → `/api/shipments` for consistency — rejected as
  unrequested, breaking scope creep into an existing, working module.

## Decision 6: `[AllowAnonymous]` is omitted — this solution has no authentication/authorization middleware at all

- **Decision**: `GetTrackingAsync` is implemented without an `[AllowAnonymous]` attribute.
- **Rationale**: `[AllowAnonymous]` only has meaning where a global or controller-level `[Authorize]`
  policy exists for it to override — this solution has no authentication middleware, no `[Authorize]`
  attribute, and no auth-related package anywhere (`CLAUDE.md`/every prior spec's Assumptions: "no
  authorization tiers... any [caller] may perform these operations"). Every endpoint in this API is
  already effectively anonymous today. Adding the attribute here would be inert and would introduce the
  first reference to an ASP.NET Core auth concept this codebase has never used, with no supporting
  infrastructure — a new, unsupported pattern for no functional benefit today.
- **Alternatives considered**: Adding it anyway as forward-looking documentation — rejected: an inert
  attribute referencing infrastructure that doesn't exist is more likely to confuse a future reader
  ("where's the auth policy this is opting out of?") than help; if/when auth is added to this solution,
  this is the natural first endpoint to annotate, and that can happen then.

## Decision 7: Public tracking is a new, dedicated endpoint — not an extension of the existing `GET /api/shipment/{trackingNumber}`

- **Decision**: `GET /api/shipments/tracking/{trackingNumber}` is a new endpoint on the new
  `ShipmentEventController`, returning a shipment summary plus its public-safe event timeline. The
  existing `GET /api/shipment/{trackingNumber}` (module `001`/`002`, `ShipmentController`) is completely
  unchanged — same response shape as always, no event timeline added to it.
- **Rationale**: `spec.md`'s own Assumptions section guessed the opposite ("this feature extends the
  shipment's existing... tracking lookup") — but that was my own auto-generated default during
  `/speckit-specify`, made before this plan input existed, not something the user explicitly confirmed
  in the `/speckit-clarify` session (that session's one question was about attempt/event sequencing, not
  this). The plan input is now far more specific — a full, deliberate endpoint list with an exact new
  route — and doesn't contradict any FR (every FR says "publicly accessible tracking information," never
  naming a specific URL). Per the same precedent as module `006`'s Decision 4 (a more specific
  planning-time detail refining an earlier, higher-level spec default), the plan's explicit routing
  wins. This also keeps `ShipmentController` (module `001`/`002`) completely untouched, which is a
  smaller, more additive change than modifying it.
- **Alternatives considered**: Extending the existing endpoint per the spec's own Assumption — rejected
  in favor of the more specific, deliberate plan input; also would have required touching
  `ShipmentController`/`ShipmentService`/`ShipmentDto`, a larger footprint on an already-shipped module
  than adding one new endpoint elsewhere.

## Decision 8: Two response shapes for events — an operational one (with `EmployeeId`) and a public-safe one (without it)

- **Decision**: `ShipmentEventDto` (used by `POST .../events`, `POST .../events/delivery-attempt`, and
  `GET .../events`) includes `EmployeeId`. `TrackingEventDto` (used only inside `GetTrackingAsync`'s
  response) omits it entirely — not nulled out, not present in the shape at all.
- **Rationale**: `GetEventsByShipmentAsync` is not marked `[AllowAnonymous]` in the plan input (only the
  tracking endpoint is) — read together, this distinguishes an operational/staff view (full detail) from
  the public one (privacy-filtered), matching `spec.md` FR-013 exactly. Omitting the field from the DTO
  shape entirely (rather than mapping it and hoping it's never serialized) makes the privacy guarantee
  structural, not a runtime judgment call.
- **Alternatives considered**: One shared DTO with `EmployeeId` conditionally nulled for the public path
  — rejected: leaves a live footgun (a future change to the tracking endpoint could easily start
  populating the field again without anyone noticing) that a shape-level omission doesn't have.

## Decision 9: `RegisterEventDto.EventType` excludes both `DeliveryAttempted` and `OrderConverted` — not just `DeliveryAttempted`

- **Decision**: The generic register-event endpoint's structural validation rejects `EventType ==
  DeliveryAttempted` (per the plan input, "that has its own endpoint") **and** `EventType ==
  OrderConverted` (an extension of the same reasoning, not in the plan input's literal text).
- **Rationale**: `OrderConverted` is exclusively written today by `OrderService.ConvertToShipmentAsync`
  (module `006`) as part of an atomic multi-entity conversion — it is never meant to be freely POSTed
  through a generic endpoint disconnected from a real order conversion, for exactly the same reason the
  plan input excludes `DeliveryAttempted` ("that has its own endpoint and validator"). Currently this
  leaves `OutForDelivery` as the only value the generic endpoint accepts — an exclusion list (rather
  than an inclusion allowlist of just `OutForDelivery`) is deliberately future-proof: a later module
  adding a new, genuinely generic event type needs no change to this validator to start flowing through
  it, consistent with `ShipmentEventType`'s "modeled openly" extensibility principle already established
  in module `006`'s research.md Decision 6.
- **Alternatives considered**: Only excluding `DeliveryAttempted` per the literal plan text, leaving
  `OrderConverted` freely POSTable — rejected: it would let a caller fabricate a semantically-invalid
  `OrderConverted` event with no real order behind it, undermining that event type's meaning for no
  benefit.

## Decision 10: No DTO inheritance for `RegisterDeliveryAttemptDto` — a flat, independent DTO; shared rules live in one Service-layer helper

- **Decision**: `RegisterDeliveryAttemptDto` is its own flat class (not `: RegisterEventDto`), carrying
  `EmployeeId`, `LocationLabel`, `Notes`, `OccurredAt`, `FailureReason`, `NextAttemptAt` — no `EventType`
  property at all (the service hardcodes `EventType = ShipmentEventType.DeliveryAttempted` regardless of
  any caller input, matching the plan's "forced internally"). `RegisterDeliveryAttemptDtoValidator`
  duplicates the handful of shared *structural* rules (`OccurredAt` not in the future) directly rather
  than using FluentValidation's `Include()` composition. The shared *database-dependent* rules (Employee
  exists/active, Shipment exists/not-terminal) are implemented once as a shared private helper on
  `ShipmentEventService`, called by both `RegisterEventAsync` and `RegisterDeliveryAttemptAsync` — this
  is where "inherits all rules from `RegisterEventValidator`" is actually realized.
- **Rationale**: This solution has never used DTO class inheritance anywhere across 6 prior modules —
  every Create/Update DTO pair is two independent flat classes even when nearly identical (e.g.
  `UpdateEmployeeDto` duplicates `CreateEmployeeDto`'s fields rather than inheriting). Introducing the
  first DTO inheritance hierarchy here, just to satisfy FluentValidation's same-type `Include()`
  requirement, would be a new, one-off pattern for a problem this codebase already has a standard answer
  to: shared *DB-dependent* validation logic goes in a shared private Service helper (the exact
  `ValidateBusinessRulesAsync(..., currentId)` shape used by `EmployeeService`/`VehicleService`/
  `OrderService`). The few shared *structural* rules are trivial enough (one rule) to duplicate directly
  without meaningful risk of drift.
- **Alternatives considered**: `RegisterDeliveryAttemptDto : RegisterEventDto` + FluentValidation
  `Include()` — technically works, but introduces DTO inheritance as a new, unprecedented pattern in this
  codebase for a benefit (avoiding one duplicated structural rule) that doesn't outweigh the consistency
  cost.

## Decision 11: `AttemptNumber` computed via the generic repository, filtering through the `ShipmentEvent` navigation — no new repository method

- **Decision**: `AttemptNumber = await _unitOfWork.DeliveryAttemptRepository.CountAsync(x =>
  x.ShipmentEvent.ShipmentId == shipmentId) + 1`. `IDeliveryAttemptRepository` stays empty (`:
  IBaseRepository<DeliveryAttempt> { }`).
- **Rationale**: `DeliveryAttempt` has no direct `ShipmentId` column (only `ShipmentEventId`, per the
  plan's explicit field list) — the shipment it belongs to is only reachable by joining through
  `ShipmentEvent`. EF Core 8 translates a navigation-property predicate like this into the necessary SQL
  join automatically, so the existing generic `CountAsync(filter)` already supports it with no schema or
  repository change — same "leave the repository interface empty, express the query as a LINQ filter"
  convention used everywhere else, and the same per-parent `CountAsync`-based sequencing style module
  `006` established for `ORD-`/`TRK-` numbers (there scoped by date, here scoped by shipment).
- **Alternatives considered**: Adding `ShipmentId` directly to `DeliveryAttempt` for a simpler filter —
  rejected: it's redundant data not in the plan's explicit field list, and the join-based filter already
  works without it.

## Decision 12: No pagination on any of this feature's four endpoints

- **Decision**: `GetEventsByShipmentAsync` and the event timeline embedded in `GetTrackingAsync` both
  return their full, unpaginated list of events (and each event's optional attempt detail). Neither
  `PagedResult<T>` nor the `X-Total-*` headers are used anywhere in this feature.
- **Rationale**: The user's instruction to "keep pagination rules for global GET endpoints" doesn't
  actually apply here — this feature adds no cross-shipment listing (nothing like `GET /api/orders` or
  `GET /api/customers`, which list *all* records of a type). Both new `GET` endpoints are scoped to one
  shipment's own small, naturally bounded event history (a handful of lifecycle events over a shipment's
  life, not open-ended user-generated content) — the same shape as `BranchSchedule`'s precedent (module
  `003`: a small, bounded child collection returned in full inside its parent, never paginated).
- **Alternatives considered**: Paginating `GetEventsByShipmentAsync` "to be consistent" — rejected: it
  would apply a mechanism designed for potentially-large, cross-entity listings to a small, bounded,
  single-parent collection that doesn't need it, adding needless complexity (Principio III/IV).

## Status

All plan-input/codebase conflicts resolved above. No `NEEDS CLARIFICATION` markers remain. No blockers
for Phase 1.
