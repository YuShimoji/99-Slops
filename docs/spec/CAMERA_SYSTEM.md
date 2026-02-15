# カメラシステム仕様書

> **文書ステータス: 実装仕様 Instance**
> 本文書は `SSOT_CORE_DOCTRINE.md` > `GDD1.0.md` の下位に位置する実装仕様である。
> 体験の不変条件（SSOT_CORE_DOCTRINE §1--§3）と矛盾する場合、**SSOT_CORE_DOCTRINE を優先**。
> 本文書の具体的パラメータ・構成は Instance（現段階の標準案）として扱う。

**ドキュメントバージョン**: 0.1
**作成日**: 2026-02-08
**基準文書**: docs/spec/SSOT_CORE_DOCTRINE.md, docs/spec/GDD1.0.md
**ステータス**: ドラフト（レビュー待ち）

## 実装状況（2026-02-09）

- 実装済み:
  - `Assets/_Project/Scripts/Camera/CameraManager.cs` にて
    - マウス感度のX/Y分離
    - 回転スムージング
    - 一人称/三人称の遷移
    - シネマティックゾーン切替
  - `Assets/_Project/Scripts/Player/PlayerController.cs` から `OnTogglePerspective` で視点切替
  - `Assets/InputSystem_Actions.inputactions` に `TogglePerspective` を追加
- 運用ドキュメント:
  - `docs/dev/PLAYER_CAMERA_PHASED_IMPLEMENTATION_2026-02-09.md`
- 未実装:
  - 本書で定義している高度な FramingComposer/SoftZone/HardBounds の完全実装
  - カメラモードを完全分離した `ICameraMode` 構成

---

## 0. 現状と課題

### 現在の実装

`PlayerController.cs` にカメラ制御が直接埋め込まれている。

| 項目 | 現状 |
|------|------|
| 視点方式 | 一人称固定（CameraHolder が Player の子オブジェクト） |
| マウス入力 | 生の `_lookInput` をそのまま適用（スムージングなし） |
| 感度設定 | `_mouseSensitivity` 単一値のみ（X/Y軸独立設定なし） |
| 視野角制限 | `_maxLookAngle = 80f` のクランプのみ |
| カメラ切替 | 未実装 |
| シネマティック | 未実装 |

### 課題

1. **スムージングがない** — マウス入力が生のまま適用され、ジッター・不自然な動きが発生する。
2. **一人称固定** — 三人称視点への切替手段がない。ゲームの物理コメディ的な見せ場（射出、落下、インベントリ暴走）が自キャラ不在で映えない。
3. **カメラロジックの責務分離** — `PlayerController` がカメラ回転も担当しており、SRP（単一責任原則）に反している。
4. **シネマティック視点が未定義** — GDD1.0 に記載なし。新規仕様として追加が必要。

---

## 1. アーキテクチャ

カメラ制御を `PlayerController` から分離し、専用のカメラシステムを構築する。

```
Assets/_Project/Scripts/Camera/
├── CameraManager.cs          # モード切替の統括
├── FirstPersonCamera.cs      # 一人称視点制御
├── ThirdPersonCamera.cs      # 三人称視点制御（オービタル）
├── CinematicCamera.cs        # シネマティック定点カメラ
├── CameraSettings.cs         # 共通設定（ScriptableObject）
└── CameraSmoother.cs         # スムージング・補間ユーティリティ
```

### クラス関係図

```
CameraManager (MonoBehaviour)
├── ICameraMode (interface)
│   ├── FirstPersonCamera : ICameraMode
│   ├── ThirdPersonCamera : ICameraMode
│   └── CinematicCamera   : ICameraMode
├── CameraSmoother (utility)
└── CameraSettings (ScriptableObject)
```

### ICameraMode インターフェース

```csharp
public interface ICameraMode
{
    void Enter(Camera camera, Transform target);
    void Exit();
    void UpdateCamera(Vector2 lookInput, float deltaTime);
    CameraModeType ModeType { get; }
}
```

---

## 2. スムージング仕様

### 2.1. 基本方針

すべてのカメラモードで共通のスムージングレイヤーを適用する。

| パラメータ | 型 | デフォルト値 | 説明 |
|-----------|-----|-------------|------|
| `SmoothingMethod` | enum | `SLerp` | 補間方式（Lerp / SLerp / CriticalDamping） |
| `SmoothTime` | float | `0.08` | 回転の追従速度（秒）。0 = 即座に追従 |
| `SensitivityX` | float | `2.0` | 水平方向の感度 |
| `SensitivityY` | float | `1.5` | 垂直方向の感度 |
| `InvertY` | bool | `false` | Y 軸反転 |
| `MaxPitchUp` | float | `80` | 上方向の視野制限（度） |
| `MaxPitchDown` | float | `80` | 下方向の視野制限（度） |

### 2.2. スムージングアルゴリズム

```
推奨: SLerp（球面線形補間）
  目標回転 = Quaternion.Euler(targetPitch, targetYaw, 0)
  現在回転 = Quaternion.Slerp(現在, 目標, smoothFactor)
  smoothFactor = 1 - exp(-deltaTime / smoothTime)

代替: Critical Damping（臨界減衰）
  速度がオーバーシュートせず滑らかに収束する。
  アクションゲームのカメラに適する。
```

---

## 3. 一人称視点（First Person Camera）

### 3.1. 基本仕様

| 項目 | 値 |
|------|-----|
| カメラ位置 | プレイヤーの頭部（CameraHolder 内、ローカルY ≈ 0.8） |
| FOV | 75°（デフォルト）、Inspector で調整可 |
| ヘッドボブ | オプション。歩行時に軽い上下揺れ（振幅: 0.02m、周波数: 歩行速度連動） |
| 衝撃リアクション | 着地時に FOV を一瞬縮小 + わずかなピッチ変化 |

### 3.2. 動作

- マウス入力 → スムージング適用 → Yaw はプレイヤーの `transform.rotation` に反映、Pitch は `CameraHolder.localRotation` に反映。
- 現行の `PlayerController.HandleLook()` と同等だが、スムージングレイヤーを経由する。

---

## 4. 三人称視点（Third Person Camera）

三人称カメラは **オービタル制御**（距離・角度）と **フレーミング制御**（画面上のどこにプレイヤーを映すか）を分離し、それぞれ独立して拡張可能な設計とする。

### 4.1. アーキテクチャ（レイヤー構成）

```
ThirdPersonCamera
├── OrbitalRig          … 距離・Yaw・Pitch の制御（マウス入力 + ホイールズーム）
├── FramingComposer     … スクリーンスペースでのプレイヤー配置制御
│   ├── ScreenOffset    … 中心からのオフセット
│   ├── DeadZone        … カメラが動かない中央領域
│   ├── SoftZone        … 緩やかに追従する領域
│   ├── HardBounds      … プレイヤーが絶対に出ない外枠（強制補正）
│   └── Drift           … ランダム揺らぎ（フレーミングの微小変動）
├── CollisionResolver   … 壁・障害物との衝突回避
└── TargetOrientation   … プレイヤーの体の向き制御
```

各レイヤーは独立した処理を担い、将来の追加仕様（ロックオン、肩越し ADS、乗り物搭乗時カメラ等）をレイヤー単位で差し替え・追加可能にする。

### 4.2. OrbitalRig（オービタル基本制御）

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `DefaultDistance` | float | 4.0 | ターゲットからのデフォルト距離（m） |
| `MinDistance` | float | 0.5 | 最小距離（これ以下で一人称に自動遷移） |
| `MaxDistance` | float | 12.0 | 最大距離 |
| `HeightOffset` | float | 1.5 | ピボットの高さオフセット（m） |
| `ZoomSpeed` | float | 2.0 | マウスホイールによるズーム速度 |
| `OrbitSmoothTime` | float | 0.1 | 回転の追従時間（秒） |
| `DistanceSmoothTime` | float | 0.15 | ズームの追従時間（秒） |

#### マウスホイールによるズーム

| 操作 | 挙動 |
|------|------|
| ホイール↑ | カメラ距離を縮小（ズームイン） |
| ホイール↓ | カメラ距離を拡大（ズームアウト） |
| 距離 < `MinDistance`（0.5m） | **自動で一人称視点に遷移** |
| 一人称中にホイール↓ | **自動で三人称視点に復帰**（MinDistance から開始） |

### 4.3. FramingComposer（フレーミング制御）

プレイヤーを画面上のどこに配置するかを、スクリーンスペース（0,0 = 左下、1,1 = 右上）で定義する。これにより「中心固定」に限らない多様な構図を実現する。

#### 4.3.1. ScreenOffset（基準オフセット）

プレイヤーを画面中央からずらした位置に配置する。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `ScreenX` | float (0-1) | 0.5 | プレイヤーの水平基準位置。0.5 = 中央、0.35 = やや左 |
| `ScreenY` | float (0-1) | 0.5 | プレイヤーの垂直基準位置。0.5 = 中央、0.4 = やや下 |

> **使用例**: `ScreenX = 0.35` で「左寄せ・右側に空間を空ける」構図。ドローンビーム照準時の視野確保や、肩越し視点の下地になる。

#### 4.3.2. DeadZone（不感帯）

スクリーンスペースに定義された矩形領域。プレイヤーがこの領域内にいる限り、カメラは回転・移動しない。微小な動きでカメラがガチャつくのを防ぐ。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `DeadZoneWidth` | float (0-1) | 0.1 | 不感帯の幅（スクリーン比率） |
| `DeadZoneHeight` | float (0-1) | 0.08 | 不感帯の高さ（スクリーン比率） |

```
┌─────────────────────────────┐  Screen (1,1)
│                             │
│      ┌───────────┐          │
│      │ DeadZone  │          │
│      │  (Player) │          │
│      └───────────┘          │
│                             │
└─────────────────────────────┘  (0,0)
 プレイヤーが DeadZone 内 → カメラ静止
```

#### 4.3.3. SoftZone（追従領域）

DeadZone の外側に定義されるより大きな矩形。プレイヤーが SoftZone 内かつ DeadZone 外にいるとき、カメラは**ゆっくりと追従**してプレイヤーを ScreenOffset 方向へ引き戻す。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `SoftZoneWidth` | float (0-1) | 0.6 | 追従領域の幅 |
| `SoftZoneHeight` | float (0-1) | 0.5 | 追従領域の高さ |
| `SoftRecenterSpeed` | float | 1.5 | 追従の速さ（高い = 速く中央へ戻す） |
| `SoftDamping` | float | 0.3 | 追従のダンピング（0 = 即座、1 = 非常に緩やか） |

```
┌───────────────────────────────────┐
│ ┌───────────────────────────────┐ │
│ │  SoftZone                     │ │
│ │      ┌───────────┐            │ │
│ │      │ DeadZone  │            │ │
│ │      │     ●     │ ← Player  │ │
│ │      └───────────┘            │ │
│ │                               │ │
│ └───────────────────────────────┘ │
│  HardBounds                       │
└───────────────────────────────────┘

  DeadZone 内   → カメラ静止
  SoftZone 内   → ゆっくり追従（SoftRecenterSpeed で制御）
  HardBounds 外 → 即座に強制補正
```

#### 4.3.4. HardBounds（強制補正境界）

SoftZone のさらに外側。プレイヤーがこの境界に達した/超えた場合、**即座にカメラを移動**してプレイヤーを領域内に収める。急な移動やグリッチ射出で画面外に飛び出すことを防ぐ。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `HardBoundsMargin` | float (0-1) | 0.05 | 画面端からのマージン。0.05 = 画面端の5%内側 |
| `HardCorrectionSpeed` | float | 15.0 | 強制補正の速さ（非常に高い値 = ほぼ即座） |
| `HardCorrectionCurve` | AnimationCurve | EaseOut | 補正のイージング。境界に近いほど強く引き戻す |

> **挙動**: プレイヤーが射出装置で吹っ飛んだ場合も、HardBounds が画面内に引き留める。ただし引き戻しの「遅れ」が生じることで、物理的な勢いの演出にもなる。

#### 4.3.5. Drift（ランダム揺らぎ / 映像的変化）

同じ構図に長時間留まることによる視覚的な飽きを防ぐため、フレーミングの基準点（ScreenOffset）にランダムな微小変動を加える。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `DriftEnabled` | bool | true | ドリフトの有効/無効 |
| `DriftAmplitudeX` | float | 0.03 | 水平方向の最大ドリフト量（スクリーン比率） |
| `DriftAmplitudeY` | float | 0.02 | 垂直方向の最大ドリフト量 |
| `DriftSpeed` | float | 0.3 | ドリフトの変化速度（低い = ゆっくりと揺らぐ） |
| `DriftNoiseType` | enum | `Perlin` | ノイズ関数の種類（Perlin / Simplex / Sine） |
| `DriftSeed` | int | auto | ノイズのシード値。auto = 起動時にランダム生成 |

```
アルゴリズム:
  offsetX = ScreenX + PerlinNoise(time * DriftSpeed, seed)     * DriftAmplitudeX
  offsetY = ScreenY + PerlinNoise(time * DriftSpeed, seed + 1) * DriftAmplitudeY

  → DeadZone / SoftZone の中心がゆっくりと移動し、
    同じ場所に立っていてもカメラの構図が微妙に変わり続ける。
```

> **設計意図**: 配信映像やリプレイにおいて「映画的な揺らぎ」を自動で生む。プレイヤーが操作していなくても画面が生きている印象を与える。戦闘中は `DriftAmplitude` を一時的に増幅させ、緊張感を演出することも可能。

#### 4.3.6. FramingComposer の拡張ポイント

以下は将来の追加仕様として受け入れ可能な拡張スロットである。FramingComposer はレイヤースタックとして構成されるため、**新しいレイヤーを追加するだけで振る舞いを拡張できる**。

| 拡張候補 | 概要 | 追加タイミング |
|---------|------|--------------|
| **LockOnReframe** | ロックオンターゲット指定時に ScreenOffset を動的に変更し、プレイヤーと敵を同一画面に収める | 敵対行動実装後 |
| **ADS / ShoulderView** | ドローンビーム精密照準時に ScreenX を肩越し位置へ遷移 | ビーム改修時 |
| **VehicleReframe** | 理想の椅子・ジープ搭乗時に距離とオフセットを動的変更 | ギミック拡張時 |
| **CinematicBlendOverride** | シネマティックカメラからの復帰時に Drift を一時停止 | Phase 4 |
| **ScreenShake** | 衝撃時にフレーミング全体を振動させる | Phase 3 |
| **SpectatorReframe** | リプレイ/観戦モード用に ScreenOffset をアニメーションで自動遷移 | 将来 |

### 4.4. CollisionResolver（コリジョン回避）

```
SphereCast: プレイヤー → カメラ目標位置
  半径: 0.2m
  ヒット時: hitPoint からわずかにプレイヤー寄り（0.2m マージン）にカメラを配置
  補間: 壁接近時は即座に寄せ、離れる時はスムーズに戻す（非対称スムージング）
```

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `CollisionRadius` | float | 0.2 | SphereCast の半径 |
| `CollisionLayers` | LayerMask | Default | 衝突判定対象のレイヤー |
| `SnapInSpeed` | float | 20.0 | 壁に近づく時の補正速度（即座に寄せる） |
| `EaseOutSpeed` | float | 3.0 | 壁から離れる時の復帰速度（ゆっくり戻す） |

### 4.5. TargetOrientation（プレイヤーの向き制御）

三人称時のプレイヤーの体の向きは、移動方向に回転させる（カメラ回転とは独立）。

| パラメータ | 型 | デフォルト | 説明 |
|-----------|-----|-----------|------|
| `RotationSpeed` | float | 10.0 | 体の回転速度 |
| `FaceMovementDirection` | bool | true | 移動方向に体を向ける |
| `FaceCameraOnAim` | bool | true | ドローンビーム使用中はカメラ方向に体を向ける |

---

## 5. シネマティックカメラ（Cinematic Camera）— 新規仕様

### 5.1. コンセプト

PS1/PS2 世代のバイオハザード・サイレントヒルなどに見られる**マップ固定位置カメラ**。  
定点からプレイヤーを映し、エリアの雰囲気・構図・緊張感を演出する。

> **設計意図**: 要塞攻略の緊迫感、AI Propの「だるまさんが転んだ」演出、ホラー的カメラワークを強化する。物理コメディのリアクション映像としても映える。

### 5.2. CinematicCameraVolume（トリガーボリューム）

各エリアに `CinematicCameraVolume` を配置する。プレイヤーがボリュームに侵入すると、対応する定点カメラに遷移する。

| パラメータ | 型 | 説明 |
|-----------|-----|------|
| `CameraPosition` | Transform | 定点カメラの位置・回転 |
| `TrackingMode` | enum | プレイヤー追従の挙動（下記参照） |
| `TrackingWeight` | float (0-1) | 追従の強さ。0 = 完全固定、1 = プレイヤーを完全に向く |
| `BlendTime` | float | 前のカメラからの遷移時間（秒） |
| `Priority` | int | 複数ボリュームが重なった場合の優先度 |
| `FieldOfView` | float | このカメラ固有の FOV |
| `DeadZone` | float | プレイヤーがこの範囲内にいる間はカメラが動かない（中央近傍） |
| `SoftZone` | float | DeadZone の外側。この範囲内でゆっくり追従 |

### 5.3. TrackingMode（追従モード）

| モード | 挙動 | 使用例 |
|--------|------|--------|
| `Fixed` | カメラは完全に固定。回転しない | 廊下の監視カメラ風 |
| `LookAt` | プレイヤーの方向を常に向く | 広間の俯瞰カメラ |
| `Track` | プレイヤーの移動に合わせてパン（水平回転のみ） | 通路を横から映す |
| `Dolly` | 定義されたパス上をプレイヤー位置に応じて移動 | 長い廊下・階段 |
| `LookAtWithLag` | LookAt だが遅延追従（TrackingWeight で制御） | 緊張感のある遅れ |

### 5.4. カメラ遷移

- ボリューム侵入時に `BlendTime` で Lerp/SLerp 遷移。
- ボリューム退出時は前のカメラモード（一人称 or 三人称）に戻る。
- 退出先のカメラモードの向きはシネマティックカメラの最終向きを基準にスムーズに復帰。

### 5.5. 入力制御

シネマティックモード中のプレイヤー入力:

| 入力 | 挙動 |
|------|------|
| 移動（WASD） | **カメラの向きではなく**、プレイヤーの画面上の位置に基づくスクリーンスペース移動 |
| マウス | カメラ回転は無効化。ドローンビームのエイムはスクリーンスペースに切替 |
| マウスホイール | 無効（シネマティック中はズーム不可） |
| ドローンビーム | スクリーン座標からの Raycast に切替 |

---

## 6. CameraManager の状態遷移

```
                    ┌─────────────────┐
                    │  FirstPerson    │
                    └────┬──────┬─────┘
         ホイール↓       │      │  ホイール↑ / 距離 < threshold
         (距離 > min)    │      │
                    ┌────▼──────▼─────┐
                    │  ThirdPerson    │
                    └────┬──────┬─────┘
    Volume 侵入          │      │  Volume 退出
                    ┌────▼──────▼─────┐
                    │  Cinematic      │
                    └─────────────────┘
```

### デフォルト起動モード

Inspector の `DefaultCameraMode` で設定（デフォルト: ThirdPerson）。

---

## 7. 設定の永続化（CameraSettings ScriptableObject）

全カメラモード共通のパラメータを `CameraSettings` ScriptableObject にまとめ、Inspector から一元管理する。プレイヤーのオプション画面での設定変更にも対応可能な設計とする。

```csharp
[CreateAssetMenu(fileName = "CameraSettings", menuName = "GlitchWorker/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Smoothing")]
    public SmoothingMethod SmoothingMethod = SmoothingMethod.SLerp;
    public float SmoothTime = 0.08f;

    [Header("Sensitivity")]
    public float SensitivityX = 2.0f;
    public float SensitivityY = 1.5f;
    public bool InvertY = false;

    [Header("Third Person")]
    public float DefaultDistance = 4.0f;
    public float MinDistance = 0.5f;
    public float MaxDistance = 12.0f;
    public float ZoomSpeed = 2.0f;
    public float FirstPersonThreshold = 0.5f;
    public float CollisionRadius = 0.2f;

    [Header("Cinematic")]
    public float DefaultBlendTime = 0.5f;
}
```

---

## 8. 実装優先度

| 優先度 | 項目 | 依存 |
|--------|------|------|
| **P0** | CameraManager + ICameraMode 基盤 | PlayerController からカメラロジック分離 |
| **P0** | スムージング（CameraSmoother） | — |
| **P1** | 一人称視点（現行動作のリファクタ） | CameraManager |
| **P1** | 三人称視点（オービタル + コリジョン） | CameraManager |
| **P2** | マウスホイールズーム + 自動視点切替 | ThirdPersonCamera |
| **P3** | シネマティックカメラ基盤（Volume + Fixed/LookAt） | CameraManager |
| **P3** | シネマティック追従モード（Track/Dolly/LookAtWithLag） | CinematicCamera 基盤 |
| **P4** | CameraSettings ScriptableObject + オプション画面連携 | 全モード安定後 |

---

## 付録: GDD1.0 への追記提案

GDD1.0 §5.1 UI階層 または §8 技術アーキテクチャに以下を追加する:

> ### カメラシステム
> - 一人称 / 三人称 / シネマティックの3モードを持つ。
> - マウスホイールで三人称の距離を調整し、一定距離以下で一人称に自動遷移する。
> - エリアに配置された CinematicCameraVolume によりPS1/PS2風の固定カメラ演出が可能。
> - 定点カメラはプレイヤーへの追従度を個別に設定でき、ホラー的緊張感や物理コメディの演出を強化する。

---

*— End of Document —*
