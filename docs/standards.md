# Standards and guidance

This is the reference list we trace decisions, requirements and delivery slices back to. ADRs, issues and pull requests are **prompted by their
templates** to name the standards and guidance they touch. Where we depart from a recommendation, we say so in an ADR and explain why.

## Sources

| Abbreviation | Source | Notes |
|---|---|---|
| **SS** | [GOV.UK Service Standard](https://www.gov.uk/service-manual/service-standard) and the [DfE version: Apply the Service Standard](https://apply-the-service-standard.education.gov.uk/service-standard) | 14 points. DfE adds context and examples |
| **SM** | [GOV.UK Service Manual](https://www.gov.uk/service-manual) | Especially the technology, accessibility and user research sections |
| **TCoP** | [Technology Code of Practice](https://www.gov.uk/guidance/the-technology-code-of-practice) | 13 points |
| **SbD** | [Secure by Design (government)](https://www.security.gov.uk/policy-and-guidance/secure-by-design/) and the [DfE Secure by Design documentation](https://secure-by-design.security.education.gov.uk/) | Principles, activities, DfE processes (for example triaging vulnerabilities) |
| **DfE-TG** | [DfE technical guidance](https://technical-guidance.education.gov.uk/) ([source](https://github.com/DFE-Digital/technical-guidance)) | Principles, standards, default technology stack, hosting, security |
| **DfE-DM** | [DfE design manual](https://design.education.gov.uk/) and [DfE Frontend](https://design.education.gov.uk/design-system/dfe-frontend) | Plus the [Digital Product Branding](https://standards.education.gov.uk/standard/digital-product-branding) standard |
| **GDS-DS** | [GOV.UK Design System](https://design-system.service.gov.uk/) | Components and patterns |
| **WCAG** | [WCAG 2.2](https://www.w3.org/TR/WCAG22/) AA | Legal requirement (Public Sector Bodies Accessibility Regulations) |
| **ASVS** | [OWASP ASVS](https://owasp.org/www-project-application-security-verification-standard/) Level 2 | Security verification target |

## DfE technical guidance pages most relevant to us

| Page | What it means for us |
|---|---|
| [Technical architecture principles](https://technical-guidance.education.gov.uk/principles/architecture/) | Start small, focus on user needs, use the web and open standards, reuse emerges, release early |
| [Development (coding) principles](https://technical-guidance.education.gov.uk/principles/coding-principles/) | "Everything fails, all of the time", TDD/BDD, the testing pyramid, consistent mocking, measuring coverage |
| [Frontend development principles](https://technical-guidance.education.gov.uk/principles/frontend-development-principles/) | Progressive enhancement, accessibility in CI (for example pa11y), functional tests covering *all* journeys, not just the happy path, beware single-page apps |
| [Data principles](https://technical-guidance.education.gov.uk/principles/data/) | Data is an asset, secure, shared and obtainable, analysable, used ethically. This matters for the data-flows capability |
| [Secure by Design principles](https://technical-guidance.education.gov.uk/principles/secure-by-design-principles/) | Follow SbD, and use the DfE SbD documentation and tooling |
| [The DfE technology stack](https://technical-guidance.education.gov.uk/guides/default-technology-stack/) | .NET preferred ✔. Azure via CIP. Terraform/ARM for infrastructure as code. GitHub Actions ✔. App Insights, Sentry, Logit, StatusCake |
| [Azure CIP hosting](https://technical-guidance.education.gov.uk/infrastructure/hosting/azure-cip/) | The DfE route to Azure subscriptions (onboarding needs Grade 7 or above sponsorship and agreed finance) |
| [Storing source code](https://technical-guidance.education.gov.uk/standards/storing-source-code/) | **Repository requirements** (see below). The DFE-Digital organisation is the default home for code |
| [Licensing software or code](https://technical-guidance.education.gov.uk/standards/licencing-software-or-code/) | MIT licence in a file called `LICENCE` or `LICENCE.md`, "Crown Copyright (Department for Education)" |
| [Naming things](https://technical-guidance.education.gov.uk/standards/naming-things/) | Self-descriptive names, no brands or puns, hyphens in repo names, repo name matches the service's hostname |
| [Managing secrets](https://technical-guidance.education.gov.uk/infrastructure/security/managing-secrets/) | Key Vault, managed identities, GitHub secrets |
| [API guidance](https://technical-guidance.education.gov.uk/guides/api-guidance/) | For the later read API and data-flow features |
| [GitHub guides: Copilot, working safely](https://technical-guidance.education.gov.uk/guides/github/) | AI-assisted delivery |

## DfE repository requirements

These come from the "Storing source code" page. They have two uses:

1. **Our own repository** must meet them (tracked in M0).
2. They're a **ready-made source of checks** for the Insights module (M2). Each requirement can become an automatic check that cites a DfE standard.

| Requirement | Ours | Can it be checked automatically? |
|---|---|---|
| Branch protection or rulesets on the default branch (an approval, dismissing stale approvals, code owners review, approving the most recent push, resolving conversations, status checks) | Partly. See U19: a single developer can't approve their own pull requests | Yes (API, with the right permissions) |
| Clear name | ✔ `service-catalogue` | Partly (heuristics) |
| `LICENCE` or `LICENCE.md` (MIT, Crown Copyright) | ✔ `LICENCE.md` | Yes |
| `CONTRIBUTING.md` | ✔ | Yes |
| `CODEOWNERS` (teams preferred) | ✔ an individual for now (U19) | Yes |
| Topics for ownership, team, product type, production or prototype | M0 | Yes (needs an agreed set of topics; see U5) |
| Software composition analysis (Dependabot) | M0 | Yes (`dependabot.yml`) |
| SAST (CodeQL or Semgrep; [DfE SAST reusable workflow](https://github.com/DFE-Digital/github-actions/tree/master/sast-reusable-workflow)) | M0 | Yes (workflow inspection) |
| DAST (ZAP or nuclei) | M0/M1 | Yes (workflow inspection) |
| Unit tests and linters run | M0 | Partly (workflow inspection) |
| Infrastructure-as-code scanning (Checkov) | When we add infrastructure as code | Yes |
| Container scanning (Trivy) | When we add containers | Yes |
| Private vulnerability reporting | M0 | Yes (API) |
| Secret scanning with push protection | M0 | Yes (API, with permissions) |
| Webhook for security events | Ask what DfE uses | Unknown |
| `.gitignore` | ✔ | Yes |
| Higher risk: signed commits, SBOMs, artefact attestations | Stretch goal | Yes |

## Service Standard points: where they apply

| SS point | Where we address it |
|---|---|
| 1 Understand users and their needs | `user-needs.md`; user research with technical leadership (U13) |
| 2 Solve a whole problem for users | Vision: from census to insight to action |
| 3 Provide a joined-up experience | Link to existing services (Find and Use an API, ServiceNow) rather than duplicating them |
| 5 Make sure everyone can use the service | NFR-A*, axe/pa11y in CI, unusual but valid input |
| 6 Have a multidisciplinary team | ⚠ A single developer plus Copilot. Recorded as a risk, and part of the case for growing the team |
| 7 Use agile ways of working | `ways-of-working.md` |
| 8 Iterate and improve frequently | Vertical slices, continuous flow |
| 9 Create a secure service which protects users' privacy | NFR-S*, NFR-P*, SbD, threat model, DPIA before personal data |
| 10 Define what success looks like and publish performance data | ⚠ Not yet defined. Add measures of success to the roadmap (U20) |
| 11 Choose the right tools and technology | ADRs, DfE technology stack |
| 12 Make new source code open | Public repository, MIT licence |
| 13 Use and contribute to open standards, common components and patterns | GOV.UK and DfE Frontend, `publiccode.yml` and Backstage conventions, contributing back |
| 14 Operate a reliable service | NFR-O*, failure modes in every story |
