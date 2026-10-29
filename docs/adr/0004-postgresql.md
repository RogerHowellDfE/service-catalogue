# 0004. PostgreSQL as the primary datastore

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on: [0002](0002-modular-monolith.md) (one schema per module)
- Required by: [0008](0008-snapshots-provenance-and-checks.md) (JSONB snapshots)
- Complements: [0014](0014-credential-less-by-default.md) (database access by Entra identity)
- Related: -
- Unknowns, requirements and needs: U4 (hosting cost), NFR-Q8 (Unicode handling)

## Context

We need to store:
- structured catalogue entities
- raw API snapshots that change shape over time
- observations recording where each piece of data came from
- relationships (a graph-like structure)
- data over time for trend analysis

It must run locally in a container and on Azure at low cost.

## Decision

Use **PostgreSQL** through EF Core (Npgsql), with one schema per module.

- JSONB for raw snapshots and flexible attributes, with GIN indexes where queries need them.
- Recursive CTEs for walking relationships at first. A graph database is only considered if this proves insufficient.
- Locally: a PostgreSQL container through Aspire. In Azure (later): Azure Database for PostgreSQL Flexible Server.
- Integration tests use Testcontainers PostgreSQL. No in-memory database provider, because it hides real behaviour.
- UTF-8 everywhere. For search and duplicate detection, use normalised, case-folded columns and/or ICU collations (from NFR-Q8).

## Standards and guidance

- DfE [data principles](https://technical-guidance.education.gov.uk/principles/data/): data is an asset, analysable, secure.
- TCoP 3 (open source), TCoP 4 (open standards).
- SS 9 (secure, protects privacy): one schema per module allows least-privilege database roles later.

## Enforcement

- **Architecture test**: bans the `Microsoft.EntityFrameworkCore.InMemory` and SQLite provider packages anywhere in the solution.
- **Integration tests** run against real PostgreSQL through Testcontainers, so provider-specific behaviour (JSONB, collation, Unicode) is exercised.

## Consequences

- Flexible, analysis-friendly, open source and portable.
- Flexible Server has no scale-to-zero option. Hosting cost is tracked in U4.

## Alternatives considered

- **Azure SQL**: more familiar within DfE, and has a serverless auto-pause tier. Rejected for now because PostgreSQL's JSONB and analytics suit this problem better.
- **Document or graph databases**: not needed at this stage.
