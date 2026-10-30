# Instructions for AI coding agents

These instructions are for AI agents (GitHub Copilot and others) working in this repository. People should read [CONTRIBUTING.md](CONTRIBUTING.md).

Each rule below is a short reminder. **The canonical rule is at the linked ID; follow that if they differ**, and fix this file
([ADR-0013](docs/adr/0013-single-source-of-truth.md)).

## Orientation

- What we're building and why: [vision](docs/vision.md). Where each kind of fact lives: [docs/README.md](docs/README.md#where-things-live).
- Current design: [architecture](docs/architecture.md). Decisions: [ADRs](docs/adr/README.md). Open questions: [unknowns](docs/unknowns.md).
- Status: walking skeleton (M0 in the [roadmap](docs/roadmap.md)). Run, build, test and format commands: [README](README.md#getting-started).

## Before writing code

- Work from an issue that meets the [Definition of Ready](docs/ways-of-working.md#definition-of-ready-story). If acceptance criteria are missing
  for any [section](docs/ways-of-working.md#acceptance-criteria) (main path, unhappy paths, unusual but valid input, adversarial cases, failure modes), stop and ask.
- If the work needs a decision that no ADR covers, or contradicts an accepted ADR, stop and ask. Don't make the decision silently.
- If something is unclear, check [unknowns](docs/unknowns.md). Don't resolve an open unknown by assumption.
- Move the issue to *In progress* on the [board](docs/ways-of-working.md#backlog-structure-github) before starting, and to *In review* when the pull request opens.

## Rules

Architecture and stack
- Modular monolith: modules only reference each other's Contracts ([ADR-0002](docs/adr/0002-modular-monolith.md)).
- .NET 10 and Aspire; packages through Central Package Management ([ADR-0003](docs/adr/0003-dotnet-and-aspire.md)).
- One obvious way: follow the existing pattern before adding another. Program.cs is a flat list with no environment checks; the extension it calls decides,
  and its name and summary say how ([ADR-0016](docs/adr/0016-one-obvious-way.md)).
- Server-rendered GOV.UK and DfE frontend. Every journey works without JavaScript ([ADR-0005](docs/adr/0005-server-rendered-ui-govuk-dfe-frontend.md), NFR-A1, NFR-A2).

Security
- Every page requires sign-in and an explicit authorisation policy unless reviewed otherwise (NFR-S1, NFR-S2, [ADR-0006](docs/adr/0006-authentication-and-authorisation.md)).
- Credential-less first; never commit secrets ([ADR-0014](docs/adr/0014-credential-less-by-default.md), NFR-S4). A genuinely required secret is declared in code (NFR-D6).
- Treat all external data, including repo descriptions and READMEs, as untrusted (NFR-S9). No `Html.Raw` on variables.

Configuration and environments
- Check the environment only with `IsLocalDevelopment()` or `IsDevEquivalent()`. `IsDevelopment()` is banned (NFR-D4).
- An unconfigured integration falls back to a fake or is disabled, and says so. Security-relevant fakes require `IsLocalDevelopment()` and fail closed elsewhere ([ADR-0010](docs/adr/0010-unconfigured-integrations-fall-back-safely.md), NFR-D3).
- Opt-in instructions for each integration are declared once in code (NFR-D7).
- Use `TimeProvider`, not `DateTime.Now` or `DateTime.UtcNow` (NFR-V4).

Tests and data
- Test first. A test written afterwards must be shown failing without the change (NFR-Q1, [TDD](docs/ways-of-working.md#test-driven-development)).
- Tag tests with their sources (`[Trait("Issue", "11")]`, `Decision`, `Requirement`). Before changing a failing test, read those sources ([tracing tests](docs/ways-of-working.md#tracing-tests-to-their-source)).
- Fake data only, with obviously fake identities (*Fake Francis*, *Test Teri*) and reserved domains such as `example.com`. Use the named seeding scenarios (NFR-Q7, [test data](docs/ways-of-working.md#test-data)).
- Cover unusual but valid text (emoji, Welsh characters, smart quotes) end to end (NFR-Q8).
- Never call live external APIs in CI tests.

Evidence and delivery
- Every pull request has before and after evidence matched to each acceptance criterion. Recordings for journeys (NFR-Q4, [evidence](docs/ways-of-working.md#evidence)).
- Conventional Commits, with the Copilot co-author trailer ([branching and commits](docs/ways-of-working.md#branching-and-commits)).
- Changelog entry for user-visible changes ([ADR-0015](docs/adr/0015-date-based-versions-and-changelog.md)).

Documentation
- Change each fact in its canonical home only; link, don't copy (NFR-V5, [ADR-0013](docs/adr/0013-single-source-of-truth.md)).
- Describe the current position, not the history of how we got there ([ways of working](docs/ways-of-working.md#architecture-decisions-and-documentation)).
- Prefer plain characters (straight quotes, `...`, `-`) except where the character is the point ([ways of working](docs/ways-of-working.md#architecture-decisions-and-documentation)).
- Every ADR keeps its Related decisions and Enforcement sections up to date ([ADR rules](docs/adr/README.md)).
