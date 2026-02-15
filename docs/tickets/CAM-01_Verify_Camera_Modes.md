# Ticket: CAM-01 Verify Camera Modes

**Priority:** High
**Type:** Manual Test
**Status:** Open

## Description

Verify that switching between First Person and Third Person works smoothly in Play Mode.

## Acceptance Criteria

- [ ] Pressing the toggle key (confirm key in Input Actions) changes the camera view.
- [ ] No jitter or visual artifacts during transition.
- [ ] Offsets for both modes match `CameraSettings`.

## Notes

- Relevant file: `CameraManager.cs`
- Input Action: `TogglePerspective` (likely)
