using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Aura
{
    public partial class AuraImportEditorWindow
    {
        private static readonly string assetsPath = "Assets";
        private static readonly string baseOutPath = Path.Combine(assetsPath, "Imported");
        private static readonly string scenesOutPath = Path.Combine(baseOutPath, "Scenes");
        private static readonly string tempPath = "Temp";

        private string auraBasePath;

        private IEnumerator ImportGame()
        {
            yield return null;
            progresses.Add(new Progress("Initialize importer", 0.0f));
            auraBasePath = Path.GetDirectoryName(auraPath);
            if (!File.Exists(Path.Combine(auraBasePath, "Aura1.exe")))
            {
                Debug.LogError("Could not find Aura1.exe at base path");
                importCoroutine = null;
                yield break;
            }
            Directory.CreateDirectory(tempPath);
            Directory.CreateDirectory(baseOutPath);

            var steps = new (string, Func<IEnumerator>)[]
            {
                ("Import scenes", ImportScenes)
            };
            for (int i = 0; i < steps.Length; i++)
            {
                progresses[0] = new Progress(
                    $"{i+1}/{steps.Length}: {steps[i].Item1}",
                    i * 100.0f / steps.Length);
                yield return ImportScenes();
            }
            

            yield return null;
            Debug.Log("Import finished");
            importCoroutine = null;
            progresses.RemoveAt(0);
        }

        private IEnumerator ImportScenes()
        {
            progresses.Add(new Progress("Finding scenes", 0.0f));
            yield return null;
            string sceneBasePath = Path.Combine(auraBasePath, "Scenes");
            string[] scenePaths = Directory.GetDirectories(sceneBasePath);
            Directory.CreateDirectory(scenesOutPath);
            for (int i = 0; i < scenePaths.Length; i++)
            {
                var scenePath = scenePaths[i];
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                var sceneScriptPath = Path.Combine(scenePath, sceneName + ".psc");
                var sceneSpritePath = Path.Combine(scenePath, sceneName + ".psp");
                var sceneVideoPath = Path.Combine(scenePath, sceneName + ".pvd");
                var sceneOutPath = Path.Combine(scenesOutPath, sceneName);
                progresses[1] = new Progress(
                    $"{i + 1}/{scenePaths.Length}: {sceneName}",
                    i * 100.0f / scenePaths.Length);
                if (!File.Exists(sceneScriptPath))
                    break;

                Directory.CreateDirectory(sceneOutPath);
                if (File.Exists(sceneSpritePath))
                    yield return ExtractPackFile(sceneSpritePath, sceneOutPath);
                if (File.Exists(sceneVideoPath))
                    yield return ConvertVideoPackFile(sceneVideoPath, sceneName, sceneOutPath);
                var sceneMiscFiles = Directory
                    .GetFiles(scenePath)
                    .Except(sceneSpritePath, sceneVideoPath);
                yield return CopyFiles(sceneMiscFiles, sceneOutPath);
            }
            progresses.RemoveAt(1);
        }

        private IEnumerator CopyFiles(IEnumerable<string> files, string outPath)
        {
            progresses.Add(new Progress("Copying files", 0.0f));
            float curProgress = 0.0f;
            float progressStep = files.Count() / 100.0f;
            foreach (var file in files)
            {
                File.Copy(file, Path.Combine(outPath, Path.GetFileName(file)));
                progresses[2] = new Progress("Copying files", (curProgress += progressStep));
                yield return null;
            }
            progresses.RemoveAt(2);
        }

        private static readonly int bufferSize = 16 * 1024;
        private IEnumerator ExtractPackFile(string packFilePath, string outPath)
        {
            progresses.Add(new Progress("Extracting pack file", 0.0f));
            var buffer = new byte[bufferSize];
            using (var stream = new FileStream(packFilePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new AuraPackFileReader(stream);
                var fileNames = reader.ReadFileList();
                float curProgress = 0.0f;
                float progressStep = fileNames.Length / 100.0f;
                foreach(var fileName in fileNames)
                {
                    int fileSize = (int)reader.ReadU32();
                    using (var fileStream = new FileStream(Path.Combine(outPath, fileName), FileMode.Create, FileAccess.Write))
                    {
                        while (fileSize > 0)
                        {
                            int chunk = Math.Min(bufferSize, fileSize);
                            stream.Read(buffer, 0, chunk);
                            fileStream.Write(buffer, 0, chunk);
                            fileSize -= chunk;
                        }
                    }
                    progresses[2] = new Progress("Extracting pack file", (curProgress += progressStep));
                    yield return null;
                }
            }
            progresses.RemoveAt(2);
        }

        private IEnumerator ConvertVideoPackFile(string packFilePath, string sceneName, string outPath)
        {
            string curTempPath = Path.Combine(tempPath, Path.GetFileName(packFilePath));
            Directory.CreateDirectory(curTempPath);
            yield return ExtractPackFile(packFilePath, curTempPath);
            var videoFiles = Directory.GetFiles(curTempPath, "*.bik").Where(fn => !fn.EndsWith(sceneName + ".bik"));
            var cubemapPath = Path.Combine(curTempPath, sceneName + ".bik");
            progresses.Add(new Progress("Converting videos", 0.0f));
            float curProgress = 0.0f;
            float progressStep = videoFiles.Count() / 100.0f;

            if (File.Exists(cubemapPath))
            {
                yield return RunFFMPEG(
                    "-i", cubemapPath,
                    "-frames", "1",
                    "-vf", "shuffleframes='tile=4x2'",
                    Path.Combine(outPath, sceneName + ".png"));
            }

            foreach (var videoFile in videoFiles)
            {
                yield return RunFFMPEG(
                    "-i", videoFile,
                    "-c:v", "libvpx",
                    "-pix_fmt", "yuva420p",
                    "-crf", "4",
                    "-metadata:s:v:0", "alpha_mode=1",
                    "-auta-alt-ref", "0",
                    Path.Combine(outPath, Path.GetFileNameWithoutExtension(videoFile) + ".webm"));
                progresses[2] = new Progress("Converting videos", (curProgress += progressStep));
            }

            progresses.RemoveAt(2);
        }

        private IEnumerator RunFFMPEG(params string[] args)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = ffmpegPath;
            startInfo.Arguments = string.Join(" ", args.Select(a => $"\"{a}\""));
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            var process = Process.Start(startInfo);
            while (!process.HasExited)
                yield return null;
            if (process.ExitCode != 0)
            {
                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                Debug.LogError($"FFMPEG run failed\nArgs: {startInfo.Arguments}\n\n-------------------STDERR-------------------\n{stderr}\n\n-------------------STDOUT-------------------\n{stdout}");
            }
        }
    }
}
