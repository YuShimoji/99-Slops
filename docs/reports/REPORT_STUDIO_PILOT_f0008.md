# Studio f0008の作品内技術試験

2026-09-30。`codex/glitch-worker-studio-pilot`の独立チェックアウトで、Game Projectsの家具試作f0008をGLITCH-WORKERのOffice向け候補としてUnityに読み込んだ。作品での使用、美術、AI Prop／Human Propとしての役割は採択していない。

## 入力と変更範囲

| 対象 | 固定した内容 |
|---|---|
| 制作元 | `studio-slice/model-library/furniture/asset.json`の`asset:furniture-study`、版`f0008`。`artStatus=pending` |
| FBX | `studio-slice/runs/furniture/f0008/asset.fbx`から`Assets/_Project/StudioPilot/f0008/asset.fbx`へ複製。両者のSHA-256は`8411c1dcba69bee514338764c3e35cfc1bd00918523ae9ad5bc42a66057c51ee` |
| 素材の利用条件 | f0008は`finish=ivory`で、この試験では外部テクスチャを複製していない。制作元が別仕上げ用に記録するPoly Haven Romantic VeneerはCC0。出典は`studio-slice/model-library/furniture/resources/romantic_veneer/resource.json`と[Poly Havenのライセンス](https://polyhaven.com/license) |
| Scene | `Sandbox.unity`をUnity Editor APIで開き、別名`Assets/_Project/Scenes/StudioPilot_f0008.unity`として保存。元の`Sandbox.unity`のSHA-256は変更前後とも`27da37594ff86f5a3f634be28fa9bc606f05911b8e27670c6e46a74c7c97ebd9` |
| 配置 | 新Sceneに`StudioPilot_f0008_TechnicalOnly`を一つ作り、位置`(4, 0, 4)`の子としてFBX prefab instanceを置いた。UnityのModel GUIDは`98a0ec06924828e4ab6f4b5080fb8ca1` |

`Assets/_Project/Editor/StudioPilotBatch.cs`の`Create`は既存の技術試験Sceneを上書きせず、`Verify`はScene内の一つのroot、位置、モデルへのprefab参照を確認する。`Build`はまず`Verify`し、出力先が空で新しい絶対`.exe`パスの時だけ、この別Scene一つをWindows 64-bit Playerへbuildする。元のPhase 5 `TASK_025`が扱う`Sandbox`の配線やStatusは変更していない。

## この端末で確認したこと

Unity 6000.4.9f1の初回起動でPackage Managerが`The "path" argument must be of type string. Received undefined`と停止した。このシェルでは`ALLUSERSPROFILE`が欠けており、空プロジェクトでも同じ失敗を再現した。起動するPowerShellプロセス内だけで`$env:ALLUSERSPROFILE = $env:ProgramData`として再実行すると、空プロジェクトと本作品の既存依存解決・スクリプトコンパイルが終了コード0になった。システム環境変数やpackage宣言は変更していない。

その条件で`StudioPilotBatch.Create`と`StudioPilotBatch.Verify`を別々のUnityバッチ起動で実行し、どちらも終了コード0だった。Unity生成YAMLの末尾空白だけを整えた後にも`Verify`を再実行し、Scene再読込、FBX prefabへの参照、固定した配置を確認した。保存する新SceneのSHA-256は`07a7e1c218586fe73a9dd8a541de7cd52fe44fb7119def1d551fff98fd0ca1ab`。ログはローカルの`Game Projects/.codex-tmp/gw-pilot-evidence/`に保管し、Gitには含めない。

同じ条件で`STUDIO_PILOT_BUILD_PATH=D:\GameProjectsStudioPilotBuild-20260930\StudioPilot.exe`を当該プロセス内だけに指定し、`StudioPilotBatch.Build`をUnityバッチで実行した。`Verify`の後、この別Scene一つのWindows 64-bit Player buildが成功し、Unityログは`STUDIO_PILOT_BUILD_SUCCEEDED`、終了コード0を記録した。Unityが報告するbuild全体のサイズは105,598,852 bytesで、出力の`StudioPilot.exe`単体のSHA-256は`a197542ad0d026c5c3bc7aead606b6b0184adad7b4ee3635326c575b25a5b423`。出力一式は`D:\GameProjectsStudioPilotBuild-20260930\`にある。元`Sandbox`と新SceneのSHA-256はbuild後も同じだった。

Unityはbuild中に既存のRender Pipeline設定などを自動更新し、PerformanceTestRunのJSONを削除した。build前にこの別チェックアウトが清潔だったことを確認し、差分と変更後ファイルを`Game Projects/.codex-tmp/gw-pilot-evidence/build-side-effects/`へ保存してから、その自動差分の対象ファイルだけを元へ戻した。作品側の保存データや元の作業レーンは触れていない。

再確認には、このチェックアウトのUnity projectを指定し、`ALLUSERSPROFILE`を当該プロセス内だけに設定して`-batchmode -nographics -quit -executeMethod GlitchWorker.EditorTools.StudioPilotBatch.Verify`を実行する。buildを再実行する場合は新しい空の出力先を`STUDIO_PILOT_BUILD_PATH`に指定し、実行メソッドを`GlitchWorker.EditorTools.StudioPilotBatch.Build`にする。`Create`は新Sceneが存在する場合に停止するため、既存Sceneを上書きする再生成には使わない。

## まだ成立していないこと

画面上の見え方、Playerの起動・操作、Unity Play Mode、机への物理・Prop属性の付与、既存Phase 5のV-01〜V-06、Studio通常操作から作品Sceneへの自動反映は検証していない。FBXは静的形状の候補で、元の材質・色が作品上で再現した証明でもない。別Scene一つのbuild成功は、既存の作品Gameplay Sceneやtestが通る証明ではない。従ってD1の「作品の一場面で使い、作品側build・testが通る」は未完了であり、美術や作品利用の採択も未了。

次は作品正本に照らして机の役割と表示条件を具体化し、`TASK_025`のScene検証と衝突しない作品内の反映・Play Modeを確認する。AI Prop／Human Propなどの意味付けや見た目は、人間の判断材料を作ってから扱う。
