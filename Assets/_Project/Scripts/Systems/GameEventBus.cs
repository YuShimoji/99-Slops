using System;
using GlitchWorker.Camera;
using GlitchWorker.Props;
using UnityEngine;

namespace GlitchWorker.Systems
{
    /// <summary>
    /// Lightweight runtime event bus for prototype-level system decoupling.
    /// </summary>
    public static class GameEventBus
    {
        public static event Action<bool> DebugViewToggled;
        public static event Action<PropBase, PropState, PropState> PropStateChanged;
        public static event Action<Rigidbody, PropBase> BeamGrabbed;
        public static event Action<Rigidbody, PropBase> BeamReleased;
        public static event Action<Rigidbody, PropBase> BeamThrown;
        public static event Action<CameraViewMode, CameraViewMode> CameraModeChanged;
        public static event Action<Transform, CinematicCameraZone> CinematicEntered;
        public static event Action CinematicExited;

        public static void RaiseDebugViewToggled(bool isActive)
        {
            DebugViewToggled?.Invoke(isActive);
        }

        public static void RaisePropStateChanged(PropBase prop, PropState previousState, PropState newState)
        {
            PropStateChanged?.Invoke(prop, previousState, newState);
        }

        public static void RaiseBeamGrabbed(Rigidbody heldBody, PropBase heldProp)
        {
            BeamGrabbed?.Invoke(heldBody, heldProp);
        }

        public static void RaiseBeamReleased(Rigidbody releasedBody, PropBase releasedProp)
        {
            BeamReleased?.Invoke(releasedBody, releasedProp);
        }

        public static void RaiseBeamThrown(Rigidbody thrownBody, PropBase thrownProp)
        {
            BeamThrown?.Invoke(thrownBody, thrownProp);
        }

        public static void RaiseCameraModeChanged(CameraViewMode previousMode, CameraViewMode newMode)
        {
            CameraModeChanged?.Invoke(previousMode, newMode);
        }

        public static void RaiseCinematicEntered(Transform cameraPoint, CinematicCameraZone zone)
        {
            CinematicEntered?.Invoke(cameraPoint, zone);
        }

        public static void RaiseCinematicExited()
        {
            CinematicExited?.Invoke();
        }
    }
}
