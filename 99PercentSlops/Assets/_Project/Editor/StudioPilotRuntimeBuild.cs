using System;
using System.IO;
using GlitchWorker.StudioPilot;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlitchWorker.EditorTools
{
    // Builds an instrumented copy. The authored pilot and Sandbox scenes remain untouched.
    public static class StudioPilotRuntimeBuild
    {
        private const string PilotScene = "Assets/_Project/Scenes/StudioPilot_f0008.unity";
        private const string ProbeScene = "Assets/_Project/Scenes/StudioPilot_f0008_RuntimeProbe.unity";

        public static void Build()
        {
            StudioPilotBatch.Verify();
            var output = Environment.GetEnvironmentVariable("STUDIO_PILOT_BUILD_PATH");
            if (string.IsNullOrWhiteSpace(output) || !Path.IsPathRooted(output) ||
                !output.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Set STUDIO_PILOT_BUILD_PATH to a new absolute .exe path.");
            if (File.Exists(output) || AssetDatabase.LoadAssetAtPath<SceneAsset>(ProbeScene) != null)
                throw new IOException("Probe build or temporary scene already exists; refusing to overwrite it.");
            var directory = Path.GetDirectoryName(output);
            if (Directory.Exists(directory))
            {
                if (Directory.GetFileSystemEntries(directory).Length != 0)
                    throw new IOException("Probe build directory is not empty: " + directory);
            }
            else Directory.CreateDirectory(directory);

            try
            {
                var scene = EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
                var probe = new GameObject("StudioPilot_RuntimeProbeOnly");
                probe.AddComponent<StudioPilotRuntimeProbe>();
                if (!EditorSceneManager.SaveScene(scene, ProbeScene, true))
                    throw new IOException("Could not save temporary probe scene.");
                AssetDatabase.ImportAsset(ProbeScene, ImportAssetOptions.ForceSynchronousImport);
                var options = new BuildPlayerOptions {
                    scenes = new[] { ProbeScene },
                    locationPathName = output,
                    target = BuildTarget.StandaloneWindows64,
                    options = BuildOptions.None
                };
                var report = BuildPipeline.BuildPlayer(options);
                if (report.summary.result != BuildResult.Succeeded)
                    throw new InvalidOperationException("Instrumented Player build failed: " + report.summary.result);
                Debug.Log("STUDIO_PILOT_RUNTIME_BUILD_SUCCEEDED path=" + output);
            }
            finally
            {
                EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ProbeScene) != null)
                    AssetDatabase.DeleteAsset(ProbeScene);
            }
        }
    }
}
