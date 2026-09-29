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
    // Local development handoff for a validated Studio furniture output.
    // The game Player reads imported assets only; it never reads the Studio source or tools.
    public static class StudioPilotConsumer
    {
        private const string PilotScene = "Assets/_Project/Scenes/StudioPilot_f0008.unity";
        private const string RootName = "StudioPilot_f0008_TechnicalOnly";
        private const string AssetRoot = "Assets/_Project/StudioPilot";
        private static readonly Vector3 Placement = new Vector3(-1.5f, 0f, 3.6f);

        [Serializable] private sealed class Surface
        {
            public string name;
            public float[] color;
            public float metallic, roughness;
        }
        [Serializable] private sealed class MeshSurface
        {
            public string objectName, surface;
        }
        [Serializable] private sealed class Request
        {
            public string assetId, revision, sourceHash, fbxHash, fbxPath;
            public Surface[] surfaces;
            public MeshSurface[] meshSurfaces;
        }
        [Serializable] private sealed class Appearance
        {
            public string assetId, revision, sourceHash, fbxHash;
            public Surface[] surfaces;
            public MeshSurface[] meshSurfaces;
        }
        [Serializable] private sealed class Receipt
        {
            public string scene, assetId, previousRevision, revision, sourceHash, fbxHash;
            public string modelGuid, rootGlobalId, sceneHash, requestHash;
            public float[] placement;
            public int meshRenderers;
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create()) using (var stream = File.OpenRead(path))
                return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
        }

        private static string AbsoluteAssetPath(string assetPath) =>
            Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                         assetPath.Replace('/', Path.DirectorySeparatorChar));

        private static string RevisionOf(GameObject instance)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            var path = AssetDatabase.GetAssetPath(source);
            var match = Regex.Match(path, @"^Assets/_Project/StudioPilot/(f\d{4})/asset\.fbx$");
            if (!match.Success) throw new InvalidOperationException("Pilot child is not a Studio revision: " + path);
            return match.Groups[1].Value;
        }

        private static bool MaterialMatches(Material material, Surface spec)
        {
            if (material == null || material.shader.name != "Universal Render Pipeline/Lit") return false;
            var color = material.GetColor("_BaseColor");
            return Math.Abs(color.r - spec.color[0]) < 0.001f &&
                   Math.Abs(color.g - spec.color[1]) < 0.001f &&
                   Math.Abs(color.b - spec.color[2]) < 0.001f &&
                   Math.Abs(material.GetFloat("_Metallic") - spec.metallic) < 0.001f &&
                   Math.Abs(material.GetFloat("_Smoothness") - (1f - spec.roughness)) < 0.001f;
        }

        public static void Apply()
        {
            var requestPath = Environment.GetEnvironmentVariable("STUDIO_PILOT_HANDOFF_REQUEST");
            var receiptPath = Environment.GetEnvironmentVariable("STUDIO_PILOT_HANDOFF_RECEIPT");
            var expectedRevision = Environment.GetEnvironmentVariable("STUDIO_PILOT_EXPECTED_REVISION");
            if (string.IsNullOrWhiteSpace(requestPath) || !Path.IsPathRooted(requestPath) ||
                string.IsNullOrWhiteSpace(receiptPath) || !Path.IsPathRooted(receiptPath) ||
                File.Exists(receiptPath) || !Regex.IsMatch(expectedRevision ?? "", @"^f\d{4}$"))
                throw new InvalidOperationException("Set absolute request/new receipt paths and expected revision.");
            var request = JsonUtility.FromJson<Request>(File.ReadAllText(requestPath));
            if (request == null || request.assetId != "asset:furniture-study" ||
                !Regex.IsMatch(request.revision ?? "", @"^f\d{4}$") ||
                string.IsNullOrWhiteSpace(request.sourceHash) ||
                !Regex.IsMatch(request.fbxHash ?? "", "^[a-f0-9]{64}$") ||
                string.IsNullOrWhiteSpace(request.fbxPath) || !Path.IsPathRooted(request.fbxPath) ||
                request.surfaces == null || request.surfaces.Length == 0 ||
                request.meshSurfaces == null || request.meshSurfaces.Length == 0 ||
                Hash(request.fbxPath) != request.fbxHash)
                throw new InvalidOperationException("Studio handoff request or FBX hash is invalid.");
            foreach (var surface in request.surfaces)
                if (surface == null || !Regex.IsMatch(surface.name ?? "", "^[A-Za-z0-9_-]+$") ||
                    surface.color == null || surface.color.Length != 3)
                    throw new InvalidOperationException("Studio handoff has an invalid surface.");
            var specs = request.surfaces.ToDictionary(surface => surface.name);
            var mapping = request.meshSurfaces.ToDictionary(item => item.objectName, item => item.surface);
            if (mapping.Values.Any(surface => !specs.ContainsKey(surface)))
                throw new InvalidOperationException("Studio handoff has an unmapped surface.");

            var scene = EditorSceneManager.OpenScene(PilotScene, OpenSceneMode.Single);
            var root = scene.GetRootGameObjects().SingleOrDefault(item => item.name == RootName);
            if (root == null || root.transform.position != Placement || root.transform.childCount != 1)
                throw new InvalidOperationException("Pilot scene root or placement changed.");
            var previous = RevisionOf(root.transform.GetChild(0).gameObject);
            if (previous != expectedRevision || previous == request.revision)
                throw new InvalidOperationException("Pilot usage changed or already uses this revision.");
            var rootId = GlobalObjectId.GetGlobalObjectIdSlow(root).ToString();

            var folder = AssetRoot + "/" + request.revision;
            var assetPath = folder + "/asset.fbx";
            var absoluteFolder = AbsoluteAssetPath(folder);
            Directory.CreateDirectory(absoluteFolder);
            var copyPath = AbsoluteAssetPath(assetPath);
            if (File.Exists(copyPath))
            {
                if (Hash(copyPath) != request.fbxHash)
                    throw new IOException("Existing pilot revision FBX differs from handoff.");
            }
            else File.Copy(request.fbxPath, copyPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (model == null) throw new InvalidOperationException("Studio handoff FBX did not import.");
            var appearancePath = folder + "/appearance.json";
            var appearanceFile = AbsoluteAssetPath(appearancePath);
            var appearance = new Appearance {
                assetId = request.assetId, revision = request.revision,
                sourceHash = request.sourceHash, fbxHash = request.fbxHash,
                surfaces = request.surfaces, meshSurfaces = request.meshSurfaces
            };
            if (File.Exists(appearanceFile))
            {
                var old = JsonUtility.FromJson<Appearance>(File.ReadAllText(appearanceFile));
                if (old == null || old.assetId != appearance.assetId || old.revision != appearance.revision ||
                    old.sourceHash != appearance.sourceHash || old.fbxHash != appearance.fbxHash ||
                    old.surfaces == null || old.surfaces.Length != appearance.surfaces.Length ||
                    old.meshSurfaces == null || old.meshSurfaces.Length != appearance.meshSurfaces.Length ||
                    old.surfaces.Where((spec, index) => spec.name != appearance.surfaces[index].name ||
                        spec.metallic != appearance.surfaces[index].metallic ||
                        spec.roughness != appearance.surfaces[index].roughness ||
                        spec.color == null || !spec.color.SequenceEqual(appearance.surfaces[index].color)).Any() ||
                    old.meshSurfaces.Where((mesh, index) =>
                        mesh.objectName != appearance.meshSurfaces[index].objectName ||
                        mesh.surface != appearance.meshSurfaces[index].surface).Any())
                    throw new InvalidOperationException("Existing pilot appearance differs from handoff.");
            }
            else File.WriteAllText(appearanceFile, JsonUtility.ToJson(appearance, true));
            AssetDatabase.ImportAsset(appearancePath, ImportAssetOptions.ForceSynchronousImport);
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            var materials = new Dictionary<string, Material>();
            foreach (var spec in request.surfaces)
            {
                var path = folder + "/" + spec.name + ".mat";
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    material = new Material(shader) { name = spec.name };
                    material.SetColor("_BaseColor", new Color(spec.color[0], spec.color[1], spec.color[2]));
                    material.SetFloat("_Metallic", spec.metallic);
                    material.SetFloat("_Smoothness", 1f - spec.roughness);
                    AssetDatabase.CreateAsset(material, path);
                }
                if (!MaterialMatches(material, spec))
                    throw new InvalidOperationException("Pilot material differs from Studio handoff: " + path);
                materials.Add(spec.name, material);
            }
            AssetDatabase.SaveAssets();

            var instance = PrefabUtility.InstantiatePrefab(model, scene) as GameObject;
            if (instance == null) throw new InvalidOperationException("Could not instantiate pilot revision.");
            instance.transform.SetParent(root.transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            var renderers = instance.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length != mapping.Count || renderers.Any(renderer => !mapping.ContainsKey(renderer.name)))
                throw new InvalidOperationException("Pilot revision mesh mapping differs from Studio handoff.");
            foreach (var renderer in renderers) renderer.sharedMaterial = materials[mapping[renderer.name]];
            UnityEngine.Object.DestroyImmediate(root.transform.GetChild(0).gameObject);
            if (!EditorSceneManager.SaveScene(scene, PilotScene))
                throw new IOException("Could not save the pilot revision usage.");
            if (GlobalObjectId.GetGlobalObjectIdSlow(root).ToString() != rootId ||
                RevisionOf(root.transform.GetChild(0).gameObject) != request.revision)
                throw new InvalidOperationException("Pilot root or revision changed while saving.");

            Directory.CreateDirectory(Path.GetDirectoryName(receiptPath));
            var receipt = new Receipt {
                scene = PilotScene, assetId = request.assetId, previousRevision = previous,
                revision = request.revision, sourceHash = request.sourceHash, fbxHash = request.fbxHash,
                modelGuid = AssetDatabase.AssetPathToGUID(assetPath), rootGlobalId = rootId,
                sceneHash = Hash(AbsoluteAssetPath(PilotScene)), requestHash = Hash(requestPath),
                placement = new[] { Placement.x, Placement.y, Placement.z }, meshRenderers = renderers.Length
            };
            File.WriteAllText(receiptPath, JsonUtility.ToJson(receipt, true));
            Debug.Log("STUDIO_PILOT_CONSUMER_APPLIED revision=" + request.revision +
                      " previous=" + previous + " sceneHash=" + receipt.sceneHash);
        }
    }
}
