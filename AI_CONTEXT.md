# AI_CONTEXT.md

## プロジェクトタイプ
Unity プロジェクト (99%Slops / GLITCH-WORKER)

## 現在の checkout
- Branch: `feature/task-016-story-catalog-metaflags`
- Upstream parity: `0 0`
- Remote fetch: 2026-06-15 実施済み

## 重要な状態
- 現在ブランチは upstream と一致している。
- `origin/master` は `0e45e57` まで進んでおり、現在 feature branch とは分岐している。
- `origin/master` 側は Unity 6000.4.9f1 / URP 17.4.0 系の Phase 5 validation lane。
- 作業ツリーには既存の未コミット変更と未追跡ファイルが残っているため、mainline へ移る前に扱いを決める。

## 次に見る場所
- 現在地の正本: `docs/WORKFLOW_STATE_SSOT.md`
- 再開手順: `docs/dev/RESUME.md`
- 仕様の最上位: `docs/spec/SSOT_CORE_DOCTRINE.md`
- ロードマップ: `docs/dev/ROADMAP_v2.md`

## 次の判断
- 最新 mainline を進めるなら、ローカル変更を stash / commit / 破棄のいずれかで処理してから `origin/master` へ移る。
- 現在 feature branch を続けるなら、`TASK_016` / `TASK_017` 系を継続し、`origin/master` は別レーンとして扱う。

## 今回実行した同期
- `git fetch --prune origin`
- `git pull --ff-only`
- `git rev-list --left-right --count "HEAD...@{u}"`
- `git rev-list --left-right --count "master...origin/master"`

## 未実行
- Unity Editor 起動
- PlayMode 検証
- `origin/master` への checkout / merge / rebase
