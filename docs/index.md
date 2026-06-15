# Local Document View

This page is a local-only entry point for browsing repository Markdown in a tree-pane web UI. It is for review, audit, and temporary browser-assisted translation checks; it is not a replacement for the source Markdown files.

## Scope

- The source Markdown files remain the authoritative documents.
- This view does not create permanent translated files.
- Browser translation is treated as a temporary reading aid for Chrome, Edge, or the DeepL extension.
- The navigation groups are practical browsing buckets, not a new specification structure.

## Start Locally

From Windows PowerShell at the repository root:

```powershell
python -m pip install mkdocs-material
python -m mkdocs serve
```

Then open:

```text
http://127.0.0.1:8000/
```

If the `mkdocs` command is already on `PATH`, `mkdocs serve` is equivalent.

## Browser Translation Check

1. Start the local server.
2. Open `http://127.0.0.1:8000/` in Chrome or Edge.
3. Choose a document from the left navigation tree.
4. Use the browser page translation feature, or the DeepL extension, on the currently displayed page.
5. Treat translated text as temporary reading support only; do not copy it back into the source Markdown as a canonical translation.

## Navigation Notes

The MkDocs navigation is intentionally shallow:

- `Project Map` is the fastest route for checking where implementation history, current status, future work, screenshots, and turn-based plans live.
- `Implementation Index` lists task/report pairs so implementation details can be opened by item.
- `Overview` contains handoff and milestone documents.
- `Specs` contains the current `.md` specification documents.
- `Runtime State` contains state and resume documents that appear to describe current project continuation points.
- `Development Notes` contains roadmaps, audits, setup notes, and collaboration rules.
- `Artifacts` contains task files, worker reports, and inbox prompts/reports.
- `Misc` is available for future Markdown that is present in the repository but does not clearly belong to the project documentation tree.

Root-level Markdown such as `AGENTS.md`, `CLAUDE.md`, and `AI_CONTEXT.md` was read and classified during setup, but is outside the MkDocs `docs_dir`. It is therefore not included in the initial tree pane. This keeps the preview structure compatible with MkDocs without copying root source files into `docs/`.

Some Markdown-like files in `docs/spec/` do not use the `.md` extension, such as `GDD0.1-1`, `GDD0.1-2`, and `PlayerControlSpec1.0`. They are left in place and are not converted or renamed by this viewer setup.

The Unity project tree is excluded from the default MkDocs source scan to avoid copying asset folders into the preview output. `99PercentSlops/Assets/MCPForUnity/README.md` was detected during the repository scan, but is treated as auxiliary package documentation rather than a project documentation page in the initial viewer.

To regenerate a navigation candidate without editing `mkdocs.yml`, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\generate-doc-nav.ps1
```

Review the generated candidate before copying anything into `mkdocs.yml`.
