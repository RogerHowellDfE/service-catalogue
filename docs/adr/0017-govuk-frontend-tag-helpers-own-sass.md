# 0017. GOV.UK Frontend through the tag helpers, compiled with our own Sass; DfE Frontend v3 added when needed

Date: 2026-11-02
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: Amends [0005](0005-server-rendered-ui-govuk-dfe-frontend.md) (chooses the DfE Frontend distribution and how frontend assets are built)
- Depends on / Required by: Depends on [0003](0003-dotnet-and-aspire.md) (packages through Central Package Management)
- Complements: -
- Related: [0016](0016-one-obvious-way.md) (one entry point for styles and one way to add scripts)
- Unknowns, requirements and needs: R7, R8 (were U2, U24), NFR-A1, NFR-A2, NFR-D1, NFR-D2, NFR-S5, NFR-Q8

## Context

[ADR-0005](0005-server-rendered-ui-govuk-dfe-frontend.md) chose server-rendered Razor with `GovUk.Frontend.AspNetCore` tag helpers, and left open which
DfE Frontend distribution to use and whether Node is needed (U2, U24). Spike #19 compared seven throwaway variants side by side: each rendered every
GOV.UK Frontend component fixture, the tag-helper documentation examples and the DfE Frontend v3 examples, under a strict Content Security Policy
(no inline script or style), with JavaScript on and off, checked by a browser test and by pixel comparison of screenshots.

What the spike found:

- **Three projects release independently.** GOV.UK Frontend, the x-govuk tag helpers and DfE Frontend have no shared compatibility matrix,
  so each service has to work out which versions go together. Each tag-helper release embeds one GOV.UK Frontend version
  (`GovUk.Frontend.AspNetCore` 5.1.0 embeds GOV.UK Frontend 6.6.0) and ships that version's Sass.
- **DfE Frontend 2.x** (npm `dfe-frontend`) is a standalone framework that depends on GOV.UK Frontend 5. It can't be used with GOV.UK Frontend 6.
- **DfE Frontend v3** (`DFE-Digital/dfefrontend`) builds on GOV.UK Frontend 6 and adds DfE components (card, side and sub-navigation, modal, toggle).
  - Its NuGet package `DfEDigital.Frontend.AspNetCore` ships tag helpers and prebuilt CSS and JavaScript, but no Sass.
    The prebuilt CSS (3.0.3) was built against GOV.UK Frontend 6.1.0, not the 6.6.0 the tag helpers embed, and applies the DfE font with `!important` overrides.
  - Its Sass doesn't compile against GOV.UK Frontend 6.6.0 (a mixin moved), although it declares support for `^6.1.0`. A one-line patch fixes it.
  - The npm package lags the NuGet package (3.0.0, without the sub-navigation and toggle Sass the 3.0.3 tag helpers render), and the repository has no release tags.
  - Adding v3 CSS didn't change how any GOV.UK component rendered. v3 Sass compiled by us matched the prebuilt NuGet CSS to within a few pixels.
- **Compiling GOV.UK Frontend's Sass ourselves needs no Node.** The tag-helper package restores its Sass into the project, and the
  `AspNetCore.SassCompiler` NuGet package (MIT, bundles Dart Sass) compiles it during `dotnet build`. The npm and Node route produced byte-identical CSS
    to the same Sass compiled by `AspNetCore.SassCompiler`, with an extra prerequisite. Build times were similar across all variants (a cold build of 6 to 13 seconds).
- **Sass settings are the only clean way to change GOV.UK Frontend's typography**: setting `$govuk-font-family` and turning off the GDS Transport
  `@font-face` changes every component, where prebuilt CSS has to be overridden selector by selector.
- **The tag helpers do more than produce markup.** With model binding they generate field error messages, an error summary at the top of `<main>`
  (only when there are errors), an `Error: ` prefix on the page title and `novalidate` on forms. The date input binder turns three fields into a
  `DateOnly`, accepts month names, and produces specific messages such as "must be a real date" and "must include a month". All of this was checked
  with JavaScript on and off, and Welsh text, emoji and smart quotes round-tripped intact (NFR-Q8). When GOV.UK Frontend changes its markup,
  upgrading the package updates every page; hand-written markup would have to be found and changed by hand.
- Every remaining console error, Content Security Policy violation and missing file came from example content, not from the libraries.
  The friction found in DfE Frontend is logged for upstream contributions (see Consequences).
- **Licences and maintenance.** `GovUk.Frontend.AspNetCore` (x-govuk) and `AspNetCore.SassCompiler` are MIT licensed;
  each tag-helper release names the GOV.UK Frontend version it embeds. DfE Frontend v3 declares MIT in its npm `package.json`, but the repository
  has no licence file and the NuGet package has no licence or repository metadata. DfE Frontend 2.x was last published in May 2024.

## Decision

- **GOV.UK components through the tag helpers.** Use `GovUk.Frontend.AspNetCore` for every GOV.UK component it supports, bound to the model with `for`
  wherever there's a model. Hand-written GOV.UK markup only where no tag helper exists.
- **One GOV.UK Frontend version, from one place: the tag-helper package.** We don't reference `govuk-frontend` separately (npm, CDN or vendored copy).
  Upgrading GOV.UK Frontend means upgrading `GovUk.Frontend.AspNetCore`; we accept that this trails GOV.UK Frontend releases slightly.
- **We compile the styles ourselves, with no Node.** The tag-helper package restores GOV.UK Frontend's Sass and its support package
  (`RestoreGovUkFrontendNpmPackage`, `RestoreGovUkFrontendSupportPackage`), and `AspNetCore.SassCompiler` compiles a single entry point,
  `app.scss`, as part of `dotnet build` (NFR-D2). `app.scss` is the one place for:
  - GOV.UK Frontend settings, including the DfE font: Inter, self-hosted, with the GDS Transport `@font-face` turned off,
  - asset URL functions from the support package, so font and image URLs match what the tag helpers serve,
  - our own Sass modules, which use GOV.UK Frontend's settings, mixins and functions,
  - DfE Frontend v3 Sass, when it's added.
- **Our own JavaScript is plain ES modules**, served as static files (fingerprinted and compressed by `MapStaticAssets`) and loaded as external
  scripts with the request's nonce. No bundler, transpiler or Node. Revisit only if a concrete need appears that native modules can't meet.
- **DfE branding without DfE Frontend, for now**: the GOV.UK header and footer with the DfE logo and the Inter font, following the DfE design manual's
  rebrand guidance. Neither DfE Frontend package is referenced yet. DfE Frontend 2.x is not used at all.
- **DfE Frontend v3 is added when a story needs one of its components.** Then we compile its Sass from source in `app.scss` (not its prebuilt CSS)
  and use its tag helpers, pinned to a specific version or commit. Before it's added, its Sass must compile against the GOV.UK Frontend version
  the tag helpers embed. If it doesn't, fix it upstream first; a local patch is allowed only with a linked upstream issue, and is removed when the fix ships.
- **Upstream first.** Problems found in DfE Frontend, its guidance or the tag helpers are reported or fixed upstream, rather than worked around silently.

## Standards and guidance

- [x] Service Standard point(s): SS 5 (everyone can use the service: works without JavaScript, accessible components), SS 13 (common components and patterns)
- [ ] Technology Code of Practice point(s):
- [x] Secure by Design principle(s): fewer build-time dependencies and no Node toolchain; all assets self-hosted under a strict Content Security Policy (NFR-S5)
- [x] DfE technical guidance page(s) (including the DfE technology stack): [frontend development principles](https://technical-guidance.education.gov.uk/principles/frontend-development-principles/) (progressive enhancement)
- [x] Other (WCAG, ASVS, NCSC...): [GOV.UK Design System](https://design-system.service.gov.uk/), [DfE design manual](https://design.education.gov.uk/) rebrand and typography guidance
Departures from a recommendation, and why: the DfE design manual points to DfE Frontend; we don't use it until we need a DfE component, because no
release works cleanly with the current GOV.UK Frontend without a patch, and DfE branding (logo and font) needs nothing from it.

## Enforcement

- **Build**: `app.scss` compiles as part of `dotnet build`, so a Sass error fails the build and CI.
- **Version check (test)**: a test fails if the GOV.UK Frontend version in the compiled CSS (`--govuk-frontend-version`) differs from the version the
  tag helpers embed. This replaces ADR-0005's "DfE Frontend distribution check".
- **Browser tests**: a component showcase page renders the GOV.UK Frontend fixtures (and DfE components, once added). A browser test fails on any
  console error, Content Security Policy violation, failed request or uninitialised component, with JavaScript on and off.
  ADR-0005's check that pages render "the DfE Frontend stylesheet" checks for the compiled `app.css` until DfE Frontend v3 is added.
- **Review only**: hand-written GOV.UK markup where a tag helper exists (reviewers look for `govuk-` classes on elements a tag helper could render),
  and new frontend tooling (`package.json`, bundlers), which needs a new ADR.

## Consequences

- GOV.UK markup changes arrive through package upgrades, without searching the codebase. Forms get GOV.UK error handling and date parsing for free.
- One `dotnet build` produces everything; there's no Node prerequisite (NFR-D1) and no npm ecosystem for Dependabot to watch.
- We control the Sass, so we can add our own modules, change settings, and leave out components we don't use.
- GOV.UK Frontend upgrades wait for a tag-helper release.
- No DfE components (card, side navigation, modal, toggle) until v3 is added. DfE icons, favicons and footer details are hand-built
  until DfE publishes them.
- Adding DfE Frontend v3 is a planned step, with an upfront compatibility check; it may need an upstream fix first.
- Follow-ups: the component showcase and browser check in the real app, the version check, and upstream contributions to DfE Frontend
  (licence and package metadata, GOV.UK Frontend 6.6 compatibility, release tags, Sass in the NuGet package, examples that work under a strict
  Content Security Policy, a compatibility matrix and a .NET install guide).

## Alternatives considered

- **Tag helpers with their prebuilt CSS**: simplest, but no Sass settings, so the font can only be changed by overriding every component.
- **DfE Frontend v3 NuGet prebuilt CSS on top of the tag helpers**: works, but it was built against an older GOV.UK Frontend (6.1.0) than the tag
  helpers embed, can't be rebuilt against ours, and applies branding with `!important` overrides.
- **DfE Frontend v3 Sass now**: works with a local patch, but we need none of its components yet, and we'd carry the patch until it's fixed upstream.
- **No tag helpers, our own Sass from a vendored GOV.UK Frontend**: the same build step, but every component's markup written and migrated by hand,
  and no model-bound errors or date binding.
- **npm and Node to compile Sass**: identical output, with an extra prerequisite and toolchain.
- **DfE Frontend 2.x (npm `dfe-frontend`)**: depends on GOV.UK Frontend 5, so it's a dead end.
