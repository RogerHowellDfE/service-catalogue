# 0009. Run locally until ready for deployment

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: - (expect a later ADR choosing hosting, M4, which will **supersede** this one)
- Amends / Amended by: -
- Depends on: [0003](0003-dotnet-and-aspire.md) (Aspire for running locally)
- Required by: -
- Complements: -
- Related: [0006](0006-authentication-and-authorisation.md) (Entra ID is a precondition for deploying), [0010](0010-unconfigured-integrations-fall-back-safely.md) (running locally with no secrets), [0014](0014-credential-less-by-default.md) (mostly applies once deployed)
- Unknowns, requirements and needs: U1, U3, U4, U17, U18, U21, NFR-S10, NFR-O5, NFR-O6

## Context

This is a personal-development proof of concept, funded from Visual Studio subscription credits. Traffic will be very low.
Internet-facing deployment needs Entra ID (U1), a WAF and DDoS protection, and a security review. Front Door with managed WAF rules is probably
too expensive for a proof of concept (U3). PostgreSQL Flexible Server has a minimum cost, even when idle (U4).
The DfE route to Azure is **Azure CIP** (U17), and DfE recommends Terraform/ARM for infrastructure as code (U18).

## Decision

- **Run locally only** (Aspire AppHost + a PostgreSQL container) until the M4 "ready for deployment" criteria are met: NFR-S10 and acceptable cost.
- Keep the application portable: containerised, configuration through environment variables and Key Vault references, OpenTelemetry for observability.
  No dependence on a specific Azure compute service.
- Container Apps (Web plus Jobs for the Worker) and App Service are both candidates. When we get closer to deploying, write a new ADR to decide,
  comparing costs and including:
  - availability within CIP
  - not exposing the service publicly at all (private ingress, or only allowing known IP ranges)
  - cheaper WAF options
  - Terraform

## Standards and guidance

- DfE [technology stack: hosting](https://technical-guidance.education.gov.uk/guides/default-technology-stack/#hosting-stack) and [Azure CIP](https://technical-guidance.education.gov.uk/infrastructure/hosting/azure-cip/).
- TCoP 5 (cloud first), TCoP 11 (purchasing strategy: value for money). SbD: don't expose the service until controls are in place.
- SS 14 (operate a reliable service): deferred, recorded as a risk.

## Enforcement

- **CI policy check**: fails if the repository contains deployment workflows or infrastructure-as-code files (for example `azure.yaml`, `*.bicep`, `*.tf`, or the `azure/login` action) while this ADR is in force. The ADR that supersedes it (M4) removes this check.

## Consequences

- No cloud cost now. Demos are run locally or recorded.
- The "deploy" part of CI/CD is deferred. CI still builds and tests everything, including container images when we add them.
- Risk that something only shows up when deployed. Running in containers locally reduces this.
