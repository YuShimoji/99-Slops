# GLITCH-WORKER プロジェクト監査レポート

**作成日**: 2026-02-08  
**基準文書**: docs/spec/GDD1.0.md, docs/dev/ROADMAP.md  
**対象**: 99PercentSlops Unity プロジェクト全体

---

## 1. 現在の実装状況サマリー

### 1.1. ROADMAP 対照表

| Phase | 項目 | 状態 | 備考 |
|-------|------|------|------|
| **Phase 1: Scaffold** | Unity プロジェクト構成 | ✅ 完了 | `_Project/` フォルダ構成確立済み |
| | PlayerController | ✅ 完了 | WASD + Space + マウスルック動作 |
| | DroneController | ✅ 完了 | 追従 + ボブ動作 |
| | AIProp（enum + 状態遷移） | ✅ 完了 | Dormant/Hostile/Normalized |
| | GameManager（骨格） | ✅ 完了 | シングルトン + SpawnPoint参照 |
| **Phase 2: Core Mechanics** | DroneBeam | ✅ 完了 | SpringJoint方式、掴み/投擲動作 |
| | AIProp 状態遷移 | ✅ 完了 | 掴み→Hostile、衝撃→Normalized |
| | GlitchCannon | ✅ 完了 | トリガー検知 → AddForce 射出 |
| | NormalizationEffect | ❌ 未実装 | パーティクル/SE 未作成 |
| | Done Definition 全5項目連結 | ⚠️ 部分的 | 個別動作は確認可だが統合フロー未検証 |
| **Phase 3: Polish & Juice** | デバッグ視界 | ⚠️ 骨格のみ | Volume の ON/OFF のみ。ワイヤーフレーム/ハイライト未実装 |
| | 定常化エフェクト | ❌ 未実装 | |
| | 基本 SE | ❌ 未実装 | |
| | カメラワーク | ❌ 未実装 | スムージングすらなし |
| **Phase 4: Playable Loop** | 納品ゲート | ❌ 未実装 | |
| | リザルト画面 | ❌ 未実装 | |
| | 死亡・リスポーン | ❌ 未実装 | |

### 1.2. スクリプト一覧と行数

| スクリプト | 行数 | 名前空間 | 責務 |
|-----------|------|---------|------|
| `PlayerController.cs` | 115 | `GlitchWorker.Player` | 移動 + ジャンプ + **カメラ回転（分離推奨）** |
| `DroneController.cs` | 34 | `GlitchWorker.Drone` | 追従 + ボブ |
| `DroneBeam.cs` | 152 | `GlitchWorker.Drone` | 掴み + 投擲 + 保持 |
| `PropBase.cs` | 47 | `GlitchWorker.Props` | 基底クラス + PropState enum |
| `AIProp.cs` | 83 | `GlitchWorker.Props` | 状態遷移 + マテリアル切替 + 敵対アニメ |
| `HumanProp.cs` | 25 | `GlitchWorker.Props` | スロット + 価値管理 |
| `GlitchCannon.cs` | 48 | `GlitchWorker.Gimmicks` | 射出ギミック |
| `DebugView.cs` | 39 | `GlitchWorker.Systems` | トグルのみ（実質スタブ） |
| `GameManager.cs` | 32 | `GlitchWorker.Systems` | シングルトン骨格のみ |
| **合計** | **575行** | | |

### 1.3. Sandboxシーン

- `Assets/Scenes/Sandbox.unity` と `Assets/_Project/Scenes/Sandbox.unity` が**重複**して存在（要整理）。
- `Assets/Scenes/SampleScene.unity`（Unity デフォルト）も残存。

---

## 2. カメラ関連の課題と対応方針

**詳細仕様**: `docs/spec/CAMERA_SYSTEM.md` を参照。

### 課題一覧

| # | 課題 | 深刻度 | 対応 |
|---|------|--------|------|
| C-1 | スムージングがない（マウス入力が生のまま） | 中 | CameraSmoother 導入（SLerp 補間） |
| C-2 | 一人称固定で三人称視点がない | 高 | ThirdPersonCamera（オービタル方式）の新規実装 |
| C-3 | マウスホイールでの視点距離調整と自動切替がない | 中 | ホイールズーム + 閾値による自動 1P⇔3P 遷移 |
| C-4 | カメラロジックが PlayerController に埋め込み | 中 | CameraManager への責務分離 |
| C-5 | シネマティック定点カメラが未定義 | 新規 | CinematicCameraVolume + 5種の追従モード |
| C-6 | X/Y 軸の感度が独立設定できない | 低 | CameraSettings ScriptableObject |

---

## 3. プレイヤーコントロールの課題と対応方針

### 3.1. 現状の問題点

| # | 課題 | 深刻度 | 詳細 |
|---|------|--------|------|
| P-1 | **移動がリニアで加減速がない** | 中 | `desiredVelocity` を直接 `linearVelocity` に代入しており、加速・減速カーブがない。氷の上を滑るような操作感。 |
| P-2 | **空中制御が地上と同じ** | 中 | ジャンプ中も地上と同じ速度で方向転換できる。物理ベースの空中慣性が効いていない。 |
| P-3 | **斜面での挙動が未処理** | 中 | 急斜面で滑り落ちない、斜面を走ると浮く等の問題がありうる。 |
| P-4 | **接地判定が甘い** | 低 | `CheckSphere` の `origin` が `position + Vector3.down * 0.1f` 固定で、CapsuleCollider の底面と微妙にずれる可能性。 |
| P-5 | **コヨーテタイムがない** | 低 | 崖端でジャンプが効かない「理不尽な落下」が発生しやすい。物理コメディ作品で致命的。 |
| P-6 | **カメラロジックの混在**（C-4 と同一） | 中 | `HandleLook()` が PlayerController 内にあり、責務が混在。 |
| P-7 | **ダッシュ/スプリントが未実装** | 低 | GDD1.0 には明記なし。要否をディレクター判断。 |
| P-8 | **落下ダメージが未実装** | 低 | GDD1.0 §2.7 に「落下死」の記述あり。将来必要。 |

### 3.2. 推奨改善（PlayerController リファクタ）

```
Assets/_Project/Scripts/Player/
├── PlayerController.cs        # 入力受付 + 状態管理のみ（Façade）
├── PlayerMovement.cs          # 地上移動・空中制御・斜面処理
├── PlayerJump.cs              # ジャンプ・接地判定・コヨーテタイム
├── PlayerHealth.cs            # HP・落下ダメージ・死亡（Phase 4）
└── PlayerInteraction.cs       # ドローンビーム呼び出し（将来分離候補）
```

### 3.3. 移動の改善仕様案

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `_moveSpeed` | float | 6.0 | 最大移動速度 |
| `_acceleration` | float | 40.0 | 地上加速度 |
| `_deceleration` | float | 50.0 | 地上減速度（入力なし時） |
| `_airMultiplier` | float | 0.3 | 空中での加速度倍率 |
| `_coyoteTime` | float | 0.15 | 崖端からジャンプ可能な猶予時間（秒） |
| `_jumpBufferTime` | float | 0.1 | 着地前のジャンプ入力バッファ（秒） |
| `_slopeLimit` | float | 45.0 | 歩行可能な最大斜面角度 |
| `_slopeSlideSpeed` | float | 8.0 | 急斜面での滑落速度 |

---

## 4. その他の推奨改善点

### 4.1. コード品質・アーキテクチャ

| # | 課題 | 深刻度 | 推奨対応 |
|---|------|--------|---------|
| A-1 | **イベントシステムが存在しない** | 高 | `GameEvents` 静的クラスまたは ScriptableObject イベントチャンネルの導入。状態遷移通知（Normalized 時のSE再生等）をイベント駆動にする。 |
| A-2 | **DroneBeam に物理とUI両方の責務** | 中 | ビーム描画（LineRenderer 等）の分離。掴みロジックと表現の分離。 |
| A-3 | **AIProp の敵対行動が未実装** | 高 | GDD1.0 §2.3 に4種の行動パターン（排他/粘着/擬態/集団）定義あり。AI行動のステートマシン基盤が必要。 |
| A-4 | **定数がマジックナンバー** | 低 | `_normalizeImpactThreshold = 5f` 等、ScriptableObject 化で調整容易性を上げる。 |
| A-5 | **asmdef（Assembly Definition）が未設定** | 低 | `GlitchWorker.Player.asmdef` 等を作成し、コンパイル分離とビルド時間短縮。 |

### 4.2. ゲームプレイ・ビジュアル

| # | 課題 | 深刻度 | 推奨対応 |
|---|------|--------|---------|
| G-1 | **DebugView が実質未実装** | 高 | URP カスタム Renderer Feature でワイヤーフレーム描画 + AI/Human ハイライト。Phase 3 の主要タスク。 |
| G-2 | **定常化エフェクトがない** | 高 | パーティクル（破片飛散 + 光）+ SE（「パキッ」音）。GDD の「掃除の快感」の核心。 |
| G-3 | **ドローンのビーム視覚表現がない** | 中 | LineRenderer または VFX Graph でビーム描画。掴み状態の視覚フィードバック。 |
| G-4 | **HumanProp に視覚的な差別化がない** | 中 | 暖色ハイライト、近接時のアナログ音キューが GDD §4.3 で定義済み。 |
| G-5 | **AI Prop のHostile時にグリッチシェーダーがない** | 中 | スケール揺らぎのみ。GDD §4.1 の「色収差、頂点のランダム揺らぎ」はシェーダー実装が必要。 |

### 4.3. プロジェクト管理・ドキュメント

| # | 課題 | 深刻度 | 推奨対応 |
|---|------|--------|---------|
| D-1 | **CLAUDE.md のフェーズ表記が不正確** | 低 | 「Phase 2（Scaffold）完了 → Phase 3」とあるが、ROADMAP の Phase 2 = Core Mechanics。Phase 1 = Scaffold。表記を修正。 |
| D-2 | **Sandbox シーンが2箇所に重複** | 低 | `Assets/Scenes/Sandbox.unity` を削除し、`Assets/_Project/Scenes/Sandbox.unity` に一本化。 |
| D-3 | **SampleScene.unity が残存** | 低 | Unity デフォルトシーン。不要なら削除。 |
| D-4 | **GDD に「カメラ」「プレイヤー操作感」の仕様が薄い** | 中 | GDD1.0 への追記または CAMERA_SYSTEM.md / PLAYER_CONTROLS.md として分冊化（推奨）。 |
| D-5 | **AI_CONTEXT.md が存在しない** | 低 | ユーザールールで言及あり。現在のフェーズ・決定事項・Backlog を管理する文書が必要。 |

---

## 5. タスクバックログ（優先度順）

以下を今後の開発タスクとして切り出す。

### Tier 1: 即座に着手（ドキュメント・基盤整備）

| ID | タスク | 分類 | 見積 |
|----|--------|------|------|
| T-001 | CLAUDE.md のフェーズ表記修正 | ドキュメント | 5分 |
| T-002 | 重複シーン整理（SampleScene 削除、Sandbox 統一） | プロジェクト整理 | 10分 |
| T-003 | CAMERA_SYSTEM.md のレビュー・確定 | 仕様策定 | — |
| T-004 | GDD1.0 にカメラシステムの節を追記参照 | ドキュメント | 15分 |
| T-005 | AI_CONTEXT.md の新規作成 | ドキュメント | 20分 |

### Tier 2: Phase 2 完了に必要

| ID | タスク | 分類 | 見積 |
|----|--------|------|------|
| T-010 | PlayerController からカメラロジック分離 → CameraManager | リファクタ | 2h |
| T-011 | CameraSmoother + 一人称カメラ実装 | 実装 | 2h |
| T-012 | 三人称カメラ（オービタル + コリジョン回避） | 実装 | 3h |
| T-013 | マウスホイールズーム + 1P⇔3P 自動切替 | 実装 | 1h |
| T-014 | PlayerMovement リファクタ（加減速 + 空中制御 + コヨーテタイム） | 実装 | 2h |
| T-015 | 接地判定の改善（斜面処理含む） | 実装 | 1h |
| T-016 | GameEvents イベントシステム基盤 | 実装 | 1h |

### Tier 3: Phase 3（ポリッシュ）

| ID | タスク | 分類 | 見積 |
|----|--------|------|------|
| T-020 | NormalizationEffect（パーティクル + SE） | 実装 | 2h |
| T-021 | DebugView 本実装（ワイヤーフレーム + ハイライト） | 実装 | 4h |
| T-022 | ドローンビーム視覚表現（LineRenderer） | 実装 | 1h |
| T-023 | AI Prop グリッチシェーダー（Hostile 時） | 実装 | 3h |
| T-024 | HumanProp 暖色ハイライト + 近接音キュー | 実装 | 2h |
| T-025 | 基本SE一式（定常化、発見、死亡、上司通信） | アセット | 2h |
| T-026 | シネマティックカメラ基盤（Volume + Fixed/LookAt） | 実装 | 3h |

### Tier 4: Phase 4（ゲームループ接続）

| ID | タスク | 分類 | 見積 |
|----|--------|------|------|
| T-030 | 納品ゲート（Upload Port）実装 | 実装 | 3h |
| T-031 | リザルト画面（業務日報 UI） | 実装 | 4h |
| T-032 | 死亡・リスポーンシステム | 実装 | 2h |
| T-033 | 落下ダメージ | 実装 | 1h |
| T-034 | AI Prop 敵対行動（排他/粘着/擬態/集団） | 実装 | 6h |
| T-035 | シネマティックカメラ追従モード拡張（Track/Dolly） | 実装 | 3h |

---

*— End of Document —*
