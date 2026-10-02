# TASK_025_UnityDeferred_Validation_Batch

## Status
BLOCKED (Scene Integration Batch prerequisite is not yet satisfied)

## Tier / Branch
- Tier: 2 (Validation)
- Branch: feature/unity-deferred-validation-batch

## Summary
Scene Integration Batch完了後にTASK_020〜024の手動検証を一括実施し、DONE昇格の証跡をまとめる。

## Dependency
- `docs/WORKFLOW_STATE_SSOT.md` の Scene Integration Batch
- `TASK_021_UploadPort_Objective_Wiring`
- `TASK_022_ResultHUD_Minimal`
- `TASK_023_PlayableLoop_ClearFail_Finalize`
- `TASK_024_Phase5_VerticalSlice_Integration`

## Scope
- SandboxでUploadPort/HUD/Restartの一括検証。
- 実測ログをレポートへ集約。
- `VALIDATING` -> `DONE` 昇格可否を判定。

## Deliverables
- `docs/reports/REPORT_025_UnityDeferred_Validation_Batch.md`
- 関連TASKのStatus更新案（DONE化）

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scenes/Sandbox.unity`, `docs/tasks`, `docs/reports`
- Forbidden: 実装拡張（検証のみ）

## Constraints
- Scene Integration Batch完了後、V-01〜V-06を一つのVerify Batchとして実行する。
- 検証はチェックリスト駆動で漏れを防止。

## Definition of Done (DoD)
- 020〜024系統の手動確認結果が証跡付きで記録される。
- DONE昇格の判断が可能な状態になる。

## Test Plan
- PlayMode:
  - UploadPort進捗更新
  - Cleared/Failed遷移
  - Restart時の進捗/HUDリセット

## Execution Block (Unity復帰後に即実行)
1. `99PercentSlops/Assets/_Project/Scenes/Sandbox.unity` を開く。
2. UploadPort / GameplayLoopController / GameplayHudPresenter の参照欠落がないことをInspectorで確認。
3. Play実行し、下表 V-01〜V-06 を順に実施。
4. `docs/reports/REPORT_025_UnityDeferred_Validation_Batch.md` に結果・証跡（ログ要約）を記録。
5. `TASK_021/022/023/024` の `DONE` 昇格可否を判定して task status 更新案を確定。

## Validation Matrix (最小手動ブロック)
| ID | 手順 | 期待結果 |
| --- | --- | --- |
| V-01 | 正常対象PropをUploadPortへ投入 | Progressが `x/y` で加算される |
| V-02 | 非対象Propを投入 | Progressが増えない |
| V-03 | 必要数到達まで投入 | GameplayStateが `Cleared` へ遷移 |
| V-04 | `Failed` 条件（デバッグキー等）を発火 | GameplayStateが `Failed` へ遷移 |
| V-05 | `Cleared` または `Failed` 後に `R` 入力 | `Playing` に戻りProgress/HUDが初期化される |
| V-06 | クリア後に追加投入を試行 | Progressが上限を超えず誤遷移しない |

## Status Update Rule
- 検証開始時: `TASK_025` を `VALIDATING`。
- V-01〜V-06がすべて PASS: `TASK_020〜025` のAcceptanceを照合し、満たしたTaskを `DONE`。
- FAILが1件以上: 原因ごとの修正Sliceへまとめ、`TASK_025` は `VALIDATING` を継続。

## Milestone
- Phase 5 (Playable Loop)
