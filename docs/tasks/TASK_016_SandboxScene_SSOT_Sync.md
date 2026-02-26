# TASK_016_SandboxScene_SSOT_Sync

## Status
DONE

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/sandbox-scene-ssot-sync

## Summary
Sandbox scene reference is split between `Assets/Scenes/Sandbox.unity` and `Assets/_Project/Scenes/Sandbox.unity`.
Unify runtime SSOT to one scene and align documents/build settings so resuming work is deterministic.

## Dependency
- `TASK_015_Phase2A_ResumeChecklist` (acceptance basis)

## Scope
- Decide and fix official sandbox scene path for Phase 2A operations.
- Align references in resume/handover/roadmap documents.
- Ensure Build Settings scene list and documentation point to the same sandbox.

## Deliverables
- Updated `docs/dev/RESUME.md` (single sandbox path)
- Updated docs that still reference old sandbox path (if any)
- Validation note in this ticket for chosen SSOT scene

## Focus Area / Forbidden Area
- Focus: `docs/dev`, `docs/tasks`, `99PercentSlops/ProjectSettings`
- Forbidden: unrelated gameplay logic changes

## Constraints
- Do not break current playable setup.
- Keep changes minimal and traceable.

## Definition of Done (DoD)
- Exactly one sandbox path is treated as operational SSOT in docs.
- Build Settings and resume docs are consistent.
- Worker report includes before/after references proving alignment.

## Test Plan
- Static:
  - verify `ProjectSettings/EditorBuildSettings.asset` scene path
  - verify `docs/dev/RESUME.md` sandbox path
- Runtime:
  - open the SSOT sandbox scene and confirm Play starts without missing references

## Risks / Notes
- Scene divergence may hide stale serialized fields in one of the two scenes.
- If both scenes are needed for historical reasons, explicit naming policy is required.

## Milestone
- Phase 2A

## Validation Note (SSOT Decision)
- Chosen operational SSOT scene: `Assets/_Project/Scenes/Sandbox.unity`
- Reason: `docs/dev/RESUME.md` and prior project audit already treat `_Project` scene as canonical for Phase 2A resume flow.
- Build Settings alignment:
  - Before: `99PercentSlops/ProjectSettings/EditorBuildSettings.asset` pointed to `Assets/Scenes/Sandbox.unity` (guid `4dd3d87dac144e94aa2e9cb39c6ca0c6`)
  - After: same guid now points to `Assets/_Project/Scenes/Sandbox.unity`, matching SSOT docs.
- Documentation scan result:
  - Operational resume doc already uses SSOT path: `docs/dev/RESUME.md`
  - Legacy path references remain only in audit/history context (`docs/dev/PROJECT_AUDIT.md`) and are not operational instructions.
- Reporting precision:
  - For this confirmation pass, no additional file changes were required.
  - Repository may still contain existing uncommitted changes from prior tasks; this does not invalidate TASK_016 completion.
