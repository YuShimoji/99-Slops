# TASK_017_OverworldDirector_VariantResolver

## Status
OPEN

## Tier / Branch
- Tier: 2 (Foundation)
- Branch: feature/task-017-overworld-variant-resolver

## Summary
Implement the execution layer that resolves chapter variants from chapter/meta-flag inputs and applies them to overworld flow.

## Scope
- Implement `OverworldDirector`
- Implement `ChapterVariantResolver`
- Connect minimal flow from chapter selection to overworld application

## Focus Area / Forbidden Area
- Focus:
  - `Assets/_Project/Scripts/Story` (new folder allowed)
  - `Assets/_Project/Scripts/Systems`
  - `Assets/Tests`
- Forbidden:
  - Camera/Player behavior changes
  - Heavy UI implementation
  - `ProjectSettings` / `Packages`

## Constraints
- Build on APIs defined by `TASK_016`
- Centralize branching logic in resolver
- Define safe fallback behavior for unresolved states

## Definition of Done (DoD)
- [ ] Variant can be resolved from chapter + meta-flag inputs
- [ ] OverworldDirector applies resolved result correctly
- [ ] At least one happy path and one fallback path are tested
- [ ] Decision logs are traceable
- [ ] Report captures test evidence and remaining risks

## Stop Conditions
- If `TASK_016` contract is unstable or incomplete, stop and record dependency issues

## Test Plan
- EditMode:
  - Variant resolver branch tests
  - Fallback branch tests
- PlayMode:
  - Overworld reflection smoke test after chapter change
- Build:
  - Confirm C# compile is clean

## Risks / Notes
- Transition rules are likely to grow; prioritize testable separation of logic

## Milestone
- SG-5 / MG-2
