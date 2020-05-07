using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;

namespace Aura
{
    [ScriptedImporter(1, "pcs_set", 200)]
    public class AuraSOSceneSetImporter : ScriptedImporter
    {
        private static readonly string sceneSetName = "SceneSet";
        private static readonly string path = $"Assets/Imported/{sceneSetName}.pcs_set";
        private static readonly string scenePath = "Assets/Imported";

        public static void RequestImport()
        {
            if (!AssetDatabase.LoadAssetAtPath<AuraSOSceneSet>(path))
                File.WriteAllText(path, "dummy");
            AssetDatabase.ImportAsset(path);
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var sceneFolders = AssetDatabase.GetSubFolders(scenePath);

            var scenes = new List<AuraScene>();
            foreach (var sceneFolder in sceneFolders)
            {
                var scenePath = Path.Combine(sceneFolder, Path.GetFileName(sceneFolder) + ".psc");
                var scene = AssetDatabase.LoadAssetAtPath<AuraScene>(scenePath);
                if (scene != null)
                {
                    ctx.DependsOnSourceAsset(scenePath);
                    scenes.Add(scene);
                }
            }

            var sceneSet = ScriptableObject.CreateInstance<AuraSOSceneSet>();
            sceneSet.name = sceneSetName;
            sceneSet.Scenes = scenes;
            ctx.AddObjectToAsset(sceneSetName, sceneSet);
            ctx.SetMainObject(sceneSet);
        }
    }
}
