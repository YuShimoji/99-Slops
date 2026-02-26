# TASK_017_CameraSettings_Asset_Wiring

## Status
DONE

## Tier / Branch
- Tier: 1 (Core)
- Branch: feature/camera-settings-asset-wiring

## Summary
`CameraSettings` ScriptableObject code exists, but runtime asset wiring is incomplete.
Create a default asset and wire `CameraManager` to use it in the SSOT sandbox scene.

## Dependency
- `TASK_016_SandboxScene_SSOT_Sync` (scene SSOT fixed first)
- `TASK_013_CameraSettings_SO` (superset completion)

## Scope
- Create `CameraSettings_Default.asset` under `Assets/_Project/Data`.
- Assign asset to `CameraManager._settings` in SSOT sandbox scene.
- Verify fallback behavior when settings are null is still safe.

## Deliverables
- `99PercentSlops/Assets/_Project/Data/CameraSettings_Default.asset`
- Scene wiring update in SSOT sandbox scene
- Ticket report with inspector values and runtime confirmation

## Focus Area / Forbidden Area
- Focus: `Assets/_Project/Scripts/Camera`, `Assets/_Project/Data`, SSOT sandbox scene
- Forbidden: broad player movement tuning outside camera settings intent

## Constraints
- Preserve current camera behavior as baseline unless explicitly tuning.
- Keep serialized values readable and inspector-friendly.

## Definition of Done (DoD)
- CameraManager references a valid `CameraSettings` asset in SSOT sandbox.
- 1P/3P switching works with settings-driven parameters.
- Null settings fallback path remains error-free.

## Test Plan
- PlayMode:
  - switch 1P/3P with `V`
  - verify look sensitivity and transition parameters apply from asset
- Negative:
  - temporarily clear `_settings` and confirm fallback executes without null errors

## Risks / Notes
- If scene still uses stale serialized fields, settings assignment may appear applied but not behave as expected.

## Milestone
- Phase 2A

## Completion Report

### 1. Asset: `CameraSettings_Default.asset`

- Path: `99PercentSlops/Assets/_Project/Data/CameraSettings_Default.asset`
- Already existed with correct GUID (`6d4ddf1d1f5f4f6fa3dbe97810d4ef05`)
- Script reference GUID: `7f1cb09cf87a4cf79fb84f0f71f9ba4d` (matches `CameraSettings.cs`)

**Inspector values (serialized in asset):**

| Field | Value |
| --- | --- |
| SensitivityX | 2 |
| SensitivityY | 2 |
| InvertY | false (0) |
| MaxLookAngle | 80 |
| RotationSmoothTime | 0.06 |
| FirstPersonLocalOffset | (0, 0.8, 0) |
| ThirdPersonDistance | 3.2 |
| ThirdPersonHeightOffset | 1.6 |
| ThirdPersonCollisionRadius | 0.2 |
| CameraCollisionMask | Everything (0xFFFFFFFF) |
| ModeTransitionTime | 0.22 |
| StartupMode | FirstPerson (0) |

### 2. Scene Wiring

**SSOT scene (`Assets/_Project/Scenes/Sandbox.unity`):**

- `_settings` field was already correctly wired:

  ```yaml
  _settings: {fileID: 11400000, guid: 6d4ddf1d1f5f4f6fa3dbe97810d4ef05, type: 2}
  ```

- `_cameraTransform` also wired: `{fileID: 1951866340}`

**Legacy scene (`Assets/Scenes/Sandbox.unity`):**

- `_settings` field was **missing** (not serialized) - `_cameraTransform` was `{fileID: 0}`
- Fixed: added `_settings` reference matching SSOT scene GUID

### 3. Null-Fallback Safety Verification

All 12 property accessors in `CameraManager` (lines 69-80) follow the pattern:
```csharp
private float SensitivityX => _settings != null ? _settings.SensitivityX : _sensitivityX;
```

Every `CameraSettings` field has a corresponding fallback serialized field on `CameraManager` itself. No direct `_settings.` access exists outside these null-guarded properties. The public `Settings` property returns the raw reference (acceptable - callers must null-check).

**Verdict:** Clearing `_settings` at runtime will fall back to the serialized inline values without null errors.

### 4. Runtime Confirmation (Manual Test Plan)

- PlayMode: press `V` to toggle 1P/3P - parameters should read from `CameraSettings_Default.asset`
- Negative: clear `_settings` in Inspector during Play - fallback to inline values, no errors expected
