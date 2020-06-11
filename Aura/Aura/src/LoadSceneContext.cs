using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Aura.Script;

namespace Aura
{
    public enum SceneType
    {
        Panorama,
        Puzzle
    }

    public class LoadSceneContext
    {
        private Dictionary<string, Func<Stream?>> sceneAssets = new Dictionary<string, Func<Stream?>>();

        public IBackend Backend { get; }
        public string ScenePath { get; }
        public string SceneName { get; }
        public SceneType Type { get; }
        public SceneNode Scene { get; }
        public IReadOnlyDictionary<string, string> ScriptTexts { get; } = new Dictionary<string, string>();
        public Queue<IWorldSprite> AvailableWorldSprites { get; set; } = new Queue<IWorldSprite>();

        public LoadSceneContext(IBackend backend, string sceneName, SceneType type)
        {
            sceneName = sceneName.StartsWith(".\\") ? sceneName.Substring(2) : sceneName;
            Backend = backend;
            SceneName = sceneName;
            ScenePath = $"Scenes/{sceneName}/";
            Type = type;

            AddAssetPack($"{ScenePath}{sceneName}.psp");
            AddAssetPack($"{ScenePath}{sceneName}.pvd");

            using var scriptPack = backend.OpenAssetFile($"{ScenePath}{sceneName}.psc");
            if (scriptPack == null)
                throw new FileNotFoundException($"Could not find required scene script pack for {sceneName}");
            ScriptTexts = ReadScriptPack(scriptPack);
            if (!ScriptTexts.TryGetValue($"{sceneName}.scc", out var sceneScriptText))
                throw new InvalidDataException($"Script pack for {sceneName} does not have a scene script");
            var sceneScanner = new Tokenizer($"{sceneName}.scc", sceneScriptText);
            Scene = new SceneScriptParser(sceneScanner).ParseSceneScript();
        }

        private void AddAssetPack(string filePath)
        {
            var packFileStream = Backend.OpenAssetFile(filePath);
            if (packFileStream == null)
                return;

            var packFile = new PackFileReader(packFileStream);
            var files = packFile
                .ReadFileList()
                .Select(fileName =>
                {
                    int length = (int)packFile.ReadU32();
                    return (fileName, buffer: packFile.ReadRaw(length));
                });
            foreach (var file in files)
                AddAssetFile(file.fileName, () => new MemoryStream(file.buffer, writable: false));
        }

        private void AddAssetFile(string fileName, Func<Stream> streamAccessor)
        {
            fileName = fileName.ToLowerInvariant();
            if (sceneAssets.ContainsKey(fileName))
                throw new InvalidDataException($"Scene asset {fileName} was found twice");
            sceneAssets[fileName] = streamAccessor;
        }

        private static Dictionary<string, string> ReadScriptPack(Stream stream)
        {
            PackFileReader scriptPack = new PackFileReader(stream);
            string[] fileNames = scriptPack.ReadFileList();
            var files = new Dictionary<string, string>();

            foreach (var fileName in fileNames)
            {
                var fileContent = new StringWriter();
                uint lineCount = scriptPack.ReadU32();
                for (uint i = 0; i < lineCount; i++)
                {
                    var nextLine = scriptPack.ReadString((int)scriptPack.ReadU32());
                    if (nextLine.EndsWith("\n"))
                        fileContent.Write(nextLine);
                    else
                        fileContent.WriteLine(nextLine);
                }
                files[fileName] = fileContent.ToString();
            }
            return files;
        }

        public Stream? OpenSceneAsset(string name)
        {
            if (name.StartsWith(".\\"))
                name = name.Substring(2);
            name = name.ToLowerInvariant();
            return sceneAssets.TryGetValue(name, out var streamAccessor)
                ? streamAccessor()
                : null;
        }

        public ITexture ScrArgLoadImage(ValueNode node)
        {
            var textureName = ((StringNode)node).Value;
            using var stream = OpenSceneAsset(textureName);
            if (stream == null)
                throw new FileNotFoundException($"{node.Position}: Could not find texture \"{textureName}\"");
            return Backend.CreateImage(stream);
        }

        public ITexture ScrArgLoadVideo(ValueNode node)
        {
            var videoName = ((StringNode)node).Value;
            var stream = OpenSceneAsset(videoName);
            if (stream == null)
                throw new FileNotFoundException($"{node.Position}: Could not find video \"{videoName}\"");
            return Backend.CreateVideo(stream);
        }
    }
}