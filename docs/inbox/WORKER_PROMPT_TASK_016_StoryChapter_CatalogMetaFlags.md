# Worker Prompt: TASK_016_StoryChapter_CatalogMetaFlags

## 参照
- チケット: docs/tasks/TASK_016_StoryChapter_CatalogMetaFlags.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md（無ければ `shared-workflows/docs/` を参照）
- HANDOVER: docs/HANDOVER.md
- Worker Metaprompt: shared-workflows/prompts/every_time/WORKER_METAPROMPT.txt
- Mission Log: .cursor/MISSION_LOG.md
- Unity必読: docs/02_design/ASSEMBLY_ARCHITECTURE.md, docs/03_guides/UNITY_CODE_STANDARDS.md, docs/03_guides/COMPILATION_GUARD_PROTOCOL.md

## 前提
- Tier: 2 (Foundation)
- Branch: feature/task-016-story-catalog-metaflags
- Test Phase: Stable
- Target Assemblies: GlitchWorker.Runtime, Tests.EditMode, Tests.PlayMode
- Report Target: docs/inbox/REPORT_TASK_016_StoryChapter_CatalogMetaFlags_20260222.md
- GitHubAutoApprove: docs/HANDOVER.md の記述を参照

## 境界
- Focus Area:
  - Assets/_Project/Scripts/Story (new folder allowed)
  - Assets/_Project/Scripts/Systems
  - Assets/Tests
- Forbidden Area:
  - Camera/Player/Drone behavior changes
  - Large scene refactors
  - ProjectSettings/
  - Packages/

## 目的
- MG-2 の基盤として、データ駆動の ChapterDefinition / ChapterCatalog と MetaFlagService を実装する。
- `TASK_017` が参照する最小契約を固定し、後続の VariantResolver 実装を開始可能にする。

## Test Plan
- テスト対象:
  - Chapter catalog registration/query
  - Meta-flag set/get/reset
  - Undefined ID handling
- EditMode:
  - Chapter catalog registration/query tests
  - Meta-flag set/get/reset tests
  - Undefined key behavior tests
- PlayMode:
  - Startup data loading smoke test
- Build:
  - C# compile clean check
- 期待結果:
  - Stable基準として EditMode/PlayMode/Build の結果が再現可能な形で記録される

## DoD
- [ ] Multiple chapter definitions can be registered and queried
- [ ] Meta-flag read/write API works as expected
- [ ] Undefined ID handling policy is documented (log vs fallback)
- [ ] Core logic is covered by EditMode tests
- [ ] Report includes implementation notes and usage steps
- [ ] Unity Editor compile/build result is recorded (`Unity Editor=コンパイル成功` を含む)

## Impact Radar
- コード: Story domain 新規クラス群の導入
- テスト: Story API契約テストの初期整備
- パフォーマンス: 起動時データロード負荷の最小確認
- UX: 直接UI影響は限定、後続タスクで露出
- 連携: TASK_017/TASK_018 の前提APIを固定

## 制約
- Use ScriptableObject-driven data where possible
- Avoid scattered string literals for chapter and flag IDs
- Keep coupling to existing GameManager low

## 停止条件
- Forbidden Area に触れないと解決できない
- 仕様仮定が3件以上
- 依存追加 / 外部通信が必要で GitHubAutoApprove=true が未確認
- 破壊的操作が必要
- SSOT が取得できない
- chapter requirements が不安定で仕様衝突
- Unity固有: ProjectSettings/, Packages/ の変更が必要
- Unity固有: Unity Editor起動が必要な長時間待機

## 納品先
- docs/inbox/REPORT_TASK_016_StoryChapter_CatalogMetaFlags_20260222.md

---
Worker Metaprompt の Phase 0〜Phase 4 に従って実行してください。
チャット報告は固定3セクション（結果 / 変更マップ / 次の選択肢）で出力してください。
