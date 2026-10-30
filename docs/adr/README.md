# Architecture decision records

We use lightweight ADRs ([Michael Nygard format](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)),
with added **Related decisions** and **Standards and guidance** sections (see [standards](../standards.md)).
Status values: **Proposed**, **Accepted**, **Superseded by ADR-NNNN**, **Deprecated**.

## Rules

- Once an ADR is accepted, it isn't edited, with two exceptions: typos and links, and **adding back-links** under *Related decisions* or updating the *Status* line.
- Exception while the walking skeleton is built (until M0 closes): a foundational ADR may be refined in place when building it reveals a better detail,
  so that one decision isn't spread across several documents. The pull request says what changed and why.
- If a decision changes, write a new ADR that **supersedes** the old one. In the same pull request, update the old ADR's status to `Superseded by NNNN` and add the back-link.
- **Links go both ways.** If ADR A says it depends on or complements ADR B, ADR B must list A too.
- Link to unknowns (`U-n`), requirements (`NFR-*`, `C-n`) and needs (`N-n`) where relevant, so the reasons for a decision can be traced.
- Every ADR says how the decision is **enforced** (see below), or says plainly that it can only be checked by review.

## Automated checks on ADRs (CI)

These rules are part of the docs checker (`dotnet run tools/docs-check.cs`, [ADR-0013](0013-single-source-of-truth.md)), which runs on every pull request. The pull request fails if any of them is broken. The checker is a .NET 10 single-file app, so it stays in C#, and it has its own tests, including examples that must fail.

| Rule | Check |
|---|---|
| Supersession is mirrored | If ADR B says `Supersedes: A`, ADR A's status must be `Superseded by [B]`, and the reverse. The same applies to Amends and Amended by. |
| Links go both ways | Every Depends on, Required by, Complements and Related link has the matching reverse link in the other ADR |
| Links resolve | Every ADR link points to a file that exists. Every `U-n`, `NFR-*`, `C-n` and `N-n` reference exists in the docs |
| Status values are valid | Status is one of the allowed values. A superseded ADR names an ADR that exists and is accepted |
| No silent rewrites | Compared with the base branch, an **Accepted** ADR may only change its Status line or its *Related decisions* section. Any other change fails, unless the pull request has the `adr-correction` label (for typos only) |
| Index is generated | The index table in this README is regenerated from the ADR files (number, title, status). A stale index fails; `--fix` regenerates it |
| Sections are complete | Every ADR has all the template sections. *Enforcement* is not empty: it lists the checks, or says "Review only" with a reason |
| Numbering | ADR numbers are unique and continuous, and match the file names |

## Enforcement: preventing regressions

Each ADR has an **Enforcement** section that answers the question: *"If someone (or an AI agent) unknowingly goes against this decision, what will fail?"*
Prefer automatic, fast feedback:

| Mechanism | Examples |
|---|---|
| Architecture tests | Modules only reference each other's Contracts. Web doesn't reference module internals. No `Microsoft.EntityFrameworkCore.InMemory` package |
| Build configuration and analysers | `global.json` pins the SDK. Central Package Management. Banned-API analysers (for example `Microsoft.CodeAnalysis.BannedApiAnalyzers` banning `Html.Raw`) |
| Startup guards | The app refuses to start if sign-in isn't configured outside `LocalDevelopment` ([0010](0010-unconfigured-integrations-fall-back-safely.md)) |
| Integration and browser tests | The fallback policy requires sign-in on every route (a test lists all endpoints). Pages work with JavaScript off. Content Security Policy header present |
| CI policy checks | ZAP rules, dependency review, workflow linting (zizmor), secret scanning, checks that infrastructure-as-code files are in the agreed format |
| Agent instructions | `.github/copilot-instructions.md` and `AGENTS.md` repeat the rule. This is guidance, not enforcement, so it's always backed by one of the mechanisms above |
| Review only | Where no reliable automatic check exists. Say so explicitly, and say what the reviewer should look for |

When an ADR is superseded, the enforcement mechanisms it introduced must be updated or removed in the same pull request. The new ADR's Enforcement section says which.

## Relationship types

| Relationship | Meaning | Reverse relationship |
|---|---|---|
| Supersedes | Replaces an earlier decision entirely | Superseded by |
| Amends | Changes or narrows part of an earlier decision, which otherwise still stands | Amended by |
| Depends on | Only makes sense if the other decision holds | Required by |
| Complements | Works with another decision; neither depends on the other | Complements |
| Related | Worth reading together | Related |

## Index

Relationships are recorded in each ADR's *Related decisions* section (the source of truth). The docs checker generates this table from the ADR files.

<!-- generated:begin source=docs/adr/*.md -->
| ADR | Title | Status |
|---|---|---|
| [0001](0001-record-architecture-decisions.md) | Record architecture decisions | Accepted |
| [0002](0002-modular-monolith.md) | Modular monolith with Web and Worker hosts | Accepted |
| [0003](0003-dotnet-and-aspire.md) | .NET 10 LTS and Aspire | Accepted |
| [0004](0004-postgresql.md) | PostgreSQL as the primary datastore | Accepted |
| [0005](0005-server-rendered-ui-govuk-dfe-frontend.md) | Server-rendered UI with GOV.UK and DfE frontend | Accepted (DfE distribution Proposed, U2) |
| [0006](0006-authentication-and-authorisation.md) | Entra ID authentication, role-based authorisation, sign-in required by default | Accepted |
| [0007](0007-github-access.md) | GitHub access through a token, behind a connector interface | Accepted |
| [0008](0008-snapshots-provenance-and-checks.md) | Raw snapshots, observations with their sources, and versioned checks | Proposed |
| [0009](0009-local-first-hosting.md) | Run locally until ready for deployment | Accepted |
| [0010](0010-unconfigured-integrations-fall-back-safely.md) | Unconfigured integrations fall back safely: faked, recorded or disabled | Accepted |
| [0011](0011-first-run-bootstrap.md) | First-run bootstrap: the first user becomes an administrator | Accepted (protections outside `LocalDevelopment` open, U25) |
| [0012](0012-verification-checks-and-drift-detection.md) | Verification checks and drift detection, applied to ourselves first | Accepted |
| [0013](0013-single-source-of-truth.md) | Single source of truth: one home for each fact; link or generate, never copy | Accepted |
| [0014](0014-credential-less-by-default.md) | Credential-less by default: workload identity and federation before secrets | Accepted |
| [0015](0015-date-based-versions-and-changelog.md) | Date-based versions and a human-readable changelog | Accepted |
| [0016](0016-one-obvious-way.md) | One obvious way: few patterns, explicit and discoverable | Accepted |
<!-- generated:end -->

## Template

```markdown
# NNNN. Title

Date: YYYY-MM-DD
Status: Proposed | Accepted | Superseded by [NNNN](NNNN-title.md) | Deprecated

## Related decisions
Every link here needs a matching back-link in the other ADR.
- Supersedes / Superseded by:
- Amends / Amended by:
- Depends on / Required by:
- Complements:
- Related:
- Unknowns, requirements and needs: (e.g. U2, NFR-S2, C4, N5)

## Context
What is the issue we're addressing, and what forces are at play?

## Decision
What we're doing.

## Standards and guidance
Which standards and guidance this decision follows or departs from, with links. See docs/standards.md.
- [ ] Service Standard point(s):
- [ ] Technology Code of Practice point(s):
- [ ] Secure by Design principle(s):
- [ ] DfE technical guidance page(s) (including the DfE technology stack):
- [ ] Other (WCAG, ASVS, NCSC...):
Departures from a recommendation, and why:

## Enforcement
How we detect or prevent the decision being undone by accident (architecture tests, analysers, startup guards,
integration and browser tests, CI policy checks), or "Review only" with the reason and what reviewers should look for.
If this ADR supersedes or amends another, say what happens to that ADR's enforcement mechanisms.
- 

## Consequences
What becomes easier or harder, including risks, failure modes and follow-up actions.

## Alternatives considered
Options rejected, and why.
```
