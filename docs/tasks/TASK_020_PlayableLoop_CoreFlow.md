# TASK_020_PlayableLoop_CoreFlow

## Status
COMPLETED

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/playable-loop-coreflow

## Summary
ゲーム完成までの最短ルートとして、Sandboxで1サイクル成立する最小ゲームループ（進行中/クリア/失敗/再開）を実装する。

## Dependency
- `TASK_013_CameraSettings_SO`
- `TASK_014_GameEvents_Camera`
- `TASK_018_CameraEvents_Cinematic_Validation`

## Scope
- ループ状態管理コンポーネントを追加（Playing/Cleared/Failed）。
- クリア・失敗の確定条件を単一箇所で判定する。
- 最小再開導線（リスタート）を追加。
- Sandboxで動作確認可能な最小配線のみ行う。

## Deliverables
- `99PercentSlops/Assets/_Project/Scripts/Systems/GameplayLoopController.cs`
- `99PercentSlops/Assets/_Project/Scripts/Systems/GameManager.cs`（必要最小限の統合）
- Sandboxループ配線更新（最小）
- Worker report: `docs/reports/REPORT_020_PlayableLoop_CoreFlow.md`

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scenes/Sandbox.unity`
- Forbidden: 新規大型ステージ追加、UI大改修、非必須演出

## Constraints
- テストは Smoke + 変更箇所限定を原則とする。
- 既存操作系（移動・カメラ・Debug入力）を壊さない。
- 実装は最短で成立する範囲に限定する。

## Definition of Done (DoD)
- Playing -> Cleared / Failed の遷移がSandboxで再現できる。
- リスタートでPlayingへ戻れる。
- 既存の致命エラー（例外・NullReference）が増えない。

## Test Plan
- PlayMode (Smoke):
  - クリア条件成立で Cleared に遷移する
  - 失敗条件成立で Failed に遷移する
  - リスタート入力で Playing に戻る
- Regression (限定):
  - 1P/3P切替と移動が継続動作する

## Risks / Notes
- 条件分散実装にすると将来の調整コストが増えるため、判定集約を徹底する。

## Milestone
- Phase 5 (Playable Loop)
