using System;
using System.IO;
using System.Linq;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace GlitchWorker.EditorTools
{
    // A read-only camera check for the isolated Studio pilot scene.
    public static class StudioPilotCapture
    {
        [Serializable]
        private class Receipt
        {
            public string scene = "Assets/_Project/Scenes/StudioPilot_f0008.unity";
            public string camera;
            public string unityVersion;
            public string graphicsDevice;
            public float[] placement;
            public float[] viewportCenter, viewportBounds;
            public int width, height;
        }

        public static void Render()
        {
            RenderTexture target = null;
            Texture2D image = null;
            UnityEngine.Camera camera = null;
            RenderTexture previous = null;
            Transform pilotTransform = null;
            Vector3 originalPosition = Vector3.zero;
            try
            {
                var output = Environment.GetEnvironmentVariable("STUDIO_PILOT_CAPTURE_PATH");
                if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
                    throw new InvalidOperationException("Camera capture needs a graphics device; omit -nographics.");
                if (string.IsNullOrWhiteSpace(output) || !Path.IsPathRooted(output) ||
                    !output.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Set STUDIO_PILOT_CAPTURE_PATH to a new absolute .png path.");
                if (File.Exists(output) || File.Exists(output + ".json"))
                    throw new IOException("Capture already exists; refusing to overwrite it.");
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                StudioPilotBatch.Verify();
                var root = GameObject.Find("StudioPilot_f0008_TechnicalOnly");
                camera = GameObject.Find("Main Camera")?.GetComponent<UnityEngine.Camera>();
                if (root == null || camera == null)
                    throw new InvalidOperationException("Pilot root or Main Camera is missing.");
                pilotTransform = root.transform;
                originalPosition = pilotTransform.position;
                var proposedPosition = Environment.GetEnvironmentVariable("STUDIO_PILOT_CAPTURE_POSITION");
                if (!string.IsNullOrWhiteSpace(proposedPosition))
                {
                    var parts = proposedPosition.Split(',');
                    if (parts.Length != 3) throw new ArgumentException("Capture position must be x,y,z");
                    pilotTransform.position = new Vector3(
                        float.Parse(parts[0], CultureInfo.InvariantCulture),
                        float.Parse(parts[1], CultureInfo.InvariantCulture),
                        float.Parse(parts[2], CultureInfo.InvariantCulture));
                }
                var renderers = root.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0)
                    throw new InvalidOperationException("Pilot model has no renderers.");
                var bounds = renderers[0].bounds;
                foreach (var renderer in renderers.Skip(1)) bounds.Encapsulate(renderer.bounds);
                var min = bounds.min;
                var max = bounds.max;
                var corners = new[] {
                    new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z),
                    new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z),
                    new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z),
                    new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z)
                }.Select(camera.WorldToViewportPoint).ToArray();
                var center = camera.WorldToViewportPoint(bounds.center);
                const int width = 960, height = 540;
                target = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                target.Create();
                previous = RenderTexture.active;
                camera.targetTexture = target;
                camera.Render();
                RenderTexture.active = target;
                image = new Texture2D(width, height, TextureFormat.RGBA32, false);
                image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                image.Apply();
                File.WriteAllBytes(output, image.EncodeToPNG());
                var receipt = new Receipt {
                    camera = camera.name, unityVersion = Application.unityVersion,
                    graphicsDevice = SystemInfo.graphicsDeviceName,
                    placement = new[] {pilotTransform.position.x, pilotTransform.position.y, pilotTransform.position.z},
                    viewportCenter = new[] {center.x, center.y, center.z},
                    viewportBounds = new[] {corners.Min(point => point.x), corners.Min(point => point.y),
                                            corners.Max(point => point.x), corners.Max(point => point.y)},
                    width = width, height = height
                };
                File.WriteAllText(output + ".json", JsonUtility.ToJson(receipt, true));
                Debug.Log("STUDIO_PILOT_CAPTURED camera=" + camera.name +
                          " viewportCenter=" + center.x + "," + center.y + "," + center.z);
            }
            catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
            finally
            {
                if (pilotTransform != null) pilotTransform.position = originalPosition;
                if (camera != null) camera.targetTexture = null;
                RenderTexture.active = previous;
                if (target != null) { target.Release(); UnityEngine.Object.DestroyImmediate(target); }
                if (image != null) UnityEngine.Object.DestroyImmediate(image);
            }
        }
    }
}
