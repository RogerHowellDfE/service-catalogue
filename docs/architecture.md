# Architecture

Status: **draft**. Decisions are recorded in [ADRs](adr/README.md). This document gives the overall picture.

## Overview

A **modular monolith**: one solution, one database server and one deployable codebase, divided into modules
with strict boundaries. It runs as two processes:

- **Web**: server-rendered ASP.NET Core using GOV.UK and DfE frontend tag helpers. It handles pages,
  sign-in and admin.
- **Worker**: runs data collection and analysis jobs in the background, on a schedule or on demand. It's the same modules,
  hosted separately, so collection never slows the website down. In Azure it could become Container Apps Jobs.

Locally, an **Aspire AppHost** runs Web, Worker and PostgreSQL together.

```
                +-------------------- Aspire AppHost (local) --------------------+
                |                                                                |
 Browser ---->  |  Web (Razor, tag helpers)        Worker (collection, analysis)  |
                |     |                               |                          |
                |     +-------- Modules (in-process) -+                          |
                |                 |                                              |
                |            PostgreSQL (one schema per module)                  |
                +----------------------------------------------------------------+
                                  |
                  GitHub API (token; GitHub App later), later ADO, ServiceNow, ...
```

## Modules (bounded contexts)

| Module | Responsibility | Horizon |
|---|---|---|
| **Sources** | Tracked organisations and source configuration, connectors (GitHub, then ADO and others), collection runs, raw snapshots, rate-limit handling | Now |
| **Catalogue** | The normalised record: repositories, then services and products, relationships, tags. Records where each piece of data came from (collected or entered by a person). | Now |
| **Insights** | Technology profile, pluggable checks, findings, aggregates and trends | Now / Next |
| **Identity** | Sign-in, roles and policies, current user, later Entra-GitHub account linking | Now |
| **Audit** | A record of changes and admin actions that can only be added to, never edited | Now |
| **People** | Contacts and roles on services, after the DPIA | Later |
| **DataFlows** | Data assets, interfaces, who creates and who consumes, links to Find and Use an API | Later |
| **Provisioning** | Onboarding and offboarding and resource requests, through adapters to GitHub, Azure, Microsoft Graph and so on | Later |

Cross-cutting work that isn't a module (the hosts, SharedKernel, CI, tooling, the checks engine, analysers and the docs checker) is labelled `module:platform` in the backlog.

### Module rules

Defined in [ADR-0002](adr/0002-modular-monolith.md): Contracts-only references between modules, one schema per module, in-process events (with an outbox later),
and a small SharedKernel. They're enforced by architecture tests. The UI is organised by feature, and a page only calls its own module's Contracts.

## Data pipeline: collect, normalise, assess

Defined in [ADR-0008](adr/0008-snapshots-provenance-and-checks.md). In outline:
**raw snapshots** (Sources, never changed) -> **observations recording their sources** (Catalogue) -> **checks and findings** (Insights) -> **read models** (dashboards).
Because we keep the raw snapshots, we can reprocess history whenever we add new checks or improve parsing, without collecting again.

## Environments, configuration and fallbacks

### Environments

| Environment | Where | Persona selector (fake sign-in) | Fakes / recorders | Seeding scenarios |
|---|---|---|---|---|
| `LocalDevelopment` | A developer's machine (set in `launchSettings.json` and the AppHost) | Allowed if sign-in isn't configured | Allowed, with a banner | Allowed |
| `Development` | Deployed development environment | **Never.** Missing sign-in configuration stops the app | Allowed, with a banner, and health shows *degraded* | Allowed (U25) |
| `Test` | Deployed test environment | Never | Allowed, with a banner, and health shows *degraded* | To decide (U25) |
| `Production` | Live | Never | Only where the ADR allows it (for example the Notify recorder). Health shows *degraded* | Never |

The rules are NFR-D4 and [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md). Once the code exists, this table will be generated from it ([ADR-0013](adr/0013-single-source-of-truth.md)).

### Environment-dependent behaviour

Every behaviour that differs by environment. Program.cs has no environment checks; the extension named here decides ([ADR-0016](adr/0016-one-obvious-way.md)).

| Behaviour | `LocalDevelopment` | `Development` | `Test`, `Production` | Decided in |
|---|---|---|---|---|
| Health endpoints `/health` (includes the database) and `/alive` | Mapped | Mapped | Not mapped | `ServiceDefaultsExtensions.MapDefaultEndpoints` |
| Unhandled errors | Developer exception page | `/Error` page, HSTS | `/Error` page, HSTS | `ErrorPagesExtensions.UseErrorPages` (Web) |
| Static web assets served from projects and packages (not yet published) | On | Off | Off | `StaticAssetsExtensions.AddStaticAssets` (Web) |
| Service registrations validated at startup (scopes, missing dependencies) | On | Off | Off | `ServiceDefaultsExtensions.AddServiceDefaults` |

### Local configuration

Local configuration has one home: the AppHost's user-secrets. Web and Worker don't load user-secrets in any environment ([ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)).

| Stage | Who | Where settings come from |
|---|---|---|
| Try it | Anyone: content designers, user researchers, demos | Nothing to set. Every integration falls back to a fake or is disabled, and says so |
| Develop, with variants | Developers, for example a personal Entra tenant or a shared database | AppHost user-secrets. Each integration's opt-in command is declared in code (NFR-D7). Removing the secret undoes it |
| Hosted | `Development`, `Test`, `Production` | Platform settings and Key Vault, read with managed identity ([ADR-0014](adr/0014-credential-less-by-default.md)). The same options classes; only the source differs |

Running Web or Worker without the AppHost means passing settings as environment variables or command-line arguments.

### Integration modes

Each integration is **Configured**, **Fake/Recording** or **Disabled** ([ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)). The current fallbacks are listed below. Later, this table is generated from the integration declarations in code.

| Integration | Not configured -> | Credential when configured | Notes |
|---|---|---|---|
| Entra ID sign-in | `LocalDevelopment`: fake persona selector (Fake Francis, Test Teri...; their roles come from the seeded scenario, U27). **Anywhere else: refuse to start** | None: federated credential from managed identity ([ADR-0014](adr/0014-credential-less-by-default.md)) | Security-critical, so it fails closed |
| PostgreSQL | Aspire-managed container | None when deployed: Entra authentication with managed identity. Local container: generated by Aspire, never stored | |
| GitHub | Fake GitHub source (seeded scenarios) | **Personal access token (a temporary departure)**; later a GitHub App whose key never leaves Key Vault | Banner "Showing fake GitHub data" |
| GOV.UK Notify (later) | **Recorder**: stores what *would* have been sent, shown on the admin page "Messages not sent" | API key (no alternative), in Key Vault | Uses a fake recipient domain in seeds |
| endoflife.date, OSV (later) | Fake source with fixed snapshot data | None (public APIs) | |
| ADO, ServiceNow (later) | Disabled, or a fake source | ADO: Entra workload identity. ServiceNow: to be decided | |

### First run

See [ADR-0011](adr/0011-first-run-bootstrap.md): the first user to sign in becomes an administrator, it's done atomically and audited. Seeding is separate: the AppHost seeds the selected scenario (default `small-estate`) into an empty database at startup.

### Continuous integration

CI runs the application as `LocalDevelopment`: it's short-lived and has no real credentials, so the fresh-clone, browser and ZAP jobs can use the persona selector and fakes.
Which tests run on each trigger (every push, pull request, after merging, on a schedule, or against a deployed environment) is U28.
The first call: `.github/workflows/ci.yml` runs everything fast on every pull request and every push to `main`, as two parallel jobs, `build-and-test` and `format`, so a formatting failure never stops tests running and reporting. Restores are in locked mode against the committed `packages.lock.json` files. Split jobs out by trigger only when the run gets slow (about 10 minutes).

## Verification checks and drift detection

One checks engine, with the outcome levels defined in [ADR-0012](adr/0012-verification-checks-and-drift-detection.md). This is where each kind of check runs:

- **Repository contents and metadata**: written against an abstraction, which is supplied by:
  - the **local working copy**, through a CLI (`dotnet run --project tools/ServiceCatalogue.Checks.Cli -- .`), run in CI against our own repository
  - the **collected snapshots**, in the Insights module, for repositories across the estate
- **Running application** (headers, Content Security Policy, no-JavaScript behaviour, DfE/GOV.UK frontend in use): Playwright and ZAP tests. Some may later be offered as a scan of other services.
- **Code** (conventions, module boundaries): Roslyn analysers and architecture tests.
- **Documentation** (IDs, links, duplication, generated blocks): the docs checker ([ADR-0013](adr/0013-single-source-of-truth.md)).

Examples, starting with ourselves:

| Decision | Check | Where it runs |
|---|---|---|
| OSI-approved licence in `LICENCE` or `LICENCE.md` | The file exists, its licence is detected, and the SPDX identifier is in the [SPDX licence list](https://spdx.org/licenses/) with `isOsiApproved` | CLI on our repo; Insights across the estate |
| Modular monolith | Modules only reference each other's Contracts | Architecture tests |
| GOV.UK and DfE frontend used | Pages contain the expected template markup and stylesheets | Playwright |
| Security headers (ASVS, TCoP, SS 9) | Content Security Policy, HSTS and so on are present and correct | Playwright + ZAP |
| Dependabot / SAST / DAST configured | The configuration and workflow files exist and contain the expected jobs | CLI; Insights |

## External services and what we reuse

| Need | Use | Notes |
|---|---|---|
| Staff sign-in | Entra ID (Microsoft.Identity.Web) | Development tenant until a DfE tenant is confirmed |
| GitHub data | GitHub REST and GraphQL APIs (Octokit) | Token now; GitHub App later for private repos and higher rate limits |
| Dependency data / SBOM | GitHub dependency graph SBOM export | Don't generate SBOMs ourselves |
| End-of-life runtimes and frameworks | [endoflife.date](https://endoflife.date) API | |
| Vulnerability data | OSV / GitHub Advisory Database | |
| Repo security posture | Consider OpenSSF Scorecard | Spike |
| Notifications | GOV.UK Notify | Later |
| API catalogue | Find and Use an API (api.gov.uk) | Link to it; don't duplicate. Register our read API there later. |
| Metadata file conventions | Backstage `catalog-info.yaml`, `publiccode.yml` | Inform the sidecar schema |
| Design system | GOV.UK Frontend + DfE Frontend | See NFR-A1 |

All external integrations sit behind interfaces in the Sources module, with tests against recorded responses.

## Security model (summary)

The requirements are NFR-S1 to S11 in [requirements](requirements.md#security-secure-by-design). The decisions are [ADR-0006](adr/0006-authentication-and-authorisation.md) and [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md).
The threat model lives in `docs/security/threat-model.md` and is updated when a slice adds a new trust boundary.

## Proposed solution layout

```
src/
  ServiceCatalogue.AppHost/                 Aspire orchestration (local)
  ServiceCatalogue.ServiceDefaults/         OpenTelemetry, health checks, resilience
  ServiceCatalogue.Web/                     Razor UI host
  ServiceCatalogue.Worker/                  Background jobs host (a minimal web host, so it can expose health endpoints)
  ServiceCatalogue.SharedKernel/            Result, clock, IDs, environment helpers (IsLocalDevelopment/IsDevEquivalent)
  ServiceCatalogue.Checks/                  Checks engine and repository checks (shared by the CLI and Insights)
  ServiceCatalogue.Analyzers/               Custom Roslyn analysers and code fixes (+ tests)
  Modules/
    Sources/ServiceCatalogue.Sources(.Contracts)
    Catalogue/ServiceCatalogue.Catalogue(.Contracts)
    Insights/ServiceCatalogue.Insights(.Contracts)
    Identity/ServiceCatalogue.Identity(.Contracts)
    Audit/ServiceCatalogue.Audit(.Contracts)
tools/
  ServiceCatalogue.Checks.Cli/              Runs the checks against a local working copy (used in CI on ourselves)
  docs-check.cs                             Docs and ADR consistency checker (.NET 10 single-file app; ADR-0013)
tests/
  *.UnitTests, *.IntegrationTests, ServiceCatalogue.BrowserTests (Playwright),
  ServiceCatalogue.ArchitectureTests, ServiceCatalogue.Analyzers.Tests, perf/ (k6 or NBomber)
docs/
```

The solution will be created from scratch using the current .NET 10 and Aspire templates ([ADR-0003](adr/0003-dotnet-and-aspire.md)).
