# Turn-Based Development Plan

This page converts the current documentation state into work-session turns. It does not replace [Roadmap v2](ROADMAP_v2.md), [Workflow State SSOT](../WORKFLOW_STATE_SSOT.md), or the task files.

A turn means one focused implementation or verification session that can end with a clear local result.

## Immediate Turns

| Turn | Main bottleneck | Source documents | Finish condition | What becomes possible next |
|------|-----------------|------------------|------------------|----------------------------|
| Turn 1 | Unity validation is still gating final DONE decisions. | [TASK_025](../tasks/TASK_025_UnityDeferred_Validation_Batch.md), [Phase 5 Validation Preflight](PHASE5_VALIDATION_PREFLIGHT.md), [Resume](RESUME.md) | V-01 through V-06 have PASS/FAIL evidence recorded, with scene wiring notes. | `COMPLETED_CORE` items can be promoted or split into concrete follow-up fixes. |
| Turn 2 | Documentation state and task statuses need to reflect the validation result. | [Workflow State SSOT](../WORKFLOW_STATE_SSOT.md), [Milestone Plan](../MILESTONE_PLAN.md), [TASK_026](../tasks/TASK_026_ProjectCompletion_Assessment.md) | Status wording is aligned across state, milestone, and assessment docs. | The next implementation turn can start without re-litigating completion state. |
| Turn 3 | Phase 2A camera implementation is still described as the next foundation gap in the roadmap. | [Roadmap v2](ROADMAP_v2.md), [Camera System](../spec/CAMERA_SYSTEM.md), [Player/Camera phased plan](PLAYER_CAMERA_PHASED_IMPLEMENTATION_2026-02-09.md) | `ICameraMode`, smoothing, and 1P/3P mode behavior have an implementation task or completed change set. | Player orientation and tuning work can rely on the camera foundation. |
| Turn 4 | Player-control remaining work depends on camera completion and Unity tuning. | [Roadmap v2](ROADMAP_v2.md), [Player Control System](../spec/PLAYER_CONTROL_SYSTEM.md) | Third-person orientation and tuning checkpoints are implemented or explicitly deferred. | Polish and feedback work can start against a stable control surface. |

## Later Build Turns

| Turn | Main bottleneck | Source documents | Finish condition | What becomes possible next |
|------|-----------------|------------------|------------------|----------------------------|
| Turn 5 | Feedback and readability are still mostly future work. | [Roadmap v2](ROADMAP_v2.md) | Debug view, beam visuals, normalization feedback, and basic audio have tasks and validation notes. | The vertical slice can be judged by feel, not only code wiring. |
| Turn 6 | Cinematic and AI behavior need a focused implementation pass. | [Roadmap v2](ROADMAP_v2.md) | Cinematic camera volume and AI behavior foundation are implemented or split into smaller tasks. | Encounter design and capture-ready scenes can be evaluated. |
| Turn 7 | Extended systems are scoped as post-vertical-slice work. | [Roadmap v2](ROADMAP_v2.md), [GDD 1.0](../spec/GDD1.0.md), [Skill Ideas](../spec/SKILL_IDEAS.md) | Scope is explicitly accepted, deferred, or broken into future tasks. | Expansion work can proceed without blurring the MVP boundary. |

## Use Rules

- Prefer finishing Turn 1 and Turn 2 before starting new feature work.
- Keep turn boundaries outcome-based rather than date-based.
- If a turn produces visual evidence, place it under [Progress Screenshots](../screenshots/README.md) and link it from the relevant task/report.
- If a turn changes source behavior, update the task/report pair rather than only this page.
