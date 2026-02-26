# Worker Prompt: TASK_018_CameraEvents_Cinematic_Validation

## 参照
- チケット: docs/tasks/TASK_018_CameraEvents_Cinematic_Validation.md
- SSOT: docs/Windsurf_AI_Collab_Rules_latest.md
- HANDOVER: docs/HANDOVER.md
- MISSION_LOG: .cursor/MISSION_LOG.md

## 境界
- Tier / Branch: Tier 1 (Core) / feature/camera-events-cinematic-validation
- Focus Area: Assets/_Project/Scripts/Systems, Assets/_Project/Scripts/Camera, SSOT sandbox scene
- Forbidden Area: unrelated AI/gameplay feature expansion

## Test Plan
- テスト対象: CameraViewModeChanged / CinematicEntered / CinematicExited の発火回数と順序
- テスト種別:
  - 先行実施: EditMode・静的確認・ログ重複防止確認
  - 保留: PlayMode手動検証（開発優先のため後続へ繰り延べ）
- 期待結果:
  - 1P/3P切替で CameraViewModeChanged が実切替ごとに1回
  - Cinematic enter/exit で対応イベントが重複なし
  - null reference error なし

## Impact Radar
- コード: Camera/Event周辺スクリプトの購読/発火経路
- テスト: 自動検証追加による既存テスト影響
- パフォーマンス: ロガー追加によるログ量増加
- UX: カメラ遷移の体感変化有無
- 連携: Scene内の重複Manager/Logger配置との干渉

## Milestone
- Phase 2A closeout

## DoD
- [ ] SSOT sandbox scene に最小検証構成（Logger + Cinematic zone）を追加
- [ ] イベント重複が起きない実装/検証根拠をレポート化
- [ ] 実行した自動検証を `<cmd>=<result>` 形式で記録
- [ ] 手動検証保留理由と再開ポイントを明記

## 停止条件
- Forbidden Area へ波及しないと解決不可
- 仕様仮定が3件以上必要
- Unity Editor依存で自動化不能かつ代替根拠を作れない

## 納品先
- docs/inbox/REPORT_TASK_018_CameraEvents_Cinematic_Validation.md

## 実行メモ（Orchestrator方針）
- 今回は開発優先のため、手動PlayMode検証はスキップしてよい。
- ただしチケット/レポートには「未実施項目」と「後続手順」を必ず明記すること。
- 完了チャットは1行で: `Done` または `Blocked` + report path + tests summary。
