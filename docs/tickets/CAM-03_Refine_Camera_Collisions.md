# Ticket: CAM-03 Refine Camera Collisions

**Priority:** Medium
**Type:** Code/Test
**Status:** Open

## Description

Verify that the Third Person camera correctly handles wall collisions.

## Acceptance Criteria

- [ ] Camera moves closer to player when backed against a wall.
- [ ] Camera returns to original distance when moving away from the wall.
- [ ] Clipping through geometry is minimized.
- [ ] `CameraCollisionMask` is correctly applied.

## Notes

- Relevant file: `ThirdPersonMode.cs`, `CameraSettings.cs`
- Check `ThirdPersonCollisionRadius` setting.
