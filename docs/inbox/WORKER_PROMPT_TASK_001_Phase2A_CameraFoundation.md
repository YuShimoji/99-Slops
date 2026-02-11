# Worker Prompt: TASK_001_Phase2A_CameraFoundation

## 参照
- チケット: docs/tasks/TASK_001_Phase2A_CameraFoundation.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md（無ければ .shared-workflows/docs/ を参照）
- HANDOVER: docs/HANDOVER.md
- Worker Metaprompt: .shared-workflows/prompts/every_time/WORKER_METAPROMPT.txt
- AI Context: AI_CONTEXT.md
- Mission Log: .cursor/MISSION_LOG.md

## 前提
- Tier: 2
- Branch: feature/task-001-camera-foundation
- Report Target: docs/inbox/REPORT_TASK_001_Phase2A_CameraFoundation_20260211.md
- GitHubAutoApprove: docs/HANDOVER.md の記述を参照
- Milestone: SG-1 / MG-1

## 境界
- Focus Area:
  - `99PercentSlops/Assets/_Project/Scripts/Camera/`
  - `99PercentSlops/Assets/_Project/Scripts/Player/PlayerController.cs`（Camera 呼び出し接続の最小変更のみ）
  - `99PercentSlops/Assets/_Project/Data/`（必要時）
  - `99PercentSlops/Assets/Tests/`（新規テスト）
- Forbidden Area:
  - `shared-workflows/` 配下
  - `99PercentSlops/ProjectSettings/`, `99PercentSlops/Packages/`
  - Camera 外の演出改修（UI/SE/VFX）

## 目的
- `CameraManager` の肥大化した責務を、モード制御・設定データ・補間ロジックに分離し、今後の 3P/Cinematic 拡張で破綻しない土台を作る。
- Unity テスト（EditMode/PlayMode）を追加し、回帰を検知可能にする。

## Test Plan
- テスト対象:
  - `CameraManager` のモード遷移（FirstPerson / ThirdPerson / Cinematic）
  - Camera 衝突解決の最低保証（めり込み回避）
  - `PlayerController` からの Look 入力連携
- EditMode テスト:
  - `CameraModeStateTests`
  - `CameraSmoothingTests`
- PlayMode テスト:
  - `CameraRuntimeSmokeTests`（Sandbox で 1P↔3P 切替と Cinematic Zone 復帰）
- ビルド検証:
  - Unity Editor で C# コンパイルエラー 0
  - 必要時 Windows ターゲット Build Settings 検証
- 期待結果:
  - 既存シーンでカメラ操作破綻なし
  - 追加テストが安定して通過

## Impact Radar
- コード: Camera/Player 接続部に直接影響
- テスト: Camera 系テスト基盤を新規導入
- パフォーマンス: 毎フレーム補間・SphereCast の影響確認
- UX: 視点切替と追従感に直接影響
- 連携: Player 入力系、Cinematic Zone、DebugView 連携
- アセット: Camera 設定 SO を追加する場合は `Assets/_Project/Data/`
- プラットフォーム: PC（Windows）基準

## DoD
- [ ] Camera 基盤の責務分離が完了し、既存挙動を維持できている
- [ ] `CameraManager` の public 利用点が破壊されていない
- [ ] Unity Editor で実機操作確認（1P/3P/Cinematic）を記録した
- [ ] EditMode テストを追加し、全通過結果を記録した
- [ ] PlayMode テストを追加し、全通過結果を記録した
- [ ] C# コンパイルエラー 0 / 必要ビルド検証を記録した
- [ ] `docs/inbox/REPORT_TASK_001_*.md` にレポートを作成した
- [ ] チケット `Report:` 欄にレポートパスを追記した

## 制約
- 仕様追加ではなく、既存挙動を維持した段階的分離を優先する。
- `PlayerController` の public API 互換を維持する。
- テストは主要パスを優先し、網羅テストは後続タスクに分離する。
- パラメータ外部化（ScriptableObject）を優先し、ハードコード増加を避ける。

## 停止条件
- Forbidden Area に触れないと解決できない
- 仕様仮定が3件以上
- 依存追加 / 外部通信が必要で GitHubAutoApprove=true が未確認
- 破壊的操作が必要
- SSOT が取得できない
- Unity固有: ProjectSettings/, Packages/ の変更が必要
- Unity固有: Unity Editor起動が必要な長時間待機

## 納品先
- docs/inbox/REPORT_TASK_001_Phase2A_CameraFoundation_20260211.md

---
Worker Metaprompt の Phase 0〜Phase 4 に従って実行してください。
チャット報告は固定3セクション（結果 / 変更マップ / 次の選択肢）で出力してください。
