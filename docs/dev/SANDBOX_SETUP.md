# Sandbox シーン セットアップガイド

Unityエディタで以下の手順を実行し、プレイアブルモックを構築する。

---

## 1. シーン準備

1. `Assets/_Project/Scenes/` に新しいシーン `Sandbox` を作成
2. デフォルトの Directional Light はそのまま残す

## 2. 地面の作成

1. **3D Object > Plane** を作成。名前: `Ground`
2. Transform: Position(0, 0, 0), Scale(5, 1, 5)
3. Material: 任意の灰色マテリアルを適用

## 3. Player（猫カプセル）の構築

1. **空の GameObject** を作成。名前: `Player`
2. Transform: Position(0, 1, 0)
3. コンポーネント追加:
   - **Capsule Collider** (Height: 2, Radius: 0.5)
   - **Rigidbody** (Freeze Rotation: X, Y, Z をすべて ON)
   - **Player Input** コンポーネント
     - Actions: `InputSystem_Actions` をアサイン
     - Behavior: **Send Messages**
   - **PlayerController** スクリプト
   - **DroneBeam** スクリプト
   - **DebugView** スクリプト
4. Player の子オブジェクトに **Capsule** (メッシュ表示用) を追加。Collider は Remove する。
5. Player の子オブジェクトに **空の GameObject** `CameraHolder` を追加。Position(0, 0.8, 0)。
6. `CameraHolder` の子に **Main Camera** を移動。Position(0, 0, 0)。
7. PlayerController の `Camera Holder` に `CameraHolder` をアサイン。

## 4. Drone の構築

1. **3D Object > Sphere** を作成。名前: `Drone`
2. Transform: Scale(0.3, 0.3, 0.3)
3. Collider を Remove（物理判定不要）
4. コンポーネント追加:
   - **DroneController** スクリプト
     - Follow Target: `Player` をアサイン
5. 子オブジェクトとして **空の GameObject** `BeamOrigin` を追加
6. Player の **DroneBeam** コンポーネント:
   - Beam Origin: `Drone/BeamOrigin` をアサイン
   - Player Camera: Main Camera をアサイン

## 5. AI Prop の配置

1. **3D Object > Cube** を作成。名前: `AIProp_Chair`
2. Transform: Position(3, 0.5, 3)
3. コンポーネント追加:
   - **Rigidbody** (Mass: 1)
   - **AIProp** スクリプト
4. マテリアル3種を作成して AIProp にアサイン:
   - `Mat_AI_Dormant`: 青系（Albedo: #4466AA）
   - `Mat_AI_Hostile`: 赤系（Albedo: #AA3333）, Emission ON
   - `Mat_AI_Normalized`: 灰色（Albedo: #888888）

## 6. Human Prop の配置

1. **3D Object > Sphere** を作成。名前: `HumanProp_Stone`
2. Transform: Position(-2, 0.5, 2), Scale(0.5, 0.5, 0.5)
3. コンポーネント追加:
   - **Rigidbody** (Mass: 0.5)
   - **HumanProp** スクリプト
4. マテリアル: 暖色系（Albedo: #CC8844）

## 7. グリッチ射出装置の配置

1. **空の GameObject** を作成。名前: `GlitchCannon`
2. Transform: Position(0, 0.5, 6)
3. コンポーネント追加:
   - **Box Collider** (Size: 2, 1, 2; **Is Trigger: ON**)
   - **GlitchCannon** スクリプト (Launch Force: 80, Launch Direction: (0, 1, 1))
4. 視覚表現として子に **Cube** を追加。Scale(2, 0.2, 2), Material: マゼンタ系

## 8. GameManager の配置

1. **空の GameObject** を作成。名前: `GameManager`
2. コンポーネント追加:
   - **GameManager** スクリプト

---

## 動作確認チェックリスト

- [ ] Play → 猫カプセルが WASD + マウスで移動・視点操作できる
- [ ] Space でジャンプできる
- [ ] Drone が Player に追従して浮遊している
- [ ] 左クリックで AI Prop を掴める
- [ ] AI Prop を掴むと色が青→赤（Dormant→Hostile）に変わる
- [ ] 左クリックで投擲できる
- [ ] AI Prop に強い衝撃を与えると灰色（Normalized）になり静止する
- [ ] オブジェクトをグリッチ射出装置の上に置く/投げると吹っ飛ぶ
- [ ] 「掴む → 射出装置に投げ込む → 吹っ飛ぶ」の一連フローが動作する

---

*— End of Document —*
