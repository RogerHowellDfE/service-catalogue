# Contributing

Contributions are welcome. This page explains how we work. The details live in [ways of working](docs/ways-of-working.md);
this page links to them rather than repeating them.

Everyone taking part must follow the [DfE code of conduct](https://github.com/DFE-Digital/.github/blob/master/CODE_OF_CONDUCT.md).

## Before you start

- Read the [vision](docs/vision.md) and [where things live](docs/README.md#where-things-live).
- Pick up or open an issue. Every change traces back to an issue, and every issue traces back to a user need and a roadmap milestone
  ([ways of working: backlog structure](docs/ways-of-working.md#backlog-structure-github)).
- An issue is ready to work on when it meets the [Definition of Ready](docs/ways-of-working.md#definition-of-ready-story).
  Acceptance criteria cover every [section](docs/ways-of-working.md#acceptance-criteria): the main path, unhappy paths,
  unusual but valid input, adversarial cases and failure modes.

## Making a change

- **Test first** ([test-driven development](docs/ways-of-working.md#test-driven-development)). If you write a test afterwards, show it failing without your change.
- **Fake data only**, with obviously fake identities such as *Fake Francis* and *Test Teri* ([test data](docs/ways-of-working.md#test-data)).
- **No secrets in the repository**, and prefer credential-less access ([ADR-0014](docs/adr/0014-credential-less-by-default.md)).
- **Change each fact in its canonical home only** ([ADR-0013](docs/adr/0013-single-source-of-truth.md)). If your change alters a decision, add or supersede an ADR in the same pull request.
- Use [Conventional Commits](https://www.conventionalcommits.org/) for commits and pull request titles ([branching and commits](docs/ways-of-working.md#branching-and-commits)).
- Add a [changelog](CHANGELOG.md) entry for every pull request; small changes can join a general entry ([changelog and versions](docs/ways-of-working.md#changelog-and-versions)).

## Pull requests

- Include **before and after evidence for each acceptance criterion**. Recordings are preferred for journeys ([evidence](docs/ways-of-working.md#evidence)).
- The pull request must meet the [Definition of Done](docs/ways-of-working.md#definition-of-done), including passing CI and security scans.

## Reporting a vulnerability

See [SECURITY.md](SECURITY.md). Please don't open a public issue.
