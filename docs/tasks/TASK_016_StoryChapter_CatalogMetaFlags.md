# TASK_016_StoryChapter_CatalogMetaFlags

## Status
OPEN

## Tier / Branch
- Tier: 2 (Foundation)
- Branch: feature/task-016-story-catalog-metaflags

## Summary
Build the MG-2 base layer by implementing data-driven chapter definitions and meta-flag management.

## Scope
- Implement `ChapterDefinition` and `ChapterCatalog`
- Implement `MetaFlagService` (get/set/reset)
- Add minimal debug logs for observability

## Focus Area / Forbidden Area
- Focus:
  - `Assets/_Project/Scripts/Story` (new folder allowed)
  - `Assets/_Project/Scripts/Systems`
  - `Assets/Tests`
- Forbidden:
  - Camera/Player/Drone behavior changes
  - Large scene refactors

## Constraints
- Use ScriptableObject-driven data where possible
- Avoid scattered string literals for chapter and flag IDs
- Keep coupling to existing `GameManager` low

## Definition of Done (DoD)
- [ ] Multiple chapter definitions can be registered and queried
- [ ] Meta-flag read/write API works as expected
- [ ] Undefined ID handling policy is documented (log vs fallback)
- [ ] Core logic is covered by EditMode tests
- [ ] Report includes implementation notes and usage steps

## Stop Conditions
- If chapter requirements are not stable and specs conflict, stop and list clarification items

## Test Plan
- EditMode:
  - Chapter catalog registration/query tests
  - Meta-flag set/get/reset tests
  - Undefined key behavior tests
- PlayMode:
  - Lightweight smoke test for data loading during startup
- Build:
  - Confirm C# compile is clean

## Risks / Notes
- Future chapter expansions are expected; keep extension points explicit

## Milestone
- SG-4 / MG-2
