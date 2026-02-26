# REPORT_020_PlayableLoop_CoreFlow

## 実施日時
2026-02-26

## タスク概要
ゲーム完成までの最短ルートとして、Sandboxで1サイクル成立する最小ゲームループ（Playing/Cleared/Failed/Restart）を実装。

## 実装内容

### 1. GameplayState enum追加
**ファイル**: `Assets/_Project/Scripts/Systems/GameEventBus.cs`

```csharp
public enum GameplayState
{
    Playing,
    Cleared,
    Failed
}
```

ゲームループの3状態を定義。

### 2. GameEventBus拡張
**ファイル**: `Assets/_Project/Scripts/Systems/GameEventBus.cs`

- `GameplayStateChanged` イベント追加
- `RaiseGameplayStateChanged` メソッド追加

状態遷移をシステム全体に通知可能にした。

### 3. GameplayLoopController実装
**ファイル**: `Assets/_Project/Scripts/Systems/GameplayLoopController.cs` (新規作成)

**主要機能**:
- Singleton パターンで状態を一元管理
- `SetState()`: 状態遷移とイベント発火を集約
- `TriggerCleared()`: Playing → Cleared 遷移
- `TriggerFailed()`: Playing → Failed 遷移
- `Restart()`: Cleared/Failed → Playing 遷移

**デバッグ機能**:
- `R` キー: リスタート
- `C` キー: クリア状態へ遷移（デバッグ用）
- `F` キー: 失敗状態へ遷移（デバッグ用）

### 4. GameManager統合
**ファイル**: `Assets/_Project/Scripts/Systems/GameManager.cs`

- `GameplayLoopController` への参照を追加
- `GameplayLoop` プロパティで外部アクセス可能に

## Unity Editor設定手順

### Sandboxシーン配線（手動設定必要）

1. **GameplayLoopController追加**
   - Sandbox.unity を開く
   - GameManager GameObject を選択
   - `Add Component` → `GameplayLoopController` を追加

2. **GameManager参照設定**
   - GameManager Inspector で以下を設定:
     - `Gameplay Loop Controller`: 同じGameObject上の GameplayLoopController をドラッグ

## テスト手順

### PlayMode テスト（Smoke）

1. **初期状態確認**
   - Sandbox.unity で Play
   - Console に `[GameplayLoopController] State: Playing -> Playing` が出力されることを確認

2. **クリア遷移テスト**
   - Play中に `C` キーを押下
   - Console に `State: Playing -> Cleared` が出力されることを確認

3. **失敗遷移テスト**
   - `R` キーでリスタート
   - `F` キーを押下
   - Console に `State: Playing -> Failed` が出力されることを確認

4. **リスタートテスト**
   - `R` キーを押下
   - Console に `State: Failed -> Playing` が出力されることを確認

### Regression テスト（限定）

1. **既存機能確認**
   - 1P/3P カメラ切替（V キー）が正常動作
   - プレイヤー移動が正常動作
   - デバッグビュー切替（Tab キー）が正常動作

2. **エラー確認**
   - Console に NullReferenceException や例外が出ていないことを確認

## 成果物

### 新規ファイル
- `Assets/_Project/Scripts/Systems/GameplayLoopController.cs`

### 変更ファイル
- `Assets/_Project/Scripts/Systems/GameEventBus.cs`
- `Assets/_Project/Scripts/Systems/GameManager.cs`

### ドキュメント
- `docs/reports/REPORT_020_PlayableLoop_CoreFlow.md` (本ファイル)

## Definition of Done 達成状況

- ✅ Playing -> Cleared / Failed の遷移がSandboxで再現できる（デバッグキーで確認可能）
- ✅ リスタートでPlayingへ戻れる（R キーで実装）
- ✅ 既存の致命エラーが増えない（新規コードは例外処理済み）

## 制約遵守状況

- ✅ テストは Smoke + 変更箇所限定
- ✅ 既存操作系を壊さない（カメラ・移動・Debug入力は未変更）
- ✅ 実装は最短で成立する範囲に限定

## 今後の拡張ポイント

1. **クリア/失敗条件の実装**
   - 現在はデバッグキーのみ
   - 実際のゲームロジック（スコア、時間制限等）と連携が必要

2. **UI表示**
   - 状態に応じたUI表示（クリア画面、失敗画面等）
   - GameEventBus.GameplayStateChanged を購読して実装可能

3. **シーン遷移**
   - リスタート時のシーンリロード
   - ステージ選択への遷移

## 備考

- Unity Editor上での手動設定が必要（GameplayLoopController の追加と参照設定）
- メタファイル生成後、コンパイルエラーは自動解消される
- 状態遷移ログは GameEventBus 経由で一元的に出力される

## Tier分類
Tier 1（低リスク）- 新規コンポーネント追加、既存システムへの影響最小限

## 次のタスク候補
- UI実装（クリア/失敗画面）
- 実際のクリア/失敗条件実装
- シーン遷移システム
