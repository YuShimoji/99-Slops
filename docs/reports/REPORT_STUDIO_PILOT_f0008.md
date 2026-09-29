# Studio f0008の作品内技術試験

2026-09-30。`codex/glitch-worker-studio-pilot`の独立チェックアウトで、Game Projectsの家具試作f0008をGLITCH-WORKERのOffice向け候補としてUnityに読み込んだ。作品での使用、美術、AI Prop／Human Propとしての役割は採択していない。

## 入力と変更範囲

| 対象 | 固定した内容 |
|---|---|
| 制作元 | `studio-slice/model-library/furniture/asset.json`の`asset:furniture-study`、版`f0008`。`artStatus=pending` |
| FBX | `studio-slice/runs/furniture/f0008/asset.fbx`から`Assets/_Project/StudioPilot/f0008/asset.fbx`へ複製。両者のSHA-256は`8411c1dcba69bee514338764c3e35cfc1bd00918523ae9ad5bc42a66057c51ee` |
| 素材の利用条件 | f0008は`finish=ivory`で、この試験では外部テクスチャを複製していない。制作元が別仕上げ用に記録するPoly Haven Romantic VeneerはCC0。出典は`studio-slice/model-library/furniture/resources/romantic_veneer/resource.json`と[Poly Havenのライセンス](https://polyhaven.com/license) |
| Scene | `Sandbox.unity`をUnity Editor APIで開き、別名`Assets/_Project/Scenes/StudioPilot_f0008.unity`として保存。元の`Sandbox.unity`のSHA-256は変更前後とも`27da37594ff86f5a3f634be28fa9bc606f05911b8e27670c6e46a74c7c97ebd9` |
| 配置 | 初回Sceneでは`StudioPilot_f0008_TechnicalOnly`を位置`(4, 0, 4)`に置いた。カメラ検証で`(0, 0, 4)`へ移し、実Playerで既存Cannonのマゼンタと重なるため、現在の別Sceneでは`(-1.5, 0, 3.6)`に置く。rootのGlobalObjectIdは版の往復でも維持した。f0008のModel GUIDは`98a0ec06924828e4ab6f4b5080fb8ca1` |

`Assets/_Project/Editor/StudioPilotBatch.cs`の`Create`は既存の技術試験Sceneを上書きせず、`Verify`はScene内の一つのroot、位置、モデルへのprefab参照を確認する。`Build`はまず`Verify`し、出力先が空で新しい絶対`.exe`パスの時だけ、この別Scene一つをWindows 64-bit Playerへbuildする。元のPhase 5 `TASK_025`が扱う`Sandbox`の配線やStatusは変更していない。

## この端末で確認したこと

Unity 6000.4.9f1の初回起動でPackage Managerが`The "path" argument must be of type string. Received undefined`と停止した。このシェルでは`ALLUSERSPROFILE`が欠けており、空プロジェクトでも同じ失敗を再現した。起動するPowerShellプロセス内だけで`$env:ALLUSERSPROFILE = $env:ProgramData`として再実行すると、空プロジェクトと本作品の既存依存解決・スクリプトコンパイルが終了コード0になった。システム環境変数やpackage宣言は変更していない。

その条件で`StudioPilotBatch.Create`と`StudioPilotBatch.Verify`を別々のUnityバッチ起動で実行し、どちらも終了コード0だった。Unity生成YAMLの末尾空白だけを整えた後にも`Verify`を再実行し、Scene再読込、FBX prefabへの参照、固定した配置を確認した。保存する新SceneのSHA-256は`07a7e1c218586fe73a9dd8a541de7cd52fe44fb7119def1d551fff98fd0ca1ab`。ログはローカルの`Game Projects/.codex-tmp/gw-pilot-evidence/`に保管し、Gitには含めない。

同じ条件で`STUDIO_PILOT_BUILD_PATH=D:\GameProjectsStudioPilotBuild-20260930\StudioPilot.exe`を当該プロセス内だけに指定し、`StudioPilotBatch.Build`をUnityバッチで実行した。`Verify`の後、この別Scene一つのWindows 64-bit Player buildが成功し、Unityログは`STUDIO_PILOT_BUILD_SUCCEEDED`、終了コード0を記録した。Unityが報告するbuild全体のサイズは105,598,852 bytesで、出力の`StudioPilot.exe`単体のSHA-256は`a197542ad0d026c5c3bc7aead606b6b0184adad7b4ee3635326c575b25a5b423`。出力一式は`D:\GameProjectsStudioPilotBuild-20260930\`にある。元`Sandbox`と新SceneのSHA-256はbuild後も同じだった。

build済みPlayerを`-batchmode -nographics -quit`で無画面起動した。`D:\GameProjectsStudioPilotBuild-20260930\player-smoke.log`にはUnity 6000.4.9f1の初期化と、`StudioPilot_f0008`内の既存`GameEventDebugLogger`の`Awake`／`OnEnable`が記録され、`exception`／`error`／`failed`の行はなかった。20秒では自動終了しなかったため、起動したプロセスIDだけを停止した。終了コード`-1`はこの手動停止によるもので、ゲームの正常終了を証明しない。ログのSHA-256は`b10652a1945fe3658858338af7adf227be981aa5c372934d05d74213eb1ceab2`。

Unityはbuild中に既存のRender Pipeline設定などを自動更新し、PerformanceTestRunのJSONを削除した。build前にこの別チェックアウトが清潔だったことを確認し、差分と変更後ファイルを`Game Projects/.codex-tmp/gw-pilot-evidence/build-side-effects/`へ保存してから、その自動差分の対象ファイルだけを元へ戻した。作品側の保存データや元の作業レーンは触れていない。

再確認には、このチェックアウトのUnity projectを指定し、`ALLUSERSPROFILE`を当該プロセス内だけに設定して`-batchmode -nographics -quit -executeMethod GlitchWorker.EditorTools.StudioPilotBatch.Verify`を実行する。buildを再実行する場合は新しい空の出力先を`STUDIO_PILOT_BUILD_PATH`に指定し、実行メソッドを`GlitchWorker.EditorTools.StudioPilotBatch.Build`にする。`Create`は新Sceneが存在する場合に停止するため、既存Sceneを上書きする再生成には使わない。

## 画面で見える技術Sceneへの更新

`StudioPilotCapture.Render`で`Main Camera`を960×540に描画し、初回配置`(4,0,4)`では机のviewport中心がx=1.0945で画面右外だったと確認した。Sceneを保存せず配置だけ変えるprobeでは、`(0,0,4)`で中心x=0.5、y=0.1433、境界x=0.3863〜0.6137、y=0.0345〜0.2351となり、画面下部に机が見える。`-nographics`はNull Graphics Deviceの灰色画像になるため描画検証に使わず、GPU付きのバッチ起動で取得した。

元f0008の`unity-request.json`から4種類の色・金属度・粗さと21 meshのsurface対応だけを`Assets/_Project/StudioPilot/f0008/appearance.json`へ固定した。FBX SHA-256を照合し、既存プロジェクトのURP Litでmaterialを作り、別SceneのFBX prefab instanceに割り当てた。最初にStandard shaderで作ると画面がマゼンタになり、URPへ修正した。現在のappearance manifestのSHA-256は`0360ff1fff37320add8ad2a6ec651f288b01de916c4aca81ca41cac78a3e8922`。色値は制作元の技術試作の再現で、作品の色や質感の採択ではない。

この段階の`StudioPilot_f0008.unity`は`(0,0,4)`、同じFBX参照と21 meshのmaterial対応を`Verify`で確認した。Unity生成YAMLの末尾空白だけを整えたSceneのSHA-256は`4b6ebd7d55658ab6164a5c7d27e0dbce419eb134942d2fa4be0e09014ee7481b`。GPUはNVIDIA GeForce GTX 1650 Tiで、保存Sceneを再読込した[カメラ画像](media/studio-pilot-f0008-camera-visible.png)のSHA-256は`c178c31b09fe3b9ca1f5055af29799f2ac2bf624da85d34ea2a2d545bfbce6eb`。机は画面に入ったが、Playerでの既存Cannonとの重なりはこの後に判明した。

更新後の別Scene一つを`D:\GameProjectsStudioPilotBuild-20260930-Visible\StudioPilot.exe`へ再buildし、`STUDIO_PILOT_BUILD_SUCCEEDED`と終了コード0を確認した。Unityが報告するbuild全体は105,600,660 bytes、`StudioPilot_Data/level0`のSHA-256は`aeaa7d2ae6e38e4004c787d18999ea29882191c8966c39985b91894254c99a31`。buildが再び変更したRender Pipeline設定とPerformanceTestRun JSONは、差分を`.codex-tmp/gw-pilot-evidence/build-visible-side-effects/`へ保全してから、開始時に清潔だった9つの対象だけを戻した。元`Sandbox.unity`は引き続きSHA-256 `27da37594ff86f5a3f634be28fa9bc606f05911b8e27670c6e46a74c7c97ebd9`。

新しいPlayerも`-batchmode -nographics`で15秒起動し、ログに別Sceneの`GameEventDebugLogger`の`Awake`と`OnEnable`を確認した。例外・エラー行はなく、試験のため起動したプロセスだけを停止した。ログSHA-256は`80ed2cb119ebafcc33e084ee52584d2f2fb563d713b303564c3b0d460ea5a60c`。これは画面表示や操作の確認ではない。

## 実PlayerとStudioからの一往復

実Playerでは`(0,0,4)`の机が既存の`GlitchCannon/CannonVisual`と重なった。Cannonのマゼンタは机のmaterialではなく、Sceneに元からある別オブジェクトのものだった。別Sceneの机だけを`(-1.5,0,3.6)`へ移して保存し、`Sandbox.unity`は変更していない。計測専用コンポーネントを別Sceneの一時コピーにだけ付けたWindows Playerをbuildし、通常のGPU付きウィンドウで起動した。[f0008の実Player画像](media/studio-pilot-f0008-player-visible.png)と[計測receipt](media/studio-pilot-f0008-player-receipt.json)を保存した。カメラの机中心はviewport `(0.31594,0.10734)`、21 renderer、URP Litの天板と収納を確認した。既存`PlayerInput`の`Player` action mapでW入力時の移動は0.497m、Space入力時の上昇は3.747mで、Jump actionが一度発火した。これは試験用のキーボード状態イベントを既存Playerへ渡す技術計測であり、人が遊んだ体感評価ではない。無画面・非前面での試行では入力が進まなかったため、正常値は通常ウィンドウでの計測に限る。

Studioの共通画面で机の幅だけを1.4mから1.5mへ編集・保存し、サーバー再起動後も1.5mが復元されることを画面で確認した。候補`f0014`を生成し、利用中`f0008`との比較・独立previewの画面読込を経て、試験用`Furniture.unity`へ明示反映した。`studio-slice/tools/pilot_handoff.py`は現在のStudio usage receiptとFBX・Sceneのhashを照合してから、`StudioPilotConsumer.Apply`へファイル経由で渡す。pilotはFBXと形・surface記録を作品側へ取り込み、同じrootの子だけを版で差し替える。HTTP APIやPlayer runtimeの依存は増やしていない。pilotへの`f0008→f0014`反映receiptでは21 renderer、モデルGUID`ab19c564e81ddbe49868367f17a6275c`、root ID`GlobalObjectId_V1-2-efbd321cecf79614cbe3506b19dec353-935718826-0`を確認した。

`f0014`の別Sceneを再読込して`StudioPilotBatch.Verify`が通り、計測用Player buildも成功した。[f0014のPlayer画像](media/studio-pilot-f0014-player-visible.png)と[計測receipt](media/studio-pilot-f0014-player-receipt.json)では机のviewport横幅がf0008の約0.208から約0.220へ変わり、既存W移動0.497mとSpace上昇3.747mも通った。これは幅変更が同じ実Player場面へ届くことの技術確認で、ゲーム内配置の採用や美術評価ではない。最後にStudio画面の切戻しとpilot handoffで両利用先を`f0008`へ戻し、編集入力もf0008の幅1.4mで保存した。`f0014`候補は履歴に残した。Unityが書いたYAMLの行末空白42 bytesを機械的に除いた後の最終pilot Scene SHA-256は`46851e2d442dd3e039f4c61e3b46f3c7e6e6764a61149738d2c28b130dec683f`（最終handoff receiptは除去前のhash）。元`Sandbox.unity`は前後とも`27da37594ff86f5a3f634be28fa9bc606f05911b8e27670c6e46a74c7c97ebd9`。Unity buildが自動変更した既存設定9ファイルはhash確認済みの事前控えから戻した。生成Playerと詳細ログはローカルの`.codex-tmp/gw-pilot-evidence/`と`D:\GameProjectsStudioPilotBuild-20260930-f0014Probe\`に保全した。

## 次に残る境界

技術別Sceneの実Player表示・既存移動とジャンプ・Studioからの版往復は成立した。`TASK_025`が所有する`Sandbox`のPhase 5 V-01〜V-06、Editor Play Mode、人間の操作感、家具の物理・Prop属性、Officeでの役割、作品への採用、色や質感の美術判断は未実施。したがってD1の「作品の一場面で固定版を使い、作品側build・testを通す」全体はまだ完了していない。次は作品正本と`TASK_025`の所有範囲を照合し、既存Phase 5検証に干渉せず、この別Sceneから作品の通常場面へ進める最小の接続点を選ぶ。GDD1.0のOfficeのデスク種別だけからAI Prop／Human Propの意味付けは決めない。
