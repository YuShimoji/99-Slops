# TASK_019_InputBinding_Conflict_Resolution

## Status
OPEN

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/input-binding-conflict-resolution

## Summary
`LeftShift` is currently used by both `Sprint` (DebugView toggle path) and `FastFall`, creating control conflicts.
Resolve binding overlap and document final controls for Phase 2A/2B validation.

## Dependency
- `TASK_016_SandboxScene_SSOT_Sync`

## Scope
- Remove or remap conflicting binding(s) in `InputSystem_Actions.inputactions`.
- Align `DebugView` trigger behavior with new input mapping.
- Update resume/task docs with final key mapping.

## Deliverables
- Updated `99PercentSlops/Assets/InputSystem_Actions.inputactions`
- Required script/doc updates for new debug toggle path
- Validation note showing FastFall and DebugView no longer conflict

## Focus Area / Forbidden Area
- Focus: `Assets/InputSystem_Actions.inputactions`, `Assets/_Project/Scripts/Systems/DebugView.cs`, `docs/dev/RESUME.md`
- Forbidden: changing dash/fastfall gameplay mechanics themselves

## Constraints
- Preserve existing player core controls unless conflict fix requires explicit remap.
- Prefer low-risk remap for debug-only action.

## Definition of Done (DoD)
- FastFall input works independently.
- DebugView toggle works on a non-conflicting binding.
- Updated control mapping is reflected in docs.

## Test Plan
- PlayMode:
  - trigger FastFall repeatedly and verify no debug toggle side effect
  - trigger DebugView with new mapping and verify expected behavior
- Regression:
  - check Move/Jump/Dash/TogglePerspective still operate correctly

## Risks / Notes
- Input asset changes can silently break callback names if action names are modified.

## Milestone
- Phase 2B readiness / Phase 2A closeout
