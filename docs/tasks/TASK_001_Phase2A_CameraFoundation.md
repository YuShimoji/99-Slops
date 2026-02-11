# Task: Phase2A Camera Foundation（責務分離 + テスト整備）
Status: DONE
Tier: 2
Branch: feature/task-001-camera-foundation
Owner: Worker-1
Created: 2026-02-11T20:51:30+09:00
Report: docs/inbox/REPORT_TASK_001_Phase2A_CameraFoundation_20260211.md
Milestone: SG-1 / MG-1

## Objective
- `CameraManager` の肥大化した責務を、モード制御・設定データ・補間ロジックに分離し、今後の 3P/Cinematic 拡張で破綻しない土台を作る。
- Unity テスト（EditMode/PlayMode）を最小限でも追加し、Camera 系の回帰を検知できる状態にする。

## Context
- 現在 `99PercentSlops/Assets/_Project/Scripts/Camera/CameraManager.cs` に 1P/3P/Cinematic/Collision/Zone が集約されている。
- `docs/dev/ROADMAP_v2.md` の Phase 2A 残件（分離・スムージング・運用安定化）を前進させる。
- 直近で Player 系が更新されており、Camera 側を同時に安定化させる必要がある。

## Focus Area
- `99PercentSlops/Assets/_Project/Scripts/Camera/`
- `99PercentSlops/Assets/_Project/Scripts/Player/PlayerController.cs`（Camera 呼び出し接続の最小変更のみ）
- `99PercentSlops/Assets/_Project/Data/`（Camera 設定 ScriptableObject を追加する場合）
- `99PercentSlops/Assets/Tests/`（新規テスト）

## Forbidden Area
- `shared-workflows/` 配下（submodule）
- `99PercentSlops/ProjectSettings/`, `99PercentSlops/Packages/`（必要が出たら停止して相談）
- 既存の大規模リネーム/ファイル移動（このタスクでは禁止）
- UI/SE/VFX など Camera 以外の演出系改修

## Constraints
- 仕様追加ではなく、既存挙動を維持した段階的分離を優先する。
- `PlayerController` の public API 互換を維持する（他コンポーネント破壊を避ける）。
- テストは主要パスに限定し、網羅テストは後続タスクに分離する。
- テキスト・数値パラメータは可能な限り外部化（ScriptableObject）し、ハードコード増加を避ける。

## Test Plan
- **テスト対象**:
  - `CameraManager` のモード遷移（FirstPerson / ThirdPerson / Cinematic）
  - Camera 衝突解決の最低保証（めり込み回避）
  - `PlayerController` からの Look 入力連携
- **EditMode テスト**:
  - `CameraModeStateTests`（モード遷移と復帰）
  - `CameraSmoothingTests`（補間の境界値）
- **PlayMode テスト**:
  - `CameraRuntimeSmokeTests`（Sandbox で 1P↔3P 切替と Cinematic Zone 復帰）
- **ビルド検証**:
  - Unity Editor で C# コンパイルエラー 0
  - 必要なら Windows ターゲットで Build Settings の検証
- **期待結果**:
  - 既存シーンでカメラ操作の破綻なし
  - 追加テストが安定して通過し、変更点の回帰検知が可能
- **テスト不要の場合**:
  - 該当なし（本タスクはテスト必須）

## Impact Radar
- **コード**: Camera/Player 接続部に直接影響
- **テスト**: Camera 系テスト基盤を新規導入
- **パフォーマンス**: 毎フレーム処理の補間・SphereCast への影響確認が必要
- **UX**: 視点切替と追従感に直接影響
- **連携**: Player 入力系、Cinematic Zone、今後の DebugView と連携
- **アセット**: Camera 設定用 ScriptableObject を追加する場合は `Assets/_Project/Data/` に生成
- **プラットフォーム**: PC（Windows）での挙動を基準。将来プラットフォーム差異は別タスク

## DoD
- [x] Camera 基盤の責務分離が完了し、既存挙動を維持できている
- [x] `CameraManager` の public 利用点が破壊されていない
- [x] Unity Editor で実機操作確認（1P/3P/Cinematic）を記録した
- [x] EditMode テストを追加し、全通過結果を記録した
- [x] PlayMode テストを追加し、全通過結果を記録した
- [x] C# コンパイルエラー 0 / 必要ビルド検証を記録した
- [x] `docs/inbox/REPORT_TASK_001_*.md` にレポートを作成した
- [x] 本チケットの `Report:` 欄にレポートパスを追記した

## 停止条件
- Forbidden Area の変更が不可避になった場合
- 仕様仮定が 3 件以上必要になった場合
- 依存追加や破壊的Git操作が必要になった場合

## Notes
- Worker は完了時に `.cursor/MISSION_LOG.md` を更新すること。
- 調査だけで終えず、必ず実装差分またはテスト差分を残すこと。
- Orchestrator回収時に Unity Batch 実行でテスト起動を試行したが、プロジェクトを開いている別Unityインスタンス検知で実行不可:
  - `Unity.exe -batchmode ... -runTests -testPlatform EditMode` -> `another Unity instance is running with this project open`
- 2026-02-12: Unity Editor 再コンパイル後、エラーなしを確認（ユーザー確認）。
- フィードバック: カメラ回転感度がまだ強いため、調整タスクは `docs/tasks/TASK_003_CameraRotationTuning.md` へ分離。
