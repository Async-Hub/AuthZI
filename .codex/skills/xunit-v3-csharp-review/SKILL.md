---
name: xunit-v3-csharp-review
description: Investigate .NET/C#, xUnit v3 without making changes.
---

# xUnit v3 C# review skill

Use this skill when the user asks to investigate .NET/C#/xUnit v3 problems.

## Behavior

- Read relevant source, test, project, and pipeline files. If relevant files are not available in context, explicitly ask the user to share them before proceeding with the analysis. Do not fabricate file contents or paths.
- Do not modify any files. If the user asks you to make changes, confirm the specific files and changes before proceeding.
- Do not commit or push.
- Identify root causes and alternatives.
- Prefer concrete file paths and commands.

## Focus areas

- xUnit v3 discovery and fixtures

## Output format

1. Summary
2. Findings
3. Risks
4. Options
5. Recommended approach
6. Verification commands
   - Include exact CLI commands needed to reproduce the identified issue and to confirm the recommended fix, using dotnet CLI syntax (e.g., dotnet test, dotnet build) with any relevant flags or filters.