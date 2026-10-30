# 0013. Single source of truth: one home for each fact; link or generate, never copy

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: Amends [0001](0001-record-architecture-decisions.md) (the ADR index and relationship summaries are generated, not hand-written)
- Depends on / Required by: -
- Complements: [0012](0012-verification-checks-and-drift-detection.md) (the docs checker is one kind of check), [0016](0016-one-obvious-way.md) (one home for each fact; one way for each job)
- Related: [0010](0010-unconfigured-integrations-fall-back-safely.md) (README sections generated from integration declarations), [0015](0015-date-based-versions-and-changelog.md) (the changelog format is checked)
- Unknowns, requirements and needs: NFR-V1, NFR-V5, NFR-D6, NFR-D7

## Context

Planning documents, ADRs, templates, agent instructions and the code all describe the same system. Copies of the same fact drift apart:
someone legitimately updates one and doesn't know about the other. This has already happened during planning. A "security model" summary
in `architecture.md` kept an out-of-date rule about the sign-in substitute after the requirement changed, and the ADR index's "Related" column had to be edited by hand alongside two ADRs.
Lighter AI models doing the implementation are especially likely to update one copy and miss the others.

We want a **pit of success**: the easy thing to do is the right thing, and the wrong thing fails loudly (but it's still possible, with a reason).

## Decision

1. **Every kind of fact has one canonical home.** The map is in [docs/README.md: where things live](../README.md#where-things-live).
   Everywhere else **links to it by ID** (`NFR-D3`, `ADR-0010`, `U24`, `SS 9`) or by section anchor.
2. **Link, don't copy.** A link may carry a short summary of no more than one line, so the text reads well
   ("fake persona selector only in `LocalDevelopment` (NFR-D4)"). Repeating a paragraph, list or table is not allowed.
3. **Repetition that can't be avoided is generated**, never hand-written. Generated blocks sit between markers:
   `<!-- generated:begin source=... -->` ... `<!-- generated:end -->`. CI fails if a block is stale. `--fix` regenerates them. Examples:
   - the ADR index (number, title, status, relationships), generated from the ADR files
   - the README's required secrets and opt-in commands, generated from the integration declarations in code (NFR-D6, NFR-D7)
   - later: the catalogues of checks and analysers, generated from their metadata
4. **Code is the source of truth for facts about code** (versions, environments, integrations, secrets, analysers, checks). Once code exists, docs describing
   those things are generated from it or replaced by a link to it, or to the running app's Integrations status page.
5. **ADRs are point-in-time records.** They may describe the context as it was at the time. Living documents don't copy decisions out of ADRs; they link to them.
   A living document that contradicts an accepted ADR means a new ADR is needed.
6. **Templates and agent instructions** (issue forms, the pull request template, `.github/copilot-instructions.md`, `AGENTS.md`) repeat rules
   where people or agents need them in front of them. Every rule repeated there **cites its canonical ID**, and the checker verifies the citation exists and the template's structure matches its source.

## Standards and guidance

- [x] Service Standard point(s): 8 (iterate and improve frequently: documents stay accurate as we change), 12 (open source: docs others can trust)
- [x] Technology Code of Practice point(s): 3 (be open), 8 (share, reuse and collaborate)
- [ ] Secure by Design principle(s):
- [x] DfE technical guidance page(s): [development principles](https://technical-guidance.education.gov.uk/principles/coding-principles/) (keep things in one place so they're easy to change)
- [ ] Other:

Departures from a recommendation, and why: none.

## Enforcement

A **docs checker** (`dotnet run tools/docs-check.cs`, which includes the ADR checks from [0001](0001-record-architecture-decisions.md)) runs in CI on every pull request. It has its own tests, including examples that must fail.

| Rule | Check |
|---|---|
| IDs are defined once | Every `N-`, `C-`, `NFR-`, `U-`, `R-` and ADR ID is **defined** in exactly one place (its canonical home), and every reference to it resolves |
| Links resolve | Relative links and `#anchors` exist. External links are checked on a schedule (lychee), not on every pull request |
| No copied text | Passages in different files that are near-duplicates (word-shingle overlap above a threshold, ignoring boilerplate such as the ADR template headings) fail. You can deliberately allow one with `<!-- duplicate-ok: reason -->`, and these are listed in the job summary |
| Generated blocks are fresh | Regenerating every `generated` block produces no diff |
| Templates match their source | The issue forms contain every acceptance-criteria section from [ways of working](../ways-of-working.md#acceptance-criteria). The pull request template's evidence table has the same columns |
| Out-of-date terms | A small list in `docs/terms.yml` maps retired terms to the current ones (for example "substitute sign-in" -> "persona selector"). A retired term found anywhere fails, and the message says what to use instead |
| Plain characters | Typographic characters (smart quotes, the ellipsis character, en and em dashes) outside `<!-- unicode-ok -->` blocks and test data are reported as *Review advised*, not as failures ([ways of working](../ways-of-working.md#architecture-decisions-and-documentation)) |
| Changelog | `CHANGELOG.md` versions match `vYYYY.MM.DD-NNN`, are in descending order, and the counter goes up within each day. Every entry cites at least one `#ref`. A pull request without the `no-changelog` label must change `## Unreleased` |

The pull request template asks: *"Did you change a fact? Change it in its canonical home only ([where things live](../README.md#where-things-live))."*

## Consequences

- Reading means following links more often. Short one-line summaries next to links keep the text readable.
- The checker needs a little upkeep (the threshold, the term list). Allowing a duplicate is deliberate, visible and explained.
- Agents and lighter models get fast, specific feedback ("this ID doesn't exist", "this paragraph duplicates architecture.md#...") instead of drift going unnoticed.

## Alternatives considered

- **Rely on review and discipline**: has already failed during planning, and doesn't scale to AI-assisted delivery.
- **A documentation site with transclusion** (for example MkDocs snippets or DocFX includes): GitHub doesn't render transclusion, and docs should read well on GitHub. We can reconsider this if we publish a docs site.
- **One huge document**: avoids copies but is hard to navigate and to review.
