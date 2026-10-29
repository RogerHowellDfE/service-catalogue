# Security

## Reporting a vulnerability

Please **don't** open a public issue for a security problem.

- For a vulnerability in this repository's code, use GitHub's [private vulnerability reporting](https://github.com/RogerHowellDfE/service-catalogue/security/advisories/new).
- For anything affecting a Department for Education system or service, follow the
  [DfE vulnerability disclosure policy](https://www.gov.uk/guidance/report-a-vulnerability-on-a-department-for-education-system)
  and the [DfE security notice](https://github.com/DFE-Digital/.github/blob/master/SECURITY.md).

Please include where the problem is, a short description, and non-destructive steps to reproduce it.

## Scope

This service runs locally only and isn't deployed anywhere reachable from the internet ([NFR-S10](docs/requirements.md#security-secure-by-design)).
How we build security in is described in the [requirements](docs/requirements.md#security-secure-by-design)
and [ways of working: automated security and quality scanning](docs/ways-of-working.md#automated-security-and-quality-scanning).
