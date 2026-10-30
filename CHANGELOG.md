# Changelog

Notable changes to the DfE Service Catalogue (working name), written for people.
Each entry is one coherent change, as a user would describe it, with every issue and pull request that contributed to it.

Versions are `vYYYY.MM.DD-NNN` (UTC date, with a counter that starts at 001 each day). See [ADR-0015](docs/adr/0015-date-based-versions-and-changelog.md)
and [ways of working: changelog and versions](docs/ways-of-working.md#changelog-and-versions).

## Unreleased

### Added

- The service runs locally with one command (`dotnet aspire run`), with no secrets: Web, Worker and PostgreSQL, a placeholder home page, and health checks that report the database (#11)

### Documentation

- Planning documents: vision and principles, user needs, requirements, architecture, roadmap, unknowns, standards and ways of working (written before the backlog existed; review findings resolved in #2)
- Architecture decision records 0001 to 0015, including first-run fallbacks, verification and drift detection, a single source of truth for docs, credential-less access, and date-based versions (before the backlog existed, so no issue)
- Architecture decision record 0016: one obvious way to do each thing, with Program.cs as a flat list and environment behaviour decided by the extension it calls (#11)
- Ways of working: backlog conventions, recording anything left over as an issue before merging, preferring small pull requests, and a changelog entry for every pull request, grouped into sections (#2, #42, #46)
- Developing: how to remove a container left behind after a resource restart (#41)

### Developer and internal

- Continuous integration: every pull request and push to `main` is built, tested and format-checked, with test results kept and NuGet package versions locked. Both checks must pass before merging, and auto-merge is on (#13, #44)
- Issue forms and a pull request template, prompting for each acceptance criteria section and before and after evidence (#37, #38)
