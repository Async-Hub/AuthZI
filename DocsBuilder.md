# AuthZI Documentation Guide (DocFX)

This guide explains how to create and build documentation for the AuthZI solution.

## Overview

- Source documentation lives in `docs-builder/`.
- Generated static site output is written to `docs/`.
- DocFX configuration file is `docs-builder/docfx.json`.

The current DocFX configuration builds:

- Markdown and TOC content: `**/*.{md,yml}` (inside `docs-builder/`)
- Resource files: `images/**` and `documents/**/*.{jpg,png,svg}`
- Output folder: `../docs`

## Prerequisites

1. Install a supported .NET SDK (the repository targets modern .NET; .NET 10 SDK is used in CI).
2. Install DocFX CLI.

### Install DocFX

Use one of the following options.

Option A: global tool (quickest)

```powershell
dotnet tool install -g docfx
```

Option B: update global tool

```powershell
dotnet tool update -g docfx
```

Verify installation:

```powershell
docfx --version
```

If `docfx` is not found after installation, restart your terminal.

## Folder Structure

Important directories under `docs-builder/`:

- `index.md`: documentation landing page
- `toc.yml`: top-level navigation
- `documents/`: primary documentation pages
- `resources/`: supplementary pages
- `images/`: shared images
- `templates/custom/`: custom DocFX template assets

Generated website output:

- `docs/` (do not hand-edit unless explicitly needed)

## How To Create Documentation Content

1. Decide where the new content belongs:
	 - Product/user documentation -> `docs-builder/documents/`
	 - Supporting references/resource pages -> `docs-builder/resources/`

2. Add a new Markdown file in the selected folder.

3. Add the page into the appropriate TOC:
	 - Top-level: `docs-builder/toc.yml`
	 - Documents section: `docs-builder/documents/toc.yml`
	 - Resources section: `docs-builder/resources/toc.yml`

4. Add images under:
	 - `docs-builder/images/` for shared assets
	 - or section-local image folders where already used

5. Use relative links between pages and images.

6. Keep naming and wording consistent with repository conventions and existing docs.

## Build Documentation

Run from repository root:

```powershell
docfx docs-builder/docfx.json
```

This compiles source files in `docs-builder/` and writes generated site files to `docs/`.

## Build And Serve Locally

For local preview, run:

```powershell
docfx docs-builder/docfx.json --serve
```

DocFX will print a local URL (commonly `http://localhost:8080`) for previewing the generated site.

## Typical Update Workflow

1. Edit or add pages in `docs-builder/`.
2. Update TOC entries.
3. Build docs with DocFX.
4. Review generated output in `docs/` via local serve.
5. Fix broken links/content issues and rebuild.

## Troubleshooting

- Missing page in navigation:
	- Verify the file is added to the correct `toc.yml`.

- Image not shown:
	- Verify path and filename case.
	- Ensure image is under included resource folders.

- Command not found for `docfx`:
	- Reinstall/update DocFX tool.
	- Restart terminal so PATH updates are loaded.

- Build output not updated:
	- Re-run build from repository root with the same `docfx.json` path.
	- Ensure source edits were made under `docs-builder/`, not `docs/`.

## Notes For This Repository

- `docs-builder/docfx.json` currently outputs to `docs/`.
- `docs/` is generated content.
- Prefer editing only `docs-builder/` source files during normal documentation work.
