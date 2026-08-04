# WORKFLOW_STATE_SSOT

## Last Updated
- 2026-06-15T23:18:48+09:00

## Current Checkout
- Branch: `feature/task-016-story-catalog-metaflags`
- Upstream: `origin/feature/task-016-story-catalog-metaflags`
- Upstream parity after fetch/pull: `0 0`
- HEAD: `8ad56a9` (`test(playmode): add minimal overworld director smoke tests`)

## Remote Sync
- `git fetch --prune origin` completed.
- `git pull --ff-only` on the current branch returned `Already up to date.`
- New remote branch observed: `origin/codex/local-doc-view-handoff`.
- `origin/master` is at `0e45e57` (`chore: sync unity upgrade and handoff context`).

## Branch Relationship
- Current feature branch and `origin/master` are not ancestor-related.
- Local `master` and `origin/master` are also divergent (`master...origin/master` = `20 1`).
- `origin/master` includes the newer Unity 6000.4.9f1 / URP 17.4.0 handoff lane and a project layout where the Unity project is under `99PercentSlops/`.
- Do not merge `origin/master` into this dirty feature checkout without an explicit lane decision.

## Local Working Tree
- Existing tracked local changes were preserved:
  - `.claude/settings.local.json`
  - `.gitmodules` deleted
  - `CLAUDE.md`
  - `docs/WORKFLOW_STATE_SSOT.md`
  - `shared-workflows` deleted
- Existing untracked local items were preserved:
  - `.serena/`
  - `AGENTS.md`
  - `nul`

## Active Decision
- If continuing the latest mainline, first preserve or discard the local dirty feature-branch changes intentionally, then move to `origin/master` / `origin/codex/local-doc-view-handoff`.
- If continuing `feature/task-016-story-catalog-metaflags`, keep the branch isolated and treat `origin/master` as a separate Phase 5 validation lane.

## Next Action
1. Decide the lane: latest mainline validation vs current story-catalog feature branch.
2. For latest mainline, work from `origin/master` and resume at `TASK_025` Unity deferred validation.
3. For current feature branch, finish or shelve the local entrypoint/workflow cleanup before attempting any cross-branch merge.
