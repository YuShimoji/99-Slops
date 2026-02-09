# Player/Camera 実装フェーズ計画（2026-02-09）

## 目的
- いったん操作系を `WASD` ベースに戻す。
- 左クリックDashの混同を解消する。
- マウス片手移動案は「将来有効化できる受け口」として残す。
- カメラの敏感さを抑え、1P/3P遷移とシネマティック切替を検証可能にする。

## フェーズ分解

## Phase 1: 入力の正規化（完了）
- `Dash` のキーボード割当を `LeftCtrl` に変更。
- `Dash` の `Mouse LeftButton` 割当を削除（Attackとの競合解消）。
- `TogglePerspective` アクションを追加。
- 既定操作:
  - 移動: `WASD`
  - ジャンプ: `Space`
  - ダッシュ: `LeftCtrl`
  - FastFall: `LeftShift`
  - スローモーション: `Q`
  - 視点切替: `V`

## Phase 2: マウス片手移動の受け口（完了・デフォルトOFF）
- `PlayerController` に `Experimental One-Hand Mouse Move` を追加。
- `Enable Mouse One Hand Move` がONのときのみ、マウスドラッグ量から前進/横移動を合成。
- デフォルトはOFFのため、現状プレイフィールは通常WASDを維持。

## Phase 3: カメラ基盤の実装（完了）
- `CameraManager` を拡張:
  - X/Y感度分離
  - ピッチ制限
  - 回転スムージング（`SmoothDampAngle`）
  - 1P/3Pブレンド遷移
  - 3P時の簡易衝突回避（SphereCast）
- `V` キーで 1P/3P を切替可能。

## Phase 4: シネマティック視点切替（完了）
- `CinematicCameraZone` コンポーネントを追加（`CameraManager.cs` 内）。
- Trigger領域にプレイヤーが入ると指定視点へ切替、出ると復帰。
- 任意オブジェクトに付与可能。

## Phase 5: Sandbox 検証手順（実施手順）
1. シーン内のカメラ管理オブジェクトに `CameraManager` をアタッチ。
2. `Camera Transform` に実カメラを割り当てる。
3. 任意オブジェクトに `BoxCollider(isTrigger=true)` + `CinematicCameraZone` をアタッチ。
4. `Cinematic Point` に固定視点Transformを指定（未指定ならゾーン自身）。
5. 再生して以下を確認:
   - `WASD` で通常移動
   - `LeftCtrl` でDash
   - 左クリックは攻撃のみ
   - `V` で1P/3Pが補間遷移
   - ゾーン侵入でシネマティック視点、退出で元視点に復帰

## 残タスク（次フェーズ）
- 片手マウス移動アルゴリズムの本設計（クリック交互入力・ドラッグスティックの正式仕様化）。
- 3Pカメラの構図制御（肩越し、FOV連動、ドローン照準時オフセット）。
- シネマティックゾーンの優先順位（重複ゾーン、カットシーンロック）。
