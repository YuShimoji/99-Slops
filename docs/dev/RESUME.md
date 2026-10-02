# 再開入口

現在地をこのファイルへ複製しません。最新の状態、検証結果、次のOutcome Sliceは [WORKFLOW_STATE_SSOT](../WORKFLOW_STATE_SSOT.md) を参照してください。

```powershell
git pull --ff-only
git submodule update --init --recursive
```

Unity Hubから `99PercentSlops` をProjectVersionと同じUnityで開きます。作業前に `git status --short --branch`、作業後に変更リスク相応の検証を行ってください。
