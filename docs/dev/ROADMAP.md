# GLITCH-WORKER 開発ロードマップ

**作成日**: 2026-02-08
**基準文書**: docs/spec/GDD1.0.md
**目標**: プレイアブルモック（Vertical Slice）の完成

---

## 1. モック完成の定義（Done Definition）

以下の5項目がSandboxシーン上で動作すること：

1. **猫カプセルが移動・ジャンプできる**（WASD + Space、Rigidbodyベース）
2. **ドローンがオブジェクトを掴み・投擲できる**（トラクタービーム操作）
3. **AI Propが状態遷移する**（Dormant → Hostile → Normalized の視覚変化）
4. **グリッチ射出装置が機能する**（オブジェクト投入 → 超加速で射出）
5. **上記を連結した一連のフローが体験できる**（掴む → 投げる → 吹っ飛ぶ → 定常化）

> **判定基準**: 「猫がドローンでバグった椅子を掴み、射出装置に投げ込んで吹っ飛ばす」映像が15秒で撮影可能な状態（GDD1.0 §9.2準拠）

---

## 2. 開発フェーズ

### Phase 1: Scaffold（基盤構築）

| 項目 | 内容 |
|------|------|
| **目標** | Unityプロジェクト作成、フォルダ構成確立、最小スクリプト骨格 |
| **成果物** | Unityプロジェクト一式、CLAUDE.md、Sandboxシーン（猫カプセルが動く） |
| **スクリプト** | PlayerController, DroneController(空実装), AIProp(enum定義), GameManager(骨格) |
| **完了条件** | シーン再生で猫カプセルがWASD+Spaceで移動・ジャンプする |

### Phase 2: Core Mechanics（コアメカニクス）

| 項目 | 内容 |
|------|------|
| **目標** | ドローンビーム、AI Prop状態遷移、射出装置の実装 |
| **成果物** | 動作するビーム操作、視覚的な状態遷移、射出ギミック |
| **スクリプト** | DroneBeam, AIPropStateMachine, GlitchCannon, NormalizationEffect |
| **完了条件** | Done Definition の項目1〜5がすべて動作する |

### Phase 3: Polish & Juice（手触り向上）

| 項目 | 内容 |
|------|------|
| **目標** | デバッグ視界、定常化エフェクト、基本的なSE、カメラワーク |
| **成果物** | ポストプロセスによるワイヤーフレーム表示、パーティクル、SE再生 |
| **完了条件** | 15秒のバイラル映像が撮影可能。視覚・聴覚フィードバックが快感を伴う |

### Phase 4: Playable Loop（ゲームループ接続）

| 項目 | 内容 |
|------|------|
| **目標** | 納品ゲート、リザルト画面（業務日報）、死亡・リスポーンの基本フロー |
| **成果物** | 1サイクル完結するゲームループ（探索→制圧→収集→納品） |
| **完了条件** | Sandboxシーンで「探索→AI Prop制圧→Human Prop回収→納品ゲート投入→リザルト表示」が一巡する |

> **スコープ外（本ロードマップでは実装しない）**: インベントリシステム、レベル生成、コンポーネントビルド、複数バイオーム、サウンドレイヤー設計、上司通信システム

---

## 3. 技術スタック

| 項目 | 選定 | 理由 |
|------|------|------|
| **エンジン** | Unity 6000.1 LTS | PhysX統合、長期サポート |
| **レンダリング** | URP (Universal Render Pipeline) | ポストプロセス拡張が容易、PC向けに十分な性能 |
| **物理** | Unity Physics (PhysX) | GDD1.0 §8.1準拠。全挙動を物理ベースで制御 |
| **入力** | Input System (New) | クロスプラットフォーム対応、アクションマップ管理 |
| **バージョン管理** | Git + .gitignore (Unity標準) | 既存リポジトリを活用 |

### フォルダ構成案

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Player/          # PlayerController, PlayerInput
│   │   ├── Drone/           # DroneController, DroneBeam
│   │   ├── Props/           # AIProp, HumanProp, PropBase
│   │   ├── Gimmicks/        # GlitchCannon, MimicPad
│   │   ├── Systems/         # GameManager, DebugView
│   │   └── UI/              # (Phase 4以降)
│   ├── Prefabs/
│   │   ├── Player/
│   │   ├── Drone/
│   │   ├── Props/
│   │   └── Gimmicks/
│   ├── Materials/
│   ├── Shaders/
│   ├── VFX/
│   ├── Audio/
│   └── Scenes/
│       └── Sandbox.unity
├── Settings/                 # URP設定、Input Actions
└── Plugins/                  # 外部パッケージ（必要時）
```

---

## 4. リスク項目と対策

GDD1.0 §8.2から抽出。Phase 2〜3で段階的に対処する。

| # | リスク | 深刻度 | 対策 | 対応Phase |
|---|--------|--------|------|-----------|
| 1 | 地形貫通（Non-manifoldメッシュ） | 極高 | モック段階ではプリミティブ形状（Box/Capsule）のみ使用。AI生成メッシュは扱わない | - (モック外) |
| 2 | 物理演算CPU負荷 | 高 | モック段階ではオブジェクト5個以下で検証。アクティブ・ポーリングは後続開発で実装 | - (モック外) |
| 3 | インベントリ内アイテム消失 | 高 | インベントリはモック範囲外。グリッドベース設計はPhase 4以降 | - (モック外) |
| 4 | ドローンビームの物理挙動不安定 | 中 | FixedJoint → 不安定ならConfigurableJoint → 最終手段はTransform直接制御 | Phase 2 |
| 5 | 射出装置の力加減調整 | 低 | AddForceの値をInspectorで調整可能にする（SerializeField） | Phase 2 |

---

## 5. 命名規約

| 対象 | 規約 | 例 |
|------|------|-----|
| 名前空間 | `GlitchWorker.{機能領域}` | `GlitchWorker.Player`, `GlitchWorker.Props` |
| クラス名 | PascalCase | `PlayerController`, `AIProp` |
| public メソッド | PascalCase | `GrabObject()`, `Release()` |
| private フィールド | _camelCase | `_moveSpeed`, `_isGrounded` |
| SerializeField | _camelCase | `[SerializeField] private float _beamRange` |
| 定数 | UPPER_SNAKE | `MAX_GRAB_DISTANCE` |
| enum | PascalCase（型・値とも） | `PropState.Dormant` |

---

*— End of Document —*
