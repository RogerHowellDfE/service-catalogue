# 0012. Verification checks and drift detection, applied to ourselves first

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: -
- Complements: [0001](0001-record-architecture-decisions.md) (every ADR's Enforcement section), [0008](0008-snapshots-provenance-and-checks.md) (the same checks engine runs on snapshots), [0013](0013-single-source-of-truth.md) (the docs checker is one kind of check), [0016](0016-one-obvious-way.md) (conventions come with checks)
- Related: [0002](0002-modular-monolith.md) (architecture tests are one kind of check)
- Unknowns, requirements and needs: NFR-V1 to V4, C6, U7

## Context

Decisions that aren't checked drift silently, especially with AI agents and lighter models doing the implementation. The Insights module (M2)
needs repository checks anyway. Writing checks for our own repository first gives us confidence that they work, and builds a flywheel: every check we write for ourselves becomes a check across the estate.

## Decision

- **One checks engine** (`ServiceCatalogue.Checks`). A check is a small, versioned, tested rule with an ID, rationale, standard references and supportive guidance.
  It returns one of a fixed set of outcomes: **Pass**, **Non-compliant**, **Review advised**, **Not applicable**, **Unable to determine**.
  "We couldn't see it" (Unable to determine) is reported differently from "it isn't there" (Non-compliant).
- Repository checks are written against an abstraction, and run in **two contexts**: a CLI against our working copy in CI, and the Insights module against collected snapshots.
  How each kind of check runs is described in [architecture: verification checks and drift detection](../architecture.md#verification-checks-and-drift-detection).
- Checks that need the running app (headers, Content Security Policy, frontend markup, no-JavaScript behaviour) are Playwright and ZAP tests.
  Checks on the code are **custom Roslyn analysers with code fixes** (`ServiceCatalogue.Analyzers`) and architecture tests.
  We use built-in and third-party analysers (`BannedApiAnalyzers`, the .NET analysers) first, and only write our own where they don't do the job.
- Every analyser's `helpLinkUri` points to the ADR or requirement it enforces. Every check names its standard references.
- Licence check: `LICENCE` or `LICENCE.md` exists, its SPDX identifier is detected, and the [SPDX licence list](https://spdx.org/licenses/) marks it `isOsiApproved`.
  The DfE expectation (MIT, Crown Copyright) is a separate, more specific check whose failing outcome is *Review advised*. US spelling (`LICENSE`) also passes the licence check but is *Review advised* under the DfE check.

## Standards and guidance

- [x] Service Standard point(s): 11 (choose the right tools), 12 (open source: licence), 14 (reliable)
- [x] Technology Code of Practice point(s): 3 (be open and use open source), 6 (make things secure)
- [ ] Secure by Design principle(s):
- [x] DfE technical guidance page(s): [storing source code](https://technical-guidance.education.gov.uk/standards/storing-source-code/) (repository requirements), [licensing](https://technical-guidance.education.gov.uk/standards/licencing-software-or-code/)
- [ ] Other:

Departures from a recommendation, and why: none.

## Enforcement

- The checks CLI runs in CI on our own repository. *Non-compliant* fails the build. *Review advised* is reported in the job summary.
- **Tests for each check**: one fixture each for passing, non-compliant and unable to determine. **Analyser tests** for each diagnostic and code fix.
- **Unit test**: every analyser has a `helpLinkUri` pointing to the ADR or requirement it enforces. (Check metadata is tested under [0008](0008-snapshots-provenance-and-checks.md).)
- The ADR checker fails if an ADR's Enforcement section is empty.

## Consequences

- Checks are reusable across the estate, and we use the practices we measure.
- Writing analysers takes time. Start with the cheapest high-value ones (NFR-V4), and prefer existing analysers.
- Failure mode: too many false positives erode trust. The *Review advised* outcome and supportive wording reduce this.

## Alternatives considered

- Rely on review and agent instructions alone: doesn't scale, and isn't reliable with lighter models.
- Separate tooling for ourselves and for the estate: twice the work, and the two would drift apart.
