## Tech Stack

- Main language: F# (.NET libraries).
- Secondary language: C# (authorization abstractions and selected tests).
- Domains: OAuth 2.0 authorization interoperability for Microsoft Orleans, Azure Functions, Microsoft Entra, and Duende IdentityServer.

## Fast Start

- Build all: `dotnet build AuthZI.slnx -c Release`
- Build packable projects only: `dotnet build AuthZI.Build.slnx -c Release`

Prefer targeted test runs for changed areas before broader test runs.

Common targeted test commands used in CI:

- `dotnet test test/AuthZI.Tests.Security -c Release`
- `dotnet test test/AuthZI.Tests.MicrosoftOrleans/Duende.IdentityServer -c Release`
- `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraID.NET8.0 -c Release`
- `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraExternalID.NET8.0 -c Release`
- `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraID.NET9.0 -c Release`
- `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraExternalID.NET9.0 -c Release`

## Repository Map

- `src/AuthZI.Security`: Core authorization and token verification/introspection.
- `src/AuthZI.Security.Authorization`: C# authorization abstractions used by integrations and adapters.
- `src/AuthZI.Identity.Duende.IdentityServer`: Duende IdentityServer integration.
- `src/AuthZI.Identity.MicrosoftEntra`: Microsoft Entra integration.
- `src/AuthZI.MicrosoftOrleans*`: Orleans integration packages.
- `src/AuthZI.Extensions`: Shared extension utilities (netstandard2.0).
- `src/AuthZI.AzureFunctions`: Azure Functions integration package (netstandard2.0).
- `test/`: Integration and unit-style tests across security and Orleans adapters.
- `deploy/MicrosoftEntra`: Azure CLI scripts/templates for Entra setup.
- `docs-builder/`: DocFX source; generated site output is under `docs/`.

## Project Conventions

- Follow `.editorconfig` (2-space indentation, C# style preferences, line-length guides).
- Keep naming and existing style consistent with surrounding code.
- F# project files list compile order explicitly; when adding a `.fs` file, update the corresponding `.fsproj` in dependency order.

## CI/Validation Expectations

- CI pipeline is defined in `azure-pipelines.yml`.
- Packaging/versioning flow uses `packing-artifacts.yml` and `GitVersion.yml`.
- Sonar settings and exclusions are in `sonar-project.properties`.

## Test Prerequisites / Pitfalls

- Some Microsoft Entra tests require environment variables:
  - `microsoftEntraIdCredentials`
  - `microsoftEntraExternalIdCredentials`
- CI runs Entra test jobs split by target framework (`net8.0` and `net9.0`).

## Reference Docs

- Overview: [README.md](../README.md)
- CI and test matrix: [azure-pipelines.yml](../azure-pipelines.yml)
- Packaging template: [packing-artifacts.yml](../packing-artifacts.yml)
- Versioning rules: [GitVersion.yml](../GitVersion.yml)
- Documentation source: [docs-builder/index.md](../docs-builder/index.md)
