using System.Collections.Generic;
using UnityEngine;
using Aura.Script;
using System;
using System.Text.RegularExpressions;

namespace Aura
{
    public class ScriptFonAVIFunctions : MonoBehaviour
    {
        private Dictionary<int, AuraVideoSprite> videos = new Dictionary<int, AuraVideoSprite>();

        private void Start()
        {
            var sceneLoader = FindObjectOfType<AuraSceneLoader>();
            if (sceneLoader == null)
                throw new MissingComponentException("ScriptFonAVIFunctions cannot find the scene loader");
            sceneLoader.OnLoadedScene += OnLoadedScene;

            var execution = GetComponentInParent<AuraScriptExecution>();
            if (execution == null)
                throw new MissingComponentException("ScriptFonAVIFunctions cannot find the script execution object");
            var interpreter = execution.Interpreter;
            interpreter.RegisterFunction<int>("LoadFonAVI", ScrLoadFonAVI);
            interpreter.RegisterFunction<int>("StopFonAVI", ScrStopFonAVI);
            interpreter.RegisterFunction<int>("ResumeFonAVI", ScrResumeFonAVI);
            interpreter.RegisterFunction<int>("ImmediatelySuspendFonAVI", ScrImmediatelySuspendFonAVI);
            interpreter.RegisterFunction<int>("ScriptStopSuspendFonAVI", ScrStopSuspendFonAVI);
        }

        private static readonly Regex IdRegex = new Regex(@"(\d+)");
        private void OnLoadedScene(AuraScene scene)
        {
            videos.Clear();
            var fonAnimate = scene.transform.Find("Fon_Animate");
            if (fonAnimate == null)
                return;
            var videoSprites = fonAnimate.GetComponentsInChildren<AuraVideoSprite>();
            foreach (var videoSprite in videoSprites)
            {
                var match = IdRegex.Match(videoSprite.gameObject.name);
                if (!match.Success)
                    throw new InvalidProgramException($"Could not find id in name of video sprite {videoSprite.gameObject.name}");
                int id = int.Parse(match.Groups[1].Value);
                videos[id] = videoSprite;
            }
        }

        private void ScrLoadFonAVI(int id)
        {
            if (!videos.TryGetValue(id, out var videoSprite))
                throw new ArgumentOutOfRangeException($"Unknown video id {id}");
            videoSprite.Play();
        }

        private void ScrStopFonAVI(int id)
        {
            if (!videos.TryGetValue(id, out var videoSprite))
                throw new ArgumentOutOfRangeException($"Unknown video id {id}");
            videoSprite.Stop();
        }

        private void ScrResumeFonAVI(int id)
        {
            if (!videos.TryGetValue(id, out var videoSprite))
                throw new ArgumentOutOfRangeException($"Unknown video id {id}");
            videoSprite.Resume();
        }

        private void ScrImmediatelySuspendFonAVI(int id)
        {
            if (!videos.TryGetValue(id, out var videoSprite))
                throw new ArgumentOutOfRangeException($"Unknown video id {id}");
            videoSprite.Suspend();
        }

        private void ScrStopSuspendFonAVI(int id)
        {
            // is it stop? is it suspend? is it stopping the suspend?
            throw new NotImplementedException();
        }
    }
}
