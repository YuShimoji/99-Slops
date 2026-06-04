# 再開用サマリ (2026-06-03)

このリポジトリは `master` / `origin/master` に同期して進める。別端末ではまず `git pull origin master` を実行し、Unity 6000.4.9f1 で `99PercentSlops` を開く。

## 現在の進捗
- Phase 5 Vertical Slice はコード上の統合まで到達済み。
- 完成度評価は `docs/tasks/TASK_026_ProjectCompletion_Assessment.md` に固定済みで、総合 80% / 実装 85% / 検証 55%。
- `TASK_020` は `COMPLETED`。
- `TASK_021` / `TASK_022` は `COMPLETED_CORE`。UploadPort と HUD の Unity 配置・Inspector 配線・PlayMode 確認が残る。
- `TASK_023` / `TASK_024` は `COMPLETED`。
- 次の主タスクは `docs/tasks/TASK_025_UnityDeferred_Validation_Batch.md`。

## 今回の同期で増えた前提
| 項目 | 現在状態 | 再開時の意味 |
| --- | --- | --- |
| Unity Editor | `6000.4.9f1` | 別端末もこの版で開くのが最短。初回起動時に ProjectSettings / Package の再生成差分が出ないか確認する。 |
| URP / Packages | URP `17.4.0` 系へ更新済み | `manifest.json` / `packages-lock.json` は更新後を正とする。 |
| プロジェクト指示 | ルート `AGENTS.md` を追加 | Codex / 他 AI / 別端末が最初に読むルールとして使う。 |
| 引き継ぎ | `AI_CONTEXT.md`, `docs/HANDOVER.md`, `docs/WORKFLOW_STATE_SSOT.md` を更新 | 現在地と次の検証入口をファイル内に保持している。 |

## 再開手順
1. `git pull origin master`
2. Unity Hub で Unity `6000.4.9f1` を使い、`99PercentSlops` プロジェクトを開く。
3. Package Manager の復元と Editor 起動後、意図しない追加差分が出ていないか `git status --short` で確認する。
4. `Assets/_Project/Scenes/Sandbox.unity` を開く。
5. `docs/dev/PHASE5_VALIDATION_PREFLIGHT.md` の C-01 から C-07 を確認する。
6. 詰まりがなければ `docs/tasks/TASK_025_UnityDeferred_Validation_Batch.md` の V-01 から V-06 を実施する。
7. 結果を `docs/reports/REPORT_025_UnityDeferred_Validation_Batch.md` に記録し、`TASK_021/022/025` の DONE 昇格可否を決める。

## 直近で確認すべきこと
- `Sandbox.unity` に `UploadPort` があり、Trigger Collider と `UploadPort` コンポーネントが有効か。
- `GameplayHudPresenter` の `_uploadPort`, `_progressText`, `_stateText`, `_restartHintPanel` が欠けていないか。
- Smoke 用の accepted prop が `AI` / `Normalized`、rejected prop が `Human` として準備されているか。
- PlayMode で UploadPort 進捗、Cleared/Failed 遷移、`R` Restart が成立するか。

## 2026-06-03 ローカル検証
| 確認 | 結果 | 次に効くこと |
| --- | --- | --- |
| `git diff --check` | 問題なし | commit 前の空白・パッチ形式の事故は見えていない。 |
| Package JSON 構文 | OK | `manifest.json` / `packages-lock.json` は JSON として読める。 |
| `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo` | 成功、52 warnings / 0 errors | Unity 6000.4.9f1 更新後も C# コンパイルは通る。 |

## 残る不確実性
- この 2026-06-03 同期では Unity Editor の PlayMode 検証は実施していない。
- build warning は主に `MCPForUnity` の obsolete API。加えて `PlayerController.FindFirstObjectByType` の obsolete warning が 1 件ある。
- 別端末の Unity 初回起動後、追加の自動再シリアライズ差分が出る可能性は残る。
