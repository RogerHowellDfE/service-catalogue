# Requirements

These are broad, up-front requirements. Each delivery slice in the backlog meets part of them.

## Functional capability map

| ID | Capability | Needs | Horizon |
|---|---|---|---|
| C1 | **Source configuration**: manage tracked GitHub organisations (and later ADO organisations) in the admin UI | N5, N22 | Now |
| C2 | **Collection**: collect repo metadata from source APIs on demand and on a schedule, with run history and status | N1, N3, N6 | Now |
| C3 | **Snapshot history**: keep raw source responses and the facts derived from them, with timestamps | N9 | Now |
| C4 | **Census browse and explore**: list, search, filter, facet and view details of repositories | N1 to N4 | Now |
| C5 | **Technology profile**: languages, then frameworks, runtimes, dependencies (from SBOMs and manifests) | N2, N7 | Now / Next |
| C6 | **Checks and signals**: pluggable rules that produce findings, each mapped to standards, with supportive guidance | N7, N8, N10 | Next |
| C7 | **Trends**: how aggregates and findings change over time | N9 | Next |
| C8 | **Services and products**: group repositories and add context entered by people, recording where each piece of data came from | N12 | Next |
| C9 | **Tags and endorsements**: community tagging, for example exemplars, with moderation | N13 | Next |
| C10 | **Relationships**: typed links between repositories and services, inferred and declared | N11 | Next |
| C11 | **Sidecar metadata**: a published schema, plus reading the sidecar file from each repository | N14 | Next (spec early) |
| C12 | **People and contacts**: link people to services, with access controls, after a DPIA | N15 | Later |
| C13 | **Identity linking**: link Entra and GitHub accounts using GitHub OAuth | N16 | Later |
| C14 | **Data assets and interfaces**: data each service creates and consumes, linked to Find and Use an API | N17, N18 | Later |
| C15 | **Wider sources**: private repos (GitHub App), more organisations, ADO, ServiceNow cross-reference | N22, N23 | Later |
| C16 | **Support initiatives**: group findings and targets into tracked initiatives | N21 | Later |
| C17 | **Provisioning and access automation**: onboarding, offboarding, resource creation | N19, N20 | Later |
| C18 | **Read API**: expose catalogue data for other tools, and register it on Find and Use an API | - | Later |
| C19 | **About the service**: a "What's new" page generated from `CHANGELOG.md`, and footer links to the source code, contributing guidance, the changelog, the accessibility statement, cookies and privacy | N24, N25 | Now |

## Non-functional requirements

### Standards we follow

See [standards and guidance](standards.md) for the full list and the abbreviations used here (SS, TCoP, SbD, DfE-TG, WCAG, ASVS...).

### Security (secure by design)

- **NFR-S1** Every page requires sign-in by default. Anonymous access is opt-in per page, after review. All findings and insights require sign-in, even when they're about public repositories.
- **NFR-S2** Sign-in uses Entra ID with OpenID Connect. Authorisation uses roles (Entra app roles such as `Catalogue.Reader`, `Catalogue.Contributor`, `Catalogue.Admin`) and policies. For local development, when sign-in isn't configured, a fake persona selector is used. It is only allowed when `IsLocalDevelopment()` is true. In any other environment, missing sign-in configuration stops the app from starting (fail closed). Automated tests and a startup guard prove this.
- **NFR-S3** Each field is classified (Public, Internal, Restricted or Personal), and sensitive fields are only shown to users who are allowed to see them.
- **NFR-S4** **Credential-less by default.** Every connection uses the first option that works in the order set out in [ADR-0014](adr/0014-credential-less-by-default.md):
  no credential; managed identity, workload identity federation or OIDC; a non-exportable key; short-lived tokens; and only then a long-lived secret (Key Vault, declared under NFR-D6 with the reason).
  No secrets are ever stored in the repository. GitHub access has least privilege.
- **NFR-S5** Secure HTTP headers: HSTS, a Content Security Policy that allows no inline script, X-Content-Type-Options and Referrer-Policy. Anti-forgery tokens on every POST. Only strictly necessary cookies.
- **NFR-S6** Audit log of changes made by people and of admin actions (who, what, when).
- **NFR-S7** Supply chain: Dependabot, CodeQL, secret scanning with push protection, pinned versions of GitHub Actions, and an SBOM of our own code (we use the practices we're measuring).
- **NFR-S8** Target OWASP ASVS Level 2. Keep a threat model up to date.
- **NFR-S9** Validate and encode all external input, including data from source APIs. Treat repo descriptions and READMEs as untrusted content.
- **NFR-S10** Before any deployment that is reachable from the internet:
  - Entra ID sign-in is in place.
  - Web application firewall (WAF) and DDoS protection are chosen and recorded in an ADR. Azure Front Door's managed WAF rules are probably too expensive for a proof of concept, so this is still open; see [unknowns](unknowns.md).
  - A security review and IT health check plan are agreed.

  Until then, the service runs **locally only**.

### Privacy

- **NFR-P1** Do not store personal data (people and contacts) until a DPIA is completed and a privacy notice is published.
- **NFR-P2** Collect the minimum personal data. Retention rules and the ability to delete must be designed in.

### Accessibility and frontend

- **NFR-A1** Server-rendered HTML using the GOV.UK and DfE design systems (components, patterns and branding). Which packages are used is decided in [ADR-0005](adr/0005-server-rendered-ui-govuk-dfe-frontend.md) (U2). We publish an accessibility statement (WCAG 2.2 AA).
- **NFR-A2** Fully usable without JavaScript. JavaScript is only a progressive enhancement. Browser tests prove that the main user journeys work with JavaScript turned off.
- **NFR-A3** Automated accessibility checks in browser tests (Playwright with axe-core). Manual checks with keyboard and assistive technology for each new pattern.

### Reliability, performance and operations

- **NFR-O1** Low traffic is expected. No need for "enterprise scale", but no design choices that rule out scaling. Collection runs separately from serving pages, so heavy collection doesn't slow the website down.
- **NFR-O2** Collection respects source API rate limits (handles rate-limit headers and backs off), can be resumed and is idempotent.
- **NFR-O3** Observability through OpenTelemetry (Aspire service defaults): logs, traces and metrics. Health checks.
- **NFR-O4** Performance tests (for example k6 or NBomber) set a baseline for the main pages and collection runs. A slice that is about performance must show before and after measurements.
- **NFR-O5** Runs fully on a local machine (Aspire with a PostgreSQL container). This is the main way the service is run until NFR-S10 is met and costs are acceptable.
- **NFR-O6** Keep costs low. Prefer Azure options that can scale to zero or have a free tier. Record cost estimates in an ADR for any hosting decision.

### Developer and first-run experience

- **NFR-D1** **Ten-minute start**: `git clone` -> `dotnet build` -> `dotnet run` (the Aspire AppHost) gives a working, realistic-looking service
  with seeded fake data, without configuring any secrets. **Aspire is fundamental to this**: it starts PostgreSQL, Web and Worker and connects them.
  Prerequisites are kept to a minimum and listed in the README: the .NET 10 SDK, a container runtime that Aspire supports (Docker Desktop or Podman, see R6),
  and Node only if it can't be avoided (see U24).
  This is checked in CI by a "fresh clone" job that runs on a clean runner, with a time limit.
- **NFR-D2** **Frontend assets are built by the .NET build.** Any npm, Sass or JS bundling runs through MSBuild targets, built incrementally
  (Inputs/Outputs), so `dotnet build` is enough. No separate manual step.
- **NFR-D3** **Anything not configured is disabled or faked, never broken**, and the UI always says when something is faked: there's a banner, a status page, and health checks report it.
  The modes and what each integration falls back to are in [architecture](architecture.md#integration-modes). The decision is [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md).
- **NFR-D4** **Environments** are `LocalDevelopment`, `Development` (deployed), `Test` and `Production` ([architecture](architecture.md#environments)).
  Code only checks the environment through `IsLocalDevelopment()` and `IsDevEquivalent()`. Security-relevant fakes require `IsLocalDevelopment()`. `IsDevelopment()` is banned.
- **NFR-D5** **First-run bootstrap**: a new installation can be set up without manual database steps. Doing it concurrently is safe, it's audited, and there are extra protections outside `LocalDevelopment` ([ADR-0011](adr/0011-first-run-bootstrap.md)).

### Verification and drift detection

- **NFR-V1** Every decision (ADR) and requirement says how it's verified and how drift is detected, where that's reasonable. See [ADR-0012](adr/0012-verification-checks-and-drift-detection.md).
- **NFR-V2** Check results distinguish "we couldn't see it" from "it isn't there", and "doesn't meet the requirement" from "a judgement call is needed". The outcome levels are defined in [ADR-0012](adr/0012-verification-checks-and-drift-detection.md).
- **NFR-V3** Repository checks are written once and run against both our own repository (in CI) and repositories across the estate (Insights) ([ADR-0012](adr/0012-verification-checks-and-drift-detection.md)).
- **NFR-V4** **Custom Roslyn analysers (and code fixes)** enforce our conventions at compile time, in a `ServiceCatalogue.Analyzers` project
  with its own tests. The suite grows over time, and each analyser links to the ADR or requirement it enforces. Initial candidates:
  - ban `IsDevelopment()` (use `IsLocalDevelopment()` or `IsDevEquivalent()`)
  - ban `Html.Raw` and `HtmlString` built from variables
  - ban `DateTime.Now` and `DateTime.UtcNow` (use `TimeProvider`)
  - every Razor page or endpoint must have an explicit authorisation policy, or `[AllowAnonymous]` with a stated reason
  - ban reading configuration or secrets outside the options classes
  - every integration options class must declare what happens when it isn't configured (see NFR-D6)
  - ban logging properties marked as secret or personal

  Built-in and third-party analysers (`BannedApiAnalyzers`, the .NET analysers) are used where they already do the job.
- **NFR-V5** **Single source of truth for documentation**: every fact has one canonical home ([where things live](README.md#where-things-live)). Anything repeated is linked or generated, never copied, and a docs checker enforces this in CI ([ADR-0013](adr/0013-single-source-of-truth.md)).

### Required secrets

- **NFR-D6** The aim is **no required secrets** in `LocalDevelopment`. Where a secret is genuinely needed and no fake is possible, it is **flagged explicitly**:
  - declared in code (for example a `[RequiredSecret("GitHub:Token", Reason = "...", Environments = ...)]` attribute on its options class, or a central list), including **why there's no credential-less option** (NFR-S4)
  - listed in the README's **Required secrets** table, with the reason, how to get it, and which environments need it
  - shown on the Integrations status page
  - checked at startup: if it's missing, the app fails fast with a clear message explaining how to fix it, and never with an obscure exception later on

  A CI check makes sure the secrets declared in code and the README table match exactly.
- **NFR-D7** **Opting in to the real thing takes little effort.** Swapping a fake for a real integration (a GitHub token, an Entra app registration, a Notify key)
  or the Aspire-managed PostgreSQL for an existing database takes **one copy and paste plus a placeholder to fill in**, not a hunt through documentation. For each integration:
  - ready-to-copy commands with obvious placeholders, for example
    `dotnet user-secrets set "Parameters:github-token" "<paste-your-token-here>" --project src/ServiceCatalogue.AppHost`,
    for PowerShell, cmd and bash where they differ
  - where to get the value, and the **minimum permissions** it needs (for example a fine-grained, read-only token for public repositories), with a link
  - how to check it worked (the Integrations status page shows *Configured*)
  - how to undo it (remove the secret, and it goes back to the fake)

  This information is **declared once, alongside the integration's options in code**. The README section and the Integrations status page (which shows
  the command for each integration that isn't configured) are **generated from that declaration**, so they can't drift apart.
  Configuration goes through Aspire parameters and connection strings on the AppHost, so there's one place to set things.

### Quality and evidence

- **NFR-Q1** Test-driven development, test-first where practical. Every slice shows the relevant tests going from failing to passing.
- **NFR-Q2** Testing pyramid:
  - unit tests
  - integration tests (WebApplicationFactory, with a PostgreSQL container via Testcontainers, and recorded or stubbed GitHub API responses)
  - **browser tests (Playwright for .NET)** for user journeys, accessibility (axe) and working without JavaScript
  - architecture tests enforcing module boundaries
- **NFR-Q3** CI runs on every pull request: build, all test layers, formatting and linting, and security scanning. Playwright screenshots and traces are kept as build artefacts.
- **NFR-Q4** **Before and after evidence is required for every issue and pull request.** Recordings (GIFs or video) are preferred for journeys. See [ways of working](ways-of-working.md#evidence).
- **NFR-Q5** Mutation testing (Stryker.NET) is a stretch goal.
- **NFR-Q6** Acceptance criteria cover every section in [ways of working: acceptance criteria](ways-of-working.md#acceptance-criteria), each with tests.
- **NFR-Q7** **Fake data only** in tests, CI, local development and demos, with obviously fake identities and named, repeatable seeding scenarios ([test data](ways-of-working.md#test-data)).
- **NFR-Q8** Text handling is Unicode-correct from end to end: unusual but valid text is kept exactly, and search and duplicate detection treat equivalent forms as equal ([unusual but valid input](ways-of-working.md#3-unusual-but-valid-input)).

### Security testing

- **NFR-S11** Automated DAST with **OWASP ZAP** (a baseline scan on every pull request, and fuller scans on a schedule), alongside SAST (CodeQL), dependency, secret, workflow (zizmor) and, when used, container and infrastructure-as-code scanning. New findings at medium severity or above fail the build. Suppressions need a reason and an expiry date. See [ways of working](ways-of-working.md#automated-security-and-quality-scanning).
