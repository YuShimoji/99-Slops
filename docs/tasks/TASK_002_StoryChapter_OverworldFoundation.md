# Task: Story Chapter / Overworld Foundation（将来実装向けバックログ）
Status: OPEN
Tier: 3
Branch: feature/task-002-story-chapter-overworld
Owner: Unassigned
Created: 2026-02-11T23:45:00+09:00
Report:
Milestone: SG-2 / MG-2

## Objective
- 「チャプターリスト」と「それを内包するメインオーバーワールド」を両立する進行基盤を設計し、縦切りで実装可能な単位に分解する。

## Context
- 要件:
  - 複数チャプター（クエスト駆動）をリストで管理できること
  - メタ進行でフラグ解放が進み、過去チャプター再訪時に会話・攻略ルートが変化すること
  - チャプター群を包含するメインオーバーワールド構造を持つこと
- 本タスクは将来実装を加速するためのバックログ起票（即時実装はしない）。

## Focus Area
- `docs/spec/`（進行仕様の追加整理）
- `docs/dev/ROADMAP_v2.md`（マイルストーン紐付け）
- `docs/tasks/`（縦切りタスクへの分割）

## Forbidden Area
- `99PercentSlops/ProjectSettings/`, `99PercentSlops/Packages/`
- 既存プレイアブル機能への直接改修（Camera/Player/Drone実装）
- `shared-workflows/`

## Constraints
- 1タスクで完結させず、縦切りスライスで最短価値を順に積む。
- セーブ互換性を壊す設計（ID再採番や破壊的データ移行）を避ける。
- 章進行とメタ進行は分離し、相互参照はフラグID経由に限定する。

## Vertical Slice Plan
- Slice 1: `ChapterDefinition` / `ChapterCatalog` の導入（章リスト表示 + 章開始のみ）
- Slice 2: `MetaFlagService` 導入（チャプター外で永続化されるフラグ解放）
- Slice 3: `OverworldDirector` 導入（章ハブ画面 + 再訪導線）
- Slice 4: `ChapterVariantResolver` 導入（再訪時の会話差分・攻略ルート分岐）

## Test Plan
- **テスト対象**:
  - チャプター一覧ロード
  - メタフラグ保存/復元
  - 再訪時の差分解決
- **EditMode テスト**:
  - `ChapterCatalogTests`
  - `MetaFlagServiceTests`
  - `ChapterVariantResolverTests`
- **PlayMode テスト**:
  - オーバーワールド -> チャプター遷移 -> 再訪差分確認のスモーク
- **ビルド検証**:
  - 進行データ追加後のコンパイルとビルド成功
- **期待結果**:
  - 章リストとオーバーワールドが共存し、再訪差分が再現可能

## Impact Radar
- **コード**: 進行管理・セーブデータ・会話分岐に影響
- **テスト**: 進行状態テストの基盤が必要
- **パフォーマンス**: フラグ評価頻度とロード時解決コスト
- **UX**: 章選択導線と再訪時の変化提示が体験を左右
- **連携**: 将来のクエスト・会話・納品ループと接続
- **アセット**: 章定義ScriptableObject・会話データ差分
- **プラットフォーム**: セーブデータ互換性（PC先行）

## DoD
- [ ] 上記4スライスに分解した実装タスクを個別チケット化した
- [ ] 章ID/フラグIDの命名規約を定義した
- [ ] 再訪差分の最小ユースケースを仕様化した
- [ ] 依存関係（セーブ/会話/クエスト）を明記した

## Notes
- このチケットは「実装着手前の分割設計」バックログ。
