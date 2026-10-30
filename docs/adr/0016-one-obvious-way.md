# 0016. One obvious way: few patterns, explicit and discoverable

Date: 2026-10-30
Status: Accepted

## Related decisions

- Supersedes / Superseded by: -
- Amends / Amended by: -
- Depends on / Required by: -
- Complements: [0012](0012-verification-checks-and-drift-detection.md) (conventions come with checks), [0013](0013-single-source-of-truth.md) (one home for each fact; this is one way for each job)
- Related: [0002](0002-modular-monolith.md) (module registration), [0010](0010-unconfigured-integrations-fall-back-safely.md) (each integration decides its own mode)
- Unknowns, requirements and needs: NFR-D4, NFR-V4

## Context

Many people and AI agents, using models of different strengths, will add to this codebase. Each extra way of doing the same job is something every
reader has to learn and every contributor has to choose between. Encapsulation helps only while a name can be trusted without opening it;
past that point it becomes indirection, and readers have to jump through definitions to learn what actually happens.

## Decision

- **Converge on one way.** Before adding a pattern, look for the existing one and follow it. A second way needs a reason, recorded where the pattern
  is defined (an ADR, or the summary comment). Prefer the framework's and C#'s own conventions over inventing ours.
- **Abstraction, not indirection.** A name must be trustworthy without opening it:
  - **Program.cs owns the order.** It is a flat, unconditional list of `Add*`, `Use*` and `Map*` calls. Each call adds its concern at exactly that
    point; an extension never adds middleware or endpoints anywhere else. Reading down Program.cs shows the pipeline.
  - **The owning extension decides its environment behaviour**, as the framework's own builders do. No environment checks in Program.cs.
  - **One level deep.** An extension's body can be understood on its own, without opening further environment-dependent layers.
  - **Honest names and summaries.** Names describe the concern, not the condition (`UseErrorPages`, not `UseLocalDevelopmentErrorPages`).
    If behaviour differs by environment, the summary's first line says how, so it shows on hover.
  - `TryAdd*` keeps its .NET meaning, "register unless already registered", and nothing else.
- **One overview.** Every environment-dependent behaviour is listed in
  [architecture: environment-dependent behaviour](../architecture.md#environment-dependent-behaviour), linking to its code.
- **Tests point to their source.** See [tracing tests to their source](../ways-of-working.md#tracing-tests-to-their-source).

## Standards and guidance

- [ ] Service Standard point(s):
- [x] Technology Code of Practice point(s): 8 (share, reuse and collaborate: code others can follow)
- [ ] Secure by Design principle(s):
- [x] DfE technical guidance page(s): [development principles](https://technical-guidance.education.gov.uk/principles/coding-principles/)
- [ ] Other:

Departures from a recommendation, and why: none.

## Enforcement

- **Analyser** (#23): no `IHostEnvironment` checks in top-level statements (Program.cs).
- **Review only**: "converge on one way", "one level deep" and honest summaries. Reviewers ask: is there already a way to do this, and could I trust this name without opening it?
- **Agent instructions** repeat the rules (AGENTS.md).

## Consequences

- Program.cs stays short and in pipeline order; environment differences are found through the overview table or the summaries, not by reading every method.
- Occasionally a simple `if` in Program.cs moves into a small extension. That costs a file, and buys one place for each concern.
- Failure mode: a summary that drifts from the code. Mitigated by keeping extensions small and listing each behaviour in the overview.

## Alternatives considered

- All environment checks in Program.cs, with unconditional extensions: everything visible in one file, but repeated across Web and Worker, and at odds
  with the framework's style and with each integration choosing its own mode ([0010](0010-unconfigured-integrations-fall-back-safely.md)).
- No convention: each contributor picks a style, and readers have to learn all of them.
