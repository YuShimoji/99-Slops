# Worker Prompt: TASK_015_Camera_Closeout_Integration (Re-Dispatch)

## 参照
- チケット: `docs/tasks/TASK_015_Camera_Closeout_Integration.md`
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`（無ければ `shared-workflows/docs/`）
- HANDOVER: `docs/HANDOVER.md`
- Worker Metaprompt: `shared-workflows/prompts/every_time/WORKER_METAPROMPT.txt`
- Mission Log: `.cursor/MISSION_LOG.md`
- Unity必読:
  - `docs/02_design/ASSEMBLY_ARCHITECTURE.md`
  - `docs/03_guides/UNITY_CODE_STANDARDS.md`
  - `docs/03_guides/COMPILATION_GUARD_PROTOCOL.md`

## 前提
- Tier: 1 (Core)
- Branch: `feature/task-015-camera-closeout-integration`
- Test Phase: Hardening
- Target Assemblies: `GlitchWorker.Runtime`, `Tests.EditMode`, `Tests.PlayMode`
- Report Target: `docs/inbox/REPORT_TASK_015_Camera_Closeout_Integration_20260223_RETRY.md`
- Artifact Target Dir: `docs/inbox/artifacts/TASK_015_20260223/`
- GitHubAutoApprove: `docs/HANDOVER.md` の記述を参照

## 境界
- Focus Area:
  - `Assets/_Project/Scripts/Camera`
  - `Assets/_Project/Scripts/Systems`
  - `Assets/Tests`
- Forbidden Area:
  - `Assets/_Project/Scenes` の大規模リファクタ
  - `Assets/_Project/Scripts/Player` の挙動変更
  - `ProjectSettings/`
  - `Packages/`

## 目的
- `TASK_015` の Hardening ゲートを「証跡ベース」で再確定する。
- 前回報告で不一致だった EditMode/PlayMode/Build を再実行し、実行ログ・結果XML・exit code を提出する。

## 最重要ルール（今回の再実行専用）
- コードレビュー代替は不可。必ず Unity Test Runner / バッチ実行ログを提出すること。
- 1つでも `exit code != 0`、または必須成果物欠落の場合、`COMPLETED` にしない（`IN_PROGRESS` か `BLOCKED`）。
- レポート本文に「成功」と書く場合、該当ログ行と exit code ファイルを併記すること。

## 実行コマンド（必須）
以下をそのまま実行し、生成ファイルを納品すること。

```powershell
$ErrorActionPreference = "Stop"
$UNITY_EXE = "C:\\Program Files\\Unity\\Hub\\Editor\\6000.3.6f1\\Editor\\Unity.exe"
$PROJECT = (Resolve-Path ".").Path
$OUT = "docs/inbox/artifacts/TASK_015_20260223"
New-Item -ItemType Directory -Force -Path $OUT | Out-Null

# 1) EditMode
& $UNITY_EXE -batchmode -projectPath $PROJECT -runTests -testPlatform EditMode -testResults "$OUT/EditMode.xml" -logFile "$OUT/EditMode.log" -quit
$LASTEXITCODE | Out-File "$OUT/EditMode.exitcode.txt" -Encoding ascii
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 2) PlayMode
& $UNITY_EXE -batchmode -projectPath $PROJECT -runTests -testPlatform PlayMode -testResults "$OUT/PlayMode.xml" -logFile "$OUT/PlayMode.log" -quit
$LASTEXITCODE | Out-File "$OUT/PlayMode.exitcode.txt" -Encoding ascii
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 3) Build (Windows64)
& $UNITY_EXE -batchmode -projectPath $PROJECT -buildTarget StandaloneWindows64 -buildWindows64Player "$OUT/TASK_015_SmokeBuild.exe" -logFile "$OUT/Build.log" -quit
$LASTEXITCODE | Out-File "$OUT/Build.exitcode.txt" -Encoding ascii
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
```

## 必須提出物（欠落禁止）
- レポート:
  - `docs/inbox/REPORT_TASK_015_Camera_Closeout_Integration_20260223_RETRY.md`
- アーティファクト:
  - `docs/inbox/artifacts/TASK_015_20260223/EditMode.log`
  - `docs/inbox/artifacts/TASK_015_20260223/EditMode.xml`
  - `docs/inbox/artifacts/TASK_015_20260223/EditMode.exitcode.txt`
  - `docs/inbox/artifacts/TASK_015_20260223/PlayMode.log`
  - `docs/inbox/artifacts/TASK_015_20260223/PlayMode.xml`
  - `docs/inbox/artifacts/TASK_015_20260223/PlayMode.exitcode.txt`
  - `docs/inbox/artifacts/TASK_015_20260223/Build.log`
  - `docs/inbox/artifacts/TASK_015_20260223/Build.exitcode.txt`

## Test Plan
- EditMode:
  - Camera mode transition logic
  - Camera smoothing behavior
  - GameEventBus publish/subscribe for camera events
- PlayMode:
  - 1P/3P switching in Sandbox
  - Cinematic enter/exit
  - Collision avoidance and recovery
- Build:
  - Windows64 向けビルドコマンドで成功終了（exit code 0）
- 期待結果:
  - Hardening基準（EditMode/PlayMode/Build）を全て実行し、再実行可能な証跡が揃うこと

## DoD
- [ ] 1P/3P/Cinematic switching and recovery work correctly
- [ ] Main CameraSettings parameters are editable in Inspector
- [ ] Camera-related event publishing/subscription logs are verified
- [ ] EditMode and PlayMode test results are recorded（XML + LOG）
- [ ] Build結果が記録されている（LOG + exit code）
- [ ] `Unity Editor=コンパイル成功` をレポートに明記
- [ ] 失敗時は cause/repro/mitigation を記録

## Impact Radar
- コード: CameraManager / GameEventBus / DebugLogger の統合品質
- テスト: Camera系 EditMode/PlayMode の回帰検知
- パフォーマンス: カメラ更新と遷移のフレーム安定性
- UX: 1P/3P/Cinematic の体感品質
- 連携: `TASK_003`/`TASK_013`/`TASK_014` の統合整合性

## 停止条件
- Forbidden Area に触れないと解決できない
- 仕様仮定が3件以上必要
- 依存追加 / 破壊的操作が必要
- Unity実行で長時間ロック・権限不整合が発生
- 必須証跡が取得不能

## 納品先
- `docs/inbox/REPORT_TASK_015_Camera_Closeout_Integration_20260223_RETRY.md`

---
Worker Metaprompt の Phase 0〜Phase 4 に従って実行してください。
チャット報告は固定3セクション（結果 / 変更マップ / 次の選択肢）で出力してください。
