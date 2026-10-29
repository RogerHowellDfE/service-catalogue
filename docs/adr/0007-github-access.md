# 0007. GitHub access through a token, behind a connector interface

Date: 2026-10-29
Status: Accepted

## Related decisions

- Supersedes / Superseded by: - (expect a later ADR on the GitHub App, M5, which will **amend** this one)
- Amends / Amended by: -
- Depends on / Required by: -
- Complements: [0006](0006-authentication-and-authorisation.md) (kept separate from user sign-in), [0008](0008-snapshots-provenance-and-checks.md) (connectors produce the raw snapshots), [0010](0010-unconfigured-integrations-fall-back-safely.md) (the fake GitHub source is the fallback when there's no token), [0014](0014-credential-less-by-default.md) (the token is a recorded, temporary departure)
- Related: -
- Unknowns, requirements and needs: U8 (rate limits), NFR-S4, NFR-O2, NFR-Q7 (fake source), C2

## Context

The proof of concept collects public repository data from DFE-Digital. A GitHub App is the better long-term choice: fine-grained permissions,
rate limits for each installation, access to private repos. But installing it needs an organisation admin. A token is quick to start with.

## Decision

- Use a **fine-grained personal access token** with read-only access to public repositories for the proof of concept. Locally it's stored in user-secrets.
  It's never committed, logged, or shown in evidence.
- All GitHub access goes through a **connector interface** in the Sources module (for example `IGitHubSource`), with implementations for:
  - the real API (Octokit REST/GraphQL), with rate-limit handling and backing off
  - a **fake source** for local development, CI and demos, which serves the seeded scenarios, including failure scenarios
- **The fake source is the default.** Using the real API is opt-in, through configuration.
- Replacing the token with GitHub App authentication later is a change to configuration and authentication only. The interface stays the same.

## Standards and guidance

- DfE [managing secrets](https://technical-guidance.education.gov.uk/infrastructure/security/managing-secrets/), [storing source code](https://technical-guidance.education.gov.uk/standards/storing-source-code/) (secrets kept separate from code).
- DfE [GitHub: working safely](https://technical-guidance.education.gov.uk/guides/github/working-safely/).
- SbD: least privilege. DfE [coding principles](https://technical-guidance.education.gov.uk/principles/coding-principles/): "Everything fails, all of the time", mock consistently.

## Enforcement

- **No live calls in tests**: the test host replaces the outgoing HTTP handler with one that fails any request to a non-loopback host, so a real GitHub call in CI fails the test.
- **Architecture test**: only the Sources module references Octokit or calls GitHub. Everything else uses `IGitHubSource`.
- **Secret scanning with push protection** on the repository. Tokens are never logged (NFR-V4 analyser on secret-marked properties).

## Consequences

- Fast to start, and it runs with no token at all by default.
- The token's rate limit (5,000 requests an hour) may be tight for collecting from DFE-Digital. This will be measured in M1 (U8), and may bring the GitHub App forward.
- A personal token is tied to one person, which is acceptable for a proof of concept but not for operational use (M5).
- Failure modes (rate limits, 403/5xx errors, timeouts, malformed responses) are tested through the fake source's `source-failures` scenario.

## Alternatives considered

- GitHub App from the start: delayed, because it needs an organisation admin.
- Calling the API without signing in: rejected, because the rate limit (60 requests an hour) is far too low.
