# TASK_022_ResultHUD_Minimal

## Status
COMPLETED_CORE (Unity配置は手動作業が必要)

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/resulthud-minimal

## Summary
ゲーム成立確認に必要な最小HUD（進捗表示・Cleared/Failed表示・再開ガイド）を追加する。

## Dependency
- `TASK_020_PlayableLoop_CoreFlow`
- `TASK_021_UploadPort_Objective_Wiring`

## Scope
- 最小HUDプレハブ/Canvasを追加。
- 進捗カウントと状態表示をバインド。
- Cleared/Failed時の表示切替とリスタート案内を実装。

## Deliverables
- `99PercentSlops/Assets/_Project/Scripts/UI/GameplayHudPresenter.cs`
- `99PercentSlops/Assets/_Project/Scenes/Sandbox.unity`（HUD最小配線）
- Worker report: `docs/reports/REPORT_022_ResultHUD_Minimal.md`

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/UI`, `Assets/_Project/Scenes/Sandbox.unity`
- Forbidden: UIデザイン大改修、演出強化、メニューシステム拡張

## Constraints
- 実装は可読性優先の最小構成とする。
- 表示要素はゲーム成立に必要な情報だけに限定する。
- テストは Smoke + 変更箇所限定。

## Definition of Done (DoD)
- Playing中に進捗が表示される。
- Cleared/Failedで状態表示が切り替わる。
- リスタート案内が表示され、再開導線が成立する。

## Test Plan
- PlayMode (Smoke):
  - 進捗表示の更新
  - Cleared表示遷移
  - Failed表示遷移
  - リスタート後に表示が初期化される

## Risks / Notes
- 先に機能成立を優先し、見た目改善は後続タスクへ分離する。

## Milestone
- Phase 5 (Playable Loop)
