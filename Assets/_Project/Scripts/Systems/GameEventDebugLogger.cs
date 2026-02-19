using GlitchWorker.Camera;
using GlitchWorker.Props;
using UnityEngine;

namespace GlitchWorker.Systems
{
    /// <summary>
    /// Optional scene helper to verify event wiring in play mode.
    /// </summary>
    public class GameEventDebugLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEventBus.DebugViewToggled += OnDebugViewToggled;
            GameEventBus.PropStateChanged += OnPropStateChanged;
            GameEventBus.BeamGrabbed += OnBeamGrabbed;
            GameEventBus.BeamReleased += OnBeamReleased;
            GameEventBus.BeamThrown += OnBeamThrown;
            GameEventBus.CameraModeChanged += OnCameraModeChanged;
        }

        private void OnDisable()
        {
            GameEventBus.DebugViewToggled -= OnDebugViewToggled;
            GameEventBus.PropStateChanged -= OnPropStateChanged;
            GameEventBus.BeamGrabbed -= OnBeamGrabbed;
            GameEventBus.BeamReleased -= OnBeamReleased;
            GameEventBus.BeamThrown -= OnBeamThrown;
            GameEventBus.CameraModeChanged -= OnCameraModeChanged;
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

        private static void OnCameraModeChanged(CameraViewMode previousMode, CameraViewMode newMode)
        {
            Debug.Log($"[GameEventBus] CameraModeChanged: {previousMode} -> {newMode}");
        }
    }
}
