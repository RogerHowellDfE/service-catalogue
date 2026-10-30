# Ways of working

## Team and cadence

- One developer (product owner and engineer) plus GitHub Copilot. The aim is to share more widely later.
- **Continuous flow (Kanban)** on a GitHub Project board. No fixed sprints. Limit work in progress to 1-2 items.
- Milestones are outcomes (see [roadmap](roadmap.md)), not dates.

## Backlog structure (GitHub)

- **GitHub Project** (v2) for the whole product. Columns: *Backlog -> Ready -> In progress -> In review -> Done*. The order within a column is the priority.
  Whoever starts an issue moves it to *In progress* before starting, and to *In review* when its pull request opens. The board shows what is actually happening.
  Two custom fields, each kept because it changes what someone does:

  | Field | Values |
  |---|---|
  | Size | `XS` (an hour or two), `S` (up to a day), `M` (two or three days), `L` (too big: split it before it's Ready) |
  | Model tier | `High-capability` or `Lighter`. See [AI-assisted delivery](#ai-assisted-delivery) |

  The milestone gives the horizon, and labels give the type and module, so they aren't repeated as fields.
- **Epics** are parent issues, with **sub-issues** for slices.
- **Labels** for type (`type:epic`, `type:story`, `type:spike`, `type:chore`, `type:bug`, `type:adr`), module (`module:sources`, `module:catalogue`, `module:insights`,
  `module:identity`, `module:audit`, and `module:platform` for cross-cutting work such as hosts, SharedKernel, CI, tooling, checks, analysers and the docs checker;
  labels for later modules are added when work on them starts), concerns that need a specialist eye in review (`security`, `accessibility`, `performance`, `privacy`)
  and process labels that CI reads (`no-changelog`, `adr-correction`). Labels stand in for issue types, because issue types are only available to organisations.
  Don't add a label unless something (a person, a query or a check) uses it.
- **Milestones** match the roadmap outcomes (M0, M1, ...).
- Issue forms (`.github/ISSUE_TEMPLATE`) prompt for every [acceptance criteria](#acceptance-criteria) section. The docs checker keeps them in step with this page ([ADR-0013](adr/0013-single-source-of-truth.md)).
- The pull request template repeats the standards checklist and the evidence table, so the reviewer can check each one.

## Acceptance criteria

Acceptance criteria must cover far more than the main path. A story is **not Ready** until each of the sections below
has been considered. If a section doesn't apply, write "N/A" and give a short reason. Leaving a section blank is not acceptable.

### 1. Main path (happy path)

The intended behaviour, written as Given/When/Then.

### 2. Unhappy paths

The ways a legitimate user can go wrong, and what they see:

- validation errors: required, format, length, duplicates, boundaries (0, 1, max, max+1). Use the GOV.UK error summary and error message patterns.
- empty states (no data yet), single item, a very large volume (pagination, how slow it gets)
- the thing has been removed, renamed or archived, or never existed (404, with a helpful page)
- stale data: show when it was last refreshed
- the user doesn't have permission (403 with a helpful explanation, not a broken page)
- two people editing at once, or a form submitted twice (idempotency, optimistic concurrency)
- no JavaScript, keyboard only, zoomed to 400%, screen reader

### 3. Unusual but valid input

Input that is surprising but **legitimate**. It must be **accepted, stored without loss, displayed correctly, and searchable and sortable**.
It must not be rejected, mangled or "sanitised away". (This is different from adversarial input, which must be made harmless.)

- **Text and Unicode**
  - Emoji, including multi-part sequences (👩🏽‍💻, 🏳️‍🌈, flags), in service names, descriptions and topics
  - Welsh and other diacritics (ŵ, ŷ, â, ë; Siân, Llŷr), non-Latin scripts, right-to-left text, mixed scripts
  - Apostrophes, hyphens and quotes in names (O'Brien, Smith-Jones)
  - **Typographic characters**, often pasted from Word, Outlook or Teams:
    - smart quotes and apostrophes: `‘ ’ “ ”` (O’Brien)
    - the ellipsis character `…` as well as three full stops `...`
    - en and em dashes `– —`, non-breaking and other unusual spaces, soft hyphens

    These must be stored and displayed exactly as entered. Search and duplicate detection must treat them as equivalent
    to their plain versions: searching for `O'Brien` finds `O’Brien`, and `Data…` matches `Data...`.
  - Very long but valid names, single-character names, names that are only numbers
  - Comparison and search: Unicode normalisation (NFC/NFD), case folding and collation for sorting
  - Truncation must never split a character or emoji sequence (grapheme cluster)
- **URLs and identifiers**
  - Emoji or international domain names (punycode) in URLs, percent-encoded paths, trailing slashes, query strings and fragments
  - Repo names containing `.`, `_` or `-`, or starting with a `.` (for example `.github`)
  - **Case-insensitive names**: GitHub organisation and repo names are case-insensitive (`DFE-Digital` = `dfe-digital`), so we must not create duplicates
- **Source-data edge cases**
  - Renamed or transferred repos (redirects), forks, template repos, mirrors, archived repos, empty repos (no default branch, no languages)
  - Very large monorepos, repos with dozens of languages, `null` or missing optional fields from the API
- **Dates and times**: time zones and daylight saving changes (store UTC, display UK time), very old dates, future dates, repos never pushed to

### 4. Adversarial and abuse cases

How someone acting in bad faith (or a compromised data source) could try to break it, and what must hold true:

- **Authorisation bypass**: changing URLs or IDs (insecure direct object references), forced browsing to admin routes, raising one's own role, a user who isn't signed in getting at insights
- **Injection**: SQL, HTML or script injection (XSS), injection into headers or logs, CSV/formula injection in exports
- **Untrusted source data**: hostile repo names, descriptions, topics, READMEs or sidecar files (scripts, oversized payloads, YAML or JSON bombs,
  Unicode used to deceive: look-alike characters, bidirectional override characters such as "Trojan Source", invisible characters)
- **Cross-site request forgery** (CSRF), open redirects, clickjacking, Content Security Policy bypasses
- **Abuse of resources**: repeatedly triggering collection, burning through the API rate limit, very large requests
- **Data leakage**: Restricted or Personal fields appearing in HTML, logs, error pages, telemetry or exports
- **Secrets**: tokens ending up in logs, exceptions, screenshots or recordings

> Where section 3 and section 4 overlap (for example Unicode), the rule is to **keep and display valid content safely** (encode it
> when it's output), and only neutralise what is actually dangerous (for example by showing bidirectional override characters visibly).
> Don't reject valid input just to block attacks.

### 5. Failure modes

What happens when things the service depends on go wrong, and how the service degrades, recovers, and is observed:

| Consideration | Questions to answer |
|---|---|
| Dependency down or slow | GitHub, endoflife.date or Entra ID unavailable, or timing out. What does the user see? Does the service partly work? |
| Rate limiting | Primary or secondary GitHub rate limits are reached partway through a run. Does it back off, pause and resume? |
| Partial failure | 300 of 1,000 repos collected and then a crash. Can it be resumed? Is the run marked partial rather than successful? |
| Bad data | Malformed or unexpected API responses, or a change to the API's format. Is it rejected safely and logged? |
| Database | Connection lost, a failed migration, a unique key clash. Are transactions safe? |
| Concurrency | Two collection runs for the same organisation at once, or a run overlapping a schedule |
| Retries | Is the operation idempotent, so retrying is safe? |
| Detection | What log, metric, trace or health check shows the failure? Would we notice? |
| Recovery | Is recovery automatic, or does an admin need to act? What does the admin see? |

### 6. Non-functional criteria (where relevant)

Accessibility (WCAG 2.2 AA), performance targets (for example p95 < X ms at N repos), security headers, audit entries, telemetry.

### 7. Test data scenarios

Which [seeded scenarios](#test-data) the story is tested and demonstrated against. Name any new scenario it needs.

### 8. Standards and guidance

Which [standards and guidance](standards.md) the story contributes to or must respect (SS point, TCoP point, SbD principle,
DfE technical guidance page, WCAG criterion, ASVS requirement). The issue and pull request templates prompt for this with a checklist of the
most common ones, so it's quick to fill in. Departures are noted and linked to an ADR.

### 9. Evidence expected

For each criterion in sections 1-8, the evidence that will prove it (see [Evidence](#evidence)).

> Every category gets **tests**, not just the main path: negative tests, tests with unusual but valid input,
> fault injection (for example a fake GitHub source returning 403/429/500, timeouts, malformed JSON), and security tests.

## Definition of Ready (story)

- Linked to a need (N-ref) and/or capability (C-ref), and to a milestone.
- [Acceptance criteria](#acceptance-criteria) sections 1-9 filled in, or explicitly "N/A" with a reason.
- Small enough to finish in a few days at most. Dependencies and unknowns are listed.
- If it adds a trust boundary or data classification, the threat model update is listed as part of the story.

## Definition of Ready (other issue types)

| Type | Ready when it has |
|---|---|
| Epic | The outcome, the milestone, the needs (N-refs) and capabilities (C-refs) it serves, and its slices listed as sub-issues (they can be refined later) |
| Spike | The question, the unknown it answers (U-ref), a time box, and the expected output (an ADR, an updated unknown, or a recommendation with evidence) |
| ADR | The decision needed, the options known so far, the unknowns and requirements it relates to, and when it's needed by |
| Chore | Why it's needed, what "done" looks like, and the evidence that will show it |
| Bug | Steps to reproduce (with a seeded scenario), expected and actual behaviour, the requirement or acceptance criterion it breaks, and the failing test to write first |

## Definition of Done

- Every acceptance criterion, in **every** section, is met and has evidence.
- Tests written test-first where practical. We have seen them **fail, then pass** for the reasons we expected.
- All CI checks pass, including the [automated security and quality scanning](#automated-security-and-quality-scanning), the analysers, the checks CLI on our own repository and the fresh-clone first-run job.
- A new or changed convention or decision is **enforced** (analyser, architecture test, check or test), or explicitly marked "Review only" with a reason ([ADR-0012](adr/0012-verification-checks-and-drift-detection.md)).
- A new integration has fake/recording and disabled modes, appears on the Integrations status page, and declares any secrets it requires ([ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)).
- Works without JavaScript, if it's a UI change. Checked with the keyboard.
- Only fake or generated data is used in tests, seeds, recordings and screenshots.
- Documents, ADRs and the threat model updated if our understanding changed.
- Reviewed. The Copilot code-review agent runs as a minimum, with the adversarial cases explicitly reviewed.
- Anything left over (follow-ups, observations, deferred review comments) is recorded as an issue on the board, even if only a stub,
  and linked from the pull request, before the pull request merges or the issue closes.

## Test data

**Real data does not belong in tests**, with very few exceptions (see below). CI, local development, demos and
performance tests all use fake data, generated or written by hand.

### Principles

- **Generated and repeatable**: test data builders, plus a generator (for example Bogus) with a **fixed seed**,
  so every run, recording and before/after comparison uses the same data.
- **Obviously fake identities**: alliterative, clearly fake names, so that nobody mistakes them for real people or data:
  *Fake Francis*, *Test Teri*, *Dummy Dana*, *Sample Sam*, *Mock Morgan*, *Pretend Priya*. The set deliberately includes
  unusual but valid names, for example *Fake Siân*, *Test Zoë*, *Dummy D'Arcy-Ó Briain*, *Mock 🦄 Morgan*.
- **Reserved domains and identifiers only**:
  - Email addresses use the [reserved domains from RFC 2606 and RFC 6761](https://www.rfc-editor.org/rfc/rfc2606),
    for example `fake.francis@example.com` or `test.teri@catalogue.test`. Never a real domain such as `education.gov.uk`.
  - GitHub logins, organisations and repos are clearly fake, for example `fake-francis`, `fake-dfe-org`, `test-repo-legacy-hotspot`.
  - GUIDs and object IDs are generated. Never copied from a real tenant.
- **Fake sources, not just fake rows**: fakes run behind the same interfaces as the real integrations, including failure behaviour such as rate limits.
  Local development and CI run without any real credentials ([ADR-0007](adr/0007-github-access.md), [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)).

### Seeding scenarios

Named, versioned scenarios. The same scenario is used by local development (selected when Aspire starts), integration and browser tests, ZAP scans,
performance tests and demo recordings. Scenarios grow as slices need them. The initial set:

| Scenario | Purpose |
|---|---|
| `empty` | First run: no administrator, organisations or repos. Checks empty states and the [first-run bootstrap](adr/0011-first-run-bootstrap.md) |
| `small-estate` | One fake organisation, about 20 repos with a mix of languages, activity and archived status. The default for local development and demos |
| `large-estate` | Several thousand repos and a long snapshot history. For performance tests and pagination |
| `legacy-hotspots` | Repos with end-of-life runtimes, very old dependencies, no CI/CD. Feeds the risk signals |
| `unusual-but-valid` | Emoji and Welsh characters in names, descriptions and topics, smart quotes, ellipses (`…`) and dashes from pasted text, very long and single-character names, `.github`-style repos, organisation names that differ only by case, renamed, transferred, empty, fork and template repos, missing optional fields |
| `hostile-data` | Script injection, oversized payloads, bidirectional override and invisible characters, malformed fields. For adversarial tests and ZAP |
| `source-failures` | The fake GitHub source returns 403, 429 and 500 errors, timeouts and malformed JSON partway through a run. For failure-mode tests |
| `people` (later) | Obviously fake people (Fake Francis...) for the contacts features, once the DPIA allows them |

Data scenarios are combined with **configuration profiles**, which say which integrations are configured
(see [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)):
`unconfigured` (nothing set, which is the fresh-clone default), `partial` (for example a GitHub token but no Notify key) and `fully-configured` (all fakes replaced by test doubles of the real clients).
Stories that touch an integration say which profiles they're tested against.

### Exceptions

- **Contract tests** may use *recorded* responses from public GitHub API endpoints, to catch changes to the API's format.
  Before committing, these recordings are scrubbed (no tokens, no personal data such as commit author emails, replaced with fake values),
  kept small, and documented with where they came from and when they were recorded. They're refreshed on purpose, never automatically in CI.
- A manual exploratory check against the real API is allowed locally, but its output is never committed and never used as evidence that shows personal data.

## Evidence

Every issue and pull request **must** have before and after evidence that shows, without any doubt, what is true after the change that was not true before.

- The evidence is **matched to each acceptance criterion**, in every category, in a table in the pull request:

  | Acceptance criterion | Before | After | Evidence type |
  |---|---|---|---|
  | AC1: Admin can add an organisation | Route returns 404 | Organisation added and listed | Recording (Playwright, GIF), `small-estate` |
  | AC3: Repo named `🚀-launch-ŵŷ` shows and sorts correctly | Shown as `??-launch-??` | Shown correctly, found by searching "launch" | Screenshot + test (`unusual-but-valid`) |
  | AC4: Non-admin POST to admin route is refused | n/a (route new) | 403, no change in the database, audit entry written | Integration test output |
  | AC6: GitHub returns 429 partway through a run | Run crashes, marked failed | Run pauses, resumes after the reset, marked complete | Fault-injection test (`source-failures`) + log excerpt |
  | AC7: No new ZAP alerts at medium severity or above | 2 medium (missing CSP) | 0 medium/high | ZAP report diff (`hostile-data`) |
  | AC8: List page p95 < 300 ms with 2k repos | p95 = 1.8 s | p95 = 210 ms | k6 summary (`large-estate`) |

- **Choose the type of evidence that fits the change.** For anything that is a *journey* or *interaction*, a recording beats a series of screenshots.
  - **UI journeys and interactions: recordings.** Playwright records video of browser tests and saves a trace. Short clips are
    converted to **GIFs** (ffmpeg) for embedding in the pull request. Full videos and traces are kept as CI artefacts. Record "before" and "after" with the
    same script and the same seeded scenario, so the two are directly comparable.
  - UI states (a single static view): screenshots, taken automatically by Playwright.
  - Behaviour or functionality: the test failing before and passing after, with test names.
  - Command line or tooling: terminal output, or a terminal recording (for example asciinema or VHS) for interactive flows.
  - Performance: measurements from the same test setup and the same scenario, before and after.
  - Security: a failing then passing negative or security test, and/or a scanner (ZAP, CodeQL...) report diff.
  - Failure modes: output from fault-injection tests, and the telemetry or log entries that show the failure is visible.
  - Docs-only changes: a link to the rendered document.
- **Collect "before" evidence before making the change**, by running against `main`. With test-first, the failing test is the "before" evidence.
- Recordings and screenshots use seeded fake data only, and never show secrets.
- If there's no meaningful "before" (for example a brand-new route), say so explicitly and show that the route or feature didn't exist before.

## Automated security and quality scanning

Defence in depth: each tool catches a different kind of problem. All of them run in CI, and as many as possible run locally too.

| Concern | Tool (initial choice) | When |
|---|---|---|
| Dynamic security testing (DAST) | **OWASP ZAP**: a baseline scan on every pull request against the app running with seeded data (`small-estate` + `hostile-data` + `unusual-but-valid`) and signed in, and a full or API scan on a schedule | PR + scheduled |
| Static analysis (SAST) | CodeQL (C#, JavaScript, Actions), preferably through the [DfE SAST reusable workflow](https://github.com/DFE-Digital/github-actions/tree/master/sast-reusable-workflow), plus .NET analysers with warnings treated as errors where practical | PR |
| Dependency vulnerabilities | Dependabot alerts and updates, `dotnet list package --vulnerable`, dependency review action | PR + scheduled |
| Secrets | GitHub secret scanning with push protection, plus a gitleaks pre-commit hook (optional) | Push / PR |
| Accessibility | axe-core in Playwright tests. Pa11y CI or Lighthouse for a crawl of the whole site (to evaluate) | PR |
| Security headers / Content Security Policy | ZAP rules, plus integration tests that assert headers are present | PR |
| GitHub Actions workflow security | Actions pinned to commit SHAs, zizmor (or similar) to lint workflows, least-privilege `permissions:` | PR |
| Containers (when used) | Trivy image scanning | PR |
| Infrastructure as code (when used) | Checkov / PSRule for Azure | PR |
| Licences and SBOM | Generate an SBOM for our own code (CycloneDX/SPDX) and check dependency licences | Release |
| Code quality | `dotnet format` check, analysers, mutation testing (Stryker.NET), stretch goal | PR / scheduled |

New findings at medium severity or above fail the build. Exceptions are recorded with a reason and an expiry date
(for example in a `.zap/rules.tsv` or a suppression file with comments). We never suppress a finding without explaining why.

## Branching and commits

- Trunk-based: a short-lived branch per slice, a pull request into a protected `main`, merged with a **merge commit** (squash and rebase merging are switched off).
  Prefer small, self-contained pull requests; one issue can be delivered by several. Some bootstrapping work has to touch many files at once, but otherwise avoid large pull requests.
  The branch's commits stay in the history, including the red-then-green commits that show [test-driven development](#test-driven-development).
  The merge commit takes the pull request's title and description. Branches are deleted after merging.
- CI (`build-and-test` and `format`) must pass before a pull request merges; both are required checks on `main`.
  Bring a pull request up to date with the latest `main` before merging it, so its green checks reflect what will land. The ruleset doesn't enforce this; any exception is deliberate.
  CI also runs on every push to `main`, which catches two separately green changes that break each other.
- [Conventional Commits](https://www.conventionalcommits.org/) for commit messages and pull request titles, for example `feat(sources): add tracked organisation admin`.
- Repository settings and the `main` ruleset aren't stored in the code. Changes to them are recorded in an issue with before and after values (#1).
- Each commit includes the Copilot co-author trailer when Copilot helped write it.

## Test-driven development

- Test-first is the default. Writing tests after is acceptable **only** if the test is then shown to fail
  against the code before the change, or when the change is temporarily reverted.
- Test layers: unit -> integration (Testcontainers PostgreSQL, fake or fault-injecting GitHub source) -> browser (Playwright, with video and trace) -> architecture -> DAST (ZAP).
- Never call live external APIs in CI tests.

### Tracing tests to their source

- Every test class has a trait for each current source of the behaviour it tests: `[Trait("Issue", "11")]`, `[Trait("Decision", "ADR-0010")]`,
  `[Trait("Requirement", "NFR-D4")]`. Put a trait on a test method when it applies to that test only.
- **Before changing a failing test, read its sources.** If the change contradicts them, the source changes first (or alongside), not just the test.
- Traits list current sources only. When a decision is superseded or an issue's behaviour is replaced, swap the trait; git history keeps the old one.
- Test names describe behaviour. Add a reference to the name (`..._issue14_ac3`) only where it really helps, for example a browser test that walks one acceptance criterion.
- **Changed behaviour leaves a trail.** When a change contradicts an earlier issue's acceptance criteria, comment on the old issue linking to the new one,
  and link back from the new issue, so either end leads to the other.

## AI-assisted delivery

- **Planning, architecture, ADRs, refining acceptance criteria (especially unusual input, adversarial cases and failure modes) and reviews**: use a high-capability model.
- **Implementing well-refined slices**: use a lighter model, guided by `.github/copilot-instructions.md` and `AGENTS.md`.
  These files hold the conventions (module rules, TDD, acceptance-criteria sections, test data, evidence, frontend, security) so that quality doesn't depend on the model.
- Every AI-written change goes through the same Definition of Done. If a lighter model struggles with a slice, it needs refining further or should go back to a higher-capability model.
- The Project's **Model tier** field records which kind of model an issue needs. `High-capability`: epics, spikes, ADRs, refining acceptance criteria,
  security-sensitive design, and setting up the solution and its conventions for the first time. `Lighter`: Ready stories, chores and bugs that follow an established pattern.

## Architecture decisions and documentation

- Decisions: see the [ADR rules](adr/README.md).
- Documentation: one home for each fact. See [where things live](README.md#where-things-live).
- **State the current position, not the story of how we got there.** Docs describe what is true and what we've decided.
  Leave out the history of drafts, earlier attempts and removed scaffolding unless it helps a reader now
  (for example, an ADR's *Alternatives considered*, or a superseded ADR that explains why something changed). Git history holds the rest.
- **Plain characters by default**: in docs, code, config and commit messages, use straight quotes `' "`, three full stops `...` and the hyphen `-`
  rather than smart quotes, the ellipsis character or en and em dashes. These are easier for people to type, search, diff and edit.
  This is a preference, not a rule. Keep other characters where they add something: test data and examples of
  [unusual but valid input](#3-unusual-but-valid-input), people's names, status symbols in tables, and diagrams.
  Files are UTF-8 (`.editorconfig`). The docs checker reports typographic characters as *Review advised* (it doesn't fail the build),
  and you can allow a block with `<!-- unicode-ok: reason -->`.

## Changelog and versions

- A human-readable [`CHANGELOG.md`](../CHANGELOG.md) at the root of the repository. Each entry describes **one coherent change, as a user would describe it**,
  and lists every issue and pull request that contributed to it, for example `- Admins can add and remove tracked GitHub organisations (#12, #15, #21)`.
  Several pull requests that deliver one change share one entry.
- Changes go under `## Unreleased` in the same pull request. Chores with nothing to tell users (for example a dependency bump with no effect) use the `no-changelog` label.
- Versions are date-based: `vYYYY.MM.DD-NNN`, where `NNN` starts at `001` each day (UTC) and goes up by one for each release that day.
  For example `v2026.10.28-001`, `v2026.10.28-002`, then `v2026.10.29-001`. See [ADR-0015](adr/0015-date-based-versions-and-changelog.md).
