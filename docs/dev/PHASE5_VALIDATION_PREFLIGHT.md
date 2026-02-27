# Phase 5 Validation Preflight

## Purpose
Unity manual validation for `TASK_025` is currently blocked by scene setup gaps. This file defines the minimum checks and the recommended order to unblock verification in `Sandbox.unity`.

## Current Blockers
| ID | Blocker | Current Finding | Impact | Recommended Response |
| --- | --- | --- | --- | --- |
| B-01 | UploadPort object missing from scene | `Sandbox.unity` does not contain a `GlitchWorker.Gimmicks.UploadPort` component reference | `V-01` to `V-06` cannot run | Create one scene object with `UploadPort` and a trigger collider |
| B-02 | HUD presenter missing from scene | `Sandbox.unity` does not contain a `GlitchWorker.UI.GameplayHudPresenter` component reference | Progress/state reset cannot be observed | Add one HUD root with `GameplayHudPresenter` and wire references |
| B-03 | Accepted prop not prepared for smoke test | `AIProp_Chair` starts as `CurrentState = Dormant` | Accepted-path validation cannot be tested immediately | For smoke only, set one AI prop to `Normalized` in Inspector before Play, or use in-game normalization flow |
| B-04 | Non-target prop typing was inconsistent | `HumanProp_Stone` had `_propType: AI` in scene data | Rejection test could become misleading | Fixed in scene YAML to `_propType: Human` |

## Confirmation Procedure
| Step | Where to look | What to confirm | Pass condition |
| --- | --- | --- | --- |
| C-01 | Hierarchy search: `UploadPort` | Scene object exists | One object is found |
| C-02 | Inspector on that object | `UploadPort (Script)` is attached | Script is visible and enabled |
| C-03 | Same object | Collider is `Is Trigger = true` | Trigger volume exists |
| C-04 | Hierarchy search: `GameplayHudPresenter` or HUD root | HUD object exists | One HUD presenter is found |
| C-05 | HUD Inspector | `_uploadPort`, `_progressText`, `_stateText`, `_restartHintPanel` are assigned | No missing references |
| C-06 | `AIProp_Chair` Inspector | `Prop Type = AI`, `Current State = Normalized` for smoke | Accepted prop ready |
| C-07 | `HumanProp_Stone` Inspector | `Prop Type = Human` | Rejection prop ready |

## Minimal Unity Setup
| Order | Action | Notes |
| --- | --- | --- |
| 1 | Add `UploadPort` object near the play area | Use a visible cube/plane plus trigger collider for easy testing |
| 2 | Attach `UploadPort.cs` | Keep default accepted type/state unless design changes |
| 3 | Add minimal Canvas/HUD root | No layout polish needed |
| 4 | Attach `GameplayHudPresenter.cs` | Wire `UploadPort` and TMP labels |
| 5 | Prepare one accepted prop and one rejected prop | `AIProp_Chair = Normalized`, `HumanProp_Stone = Human` |
| 6 | Run `TASK_025` matrix `V-01` to `V-06` | Record results in `REPORT_025` |

## MCP Use
| Case | Needed | Reason |
| --- | --- | --- |
| Manual scene inspection only | No | Unity Editor alone is enough |
| Automated scene/object inspection or scripted setup | Maybe | Useful only if Unity MCP server is already available |
| Current blocker resolution | No | Missing scene wiring can be confirmed manually faster |

## Notes
- This preflight does not replace `TASK_025`; it reduces setup ambiguity before manual validation.
- Do not expand gameplay scope here. The goal is only to make the existing loop testable.
