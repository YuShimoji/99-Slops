# TASK_015_Phase2A_ResumeChecklist

## Status
DONE (2026-02-26)

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/phase2a-resume-checklist

## Summary
Phase 2A 再開時の実装順を固定し、`TASK_013` / `TASK_014` の着手条件と完了条件を先に揃える。

## Scope
- `Sandbox.unity` の Camera 関連配線確認
- `CameraSettings` SO 反映前チェックリスト作成
- `GameEventBus` 導入前チェックリスト作成
- 完了後に `docs/dev/RESUME.md` と整合

## Deliverables
- 本チケット内の再開チェック項目
- `docs/dev/RESUME.md` の「次タスクチェックリスト」更新
- `TASK_013` / `TASK_014` の着手順ガイド（本チケットから参照可能な形）

## Focus Area / Forbidden Area
- Focus: `docs/dev`, `docs/tasks`, `Assets/_Project/Scripts/Camera`, `Assets/_Project/Scripts/Systems`
- Forbidden: `ProjectSettings`, `Packages`, 無関係な Gameplay 改修

## Constraints
- Phase 2A の残タスクだけに絞る
- 実装前に PlayMode 確認項目を固定する

## Definition of Done (DoD)
- `TASK_013` と `TASK_014` の実行順を明文化できている
- Camera 側の受け入れ確認項目が列挙されている
- Event 側の受け入れ確認項目が列挙されている
- 再開用メモ (`docs/dev/RESUME.md`) の次タスクと一致している
- 3チケットの `Status` 更新ルール（OPEN → IN_PROGRESS → DONE）が明記されている

## Test Plan
- PlayMode:
  - `V` で 1P/3P 切替
  - マウス入力で視点回転
  - Camera 衝突回避が破綻しない
- Event:
  - ViewModeChanged 発火時にログで追跡可能
  - Cinematic Enter/Exit が重複発火しない

## Risks / Notes
- `TASK_013` 未完のまま `TASK_014` を先行すると設定値参照が不安定になる可能性あり
- 先に最低限の SO 定義を確定してから Event 化する
- Worker 起動前チェックは `node shared-workflows/scripts/worker-activation-check.js --ticket <ticket-path>` を使用

## Milestone
- Phase 2A
