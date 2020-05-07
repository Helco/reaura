using System;
using System.Linq;
using UnityEngine;

namespace Aura
{
    public class AuraSceneLoader : MonoBehaviour
    {
        [SerializeField]
        private AuraScene sceneToLoadAtStart = null;
        [SerializeField]
        private AuraSOSceneSet sceneSet = null;

        public AuraScene CurrentScene { get; private set; } = null;
        public event Action<AuraScene> OnLoadedScene = _ => { };

        private void Awake()
        {
            if (sceneToLoadAtStart == null)
                throw new MissingReferenceException("AuraSceneLoader needs a scene to load");
            if (sceneSet == null)
                throw new MissingReferenceException("AuraSceneLoader needs a scene set");
            if (!sceneSet.Scenes.Contains(sceneToLoadAtStart))
                throw new MissingReferenceException("Somehow the scene to load at start is not in the scene set, maybe you should reimport the scene set!");

            var execution = FindObjectOfType<AuraScriptExecution>();
            if (execution == null)
                throw new MissingReferenceException("Could not find the aura script execution object");
            var interpreter = execution.Interpreter;
            interpreter.RegisterFunction<string, int, int>("LoadScene", ScrLoadScene);
            interpreter.RegisterFunction<string, int, int>("LoadSceneTransfuse", ScrLoadSceneTransfuse);
            interpreter.RegisterFunction<string>("LoadPuzzle", ScrLoadPuzzle);
            interpreter.RegisterFunction<string>("LoadPuzzleTransfuse", ScrLoadPuzzleTransfuse);
        }

        private void Start() => LoadScene(sceneToLoadAtStart);

        private void LoadScene(AuraScene scene)
        {
            if (CurrentScene != null)
                Destroy(CurrentScene);
            CurrentScene = Instantiate(sceneToLoadAtStart);
            OnLoadedScene.Invoke(CurrentScene);
        }

        public void LoadScene(string sceneName)
        {
            if (!sceneSet.ScenesByName.TryGetValue(sceneName, out var scene))
                throw new System.ArgumentOutOfRangeException($"Unknown scene to load{sceneName}");
            LoadScene(scene);
        }

        private void ScrLoadScene(string sceneName, int startPosX, int startPosY)
        {
            throw new NotImplementedException();
        }

        private void ScrLoadSceneTransfuse(string sceneName, int startPosX, int startPosY)
        {
            throw new NotImplementedException();
        }

        private void ScrLoadPuzzle(string sceneName)
        {
            throw new NotImplementedException();
        }

        private void ScrLoadPuzzleTransfuse(string sceneName)
        {
            throw new NotImplementedException();
        }
    }
}
