using GlitchWorker.Props;
using UnityEngine;

namespace GlitchWorker.Systems
{
    /// <summary>
    /// Optional scene helper to verify event wiring in play mode.
    /// </summary>
    public class GameEventDebugLogger : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[GameEventDebugLogger] Awake on '{gameObject.name}' in scene '{gameObject.scene.name}'");
        }

        private void OnEnable()
        {
            Debug.Log("[GameEventDebugLogger] OnEnable - subscribing to events");
            GameEventBus.DebugViewToggled += OnDebugViewToggled;
            GameEventBus.PropStateChanged += OnPropStateChanged;
            GameEventBus.BeamGrabbed += OnBeamGrabbed;
            GameEventBus.BeamReleased += OnBeamReleased;
            GameEventBus.BeamThrown += OnBeamThrown;
            GameEventBus.CameraViewModeChanged += OnCameraViewModeChanged;
            GameEventBus.CinematicEntered += OnCinematicEntered;
            GameEventBus.CinematicExited += OnCinematicExited;
        }

        private void OnDisable()
        {
            GameEventBus.DebugViewToggled -= OnDebugViewToggled;
            GameEventBus.PropStateChanged -= OnPropStateChanged;
            GameEventBus.BeamGrabbed -= OnBeamGrabbed;
            GameEventBus.BeamReleased -= OnBeamReleased;
            GameEventBus.BeamThrown -= OnBeamThrown;
            GameEventBus.CameraViewModeChanged -= OnCameraViewModeChanged;
            GameEventBus.CinematicEntered -= OnCinematicEntered;
            GameEventBus.CinematicExited -= OnCinematicExited;
        }

        private static void OnDebugViewToggled(bool isActive)
        {
            Debug.Log($"[GameEventBus] DebugView toggled: {isActive}");
        }

        private static void OnPropStateChanged(PropBase prop, PropState previousState, PropState newState)
        {
            string name = prop != null ? prop.name : "null";
            Debug.Log($"[GameEventBus] PropStateChanged: {name} {previousState} -> {newState}");
        }

        private static void OnBeamGrabbed(Rigidbody body, PropBase prop)
        {
            string bodyName = body != null ? body.name : "null";
            string propName = prop != null ? prop.name : "none";
            Debug.Log($"[GameEventBus] BeamGrabbed: body={bodyName}, prop={propName}");
        }

        private static void OnBeamReleased(Rigidbody body, PropBase prop)
        {
            string bodyName = body != null ? body.name : "null";
            string propName = prop != null ? prop.name : "none";
            Debug.Log($"[GameEventBus] BeamReleased: body={bodyName}, prop={propName}");
        }

        private static void OnBeamThrown(Rigidbody body, PropBase prop)
        {
            string bodyName = body != null ? body.name : "null";
            string propName = prop != null ? prop.name : "none";
            Debug.Log($"[GameEventBus] BeamThrown: body={bodyName}, prop={propName}");
        }

        private static void OnCameraViewModeChanged(GlitchWorker.Camera.CameraViewMode previousMode, GlitchWorker.Camera.CameraViewMode nextMode)
        {
            Debug.Log($"[GameEventBus] CameraViewModeChanged: {previousMode} -> {nextMode}");
        }

        private static void OnCinematicEntered(Transform point)
        {
            string pointName = point != null ? point.name : "null";
            Debug.Log($"[GameEventBus] CinematicEntered: point={pointName}");
        }

        private static void OnCinematicExited(GlitchWorker.Camera.CameraViewMode restoredMode)
        {
            Debug.Log($"[GameEventBus] CinematicExited: restore={restoredMode}");
        }
    }
}
