# Ticket: CAM-04 Input System Integration

**Priority:** Medium
**Type:** Code Review
**Status:** Open

## Description

Confirm that `PlayerController.OnLook` receives correct Delta values from the Input System and that sensitivity settings feel natural.

## Acceptance Criteria

- [ ] `PlayerController.OnLook` reads `context.ReadValue<Vector2>()` correctly.
- [ ] Input smoothing in `CameraManager` works as expected.
- [ ] `SensitivityX` and `SensitivityY` in `CameraSettings` have a noticeable effect.

## Notes

- Relevant file: `PlayerController.cs`
- Verify `InputSystem_Actions.inputactions`.
