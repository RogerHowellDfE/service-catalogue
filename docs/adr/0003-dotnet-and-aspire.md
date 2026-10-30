# 0003. .NET 10 LTS and Aspire

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on: -
- Required by: [0005](0005-server-rendered-ui-govuk-dfe-frontend.md) (ASP.NET Core UI), [0009](0009-local-first-hosting.md) (Aspire for running locally), [0010](0010-unconfigured-integrations-fall-back-safely.md) (Aspire orchestrates the first run)
- Complements: -
- Related: -
- Unknowns, requirements and needs: U18 (infrastructure as code), U21 (telemetry backend), NFR-O3, NFR-O5

## Context

The technology is expected to be C#. .NET 10 is the current LTS (supported until November 2028). .NET 8 and 9 reach end of support
in November 2026. Aspire (13.x) gives local orchestration (Web, Worker, PostgreSQL) and service defaults (OpenTelemetry, health checks, resilience).

## Decision

- Target **.NET 10** (pinned with `global.json`) and the current **Aspire 13.x**.
- Create the solution from the current templates, using the `ServiceCatalogue.*` naming and the module layout in the [architecture](../architecture.md#proposed-solution-layout).
- No separate API service (see alternatives). A read API (C18) can be added later.
- Use Central Package Management (`Directory.Packages.props`) and shared `Directory.Build.props` (nullable enabled, warnings as errors where practical, analysers).
- Tests use **xUnit v3** on Microsoft.Testing.Platform.
- Run locally with the **Aspire CLI**, pinned as a local tool in `dotnet-tools.json` (commands: [getting started](../../README.md#getting-started)).
- Aspire is for **local orchestration only** for now. The deployment tooling and infrastructure as code are decided separately (U18: DfE recommends Terraform/ARM).

## Standards and guidance

- DfE [technology stack](https://technical-guidance.education.gov.uk/guides/default-technology-stack/): "C#.NET is our preferred tech stack" ✔.
- TCoP 3 (open source), TCoP 5 (cloud first: containers and OpenTelemetry keep hosting portable).
- SS 11 (choose the right tools and technology).

## Enforcement

- `global.json` pins the .NET 10 SDK (with `rollForward: latestFeature`). `Directory.Build.props` sets `net10.0` for every project.
- **Central Package Management** (`Directory.Packages.props`), so versions are declared in one place. Dependabot keeps Aspire and .NET packages up to date.
- **Fresh-clone CI job** (NFR-D1) proves the solution builds and runs from a clean checkout.

## Consequences

- We start on supported versions. Aspire only supports its latest release, so we must keep up with Aspire updates (Dependabot).
- OpenTelemetry can send data to App Insights or other DfE-supported backends later (U21).

## Alternatives considered

- Keep the default Aspire template's separate API service: rejected because there's no need for an HTTP API between our own components inside the monolith.
- Not using Aspire: rejected because it would mean losing simple local orchestration and the telemetry defaults.
- NUnit or MSTest: both capable. xUnit v3 is what the Aspire, Testcontainers and Playwright examples use, and it runs natively on Microsoft.Testing.Platform.
- `dotnet run --project src/ServiceCatalogue.AppHost` as the run command: works, but the Aspire CLI finds the AppHost, trusts the development
  certificate and prints the dashboard link. Pinning it as a local tool means everyone gets the same version without a global install.
