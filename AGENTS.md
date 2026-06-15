# AGENTS.md

This file helps coding agents be productive in the AuthZI repository.

## Scope

- Applies to the whole repository.
- Prefer targeted, minimal edits; avoid broad refactors unless explicitly requested.

## Repository map

- `src/AuthZI.Security`: core authorization, token verification, and introspection.
- `src/AuthZI.Security.Authorization`: C# authorization abstractions used by integrations and adapters.
- `src/AuthZI.Identity.Duende.IdentityServer` and `src/AuthZI.Identity.MicrosoftEntra`: identity-provider integrations.
- `src/AuthZI.MicrosoftOrleans*`: Orleans integration packages, including Duende IdentityServer and Microsoft Entra adapters.
- `src/AuthZI.Extensions` and `src/AuthZI.AzureFunctions`: shared `netstandard2.0` support libraries.
- `test/AuthZI.Tests.Security`, `test/AuthZI.Tests.Duende.IdentityServer.*`, and `test/AuthZI.Tests.MicrosoftOrleans`: test projects split by integration area and target framework.
- `samples/microsoft-orleans/IdentityServer/`: runnable sample apps used in `README.md`.
- `deploy/MicrosoftEntra/`: Azure CLI scripts and configuration for Entra setup.
- `docs-builder/`: DocFX source; generated site output is under `docs/`.

## Build and test

- Build the full solution with `dotnet build AuthZI.slnx -c Release`.
- Build packable projects with `dotnet build AuthZI.Build.slnx -c Release`.
- Prefer targeted test runs for the changed area before broader runs.
- Common CI test targets include:
  - `dotnet test test/AuthZI.Tests.Security -c Release`
  - `dotnet test test/AuthZI.Tests.Duende.IdentityServer.7.4 -c Release`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/Duende.IdentityServer.7.4 -c Release`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraID -c Release -f net8.0`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraExternalID -c Release -f net8.0`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraID -c Release -f net9.0`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraExternalID -c Release -f net9.0`
  - `dotnet test test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/MicrosoftEntraID -c Release -f net10.0`

## Conventions

- Follow `.editorconfig`: 2-space indentation, 80/120 guidance lines, and CRLF line endings.
- Keep F# file order aligned with the owning `.fsproj`; new `.fs` files must be inserted in the correct compile order.
- Prefer F# native `Option<'T>`/`Result<'T, 'Error>` inside F#-only boundaries; expose C#-friendly `Maybe<T>`/`Result` shapes at public interop boundaries.
- `src/AuthZI.Security` and `src/AuthZI.Identity.MicrosoftEntra` already reference `CSharpFunctionalExtensions` `3.7.0` for public C#-facing APIs.

## CI and test pitfalls

- `azure-pipelines.yml` builds `AuthZI.slnx` with .NET SDK 10.0.x and splits integration tests by target framework.
- Microsoft Entra tests read `microsoftEntraIdCredentials` and `microsoftEntraExternalIdCredentials`; see `test/AuthZI.Tests.MicrosoftOrleans/MicrosoftEntra/*/Initialization/Starter.fs` for the fallback JSON used when those variables are absent.
- Packaging/versioning flow uses `packing-artifacts.yml` and `GitVersion.yml`.
- Sonar settings and exclusions are in `sonar-project.properties`.

## Reference Docs

- Overview: [README.md](README.md)
- CI and test matrix: [azure-pipelines.yml](azure-pipelines.yml)
- Packaging template: [packing-artifacts.yml](packing-artifacts.yml)
- Versioning rules: [GitVersion.yml](GitVersion.yml)
- Documentation source: [docs-builder/index.md](docs-builder/index.md)

