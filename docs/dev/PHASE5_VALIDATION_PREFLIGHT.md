# Phase 5 Validation Preflight

## Purpose
Unity manual validation for `TASK_025` is currently blocked by scene setup gaps. This file defines the minimum checks and the recommended order to unblock verification in `Sandbox.unity`.

## Current Blockers
| ID | Blocker | Current Finding | Impact | Recommended Response |
| --- | --- | --- | --- | --- |
| B-00 | GameplayLoopController missing from scene | `Sandbox.unity` does not contain a `GameplayLoopController` component, and `GameManager._gameplayLoopController` is not serialized | State transitions and all V-01 to V-06 checks cannot run | Add the controller and assign the GameManager reference before validation |
| B-01 | UploadPort object missing from scene | `Sandbox.unity` does not contain a `GlitchWorker.Gimmicks.UploadPort` component reference | `V-01` to `V-06` cannot run | Create one scene object with `UploadPort` and a trigger collider |
| B-02 | HUD presenter missing from scene | `Sandbox.unity` does not contain a `GlitchWorker.UI.GameplayHudPresenter` component reference | Progress/state reset cannot be observed | Add one HUD root with `GameplayHudPresenter` and wire references |
| B-03 | UploadPort acceptance conflicts with GDD | Code defaults to `AI / Normalized`, while GDD §3.3 and the glossary define Human Prop as the delivery target | A passing smoke test would certify the wrong product flow | Align UploadPort with Human Prop before scene wiring; use `HumanProp_Stone` as accepted |
| B-04 | Rejected prop must be explicit | `AIProp_Chair` is `AI / Dormant` | Rejection evidence is ambiguous unless the product contract is fixed first | Use `AIProp_Chair` as the rejected prop after B-03 is corrected |

## Confirmation Procedure
| Step | Where to look | What to confirm | Pass condition |
| --- | --- | --- | --- |
| C-00 | `GameManager` Inspector | `GameplayLoopController` is attached and `_gameplayLoopController` is assigned | One enabled controller and no missing GameManager reference |
| C-01 | Hierarchy search: `UploadPort` | Scene object exists | One object is found |
| C-02 | Inspector on that object | `UploadPort (Script)` is attached | Script is visible and enabled |
| C-03 | Same object | Collider is `Is Trigger = true` | Trigger volume exists |
| C-04 | Hierarchy search: `GameplayHudPresenter` or HUD root | HUD object exists | One HUD presenter is found |
| C-05 | HUD Inspector | `_uploadPort`, `_progressText`, `_stateText`, `_restartHintPanel` are assigned | No missing references |
| C-06 | `HumanProp_Stone` Inspector | `Prop Type = Human`; UploadPort accepts the Human contract | Accepted prop ready |
| C-07 | `AIProp_Chair` Inspector | `Prop Type = AI` | Rejection prop ready |

## Minimal Unity Setup
| Order | Action | Notes |
| --- | --- | --- |
| 1 | Add `GameplayLoopController` and assign it to `GameManager` | Establish the state source before objective/UI wiring |
| 2 | Add `UploadPort` object near the play area | Use a visible cube/plane plus trigger collider for easy testing |
| 3 | Attach `UploadPort.cs` | Do not keep the current `AI / Normalized` defaults. Align the accepted type with the GDD Human Prop contract; decide whether state is ignored or fixed to the scene's Human Prop state within the Scene Integration Batch |
| 4 | Add minimal Canvas/HUD root | No layout polish needed |
| 5 | Attach `GameplayHudPresenter.cs` | Wire `UploadPort` and TMP labels |
| 6 | Prepare one accepted prop and one rejected prop | `HumanProp_Stone = accepted`, `AIProp_Chair = rejected` |
| 7 | Run `TASK_025` matrix `V-01` to `V-06` | Record results in `REPORT_025` |

## MCP Use
| Case | Needed | Reason |
| --- | --- | --- |
| Manual scene inspection only | No | Unity Editor alone is enough |
| Automated scene/object inspection or scripted setup | Maybe | Useful only if Unity MCP server is already available |
| Current blocker resolution | No | Missing scene wiring can be confirmed manually faster |

## Notes
- This preflight does not replace `TASK_025`; it reduces setup ambiguity before manual validation.
- Do not expand gameplay scope here. The goal is only to make the existing loop testable.
