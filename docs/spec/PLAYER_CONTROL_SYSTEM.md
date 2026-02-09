# プレイヤーコントロール仕様書

**ドキュメントバージョン**: 1.1  
**作成日**: 2026-02-08  
**最終更新**: 2026-02-09  
**基準文書**: docs/spec/GDD1.0.md  
**関連文書**: docs/spec/CAMERA_SYSTEM.md  
**ステータス**: GAP 分析反映済み（実装と整合確認完了）

---

## 0. 設計思想

### コアコンセプト

1. **高速移動と緩急**: 基本はハイスピードだが、プレイヤーの任意または特定条件下で **「スローモーション（バレットタイム）」** を発動可能。速さと静寂のコントラストで物理コメディを際立たせる。
2. **物理的自由度**: 「急降下」「コヨーテタイム」など、重力を制御する感覚を重視。猫＋ドローンというキャラクター特性を活かし、通常の人間キャラにはない空中制御を提供する。
3. **スキル拡張型**: 移動能力（ダッシュ、旋回性能等）はスキルツリー（スフィア盤）形式で段階的に開放・強化される。ゲーム開始時は基本移動のみ、進行に伴い物理法則そのものを「改変」する感覚を得られる。
4. **レスポンシブ入力**: 中級者以上が「自分の操作が 100% キャラクターに伝わっている」と感じる設計。先行入力バッファ、キャンセルフレーム、非対称な加減速で操作の気持ちよさを追求する。

### 現状の課題（PROJECT_AUDIT.md より）

| # | 課題 | 深刻度 |
|---|------|--------|
| P-1 | 加速/減速がなく速度が即座に最大値 | 高 |
| P-2 | 空中制御が地上と同一 | 中 |
| P-3 | 斜面での挙動が未処理 | 中 |
| P-4 | 接地判定が甘い（CheckSphere のオフセット） | 低 |
| P-5 | コヨーテタイムがない | 低 |
| P-6 | カメラロジックの混在（→ CAMERA_SYSTEM.md で解決予定） | 中 |
| P-7 | ダッシュ/スプリントが未実装 | 低 |
| P-8 | 落下ダメージが未実装 | 低 |

---

## 1. アーキテクチャ

### 1.1. スクリプト構成

```
Assets/_Project/Scripts/Player/
├── PlayerController.cs        # 入力受付 + 状態管理の Façade（Input System コールバック）
├── PlayerMovement.cs          # 地上移動・空中制御・斜面処理・ダッシュ
├── PlayerJump.cs              # ジャンプ・接地判定・コヨーテタイム・Fast Fall
├── PlayerStateMachine.cs      # State パターンによる状態遷移管理
├── PlayerStats.cs             # 能力値の一元管理（スキル開放で動的変更）
├── PlayerHealth.cs            # HP・落下ダメージ・死亡（Phase 4 以降）
├── PlayerInteraction.cs       # ドローンビーム呼び出し（将来分離候補）
└── InputBuffer.cs             # 先行入力バッファ

Assets/_Project/Data/
├── PlayerBaseStats.asset      # ScriptableObject: 基本パラメータ
├── SkillDefinitions/          # ScriptableObject: 各スキルの定義データ
│   ├── Skill_OrbitalDash.asset
│   ├── Skill_FastFall.asset
│   └── ...
└── PlayerInputConfig.asset    # ScriptableObject: 入力マッピング設定
```

### 1.2. 依存関係

```
PlayerController (Façade)
├── PlayerStateMachine        ← 状態遷移を管理
│   ├── IdleState
│   ├── RunState
│   ├── AirborneState
│   ├── DashState
│   ├── FastFallState
│   ├── SlowMotionState
│   └── (将来: 追加スキル状態)
├── PlayerMovement            ← 物理移動の実行
├── PlayerJump                ← ジャンプ・接地判定
├── PlayerStats               ← 全パラメータの参照元
│   └── PlayerBaseStats (SO)  ← デフォルト値
├── InputBuffer               ← 先行入力キュー
└── CameraManager (外部参照)  ← カメラ前方ベクトルの取得
```

---

## 2. 基本パラメータ

すべてのパラメータは `PlayerStats` クラスを通じて参照する。基本値は `PlayerBaseStats`（ScriptableObject）で定義し、スキル開放やバフ/デバフで実行時に動的変更可能とする。

### 2.1. 移動パラメータ

| パラメータ | 型 | デフォルト | 説明 | スキル影響 |
|-----------|-----|-----------|------|-----------|
| `MoveSpeed` | float | 6.0 | 最大移動速度 (m/s) | ✓ 強化可能 |
| `Acceleration` | float | 40.0 | 地上加速度 (m/s²) | ✓ |
| `Deceleration` | float | 50.0 | 地上減速度（入力なし時） | — |
| `AirAccelMultiplier` | float | 0.3 | 空中での加速度倍率 | ✓ 強化可能 |
| `AirDragMultiplier` | float | 0.1 | 空中での減速度倍率 | — |
| `AirSpeedRatio` | float | 1.0 | 空中最大速度の対地上速度比率（MaxAirSpeed = MoveSpeed × AirSpeedRatio）。デフォルトは同値。スキルで上昇させることでバニーホッピングを段階的に解放 | ✓ |
| `TurnSpeed` | float | 720.0 | キャラクター回転速度 (°/s) | ✓ |

### 2.2. ジャンプパラメータ

| パラメータ | 型 | デフォルト | 説明 | スキル影響 |
|-----------|-----|-----------|------|-----------|
| `JumpForce` | float | 7.0 | ジャンプ力 (Impulse) | ✓ |
| `GravityScale` | float | 2.5 | 通常時の重力倍率 | — |
| `FallGravityScale` | float | 3.5 | 落下時の重力倍率（上昇→落下で切替） | — |
| `MaxFallSpeed` | float | 30.0 | 最大落下速度 | — |
| `CoyoteTime` | float | 0.15 | 崖端からのジャンプ猶予時間 (s) | ✓ 延長可能 |
| `JumpBufferTime` | float | 0.1 | 着地前のジャンプ先行入力受付 (s) | — |

### 2.3. 接地判定

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `GroundCheckMethod` | enum | `SphereCast` | SphereCast / CapsuleCast / Raycast |
| `GroundCheckRadius` | float | 0.3 | チェック半径 |
| `GroundCheckOffset` | float | 0.05 | Collider 底面からのオフセット |
| `GroundLayers` | LayerMask | Default | 接地判定対象レイヤー |
| `SlopeLimit` | float | 45.0 | 歩行可能な最大斜面角度 (°)。ヒステリシス: 進入 >45°、離脱 <40° でジッター防止 |
| `SlopeSlideSpeed` | float | 8.0 | 急斜面での滑落速度 (m/s) |
| `SlopeSlideGravity` | float | 15.0 | 急斜面での滑落加速度 |

### 2.4. ダッシュパラメータ

| パラメータ | 型 | デフォルト | 説明 | スキル影響 |
|-----------|-----|-----------|------|-----------|
| `DashSpeed` | float | 18.0 | ダッシュ速度 (m/s) | ✓ |
| `DashDuration` | float | 0.25 | ダッシュ持続時間 (s) | ✓ |
| `DashCooldown` | float | 0.8 | ダッシュ再使用待ち (s) | ✓ 短縮可能 |
| `DashCharges` | int | 1 | ダッシュの最大チャージ数 | ✓ 増加可能 |
| `DashFollowCamera` | bool | true | ダッシュ方向をカメラ前方に追従 | — |
| `DashTurnRate` | float | 0.0 | ダッシュ中の旋回速度 (°/s)。初期は0（直進のみ） | ✓ 開放で追加 |

### 2.5. スローモーションパラメータ

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `SlowMotionScale` | float | 0.3 | `Time.timeScale` の目標値 |
| `SlowMotionMaxDuration` | float | 5.0 | 最大持続時間 (リアルタイム秒) |
| `SlowMotionGauge` | float | 100.0 | ゲージ最大値 |
| `SlowMotionDrainRate` | float | 20.0 | ゲージ消費速度 (/s) |
| `SlowMotionRechargeRate` | float | 10.0 | ゲージ回復速度（非使用時、/s） |
| `SlowMotionRechargeDelay` | float | 2.0 | 解除後、回復開始までの遅延 (s) |
| `CameraUnscaledSensitivity` | bool | true | カメラ感度を timeScale の影響から除外 |
| `InputUnscaledMode` | bool | true | プレイヤー入力感度を timeScale 非依存にする |

> **実装方針**: `Time.timeScale` を変更しつつ、カメラ回転と入力受付は `Time.unscaledDeltaTime` を使用して感度を一定に保つ。`FixedUpdate` のステップが変わるため、物理演算は `Time.fixedDeltaTime` の動的調整で対応する。

---

## 3. 入力系（Input Mapping）

### 3.1. 標準入力マッピング

| アクション | キーボード | マウス | ゲームパッド | 備考 |
|-----------|-----------|--------|------------|------|
| 移動 | WASD | — | 左スティック | 視点方向に進む |
| カメラ操作 | — | マウス移動 | 右スティック | |
| ジャンプ | Space | ホイールクリック | A/× | |
| 急降下 (Fast Fall) | 空中で Shift | ホイール↓ | 空中で B/○ | 着地硬直キャンセル可 |
| ダッシュ (Orbit型) | 左クリック | 左クリック | LB/L1 | カメラ前方へ加速。入力なし時もカメラ Forward |
| スローモーション | Q / Tab | サイドボタン (MB4/5) | RB/R1 | ゲージ消費型。トグル/ホールド切替可（設定） |
| ドローンビーム発射 | 右クリック | 右クリック | RT/R2 | 旧 Attack から分離。掴み/投げ |
| ドローンビーム解放 | E | — | X/□ | 旧 Interact から分離。掴みの解放 |
| デバッグビュー | F3 | — | — | 旧 Sprint(Shift) から移動 |

### 3.2. マウス片手モード（オプション）

マウス操作だけでキャラクターの移動を制御するモード。アクセシビリティとカジュアルプレイ向け。

| 操作 | 挙動 |
|------|------|
| 左右同時クリック | カメラ正面方向への移動開始（MMO 方式） |
| マウス移動 | カメラ回転（通常通り） |
| 左クリックのみ | ダッシュ |
| 右クリックのみ | ドローンビーム |
| ホイールクリック | ジャンプ |
| ホイール↓ | 急降下 |

> **設計方針**: 片手モードは `PlayerInputConfig`（ScriptableObject）で有効/無効を切替可能。入力マッピング自体を差し替える形で実装し、ゲームロジック側には影響しない。将来的に「画面中央からのドラッグ距離で移動速度・方向を制御するバーチャルスティック」方式もオプションとして検討可能。

### 3.3. 入力系の設定可能項目

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `MouseOneHandMode` | bool | false | マウス片手モードの有効/無効 |
| `MouseForwardTrigger` | enum | `BothClick` | 前進トリガー方式（BothClick / DragStick / Disabled） |
| `MouseDragDeadZone` | float | 30.0 | バーチャルスティック方式時のデッドゾーン (px) |
| `MouseDragMaxRadius` | float | 150.0 | バーチャルスティック方式時の最大半径 (px) |

---

## 4. 特殊移動アクション

### 4.1. Orbit 追従型ダッシュ（Orbital Dash）

ドローンの補助により、カメラの前方ベクトルを基準にキャラクターを瞬間的に加速させる。

#### 基本動作

```
1. ダッシュ入力検出
2. CameraManager から前方ベクトルを取得（Y成分を除去して水平化）
3. DashDuration の間、DashSpeed で前方ベクトル方向に加速
4. 終了後、通常移動に復帰（速度は通常の Deceleration で減衰）
5. DashCooldown 経過後に再使用可能
```

#### スキル拡張

| スキル名 | 効果 | 開放条件 |
|---------|------|---------|
| **Dash Boost** | DashSpeed +20% | 初期スキル |
| **Dash Turn** | ダッシュ中の旋回を許可（DashTurnRate > 0） | Dash Boost 後 |
| **Double Dash** | DashCharges +1 | Dash Turn 後 |
| **Air Dash** | 空中でもダッシュ可能 | Double Dash 後 |
| **Dash Sensitivity** | ダッシュ中のカメラ追従感度アップ | Dash Turn 後 |

#### ドローン連携演出

- ダッシュ発動時、ドローンが一瞬プレイヤー後方に移動し、推進ビームを照射するビジュアルエフェクト
- `DroneController` に `OnDashStart` / `OnDashEnd` イベントを追加し、演出のみを担当（物理演算には影響しない）

### 4.2. Fast Fall（高速急降下）

空中で下方向入力により落下加速度を大幅に引き上げる。スマブラDX 風の高速落下。

#### パラメータ

| パラメータ | 型 | デフォルト | 説明 | スキル影響 |
|-----------|-----|-----------|------|-----------|
| `FastFallMultiplier` | float | 4.0 | 通常落下速度への倍率（3〜5x 範囲） | ✓ |
| `FastFallMaxSpeed` | float | 50.0 | 急降下時の最大落下速度 | ✓ |
| `FastFallEntryMinHeight` | float | 1.0 | 急降下を開始できる最低高度 (m)。下方 SphereCast で地面距離を測定。地面未検出時は許可 | — |
| `LandingLagDuration` | float | 0.133 | 着地硬直時間 (s)。フレーム非依存（旧 LandingLagFrames=8 @60fps 相当） | ✓ 短縮可能 |
| `LandingLagCancelable` | bool | true | 着地硬直をダッシュ/ジャンプでキャンセル可能か | — |

#### 動作フロー

```
1. 空中かつ下方向入力を検出
2. GravityScale を FastFallMultiplier 倍に変更
3. 落下速度が FastFallMaxSpeed を超えないようにクランプ
4. 着地時:
   a. LandingLagCancelable = true の場合:
      - バッファにダッシュ/ジャンプ入力があれば硬直キャンセル → 即次アクション
      - なければ LandingLagFrames 分の着地硬直
   b. false の場合: 常に着地硬直
5. GravityScale を通常に復帰
```

#### 演出

- 急降下開始時: カメラに軽い縦方向のシェイク + FOV を少し広げる
- 着地時: パーティクル（地面の破片）+ 画面揺れ + SE
- キャンセル成功時: キャンセルの「気持ちよさ」を示す小さなヒットストップ（1-2F）

### 4.3. コヨーテタイム & アニメ調復帰

崖から足が離れた後の猶予時間。初心者の理不尽な落下死を防止する救済措置。

#### 基本動作

```
1. 接地状態 → 非接地に遷移した瞬間、CoyoteTimer を開始
2. CoyoteTime 秒以内にジャンプ入力があれば、ジャンプを発動
3. CoyoteTime 経過後は通常の空中状態に遷移
```

#### アニメ調復帰演出

| 要素 | 仕様 |
|------|------|
| アニメーション | 足がバタバタする Looney Tunes 風モーション（コヨーテタイム中のみ再生） |
| 慣性補正 | CoyoteTime 中、崖方向（直前の地面方向）への移動慣性を強く補正し、復帰を助ける |
| 復帰力 | `CoyoteRecoveryForce`: float = 5.0 — 崖方向への引き戻し力 |
| 視覚的ヒント | 猫キャラの目が大きくなる等のリアクション（将来的にアニメーション対応時） |

> **ゲームプレイ的意図**: 物理コメディ作品として「崖際のコヨーテ演出」はトムとジェリー的な笑いの要素にもなる。救済措置としての機能と演出的な面白さを両立する。

### 4.4. スローモーション（バレットタイム）

#### 動作フロー

```
1. スローモーション入力（トグル式 or ホールド式、設定で切替可能）
2. SlowMotionGauge の残量を確認（0 なら発動不可）
3. Time.timeScale を SlowMotionScale へ補間遷移（即座ではなく 0.1s かけて遷移）
4. Time.fixedDeltaTime を timeScale に連動して調整
5. カメラ感度は Time.unscaledDeltaTime ベースで維持
6. ゲージを DrainRate で消費
7. 解除条件: 再入力 / ゲージ枯渇 / 特定イベント
8. 解除時: timeScale を 1.0 へ補間復帰（0.15s）
9. RechargeDelay 後にゲージ回復開始
```

#### スローモーション中の特殊挙動

| 項目 | 挙動 |
|------|------|
| カメラ感度 | `unscaledDeltaTime` 使用で timeScale の影響を受けない |
| 入力受付 | `unscaledDeltaTime` ベースで通常通りのレスポンスを維持 |
| ドローンビーム | 照準精度ボーナス（Raycast の判定を少し甘くする等）— 将来検討 |
| 物理演算 | 全オブジェクトに timeScale が適用される。プレイヤーのみ特別扱いはしない |
| VFX | ポストプロセスで色収差 + ラジアルブラーを軽くかけ、スロー状態を視覚化 |

---

## 5. 状態遷移とキャンセル（State Machine）

### 5.1. プレイヤー状態一覧

```
PlayerStateMachine
├── GroundedStates
│   ├── Idle           … 待機
│   ├── Run            … 移動中
│   ├── Landing        … 着地硬直（LandingLagFrames 分）
│   └── SlideOnSlope   … 急斜面滑落
├── AirborneStates
│   ├── Jump           … ジャンプ上昇中
│   ├── Fall           … 通常落下
│   ├── FastFall       … 急降下
│   └── CoyoteHang     … コヨーテタイム猶予中
├── ActionStates
│   ├── Dash           … ダッシュ中
│   └── (将来: Attack, Interact, etc.)
├── SpecialStates
│   ├── SlowMotion     … バレットタイム（他の状態とオーバーレイ可能）
│   └── (将来: Stunned, KnockBack, etc.)
└── (将来: VehicleState, CutsceneState, etc.)
```

### 5.2. 状態遷移図

```
         ┌──────────────────────────────────────────────────────┐
         │                                                      │
         ▼                                                      │
  ┌──────────┐    移動入力    ┌──────────┐                      │
  │   Idle   │──────────────▶│   Run    │                      │
  │          │◀──────────────│          │                      │
  └────┬─────┘   入力停止     └────┬─────┘                      │
       │                           │                            │
       │    ジャンプ入力            │   ジャンプ入力              │
       ▼                           ▼                            │
  ┌──────────┐              ┌──────────┐                        │
  │   Jump   │──(頂点)────▶│   Fall   │──(着地)──▶ Landing ──┘
  │          │              │          │
  └────┬─────┘              └────┬─────┘
       │                         │
       │    ダッシュ入力          │   下方向入力
       ▼                         ▼
  ┌──────────┐              ┌──────────┐
  │   Dash   │              │ FastFall │──(着地)──▶ Landing
  │          │              │          │
  └──────────┘              └──────────┘

  ※ SlowMotion はオーバーレイ: 他の任意の状態と同時に有効化可能
  ※ CoyoteHang は Fall と同格の独立ステート（Grounded → 歩行離脱時に CoyoteHang → タイマー切れで Fall）
  ※ GlitchCannon 等の外力による離脱（速度 > MoveSpeed×2）時はコヨーテタイム無効 → 直接 Fall
```

### 5.3. キャンセルフレーム

全てのアクションは **Startup（始動）** → **Active（実行）** → **Recovery（回復）** の3フェーズで構成される。Recovery フレームの間、特定の先行入力でアクションを上書き（キャンセル）可能。

| 遷移元状態 (Recovery中) | キャンセル先 | 条件 |
|------------------------|-------------|------|
| Landing | Dash | ダッシュ入力がバッファにある |
| Landing | Jump | ジャンプ入力がバッファにある |
| Dash (Recovery) | Jump | ジャンプ入力 |
| Dash (Recovery) | Dash | DashCharges > 0 |
| FastFall → Landing | Dash | 着地硬直キャンセル |
| FastFall → Landing | Jump | 着地硬直キャンセル |
| (将来) Attack (Recovery) | Dash | 硬直キャンセル |
| (将来) Attack (Recovery) | Jump | 硬直キャンセル |

> **設計原則**: キャンセルは「Recovery フレームのみ」で許可。Startup / Active 中のキャンセルは原則禁止（例外はスキル開放で追加）。これにより「コミット感」と「快適さ」のバランスを取る。

### 5.4. 先行入力バッファ（Input Buffer）

操作が忙しくなる中で入力を取りこぼさないため、先行入力を受け付けるバッファを実装する。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `BufferWindowFrames` | int | 8 | バッファウィンドウの長さ（フレーム数、5〜10F 推奨） |
| `BufferActions` | flags | Jump \| Dash | バッファ対象のアクション |

#### 動作

```
1. アクション入力を検出した瞬間、バッファにアクション種別とタイムスタンプを記録
2. 毎フレーム、バッファ内の入力が BufferWindowFrames 以内か確認
3. 現在の状態が該当アクションを受付可能（= キャンセル可能 or 遷移可能）になった瞬間、
   バッファ内の最も古い入力を消費してアクションを発動
4. BufferWindowFrames を超過した入力はバッファから破棄
```

> **ゲーム体験**: 「ジャンプを押したのに出なかった」「ダッシュが遅れた」という不快感を排除。格闘ゲームやプラットフォーマーで標準的な手法。

---

## 6. PlayerStats — 能力値管理

### 6.1. 設計方針

すべての移動パラメータは `PlayerStats` クラスを経由して参照する。`PlayerBaseStats`（ScriptableObject）が基本値を保持し、スキル開放やバフ/デバフが `Modifier` としてスタックされ、最終的な実効値を算出する。

```csharp
// 概念的な構造（実装時の参考）
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerBaseStats _baseStats;
    
    private Dictionary<string, List<StatModifier>> _modifiers;
    
    public float GetStat(StatType type)
    {
        float baseValue = _baseStats.GetBase(type);
        float modified = ApplyModifiers(baseValue, type);
        return modified;
    }
    
    public void AddModifier(StatType type, StatModifier mod) { ... }
    public void RemoveModifier(StatType type, StatModifier mod) { ... }
}
```

### 6.2. StatModifier の種類

| 種類 | 計算方式 | 用途 |
|------|---------|------|
| `Flat` | base + value | 固定値加算（例: +2.0 m/s） |
| `Percent` | base × (1 + value) | 割合増減（例: +20%） |
| `Override` | value そのもの | 強制上書き（デバッグ用） |
| `Cap` | min(result, value) | 上限設定 |

### 6.3. 主な StatType 一覧

```
MoveSpeed, Acceleration, Deceleration,
AirAccelMultiplier, AirSpeedRatio, TurnSpeed,
JumpForce, CoyoteTime, MaxFallSpeed,
DashSpeed, DashDuration, DashCooldown, DashCharges, DashTurnRate,
FastFallMultiplier, FastFallMaxSpeed, LandingLagDuration,
SlowMotionMaxDuration, SlowMotionDrainRate
```

---

## 7. スキル開放システム（概要）

> **詳細は `docs/spec/SKILL_IDEAS.md` を参照。** 本セクションはプレイヤーコントロールとの接続点のみ記述する。

### 7.1. アーキテクチャ

```
SkillManager
├── SkillTree (データ: ScriptableObject グラフ)
│   └── SkillNode[]
│       ├── SkillDefinition (SO)  … スキルの効果定義
│       ├── Prerequisites[]       … 前提スキル
│       ├── Cost                  … 解放コスト
│       └── Unlocked (runtime)    … 解放済みフラグ
├── PlayerStats への Modifier 追加/削除
└── UI: SkillTreePanel（将来）/ DebugPanel（現段階）
```

### 7.2. 現段階の開発方針

| 項目 | 方針 |
|------|------|
| スキルデータ | `SkillDefinition` (ScriptableObject) で定義。Inspector で編集可能 |
| 開放手段（暫定） | デバッグパネルからトグル式で開放。将来的に UI パネル化 |
| 永続化 | 暫定的に `PlayerPrefs` or JSON。将来セーブシステムに統合 |
| スキルの効果適用 | `PlayerStats.AddModifier()` でパラメータを変更 |
| 新規状態の追加 | スキル開放時に `PlayerStateMachine` に新しい State を登録 |

### 7.3. デバッグパネル連携

```
[DebugPanel]
  ├── スキル一覧表示（名前 + 状態）
  ├── トグルボタンで即時開放/封印
  ├── 現在の実効パラメータ値をリアルタイム表示
  └── パラメータの Override 入力欄（テスト用）
```

> **推奨進行**: まず `PlayerStats` + `StatModifier` の仕組みを構築し、デバッグパネルからパラメータを自由に変更可能にする。スキルツリー UI の実装は Phase 4 以降。

---

## 8. ドローンとの連携

### 8.1. カメラシステムとの統合

- `PlayerMovement` は移動方向の決定に `CameraManager.ActiveCamera.Forward` を参照する。
- `HandleLook()` は `PlayerController` から完全に除去し、`CameraManager` に委譲する（CAMERA_SYSTEM.md 参照）。
- Orbit Dash はカメラの前方ベクトルを基準とするため、三人称/一人称どちらでも同じインターフェースで動作する。

### 8.2. DroneBeam との互換性

| 影響箇所 | 対応方針 |
|---------|---------|
| `DroneBeam._playerCamera` | `CameraManager.ActiveCamera` に差し替え |
| ダッシュ中の Beam 操作 | Dash 状態中は Beam 発射を抑制。`PlayerController.CurrentStateType` を参照して判定（or スキルで解放） |
| FastFall 中の Beam 操作 | FastFall 開始時に `DroneBeam.ForceRelease()` で即時リリース。オブジェクトはプレイヤーの現在速度を継承 |
| SlowMotion 中の Beam | 照準精度ボーナス検討（将来） |

---

## 9. 実装ロードマップ

| Phase | 実装内容 | 依存 |
|-------|---------|------|
| **2A** | カメラ分離（CAMERA_SYSTEM.md）、`PlayerStats` + `PlayerBaseStats` の骨格 | — |
| **2B-1** | `PlayerMovement` リファクタ（加減速、空中制御、斜面処理） | 2A |
| **2B-2** | `PlayerJump` リファクタ（コヨーテタイム、JumpBuffer） | 2B-1 |
| **2B-3** | `PlayerStateMachine` 導入（Idle/Run/Jump/Fall） | 2B-1 |
| **2B-4** | `InputBuffer` 実装 | 2B-3 |
| **2B-5** | Orbital Dash（基本版: 直進のみ） | 2B-3 + 2A |
| **2B-6** | Fast Fall（基本版） | 2B-3 |
| **2B-7** | スローモーション（基本版） | 2B-3 |
| **3** | キャンセルフレーム、マウス片手モード、演出（パーティクル・SE） | 2B 完了 |
| **4** | スキルツリー UI、Dash 拡張スキル群、デバッグパネル統合 | 3 完了 |

---

## 付録 A: 検討事項（決定済み）

| # | 項目 | 決定 | 決定日 |
|---|------|------|--------|
| Q-1 | スローモーションの発動方式 | **トグル＋ホールド切替可**（設定で選択、MVP はトグルをデフォルト） | 2026-02-08 |
| Q-2 | マウス片手モードの前進トリガー | **左右同時クリック**（MVP）→ バーチャルスティック（将来） | 2026-02-08 |
| Q-3 | ダッシュのリソース消費 | **チャージ式**（スキルでチャージ数増加） | 2026-02-08 |
| Q-4 | Fast Fall の着地硬直 | **固定時間 0.133s**（キャンセル可能）。フレーム非依存 | 2026-02-08 |
| Q-5 | コヨーテタイム中の復帰補正の強さ | **強い**（初心者フレンドリー＋物理コメディ演出） | 2026-02-08 |
| Q-6 | Air Dash の実装タイミング | **Phase 4**（スキル開放で追加） | 2026-02-08 |
| Q-7 | バレットタイム中のプレイヤー速度 | **全体スロー**（入力感度のみ unscaledDeltaTime で補正） | 2026-02-08 |

## 付録 B: 実装時の補足決定事項

| # | 項目 | 決定 |
|---|------|------|
| GAP-01 | コヨーテタイム: 外力離脱時の扱い | 地面離脱時の速度が MoveSpeed×2 超ならコヨーテタイム無効 |
| GAP-03 | 斜面判定ジッター防止 | ヒステリシス導入（進入 >45°、離脱 <40°） |
| GAP-05 | 移動入力なし時の Dash 方向 | カメラ Forward をデフォルト方向に使用 |
| GAP-06 | FastFall 最低高度の測定方法 | 下方 SphereCast で地面距離を測定。未検出時は許可 |
| GAP-09 | Dash 中の DroneBeam 抑制方法 | PlayerController.CurrentStateType を公開、DroneBeam が参照 |
| GAP-10 | FastFall 時のオブジェクトリリース | FastFall 開始時に即リリース。オブジェクトはプレイヤーの速度を継承 |
| GAP-11 | MaxAirSpeed の派生化 | MaxAirSpeed = MoveSpeed × AirSpeedRatio（デフォルト 1.0 = 同値） |
| GAP-15 | SlowMotion のステートマシン統合 | 独立コンポーネント（PlayerSlowMotion.cs）。SM のオーバーレイではない |
| GAP-16 | Left Click 入力競合 | Dash=LClick/LB、BeamFire=RClick/RT に分離 |
| GAP-17 | DebugView のキーバインド | F3 に移動（Shift を FastFall に解放） |
| GAP-19 | 壁への引っかかり防止 | PhysicMaterial（摩擦0）をプレイヤーに適用 |

---

## 変更履歴

| 日付 | バージョン | 変更内容 |
|------|-----------|---------|
| 2026-02-08 | 1.0 | 初版作成（たたき台をベースに正式仕様書化） |
| 2026-02-08 | 1.1 | GAP 分析結果を反映: MaxAirSpeed→AirSpeedRatio 派生化、LandingLagFrames→LandingLagDuration 秒数化、CoyoteHang 独立ステート化、入力マッピング競合解消（Dash/BeamFire 分離）、斜面ヒステリシス追加、付録A 全項目決定、付録B 実装補足追加 |
