# Threat model

Status: **draft**, covering M0 (walking skeleton) and M1 (GitHub census). Requirement: NFR-S8 in [requirements](../requirements.md#security-secure-by-design).

This is the single list of threats to the service and how each is handled. The requirements and decisions it links to are the canonical
home of each mitigation; this page says which one covers which threat. Threats with no mitigation yet are listed as open risks against an
[unknown](../unknowns.md), never left out.

## Scope

- **In scope**: the M0 and M1 parts of the [architecture](../architecture.md): the browser, Web, Worker, PostgreSQL, the GitHub API (token, M1),
  data collected from GitHub, CI and the supply chain, and the AI agents that help build the service.
- **Where it runs**: locally only (R5 in [unknowns](../unknowns.md#resolved), NFR-O5). Nothing is reachable from the internet until NFR-S10 is met (M4).
- **Out of scope until their milestone adds the boundary**: Entra ID in a deployed environment and the WAF (M4), the GitHub App, ADO and
  ServiceNow (M5), personal data (M6), GOV.UK Notify (M8), sidecar files (M3). Each is added here by the slice that introduces it.

## Data flow and trust boundaries

```
  [B7] AI agents (local and cloud)              [B6] CI and supply chain
     | read issues, PRs, repo data, web pages      | GitHub Actions, NuGet packages,
     | write branches, PRs, comments               | Dependabot, frontend assets
     v                                             v
  +------------------------ GitHub repository (public) ------------------------+
                                     |
                                     | build, test, evidence
                                     v
  +-------------------------- Aspire AppHost (local) --------------------------+
  |                                                                            |
  |  Browser --[B1]--> Web (Razor Pages) ----+                                 |
  |   (staff)           |  [B2] persona      |                                 |
  |                     |  selector,         |                                 |
  |                     |  first-run         |                                 |
  |                     |  bootstrap         |                                 |
  |                     v                    v                                 |
  |               +-------------[B3]---------------+                           |
  |               | PostgreSQL (schema per module) |                           |
  |               +--------------------------------+                           |
  |                                    ^                                       |
  |                                    |                                       |
  |                                 Worker --[B4]--> GitHub API (M1, token)    |
  |                                    ^                                       |
  +------------------------------------|---------------------------------------+
                                       |
                             [B5] untrusted source data: names, descriptions,
                                  topics, homepage URLs (M1); READMEs and
                                  sidecar files later
```

| Boundary | Between | What crosses it |
|---|---|---|
| B1 | Browser and Web | Requests, form posts, cookies; pages containing collected data |
| B2 | Anonymous and signed-in, user and administrator | The persona selector (`LocalDevelopment` only), the first-run bootstrap, seeding scenarios |
| B3 | Web and Worker, and PostgreSQL | Queries, migrations, the audit log, raw snapshots |
| B4 | Worker and the GitHub API | The token, API requests and responses |
| B5 | Data collected from sources and everything that processes or displays it | Repository names, descriptions, topics, URLs; later READMEs and sidecar files |
| B6 | Third-party code and this repository's build | Actions, NuGet packages, Dependabot updates, frontend assets |
| B7 | AI agents and this repository | Untrusted text that agents read; branches, commits, pull requests and comments that agents write |

## Assets

Classifications follow NFR-S3. Per-field classification in code is not built yet (T12).

| Asset | Classification | Notes |
|---|---|---|
| Collected repository data (public repositories, M1) | Public | Public at source; still shown only to signed-in users (NFR-S1) |
| Insights and findings about the estate | Internal | Security posture across many repositories is more sensitive than any single fact |
| Role assignments, tracked organisations, run history | Internal | |
| Audit log | Internal | Holds fake persona identifiers until Entra ID is used; staff identifiers then make it Personal (T21) |
| GitHub token | Restricted | [ADR-0007](../adr/0007-github-access.md); a recorded departure under [ADR-0014](../adr/0014-credential-less-by-default.md) |
| Sign-in and anti-forgery cookies | Restricted | |
| One-time setup token (outside `LocalDevelopment`, proposed) | Restricted | [ADR-0011](../adr/0011-first-run-bootstrap.md), U25 |
| Source code, workflows and the `main` branch | Public (integrity matters) | Write access is the asset: the maintainer's GitHub credentials and those agents use |
| Evidence: screenshots, recordings, logs, CI artifacts | Public | Attached to public pull requests, so fake data only (NFR-Q7) |

## Threats

Each threat has an ID (`T1`, `T2`...), a STRIDE category (**S**poofing, **T**ampering, **R**epudiation, **I**nformation disclosure,
**D**enial of service, **E**levation of privilege) and its mitigation. Status is one of:

- **In place**: exists in the code or repository settings today.
- **Planned (#n)**: an issue in the backlog delivers it, with tests.
- **Rule**: a requirement or convention with no automated check yet.
- **Open (U-n)**: no mitigation decided; the unknown tracks it.

IDs are never reused. A threat that no longer applies stays in the table, struck through, with the reason.

### B1 Browser to Web

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T1 | S, I | An anonymous user reaches pages, insights or findings | Sign-in required by a fallback policy; endpoint enumeration test against a reviewed allow-list (NFR-S1, [ADR-0006](../adr/0006-authentication-and-authorisation.md)) | Planned (#15) |
| T2 | E | Forced browsing to admin routes, or raising one's own role | Named policies on every page; analyser requires an explicit policy (NFR-S2, NFR-V4, ADR-0006); role-by-route tests | Planned (#15, #67, #61) |
| T50 | E | Insecure direct object reference (IDOR): changing an ID in a URL or form to reach an object the user has no rights to | In M0 and M1, roles are site-wide and every signed-in user may see every object, so there is no object-level rule to bypass. Per-object permissions (for example by association) need object-level checks | Open (U27) |
| T3 | T | Cross-site request forgery on any POST, including sign-in and sign-out | Anti-forgery token on every POST (NFR-S5); `GET /sign-out` refused | Planned (#15, #20) |
| T4 | S | Open redirect through `ReturnUrl` after sign-in | Only local return URLs are accepted (NFR-S9); hostile `ReturnUrl` tests | Planned (#15) |
| T5 | S | A stolen, forged or replayed sign-in cookie, including one from a `LocalDevelopment` host replayed to another environment | HttpOnly, Secure, `SameSite=Lax`, session-only cookies (NFR-S5); persona scheme not registered outside `LocalDevelopment` (NFR-S2) | Planned (#15, #65) |
| T6 | I, T | Traffic read or changed in transit | HTTPS redirection; HSTS outside `LocalDevelopment` (NFR-S5) | In place |
| T7 | T | Clickjacking, or a Content Security Policy that allows injected script | CSP with no inline script and a frame restriction; headers asserted by tests and ZAP (NFR-S5, NFR-S11) | Planned (#20, #27) |
| T8 | I | Error pages or health endpoints reveal internals | Developer exception page and health endpoints are environment-gated ([environment-dependent behaviour](../architecture.md#environment-dependent-behaviour)) | In place |
| T9 | I | Route enumeration reveals which admin features exist | Unauthorised requests get the same response whether or not the route exists (ADR-0006) | Planned (#15) |
| T10 | R | A user denies an admin action they took | Audit log of admin actions (NFR-S6) | Planned (#68) |
| T11 | D | The website is flooded with requests | Runs locally only (R5); WAF and DDoS protection before any internet exposure (NFR-S10) | Open (U3) |
| T12 | I | Restricted or Personal fields shown to users not allowed to see them, or leaked into exports, telemetry or logs | Field classification and output filtering (NFR-S3); no Restricted or Personal field is collected in M0 or M1 | Rule; needed by the first slice that adds such a field |

### B2 Persona selector, first-run bootstrap and seeding

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T13 | S, E | The persona selector is enabled outside `LocalDevelopment`, letting anyone pick an administrator | Startup guard and tests in every other environment; missing sign-in configuration stops the app (NFR-S2, [ADR-0006](../adr/0006-authentication-and-authorisation.md), [ADR-0010](../adr/0010-unconfigured-integrations-fall-back-safely.md)) | Planned (#65, #15) |
| T14 | E | Environment checks bypassed: `IsDevelopment()` used instead of `IsLocalDevelopment()`, or a look-alike environment name (trailing space, zero-width space) treated as `LocalDevelopment` | `IsDevelopment()` banned (NFR-D4); exact-match tests for look-alike names | Planned (#23, #65) |
| T15 | E | A deployed environment is started with its environment name set to `LocalDevelopment` | The environment name is the only gate. Nothing is deployed until NFR-S10's security review, which must cover how the environment is set | Rule (NFR-S10, M4) |
| T16 | E | The test authentication handler is registered by the app rather than only by the test host | Registered only by `WebApplicationFactory` (ADR-0006) | Planned (#65) |
| T17 | E | Bootstrap race: two people both become the first administrator | Promotion is atomic, enforced in the database (NFR-D5, [ADR-0011](../adr/0011-first-run-bootstrap.md)); concurrent-attempt test | Planned (#17) |
| T18 | S, E | Bootstrap hijack: a stranger claims administrator on a new deployed environment first | Proposed one-time setup token; bootstrap route returns 404 once an administrator exists (ADR-0011) | Open (U25) |
| T19 | E | A seeding scenario, with its fake administrator personas, runs in a non-dev-equivalent environment | Seeding refused outside dev-equivalent environments (ADR-0011) | Planned (#16); `Test` is open (U25) |
| T20 | R | A bootstrap promotion or re-seed can't be attributed | Written to the audit log (NFR-S6, ADR-0011) | Planned (#17, #68) |

### B3 Web and Worker to PostgreSQL

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T21 | I | The audit log holds staff identifiers once Entra ID is used, which is personal data | No personal data before a DPIA (NFR-P1, NFR-P2); fake personas only until then (NFR-Q7) | Open (U9) |
| T22 | T | SQL injection | EF Core with parameterised queries ([ADR-0004](../adr/0004-postgresql.md)); CodeQL (NFR-S7) | Planned (#66, #58) |
| T23 | I | Database credentials leak | Locally generated by Aspire and never stored; Entra authentication with managed identity when deployed (NFR-S4, [ADR-0014](../adr/0014-credential-less-by-default.md)) | Local: in place. Deployed: open (U26) |
| T24 | T, R | The audit log is edited or deleted | Append-only, enforced by a database trigger (NFR-S6) | Planned (#68) |
| T25 | E | One module reads or changes another module's schema, because all modules share one database login | Module boundaries enforced in code by architecture tests ([ADR-0002](../adr/0002-modular-monolith.md)), not by the database | Open (U30) |
| T26 | D | Database unavailable partway through a run, or a migration fails | Failed runs are recorded and retried safely, because storage is idempotent (NFR-O2); migrations through the module persistence pattern ([ADR-0004](../adr/0004-postgresql.md)); health checks | Planned (#93, #66, #40) |

### B4 Worker to GitHub (M1)

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T27 | I | The token leaks into logs, exceptions, traces, pages, raw snapshots, the run history, evidence or recordings | Never logged or shown (ADR-0007); analyser bans logging secret-marked properties (NFR-V4); canary tests with a fake token value; evidence uses fakes (NFR-Q7); secret scanning with push protection (NFR-S7) | Planned (#90, #91); push protection in place (#1) |
| T28 | E | The token has more access than needed (write scopes, private repositories) | Fine-grained, read-only, public repositories only ([ADR-0007](../adr/0007-github-access.md), NFR-S4); minimum permissions declared (NFR-D6) | Planned (#90, #91) |
| T29 | S | Responses from something that isn't GitHub (wrong base URL, interception) are trusted | HTTPS with certificate validation; base URL fixed in options (ADR-0007) | Planned (#91) |
| T30 | D | Rate limits exhausted, or GitHub down or slow, partway through a run | Rate-limit headers respected, backing off within bounds; a failed run is retried from the beginning, safely because storage is idempotent (NFR-O2); `source-failures` scenario. Resuming from a checkpoint is added only if measurements show retrying is too costly | Planned (#92, #93, #101); cost open (U8) |
| T31 | D | Repeated "collect now" requests burn through the rate limit | Admin only; at most one queued or running run per organisation, enforced in the database (NFR-O2) | Planned (#88) |

### B5 Untrusted source data

Where a threat overlaps with valid input, the rule in [adversarial and abuse cases](../ways-of-working.md#4-adversarial-and-abuse-cases) applies:
keep and display valid content safely, and neutralise only what is dangerous. Unusual but valid text (emoji, Welsh characters, smart quotes)
is a requirement, not a threat (NFR-Q8).

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T32 | T, I | Stored XSS through repository names, descriptions or topics (later READMEs) | Encode all output (NFR-S9); `Html.Raw` on variables banned (NFR-V4); CSP with no inline script (NFR-S5); ZAP against `hostile-data` (NFR-S11) | Planned (#53, #20, #94, #27) |
| T33 | S | Deceptive Unicode: bidirectional overrides ("Trojan Source"), look-alike characters or invisible characters used to impersonate a repository or organisation, or to hide content | Dangerous characters shown visibly, valid text kept exactly (NFR-S9, NFR-Q8); `hostile-data` scenario seeds them | Rule: seeded by #94, but no issue yet requires them to be shown visibly (raised on #94) |
| T34 | S | A homepage URL with a `javascript:` or other non-web scheme rendered as a link | Only `http` and `https` URLs become links (NFR-S9) | Planned (#94) |
| T35 | D | Oversized values or deeply nested responses exhaust memory or storage (later, YAML or JSON bombs in sidecar files) | Size and depth limits on everything collected (NFR-S9); `hostile-data` scenario | Rule: no issue yet sets collection limits (raised on #78); sidecar files in M3 |
| T36 | T | Log injection: line breaks or control characters in collected values forge log entries | Structured logging only (NFR-S9) | Rule |
| T37 | T | Malformed or changed API responses corrupt the catalogue | Validated and rejected safely (NFR-S9); raw snapshots kept so data can be reprocessed ([ADR-0008](../adr/0008-snapshots-provenance-and-checks.md)); `source-failures` scenario (malformed JSON and headers) | Planned (#93, #92) |
| T38 | T | CSV or formula injection when collected values are exported and opened in a spreadsheet | Exports neutralise values starting `=`, `+`, `-` or `@` (NFR-S9). There are no exports in M0 or M1 | Rule; needed by the first export slice |

### B6 CI and supply chain

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T39 | T | A compromised or re-tagged GitHub Action runs in CI | Actions pinned to commit SHAs; `permissions: contents: read`; `persist-credentials: false` (NFR-S7); workflow linting; Dependabot updates | Pins and permissions in place; planned (#60, #26) |
| T40 | E | Workflow injection: untrusted pull request titles or branch names in `run:` steps, or `pull_request_target` with secrets | CI uses `pull_request`, holds no configured secrets, and its `GITHUB_TOKEN` is read-only; zizmor and CodeQL for Actions (NFR-S7) | Least privilege in place; planned (#60, #58) |
| T41 | T | A malicious or vulnerable NuGet package (typosquat, dependency confusion, compromised update) | Central Package Management ([ADR-0003](../adr/0003-dotnet-and-aspire.md)); one package source with source mapping; lock files restored in locked mode; NuGetAudit; dependency review; Dependabot (NFR-S7) | Source mapping and lock files in place; planned (#56, #59, #26) |
| T42 | T | Frontend assets tampered with, or loaded from a third-party CDN | Built from NuGet packages and our own Sass, served by the app, no CDN ([ADR-0017](../adr/0017-govuk-frontend-tag-helpers-own-sass.md)); CSP limits sources (NFR-S5) | Planned (#20) |
| T43 | I | Secrets committed to the repository | Credential-less by default (NFR-S4); secret scanning with push protection (NFR-S7); repository checks for secrets in configuration | Push protection in place (#1); planned (#25) |
| T44 | T, R | Changes reach `main` without review | Ruleset requires pull requests and passing checks. With one maintainer no approval is required; Copilot code review is requested manually as a stand-in | Pull requests and checks in place (#1); approval open (U19) |
| T45 | I | Evidence, CI artifacts or recordings attached to public pull requests contain real data or tokens | Fake data only (NFR-Q7); tokens never shown in evidence (ADR-0007); checked in each pull request review | Rule |

### B7 AI agents acting on the repository

The repository is public: anyone can open issues and comment. Agents read that text, collected repository data and web pages, and can write
branches, commits, pull requests and comments, sometimes with the maintainer's credentials. See [AI-assisted delivery](../ways-of-working.md#ai-assisted-delivery).

| ID | STRIDE | Threat | Mitigation | Status |
|---|---|---|---|---|
| T46 | T, E | Prompt injection in issues, pull requests, comments or fetched pages makes an agent change code, workflows or settings | External data is untrusted ([AGENTS.md](../../AGENTS.md)); every change goes through a pull request and required checks, merged by the maintainer, not the agent (T44) | Open (U29) |
| T47 | I | Prompt injection makes an agent leak secrets it can reach, such as the maintainer's GitHub credentials or user-secrets | Credential-less by default, so there is little to leak (NFR-S4); no real token in CI (NFR-Q7) | Open (U29) |
| T48 | T | An agent weakens a guard: edits a failing test to pass, adds an unexplained suppression, or widens workflow `permissions:` | Read a test's sources before changing it ([tracing tests](../ways-of-working.md#tracing-tests-to-their-source)); suppressions need a reason and an expiry (NFR-S11); security-sensitive work uses a high-capability model; workflow linting (#60) | Rule; open (U29) |
| T49 | R | An agent's change can't be told apart from a person's | Copilot co-author trailer on every commit it helps write ([branching and commits](../ways-of-working.md#branching-and-commits)) | Rule |

## Test data scenarios that exercise threats

From [seeding scenarios](../ways-of-working.md#seeding-scenarios):

- `hostile-data`: T32, T33, T34, T35, and ZAP's signed-in scan (T7).
- `small-estate`: T1, T2 (role-by-route tests with its personas).
- `source-failures`: T30, T37.
- `empty`: T17, T18 (the bootstrap journey).

## Updating this model

- **Which slices must**: any story that adds a trust boundary or a data classification lists the update in its scope
  ([Definition of Ready](../ways-of-working.md#definition-of-ready-story)), and any change to our understanding is made before it's done
  ([Definition of Done](../ways-of-working.md#definition-of-done)).
- **How**:
  - A new boundary gets a row in the boundary table, a place in the diagram and its own section.
  - A new threat takes the next free `T` number. Link its mitigation (NFR, ADR or issue); if there isn't one, link an unknown.
  - When a mitigation lands, change its status to **In place** in the same pull request.
  - Never reuse or renumber IDs. Strike through a threat that no longer applies and say why.
- **When to review the whole model**: at the start of each milestone, and before any deployment (NFR-S10).

## Standards and guidance

From [standards](../standards.md):

- Secure by Design (SbD): threat modelling is one of its activities.
- NFR-S8: target OWASP ASVS Level 2, and keep this model up to date.
- OWASP ASVS V1 (architecture, design and threat modelling).
- Service Standard point 9 (create a secure service which protects users' privacy).
- Technology Code of Practice point 6 (make things secure).
