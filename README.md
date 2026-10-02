# GLITCH-WORKER (99%Slops)

AI生成ディストピアで、猫の社畜がドローンと物理挙動を使ってバグだらけの世界を掃除する Unity 6 / URP 向けグリッチ・イマーシブシムです。

## まず見る場所

| 知りたいこと | 正本 | 用途 |
|---|---|---|
| いま何が動き、何が止まっているか | [現在地と次の一手](docs/WORKFLOW_STATE_SSOT.md) | 唯一の進捗正本。再開地点、検証ゲート、意思決定待ちを確認する |
| 何を作るゲームか | [GDD 1.0](docs/spec/GDD1.0.md) | ゲーム仕様の唯一の正本。編集しない |
| AIがどこまで自律実行するか | [AGENTS.md](AGENTS.md) | 不変のプロジェクト制約と優先順位 |
| 監修AIから実装AIへどう渡すか | [AI協調開発ルール](docs/Windsurf_AI_Collab_Rules_latest.md) | Execution Brief、停止条件、検証、創造的チェックポイント |
| 中長期の実装範囲 | [Roadmap v2](docs/dev/ROADMAP_v2.md) | スコープ地図。進捗判定には使わない |

## 現在のチェックポイント

2026-07-10 の監査では、Unity 6000.4.9f1 / URP 17.4.0 の C# ビルドは通過しています。一方、簡易Playable Loopのコードは `Sandbox.unity` に未配線で、PlayMode検証は未実施です。GDD §9.1 のMVPやバーティカルスライス全体を完成済みとは扱いません。詳細と最新状態は常に [WORKFLOW_STATE_SSOT](docs/WORKFLOW_STATE_SSOT.md) を参照してください。

## ローカルで再開する

```powershell
git pull --ff-only
git submodule update --init --recursive
```

Unity Hub から `99PercentSlops` を Unity `6000.4.9f1` で開きます。速いC#確認はリポジトリルートで次を実行します。

```powershell
dotnet build 99PercentSlops/Assembly-CSharp.csproj -nologo
```

このREADMEはGitHub上の安定した入口です。WikiやPagesを採用する場合も、現在地を手作業で複製せず、`WORKFLOW_STATE_SSOT.md`への導線または自動生成ビューとして扱います。
