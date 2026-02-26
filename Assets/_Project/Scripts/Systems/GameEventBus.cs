using System;
using GlitchWorker.Camera;
using GlitchWorker.Props;
using UnityEngine;

namespace GlitchWorker.Systems
{
    public enum GameplayState
    {
        Playing,
        Cleared,
        Failed
    }

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
        // Alias for compatibility with newer naming in master.
        public static event Action<CameraViewMode, CameraViewMode> CameraViewModeChanged;
        public static event Action<Transform, CinematicCameraZone> CinematicEntered;
        public static event Action CinematicExited;
        public static event Action<GameplayState, GameplayState> GameplayStateChanged;

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
            if (previousMode == newMode) return;
            int listenerCount = CameraModeChanged?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCameraModeChanged: {previousMode} -> {newMode} (listeners: {listenerCount})");
            CameraModeChanged?.Invoke(previousMode, newMode);
            CameraViewModeChanged?.Invoke(previousMode, newMode);
        }

        public static void RaiseCinematicEntered(Transform cameraPoint, CinematicCameraZone zone)
        {
            string pointName = cameraPoint != null ? cameraPoint.name : "null";
            int listenerCount = CinematicEntered?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCinematicEntered: {pointName} (listeners: {listenerCount})");
            CinematicEntered?.Invoke(cameraPoint, zone);
        }

        public static void RaiseCinematicExited()
        {
            int listenerCount = CinematicExited?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseCinematicExited (listeners: {listenerCount})");
            CinematicExited?.Invoke();
        }

        public static void RaiseCameraViewModeChanged(CameraViewMode previousMode, CameraViewMode nextMode)
        {
            RaiseCameraModeChanged(previousMode, nextMode);
        }

        public static void RaiseGameplayStateChanged(GameplayState previousState, GameplayState newState)
        {
            if (previousState == newState) return;
            int listenerCount = GameplayStateChanged?.GetInvocationList().Length ?? 0;
            Debug.Log($"[GameEventBus] RaiseGameplayStateChanged: {previousState} -> {newState} (listeners: {listenerCount})");
            GameplayStateChanged?.Invoke(previousState, newState);
        }
    }
}
