# TASK_023_PlayableLoop_ClearFail_Finalize

## Status
COMPLETED

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/playable-loop-clearfail-finalize

## Summary
Phase 5の完成最短ルートとして、Cleared/Failed確定条件を一本化し、ループ成立の品質を安定化する。

## Dependency
- `TASK_020_PlayableLoop_CoreFlow`
- `TASK_021_UploadPort_Objective_Wiring`

## Scope
- Cleared条件とFailed条件の判定責務をGameplayLoopControllerへ集約。
- UploadPort達成時の状態遷移を最終形に固定。
- 失敗条件（最小版）を導入し誤遷移を防止。

## Deliverables
- `99PercentSlops/Assets/_Project/Scripts/Systems/GameplayLoopController.cs`
- `99PercentSlops/Assets/_Project/Scripts/Gimmicks/UploadPort.cs`（連携最小修正）
- `docs/reports/REPORT_023_PlayableLoop_ClearFail_Finalize.md`

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Systems`, `Assets/_Project/Scripts/Gimmicks`
- Forbidden: UI大改修、非必須演出、別シーン展開

## Constraints
- コンパイル通過を必須。
- テストは Smoke + 変更箇所限定。
- Unity手動検証は deferred 記載で進行可。

## Definition of Done (DoD)
- 状態遷移が単一責務に収束している。
- Cleared/Failed誤遷移が起きない実装になっている。
- dotnet build でエラーがない。

## Test Plan
- Compile: `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo`
- Code review: 遷移条件の重複・分散がないことを確認

## Milestone
- Phase 5 (Playable Loop)
