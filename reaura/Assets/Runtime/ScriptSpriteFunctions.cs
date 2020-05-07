using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Aura.Script;

namespace Aura
{
    public class ScriptSpriteFunctions : MonoBehaviour
    {
        private Dictionary<int, AuraSpriteRenderer> sprites = new Dictionary<int, AuraSpriteRenderer>();

        private void Start()
        {
            var sceneLoader = FindObjectOfType<AuraSceneLoader>();
            if (sceneLoader == null)
                throw new MissingComponentException("ScriptSpriteFunctions cannot find the scene loader");
            sceneLoader.OnLoadedScene += OnLoadedScene;

            var execution = GetComponentInParent<AuraScriptExecution>();
            if (execution == null)
                throw new MissingComponentException("ScriptSpriteFunctions cannot find the script execution object");
            var interpreter = execution.Interpreter;
            interpreter.RegisterFunction<int>("ShowSprite", ScrShowSprite);
            interpreter.RegisterFunction<int>("ShowAlphaSprite", ScrShowAlphaSprite);
            interpreter.RegisterFunction<int>("HideAlphaSprite", ScrHideAlphaSprite);
        }

        private static readonly Regex IdRegex = new Regex(@"(\d+)");
        private void OnLoadedScene(AuraScene scene)
        {
            sprites.Clear();
            var fonAnimate = scene.transform.Find("Sprites");
            if (fonAnimate == null)
                return;
            var spriteRenderers = fonAnimate.GetComponentsInChildren<AuraSpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                var match = IdRegex.Match(spriteRenderer.gameObject.name);
                if (!match.Success)
                    throw new InvalidProgramException($"Could not find id in name of sprite {spriteRenderer.gameObject.name}");
                int id = int.Parse(match.Groups[1].Value);
                sprites[id] = spriteRenderer;
            }
        }

        private void ScrShowSprite(int id)
        {
            if (!sprites.TryGetValue(id, out var sprite))
                throw new ArgumentOutOfRangeException($"Unknown sprite id {id}");
            sprite.enabled = true;
        }

        private void ScrShowAlphaSprite(int id)
        {
            throw new NotImplementedException();
        }

        private void ScrHideAlphaSprite(int id)
        {
            throw new NotImplementedException();
        }
    }
}
