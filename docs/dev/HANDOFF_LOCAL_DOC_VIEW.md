# Local Doc View Handoff

This handoff keeps the current documentation-view context inside the repository so another terminal can resume without relying on chat history.

## Branch and Purpose

- Branch: `codex/local-doc-view-handoff`
- Remote: `origin` / `YuShimoji/99-Slops`
- Purpose: add a local MkDocs Material viewer that makes the Markdown corpus browsable, auditable, and easier to inspect with browser-side translation.

## What Was Added

| File | Role |
|------|------|
| [Local Document View](../index.md) | Entry point, browser translation guidance, and local serve commands. |
| [Project Map](../PROJECT_MAP.md) | Fast route to current state, implementation history, future work, screenshots, and specifications. |
| [Implementation Index](../IMPLEMENTATION_INDEX.md) | Task/report pairing for itemized implementation review. |
| [Turn-Based Development Plan](TURN_PLAN.md) | Work-session turns that are not date-based. |
| [Progress Screenshots](../screenshots/README.md) | Placement convention for future visual progress evidence. |
| `mkdocs.yml` | Tree-pane navigation and Material theme configuration. |
| `tools/generate-doc-nav.ps1` | Non-destructive nav candidate generator. |

## Resume Commands

From a fresh terminal:

```powershell
git fetch origin
git switch codex/local-doc-view-handoff
python -m pip install mkdocs-material
python -m mkdocs serve
```

Then open:

```text
http://127.0.0.1:8000/
```

If port `8000` is occupied:

```powershell
python -m mkdocs serve -a 127.0.0.1:8010
```

## Validation Already Run

```powershell
python -m mkdocs build --clean
git diff --check
```

The build succeeded. The only remaining MkDocs warning is an existing link in `docs/Windsurf_AI_Collab_Rules_v2.0.md` pointing to `../README.md#reference-navigation`, where no root `README.md` currently exists.

The following local pages returned HTTP 200 during verification:

- `/PROJECT_MAP/`
- `/IMPLEMENTATION_INDEX/`
- `/dev/TURN_PLAN/`
- `/screenshots/`

## Existing Local Context Included In This Branch

The working tree also contained local documentation-context changes that were not created by the local viewer itself:

- `CLAUDE.md` is deleted.
- `docs/dev/PROJECT_AUDIT.md`, `docs/dev/ROADMAP.md`, and `docs/dev/ROADMAP_v2.md` refer to `AGENTS.md` instead of `CLAUDE.md` in the touched lines.

These changes were kept as part of the local context because they affect which project rule document future agents should read. If this was not intended, review that migration before merging the branch.

## Next Work Choices

| Turn | Start here | Purpose |
|------|------------|---------|
| Verify | [TASK_025](../tasks/TASK_025_UnityDeferred_Validation_Batch.md) | Confirm Unity scene wiring and produce validation evidence. |
| Capture | [Progress Screenshots](../screenshots/README.md) | Add visual proof for the current playable state once Unity validation is run. |
| Audit | [Implementation Index](../IMPLEMENTATION_INDEX.md) | Check whether task/report statuses still match the current implementation state. |
| Advance | [Turn-Based Development Plan](TURN_PLAN.md) | Continue from the next work-session turn after validation state is clear. |
