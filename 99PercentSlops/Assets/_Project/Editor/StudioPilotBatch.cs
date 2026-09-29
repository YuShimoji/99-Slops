using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlitchWorker.EditorTools
{
    public static class StudioPilotBatch
    {
        private const string SourceScene = "Assets/_Project/Scenes/Sandbox.unity";
        private const string PilotScene = "Assets/_Project/Scenes/StudioPilot_f0008.unity";
        private const string ModelPath = "Assets/_Project/StudioPilot/f0008/asset.fbx";
        private const string PilotRoot = "StudioPilot_f0008_TechnicalOnly";
        private static readonly Vector3 VisiblePlacement = new Vector3(-1.5f, 0f, 3.6f);

        [Serializable] private class SurfaceSpec
        {
            public string name;
            public float[] color;
            public float metallic;
            public float roughness;
        }
        [Serializable] private class MeshSurface
        {
            public string objectName;
            public string surface;
        }
        [Serializable] private class Appearance
        {
            public string assetId, revision, sourceHash, fbxHash;
            public SurfaceSpec[] surfaces;
            public MeshSurface[] meshSurfaces;
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create()) using (var stream = File.OpenRead(path))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        private static Appearance ReadAppearance(string revision)
        {
            var modelPath = "Assets/_Project/StudioPilot/" + revision + "/asset.fbx";
            var appearancePath = "Assets/_Project/StudioPilot/" + revision + "/appearance.json";
            var manifest = JsonUtility.FromJson<Appearance>(File.ReadAllText(AbsoluteAssetPath(appearancePath)));
            if (manifest == null || manifest.assetId != "asset:furniture-study" || manifest.revision != revision ||
                string.IsNullOrEmpty(manifest.sourceHash) || manifest.surfaces == null || manifest.meshSurfaces == null ||
                Hash(AbsoluteAssetPath(modelPath)) != manifest.fbxHash)
                throw new InvalidOperationException("Studio pilot appearance does not match " + revision + ".");
            return manifest;
        }

        private static void ApplySurfaces(GameObject root)
        {
            var manifest = ReadAppearance("f0008");
            var folder = "Assets/_Project/StudioPilot/f0008";
            var materials = new Dictionary<string, Material>();
            foreach (var spec in manifest.surfaces)
            {
                var file = folder + "/" + spec.name + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(file);
                if (material == null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
                    material = new Material(shader);
                    material.name = spec.name;
                    material.SetColor("_BaseColor", new Color(spec.color[0], spec.color[1], spec.color[2]));
                    material.SetFloat("_Metallic", spec.metallic);
                    material.SetFloat("_Smoothness", 1f - spec.roughness);
                    AssetDatabase.CreateAsset(material, file);
                }
                if (!MaterialMatches(material, spec))
                    throw new InvalidOperationException("Pilot material differs from the recorded appearance: " + file);
                materials.Add(spec.name, material);
            }
            var mapping = manifest.meshSurfaces.ToDictionary(item => item.objectName, item => item.surface);
            var renderers = root.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length != mapping.Count) throw new InvalidOperationException("Pilot mesh count differs from appearance manifest.");
            foreach (var renderer in renderers)
            {
                if (!mapping.ContainsKey(renderer.name) || !materials.ContainsKey(mapping[renderer.name]))
                    throw new InvalidOperationException("Unmapped pilot mesh: " + renderer.name);
                renderer.sharedMaterial = materials[mapping[renderer.name]];
            }
            AssetDatabase.SaveAssets();
        }

        public static void UpgradeMaterialsForPipeline()
        {
            var manifest = ReadAppearance("f0008");
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            foreach (var spec in manifest.surfaces)
            {
                var file = "Assets/_Project/StudioPilot/f0008/" + spec.name + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(file);
                if (material == null || material.shader.name != "Standard")
                    throw new InvalidOperationException("Expected prior Standard pilot material: " + file);
                material.shader = shader;
                material.SetColor("_BaseColor", new Color(spec.color[0], spec.color[1], spec.color[2]));
                material.SetFloat("_Metallic", spec.metallic);
                material.SetFloat("_Smoothness", 1f - spec.roughness);
                EditorUtility.SetDirty(material);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("STUDIO_PILOT_URP_MATERIALS_READY");
        }

        private static bool MaterialMatches(Material material, SurfaceSpec spec)
        {
            if (material == null || material.shader.name != "Universal Render Pipeline/Lit") return false;
            var color = material.GetColor("_BaseColor");
            return Math.Abs(color.r - spec.color[0]) < 0.001f &&
                   Math.Abs(color.g - spec.color[1]) < 0.001f &&
                   Math.Abs(color.b - spec.color[2]) < 0.001f &&
                   Math.Abs(material.GetFloat("_Metallic") - spec.metallic) < 0.001f &&
                   Math.Abs(material.GetFloat("_Smoothness") - (1f - spec.roughness)) < 0.001f;
        }

        private static void VerifySurfaces(GameObject root, string revision)
        {
            var manifest = ReadAppearance(revision);
            var mapping = manifest.meshSurfaces.ToDictionary(item => item.objectName, item => item.surface);
            var specs = manifest.surfaces.ToDictionary(item => item.name);
            var renderers = root.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length != mapping.Count) throw new InvalidOperationException("Pilot mesh count changed.");
            foreach (var renderer in renderers)
            {
                if (!mapping.ContainsKey(renderer.name) ||
                    AssetDatabase.GetAssetPath(renderer.sharedMaterial) !=
                    "Assets/_Project/StudioPilot/" + revision + "/" + mapping[renderer.name] + ".mat" ||
                    !MaterialMatches(renderer.sharedMaterial, specs[mapping[renderer.name]]))
                    throw new InvalidOperationException("Pilot surface mapping changed: " + renderer.name);
            }
        }

        public static void Create()
        {
            if (File.Exists(AbsoluteAssetPath(PilotScene)))
                throw new InvalidOperationException("Studio pilot scene already exists; refusing to overwrite it.");
            if (!File.Exists(AbsoluteAssetPath(SourceScene)))
                throw new FileNotFoundException("Sandbox scene is missing.", SourceScene);

            AssetDatabase.ImportAsset(ModelPath, ImportAssetOptions.ForceSynchronousImport);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
            if (model == null)
                throw new InvalidOperationException("Studio furniture FBX did not import as a model prefab.");

            var scene = EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
            var root = new GameObject(PilotRoot);
            SceneManager.MoveGameObjectToScene(root, scene);
            root.transform.position = VisiblePlacement;

            var instance = PrefabUtility.InstantiatePrefab(model, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException("Could not instantiate the imported furniture model.");
            instance.transform.SetParent(root.transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            ApplySurfaces(root);

            if (!EditorSceneManager.SaveScene(scene, PilotScene))
                throw new IOException("Could not save the separate Studio pilot scene.");

            Debug.Log("STUDIO_PILOT_CREATED scene=" + PilotScene +
                      " modelGuid=" + AssetDatabase.AssetPathToGUID(ModelPath));
        }

        public static void PrepareVisibleReview()
        {
            var scene = EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
            var root = scene.GetRootGameObjects().SingleOrDefault(item => item.name == PilotRoot);
            if (root == null || root.transform.childCount != 1 ||
                root.transform.position != new Vector3(4f, 0f, 4f))
                throw new InvalidOperationException("Pilot scene is not at the prior fixed placement.");
            var instance = root.transform.GetChild(0).gameObject;
            if (AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(instance)) != ModelPath)
                throw new InvalidOperationException("Pilot FBX reference changed.");
            ApplySurfaces(root);
            root.transform.position = VisiblePlacement;
            if (!EditorSceneManager.SaveScene(scene, PilotScene))
                throw new IOException("Could not save the visible Studio pilot scene.");
            Debug.Log("STUDIO_PILOT_VISIBLE_REVIEW_READY");
        }

        public static void RepositionForPlayer()
        {
            var scene = EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
            var root = scene.GetRootGameObjects().SingleOrDefault(item => item.name == PilotRoot);
            if (root == null || root.transform.childCount != 1 ||
                root.transform.position != new Vector3(0f, 0f, 4f))
                throw new InvalidOperationException("Pilot root differs from the camera-visible technical scene.");
            var instance = root.transform.GetChild(0).gameObject;
            if (AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(instance)) != ModelPath)
                throw new InvalidOperationException("Pilot FBX reference changed.");
            VerifySurfaces(root, "f0008");
            root.transform.position = VisiblePlacement;
            if (!EditorSceneManager.SaveScene(scene, PilotScene))
                throw new IOException("Could not save the Player-visible pilot placement.");
            Debug.Log("STUDIO_PILOT_PLAYER_PLACEMENT_READY");
        }

        public static void Verify()
        {
            if (!File.Exists(AbsoluteAssetPath(PilotScene)))
                throw new FileNotFoundException("Studio pilot scene is missing.", PilotScene);

            var scene = EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
            GameObject root = null;
            foreach (var item in scene.GetRootGameObjects())
            {
                if (item.name == PilotRoot)
                {
                    if (root != null)
                        throw new InvalidOperationException("Duplicate Studio pilot roots found.");
                    root = item;
                }
            }

            if (root == null || root.transform.childCount != 1 ||
                root.transform.position != VisiblePlacement)
                throw new InvalidOperationException("Studio pilot placement does not match the fixed test scene.");

            var instance = root.transform.GetChild(0).gameObject;
            var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            var modelPath = AssetDatabase.GetAssetPath(prefabSource);
            var match = Regex.Match(modelPath, @"^Assets/_Project/StudioPilot/(f\d{4})/asset\.fbx$");
            if (!match.Success)
                throw new InvalidOperationException("Studio pilot model is no longer linked to the imported FBX.");
            var revision = match.Groups[1].Value;
            VerifySurfaces(root, revision);

            Debug.Log("STUDIO_PILOT_VERIFIED scene=" + PilotScene +
                      " revision=" + revision + " modelGuid=" + AssetDatabase.AssetPathToGUID(modelPath));
        }

        public static void Build()
        {
            Verify();

            var outputPath = Environment.GetEnvironmentVariable("STUDIO_PILOT_BUILD_PATH");
            if (string.IsNullOrWhiteSpace(outputPath) ||
                !Path.IsPathRooted(outputPath) ||
                !outputPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Set STUDIO_PILOT_BUILD_PATH to a new absolute .exe path.");
            if (File.Exists(outputPath))
                throw new IOException("Studio pilot build already exists; refusing to overwrite it.");

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (Directory.Exists(outputDirectory))
            {
                foreach (var entry in Directory.EnumerateFileSystemEntries(outputDirectory))
                    throw new IOException("Studio pilot build directory is not empty: " + entry);
            }
            else
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var options = new BuildPlayerOptions
            {
                scenes = new[] { PilotScene },
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new InvalidOperationException("Studio pilot Player build failed: " + report.summary.result);

            Debug.Log("STUDIO_PILOT_BUILD_SUCCEEDED path=" + outputPath +
                      " bytes=" + report.summary.totalSize);
        }

        private static string AbsoluteAssetPath(string assetPath)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                                assetPath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
