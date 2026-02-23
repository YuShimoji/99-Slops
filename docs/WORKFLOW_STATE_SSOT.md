# WORKFLOW_STATE_SSOT

## Meta
- Last Updated: 2026-02-23T04:20:45+09:00
- Owner: Orchestrator
- Source of Truth: `shared-workflows/docs/windsurf_workflow/EVERY_SESSION.md`

## Current Context
- Current Phase: P5 (Worker Dispatch Preparation)
- Active Task: `TASK_015_Camera_Closeout_Integration`
- Test Phase: Hardening
- Branch: `feature/task-015-camera-closeout-integration`

## Layer Split (Verification Gate)
- Layer A (AI-completable): DONE
  - 再ディスパッチプロンプトを作成
  - 必須証跡（EditMode/PlayMode/Build: XML/LOG/exitcode）を定義
  - チケットステータスを `IN_PROGRESS` に差し戻し
- Layer B (manual / runtime execution): TODO
  - WorkerがUnity Test RunnerとBuildを実行
  - 証跡ファイルを `docs/inbox/artifacts/TASK_015_20260223/` に格納
  - 証跡照合後にゲート判定更新

## Blocked Normal Form
- Blocker Type: Environment / Verification Integrity
- Blocked Scope: `TASK_015` Layer B
- AI-Completable Scope (Layer A): 実行手順と提出物定義までは完了
- User Runbook (Layer B): Workerに再ディスパッチPromptを投入し、XML/LOG/exitcodeを回収
- Resume Trigger: `REPORT_TASK_015_Camera_Closeout_Integration_20260223_RETRY.md` と必須証跡が提出される
- Re-proposal Suppression:
  - 同一ブロッカー（証跡欠落）が継続する場合は新規タスク起票を行わず、同じ再投入手順を再提示する

## Next Action
- `docs/inbox/WORKER_PROMPT_TASK_015_Camera_Closeout_Integration_REDISPATCH_20260223.md` を Worker に投入する

