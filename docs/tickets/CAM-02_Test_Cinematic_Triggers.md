# Ticket: CAM-02 Test Cinematic Triggers

**Priority:** High
**Type:** Manual Test
**Status:** Open

## Description

Verify that entering a `CinematicCameraZone` activates the Cinematic mode and moves the camera to the specified point.

## Acceptance Criteria

- [ ] Create a test scene with a `CinematicCameraZone`.
- [ ] Entering the trigger moves the camera to the target `CameraPoint`.
- [ ] Exiting the trigger returns the camera to the previous mode.
- [ ] Transitions use the specified `ModeTransitionTime`.

## Notes

- Relevant file: `CinematicCameraZone.cs`, `CameraManager.cs`
