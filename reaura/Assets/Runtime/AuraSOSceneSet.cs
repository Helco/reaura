using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aura
{
    public class AuraSOSceneSet : ScriptableObject
    {
        [SerializeField]
        private AuraScene[] scenes;

        public IEnumerable<AuraScene> Scenes
        {
            get => scenes;
            set
            {
                scenes = value.ToArray();
                scenesByName = null;
            }
        }

        private IReadOnlyDictionary<string, AuraScene> scenesByName = null;
        public IReadOnlyDictionary<string, AuraScene> ScenesByName => scenesByName ??
            (scenesByName = Scenes.ToDictionary(s => s.SceneName, s => s));
    }
}
