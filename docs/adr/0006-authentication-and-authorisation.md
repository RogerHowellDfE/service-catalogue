# 0006. Entra ID authentication, role-based authorisation, sign-in required by default

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: Required by [0011](0011-first-run-bootstrap.md) (the first admin is assigned within these roles)
- Complements: [0010](0010-unconfigured-integrations-fall-back-safely.md) (the persona selector is the fallback when sign-in isn't configured), [0005](0005-server-rendered-ui-govuk-dfe-frontend.md) (UI security: Content Security Policy, anti-forgery), [0007](0007-github-access.md) (GitHub credentials are separate from user sign-in; GitHub OAuth only for account linking later), [0014](0014-credential-less-by-default.md) (the app registration uses a federated credential)
- Related: [0009](0009-local-first-hosting.md) (Entra ID must be in place before deploying)
- Unknowns, requirements and needs: U1, NFR-S1 to S3, NFR-D4, N16, C13

## Context

Some data must not be public: insights about security posture, and personal data later. Staff are in Entra ID. Later we need to link
staff identities to GitHub identities, for example for organisation membership and Copilot licences. The DfE Entra tenant may not be available yet (U1),
and the service runs locally for now.

## Decision

- **Sign in with Entra ID** (OpenID Connect, Microsoft.Identity.Web). The tenant and client details are configuration, so a personal
  development tenant can be used now and the DfE tenant later.
- **Sign-in required by default**: an authorisation fallback policy means every endpoint requires a signed-in user unless it explicitly opts out.
- **Roles** as Entra app roles: `Catalogue.Reader`, `Catalogue.Contributor`, `Catalogue.Admin`. Each feature uses named policies,
  not role checks scattered through the code.
- **Fake persona selector when sign-in isn't configured**: only when `IsLocalDevelopment()` is true. In every other environment, missing sign-in configuration
  stops the app from starting (fail closed). The detail of environments and fallbacks is in [ADR-0010](0010-unconfigured-integrations-fall-back-safely.md).
  Automated tests use a test authentication handler that is registered only by the test host (WebApplicationFactory), never by the app.
- **GitHub OAuth is for account linking only (later)**, never for signing in to the catalogue.

## Standards and guidance

- SbD principles: "secure by default", least privilege. SS 9 (secure, protects privacy). TCoP 6 (make things secure), TCoP 8 (share, reuse and collaborate: use the existing identity provider).
- DfE [managing secrets](https://technical-guidance.education.gov.uk/infrastructure/security/managing-secrets/): use managed identities where possible when deployed.
- OWASP ASVS V2 to V4 (authentication, session management, access control).

## Enforcement

- **Endpoint enumeration test**: lists every endpoint from `EndpointDataSource`, and fails if any endpoint allows anonymous access without being on a reviewed allow-list.
- **Analyser**: every Razor page or endpoint has an explicit policy, or `[AllowAnonymous]` with a reason (NFR-V4).
- **Startup guard and tests**: the persona selector can't be registered unless `IsLocalDevelopment()` is true. Tests start the host in every other environment with sign-in unconfigured, and expect it to fail.
- **Analyser**: bans `IsDevelopment()` (NFR-D4).
- **Integration tests**: each role (and anonymous) against each policy-protected route, including forced browsing to admin routes.

## Consequences

- Local development and CI don't need Entra ID. Deploying does (M4).
- Field classification and output filtering (NFR-S3) are built on these policies.
- Failure mode: the persona selector is a serious risk if it's misconfigured. It's covered by the startup guard, the tests above and the threat model.
- Failure mode: if Entra ID is unavailable, users can't sign in. Show a helpful error page, and never fall back to letting people in without signing in.

## Alternatives considered

- DfE Sign-in: aimed at external and education-sector users, not staff.
- GitHub as the main sign-in: doesn't represent staff identity or joiners and leavers.
