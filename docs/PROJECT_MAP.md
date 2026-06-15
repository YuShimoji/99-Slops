# Project Map

This page is a review map for the local documentation view. It does not replace the source documents; use the linked files as the authority for specifications, task status, and implementation evidence.

## Fast Routes

| Need | Open first | Then inspect | What is currently visible |
|------|------------|--------------|---------------------------|
| Resume this exact documentation-view work | [Current Handoff](dev/HANDOFF_LOCAL_DOC_VIEW.md) | [Local Document View](index.md), [Turn-Based Development Plan](dev/TURN_PLAN.md) | Branch, commands, validation result, and remaining caveats are recorded for another terminal. |
| Current project state | [Resume](dev/RESUME.md) | [Workflow State SSOT](WORKFLOW_STATE_SSOT.md), [Project Completion Assessment](tasks/TASK_026_ProjectCompletion_Assessment.md) | Phase 5 vertical slice is code-integrated, with Unity manual validation still gating final DONE decisions. |
| Completed implementation work | [Implementation Index](IMPLEMENTATION_INDEX.md) | [Tasks](tasks/TASK_013_CameraSettings_SO.md), [Reports](reports/REPORT_013_CameraSettings_SO.md) | Work is itemized by task/report, but the new index is the quickest table view. |
| Upcoming development | [Turn-Based Development Plan](dev/TURN_PLAN.md) | [Roadmap v2](dev/ROADMAP_v2.md), [GDD 1.0](spec/GDD1.0.md), [Skill Ideas](spec/SKILL_IDEAS.md) | Future work exists by phase and feature area; the new turn plan adds non-date-based execution slices. |
| Progress screenshots | [Progress Screenshots](screenshots/README.md) | Task/report pages that should link captured evidence later | No project progress screenshots were found in the repository scan; a placement convention now exists. |
| Formal design constraints | [GDD 1.0](spec/GDD1.0.md) | [Camera System](spec/CAMERA_SYSTEM.md), [Player Control System](spec/PLAYER_CONTROL_SYSTEM.md) | GDD remains the specification source; this viewer only links to it. |

## Coverage Check

| Question | Current answer | Viewer adjustment |
|----------|----------------|-------------------|
| Can previous feature implementation be found quickly? | Yes, but it was scattered across task and report files. | Added [Implementation Index](IMPLEMENTATION_INDEX.md) and placed it in the top nav. |
| Can upcoming features and progress be found quickly? | Yes, mostly through [Roadmap v2](dev/ROADMAP_v2.md), [Skill Ideas](spec/SKILL_IDEAS.md), and current-state files. | Added [Turn-Based Development Plan](dev/TURN_PLAN.md) so the next work can be read by turn rather than by date. |
| Is implementation content grouped by item? | Mostly yes: task files define scope/status and report files describe worker results. | The index now pairs task/report entries by item where a report exists. |
| Are there immediate progress screenshots and a known location? | No progress screenshots were found; only a Unity tutorial icon was detected under the Unity asset tree. | Added [Progress Screenshots](screenshots/README.md) as the expected repository location and linking convention. |
| Is future planning split by turns rather than dates? | Not as a dedicated page. Existing plans are phase/date oriented. | Added a turn-based plan with Turn 1, Turn 2, and later build turns. |

## Reading Order

1. Start with [Current Handoff](dev/HANDOFF_LOCAL_DOC_VIEW.md) when resuming this docs-view branch from another terminal.
2. Open [Resume](dev/RESUME.md) for the shortest current-state pass.
3. Open [Project Completion Assessment](tasks/TASK_026_ProjectCompletion_Assessment.md) to see completion percentage, blockers, and the current validation gap.
4. Use [Implementation Index](IMPLEMENTATION_INDEX.md) when you need the implementation evidence by item.
5. Use [Turn-Based Development Plan](dev/TURN_PLAN.md) when deciding what the next work session should do.
6. Use [Progress Screenshots](screenshots/README.md) to check whether visual proof exists for the current state.
