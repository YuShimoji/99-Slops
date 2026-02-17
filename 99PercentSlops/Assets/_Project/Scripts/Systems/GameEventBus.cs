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
        public static event Action<CameraViewMode, CameraViewMode> CameraViewModeChanged;
        public static event Action<Transform> CinematicEntered;
        public static event Action<CameraViewMode> CinematicExited;

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

        public static void RaiseCameraViewModeChanged(CameraViewMode previousMode, CameraViewMode nextMode)
        {
            if (previousMode == nextMode) return;
            int listenerCount = CameraViewModeChanged?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCameraViewModeChanged: {previousMode} -> {nextMode} (listeners: {listenerCount})");
            CameraViewModeChanged?.Invoke(previousMode, nextMode);
        }

        public static void RaiseCinematicEntered(Transform cinematicPoint)
        {
            string pointName = cinematicPoint != null ? cinematicPoint.name : "null";
            int listenerCount = CinematicEntered?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCinematicEntered: {pointName} (listeners: {listenerCount})");
            CinematicEntered?.Invoke(cinematicPoint);
        }

        public static void RaiseCinematicExited(CameraViewMode restoredMode)
        {
            int listenerCount = CinematicExited?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCinematicExited: {restoredMode} (listeners: {listenerCount})");
            CinematicExited?.Invoke(restoredMode);
        }
    }
}
