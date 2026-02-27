# Worker Prompt: TASK_025_UnityDeferred_Validation_Batch

## 参照
- チケット: `docs/tasks/TASK_025_UnityDeferred_Validation_Batch.md`
- SSOT: `.cursor/MISSION_LOG.md`, `docs/WORKFLOW_STATE_SSOT.md`, `docs/MILESTONE_PLAN.md`
- 既存証跡: `docs/reports/REPORT_024_Phase5_VerticalSlice_Integration.md`
- 納品先: `docs/reports/REPORT_025_UnityDeferred_Validation_Batch.md`

## 前提
- Tier: 2 (Validation)
- Branch: feature/unity-deferred-validation-batch
- 目的: Unity復帰後に020-024系統の手動検証を一括実施し、DONE昇格可否を確定する。

## 境界
- Focus: `Assets/_Project/Scenes/Sandbox.unity`, `docs/tasks`, `docs/reports`
- Forbidden: 実装拡張（検証のみ）

## 実行手順（最小ブロック）
1. Sandboxを開き、参照欠落がないことを確認。
2. `TASK_025` の Validation Matrix（V-01〜V-06）を順に実施。
3. PASS/FAILと証跡ログを `REPORT_025` に記録。
4. `TASK_021/022` の `COMPLETED_CORE -> DONE` 可否を判定。
5. 判定結果を task status 更新案として明記。

## DoD
- V-01〜V-06 の結果が全件記録されている。
- DONE昇格可否が説明付きで判断できる。
- 検証対象外の実装変更を含まない。

## 停止条件
- Unity起動不可/Scene破損で検証不能。
- 仕様外の改修がないと検証続行できない。

## 期待アウトプット
- `docs/reports/REPORT_025_UnityDeferred_Validation_Batch.md` 更新
- `docs/tasks/TASK_021_*.md` / `TASK_022_*.md` / `TASK_025_*.md` の状態更新案
