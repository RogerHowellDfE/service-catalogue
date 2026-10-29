# Users and needs

These needs are early assumptions. They come from the initial brief and have **not** been validated by user research
(see [unknowns](unknowns.md)). Horizons: **Now** (current focus), **Next**, **Later**.

## Users

| User group | Description | First horizon |
|---|---|---|
| Technical leadership | Heads of profession, technical and principal architects, engineering leads | Now |
| Architects and technical leads | Shape standards, assess services, look for reuse | Now / Next |
| Developers | Want exemplars, people to talk to, reusable components | Next |
| Service owners and delivery managers | Keep records current, manage team access | Next / Later |
| Security and operations | Need to know about vulnerabilities, ownership and exposure | Next |
| Data teams | Need to understand data flows and duplication | Later |
| Platform and support teams | Provisioning, onboarding and offboarding | Later |
| Catalogue administrators | Configure sources, organisations and collection | Now |
| Public / cross-government partners | Possibly, in the future; not currently in scope | Later (maybe) |

## Needs

### Census and discovery (Now)

- **N1** As technical leadership, I need a list of all repositories in the organisations we track, so that I know the size and shape of the code we own.
- **N2** As technical leadership, I need to see which languages and technologies are used, and how much, so that I can spot technologies used only in isolated pockets and plan capability.
- **N3** As technical leadership, I need to see activity signals (last push, archived, size), so that I can tell live code from dormant code.
- **N4** As an architect, I need to filter, search and drill into repositories by attributes, so that I can explore questions I didn't know I had.
- **N5** As a catalogue administrator, I need to manage which GitHub organisations are tracked through the web UI, so that scope can grow without code changes.
- **N6** As a catalogue administrator, I need to trigger and monitor data collection, and see when data was last refreshed, so that I can trust how current it is.

### Risk and maturity signals (Now / Next)

- **N7** As technical leadership, I need to see legacy hotspots, for example outdated or end-of-life runtimes and dependencies, so that I can direct support where the risk is.
- **N8** As technical leadership, I need to see where CI/CD, automated testing, accessibility testing or infrastructure as code is missing, so that I can find teams that need support.
- **N9** As technical leadership, I need to see how signals change over time, so that I can judge whether support is working.
- **N10** As an architect, I need signals mapped to Service Standard and Technology Code of Practice points, so that the findings link to recognised expectations.

### Relationships and enrichment (Next)

- **N11** As an architect, I need to see how repositories relate to each other, for example shared packages, reusable workflows, infrastructure modules and container images, so that I understand coupling and reuse.
- **N12** As a service owner, I need to group repositories into a service or product and add context such as business area, description and phase, so that the catalogue reflects how we actually deliver.
- **N13** As a developer, I need to tag or endorse repositories as exemplars, for example "exemplar mutation testing", so that good practice can be found.
- **N14** As a service owner, I need to declare metadata in a sidecar file in my repository, so that my record stays current without having to visit the catalogue.

### People (Next / Later - subject to a DPIA)

- **N15** As a developer, I need to find who to talk to about a service, so that I can collaborate rather than duplicate work.
- **N16** As a staff member, I need to link my Entra identity to my GitHub identity, so that access and licences can be managed for me.

### Data flows (Later)

- **N17** As a data team member, I need to see what data each service creates and consumes, and how to get access (API, file, other), so that duplication can be reduced and reuse encouraged.
- **N18** As a developer, I need API entries to link to Find and Use an API, so that I'm not looking at a duplicate, out-of-date copy.

### Action and automation (Later)

- **N19** As a service owner, I need onboarding and offboarding of team members to grant and remove access (GitHub teams, Azure, ADO, Teams, SharePoint, licences) from the catalogue, so that I'm not raising lots of support tickets.
- **N20** As a service owner, I need to request new repositories or Azure resources from the catalogue, so that provisioning is consistent and recorded.
- **N21** As technical leadership, I need to form and track support initiatives that target groups of repositories or services, so that analysis turns into action.

### Wider sources (Next / Later)

- **N22** As technical leadership, I need private repositories, more GitHub organisations and ADO repositories included, so that the census is complete.
- **N23** As technical leadership, I need to cross-reference ServiceNow / the CMDB and other records, so that gaps and inconsistencies are visible.

### About the service (Now)

- **N24** As anyone using the service, including people who don't use GitHub, I need to see what has changed and when, in plain language, so that I can trust it's maintained and notice new features.
- **N25** As a developer or interested colleague, I need to find the source code and know that contributions are welcome, so that I can report problems, suggest improvements or contribute.
