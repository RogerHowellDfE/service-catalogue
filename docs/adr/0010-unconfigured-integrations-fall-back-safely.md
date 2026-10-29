# 0010. Unconfigured integrations fall back safely: faked, recorded or disabled

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: Depends on [0003](0003-dotnet-and-aspire.md) (Aspire orchestrates the first run, and holds parameters and secrets)
- Complements: [0006](0006-authentication-and-authorisation.md) (the persona selector is the sign-in fallback), [0007](0007-github-access.md) (the fake GitHub source), [0011](0011-first-run-bootstrap.md) (first run builds on a working, unconfigured app), [0014](0014-credential-less-by-default.md) (secrets that are still needed are declared)
- Related: [0009](0009-local-first-hosting.md) (running locally with no secrets), [0013](0013-single-source-of-truth.md) (README sections generated from declarations)
- Unknowns, requirements and needs: NFR-D1, NFR-D3, NFR-D4, NFR-D6, NFR-D7, NFR-Q7, R6, U24

## Context

The first-run experience comes first (vision principle 10). A new contributor, or an AI agent, must get a working service from
`git clone` -> `dotnet build` -> `dotnet run` without asking anyone for secrets. Every integration we add (GitHub, Entra ID, GOV.UK Notify,
endoflife.date, ADO...) is a new way for that to break. Fakes also bring a risk of their own: demo data being mistaken for real data, or a
security-relevant fake (sign-in) being switched on where it shouldn't be.

## Decision

- Every external integration is registered through **one pattern**. At startup its options are validated and it gets one mode:
  **Configured**, **Fake/Recording** or **Disabled**. Missing configuration never causes an obscure failure later on.
- What each integration falls back to, and the environment rules, are kept in one place:
  [architecture: environments, configuration and fallbacks](../architecture.md#environments-configuration-and-fallbacks).
- **Security-critical integrations fail closed.** Missing sign-in configuration only falls back to the persona selector when `IsLocalDevelopment()` is true.
  Everywhere else the app refuses to start.
- **Fakes are always visible**: a site-wide banner whenever any fake is in use, an admin **Integrations status** page, and health checks reporting *degraded*.
- **Recorders instead of silence**: outbound side effects (for example GOV.UK Notify) are recorded and shown on an admin page when not configured.
- **Required secrets are declared in code** (NFR-D6), together with how to opt in to the real integration: copy-paste commands with placeholders,
  where to get the value, its minimum permissions, and how to check it worked (NFR-D7). The README section and the Integrations status page are
  **generated from those declarations**, so they can't drift. Configuration is set through Aspire parameters and connection strings on the AppHost.
- The environment is only checked through `IsLocalDevelopment()` and `IsDevEquivalent()`. ASP.NET Core defaults that only switch on in `Development`
  (user-secrets, the developer exception page) are switched on explicitly for `LocalDevelopment`.

## Standards and guidance

- [x] Service Standard point(s): 9 (secure), 14 (reliable: degrade gracefully, with the problem visible)
- [x] Technology Code of Practice point(s): 6 (make things secure), 8 (share, reuse and collaborate: easy for others to run)
- [x] Secure by Design principle(s): secure by default; fail closed
- [x] DfE technical guidance page(s): [managing secrets](https://technical-guidance.education.gov.uk/infrastructure/security/managing-secrets/), [development principles](https://technical-guidance.education.gov.uk/principles/coding-principles/) ("everything fails, all of the time")
- [ ] Other:

Departures from a recommendation, and why: none.

## Enforcement

- **Startup tests**: for each environment x each integration (configured or not), start the host and check the mode it gets. Expect a failure for sign-in that isn't configured outside `LocalDevelopment`.
- **Analyser**: every integration options class declares what happens when it isn't configured, and nothing reads configuration outside options classes (NFR-V4).
- **Analyser**: bans `IsDevelopment()`.
- **Test**: user-secrets and the developer exception page are on in `LocalDevelopment` and off everywhere else.
- **Playwright**: the banner is shown whenever any fake is active, and the Integrations status page lists every registered integration.
- **CI**: the README's required-secrets and opt-in section is regenerated from the declarations and compared, and the build fails if it differs.
- **Test**: every registered integration has an opt-in declaration with at least one command, a "where to get it" link and its minimum permissions.
- **Fresh-clone CI job**: runs with no configuration at all, and must reach a working, seeded home page.

## Consequences

- Each new integration costs a little more: a fake or recorder, plus tests for each mode. This is paid back every time someone runs the app or the CI suite.
- Fakes need maintenance as the real APIs change. Contract tests against recorded responses (see [test data exceptions](../ways-of-working.md#exceptions)) catch drift.
- Failure mode: a fake left on in a deployed environment. This is mitigated by the banner, *degraded* health and the Integrations status page, and for sign-in it isn't possible at all.

## Alternatives considered

- Require developers to configure real credentials: fails the ten-minute start and pushes people towards sharing secrets.
- Silently disable missing integrations: leads to confusing behaviour and demo data being mistaken for real data.
- Use `Development` for local machines: clashes with the deployed development environment, where the persona selector must never be enabled.
