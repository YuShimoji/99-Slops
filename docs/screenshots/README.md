# Progress Screenshots

This directory is the expected location for progress screenshots used by the local documentation view.

## Current Repository Scan

No project progress screenshots were found in the Markdown/documentation scan. The only image detected by the broad repository image search was:

- `99PercentSlops/Assets/TutorialInfo/Icons/URP.png`

That file appears to be a Unity tutorial/URP icon, not progress evidence for the game state.

## Placement Convention

Use this directory for visual proof that should be easy to find from the MkDocs tree:

| Evidence type | Suggested path | Link from |
|---------------|----------------|-----------|
| Current playable-state screenshots | `docs/screenshots/progress/` | [Project Map](../PROJECT_MAP.md), [TASK_026](../tasks/TASK_026_ProjectCompletion_Assessment.md) |
| Unity validation captures | `docs/screenshots/validation/` | [TASK_025](../tasks/TASK_025_UnityDeferred_Validation_Batch.md), [REPORT_025](../reports/REPORT_025_UnityDeferred_Validation_Batch.md) |
| Before/after comparison captures | `docs/screenshots/comparisons/` | The task/report that made the change |

Suggested filename shape:

```text
TURN-XX_short-topic_view-or-test.png
```

## Guardrails

- Screenshots are supporting evidence, not specifications.
- Do not use screenshots to replace task/report acceptance criteria.
- If a screenshot proves a validation result, link it from the relevant task or report.
- Do not commit large capture batches; keep only review-useful images.
