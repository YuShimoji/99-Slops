using System;
using System.Collections;
using System.IO;
using System.Linq;
using GlitchWorker.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace GlitchWorker.StudioPilot
{
    // Attached only to a temporary copy of the pilot scene by StudioPilotRuntimeBuild.
    public sealed class StudioPilotRuntimeProbe : MonoBehaviour
    {
        private void Awake()
        {
            // Keep the probe advancing if focus briefly moves during the test.
            Application.runInBackground = true;
        }

        [Serializable]
        private sealed class Receipt
        {
            public string scene, unityVersion, graphicsDevice, actionMap, playerState, moveState, jumpState, cameraName;
            public bool inputActive, moveEnabled, jumpEnabled, grounded, moveGrounded, jumpGrounded;
            public string controlScheme, probeInput;
            public string[] topMaterials, storageMaterials;
            public float[] initialPlayer, afterMovePlayer, afterJumpPlayer, cameraPosition;
            public float[] furnitureViewportCenter, furnitureViewportBounds;
            public float movementMetres, jumpRiseMetres, moveActionPeak;
            public int furnitureRenderers, moveFrames, jumpFrames, jumpTriggeredFrames,
                imageWidth = 960, imageHeight = 540;
        }

        private IEnumerator Start()
        {
            var output = Environment.GetEnvironmentVariable("STUDIO_PILOT_PROBE_DIR");
            if (string.IsNullOrWhiteSpace(output) || !Path.IsPathRooted(output))
            {
                Debug.LogError("STUDIO_PILOT_PROBE_DIR must be an absolute path.");
                Application.Quit(2);
                yield break;
            }

            if (Directory.Exists(output) && Directory.EnumerateFileSystemEntries(output).Any())
            {
                Debug.LogError("Studio pilot probe output must be empty: " + output);
                Application.Quit(2);
                yield break;
            }
            Directory.CreateDirectory(output);

            // Let the existing Player, CameraManager and physics settle before measuring them.
            for (var frame = 0; frame < 20; frame++) yield return null;
            var player = GameObject.Find("Player");
            var furniture = GameObject.Find("StudioPilot_f0008_TechnicalOnly");
            var camera = UnityEngine.Camera.main;
            var input = player != null ? player.GetComponent<PlayerInput>() : null;
            var controller = player != null ? player.GetComponent<PlayerController>() : null;
            var jump = player != null ? player.GetComponent<PlayerJump>() : null;
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (player == null || furniture == null || camera == null || input == null ||
                controller == null || jump == null || keyboard == null || mouse == null ||
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                Debug.LogError("Studio pilot Player, furniture, camera, input or graphics device is missing. " +
                               "keyboard=" + (keyboard != null) + " graphics=" + SystemInfo.graphicsDeviceType);
                Application.Quit(2);
                yield break;
            }

            var renderers = furniture.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogError("Studio pilot furniture has no renderers.");
                Application.Quit(2);
                yield break;
            }

            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
            var min = bounds.min;
            var max = bounds.max;
            var corners = new[] {
                new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, max.y, max.z)
            }.Select(camera.WorldToViewportPoint).ToArray();
            var center = camera.WorldToViewportPoint(bounds.center);
            var receipt = new Receipt {
                scene = SceneManager.GetActiveScene().name,
                unityVersion = Application.unityVersion,
                graphicsDevice = SystemInfo.graphicsDeviceName,
                actionMap = input.currentActionMap != null ? input.currentActionMap.name : "",
                inputActive = input.inputIsActive,
                controlScheme = input.currentControlScheme,
                probeInput = "queued W and Space on existing Keyboard device",
                moveEnabled = input.actions.FindAction("Player/Move")?.enabled == true,
                jumpEnabled = input.actions.FindAction("Player/Jump")?.enabled == true,
                playerState = controller.CurrentStateType.ToString(),
                cameraName = camera.name,
                grounded = jump.IsGrounded,
                initialPlayer = Position(player.transform.position),
                cameraPosition = Position(camera.transform.position),
                furnitureRenderers = renderers.Length,
                topMaterials = DescribeMaterials(renderers.Single(renderer => renderer.name == "Top")),
                storageMaterials = DescribeMaterials(renderers.Single(renderer => renderer.name == "CabinetOuter")),
                furnitureViewportCenter = Position(center),
                furnitureViewportBounds = new[] { corners.Min(point => point.x), corners.Min(point => point.y),
                    corners.Max(point => point.x), corners.Max(point => point.y) }
            };

            try { Capture(camera, Path.Combine(output, "player-initial.png")); }
            catch (Exception error)
            {
                Debug.LogException(error);
                Application.Quit(2);
                yield break;
            }

            // Drive the existing PlayerInput action, controller and Rigidbody using keyboard state events.
            // The ordinary pilot scene is never changed or given this component.
            var moveAction = input.actions.FindAction("Player/Move");
            var jumpAction = input.actions.FindAction("Player/Jump");
            var moveStart = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - moveStart < 0.2f)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
                receipt.moveActionPeak = Mathf.Max(receipt.moveActionPeak, moveAction.ReadValue<Vector2>().magnitude);
                receipt.moveFrames++;
                yield return null;
            }
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            var settleStart = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - settleStart < 0.2f) yield return null;
            receipt.afterMovePlayer = Position(player.transform.position);
            receipt.moveState = controller.CurrentStateType.ToString();
            receipt.moveGrounded = jump.IsGrounded;
            receipt.movementMetres = Vector3.Distance(
                new Vector3(receipt.initialPlayer[0], 0f, receipt.initialPlayer[2]),
                new Vector3(receipt.afterMovePlayer[0], 0f, receipt.afterMovePlayer[2]));

            var baseY = player.transform.position.y;
            var maxY = baseY;
            var pressStart = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - pressStart < 0.08f)
            {
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Space));
                if (jumpAction.triggered) receipt.jumpTriggeredFrames++;
                maxY = Mathf.Max(maxY, player.transform.position.y);
                receipt.jumpFrames++;
                yield return null;
            }
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            var jumpStart = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - jumpStart < 0.5f)
            {
                maxY = Mathf.Max(maxY, player.transform.position.y);
                yield return null;
            }
            receipt.afterJumpPlayer = Position(player.transform.position);
            receipt.jumpState = controller.CurrentStateType.ToString();
            receipt.jumpGrounded = jump.IsGrounded;
            receipt.jumpRiseMetres = maxY - baseY;
            File.WriteAllText(Path.Combine(output, "receipt.json"), JsonUtility.ToJson(receipt, true));
            Debug.Log("STUDIO_PILOT_PLAYER_PROBE_COMPLETE move=" + receipt.movementMetres +
                      " jump=" + receipt.jumpRiseMetres + " graphics=" + receipt.graphicsDevice);
            Application.Quit(0);
        }

        private static float[] Position(Vector3 point) => new[] { point.x, point.y, point.z };

        private static string[] DescribeMaterials(Renderer renderer) => renderer.sharedMaterials.Select(material =>
            material == null ? "null" : material.name + "|" + material.shader.name + "|" +
            material.shader.isSupported + "|" +
            (material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor").ToString() : "no-base-color"))
            .ToArray();

        private static void Capture(UnityEngine.Camera camera, string path)
        {
            const int width = 960, height = 540;
            var previous = RenderTexture.active;
            var previousTarget = camera.targetTexture;
            var target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            Texture2D image = null;
            try
            {
                target.Create();
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                image = new Texture2D(width, height, TextureFormat.RGBA32, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(path, image.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previous;
                target.Release();
                Destroy(target);
                if (image != null) Destroy(image);
            }
        }
    }
}
