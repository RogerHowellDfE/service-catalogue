# Unknowns, risks and dependencies

Things we know we don't know yet. Each has a horizon by which it needs answering. When one is answered, record the
decision (often as an ADR) and move it to *Resolved*.

## Open

| # | Item | Type | Needed by | Notes / next step |
|---|---|---|---|---|
| U1 | Entra ID tenant: DfE tenant or a personal Visual Studio subscription development tenant? | Dependency | M4 (M0 works with the fake persona selector) | A personal development tenant is likely for now |
| U2 | Which DfE Frontend distribution: npm `dfe-frontend`, or v3 `@dfedigital/dfe-frontend` + `DfEDigital.Frontend.AspNetCore` (currently only a basic card tag helper)? Are the GOV.UK Frontend versions compatible with `GovUk.Frontend.AspNetCore` (currently GOV.UK Frontend 6.6.0)? | Spike | M0 | Speak to DfE DesignOps if unclear |
| U3 | WAF and DDoS protection that fits a proof of concept budget. Front Door managed WAF rules are probably too expensive. Alternatives: App Gateway WAF (also costly), only allowing access from known IP ranges, or not exposing the service publicly at all. | Decision | M4 | Run locally until resolved |
| U4 | Final hosting (Container Apps provisional) and its cost at near-zero traffic | Decision | M4 | Includes the cost of PostgreSQL Flexible Server (no scale-to-zero) |
| U5 | Domain model: what is a "service" vs a "product" vs a "repo" vs a "component"? Is there a DfE definition already, for example in the service register or ServiceNow? | Discovery | M3 | Affects the sidecar schema |
| U6 | How to infer links between repos reliably (packages, workflows, infrastructure modules, images, submodules, APIs) | Spike | M3 | |
| U7 | Which Service Standard and Technology Code of Practice points can be checked automatically, and how | Discovery | M2 | Build up a catalogue of checks step by step |
| U8 | GitHub rate limits and collection cost for DFE-Digital (number of repos, calls per repo) with a token | Spike | M1 | Measure in M1; may bring the GitHub App forward |
| U9 | DPIA, privacy notice, data owner for personal data | Dependency | M6 | Long lead time; start early |
| U10 | Who governs the catalogue: data owner, moderation of tags, appeals process for findings | Discovery | M3 | |
| U11 | How findings are communicated to teams, so they feel supportive rather than punitive | Discovery | M2 | |
| U12 | Access to ADO organisations and ServiceNow APIs | Dependency | M5 | |
| U13 | User research: the needs in `user-needs.md` are assumptions | Risk | Ongoing | Test with technical leadership in the M1 demo |
| U14 | Existing DfE or cross-government tools that overlap (catalogues, inventories, API registers) | Risk | M1 | Check before building too much; reuse and link |
| U15 | Product name | Decision | Any time | Follow [DfE naming guidance](https://technical-guidance.education.gov.uk/standards/naming-things/): self-descriptive, no brands or puns |
| U16 | Repository location. DfE guidance says code belongs in the DFE-Digital organisation, and work started elsewhere should be transferred "at the earliest opportunity". The repo is currently under a personal account. | Decision | Before sharing more widely | Transferring keeps history and redirects. The GitHub Project is owned by the personal account and doesn't move with the repo. Repository topics are agreed when it's adopted |
| U17 | Hosting through **Azure CIP**, the DfE route, versus a personal Visual Studio subscription. Is Container Apps available within CIP? (The CIP page lists App Service and Container Instances.) | Decision | M4 | Could change ADR-0009 to App Service |
| U18 | Infrastructure as code: DfE recommends **Terraform/ARM**. Aspire/`azd` uses Bicep by default. | Decision | M4 | Lean towards Terraform to match DfE practice, unless there's a strong reason not to |
| U19 | The branch protection DfE requires (at least one approval, code owners review) can't be met by a single developer | Departure | M0 | Require pull requests, status checks and resolved conversations, with the Copilot code review as a stand-in (requested manually per pull request, not automatic, to control cost). Turn on required approvals when a second maintainer joins |
| U20 | Measures of success (SS point 10): what tells us the census and insights are valuable? For example, leadership decisions made using it, support initiatives started, data freshness, number of repos covered | Discovery | M1 demo | |
| U21 | Which monitoring and telemetry backend to use when deployed (App Insights, Sentry, Logit are the DfE options). OpenTelemetry keeps the options open | Decision | M4 | |
| U22 | DfE's security events webhook (repository requirement): what it is and where it goes | Discovery | M0 / M4 | Ask the DfE CIS / SbD team |
| U23 | *(Resolved: see R6)* | | | |
| U24 | Is Node/npm needed at all? It depends on U2: the NuGet static-asset route may avoid it. If it's needed, run it through MSBuild targets (NFR-D2) | Spike | M0 | Example of an MSBuild/npm integration: [DFE-Digital/sts-plan-technology-for-your-school#1296](https://github.com/DFE-Digital/sts-plan-technology-for-your-school/pull/1296) (`Dfe.PlanTech.Web.SiteOfflineMicrosite.csproj`). Its `SecurityHeadersTests.cs` is useful inspiration for NFR-S5 tests, but not to copy wholesale |
| U25 | Bootstrap protections outside `LocalDevelopment`: a one-time setup token (from logs or Key Vault)? Seeding allowed in `Development` and/or `Test`? ([ADR-0011](adr/0011-first-run-bootstrap.md)) | Decision | M4 | Proposed: token required; seeding only in dev-equivalent environments |
| U26 | Are credential-less options ([ADR-0014](adr/0014-credential-less-by-default.md)) available in the target tenant and subscription: managed identities, Entra authentication for PostgreSQL, federated credentials on app registrations, GitHub OIDC to Azure? CIP or tenant policy may restrict them | Dependency | M4 | Ask CIP. If they're restricted, record the departure and use the next option down |
| U27 | Authorisation model: where roles and permissions come from. Options include Entra app roles for site-wide and high-assurance roles with app-managed grants for everything else; the app managing Entra role assignments; and permissions **by association** (someone added to a service in a role gets rights on that service). Also: should sensitive configuration (GitHub credentials, background job settings) be changed at runtime by admins, or only at deploy time? | Decision | Before the first admin feature (M1, C1) | Constraints: no large-scale manual role administration; don't design ourselves into a corner; [ADR-0011](adr/0011-first-run-bootstrap.md) needs the app to record at least the first administrator. Write an ADR that amends [ADR-0006](adr/0006-authentication-and-authorisation.md) |
| U28 | Which tests and scans run on which trigger: every push, pull request, after merging, nightly, weekly, or against a deployed environment (for example performance tests as part of continuous delivery) | Decision | M0 (first CI), revisited at M4 | Start simple: everything fast on every pull request. Split it out only when it gets slow. Don't over-engineer this early |

## Resolved

| # | Item | Resolution |
|---|---|---|
| R1 | Build or adopt (for example Backstage) | Build in C# with the DfE frontend; borrow conventions (`catalog-info.yaml`, `publiccode.yml`) |
| R2 | Datastore | PostgreSQL ([ADR-0004](adr/0004-postgresql.md)) |
| R3 | GitHub access for the proof of concept | Fine-grained token, behind an interface ([ADR-0007](adr/0007-github-access.md)) |
| R4 | Repository visibility | Public (coding in the open) |
| R5 | Where the service runs | Locally only until M4 |
| R6 | Container runtime as a prerequisite (was U23) | Yes. Aspire is fundamental to the first run, and it needs a container runtime for PostgreSQL (also used by Testcontainers). Docker Desktop or Podman: Podman avoids Docker Desktop licensing questions on DfE devices. Using an existing database instead is an opt-in (NFR-D7) |
