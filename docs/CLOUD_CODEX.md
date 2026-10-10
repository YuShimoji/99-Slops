# 99%SlopsをCloud Codexで開発する

CloudではGLITCH-WORKERのC#・Shader・Scene設定を編集し、Unity構造の検査を実行できる。ライセンス付きUnity Editorがない環境では、コンパイル、Scene配線、PlayMode、操作感・描画の検証を実行していない状態で返す。

## 環境の設定

| 設定 | 値 |
|---|---|
| Repository | `YuShimoji/99-Slops` |
| 開発branch | `codex/cloud-codex-environment-20261011`。基点は`master`の`a8c40b0` |
| Unity project root | `99PercentSlops/` |
| Unity Editor | `6000.4.9f1`。`99PercentSlops/ProjectSettings/ProjectVersion.txt`を正本とする |
| ソース検査用runtime | Node.js 22以上、Git。追加のnpm依存は不要 |
| Install script / legacy Setup script | `bash scripts/codex/setup.sh` |
| legacy Maintenance script | `bash scripts/codex/maintenance.sh` |
| Secrets | ソース編集・構造検査には不要 |
| Privacy | Only me |

本線のUnity rootは一段内側にある。`feature/task-016-story-catalog-metaflags`のflatten済み構成やStudio pilotのScene変更を本線へ混ぜない。検査コマンドはrootとnestedの両構成を判別し、二つのUnity projectが見つかった場合は曖昧さをエラーとして返す。

現行CloudではInstall scriptとStart skillを保存してPublishする。既存環境の編集はSave後にRepublishし、新しいtaskで確認する。legacy環境にはSetup / Maintenance scriptの同じコマンドを設定できる。[公式の環境設定手順](https://learn.chatgpt.com/docs/environments/cloud-environments)を参照。

Start skillには次の手順を指定する:

1. Repository rootの`AGENTS.md`、`docs/WORKFLOW_STATE_SSOT.md`と今回の変更対象の仕様を読む。通常の起動で`docs/reports/`や`docs/inbox/`を読まない。
2. `bash scripts/codex/maintenance.sh`を実行する。常駐サービスは不要。
3. Assetsの変更は`99PercentSlops/Assets/_Project/`を使い、対応する`.meta`を保つ。Unity未実行ならSceneや操作の検証状態を残す。
4. 結果は同じtaskと担当ファイルへ保存する。別chatへの通知・ACK・自動転送を行わない。

## 検査とEditorへ渡す範囲

```bash
bash scripts/codex/setup.sh
node --test scripts/codex/check.test.mjs
node scripts/codex/check.mjs
git diff --check
```

検査はmanifest/lockのJSONとdirect依存の存在、asmdef等のJSON、GUIDの妥当性・重複、ファイルの`.meta`の欠落、大小文字が衝突するpathを確認する。trackedと未追跡の編集対象を読み、Libraryや生成csprojに依存しない。C#の意味・型・Unity API互換、Sceneの参照先、描画・操作はEditorで検証する。

`shared-workflows`は固定commitの参照資料。今回のコマンドはsubmoduleのinitや最新版への更新を必要としない。LFS pointerがある場合はEditor import前に認証付き`git lfs pull`を行い、`node scripts/codex/check.mjs --require-assets`を通す。

Editorでの検証は`99PercentSlops/`を開き、変更対象のimport/compileとPlayModeを実行する。通常Sandboxの正本は`Assets/_Project/Scenes/Sandbox.unity`。手動V-01〜V-06の結果、Cloudの静的検査、過去の別worktreeの結果を別々に記録する。

2026-10-11の隔離checkoutでsetup、maintenanceと検査の異常入力7試験を実行した。nested root判別、3件のJSON、334件のmetaにエラーはなかった。Editor検証は未実施。Cloudでの実行結果は環境のsetup reportで確認する。
