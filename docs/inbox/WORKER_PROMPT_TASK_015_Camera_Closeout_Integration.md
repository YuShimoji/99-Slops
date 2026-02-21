# Worker Prompt: TASK_015_Camera_Closeout_Integration

## 参照
- チケット: docs/tasks/TASK_015_Camera_Closeout_Integration.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md（無ければ `shared-workflows/docs/` を参照）
- HANDOVER: docs/HANDOVER.md
- Worker Metaprompt: shared-workflows/prompts/every_time/WORKER_METAPROMPT.txt
- Mission Log: .cursor/MISSION_LOG.md
- Unity必読: docs/02_design/ASSEMBLY_ARCHITECTURE.md, docs/03_guides/UNITY_CODE_STANDARDS.md, docs/03_guides/COMPILATION_GUARD_PROTOCOL.md

## 前提
- Tier: 1 (Core)
- Branch: feature/task-015-camera-closeout-integration
- Test Phase: Hardening
- Target Assemblies: GlitchWorker.Runtime, Tests.EditMode, Tests.PlayMode
- Report Target: docs/inbox/REPORT_TASK_015_Camera_Closeout_Integration_20260220.md
- GitHubAutoApprove: docs/HANDOVER.md の記述を参照

## 境界
- Focus Area:
  - Assets/_Project/Scripts/Camera
  - Assets/_Project/Scripts/Systems
  - Assets/Tests
- Forbidden Area:
  - Assets/_Project/Scenes の大規模リファクタ
  - Assets/_Project/Scripts/Player の挙動変更
  - ProjectSettings/
  - Packages/

## 目的
- `TASK_003` / `TASK_013` / `TASK_014` の統合結果を最終検証し、MG-1 Camera closeoutを完了条件まで持っていく。
- 1P/3P/Cinematic遷移と回復、イベントログ、テスト、ビルドの証跡をレポートへ確定させる。

## Test Plan
- テスト対象:
  - Camera mode transition logic
  - Camera smoothing behavior
  - GameEventBus publish/subscribe for camera events
- EditMode:
  - Camera mode transition logic
  - Camera smoothing behavior
  - GameEventBus publish/subscribe for camera events
- PlayMode:
  - 1P/3P switching in Sandbox
  - Cinematic enter/exit
  - Collision avoidance and recovery
- Build:
  - dev target build/compile verification
- 期待結果:
  - Hardening基準として EditMode/PlayMode/Build 全ての結果が記録され、再現手順付きで確認可能

## DoD
- [ ] 1P/3P/Cinematic switching and recovery work correctly
- [ ] Main CameraSettings parameters are editable in Inspector
- [ ] Camera-related event publishing/subscription logs are verified
- [ ] EditMode and PlayMode test results are recorded
- [ ] If failures occur, cause/repro/mitigation are recorded
- [ ] Unity Editor compile/build result is recorded (`Unity Editor=コンパイル成功` を含む)

## Impact Radar
- コード: CameraManager / CameraSettings / GameEventBus の挙動確定
- テスト: Camera系回帰の基準値更新
- パフォーマンス: camera transition/smoothingの回帰確認
- UX: 視点切替と復帰の体感品質に直結
- 連携: TASK_003 / 013 / 014 由来の統合不整合検出

## 制約
- No breaking changes to public API
- No regressions in existing gameplay behavior
- Keep diff size minimal and targeted

## 停止条件
- Forbidden Area に触れないと解決できない
- 仕様仮定が3件以上
- 依存追加 / 外部通信が必要で GitHubAutoApprove=true が未確認
- 破壊的操作が必要
- SSOT が取得できない
- Unity test environment lock でテスト実行不能
- Unity固有: ProjectSettings/, Packages/ の変更が必要
- Unity固有: Unity Editor起動が必要な長時間待機

## 納品先
- docs/inbox/REPORT_TASK_015_Camera_Closeout_Integration_20260220.md

---
Worker Metaprompt の Phase 0〜Phase 4 に従って実行してください。
チャット報告は固定3セクション（結果 / 変更マップ / 次の選択肢）で出力してください。
