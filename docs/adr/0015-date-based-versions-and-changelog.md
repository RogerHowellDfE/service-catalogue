# 0015. Date-based versions and a human-readable changelog

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: -
- Complements: -
- Related: [0013](0013-single-source-of-truth.md) (the docs checker validates the changelog)
- Unknowns, requirements and needs: NFR-Q3, NFR-S1, NFR-S9, C18, C19, N24, N25

## Context

This is a continuously delivered service, not a library. Semantic versioning's promises about compatibility don't mean much for a website,
but people (leadership, users, support) still need to know *what changed and when*, in their own words, not as a list of commits.
One change that users notice is often delivered through several issues and pull requests.

## Decision

- **Version format**: `vYYYY.MM.DD-NNN`. The date is the **UTC** release date, and `NNN` is a zero-padded counter that starts at `001` each day.
  For example `v2026.10.28-001`, `v2026.10.28-002`, then `v2026.10.29-001`. Zero-padding means the versions sort correctly as plain text. That allows up to 999 releases a day.
- **A release** is a git tag on `main` plus a GitHub Release, whose notes are that version's changelog section. A release script or workflow (M0)
  works out the next version from the existing tags, renames `## Unreleased` to the new version, tags the release and publishes it. If the tag already exists
  (two releases at once), it retries with the next number.
- **.NET versions**: `InformationalVersion` = `2026.10.29-001+<commit sha>`. `AssemblyVersion` and `FileVersion` = `2026.10.29.1` (each part has to be a number no bigger than 65534).
  The version is shown in the page footer and on the health and status endpoints. Container images (later) are tagged with the version.
- **Changelog**: `CHANGELOG.md` at the root of the repository, written for people, grouped by **coherent change as a user would describe it**,
  with every contributing issue and pull request listed against that change. How we write it is in [ways of working](../ways-of-working.md#changelog-and-versions).
- **The changelog is also a page on the site** ("What's new", C19), for people who don't use GitHub. It's generated from `CHANGELOG.md`, so there's only one source:
  - `CHANGELOG.md` is embedded in the Web build, and rendered with Markdig with **raw HTML turned off**. Only `http(s)` and relative links are allowed, and the output is encoded (it's still untrusted content, NFR-S9).
  - The output uses **GOV.UK Frontend styles**, through one shared "GOV.UK Markdown" renderer (a Markdig extension) that other Markdown on the site will use too
    (for example repository descriptions and sidecar text later). It follows the conventions of [x-govuk/govuk-markdown](https://github.com/x-govuk/govuk-markdown):
    - paragraphs: `govuk-body`
    - lists: `govuk-list govuk-list--bullet` and `govuk-list--number`
    - links: `govuk-link`
    - headings: `govuk-heading-*`, moved down a level so the page keeps a single `h1`
    - rules: `govuk-section-break`
    - tables: `govuk-table`, with `govuk-table__head`, `__body`, `__row`, `__header` (`scope="col"`) and `__cell`.
      Right-aligned Markdown columns (`--:`) get the `--numeric` modifiers. Wide tables sit in a focusable, labelled scroll container, so they can still be used at 400% zoom and on small screens
    - code: `app-code`, a small local style

    Nothing appears unstyled, with browser default bullets or margins.
  - `#123` references become links to the issue or pull request on GitHub. Each version heading has an anchor, so the version in the footer links to its own entry.
  - `## Unreleased` is only shown in dev-equivalent environments.
  - The page is anonymous (no sign-in), as it contains nothing that isn't already in the public repository. It's on the reviewed anonymous allow-list (NFR-S1).
  - The footer links to the GitHub repository, `CONTRIBUTING.md` (saying that contributions are welcome) and "What's new", alongside the usual GOV.UK footer links.
- The **read API** (C18), when it exists, is versioned separately (for example `/v1/`), because API consumers do need compatibility promises.

## Standards and guidance

- [x] Service Standard point(s): 8 (iterate and improve frequently: changes are visible), 10 (define success: what changed and when)
- [ ] Technology Code of Practice point(s):
- [ ] Secure by Design principle(s):
- [ ] DfE technical guidance page(s):
- [x] Other: [Keep a Changelog](https://keepachangelog.com/) (written for people, newest first, an Unreleased section). We use dates rather than SemVer

Departures from a recommendation, and why: Keep a Changelog suggests semantic versioning. We use date-based versions because this is a service, not a library (see Context).

## Enforcement

- **Docs checker** ([0013](0013-single-source-of-truth.md)): version headings match the format, are newest first, and the counter goes up within each day. Every entry cites at least one `#ref`.
  Pull requests without the `no-changelog` label must change `## Unreleased`.
- **Tests for the release script**: the first release of a day, the next release the same day, a release just after midnight UTC, and a tag clash (retry).
- **Unit test**: the version the app shows matches the `InformationalVersion` format.
- **Playwright**: "What's new" shows the newest released version heading, matching the app version, and links `#refs` to GitHub. It works without JavaScript, passes axe, and is reachable from the footer while signed out.
- **Unit tests for rendering**: raw HTML, `javascript:` links and the `hostile-data` content in the changelog are neutralised. Every element in a sample covering all the Markdown features gets the expected GOV.UK class, and the heading levels are correct.

## Consequences

- Anyone can read when and what changed without reading git history. The version alone says when it was released.
- Versions carry no compatibility meaning. That's fine for a service, and the API is versioned separately.
- Writing good changelog entries is a small, extra task in each pull request. The pull request template prompts for it.

## Alternatives considered

- **SemVer**: implies compatibility promises that don't apply to a website. Deciding major, minor or patch adds friction with no benefit.
- **CalVer without zero-padding** (for example `2026.10.29.1`): doesn't sort correctly as text (`.10` sorts before `.2`). Only used where .NET requires numeric parts.
- **Auto-generated release notes from pull request titles**: one line per pull request, not grouped by what users notice. They could be used as a draft, but they aren't the changelog.
