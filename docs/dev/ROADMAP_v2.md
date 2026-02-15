# GLITCH-WORKER 開発ロードマップ v2

**作成日**: 2026-02-08
**最終更新**: 2026-02-15
**前版**: docs/dev/ROADMAP.md (v1)
**基準文書**: docs/spec/SSOT_CORE_DOCTRINE.md（憲章）, docs/spec/GDD1.0.md（大型 Instance）, docs/spec/CAMERA_SYSTEM.md, docs/spec/PLAYER_CONTROL_SYSTEM.md
**変更理由**: カメラシステム・プレイヤー操作の仕様追加に伴うフェーズ再構成

---

## 0. 変更サマリー（v1 → v2）

| 項目 | v1 | v2 |
|------|-----|-----|
| フェーズ数 | 4 | 6（Foundation 追加 + Camera/Controls 独立化） |
| カメラ仕様 | Phase 3 の一部（曖昧） | **Phase 2A として独立**。3モード（1P/3P/Cinematic）を段階的に実装 |
| プレイヤー操作 | Phase 1 で完了扱い | **Phase 2B として改善フェーズ**を新設。加減速・空中制御・コヨーテタイム |
| イベントシステム | なし | Phase 2A で基盤構築 |
| シネマティックカメラ | なし | Phase 4 で実装（要塞攻略演出と連動） |
| AI Prop 敵対行動 | Phase 2 に暗黙的に含む | Phase 4 に明示的に配置 |

---

## 1. モック完成の定義（Done Definition）— 据え置き

v1 と同一。以下の5項目が Sandbox シーン上で動作すること：

1. 猫カプセルが移動・ジャンプできる（WASD + Space、Rigidbody ベース）
2. ドローンがオブジェクトを掴み・投擲できる（トラクタービーム操作）
3. AI Prop が状態遷移する（Dormant → Hostile → Normalized の視覚変化）
4. グリッチ射出装置が機能する（オブジェクト投入 → 超加速で射出）
5. 上記を連結した一連のフローが体験できる

> **追加判定基準（v2）**: 三人称視点から上記フローを観察し、「15秒のバイラル映像」が撮影可能な状態。

---

## 2. 開発フェーズ

### Phase 1: Scaffold（基盤構築）— ✅ 完了

| 項目 | 内容 | 状態 |
|------|------|------|
| Unity プロジェクト構成 | フォルダ構成、CLAUDE.md、命名規約 | ✅ |
| PlayerController | WASD + Space + マウスルック | ✅ |
| DroneController + DroneBeam | 追従 + ビーム操作（掴み/投擲） | ✅ |
| PropBase / AIProp / HumanProp | 状態遷移 + マテリアル切替 | ✅ |
| GlitchCannon | 射出ギミック | ✅ |
| GameManager / DebugView | 骨格 | ✅ |

---

### Phase 2A: Camera & Foundation（カメラ基盤 + アーキテクチャ整備） — 部分完了

**目標**: カメラシステムの分離と構築、イベントシステム導入、コード品質の基盤整備。

| # | タスク | 依存 | 見積 | 状態 |
|---|--------|------|------|------|
| 2A-1 | PlayerController からカメラロジック分離 | — | 2h | ✅ CameraManager スタブで分離済 |
| 2A-2 | CameraManager + ICameraMode インターフェース構築 | 2A-1 | 1h | ⭕ スタブ完了。1Pラップのみ。ICameraMode 未導入 |
| 2A-3 | CameraSmoother（SLerp 補間）実装 | — | 1h | — 未着手 |
| 2A-4 | FirstPersonCamera（既存動作の移行 + スムージング適用） | 2A-2, 2A-3 | 1.5h | — 未着手 |
| 2A-5 | ThirdPersonCamera（オービタル + コリジョン回避 + FramingComposer） | 2A-2, 2A-3 | 3h | — 未着手 |
| 2A-6 | マウスホイールズーム + 1P⇔3P 自動切替 | 2A-5 | 1h | — 未着手 |
| 2A-7 | CameraSettings ScriptableObject | 2A-4, 2A-5 | 0.5h | — 未着手 |
| 2A-8 | GameEvents イベントシステム基盤 | — | 1h | — 未着手 |
| 2A-9 | 重複シーン整理 + CLAUDE.md 修正 | — | 0.5h | ✅ CLAUDE.md 修正済 |

**完了条件**:
- 三人称でプレイヤーの周囲を滑らかにオービットできる
- マウスホイールでズームし、一定距離以下で一人称に自動遷移する
- 壁際でカメラがプレイヤーにめり込まない

**見積合計**: 約 11.5h

---

### Phase 2B: Player Controls（プレイヤー操作改善） — 大部分完了

**目標**: 操作の手触りを大幅に改善し、物理コメディに耐える操作感を実現する。

| # | タスク | 依存 | 見積 | 状態 |
|---|--------|------|------|------|
| 2B-1 | PlayerMovement 分離（加速/減速カーブ導入） | 2A-1 | 2h | ✅ |
| 2B-2 | 空中制御の制限（airMultiplier） | 2B-1 | 0.5h | ✅ |
| 2B-3 | コヨーテタイム + ジャンプバッファ + 先行入力バッファ | 2B-1 | 1h | ✅ |
| 2B-4 | 斜面処理（スライド + 歩行制限 + ヒステリシス） | 2B-1 | 1h | ✅ |
| 2B-5 | 接地判定の改善（SphereCast + CapsuleCollider 底面基準） | 2B-1 | 0.5h | ✅ |
| 2B-5b | PlayerStateMachine + 9ステート実装 | 2B-1 | 2h | ✅ 追加実装 |
| 2B-5c | PlayerStats + StatModifier + PlayerBaseStats (SO) | — | 1.5h | ✅ 追加実装 |
| 2B-5d | Orbital Dash（Active→Recovery→キャンセル窓） | 2B-5b | 1h | ✅ 追加実装 |
| 2B-5e | Fast Fall（4x 重力 + 着地ラグ + DroneBeam 連携） | 2B-5b | 1h | ✅ 追加実装 |
| 2B-5f | SlowMotion（トグル/ホールド、ゲージ、timeScale 補正） | — | 1h | ✅ 追加実装 |
| 2B-5g | DroneBeam リマップ（BeamFire/BeamRelease + ForceRelease） | 2B-5d | 0.5h | ✅ 追加実装 |
| 2B-6 | 三人称時のプレイヤー向き制御（移動方向に回転） | 2A-5 | 1h | — 3Pカメラ待ち |
| 2B-7 | 操作感の調整パス（Unity 上でパラメータチューニング） | 2B-1〜6 | 1h | — Unity テスト待ち |

**完了条件**:
- ✅ 崖端でコヨーテタイムが効き、理不尽な落下が起きない（実装済、Unityテスト待ち）
- ✅ 空中で慣性が効き、ジャンプが「気持ちいい」（実装済、チューニング待ち）
- ✅ 急斜面で適切に滑り落ちる（実装済、チューニング待ち）
- — 三人称時に移動方向へプレイヤーが自然に向く（Phase 2A の 3Pカメラ完了待ち）
- ✅ 追加: Orbital Dash / Fast Fall / SlowMotion / InputBuffer / StateMachine 実装済

**見積合計**: 元 7h → 拡張分含め約 14h（実績: 大部分完了）

---

### Phase 3: Polish & Juice（手触り向上）

**目標**: デバッグ視界、定常化エフェクト、SE、ビーム描画を実装し、「掃除の快感」を成立させる。

| # | タスク | 依存 | 見積 |
|---|--------|------|------|
| 3-1 | NormalizationEffect（パーティクル + SE「パキッ」） | 2A-8 | 2h |
| 3-2 | DebugView 本実装（URP Renderer Feature + ワイヤーフレーム） | — | 4h |
| 3-3 | AI/Human 識別ハイライト（DebugView 連動） | 3-2 | 1h |
| 3-4 | ドローンビーム視覚表現（LineRenderer / VFX） | — | 1.5h |
| 3-5 | AI Prop グリッチシェーダー（Hostile 時の色収差 + 頂点揺らぎ） | — | 3h |
| 3-6 | HumanProp 暖色ハイライト + 近接アナログ音キュー | — | 2h |
| 3-7 | 基本 SE 一式（定常化/発見/死亡/上司通信/グリッチ射出） | — | 2h |
| 3-8 | フィードバック演出（着地時カメラシェイク、投擲時 FOV パンチ等） | 2A-4 | 1.5h |

**完了条件**:
- 定常化時に「パキッ」とした快感演出がある
- デバッグ視界でAI Prop（寒色）とHuman Prop（暖色）が視覚的に区別できる
- ドローンビームが光線として見える
- 15秒のバイラル映像が撮影可能

**見積合計**: 約 17h

---

### Phase 4: Cinematic & AI Behavior（シネマティック演出 + 敵 AI）

**目標**: シネマティックカメラと AI Prop の敵対行動を実装し、要塞攻略の体験を構築する。

| # | タスク | 依存 | 見積 |
|---|--------|------|------|
| 4-1 | CinematicCameraVolume 基盤（ボリューム侵入 → 定点カメラ遷移） | 2A-2 | 2h |
| 4-2 | TrackingMode 実装（Fixed / LookAt / LookAtWithLag） | 4-1 | 2h |
| 4-3 | TrackingMode 拡張（Track / Dolly） | 4-2 | 2h |
| 4-4 | シネマティック中のスクリーンスペース入力切替 | 4-1, 2B-6 | 2h |
| 4-5 | AI Prop 敵対行動基盤（行動ステートマシン） | — | 3h |
| 4-6 | 排他的（Exclusive）行動パターン | 4-5 | 1.5h |
| 4-7 | 粘着（Sticky）行動パターン | 4-5 | 1.5h |
| 4-8 | 擬態（Mimic）行動パターン | 4-5 | 2h |
| 4-9 | 集団（Swarm）行動パターン | 4-5 | 2h |
| 4-10 | シネマティック × AI 行動の統合テスト | 4-3, 4-9 | 2h |

**完了条件**:
- Sandbox に CinematicCameraVolume を配置し、定点カメラから AI Prop の動きを観察できる
- 4種の AI 行動パターンがそれぞれ動作する
- 「だるまさんが転んだ」の木が定点カメラから映る演出が成立

**見積合計**: 約 20h

---

### Phase 5: Playable Loop（ゲームループ接続）

**目標**: 1サイクル完結するゲームループを構築する。

| # | タスク | 依存 | 見積 |
|---|--------|------|------|
| 5-1 | 死亡判定（落下 / 圧殺 / 衝突死） | 2B-1 | 2h |
| 5-2 | リスポーンシステム（チェックポイント方式） | 5-1 | 1.5h |
| 5-3 | 上司通信（死亡時の嫌味テキスト表示） | 5-1, 2A-8 | 1h |
| 5-4 | 納品ゲート（Upload Port）実装 | — | 3h |
| 5-5 | リザルト画面 UI（業務日報形式） | 5-4 | 4h |
| 5-6 | HP / ダメージシステム基盤 | — | 2h |
| 5-7 | ゲームループ統合テスト（探索→制圧→収集→納品の1サイクル） | 5-1〜5-5 | 3h |

**完了条件**:
- 「探索 → AI Prop制圧 → Human Prop回収 → 納品ゲート投入 → リザルト表示」が一巡する
- 死亡時にリスポーン＋上司の嫌味が表示される
- 業務日報に回収率・被害額・上司コメントが表示される

**見積合計**: 約 16.5h

---

### Phase 6: Extended Systems（拡張システム）— スコープ外予備

以下は Vertical Slice 完成後に着手する項目。本ロードマップでは計画のみ。

| # | 項目 | 参照 |
|---|------|------|
| 6-1 | インベントリシステム（グリッドベース疑似物理） | GDD1.0 §2.5 |
| 6-2 | コンポーネント・ビルドシステム | GDD1.0 §2.6 |
| 6-3 | モジュラーレベル生成（タグベース） | GDD1.0 §3.2 |
| 6-4 | 缶蹴りシールドメカニクス | GDD1.0 §2.3 |
| 6-5 | 音響三層レイヤー本実装 | GDD1.0 §4.3 |
| 6-6 | 上司ポップアップ通知システム | GDD1.0 §5.1 |
| 6-7 | 追加ギミック（偽トランポリン、だるまさんが転んだ、理想の椅子） | GDD1.0 §3.4 |
| 6-8 | アセット自動コンバーター（Decimation + コリジョン生成 + LOD） | GDD1.0 §8.3 |

---

## 3. フェーズ依存関係図

```
Phase 1 (Scaffold) ✅
    │
    ├── Phase 2A (Camera & Foundation) ⭕ スタブ完了、本実装待ち
    │       │
    │       ├── Phase 2B (Player Controls) ✅ 大部分完了
    │       │       │
    │       │       └── Phase 3 (Polish & Juice)
    │       │               │
    │       └───────────────├── Phase 4 (Cinematic & AI)
    │                       │
    │                       └── Phase 5 (Playable Loop)
    │                               │
    │                               └── Phase 6 (Extended) [スコープ外]
```

---

## 4. 工数サマリー

| Phase | 見積合計 | 累積 |
|-------|---------|------|
| Phase 1 (Scaffold) | — | ✅ 完了 |
| Phase 2A (Camera & Foundation) | 11.5h | 11.5h | スタブ完了、本実装待ち |
| Phase 2B (Player Controls) | 14h (拡張) | 25.5h | 大部分完了 |
| Phase 3 (Polish & Juice) | 17h | 35.5h |
| Phase 4 (Cinematic & AI) | 20h | 55.5h |
| Phase 5 (Playable Loop) | 16.5h | 72h |
| **Vertical Slice 完成まで** | **72h** | |

---

## 5. リスク項目（v1 から引き継ぎ + 追加）

| # | リスク | 深刻度 | 対策 | 対応 Phase |
|---|--------|--------|------|-----------|
| 1 | 地形貫通（Non-manifold メッシュ） | 極高 | プリミティブ形状のみ使用（モック段階） | — (モック外) |
| 2 | 物理演算 CPU 負荷 | 高 | オブジェクト数制限 + 後続でアクティブ・ポーリング | — (モック外) |
| 3 | ドローンビームの物理挙動不安定 | 中 | SpringJoint → ConfigurableJoint → Transform 直接制御 | Phase 1 ✅ |
| 4 | 射出装置の力加減調整 | 低 | SerializeField で Inspector 調整可 | Phase 1 ✅ |
| 5 | **三人称カメラのコリジョン不具合** | 中 | SphereCast + 非対称スムージングで段階的に改善 | Phase 2A |
| 6 | **シネマティックカメラ中の入力混乱** | 中 | スクリーンスペース入力への明示的な切替 + テストケース整備 | Phase 4 |
| 7 | **AI 行動パターンの調整コスト** | 中 | ScriptableObject で行動パラメータを外部化し、Inspector で即調整 | Phase 4 |

---

## 6. 推奨着手順序（次のアクション）

1. **即座**: ✅ T-001（CLAUDE.md 修正）完了、T-002（シーン整理）未実施、T-005（AI_CONTEXT.md 作成）未実施
2. **Phase 2A 本実装**: CameraManager スタブを ICameraMode ベースに拡張、CameraSmoother / 1P / 3P モードを実装
3. **Phase 2B 残件**: 2B-6（三人称向き制御）は 2A-5 完了後、 2B-7（チューニング）は Unity 上で実施
4. **Unity セットアップ**: Player コンポーネント追加、PlayerBaseStats SO 作成、InputActions 更新、PhysicMaterial 適用

---

## 7. ストーリーチャプター型（将来バックログ）

要件要約:
- チャプター駆動（クエスト型）の進行リストを持つ
- メタ進行で解放されるフラグにより、過去チャプター再訪時に会話・攻略ルートが変化する
- チャプター群を包含するメインオーバーワールドを持つ

縦切りスライス（実装促進用）:

| Slice | 目的 | 最小実装 | 出口条件 |
|------|------|---------|---------|
| S1 | 章リスト基盤 | ChapterDefinition / ChapterCatalog | 章選択と章開始が可能 |
| S2 | メタ進行基盤 | MetaFlagService（永続化） | 章外フラグの保存/復元が可能 |
| S3 | オーバーワールド基盤 | OverworldDirector（ハブ） | オーバーワールド⇔章遷移が可能 |
| S4 | 再訪差分基盤 | ChapterVariantResolver | 再訪時の差分会話/分岐ルートが反映 |

対応チケット:
- `docs/tasks/TASK_002_StoryChapter_OverworldFoundation.md`（OPEN）

---

*— End of Document —*
