# DfE Service Catalogue - planning and design documentation

> Working name. A better name may emerge.

| Document | Purpose |
|---|---|
| [Vision and principles](vision.md) | Why this exists, what success looks like, guiding principles |
| [Users and needs](user-needs.md) | Who the users are and what they need, tagged by horizon |
| [Requirements](requirements.md) | Capability map (functional) and non-functional / standards requirements |
| [Architecture](architecture.md) | Modular monolith design, modules, data pipeline, security model |
| [Roadmap](roadmap.md) | Now / Next / Later, outcome-based milestones |
| [Unknowns, risks and dependencies](unknowns.md) | Open questions, with "needed by" horizon |
| [Ways of working](ways-of-working.md) | Kanban flow, acceptance criteria, test data, evidence, TDD, security scanning, AI-assisted delivery |
| [Standards and guidance](standards.md) | Service Standard, TCoP, Secure by Design, DfE technical guidance: what we trace back to |
| [Architecture decision records](adr/README.md) | Decisions and their rationale |

## Where things live

Each kind of fact has **one canonical home**. Everywhere else links to it by ID or anchor, with at most a one-line summary.
Repetition that can't be avoided is **generated**. See [ADR-0013](adr/0013-single-source-of-truth.md).

| Kind of fact | Canonical home | Referenced elsewhere as |
|---|---|---|
| Why we exist; principles | [vision.md](vision.md) | "principle 11" |
| Who needs what | [user-needs.md](user-needs.md) | `N7` |
| What must be true (testable) | [requirements.md](requirements.md) | `C4`, `NFR-D3` |
| A decision, why, and how it's enforced | [adr/](adr/README.md) | `ADR-0010` |
| The current design: modules, data pipeline, environments and integration modes, solution layout | [architecture.md](architecture.md) | a section link |
| What we'll deliver, in which order | [roadmap.md](roadmap.md) | `M1` (lists IDs; doesn't restate requirements) |
| How we work: acceptance criteria, DoR and DoD, test data, evidence, scanning | [ways-of-working.md](ways-of-working.md) | a section link |
| External standards and DfE guidance | [standards.md](standards.md) | `SS 9`, `TCoP 6`, `DfE-TG: storing source code` |
| Open questions and resolved ones | [unknowns.md](unknowns.md) | `U24`, `R6` |
| Threats and mitigations | `security/threat-model.md` (created in M0) | a section link |
| Retired terms and what replaces them | `terms.yml` (created with the docs checker in M0) | checked automatically |
| Facts about the code (versions, environments, integrations, secrets, analysers, checks) | **The code** | Generated sections, or the Integrations status page |
| Delivery slices | GitHub issues | `#123` |

Templates (issue forms, the pull request template) and agent instructions can repeat rules, but must cite the canonical ID for each.
A docs checker in CI enforces all this ([ADR-0013: Enforcement](adr/0013-single-source-of-truth.md#enforcement)).

## How these documents relate to the backlog

These documents describe the **broad, up-front technical intent** (requirements, architecture, standards).
The GitHub Project board holds the **delivery slices** that move us towards that intent. Slices are refined
"just in time", but always trace back to a need in `user-needs.md` and a milestone in `roadmap.md`.

When a delivery slice changes our understanding, update the relevant document (or add/supersede an ADR)
in the same pull request.
