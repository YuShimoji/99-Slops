# GLITCH-WORKER 現在地

最終監査: 2026-08-12 (Asia/Tokyo)

このファイルだけが動的な進捗正本です。仕様は `docs/spec/GDD1.0.md`、不変ルールは `AGENTS.md`、監修AIと実装AIの受け渡しは `docs/Windsurf_AI_Collab_Rules_latest.md` を参照します。

## 三つの軸で見た現在地

| 軸 | 現在 | 判定 |
|---|---|---|
| Product milestone | GDD §9.1 MVP / 最初のVertical Sliceへ向かう途中 | MVP完成ではない。GDD上のDebug View、偽トランポリン、Inventory、Level生成、Upload Port→Resultなどが未完で、Roadmap上のCamera foundationも未完 |
| Current slice | 開発再開監査とローカル環境再検証 | `DONE` locally on `codex/workflow-reset-20260710`。既存の文書差分とREADMEを保護したままremote parity、固定依存、Unity import/compileを再確認。commit / publishは未実施 |
| Verification gate | Unity 6000.4.9f1 batch import/compileとC# buildは通過、簡易Playable LoopのScene/PlayModeは未検証 | `delivery: scripts_implemented / verification: scene_wiring_missing`。Scene Integration Batchが引き続き次の実装ゲート |

「P4」「Phase 5」「Phase 2A」のような裸のPhase表記は使いません。AI workflow、Roadmap上の開発領域、検証段階を別軸で記述します。

## Gitと開発環境

| 項目 | 2026-07-10 の確認結果 | 開発上の意味 |
|---|---|---|
| 親リポジトリ | 2026-08-12 fetch後も `origin/master` = `HEAD` = `0e45e57`、ahead / behind = `0 / 0` | default branchに取り込むcommitはない。現在branchはupstream未設定かつ既存文書差分20件 + untracked READMEを保持しているため、checkout / merge / rebaseは行っていない |
| 前回の文書ビュー | `codex/local-doc-view-handoff`、Draft PR [#1](https://github.com/YuShimoji/99-Slops/pull/1) | 本線へ未マージ。壊さず隔離し、今回のclean baselineへ混ぜていない |
| Unity | `6000.4.9f1 (f7258d6eebbe)` | ローカルEditorとProjectVersionが一致 |
| URP / Input | URP `17.4.0`、Input System `1.19.0` | Package manifest / lockはJSONとして正常 |
| C# build | 2026-08-12 `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo` は0 error / 52 warnings | 開発可能。51件は同梱MCPForUnity、1件はPlayerControllerのobsolete API |
| Unity batch | 2026-08-12、同一Editor `6000.4.9f1` のbatch / no-graphics import・script compileがreturn code 0。compiler error / warning、Package Manager error、Asset import failureはいずれも0件 | 現行checkoutのimport/compileは確認済み。Scene操作とPlayModeのV-01〜V-06は未実施 |
| Tests | Unity Test Frameworkは導入済みだがproject固有test / test asmdefは0 | 自動回帰は未整備。全作業を止めず、対象リスクに応じて追加する |
| Scenes | Build Settingsは `_Project/Scenes/Sandbox` とtemplate `SampleScene` を有効化。別の `Assets/Scenes/Sandbox` も残存 | 検証は `_Project` 側を正本とする。重複・SampleScene整理は別の小さなExcise候補 |
| Submodule | 親が固定する `shared-workflows@3e62f33` を再現 | upstream `origin/main` より6 commits古いが、停止ゲート増加と互換性差分を含むため無条件更新しない |
| GitHub current view | Wiki機能は有効だがWiki repository未作成、Pagesは無効 | 今回追加したREADMEはcommit / merge後に外部入口になる。Pagesは同じSSOTから自動生成する場合だけ採用 |

## 実装の実態

| 領域 | あるもの | まだ成立していないもの |
|---|---|---|
| Camera foundation | `CameraManager`, `CameraSettings`、イベント基盤 | `ICameraMode`, `CameraSmoother`, First/Third Person mode本実装、3P collision / zoom |
| Player controls | 移動、Jump、State Machine、Dash、Fast Fall、Slow Motionなどのコード | 3P向き制御、Unity上の操作感調整、回帰証跡 |
| Minimal gameplay loop | `GameplayLoopController`, `UploadPort`, `GameplayHudPresenter` のコード | Scene未配線に加え、UploadPort既定値が `AI / Normalized` で、GDD §3.3と用語集の「Human Propを納品」に競合する |
| TASK_020〜024 | コード側の状態遷移、objective、HUD、guard / restart連携 | Task内DoDが要求するSandbox再現とPlayMode証跡。従来の `COMPLETED` は検証済みを意味しない |
| TASK_025 | V-01〜V-06の検証表とReport雛形 | 前提Sceneがないため全件TBD。実態はvalidation開始前のScene integration待ち |

`TASK_026` の「総合80% / 実装85%」は2026-02-27時点の評価文書であり、現在のGDD完成率には使用しません。以後は根拠のない総合パーセントではなく、OutcomeとVerification Gateで報告します。

## 次の実行可能なOutcome Slice

### Scene Integration Batch — `READY`

Outcomeは、`Sandbox.unity` でTASK_025のV-01〜V-06を実行できる状態にすることです。これは「検証」ではなく、検証前提を作るScene実装です。

含める作業:

1. GDDに合わせ、UploadPortの受け入れ対象をHuman Propへ直す。Human Propでは状態条件を無視するか、現行データに合わせ `Dormant` とする最小実装を選ぶ。
2. `GameplayLoopController` をSceneへ配置し、`GameManager._gameplayLoopController` を接続する。
3. `UploadPort` とTrigger Colliderを配置し、Human Propをaccepted、AI Propをrejectedとして準備する。
4. 最小Canvas、TMP labels、restart hint、`GameplayHudPresenter` を配置・配線する。
5. Unity importでMissing Script / compile error / 参照欠落がないことを確認する。

Acceptance:

- C-00〜C-07の事前確認が通り、GDD準拠の対象でV-01〜V-06を開始できる。
- Scene保存後も意図しないProjectSettings / Package差分がない。
- ここでは見た目のポリッシュ、正式Result画面、GDD外機能を増やさない。

このBatch完了後、TASK_025を一度のVerify Batchとして実施します。V-01〜V-06を個別Promptに分けず、ログとスクリーンショットをまとめて記録し、Task状態を `DONE` または具体的な修正Sliceへ移します。

## 人間が選ぶべき論点

| 論点 | 推奨 | 決めると可能になること |
|---|---|---|
| 現在の簡易loopを何と呼ぶか | 「Phase 5完成」ではなく「Minimal loop prototype」と呼ぶ | GDD §9.1の未実装を隠さず、次のMVP優先順位を選べる |
| Draft PR #1の扱い | 今回の運用resetを確認後、local-only MkDocsが不要ならclose | 前回案と新運用の二重入口を解消できる |
| 分岐したCamera実装の回収 | `origin/feature/task-016-story-catalog-metaflags` を丸ごとmergeせず、Camera files / commitsだけ監査 | ルートflatten・Story追加・GDD変更を避けつつPhase 2Aを加速できる |
| 外部ビュー | まずREADME→本SSOT。必要なら同じMarkdownをGitHub Pagesへ自動公開 | Wikiの手動二重更新なしで、ブラウザから現在地を確認できる |

これらはScene Integration Batchの開始を止めません。呼称と公開方式はCloseout時、Camera回収は次の機能Slice前に判断できます。

## 今なら安価に比較できる創造的な入口

| 入口 | 仮説と利得 | 小さな試し方 | 着手タイミング |
|---|---|---|---|
| 業務端末HUDの視覚方向 | 現在の英語ハードコードHUDを、冷たいKPI端末／壊れた社内ツールのどちらかに寄せると世界観と可読性を同時に上げられる | Scene配線後に同じ情報量で2枚の粗いレイアウトを比較する | TASK_025で機能が通った直後 |
| JP/EN表示の境界 | UI量が増える前に表示文字列を集約すると、言語対応と文体監修の手戻りが減る | HUDの4文字列だけを対象にString Table導入コストを見積もる。採用までは実装しない | Result UI設計前 |
| 3Pカメラの撮影性 | 既存の分岐実装を選択回収できれば、15秒映像のフレーミング検証を早められる | ルート変更を除外し、Camera関連差分だけコードレビューする | Minimal loop検証後 |

## 今回の検証記録

- `git fetch --prune --tags origin`: 成功。remote HEADは`master`、`origin/master`とlocal HEADは同じ`0e45e57`。取り込み対象がないためpull / fast-forwardは不要だった。
- `git submodule sync --recursive` / `git submodule update --init --recursive`: 成功。`shared-workflows@3e62f33`で親指定commitと一致。
- Package manifest / lock JSON parse: 成功。direct dependency 46件に欠落・version/depth不一致なし。registry配布のdirect dependency 8件はローカルPackageCacheの実体versionとも一致。
- Unity `6000.4.9f1 -batchmode -nographics -quit`: 成功。現行projectを読み込み、Package Manager登録とscript compileを完了してreturn code 0。PlayModeはScene前提不足のため未実行。
- `dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo`: 成功、0 error / 52 warnings。
- `git lfs fsck`: 問題なし。
- Unity batch前後で既存status、tracked diff fingerprint、README、manifest、lockfileのhashは一致。今回生成した一時ログは集計後に削除した。

## Cloud Codexの環境

環境設定と実行コマンドは[Cloud開発手順](CLOUD_CODEX.md)を使う。
`codex/cloud-codex-environment-20261011`は`master/a8c40b0`から分けた環境整備branchで、Unity rootは`99PercentSlops/`、Editorは6000.4.9f1。
setup / maintenanceと構造検査の異常入力7試験を隔離checkoutで確認済み。Cloud登録・PublishとEditorの実行結果は、それぞれの環境setup report・検証記録で照合する。
上記のScene integrationや手動検証の状態は維持する。

## 更新規則

このファイルは、Current slice、Verification gate、Human Authority、次の一手のいずれかが変わったCloseoutで一度だけ更新します。Task / Report / Handover / Resume / Mission Logへ同じ現在地を複製しません。証跡はTask、PR、ログ、画像へ置き、ここから必要なものだけをリンクします。
