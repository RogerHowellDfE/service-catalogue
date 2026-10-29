# Roadmap

We deliver with continuous flow (Kanban), so there are no fixed sprints. Milestones are **outcomes**, not dates.
Items are refined into slices "just in time" as they reach the top of the board.

## Now

### M0: Walking skeleton

Goal: everything needed to deliver slices safely, and the smallest end-to-end path.

- New .NET 10 / Aspire solution: Web, Worker, PostgreSQL, the module structure, and architecture tests.
- **First-run experience** (NFR-D1 to D7): the ten-minute start, including:
  - environments and helpers (NFR-D4)
  - integration modes, the banner and the Integrations status page ([ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md))
  - first-run bootstrap ([ADR-0011](adr/0011-first-run-bootstrap.md))
  - the fresh-clone CI job, with a recording kept as evidence
- A page laid out with the DfE frontend that works without JavaScript and passes axe (NFR-A1 to A3).
- Sign-in required by default (NFR-S1, NFR-S2, [ADR-0006](adr/0006-authentication-and-authorisation.md)).
- About the service (C19): footer links and the "What's new" page generated from the changelog ([ADR-0015](adr/0015-date-based-versions-and-changelog.md)).
- Release script or workflow, and the version shown in the app ([ADR-0015](adr/0015-date-based-versions-and-changelog.md)).
- Seeding scenarios, starting with `empty` and `small-estate`, selected by an AppHost parameter ([test data](ways-of-working.md#seeding-scenarios)).
- A first threat model (`docs/security/threat-model.md`, NFR-S8), drafted early and updated by each slice that adds a trust boundary.
- **Verification on ourselves** ([ADR-0012](adr/0012-verification-checks-and-drift-detection.md)):
  - the checks engine and CLI, starting with an OSI-approved `LICENCE` and the presence of the repo hygiene files
  - the analysers project, starting with banning `IsDevelopment()` and the EF InMemory provider
  - the docs checker, including the ADR checks (`tools/docs-check.cs`, [ADR-0013](adr/0013-single-source-of-truth.md)) and the retired-terms list (`docs/terms.yml`)
  - the repository checks that accepted ADRs already call for: no deployment or infrastructure-as-code files while running locally ([ADR-0009](adr/0009-local-first-hosting.md)), and no secrets in configuration files ([ADR-0014](adr/0014-credential-less-by-default.md))
  - Playwright checks for headers, Content Security Policy and frontend markup
- CI: build, test (all layers), CodeQL (DfE SAST workflow), Dependabot, secret scanning, ZAP baseline. Pull request template with an evidence section.
- Repo hygiene: meet every row marked M0 in the [DfE repository requirements](standards.md#dfe-repository-requirements) (with the single-developer departure, U19), plus a README, SECURITY.md and the Copilot/agent instructions.
- Spike: decide which DfE Frontend distribution to use (and record the decision in an ADR).

### M1: GitHub census (first value for leadership)

Goal: leadership can browse the code in DFE-Digital and see its shape.

- Admin: manage tracked organisations (C1).
- Collect repositories and languages for tracked organisations, using a token. Snapshots and run history. Admins can run collection on demand (C2, C3).
- Repository list with search and filters (language, archived, visibility, activity), plus a detail page (C4).
- Language breakdown and activity summary across the organisation (C5, in part).
- Performance baseline for the list and summary pages.

**Demo outcome:** "Here is our DFE-Digital estate: N repos, of which X are active, and these are the languages used and how widely."

## Next

### M2: Risk signals v1

- The checks framework, and a findings dashboard grouped by signal.
- First checks, starting with the **[DfE repository requirements](standards.md#dfe-repository-requirements)**, which already cite a DfE standard:
  - LICENCE, CONTRIBUTING, CODEOWNERS and topics are present
  - Dependabot, SAST, DAST and infrastructure-as-code scanning are configured
  - branch protection is on
  - no CI/CD workflows we can see
  - end-of-life runtimes and frameworks (from the SBOM, manifests and endoflife.date)
  - very old dependencies
  - archived repos that are still being referred to
- Each signal mapped to a Service Standard or Technology Code of Practice point, with supportive guidance.
- Trends over time from the snapshot history.
- Scheduled refresh.
- **Publish a draft of the sidecar metadata schema for comment** (this takes time, so start it early).

### M3: Enrichment and relationships

- Services and products that group repositories, with the source of each piece of data recorded (C8).
- Tags and exemplar endorsements (C9).
- Relationships we can infer: reusable workflows, shared packages, Terraform and Bicep module sources, container images (C10).
- Reading the sidecar file, v1 (C11).

## Later

- **M4: Ready for deployment**: Entra ID tenant, WAF/DDoS choice (cost-aware), credential-less access throughout ([ADR-0014](adr/0014-credential-less-by-default.md), U26), infrastructure as code (U18), security review. This is the condition for any Azure deployment.
- **M5: Wider sources**: GitHub App (private repos, more organisations), ADO, ServiceNow/CMDB cross-reference.
- **M6: People and identity**: DPIA, contacts, Entra-GitHub account linking.
- **M7: Data flows**: data assets and interfaces, linking to Find and Use an API.
- **M8: Action**: support initiatives, onboarding and offboarding, provisioning automation, notifications through GOV.UK Notify.

The order within Next and Later will be revisited as we learn.
