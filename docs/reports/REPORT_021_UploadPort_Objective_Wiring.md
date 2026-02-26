# REPORT_021_UploadPort_Objective_Wiring

## Task Reference
- Task ID: TASK_021
- Branch: feature/uploadport-objective-wiring
- Tier: 1 (Core)
- Status: COMPLETED

## Summary
最小ゲームループ成立に必要な「目標達成点（UploadPort）」を実装し、クリア判定へ接続しました。

## Implementation Details

### 1. UploadPort.cs
**Location**: `99PercentSlops/Assets/_Project/Scripts/Gimmicks/UploadPort.cs`

**Features**:
- OnTriggerEnterで対象オブジェクト（PropBase）を検出
- PropTypeとPropStateによる受け入れ条件判定
- 受け入れ成功時に進捗カウントを加算
- 必要数到達でGameplayLoopController.TriggerCleared()を呼び出し
- GameplayRestartedイベントを購読して進捗を自動リセット

**Configuration**:
- `_requiredCount`: 必要な投入数（デフォルト: 3）
- `_acceptedPropType`: 受け入れるPropType（デフォルト: AI）
- `_acceptedPropState`: 受け入れるPropState（デフォルト: Normalized）

### 2. GameEventBus拡張
**Location**: `99PercentSlops/Assets/_Project/Scripts/Systems/GameEventBus.cs`

**Changes**:
- `GameplayRestarted` イベントを追加
- `RaiseGameplayRestarted()` メソッドを追加

### 3. GameplayLoopController拡張
**Location**: `99PercentSlops/Assets/_Project/Scripts/Systems/GameplayLoopController.cs`

**Changes**:
- `Restart()` メソッド内で `GameEventBus.RaiseGameplayRestarted()` を呼び出し
- リスタート時に全システムへ通知を送信

## Integration Points
- **GameplayLoopController**: クリア判定の通知先
- **GameEventBus**: リスタート時の進捗リセット通知
- **PropBase**: 受け入れ対象の判定基準

## Test Plan Status
以下のテストは手動検証が必要です（Sandbox.unityへの配置後）:

### Smoke Tests
- [ ] 対象投入で進捗が増える
- [ ] 必要数到達でクリアになる
- [ ] 対象外では進捗が増えない
- [ ] リスタート時に進捗がリセットされる

## Remaining Work
- Sandbox.unityへのUploadPort配置
- 手動テストによる動作検証
- 必要に応じてパラメータ調整

## Notes
- 判定条件は厳しめに設定（PropType + PropState の両方一致が必須）
- デバッグログは `_enableDebugLogs` フラグで制御可能
- 受け入れ成功時に対象オブジェクトは自動的にDestroy

## Dependencies Met
- TASK_020_PlayableLoop_CoreFlow: GameplayLoopControllerが既存

## Risks Mitigated
- 誤検知防止: PropTypeとPropStateの両方をチェック
- リスタート時の状態不整合: GameplayRestartedイベントで自動リセット

## Deliverables
- [x] `UploadPort.cs` 実装完了
- [x] `GameEventBus.cs` 拡張完了
- [x] `GameplayLoopController.cs` 拡張完了
- [ ] `Sandbox.unity` 配置（手動作業）
- [x] Worker report 作成

## Conclusion
UploadPortのコア機能は実装完了。Unity Editor上での配置と検証が次のステップです。
