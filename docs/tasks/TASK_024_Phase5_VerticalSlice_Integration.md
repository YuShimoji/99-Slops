# TASK_024_Phase5_VerticalSlice_Integration

## Status
COMPLETED (2026-02-27)

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/phase5-verticalslice-integration

## Summary
最小ゲームループ（020/021/022）を1本のVertical Sliceとして統合し、完成直結の進行基盤を確定する。

## Dependency
- `TASK_020_PlayableLoop_CoreFlow`
- `TASK_021_UploadPort_Objective_Wiring`
- `TASK_022_ResultHUD_Minimal`

## Scope
- ループ・目標・HUDの結線をコード側で整合。
- 初期化/再開時の状態リセット連携を統一。
- 必要なイベント購読解除とnull-safeを最終確認。

## Deliverables
- `99PercentSlops/Assets/_Project/Scripts/Systems/GameplayLoopController.cs`
- `99PercentSlops/Assets/_Project/Scripts/Gimmicks/UploadPort.cs`
- `99PercentSlops/Assets/_Project/Scripts/UI/GameplayHudPresenter.cs`
- `docs/reports/REPORT_024_Phase5_VerticalSlice_Integration.md`

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scripts/Gimmicks`, `Assets/_Project/Scripts/UI`
- Forbidden: 新機能拡張、アート・演出リッチ化、仕様外の最適化

## Constraints
- 完成最短ルート優先（追加要求は次チケットへ分離）。
- コンパイル通過を必須。
- Unity上の配置確認は deferred 管理可。

## Definition of Done (DoD)
- 3コンポーネント間の主要イベント連携が破綻しない。
- Restart時の進捗/UI状態が初期化される実装である。
- dotnet build でエラーがない。

## Test Plan
- Compile: `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo`
- Regression (code): 主要イベント購読/解除の対応確認

## Milestone
- Phase 5 (Playable Loop)
