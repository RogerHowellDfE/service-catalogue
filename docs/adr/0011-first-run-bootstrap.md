# 0011. First-run bootstrap: the first user becomes an administrator

Date: 2026-10-29
Status: Accepted (the protections outside `LocalDevelopment` are still open under U25; an ADR that amends this one will settle them)

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: Depends on [0006](0006-authentication-and-authorisation.md) (the roles being assigned)
- Complements: [0010](0010-unconfigured-integrations-fall-back-safely.md) (a working app before anything is configured)
- Related: -
- Unknowns, requirements and needs: U25, U27, NFR-D1, NFR-D5, NFR-S6

## Context

A new installation has no administrators, so nobody can configure tracked organisations, seed data or grant roles. Requiring a manual database
step goes against the ten-minute start. Automatically promoting the first user creates a window in which whoever arrives first gets the highest privilege.
The user accepts a limited exposure window, but it must be small, visible and audited.

## Decision

- When **no administrator exists**, the first signed-in user is offered the chance to become an administrator. Promoting them is a
  single atomic operation: a database constraint or transaction makes sure that at most one "first admin" can ever succeed, even when two people try at once.
- The promotion is written to the audit log (who, when, which environment).
- Seeding is separate from bootstrapping. In dev-equivalent environments (U25), the AppHost's seeding-scenario parameter (default `small-estate`) seeds an empty
  database at startup, including the role assignments that scenario's fake personas need. The `empty` scenario seeds nothing, so the bootstrap journey above applies.
  An administrator can re-seed a different scenario later. See [test data](../ways-of-working.md#seeding-scenarios).
  How role assignments are stored and where they come from is open (U27). Whatever is decided must allow the application to record at least the first administrator.
- **In `LocalDevelopment`**: no extra protections.
- **Elsewhere (proposed, U25)**: the user must also enter a **one-time setup token** that is written to the startup logs (or Key Vault). It expires
  after first use or after a time limit. Once an administrator exists, the bootstrap route returns 404.

## Standards and guidance

- [x] Service Standard point(s): 9 (secure)
- [ ] Technology Code of Practice point(s):
- [x] Secure by Design principle(s): least privilege, secure by default (the exposure window is minimised and audited)
- [ ] DfE technical guidance page(s):
- [x] Other (WCAG, ASVS, NCSC...): ASVS V4 (access control), V7 (logging)

Departures from a recommendation, and why: automatically granting the highest privilege is unusual. It's limited to an empty installation, protected by a one-time token outside local development, and audited.

## Enforcement

- **Integration test**: two concurrent bootstrap attempts, and exactly one succeeds.
- **Integration test**: once an administrator exists, the bootstrap route returns 404 for every role and for anonymous users.
- **Integration test**: outside `LocalDevelopment`, bootstrapping without a valid one-time token is refused and audited.
- **Integration test**: seeding is refused in environments that aren't dev-equivalent.
- **ZAP** includes the bootstrap route in its scan of an empty installation.

## Consequences

- The first run needs no manual database steps.
- Failure mode: someone unexpected claims a new deployed environment first. Mitigated by the setup token, the audit entry and an admin page showing who was promoted, and when.

## Alternatives considered

- Configure the first administrator through configuration (an object ID or email address): safe, but it adds a required setting, against NFR-D1. Could be offered as an alternative later.
- Grant administrator rights through Entra app roles only: correct for deployed environments once U1 is resolved, but no help with the persona selector.
