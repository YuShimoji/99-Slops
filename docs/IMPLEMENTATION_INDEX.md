# Implementation Index

This page is an item-by-item index for implementation review. The linked task and report files remain the authority; this table only helps locate them quickly.

## Current Item Index

| Area | Item | Task status shown in source | Report / detail page | Review use |
|------|------|-----------------------------|----------------------|------------|
| Camera foundation | [TASK_013 CameraSettings SO](tasks/TASK_013_CameraSettings_SO.md) | DONE | [REPORT_013](reports/REPORT_013_CameraSettings_SO.md) | CameraSettings asset and fallback behavior. |
| Camera events | [TASK_014 GameEvents Camera](tasks/TASK_014_GameEvents_Camera.md) | DONE | [REPORT_014](reports/REPORT_014_GameEvents_Camera.md) | Camera event publication, logging, and null-safe behavior. |
| Camera resume checklist | [TASK_015 Phase2A ResumeChecklist](tasks/TASK_015_Phase2A_ResumeChecklist.md) | DONE | [REPORT_015](reports/REPORT_015_Phase2A_ResumeChecklist.md) | Phase 2A resume criteria and checked dependencies. |
| Scene / SSOT alignment | [TASK_016 SandboxScene SSOT Sync](tasks/TASK_016_SandboxScene_SSOT_Sync.md) | DONE | Task file only | Official Sandbox scene path and reference alignment. |
| Camera asset wiring | [TASK_017 CameraSettings Asset Wiring](tasks/TASK_017_CameraSettings_Asset_Wiring.md) | DONE | Task file only | CameraSettings field wiring and fallback access pattern. |
| Cinematic validation | [TASK_018 CameraEvents Cinematic Validation](tasks/TASK_018_CameraEvents_Cinematic_Validation.md) | DONE | [REPORT_018](reports/REPORT_018_CameraEvents_Cinematic_Validation.md) | Cinematic camera event validation evidence. |
| Input binding | [TASK_019 InputBinding Conflict Resolution](tasks/TASK_019_InputBinding_Conflict_Resolution.md) | CLOSED | Task file only | DebugView / FastFall / Sprint binding conflict resolution. |
| Playable loop | [TASK_020 PlayableLoop CoreFlow](tasks/TASK_020_PlayableLoop_CoreFlow.md) | COMPLETED | [REPORT_020](reports/REPORT_020_PlayableLoop_CoreFlow.md) | Playing/Cleared/Failed/Restart loop core. |
| Objective wiring | [TASK_021 UploadPort Objective Wiring](tasks/TASK_021_UploadPort_Objective_Wiring.md) | COMPLETED_CORE | [REPORT_021](reports/REPORT_021_UploadPort_Objective_Wiring.md) | UploadPort objective progress and Unity placement gap. |
| HUD | [TASK_022 ResultHUD Minimal](tasks/TASK_022_ResultHUD_Minimal.md) | COMPLETED_CORE | [REPORT_022](reports/REPORT_022_ResultHUD_Minimal.md) | Progress/state HUD and Unity placement gap. |
| Loop finalization | [TASK_023 PlayableLoop ClearFail Finalize](tasks/TASK_023_PlayableLoop_ClearFail_Finalize.md) | COMPLETED | [REPORT_023](reports/REPORT_023_PlayableLoop_ClearFail_Finalize.md) | Clear/fail transition guard and restart behavior. |
| Vertical slice integration | [TASK_024 Phase5 VerticalSlice Integration](tasks/TASK_024_Phase5_VerticalSlice_Integration.md) | COMPLETED | [REPORT_024](reports/REPORT_024_Phase5_VerticalSlice_Integration.md) | Integration of TASK_020 through TASK_023 and null-safe refresh behavior. |
| Unity validation | [TASK_025 UnityDeferred Validation Batch](tasks/TASK_025_UnityDeferred_Validation_Batch.md) | OPEN | [REPORT_025](reports/REPORT_025_UnityDeferred_Validation_Batch.md) | Manual Unity checks and DONE promotion decisions. |
| Completion assessment | [TASK_026 ProjectCompletion Assessment](tasks/TASK_026_ProjectCompletion_Assessment.md) | COMPLETED | Task file only | Overall completion assessment and remaining blockers. |

## How to Read This

- `DONE`, `COMPLETED`, `COMPLETED_CORE`, `OPEN`, and `CLOSED` are copied as status categories from the source task files.
- `COMPLETED_CORE` means the code-side core appears done, but Unity scene placement or PlayMode evidence is still called out by the source task.
- A missing report does not imply missing work; it means the task file is the only indexed detail page found in `docs/reports/`.
- Screenshots are not embedded here. Visual proof should be linked from the relevant task/report once captured.
