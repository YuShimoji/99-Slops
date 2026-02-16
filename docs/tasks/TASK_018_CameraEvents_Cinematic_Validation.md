# TASK_018_CameraEvents_Cinematic_Validation

## Status
OPEN

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/camera-events-cinematic-validation

## Summary
Camera event hooks are implemented in code, but scene-level validation is incomplete.
Add minimal scene wiring to validate `CameraViewModeChanged`, `CinematicEntered`, and `CinematicExited`.

## Dependency
- `TASK_016_SandboxScene_SSOT_Sync`
- `TASK_014_GameEvents_Camera` (superset completion)

## Scope
- Place `GameEventDebugLogger` in SSOT sandbox scene.
- Add one `CinematicCameraZone` + camera point for enter/exit verification.
- Verify event dispatch count and ordering during 1P/3P switch and cinematic enter/exit.

## Deliverables
- Scene object for `GameEventDebugLogger`
- Scene object(s) for minimal cinematic zone validation
- Worker report including event log snippets and duplication check

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scripts/Camera`, SSOT sandbox scene
- Forbidden: unrelated AI/gameplay feature expansion

## Constraints
- No broad scene redesign; add only minimal validation setup.
- Ensure null-safe event flow remains intact.

## Definition of Done (DoD)
- Mode switch emits `CameraViewModeChanged` once per actual switch.
- Entering/leaving cinematic emits corresponding events without duplicates.
- No runtime null reference errors in logger/event flow.

## Test Plan
- PlayMode:
  - toggle 1P/3P with `V` and inspect logs
  - enter/exit cinematic zone and inspect logs
- Negative:
  - ensure no events fire when switching to same mode repeatedly without mode change

## Risks / Notes
- Event duplication can occur if multiple loggers are present or scene contains duplicate camera managers.

## Milestone
- Phase 2A
