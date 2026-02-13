# 再開用サマリ (2026-02-12)

## 現在の進捗 (最終更新 2026-02-10)
- リポジトリ最新コミット: d239632 "Fix input/grounding and sync sandbox scene"
- Phase 1: Scaffold 完了
  - Player/Drone/Props/GlitchCannon/GameManager/DebugView
  - Sandbox シーンの基本動作
- Phase 2A: Camera & Foundation 部分的に実装済み
  - `CameraManager` に 1P/3P 切替、スムージング、衝突回避、Cinematic 切替の仕組み
  - `PlayerController` から `CameraManager.HandleLook()` と 1P/3P 切替入力が接続済み
- Phase 2B: Player Controls 実装進行中
  - `PlayerStateMachine` + 複数ステート
  - `PlayerMovement` / `PlayerJump` / `PlayerStats` / `InputBuffer` / `PlayerSlowMotion`
- Sandbox シーン: `Assets/_Project/Scenes/Sandbox.unity`

## 直近で確認すべきこと
- `Sandbox.unity` に `CameraManager` が存在しているか (配置済み)
- `InputSystem_Actions.inputactions` が `PlayerInput` に割当済みか
- CinematicCameraZone は未配置 (必要なら追加)

## 再開手順 (最短)
1. Unity 6000.x LTS (URP) で `99PercentSlops` プロジェクトを開く
2. `Assets/_Project/Scenes/Sandbox.unity` を開く
3. Play で基本挙動を確認
   - `WASD` + `Space` で移動/ジャンプ
   - マウスで視点
   - `V` で 1P/3P 切替
   - `LeftCtrl` Dash、`LeftShift` FastFall (設定済みの場合)
4. 問題があれば `PlayerController` と `CameraManager` の接続を確認

## 次の開発予定 (推奨順)
1. Phase 2A 残タスク
   - 1P/3P のパラメータ最終調整 (既存 `CameraManager` の検証)
   - 1P/3P 切替 UX の調整 (感度/FOV/入力)
   - `CameraSettings` ScriptableObject 化
   - `GameEvents` 導入
   - ドキュメント更新
2. Phase 2B
   - 3P の移動方向/入力の整合
   - InputActions/PhysicsMaterial の仕上げ
3. Phase 3 以降
   - VFX / DebugView / AI 行動

## 注意
- `AI_CONTEXT.md` と `CLAUDE.md` が文字化けして見える可能性あり (エンコーディング再整備の余地)

## 次タスクチェックリスト (2026-02-13)
1. TASK_015_Phase2A_ResumeChecklist.md を基準に再開確認を固定
   - Sandbox.unity の Camera 配線確認
   - InputSystem_Actions の割当確認
2. TASK_013_CameraSettings_SO.md に着手
   - CameraSettings ScriptableObject を定義
   - CameraManager の参照先を Settings 経由に統一
3. TASK_014_GameEvents_Camera.md に着手
   - CameraViewModeChanged, CinematicEntered, CinematicExited を Event 化
   - GameEventDebugLogger で発火検証
4. 受け入れ確認
   - PlayMode で 1P/3P 切替と視点回転、衝突回避を確認
   - Event 重複発火がないことを確認