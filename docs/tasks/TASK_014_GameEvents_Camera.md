# TASK_014_GameEvents_Camera

## Status
DONE

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/gameevents-camera

## Summary
Camera の状態変更通知を `GameEventBus` 経由に統一し、`CameraManager` の状態遷移を疎結合化する。

## Scope
- `GameEventBus` にカメラ系イベントを追加
  - CameraViewModeChanged (1P/3P 切替)
  - CinematicEntered / CinematicExited
- `CameraManager` からイベント発火
- `GameEventDebugLogger` で可視化

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scripts/Camera`
- Forbidden: `Assets/_Project/Scenes` の大規模変更

## Constraints
- 既存挙動は維持する
- イベント二重発火と null 参照を防止する

## Definition of Done (DoD)
- カメラ状態変更時にイベントが期待どおり発火する
- DebugLogger でイベント内容が確認できる
- 主要イベントに購読者がいなくても安全に動作する

## Test Plan
- PlayMode: 1P/3P 切替時と Cinematic enter/exit 時の発火確認
- 回帰: イベント購読なし・null 時にエラーが出ないことを確認

## Risks / Notes
- EventBus の責務拡大を避けるため、カメラ関連イベントに限定して導入する

## Milestone
- Phase 2A
