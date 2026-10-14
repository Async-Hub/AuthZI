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


# Testing

Write deterministic, independent, repeatable, readable tests that protect observable behavior and justify their maintenance cost.

* Follow Arrange/Act/Assert where appropriate.
* Name tests after the behavior or invariant they protect.
* Prefer behavioral contracts over implementation details so tests survive internal refactoring.
* Aim for **one primary reason to fail**, not necessarily one assertion. Use the minimum assertions needed to verify one logical outcome or invariant; split unrelated concerns.
* Maximize confidence, not coverage or test count. Before adding a test, ask: **"What realistic regression would this catch?"**
* Avoid testing trivial accessors, framework behavior, or implementation details unless they protect an important contract.

## Test Isolation

Avoid process-global mutable state. Explicitly isolate tests modifying environment variables (`Environment.SetEnvironmentVariable` is process-wide), static state, shared files, fixed ports, or external test tenants.

Account for concurrent execution across assemblies and processes; disabling test-method parallelism alone is insufficient.

## Unit vs. Integration Tests

Unit tests must not depend on Microsoft Entra, Microsoft Graph, internet connectivity, external databases, or real credentials.

Prefer real domain objects and simple deterministic collaborators. Mock or fake external boundaries when necessary.

Separate integration tests requiring real services and credentials. Normal builds must not depend on external integration-test infrastructure.

## Test Libraries

Follow the repository's existing stack (xUnit v3, FluentAssertions, NSubstitute where already used). Introduce additional mocking or assertion frameworks only with concrete justification.

## Assertions

* Prefer expressive state/output assertions over interaction verification.
* Use only the assertions necessary to prove the behavior.
* Multiple assertions are valid when protecting one invariant (e.g., distinguishing Entra service principal object ID, application object ID, and client ID).
* Avoid asserting internal invocation counts/order, private state, intermediate objects, or exact request counts unless explicitly part of a behavioral, performance, or protocol contract.
* For `Result`, verify success/failure and the meaningful value/error when both are necessary.
* Avoid assertions that obscure the behavior being tested.

## AI-Generated Tests

Do not generate tests mechanically for every method, branch, property, or DTO.

Before adding a test:

1. Identify the behavior or invariant being protected.
2. Identify a realistic regression it would detect.
3. Check whether existing tests already cover it.
4. Choose the smallest useful test boundary.
5. Introduce abstractions only when they meaningfully reduce duplication or improve readability.

Avoid unnecessary mocks, duplicating production algorithms in tests, or making tests more complex than the behavior they protect.

When modifying production code, add or update only the smallest set of tests necessary to protect the changed behavior.


## Reference Docs

- Overview: [README.md](README.md)
- CI and test matrix: [azure-pipelines.yml](azure-pipelines.yml)
- Packaging template: [packing-artifacts.yml](packing-artifacts.yml)
- Versioning rules: [GitVersion.yml](GitVersion.yml)
- Documentation source: [docs-builder/index.md](docs-builder/index.md)

