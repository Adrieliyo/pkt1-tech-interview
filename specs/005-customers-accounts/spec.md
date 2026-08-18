# Feature Specification: Customers & Accounts Module

**Feature Branch**: `[005-customers-accounts]`

**Created**: 2026-08-17

**Status**: Draft

**Input**: User description: "Build the Customers & Accounts module for a parcel delivery company.

  A Customer is any person or organization that places shipment orders.
  There are two types of customers: Individual (physical person) and
  Business (legal entity / company). Both types share common contact
  information but have distinct identifying fields.

  All customers share: a unique email, a phone number, a full address
  (street, city, state, zip code, country), an active/inactive status,
  and a customer type discriminator.

  An Individual Customer additionally has: first name, last name,
  an optional birth date, and a government-issued ID number (CURP or INE).

  A Business Customer additionally has: legal business name, a tax ID (RFC),
  a legal representative name, an optional industry category,
  and an optional credit limit for corporate accounts.

  Customers can be created, updated, listed, and deactivated — never deleted.
  Listing supports an optional filter by active status and by customer type.
  Retrieving a single customer always returns the full detail
  including the type-specific fields.

  Email must be globally unique across both customer types."

## Clarifications

### Session 2026-08-17

- Q: Can a customer's type (Individual/Business) be changed after creation via update, or is it fixed permanently once set at creation? → A: Fixed at creation — update only edits fields within the existing type; changing type is rejected.
- Q: Besides email, should the type-specific government identifiers (CURP/INE for Individual, RFC for Business) also be required unique, or is only email required to be globally unique? → A: Yes, also unique — CURP/INE and RFC are real-world unique identifiers and must be enforced as such.
- Q: Is the Individual customer's government ID a single field that holds either a CURP or an INE value, or two separate fields? → A: Single field — one "government ID number" field per Individual customer; CURP/INE are just examples of the value format.
- Q: Should the government ID (CURP/INE) and tax ID (RFC) be validated against Mexico's official format patterns, or just required to be non-empty text? → A: Validate against the official format — an Individual's government ID must match the official CURP structure (18 alphanumeric characters), and a Business's tax ID must match the official RFC structure for legal entities (12 alphanumeric characters); a non-empty value that doesn't match the required structure is rejected.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Register a new customer (Priority: P1)

An operations or sales staff member registers a new customer — either an individual person or a business — recording their contact information, address, and the identifying fields specific to their type, so shipment orders can be placed on their behalf.

**Why this priority**: No other capability in this module has anything to operate on until customers exist. This is the foundation for every other capability.

**Independent Test**: Can be fully tested by submitting a new Individual customer with a unique email and government ID, and a new Business customer with a unique email and tax ID, and confirming both are created and marked active.

**Acceptance Scenarios**:

1. **Given** no existing customer with the submitted email, **When** a user creates an Individual customer with first name, last name, a unique government ID, phone, and a full address, **Then** the customer is created, marked active by default, and typed as Individual.
2. **Given** no existing customer with the submitted email, **When** a user creates a Business customer with legal business name, a unique tax ID, legal representative name, phone, and a full address, **Then** the customer is created, marked active by default, and typed as Business.
3. **Given** an email already used by another customer (of either type, active or inactive), **When** a user attempts to create a new customer with that same email, **Then** the creation is rejected and the specific validation problem is reported.
4. **Given** a government ID or tax ID already used by another customer of the same type, **When** a user attempts to create a new customer with that same identifier, **Then** the creation is rejected and the specific validation problem is reported.
5. **Given** a user is creating a customer, **When** they omit a required field for the declared type (e.g., last name for an Individual, tax ID for a Business), **Then** the creation is rejected and the specific validation problem is reported.

---

### User Story 2 - Find and review customers (Priority: P2)

An operations or sales staff member browses the customer roster to find customers by type or status, and drills into a specific customer to review their full profile, including type-specific details.

**Why this priority**: Once customers exist, staff need to locate and inspect them before any updates or deactivations are meaningful.

**Independent Test**: Can be fully tested by listing customers with and without filters, and retrieving a single Individual and a single Business customer to confirm their respective type-specific fields are always included.

**Acceptance Scenarios**:

1. **Given** customers of both types and both statuses exist, **When** a user lists customers with no filters applied, **Then** only active customers are returned.
2. **Given** customers of both types exist, **When** a user lists customers filtered by type Business, **Then** only active Business customers are returned.
3. **Given** both active and inactive customers exist, **When** a user lists customers explicitly filtered to inactive status, **Then** only inactive customers are returned.
4. **Given** a user lists customers filtered by both status and type simultaneously, **When** the filters are applied, **Then** only customers matching both criteria are returned.
5. **Given** an existing Individual customer, **When** a user retrieves that customer by their identifier, **Then** the response includes all shared fields plus the Individual-specific fields (first name, last name, birth date, government ID).
6. **Given** an existing Business customer, **When** a user retrieves that customer by their identifier, **Then** the response includes all shared fields plus the Business-specific fields (legal business name, tax ID, legal representative, industry category, credit limit).
7. **Given** a customer identifier that does not exist, **When** a user retrieves it, **Then** the system reports that the customer was not found.

---

### User Story 3 - Update customer information (Priority: P3)

An operations or sales staff member corrects or refreshes a customer's details — such as their phone, address, or type-specific fields — as real-world conditions change.

**Why this priority**: Customer data drifts over time (address changes, corrected tax IDs, updated credit limits). This capability keeps the customer roster accurate but is only meaningful after customers exist and can be found.

**Independent Test**: Can be fully tested by updating an existing customer's address and a type-specific field, then retrieving the customer to confirm the changes were persisted and re-validated.

**Acceptance Scenarios**:

1. **Given** an existing customer, **When** a user updates their phone, address, or type-specific fields (e.g., an Individual's birth date, a Business's credit limit) to valid, non-conflicting values, **Then** the customer reflects the new values.
2. **Given** an existing customer, **When** a user attempts to change their customer type (Individual to Business, or vice versa) via update, **Then** the update is rejected and the customer's prior data remains unchanged.
3. **Given** an existing customer, **When** a user attempts to update their email to a value already used by another customer, **Then** the update is rejected and the customer's prior data remains unchanged.
4. **Given** an existing Individual or Business customer, **When** a user attempts to update their government ID or tax ID to a value already used by another customer of the same type, **Then** the update is rejected and the customer's prior data remains unchanged.
5. **Given** an inactive customer, **When** a user updates their status back to active, **Then** the customer becomes active again and continues to appear in default listings.

---

### User Story 4 - Deactivate a customer (Priority: P4)

An operations or sales staff member deactivates a customer who is no longer active with the company, removing them from default operational views while preserving their record.

**Why this priority**: Lower-frequency action than the above, and depends on customers already existing. Preserving history (never deleting) protects downstream records that may reference the customer.

**Independent Test**: Can be fully tested by deactivating an active customer and confirming they no longer appear in default listings but remain fully retrievable by their identifier with all data intact.

**Acceptance Scenarios**:

1. **Given** an active customer, **When** a user deactivates them, **Then** their status becomes inactive and they no longer appear in default (unfiltered) listings.
2. **Given** an inactive customer, **When** a user retrieves them directly by identifier, **Then** all of their details, including type-specific fields, are still returned unchanged.
3. **Given** an already-inactive customer, **When** a user deactivates them again, **Then** they remain inactive without error (the action is idempotent).
4. **Given** a customer (active or inactive), **When** any user attempts to permanently delete them, **Then** no such capability exists — deactivation is the only way to retire a customer.

---

### Edge Cases

- What happens when a create or update request for an Individual customer includes Business-only fields (e.g., a tax ID)? Rejected as inconsistent with the declared customer type.
- What happens when a create or update request for a Business customer includes Individual-only fields (e.g., a government ID)? Rejected as inconsistent with the declared customer type.
- What happens when an email, government ID, or tax ID collides with an existing one only after trimming whitespace or differing by case? Treated as a duplicate and rejected — uniqueness checks are case-insensitive and ignore leading/trailing whitespace.
- What happens when a Business customer's credit limit is submitted as a negative number? Rejected — credit limit must not be negative.
- What happens when a government ID or tax ID is non-empty but does not match the official CURP/RFC structure for its type? Rejected as an invalid format — a distinct validation problem from a duplicate-value rejection.
- What happens when listing customers with an invalid/unrecognized customer type filter value? The request is rejected with a validation error rather than silently returning no results.
- Does deactivating a customer affect any of their past shipment orders? No relationship between Customer and Shipment is modeled by this module — that connection, if needed, is out of scope here.

## Requirements *(mandatory)*

### Functional Requirements

#### Shared

- **FR-001**: System MUST allow users to create a Customer as either Individual or Business, capturing the shared fields (email, phone, full address consisting of street/city/state/zip code/country, and customer type) plus the fields specific to the declared type.
- **FR-002**: System MUST reject Customer creation or update if the email is already in use by a different customer, company-wide across both customer types, regardless of whether that other customer is active or inactive.
- **FR-003**: System MUST mark every newly created Customer as active by default.
- **FR-004**: System MUST NOT allow a Customer's type to be changed via update once set at creation; an update request attempting to change type is rejected, leaving prior data unchanged.
- **FR-005**: System MUST allow users to update an existing Customer's shared fields and its type-specific fields, re-validating all applicable creation rules (required fields, uniqueness) on every update, and MUST reject the update — leaving prior data unchanged — if validation fails.
- **FR-006**: System MUST allow users to deactivate an active Customer, setting their status to inactive.
- **FR-007**: System MUST treat deactivating an already-inactive Customer as a no-op that does not raise an error.
- **FR-008**: System MUST NOT provide any capability to permanently delete a Customer.
- **FR-009**: System MUST allow users to list Customers, optionally filtered by active status, by customer type, or by both simultaneously.
- **FR-010**: System MUST return only active Customers when a list request is made with no active-status filter specified.
- **FR-011**: System MUST allow users to retrieve a single Customer by their identifier, and the response MUST always include the full detail, including the fields specific to that customer's type.
- **FR-012**: System MUST report a clear not-found result when a user retrieves or updates a Customer identifier that does not exist.
- **FR-013**: System MUST reject a create or update request that includes fields belonging to the customer type other than the one declared (e.g., a tax ID submitted for an Individual customer).

#### Individual Customers

- **FR-014**: System MUST require an Individual customer to have a first name, a last name, and a government-issued ID number matching the official CURP structure (18 alphanumeric characters in the government-defined layout); a birth date is optional.
- **FR-015**: System MUST reject Individual Customer creation or update if the government ID is already in use by a different Individual customer, regardless of whether that other customer is active or inactive.

#### Business Customers

- **FR-016**: System MUST require a Business customer to have a legal business name, a tax ID matching the official RFC structure for legal entities (12 alphanumeric characters in the government-defined layout), and a legal representative name; an industry category and a credit limit are optional.
- **FR-017**: System MUST reject Business Customer creation or update if the tax ID is already in use by a different Business customer, regardless of whether that other customer is active or inactive.
- **FR-018**: System MUST validate that a Business customer's credit limit, when provided, is not negative.

### Key Entities

- **Customer**: A person or organization that places shipment orders. Shared attributes: email (unique, company-wide, across both types), phone, full address (street, city, state, zip code, country), active/inactive status, and a customer type discriminator (Individual or Business, fixed once set). Every Customer record carries exactly one type's additional attributes, described below.
- **Individual Customer** *(Customer where type = Individual)*: Additional attributes: first name, last name, optional birth date, government-issued ID number (unique among Individual customers).
- **Business Customer** *(Customer where type = Business)*: Additional attributes: legal business name, tax ID (unique among Business customers), legal representative name, optional industry category, optional credit limit.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A staff member can register a new customer of either type, fully capturing their contact information and type-specific identifiers, in a single submission without needing follow-up corrections for well-formed input.
- **SC-002**: 100% of customers in the system have a unique email and a unique type-specific government/tax identifier — no duplicate customer account can ever be created under a shared identity.
- **SC-003**: A staff member can find every active customer of a given type in a single lookup, without manually inspecting unrelated or inactive records.
- **SC-004**: A staff member can retrieve any customer's complete profile — including all type-specific details — in a single lookup, regardless of whether the customer is an Individual or a Business.
- **SC-005**: Deactivated customers are never returned in default listings, while their historical records — including type-specific identifying fields — remain fully intact and are never lost through deletion.

## Assumptions

- **State and Country as free text**: Consistent with the Branches & Hubs module's precedent for "State," both `state` and `country` are free-text fields, not restricted to a fixed list of codes.
- **Industry category as free text**: No fixed list of industries was provided in the request (unlike the two-value Customer type discriminator), so industry category is treated as an optional free-text field rather than a fixed enumeration.
- **Permanent uniqueness, including inactive records**: Consistent with the precedent confirmed in the Employees & Vehicles module, uniqueness for email, government ID, and tax ID applies against every customer record, active or inactive — an identifier used once is never available for reuse.
- **No authorization tiers**: Consistent with the rest of this system, no role-based restriction is assumed on who may perform these operations; any authenticated user of the system may create, update, list, and deactivate Customers.
- **Reactivation via update**: There is no separate "activate" action; setting an inactive customer's status back to active is done through the standard update capability (see FR-005), consistent with the established precedent from the Branches and Employees & Vehicles modules.
- **No relationship to Shipment**: This module does not model any relationship between Customer and Shipment orders; connecting a shipment to the customer who placed it, if needed, is out of scope for this specification.
- **Phone number format**: No specific format or country-code validation is specified; phone is treated as a required free-text field.
- **CURP as the validated government-ID structure**: The Individual government-ID field's official-format check validates against the CURP structure (18 alphanumeric characters) specifically. INE/voter-credential numbers are cited in the request only as an example value, not as a second structure to validate against — a value must satisfy the CURP structure to be accepted. The exact regular expression / character-position rules are a planning-level detail, not specified here.
