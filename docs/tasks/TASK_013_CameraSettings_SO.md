# TASK_013_CameraSettings_SO

## Status
OPEN

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/camera-settings-so

## Summary
Camera 設定を ScriptableObject に分離し、`CameraManager` が設定アセットを参照して動作する形に統一する。

## Dependency
- `TASK_015_Phase2A_ResumeChecklist` の確認項目を満たしてから着手

## Scope
- `CameraSettings` ScriptableObject を `Assets/_Project/Data` に追加
- `CameraManager` の各種パラメータ参照を Settings 経由へ置換
- 既存 Inspector 値の移行と初期値整備

## Deliverables
- `Assets/_Project/Scripts/Camera/CameraSettings.cs`
- `Assets/_Project/Data/CameraSettings_Default.asset`（名称は実装時に調整可）
- `Assets/_Project/Scripts/Camera/CameraManager.cs` の Settings 参照化

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Camera`
- Forbidden: `Assets/_Project/Scenes` の大規模変更、InputActions の改変

## Constraints
- 既存の視点挙動を壊さない
- Settings 未割当時の安全なフォールバックを用意する

## Definition of Done (DoD)
- `CameraSettings` から Inspector で全主要項目を調整できる
- `CameraManager` が Settings 参照のみで動作する
- Settings 差し替えで挙動が変化することを確認できる
- PlayMode で 1P/3P の基本挙動が維持される

## Test Plan
- PlayMode: 1P/3P の切替、感度、距離、衝突回避を確認
- 回帰: Settings 未設定時でもエラーなく動作することを確認
- 手動確認: Settings アセット差し替えで挙動差分が出ることを確認

## Risks / Notes
- Settings の粒度を細かくしすぎると Inspector が複雑化する

## Milestone
- Phase 2A
