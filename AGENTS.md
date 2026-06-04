# GLITCH-WORKER (99%Slops)

AI生成ディストピアで猫の社畜がバグだらけの世界を物理で殴って掃除する、グリッチ・イマーシブシム。Unity 6 / URP / PC向け。

## プロジェクト構成

```
99%Slops/                    # リポジトリルート
├── docs/
│   ├── spec/                # GDD (Game Design Documents)
│   │   ├── GDD0.1-1         # 初期設計（実装寄り）
│   │   ├── GDD0.1-2         # 補完設計（戦略寄り）
│   │   └── GDD1.0.md        # 統合版 GDD ← 正式仕様
│   └── dev/
│       └── ROADMAP.md       # 開発ロードマップ
├── 99PercentSlops/          # Unity プロジェクト
│   └── Assets/_Project/     # 全カスタムアセットはここに配置
│       ├── Scripts/         # C# スクリプト（名前空間: GlitchWorker.*）
│       ├── Prefabs/
│       ├── Materials/
│       ├── Shaders/
│       ├── VFX/
│       ├── Audio/
│       └── Scenes/
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

## 重要な設計方針

1. **仕様の正式ソース**: `docs/spec/GDD1.0.md` が唯一の正式GDD。改変しないこと。
2. **物理ベース**: すべての挙動は Rigidbody + Collider で制御。アニメーションで誤魔化さない。
3. **ドローン代理操作**: 猫はアイテムに直接触れない。ドローンのトラクタービームで操作する（ハンドIK不要）。
4. **3バイオーム限定**: Office / Industrial / Nature。アセット種類を増やさない。
5. **過剰設計禁止**: GDD1.0 §9.1 の MVP 定義に厳密に従う。記載のない機能は実装しない。

## 現在のフェーズ

`docs/dev/ROADMAP_v2.md` を参照。Phase 1（Scaffold）✅ → Phase 2A（Camera）スタブ完了 → Phase 2B（Player Controls）大部分完了。次は Phase 2A 本実装（ICameraMode / CameraSmoother / 1P・3P モード）。

## 関連ドキュメント

- `docs/spec/GDD1.0.md` — 正式 GDD（SSOT）
- `docs/spec/CAMERA_SYSTEM.md` — カメラシステム仕様書
- `docs/spec/PLAYER_CONTROL_SYSTEM.md` — プレイヤーコントロール仕様書
- `docs/spec/SKILL_IDEAS.md` — スキルアイデア集（継続更新）
- `docs/dev/ROADMAP_v2.md` — 開発ロードマップ v2
- `docs/dev/PROJECT_AUDIT.md` — プロジェクト監査レポート（課題・タスクバックログ）
