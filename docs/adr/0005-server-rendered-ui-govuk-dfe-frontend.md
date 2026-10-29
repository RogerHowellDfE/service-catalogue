# 0005. Server-rendered UI with GOV.UK and DfE frontend

Date: 2026-10-29
Status: Accepted. The choice of DfE Frontend distribution is still **Proposed**, pending the spike (U2). The outcome will be recorded in an ADR that **amends** this one.

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: - (expect an ADR on the DfE Frontend distribution, from U2)
- Depends on: [0003](0003-dotnet-and-aspire.md) (ASP.NET Core)
- Required by: -
- Complements: [0006](0006-authentication-and-authorisation.md) (the Content Security Policy, anti-forgery and sign-in pages use these components)
- Related: -
- Unknowns, requirements and needs: U2, NFR-A1 to A3, NFR-S5

## Context

DfE services should use DfE Frontend, which builds on GOV.UK Frontend, and should use progressive enhancement. There are several packages:

- `GovUk.Frontend.AspNetCore` (x-govuk): mature tag helpers for GOV.UK Design System components. Tracks GOV.UK Frontend 6.6.x.
- `dfe-frontend` (npm, DFE-Digital/dfe-frontend): the established DfE Sass, CSS and components. The DfE design manual links to it.
- `@dfedigital/dfe-frontend` + `DfEDigital.Frontend.AspNetCore` (DFE-Digital/dfefrontend, v3 monorepo): builds on GOV.UK Frontend 6.1.x
  without forking the CSS, with DfE branding switched on by a body class. The NuGet Razor library is currently basic (only a `<dfe-card>` tag helper).

## Decision

- Server-rendered ASP.NET Core (Razor Pages, organised by feature) using **`GovUk.Frontend.AspNetCore`** tag helpers for GOV.UK components.
- DfE branding and DfE-specific components come from DfE Frontend. A **spike** (U2) chooses between npm `dfe-frontend` and v3 `@dfedigital/dfe-frontend` / `DfEDigital.Frontend.AspNetCore`, and checks GOV.UK Frontend version compatibility.
- Every function works without JavaScript. Any JavaScript is an enhancement only, loaded as external files (no inline script, as required by the Content Security Policy).
- All user-facing text lives in views and resources, not scattered through C# code, so content designers can change it and it can be translated.
- Where we build a reusable DfE pattern, consider contributing it back to DfE Frontend.

## Standards and guidance

- DfE [frontend development principles](https://technical-guidance.education.gov.uk/principles/frontend-development-principles/): progressive enhancement, accessibility in CI, keep copy in one place so it's easy to change, beware single-page apps, functional tests covering all journeys.
- DfE design manual, [DfE Frontend](https://design.education.gov.uk/design-system/dfe-frontend), [Digital Product Branding](https://standards.education.gov.uk/standard/digital-product-branding).
- SS 5 (everyone can use the service), SS 13 (common components and patterns). WCAG 2.2 AA.

## Enforcement

- **Playwright**: every page in the smoke list renders the GOV.UK template markup and the DfE Frontend stylesheet. Main journeys pass with JavaScript turned off. axe finds no violations.
- **Integration test**: the Content Security Policy header is present and has no `unsafe-inline` for scripts.
- **Analyser**: bans `Html.Raw` and `HtmlString` built from variables (NFR-V4). A repository check fails on inline `<script>` blocks in `.cshtml` files.
- The DfE Frontend distribution check is added when U2 is decided.

## Consequences

- Accessible, consistent, and quick to build forms with GOV.UK validation patterns.
- Rich data visualisation (charts) must start with an accessible HTML table, and only be enhanced into a chart afterwards.
- Possible version clashes between the GOV.UK Frontend versions the packages expect. The spike will check this.

## Alternatives considered

- Blazor or a single-page app: rejected because they don't fit the progressive enhancement and no-JavaScript requirements, and DfE guidance advises against single-page apps.
