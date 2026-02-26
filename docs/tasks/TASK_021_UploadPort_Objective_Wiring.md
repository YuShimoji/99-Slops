# TASK_021_UploadPort_Objective_Wiring

## Status
COMPLETED_CORE (Unity配置は手動作業が必要)

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/uploadport-objective-wiring

## Summary
最小ゲームループ成立に必要な「目標達成点（UploadPort）」を実装し、クリア判定へ接続する。

## Dependency
- `TASK_020_PlayableLoop_CoreFlow`

## Scope
- UploadPortトリガーを実装。
- 対象オブジェクトの受け入れ条件（最小条件）を定義。
- 受け入れ成功時に進捗をGameplayLoopControllerへ通知。
- Sandboxに最小配置して検証可能にする。

## Deliverables
- `99PercentSlops/Assets/_Project/Scripts/Gimmicks/UploadPort.cs`
- `99PercentSlops/Assets/_Project/Scenes/Sandbox.unity`（UploadPort最小配置）
- `99PercentSlops/Assets/_Project/Scripts/Systems/GameplayLoopController.cs`（必要最小限の連携）
- Worker report: `docs/reports/REPORT_021_UploadPort_Objective_Wiring.md`

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Gimmicks`, `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scenes/Sandbox.unity`
- Forbidden: 複雑な演出追加、AI仕様拡張、別シーン展開

## Constraints
- 目標判定は「最小仕様」で先に成立させる。
- 既存Prop/Droneの挙動を壊さない。
- 最短でPhase 5の完成条件へ寄与する変更に限定する。

## Definition of Done (DoD)
- UploadPortへ対象を投入すると進捗が加算される。
- 必要数到達で Cleared 遷移が成立する。
- 未達状態での誤クリアが発生しない。

## Test Plan
- PlayMode (Smoke):
  - 対象投入で進捗が増える
  - 必要数到達でクリアになる
  - 対象外では進捗が増えない

## Risks / Notes
- 判定条件を広げすぎると誤検知しやすいため、初期版は厳しめ条件で実装する。

## Milestone
- Phase 5 (Playable Loop)
