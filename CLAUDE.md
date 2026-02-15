# GLITCH-WORKER (99%Slops)

AI生成ディストピアで猫の社畜がバグだらけの世界を物理で殴って掃除する、グリッチ・イマーシブシム。Unity 6 / URP / PC向け。

## プロジェクト構成

```
99%Slops/                    # リポジトリルート
├── docs/
│   ├── spec/                # 仕様書
│   │   ├── SSOT_CORE_DOCTRINE.md  # 憲章（最上位 SSOT）
│   │   ├── GDD1.0.md        # 統合版 GDD（大型 Instance）
│   │   ├── CAMERA_SYSTEM.md  # カメラ実装仕様（Instance）
│   │   ├── PLAYER_CONTROL_SYSTEM.md # プレイヤー操作仕様（Instance）
│   │   └── SKILL_IDEAS.md   # スキルアイデア集
│   ├── dev/
│   │   ├── ROADMAP_v2.md    # 開発ロードマップ v2
│   │   └── PROJECT_AUDIT.md # 監査レポート
│   ├── tasks/               # 開発タスクチケット
│   ├── tickets/             # QA チケット
│   └── inbox/               # 作業中レポート
├── Assets/_Project/         # 全カスタムアセットはここに配置
│   ├── Scripts/             # C# スクリプト（名前空間: GlitchWorker.*）
│   ├── Prefabs/
│   ├── Materials/
│   ├── Shaders/
│   ├── VFX/
│   ├── Audio/
│   └── Scenes/
├── Packages/
├── ProjectSettings/
└── shared-workflows/        # AI ワークフロー（サブモジュール）
```

## 技術スタック

- **Engine**: Unity 6000.x LTS (URP)
- **Physics**: Unity Physics (PhysX)
- **Input**: Input System (New)
- **Language**: C# (.NET Standard 2.1)

## コーディング規約

| 対象 | 規約 | 例 |
|------|------|-----|
| 名前空間 | `GlitchWorker.{領域}` | `GlitchWorker.Player` |
| クラス名 | PascalCase | `PlayerController` |
| public メソッド | PascalCase | `GrabObject()` |
| private フィールド | _camelCase | `_moveSpeed` |
| SerializeField | _camelCase | `[SerializeField] private float _beamRange` |
| 定数 | UPPER_SNAKE | `MAX_GRAB_DISTANCE` |
| enum | PascalCase | `PropState.Dormant` |

## 文書階層（SSOT ヒエラルキー）

```
SSOT_CORE_DOCTRINE.md        ← 憲章（体験の不変条件・ドキュメント規約）
  └─ GDD1.0.md               ← 大型 Instance（設計の参照集、旧 SSOT）
      ├─ CAMERA_SYSTEM.md     ← 実装仕様 Instance
      ├─ PLAYER_CONTROL_SYSTEM.md ← 実装仕様 Instance
      └─ SKILL_IDEAS.md       ← アイデア・プール
```

矛盾が発生した場合、上位文書を優先する。下位文書の記述は **Invariant / Dial / Instance / Test** で分類すること（詳細は SSOT_CORE_DOCTRINE §5）。

## 重要な設計方針

1. **仕様の最上位ソース**: `docs/spec/SSOT_CORE_DOCTRINE.md` が体験の憲章。`GDD1.0.md` は大型 Instance として参照する。
2. **物理ベース**: すべての挙動は Rigidbody + Collider で制御。アニメーションで誤魔化さない。（Invariant）
3. **ドローン代理操作**: 猫はアイテムに直接触れない。ドローンのトラクタービームで操作する。（Invariant）
4. **3バイオーム基本構成**: Office / Industrial / Nature。ただし Dial として扱い、遠景・近景の多様性は許容する。
5. **過剰設計禁止**: MVP 定義に従う。記載のない機能は実装しない。

## 現在のフェーズ

`docs/dev/ROADMAP_v2.md` を参照。Phase 1（Scaffold）✅ → Phase 2A（Camera）スタブ完了 → Phase 2B（Player Controls）大部分完了。次は Phase 2A 本実装（ICameraMode / CameraSmoother / 1P・3P モード）。

## 関連ドキュメント

- `docs/spec/SSOT_CORE_DOCTRINE.md` — **憲章（最上位 SSOT）**
- `docs/spec/GDD1.0.md` — 統合版 GDD（大型 Instance）
- `docs/spec/CAMERA_SYSTEM.md` — カメラシステム仕様書（Instance）
- `docs/spec/PLAYER_CONTROL_SYSTEM.md` — プレイヤーコントロール仕様書（Instance）
- `docs/spec/SKILL_IDEAS.md` — スキルアイデア集（継続更新）
- `docs/dev/ROADMAP_v2.md` — 開発ロードマップ v2
- `docs/dev/PROJECT_AUDIT.md` — プロジェクト監査レポート（課題・タスクバックログ）
