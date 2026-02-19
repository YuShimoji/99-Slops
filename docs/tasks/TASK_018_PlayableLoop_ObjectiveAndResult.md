# TASK_018_PlayableLoop_ObjectiveAndResult

## Status
OPEN

## Tier / Branch
- Tier: 2 (Integration)
- Branch: feature/task-018-playableloop-objective-result

## Summary
Deliver a visible one-cycle playable loop for vertical slice validation: objective start -> progress -> success/failure -> result display.

## Scope
- Objective state management (start/progress/success/failure)
- Minimal UI for objective and result display
- Sandbox-ready end-to-end verification path

## Focus Area / Forbidden Area
- Focus:
  - `Assets/_Project/Scripts/Systems`
  - `Assets/_Project/Scripts/UI`
  - `Assets/Tests`
- Forbidden:
  - Heavy production polish work (full VFX/SE pass)
  - Core camera/player behavior rewrites
  - Large asset ingestion

## Constraints
- Prioritize smallest useful implementation for fast validation
- Keep UI functional-first; visual polish comes later
- Failure path must be explicitly reproducible

## Definition of Done (DoD)
- [ ] Full one-cycle loop runs in Sandbox
- [ ] Both success and failure paths are validated
- [ ] UI display is synchronized with state
- [ ] PlayMode smoke result is recorded
- [ ] Demo procedure (target: <30 sec) is documented

## Stop Conditions
- If required Story/Overworld APIs are unavailable, stop and propose a temporary mock boundary

## Test Plan
- EditMode:
  - State transition tests (start/success/failure)
- PlayMode:
  - One-cycle run in Sandbox
  - Success path and failure path checks
- Build:
  - Verify build on dev target (capture logs on failure)

## Risks / Notes
- Future UI spec changes are expected, so keep state and rendering loosely coupled

## Milestone
- SG-6 / LG-1
