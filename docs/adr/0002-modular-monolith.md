# 0002. Modular monolith with Web and Worker hosts

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on: -
- Required by: [0004](0004-postgresql.md) (one schema per module), [0008](0008-snapshots-provenance-and-checks.md) (how the data pipeline is split between modules)
- Complements: -
- Related: [0012](0012-verification-checks-and-drift-detection.md) (architecture tests are one kind of check)
- Unknowns, requirements and needs: NFR-O1, NFR-Q2

## Context

The product will grow across several areas (sources, catalogue, insights, identity, people, data flows,
provisioning). It has a single developer, low traffic, and needs low running costs and fast iteration. Microservices would add
operational and cognitive overhead with no benefit at this scale. An unstructured monolith risks becoming tangled.

## Decision

- Build a **modular monolith**: one solution and one database server, with modules as bounded contexts (see [architecture](../architecture.md#modules-bounded-contexts)).
- Each module has an internal implementation and a public **Contracts** project. Other modules may only reference Contracts.
- Each module owns its own database **schema**. No queries across schemas from another module's code.
- Modules communicate through Contracts interfaces and in-process integration events. An outbox is added when reliability needs it.
- Two hosts run the same modules: **Web** (UI) and **Worker** (collection and analysis jobs).
- **Architecture tests** (for example NetArchTest or ArchUnitNET) enforce these rules in CI.

## Standards and guidance

- DfE [architecture principles](https://technical-guidance.education.gov.uk/principles/architecture/): "Start small", "Embrace change and plan for it" ("clear interfaces between components ... each ... changed at the appropriate pace").
- DfE [coding principles](https://technical-guidance.education.gov.uk/principles/coding-principles/): optimise for change, think smaller, any attempt to predict the future is likely to be wrong.
- SS 14 (operate a reliable service): collection is kept separate from serving pages.

## Enforcement

- **Architecture tests** (`ServiceCatalogue.ArchitectureTests`): modules only reference each other's Contracts, Web and Worker don't reference module internals, and SharedKernel has no module references.
- **Integration test**: each module's `DbContext` uses its own default schema, and its migrations only touch that schema.
- Review only: queries against another module's schema written in raw SQL. Reviewers look for schema names in SQL strings.

## Consequences

- Simple to run, debug and deploy. Module boundaries keep the option of extracting a module later.
- Needs discipline. The architecture tests stop boundaries quietly wearing away, including when AI is writing the code.
- Combined screens need read models built from events, or composition in the UI, rather than SQL joins across modules.
- Failure mode: if the Worker fails, the Web keeps serving the last collected data, with its "last refreshed" time shown.

## Alternatives considered

- **Microservices**: rejected because of operational cost and complexity at this scale.
- **Single project with folders**: rejected because the boundaries couldn't be enforced.
