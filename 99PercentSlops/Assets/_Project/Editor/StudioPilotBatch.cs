using System;
using System.IO;
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
            root.transform.position = new Vector3(4f, 0f, 4f);

            var instance = PrefabUtility.InstantiatePrefab(model, scene) as GameObject;
            if (instance == null)
                throw new InvalidOperationException("Could not instantiate the imported furniture model.");
            instance.transform.SetParent(root.transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            if (!EditorSceneManager.SaveScene(scene, PilotScene))
                throw new IOException("Could not save the separate Studio pilot scene.");

            Debug.Log("STUDIO_PILOT_CREATED scene=" + PilotScene +
                      " modelGuid=" + AssetDatabase.AssetPathToGUID(ModelPath));
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
                root.transform.position != new Vector3(4f, 0f, 4f))
                throw new InvalidOperationException("Studio pilot placement does not match the fixed test scene.");

            var instance = root.transform.GetChild(0).gameObject;
            var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(instance);
            if (AssetDatabase.GetAssetPath(prefabSource) != ModelPath)
                throw new InvalidOperationException("Studio pilot model is no longer linked to the imported FBX.");

            Debug.Log("STUDIO_PILOT_VERIFIED scene=" + PilotScene +
                      " modelGuid=" + AssetDatabase.AssetPathToGUID(ModelPath));
        }

        private static string AbsoluteAssetPath(string assetPath)
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                                assetPath.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
