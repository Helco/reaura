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

namespace Aura
{
    [ScriptedImporter(1, "psc")]
    public class AuraSceneScriptImporter : ScriptedImporter
    {

        private GameObject sceneGO;
        private Dictionary<string, string> files;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            files = ReadFiles(ctx);
            var sceneName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            if (!files.TryGetValue(sceneName + ".scc", out var sceneScriptText))
                throw new InvalidDataException("Aura script pack does not contain a scene script");
            var sceneScriptTokenizer = new Tokenizer(sceneName + ".scc", sceneScriptText);
            var sceneScript = new SceneScriptParser(sceneScriptTokenizer).ParseSceneScript();

            sceneGO = new GameObject($"Scene {sceneName}");
            ctx.AddObjectToAsset(sceneGO.name, sceneGO);
            ctx.SetMainObject(sceneGO);

            var graphicLists = sceneScript
                .EntityLists
                .Values
                .Where(l => l is GraphicListNode)
                .Cast<GraphicListNode>();
            foreach (var graphicList in graphicLists)
                ImportGraphicList(ctx, graphicList);

            var cellLists = sceneScript
                .EntityLists
                .Values
                .Where(l => l is CellListNode)
                .Cast<CellListNode>();
            foreach (var cellList in cellLists)
                ImportCellList(ctx, cellList);
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

        private void ImportCellList(AssetImportContext ctx, CellListNode cellList)
        {
            void ExpectProperty<T>(CellNode cell, string name, out T value) where T : ValueNode
            {
                if (!cell.Properties.TryGetValue(name, out var cellProp))
                    throw new InvalidDataException($"Expected the cell property {name}");
                if (!(cellProp.Value is T))
                    throw new InvalidDataException($"Expected cell property {name} to be {typeof(T).Name}");
                value = cellProp.Value as T;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AuraCell.prefab");
            GameObject parentGO = new GameObject(cellList.Name.Substring(1));
            parentGO.transform.parent = sceneGO.transform;

            foreach (var cellNode in cellList.Cells.Values)
            {
                ExpectProperty(cellNode, "Pos", out VectorNode posNode);
                ExpectProperty(cellNode, "CellSize", out VectorNode sizeNode);
                ExpectProperty(cellNode, "script", out StringNode scriptNode);
                var scriptFile = scriptNode.Value.Replace(".\\", "");
                if (!files.TryGetValue(scriptFile, out var scriptContent))
                    throw new InvalidDataException($"Unknown script name {scriptNode.Value} in cell {cellNode.Name}");

                var cellGO = Instantiate(prefab);
                cellGO.name = "Cell " + cellNode.Name;
                cellGO.transform.parent = parentGO.transform;
                var cellArea = cellGO.GetComponent<SphericalArea>();
                cellArea.upperLeft = new Vector2(posNode.X, posNode.Y);
                cellArea.size = new Vector2(sizeNode.X, sizeNode.Y);
                var cell = cellGO.GetComponent<AuraCell>();
                cell.ScriptFile = scriptFile;
                cell.Script = scriptContent;
            }
        }

        private void ImportGraphicList(AssetImportContext ctx, GraphicListNode graphicList)
        {
            GameObject parentGO = new GameObject(graphicList.Name.Substring(1));
            parentGO.transform.parent = sceneGO.transform;
            int currentId = 0;

            var interpreter = new Interpreter();
            interpreter.RegisterArgumentMapper(typeof(CubemapFace), typeof(StringNode), MapStringToCubemapFace);
            interpreter.RegisterArgumentMapper(typeof(Texture2D), typeof(StringNode), v => MapStringToTexture(ctx, v));
            interpreter.RegisterArgumentMapper(typeof(VideoClip), typeof(StringNode), v => MapStringToVideoClip(ctx, v));
            interpreter.RegisterFunction("Sprite", (Texture2D tex, int posX, int posY, CubemapFace face) => ImportSprite(parentGO, currentId, tex, posX, posY, face));
            interpreter.RegisterFunction("PlayAVI", (VideoClip clip, string unk, int posX, int posY, CubemapFace face) => ImportVideoSprite(parentGO, currentId, clip, posX, posY, face));

            foreach (var graphic in graphicList.Graphics.Values)
            {
                currentId = graphic.ID;
                interpreter.Execute(graphic.Value);
            }
        }

        private object MapStringToCubemapFace(ValueNode v)
        {
            string constantName = (v as StringNode).Value;
            if (constantName == "FRONT") return CubemapFace.PositiveZ;
            if (constantName == "BACKK") return CubemapFace.NegativeZ;
            if (constantName == "RIGHT") return CubemapFace.PositiveX;
            if (constantName == "LEFTT") return CubemapFace.NegativeX;
            if (constantName == "UPPPP") return CubemapFace.PositiveY;
            if (constantName == "DOWNN") return CubemapFace.NegativeY;
            throw new Exception($"No known face name {constantName}");
        }

        private Texture2D MapStringToTexture(AssetImportContext ctx, ValueNode v)
        {
            string path = (v as StringNode).Value;
            if (path.StartsWith(".\\"))
                path = path.Substring(2);
            path = Path.Combine(Path.GetDirectoryName(ctx.assetPath), path);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex == null)
                throw new Exception($"Could not find texture at {path}");
            return tex;
        }

        private VideoClip MapStringToVideoClip(AssetImportContext ctx, ValueNode v)
        {
            string path = (v as StringNode).Value.Replace(".bik", ".webm");
            if (path.StartsWith(".\\"))
                path = path.Substring(2);
            path = Path.Combine(Path.GetDirectoryName(ctx.assetPath), path);
            var clip = AssetDatabase.LoadAssetAtPath<VideoClip>(path);
            if (clip == null)
                throw new Exception($"Could not find video at {path}");
            return clip;
        }

        private void ImportSprite(GameObject parent, int id, Texture2D texture, int posX, int posY, CubemapFace face)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AuraSprite.prefab");
            var go = Instantiate(prefab);
            go.name = $"Sprite {id} {texture.name}";
            var sprite = go.GetComponent<AuraSpriteRenderer>();
            sprite.Texture = texture;
            sprite.TexturePos = new Vector2(posX, posY);
            sprite.Face = face;
            go.transform.parent = parent.transform;
        }

        private void ImportVideoSprite(GameObject parent, int id, VideoClip clip, int posX, int posY, CubemapFace face)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AuraVideoSprite.prefab");
            var go = Instantiate(prefab);
            go.name = $"Video {id} {clip.name}";
            go.GetComponent<VideoPlayer>().clip = clip;
            var sprite = go.GetComponent<AuraSpriteRenderer>();
            sprite.TexturePos = new Vector2(posX, posY);
            sprite.Face = face;
            go.transform.parent = parent.transform;
        }
    }
}