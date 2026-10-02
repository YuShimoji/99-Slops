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

## 文書の優先順位

1. ゲーム仕様は `docs/spec/GDD1.0.md`。
2. 不変の開発制約は本ファイル。
3. 現在地・次のスライス・検証ゲートは `docs/WORKFLOW_STATE_SSOT.md`。
4. 監修AIと実装AIの受け渡しは `docs/Windsurf_AI_Collab_Rules_latest.md`。

`AI_CONTEXT.md`、`CLAUDE.md`、`docs/HANDOVER.md`、`docs/MILESTONE_PLAN.md`、`.cursor/MISSION_LOG.md` は互換入口または履歴であり、現在地の正本にしない。`shared-workflows/` の汎用ルールと競合するときは、このリポジトリの上記ルールを優先する。

## 実行原則

- Promptは微作業ではなく、ユーザーが確認できる一つの成果状態（Outcome Slice）を単位にする。
- 実装AIは、その成果に必要な調査、関連修正、限定検証、現在地更新まで連続して進める。可逆でGDD内の小さな曖昧さは妥当な既定値を選び、逐一停止しない。
- 停止するのは、破壊的・復旧困難な変更、依存追加やメジャー更新、DB・認証・外部API契約変更、GDDとの仕様競合またはMVP外拡張、高コストなビジュアル方向を人間が未選択の場合に限る。
- Unityを使えないなど一部検証が保留でも、独立して安全に進められる実装は進める。検証待ちは `VALIDATING` とし、虚偽の `DONE` にもしなければ作業全体の停止理由にもしない。
- 検証は変更リスクに比例させる。文書はリンク・差分、C#はコンパイルと対象確認、Scene/物理/UIはUnity importまたはPlayMode、節目だけ回帰を行う。
- レイアウト、色、フォント、アニメーション、言語対応、隣接コンテンツなど主観的で手戻りの大きい作業は、粗い複数方向を先に提示し、選択後に仕上げる。
- 現在地は作業完了時に `docs/WORKFLOW_STATE_SSOT.md` だけを更新する。開始時・途中の細かな報告のために複数文書を同期しない。

## 関連ドキュメント

- `docs/spec/GDD1.0.md` — 正式 GDD（SSOT）
- `docs/WORKFLOW_STATE_SSOT.md` — 現在地と次の実行スライス（進捗SSOT）
- `docs/Windsurf_AI_Collab_Rules_latest.md` — project-local AI協調開発ルール
- `docs/spec/CAMERA_SYSTEM.md` — カメラシステム仕様書
- `docs/spec/PLAYER_CONTROL_SYSTEM.md` — プレイヤーコントロール仕様書
- `docs/spec/SKILL_IDEAS.md` — スキルアイデア集（継続更新）
- `docs/dev/ROADMAP_v2.md` — 開発ロードマップ v2
- `docs/dev/PROJECT_AUDIT.md` — プロジェクト監査レポート（課題・タスクバックログ）
