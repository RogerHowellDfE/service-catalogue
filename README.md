# DfE Service Catalogue

> Working name.

A single view of the Department for Education's code repositories and the services they support, across GitHub organisations and Azure DevOps.
It brings together information that people provide with data collected from source APIs, so that we can find things, analyse them and act on what we find.
The aim is to support teams, not to police them. See the [vision](docs/vision.md).

## Status

Walking skeleton (M0 in the [roadmap](docs/roadmap.md)). Web, Worker and PostgreSQL run locally with no secrets ([NFR-D1](docs/requirements.md#developer-and-first-run-experience)).
There are no features yet.

## Getting started

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download) and a container runtime ([Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/)) running.

```shell
git clone https://github.com/RogerHowellDfE/service-catalogue.git
cd service-catalogue
dotnet tool restore
dotnet aspire run
```

Open the dashboard link printed in the terminal. It lists each resource and its address; select the `web` endpoint to open the site.
Nothing needs configuring: anything not set up falls back to a fake or is turned off, and says so.

If resources stay "Waiting" or show as unhealthy, the terminal may not say why; check the dashboard for details.
Stop the app with Ctrl+C or `dotnet aspire stop`, which removes its containers. If a container was restarted with `dotnet aspire resource <name> start`, it can be left running afterwards,
using memory and CPU until it's stopped and disk until it's removed. An interrupted run can also leave a stopped container behind, which uses disk only.
`docker ps -a` lists both (the name starts with the resource name, for example `postgres-`), and `docker rm -f -v <name>` removes it and its data.

### Developing

```shell
dotnet build
dotnet test
dotnet format --verify-no-changes --severity info
```

To fix formatting, run the last command without `--verify-no-changes`.
To use a real integration instead of its fake, or for anything about deployed environments, see [local configuration](docs/architecture.md#local-configuration).

## Documentation

- [Planning and design documentation](docs/README.md), including [where things live](docs/README.md#where-things-live)
- [Architecture decision records](docs/adr/README.md)
- [Changelog](CHANGELOG.md)

## Contributing and security

- [How to contribute](CONTRIBUTING.md)
- [Reporting a vulnerability](SECURITY.md)

## Licence

[MIT](LICENCE.md), Crown Copyright (Department for Education).
