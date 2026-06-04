# AI_CONTEXT.md

## プロジェクトタイプ
Unity プロジェクト (99%Slops / GLITCH-WORKER)

## 現在の再開地点
2026-06-03 時点では `master` 上で、Phase 5 Vertical Slice の実装収束後に Unity 手動検証へ戻る段階です。

## 最新同期で保持したローカル文脈
- `AGENTS.md` をリポジトリルートに追加し、プロジェクト構成、命名規約、正式仕様、MVP 方針を別端末でも読めるようにした。
- Unity Editor を `6000.3.6f1` から `6000.4.9f1` へ更新した状態を保持する。
- `Packages/manifest.json` と `Packages/packages-lock.json` は Unity 6000.4.9f1 によるパッケージ更新後の状態。
- `UniversalRenderPipelineGlobalSettings.asset` は URP 17.4.0 系の再シリアライズ後の状態。
- `EditorBuildSettings.asset` は Unity 6000.4.9f1 による設定項目差分を含む。

## 実装状態
- Phase 5 Vertical Slice はコード上では概ね接続済み。
- `TASK_020` は `COMPLETED`。
- `TASK_021` / `TASK_022` は `COMPLETED_CORE`。Unity 配置と手動検証が残る。
- `TASK_023` / `TASK_024` は `COMPLETED`。
- `TASK_025_UnityDeferred_Validation_Batch.md` が次の主要ブロッカー。

## 次に見る場所
- 再開手順: `docs/dev/RESUME.md`
- 現在地の正本: `docs/WORKFLOW_STATE_SSOT.md`
- Unity 手動検証の手順: `docs/tasks/TASK_025_UnityDeferred_Validation_Batch.md`
- 手動検証前の詰まりどころ: `docs/dev/PHASE5_VALIDATION_PREFLIGHT.md`
- 完成度評価: `docs/tasks/TASK_026_ProjectCompletion_Assessment.md`

## 次の中断可能点
別端末では `git pull origin master` 後、Unity 6000.4.9f1 で `99PercentSlops` を開き、`TASK_025` の V-01 から検証を再開できる。

## ローカル検証
- `git diff --check`: 問題なし。
- `Packages/manifest.json` / `Packages/packages-lock.json`: PowerShell `ConvertFrom-Json` で構文確認済み。
- `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo`: 成功。52 warnings / 0 errors。

## 残る不確実性
- この同期では Unity Editor の PlayMode 検証は実行していない。
- build warning は主に `MCPForUnity` の obsolete API。加えて `PlayerController.FindFirstObjectByType` の obsolete warning が 1 件ある。
- 別端末では Unity 初回起動後に追加の ProjectSettings / Package 差分が出ないか確認する。
