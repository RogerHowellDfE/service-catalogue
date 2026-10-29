# 0008. Raw snapshots, observations with their sources, and versioned checks

Date: 2026-10-29
Status: Proposed (to be confirmed during M1)

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on: [0002](0002-modular-monolith.md) (how responsibilities are split between modules), [0004](0004-postgresql.md) (JSONB storage)
- Required by: -
- Complements: [0007](0007-github-access.md) (connectors produce the snapshots), [0012](0012-verification-checks-and-drift-detection.md) (the same checks engine runs on our own repository)
- Related: -
- Unknowns, requirements and needs: U7 (which checks can be automated), C2, C3, C6, C7, N9, N10

## Context

Data comes from several sources: collected from APIs, entered by people, and later from sidecar files, ADO and ServiceNow. We need history and trends, and the
ability to re-run analysis when checks improve. Data entered by people must be able to correct collected data, without losing either version.

## Decision

1. **Raw snapshots** (Sources): source responses are stored as JSONB with the source, entity key, `collected_at`, run ID and a content hash.
   They're never changed afterwards. Responses that haven't changed (same hash) aren't stored again.
2. **Observations** (Catalogue): each attribute value records where it came from (`collector:github`, `user:{id}`, `sidecar`), the time
   and, where relevant, a confidence level. Precedence rules decide which value is shown. All values are kept and can be shown.
3. **Checks** (Insights): pure, versioned rules over the catalogue's facts that produce **findings**. Each finding has a signal, severity, evidence,
   a **standard reference** (for example a DfE repository requirement, an SS or TCoP point) and supportive guidance. Changing a check's version allows the history to be re-assessed.
4. **Read models** for dashboards are rebuilt from the above and can be optimised separately.

## Standards and guidance

- DfE [data principles](https://technical-guidance.education.gov.uk/principles/data/): data is an asset, analysable, and its use is ethical (findings are supportive).
- DfE [storing source code: repository requirements](https://technical-guidance.education.gov.uk/standards/storing-source-code/#repository-requirements) are the first source of checks.
- SS 10 (define success): trends from history.

## Enforcement

- **Database**: the application's database role can't run `UPDATE` or `DELETE` on snapshot tables. An integration test tries to, and expects it to be refused.
- **Unit test**: every check declares an ID, a version and at least one standard reference.
- Review only: whether a change to a check's logic needs a version bump. The pull request template asks.
- Confirmed during M1, when this ADR is expected to become Accepted.

## Consequences

- Full ability to audit and reprocess. Trends are possible from day one.
- Storage grows over time. A retention policy (for example keep daily, then weekly, then monthly) is needed later.
- More moving parts than plain CRUD. M1 starts with the smallest useful version: snapshots plus repository entities, with no checks yet.
- Raw snapshots are untrusted input. They're only ever parsed and encoded, never rendered directly.

## Alternatives considered

- Overwriting the current state only: rejected, because we'd lose history, trends and the ability to reprocess.
- Full event sourcing: more than we need right now.
