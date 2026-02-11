# Task: Camera Rotation Tuning（感度・回転フィーリング調整）
Status: OPEN
Tier: 2
Branch: feature/task-003-camera-rotation-tuning
Owner: Unassigned
Created: 2026-02-12T00:05:00+09:00
Report:
Milestone: SG-2 / MG-1

## Objective
- 現在「回転の効きが強い」カメラ挙動を調整し、1P/3P で違和感の少ない回転フィーリングにする。

## Context
- Camera Foundation の責務分離後、機能面は安定したが体感上の感度が高い。
- `CameraSettings` は導入済みのため、コード変更を最小化してパラメータ調整を優先できる。

## Focus Area
- `99PercentSlops/Assets/_Project/Data/DefaultCameraSettings.asset`
- `99PercentSlops/Assets/_Project/Scripts/Camera/CameraManager.cs`（必要最小限）
- `99PercentSlops/Assets/_Project/Scripts/Camera/CameraSmoother.cs`（必要時）

## Forbidden Area
- `shared-workflows/`
- `99PercentSlops/ProjectSettings/`, `99PercentSlops/Packages/`
- Camera以外のプレイヤー制御改修

## Constraints
- まずは値調整（Sensitivity/RotationSmoothTime）を優先する。
- 新機能追加（新入力方式・新カメラモード）は本タスクでは実施しない。

## Test Plan
- **テスト対象**: 1P/3P のマウス視点回転、急旋回時の追従感
- **EditMode テスト**: `CameraSmoothingTests` の既存通過確認
- **PlayMode テスト**: `CameraRuntimeSmokeTests` の既存通過確認
- **手動検証**:
  - Sandbox で 1P: 細かいエイム時の過回転有無
  - Sandbox で 3P: 横振り時の追従ラグと酔い
- **期待結果**:
  - 小入力で過回転しない
  - 大入力で意図どおりに旋回できる

## DoD
- [ ] `DefaultCameraSettings.asset` の調整値を更新
- [ ] Unity Editor で 1P/3P の手動確認結果を記録
- [ ] EditMode / PlayMode 既存テスト通過
- [ ] レポート作成と本チケット `Report:` 更新

## Notes
- 将来のチャプター/オーバーワールド実装前に、基礎操作感を先に固める。
