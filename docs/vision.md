# Vision and principles

## Problem

DfE's code is spread across multiple GitHub organisations and Azure DevOps (ADO). There is little visibility of:

- what code we own, where it lives and who looks after it
- which technologies, versions and patterns are in use
- how repositories, services and data relate to each other
- where the legacy hotspots and risks are, for example outdated dependencies or no visible CI/CD
- where teams duplicate effort, for example creating the same data, rebuilding the same consumers or solving the same problem 75 different ways

Without this visibility we can't scope what support is needed, let alone deliver it.

## Vision

A single place where DfE can **discover, understand and act** on its software estate:

1. **Census:** a trustworthy inventory of repos and services, built from collected data and added to by people.
2. **Insight:** signals about technology, maturity and risk, mapped to the Service Standard and Technology Code of Practice.
3. **Action:** support targeted where it's needed, exemplars made visible, and, in the long term, self-service onboarding, offboarding and provisioning that gives teams a reason to keep their records up to date.

## Who the first value is for

Leadership in the technical and developer community. First we need to show that collecting this data is
valuable and gives useful insight. That justifies further investment.

## Principles

1. **Discovery and exploration first.** The core purpose is to make the unknown visible. Prefer flexible
   exploration (filter, facet, drill-down) over fixed reports.
2. **Supportive, not punitive.** Findings are *signals that a team may need support*, not compliance failures.
   Language in the UI should reflect this, for example "opportunity" or "signal" rather than "violation".
3. **Collected data first, people's input second.** Automate collection wherever possible. Where people add or
   correct data, record **where each piece of data came from**, so we always know whether a fact was collected
   from a system or entered by a person, when, and by whom.
4. **Reuse before build.** Use existing government and DfE services and components where they exist,
   for example GOV.UK Notify, Find and Use an API, the DfE and GOV.UK design systems, Entra ID, GitHub's
   dependency graph and SBOM export, endoflife.date and OSV. Link to them; don't duplicate them.
5. **Secure by design.** Assume the analysis is sensitive even when the source data is public. Require sign-in
   by default, give the least privilege needed, and design for privacy from the start.
6. **Progressive enhancement.** Everything works without JavaScript. Pages are server-rendered with the DfE
   frontend. JavaScript only enhances.
7. **Open by default.** Code in the open (public repository), following Service Standard point 12. Keep secrets and
   sensitive data out of the repository.
8. **Keep options open.** Make choices that are easy to reverse, and avoid designing ourselves into a corner,
   for example on hosting, scale, identity providers or data sources.
9. **Incremental vertical slices, test-driven.** Every slice is deployable (locally at minimum), tested, and
   delivers something a user can see.
10. **The first-run experience comes first.** Anyone can clone, build and run a realistic, working service in about ten minutes,
    with no secrets to set up (NFR-D1, NFR-D2).
11. **Anything not configured is disabled or faked, never broken**, and the UI always says so. Security-critical integrations fail closed
    instead (NFR-D3, [ADR-0010](adr/0010-unconfigured-integrations-fall-back-safely.md)).
12. **Decisions can be checked, and drift is detected.** Wherever it's reasonable, each decision comes with an automatic check. We apply the checks
    to ourselves first, and then they become the Insights checks run across the DfE estate (NFR-V1 to V5, [ADR-0012](adr/0012-verification-checks-and-drift-detection.md)).
    This applies to documentation too: one home for each fact ([ADR-0013](adr/0013-single-source-of-truth.md)).
