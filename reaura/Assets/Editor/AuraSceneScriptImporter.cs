using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using Aura.Script;
using System;
using UnityEngine.Video;

[ScriptedImporter(1, "psc")]
public class AuraSceneScriptImporter : ScriptedImporter
{

    private GameObject sceneGO;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var files = ReadFiles(ctx);
        var sceneName = Path.GetFileNameWithoutExtension(ctx.assetPath);

        if (!files.TryGetValue(sceneName + ".scc", out var sceneScriptText))
            throw new InvalidDataException("Aura script pack does not contain a scene script");
        var sceneScriptTokenizer = new Aura.Script.Tokenizer(sceneName + ".scc", sceneScriptText);
        var sceneScript = new Aura.Script.SceneScriptParser(sceneScriptTokenizer).ParseSceneScript();

        sceneGO = new GameObject($"Scene {sceneName}");
        ctx.AddObjectToAsset(sceneGO.name, sceneGO);
        ctx.SetMainObject(sceneGO);
    }

    private Dictionary<string, string> ReadFiles(AssetImportContext ctx)
    {
        var files = new Dictionary<string, string>();
        var reader = new AuraPackFileReader(new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read));
        var fileNames = reader.ReadFileList();
        foreach (var fileName in fileNames)
        {
            var fileContent = new StringWriter();
            uint lineCount = reader.ReadU32();
            for (uint i = 0; i < lineCount; i++)
            {
                var nextLine = reader.ReadString((int)reader.ReadU32());
                if (nextLine.EndsWith("\n"))
                    fileContent.Write(nextLine);
                else
                    fileContent.WriteLine(nextLine);
            }
            files[fileName] = fileContent.ToString();
        }
        return files;
    }

    private static IEnumerable<(Type csharp, Type aura, Func<AssetImportContext, ValueNode, object> converter)> ArgumentMappings = new (Type csharp, Type aura, Func<AssetImportContext, ValueNode, object> converter)[]
    {
        (csharp: typeof(string), aura: typeof(StringNode), converter: (ctx, v) => (v as StringNode).Value),
        (csharp: typeof(int), aura: typeof(NumericNode), converter: (ctx, v) => (v as NumericNode).Value),
        (csharp: typeof(int), aura: typeof(StringNode), converter: (ctx, v) =>
        {
            string constantName = (v as StringNode).Value;
            if (constantName == "TRUE") return 1;
            if (constantName == "FALSE") return 0;
            throw new Exception($"No known numeric constant {constantName}");
        }),
        (csharp: typeof(CubemapFace), aura: typeof(StringNode), converter: (ctx, v) =>
        {
            string constantName = (v as StringNode).Value;
            if (constantName == "FRONT") return CubemapFace.NegativeZ;
            if (constantName == "BACK") return CubemapFace.PositiveZ;
            if (constantName == "RIGHT") return CubemapFace.NegativeX;
            if (constantName == "LEFT") return CubemapFace.PositiveX;
            if (constantName == "UP") return CubemapFace.PositiveY;
            if (constantName == "DOWN") return CubemapFace.NegativeY;
            throw new Exception($"No known face name {constantName}");
        }),
        (csharp: typeof(VideoClip), aura: typeof(StringNode), converter: (AssetImportContext ctx, ValueNode v) =>
        {
            string path = (v as StringNode).Value;
            if (path.StartsWith(".\\"))
                path = path.Substring(2);
            path = Path.Combine(Path.GetDirectoryName(ctx.assetPath), path);
            var clip = AssetDatabase.LoadAssetAtPath<VideoClip>(path);
            if (clip == null)
                throw new Exception($"Could not find video at {path}");
            return clip;
        })
    };

    private void ImportGraphicList(AssetImportContext ctx, GraphicListNode graphicList, IReadOnlyDictionary<string, string> methodMapping)
    {
        foreach (var graphic in graphicList.Graphics.Values)
        {
            var call = graphic.Value;
            if (!methodMapping.TryGetValue(call.Function, out var csharpMethodName))
                throw new Exception($"No mapping for AuraScript function {call.Function}");

            var csharpMethod = GetType().GetMethod(csharpMethodName, System.Reflection.BindingFlags.NonPublic);
            var csharpParams = csharpMethod.GetParameters();
            if (csharpParams.Length != call.Arguments.Count() + 1)
                throw new Exception($"Invalid number for AuraScript function {call.Function}");
            if (csharpParams[0].ParameterType != typeof(int))
                throw new Exception($"CSharp function for {call.Function} does not have an int parameter for the id");

            object[] args = new object[csharpParams.Length];
            args[0] = graphic.ID;
            for (int i = 0; i < call.Arguments.Count(); i++)
            {
                var csharpParamType = csharpParams[i + 1].ParameterType;
                var auraArgument = call.Arguments.ElementAt(i);
                if (auraArgument == null)
                {
                    if (csharpParams[i + 1].CustomAttributes.Any(a => a.AttributeType == typeof(AuraOptionalAttribute)))
                        args[i] = null;
                    else
                        throw new Exception($"Parameter {i + 1} for {call.Function} is not optional");
                }
                else
                {
                    var mapping = ArgumentMappings.SingleOrDefault(t => t.csharp == csharpParamType && t.aura == auraArgument.GetType());
                    if (mapping.converter == null)
                        throw new Exception($"No known mapping between C# {csharpParamType.Name} and Aura {auraArgument.GetType().Name}");
                    else
                        args[i] = mapping.converter(ctx, auraArgument);
                }
            }
            csharpMethod.Invoke(this, args);
        }
    }

    private void Import_FonAnimate_LoadAVI(int id, VideoClip clip, [AuraOptional] string path2, int posX, int posY, CubemapFace face)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AuraVideoSprite.prefab");
        var go = Instantiate(prefab);
        go.name = $"Video {id} {clip.name}";
        go.GetComponent<VideoPlayer>().clip = clip;
        var sprite = go.GetComponent<AuraSpriteRenderer>();
    }


    private class AuraOptionalAttribute : Attribute
    {
    }
}
