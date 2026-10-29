# 0001. Record architecture decisions

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: Amended by [0013](0013-single-source-of-truth.md) (the index and relationship summaries are generated, and the docs checker runs on every pull request, not only those that touch `docs/`)
- Depends on / Required by: - (this ADR sets up the process that all other ADRs follow)
- Complements: [0012](0012-verification-checks-and-drift-detection.md) (ADR enforcement is one use of the checks)
- Related: -
- Unknowns, requirements and needs: -

## Context

The project is being delivered step by step, with AI-assisted implementation that sometimes uses different models. Decisions need to
be visible and explained, so that the people (and models) implementing later slices don't go against earlier decisions without realising.

## Decision

Record significant architecture decisions as ADRs in `docs/adr/`, using the template and rules in the [ADR index](README.md),
including links between ADRs in both directions.

## Standards and guidance

- DfE [architecture principles](https://technical-guidance.education.gov.uk/principles/architecture/): "Document your choices and why they were made so everyone can learn from your journey."
- DfE [technology stack](https://technical-guidance.education.gov.uk/guides/default-technology-stack/): departures should be justified and traced back to a user need. ADRs are where we do this.
- SS 11 (choose the right tools and technology). TCoP 3 (be open and use open source).

## Enforcement

- **ADR checks** in the docs checker (`dotnet run tools/docs-check.cs`, see [0013](0013-single-source-of-truth.md)) runs in CI on every pull request that touches `docs/`. The rules are in the [ADR README](README.md#automated-checks-on-adrs-ci).
- The pull request template asks "Does this change or depend on a decision?", with a link to the index.

## Consequences

- Decisions can be reviewed in pull requests, and are kept with the code.
- Copilot instruction files point to the ADRs, so implementing agents follow them.
- There's a small writing overhead for each decision. A CI check keeps the links between ADRs consistent.
