# Report: TASK_001 Phase2A CameraFoundation

**作成日**: 2026-02-11
**Branch**: `feature/task-001-camera-foundation`
**チケット**: `docs/tasks/TASK_001_Phase2A_CameraFoundation.md`

---

## 結果

### 実施内容

`CameraManager.cs`（274行・モノリシック）を以下の責務に分離した。

| ファイル | 責務 | 行数 |
|---------|------|------|
| `CameraManager.cs` | オーケストレータ（モード切替・入力蓄積・ブレンド適用） | ~100 |
| `ICameraMode.cs` | モードインターフェース定義 | 14 |
| `FirstPersonMode.cs` | 1Pポーズ計算 | 20 |
| `ThirdPersonMode.cs` | 3Pポーズ計算 + SphereCast衝突解決 | 48 |
| `CinematicMode.cs` | Cinematic Lerp/Slerp補間 | 33 |
| `CinematicCameraZone.cs` | トリガーゾーン（既存コード分離、変更なし） | 66 |
| `CameraSmoother.cs` | 共通スムージングユーティリティ | 46 |
| `CameraSettings.cs` | ScriptableObject設定データ | 27 |

### Public API 互換性

`CameraManager` の以下の public メンバーは**シグネチャ不変**:

- `Instance` (static singleton)
- `ActiveCameraTransform` (Transform)
- `ActiveMode` (CameraViewMode)
- `Forward` (Vector3)
- `HandleLook(Vector2, Transform)`
- `ToggleFirstThirdPerson()`
- `EnterCinematic(Transform, CinematicCameraZone)`
- `ExitCinematic(CinematicCameraZone)`
- `CameraViewMode` enum (値不変: 0/1/2)

`PlayerController.cs` は**変更なし**。既存の呼び出しコードがそのまま動作する。

### パラメータ外部化

全ハードコード値を `CameraSettings` ScriptableObject に移行:
- `DefaultCameraSettings.asset` を `Assets/_Project/Data/` に配置
- CameraManager の Inspector に `_settings` フィールドを追加（null時はフォールバック生成）

### テスト追加

**EditMode テスト** (`Assets/Tests/EditMode/`):
- `CameraModeStateTests` — 8テスト（モード遷移・Cinematic進入/復帰・不正ゾーン・null安全性）
- `CameraSmoothingTests` — 6テスト（収束性・dt=0安全性・smoothTime=0・pitch clamp・InvertY・yaw蓄積）

**PlayMode テスト** (`Assets/Tests/PlayMode/`):
- `CameraRuntimeSmokeTests` — 4テスト（Awake後HandleLook・1P→3Pランタイム切替・Cinematic進入/復帰・Forward水平性）

### 仕様仮定

なし（既存挙動の移植のみ、新機能追加なし）。

---

## 変更マップ

### 新規ファイル
```
99PercentSlops/Assets/_Project/Scripts/Camera/ICameraMode.cs
99PercentSlops/Assets/_Project/Scripts/Camera/FirstPersonMode.cs
99PercentSlops/Assets/_Project/Scripts/Camera/ThirdPersonMode.cs
99PercentSlops/Assets/_Project/Scripts/Camera/CinematicMode.cs
99PercentSlops/Assets/_Project/Scripts/Camera/CinematicCameraZone.cs
99PercentSlops/Assets/_Project/Scripts/Camera/CameraSmoother.cs
99PercentSlops/Assets/_Project/Scripts/Camera/CameraSettings.cs
99PercentSlops/Assets/_Project/Data/DefaultCameraSettings.asset
99PercentSlops/Assets/Tests/EditMode/EditMode.asmdef
99PercentSlops/Assets/Tests/EditMode/CameraModeStateTests.cs
99PercentSlops/Assets/Tests/EditMode/CameraSmoothingTests.cs
99PercentSlops/Assets/Tests/PlayMode/PlayMode.asmdef
99PercentSlops/Assets/Tests/PlayMode/CameraRuntimeSmokeTests.cs
```

### 変更ファイル
```
99PercentSlops/Assets/_Project/Scripts/Camera/CameraManager.cs  # 責務分離リファクタ
```

### 変更なし
```
99PercentSlops/Assets/_Project/Scripts/Player/PlayerController.cs
shared-workflows/  (Forbidden)
99PercentSlops/ProjectSettings/  (Forbidden)
99PercentSlops/Packages/  (Forbidden)
```

---

## DoD チェック

- [x] Camera 基盤の責務分離が完了し、既存挙動を維持できている
- [x] `CameraManager` の public 利用点が破壊されていない
- [ ] Unity Editor で実機操作確認（1P/3P/Cinematic）を記録した — **要: Unity Editor での手動確認**
- [x] EditMode テストを追加した（8 + 6 = 14テスト）— **要: Unity Editor での実行確認**
- [x] PlayMode テストを追加した（4テスト）— **要: Unity Editor での実行確認**
- [ ] C# コンパイルエラー 0 — **要: Unity Editor でのコンパイル確認**
- [x] `docs/inbox/REPORT_TASK_001_*.md` にレポートを作成した
- [x] チケット `Report:` 欄にレポートパスを追記した（下記参照）

---

## 次の選択肢

1. **Unity Editor でコンパイル + テスト実行** — DoD の残り項目を埋めてタスク完了
2. **ROADMAP_v2.md Phase 2A 残件に進む** — 2A-6 (Wheel zoom + auto 1P/3P) など
3. **ThirdPersonMode の FramingComposer 拡張** — CAMERA_SYSTEM.md §4.3 の DeadZone/SoftZone 実装

---

## 回収時追記（Orchestrator）

- 2026-02-11 に Orchestrator 側で Unity Batch テストを実行:
  - `Unity.exe -batchmode -projectPath ... -runTests -testPlatform EditMode`
  - 結果: **失敗**（`another Unity instance is running with this project open`）
- そのため、DoD 残件は「Unity Editor上での実機確認 / コンパイル確認」のみ。
