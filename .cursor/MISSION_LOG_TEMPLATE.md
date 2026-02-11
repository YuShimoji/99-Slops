# Mission Log Template

> このファイルは、AIエージェントワークフローの状態管理を一元化するためのテンプレートです。
> Orchestrator と Worker は、このファイルを参照・更新することで、作業の進捗とコンテキストを維持します。

---

## 基本情報

- **Mission ID**: `<MISSION_ID>`（例: ORCH_20250101_1200）
- **開始時刻**: `<ISO8601_TIMESTAMP>`
- **最終更新**: `<ISO8601_TIMESTAMP>`
- **現在のフェーズ**: `<PHASE_NAME>`（例: Phase 2: 状況把握）
- **ステータス**: `<STATUS>`（IN_PROGRESS / BLOCKED / COMPLETED）

---

## 現在のミッション

### 目標
- `<主要な目標を1-3行で記述>`

### 進捗サマリ
- `<完了した作業の要約>`
- `<進行中の作業>`
- `<未着手の作業>`

---

## フェーズ別進捗

### Phase 0: Bootstrap / SSOT確認
- [ ] `.shared-workflows/` の存在確認
- [ ] SSOT ファイル（`docs/Windsurf_AI_Collab_Rules_latest.md`）の確認
- [ ] `ensure-ssot.js` の実行（必要時）
- [ ] 基本ディレクトリ構造の確認

**完了条件**: SSOT が読み込め、参照ファイルが全て揃っている

---

### Phase 1: Sync & Merge
- [ ] `git fetch origin` 実行
- [ ] `git status -sb` で作業ツリー確認
- [ ] `docs/inbox/` のレポート回収
- [ ] `docs/HANDOVER.md` への統合

**完了条件**: リモート同期済み、inbox が空（.gitkeep のみ）、HANDOVER が最新状態

---

### Phase 1.5: 巡回監査
- [ ] `docs/tasks/` の DONE チケットに Report パスが存在するか確認
- [ ] `docs/inbox/` の REPORT に対応するチケットが存在するか確認
- [ ] `docs/HANDOVER.md` と未完了チケットの整合性確認
- [ ] 異常検知時の是正実施

**完了条件**: 全ての異常が解消され、整合性が保たれている

---

### Phase 2: 状況把握
- [ ] `docs/HANDOVER.md` の読み込み
- [ ] `docs/tasks/` の OPEN/IN_PROGRESS チケット列挙
- [ ] `AI_CONTEXT.md` の `todo_list` 更新
- [ ] `node scripts/todo-sync.js` の実行

**完了条件**: 現在のタスク状況が明確になり、todo_list が同期済み

---

### Phase 3: 分割と戦略
- [ ] タスクの Tier 分類（1/2/3）
- [ ] 並列化可能性の判断
- [ ] Worker 数の決定（最大3）
- [ ] 各 Worker の Focus Area / Forbidden Area 定義

**完了条件**: タスク分割が完了し、各 Worker の担当範囲が明確

---

### Phase 4: チケット発行
- [ ] `docs/tasks/TASK_XXX_*.md` の作成
- [ ] Status: OPEN で登録
- [ ] DoD チェックリストの定義
- [ ] テスト範囲の明示（主要パスのみ）

**完了条件**: 全てのタスクがチケット化され、DoD が定義済み

---

### Phase 5: Worker起動用プロンプト生成
- [ ] 各チケット用の Worker プロンプト生成
- [ ] `docs/inbox/WORKER_PROMPT_TASK_XXX.md` への保存（任意）
- [ ] Context（背景情報）の統合

**完了条件**: 全ての Worker 用プロンプトが生成済み

---

### Phase 6: Orchestrator Report
- [ ] チャット出力（固定5セクション: 現状/次のアクション/ガイド/メタプロンプト再投入条件/改善提案）
- [ ] `docs/inbox/REPORT_ORCH_<ISO8601>.md` の作成
- [ ] `node scripts/report-validator.js` の実行
- [ ] `node scripts/finalize-phase.js` の実行

**完了条件**: レポートが作成・検証済み、git コミット済み

---

## タスク一覧

### アクティブタスク
| チケットID | タイトル | Tier | Status | Worker | 進捗 |
|-----------|---------|------|--------|--------|------|
| TASK_XXX | `<タイトル>` | 2 | IN_PROGRESS | Worker-1 | 50% |

### 完了タスク
| チケットID | タイトル | 完了時刻 | Report |
|-----------|---------|---------|--------|
| TASK_YYY | `<タイトル>` | `<ISO8601>` | `docs/inbox/REPORT_YYY_*.md` |

### ブロックタスク
| チケットID | タイトル | ブロック理由 | 次手 |
|-----------|---------|------------|------|
| TASK_ZZZ | `<タイトル>` | `<理由>` | `<次手候補>` |

---

## コンテキスト情報

### 参照ファイル
- SSOT: `docs/Windsurf_AI_Collab_Rules_latest.md`
- HANDOVER: `docs/HANDOVER.md`
- AI_CONTEXT: `AI_CONTEXT.md`
- REPORT_CONFIG: `REPORT_CONFIG.yml`

### 重要な決定事項
- `<決定事項1>: <理由>`
- `<決定事項2>: <理由>`

### リスク・懸念事項
- `<リスク1>: <影響範囲>`
- `<リスク2>: <影響範囲>`

---

## 次回アクション

### 即座に実行すべきこと
1. `<アクション1>`
2. `<アクション2>`

### 次回 Orchestrator 起動時の確認事項
- [ ] `<確認事項1>`
- [ ] `<確認事項2>`

---

## 改善提案（New Feature Proposal）

### コードベース改善案
- **提案1**: `<改善内容>` - `<理由>` - `<優先度: High/Medium/Low>`
- **提案2**: `<改善内容>` - `<理由>` - `<優先度: High/Medium/Low>`

### ワークフロー改善案
- **提案1**: `<改善内容>` - `<理由>` - `<優先度: High/Medium/Low>`
- **提案2**: `<改善内容>` - `<理由>` - `<優先度: High/Medium/Low>`

---

## ログ履歴

### `<ISO8601_TIMESTAMP>` - `<ACTOR>` - `<ACTION>`
- `<詳細>`

### `<ISO8601_TIMESTAMP>` - `<ACTOR>` - `<ACTION>`
- `<詳細>`

---

## 注意事項

- このファイルは **常に最新状態を保つ** こと。各フェーズ完了時に必ず更新する。
- Worker は作業開始時にこのファイルを読み、作業完了時に進捗を更新する。
- Orchestrator は Phase 移行時にこのファイルを更新し、Worker に最新状態を伝える。
- ファイルパスは **動的に確認** すること（`ls`, `find`, `Test-Path` 等を使用）。ハードコード禁止。
