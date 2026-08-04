# 再開用サマリ (2026-06-15)

## 現在の Git 状態
- 現在の checkout は `feature/task-016-story-catalog-metaflags`。
- `git fetch --prune origin` 実行済み。
- 現在ブランチの upstream とは一致済み: `HEAD...@{u}` = `0 0`。
- `git pull --ff-only` は `Already up to date.`。
- `origin/master` は `0e45e57` まで進んでおり、現在 feature branch とは分岐している。
- local `master` と `origin/master` も `20 1` で分岐している。

## 重要な注意
- `origin/master` は Unity 6000.4.9f1 / URP 17.4.0 系の handoff lane。
- `origin/master` 側では Unity プロジェクトが `99PercentSlops/` 配下にある。
- 現在 checkout は feature branch で、作業ツリーに未コミット変更があるため、ここで `origin/master` を直接混ぜない。

## 残っているローカル変更
| 対象 | 状態 | 目的 | 効果 | 要件 | Owner | Next |
| --- | --- | --- | --- | --- | --- | --- |
| `.claude/settings.local.json` | modified | Claude ローカル許可設定 | Unity/Assets 読取・起動許可の追加 | 機密/端末依存設定として扱う | local | commit 対象にするか判断 |
| `.gitmodules` / `shared-workflows` | deleted | shared-workflows サブモジュール除去 | サブモジュール依存を外す | remote lane と衝突しうる | local | mainline lane へ混ぜる前に方針決定 |
| `CLAUDE.md` / `AGENTS.md` | modified/untracked | AI entrypoint 整理 | ルート指示を明示 | AGENTS を薄く保つ | local | tracked 化するか判断 |
| `.serena/` | untracked | Serena ローカル設定 | symbolic tool 前提の補助 | 端末依存の可能性あり | local | commit 対象外候補 |
| `nul` | untracked | 不明 | 不明 | 内容確認後に扱う | local | 削除/保持を判断 |

## 再開ルート A: 最新 mainline を進める
1. いまの未コミット変更を stash / commit / 破棄のいずれかで明示的に処理する。
2. `origin/master` へ移る。
3. Unity 6000.4.9f1 で `99PercentSlops` プロジェクトを開く。
4. `docs/dev/PHASE5_VALIDATION_PREFLIGHT.md` の C-01 から確認する。
5. `docs/tasks/TASK_025_UnityDeferred_Validation_Batch.md` の V-01 から V-06 を実施する。

## 再開ルート B: 現在 feature branch を続ける
1. `docs/WORKFLOW_STATE_SSOT.md` の current checkout を確認する。
2. `TASK_016` / `TASK_017` 系の story catalog / overworld work を続ける。
3. `origin/master` の Phase 5 validation lane は別レーンとして扱い、直接マージしない。

## 推奨
- プロジェクト全体の最新開発再開はルート A。
- 現在の未コミット entrypoint/workflow cleanup を採用するか判断してから、latest mainline に移る。
