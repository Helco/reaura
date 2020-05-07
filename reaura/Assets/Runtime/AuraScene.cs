using UnityEngine;
using Aura.Script;

namespace Aura
{
    public class AuraScene : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;
        public string SceneName
        {
            get => sceneName;
            set => sceneName = value;
        }

        [SerializeField]
        [TextArea(3, 24)]
        private string onLoadSceneScript;
        public string OnLoadSceneScript
        {
            get => onLoadSceneScript;
            set
            {
                onLoadSceneScript = value;
                if (CompiledOnLoadSceneScript != null)
                    ParseScripts();
            }
        }

        [SerializeField]
        [TextArea(3, 24)]
        private string onLoadFirstCubeFaceScript;
        public string OnLoadFirstCubeFaceScript
        {
            get => onLoadFirstCubeFaceScript;
            set
            {
                onLoadFirstCubeFaceScript = value;
                if (CompiledOnLoadFirstCubeFaceScript != null)
                    ParseScripts();
            }
        }

        public InstructionBlockNode CompiledOnLoadSceneScript { get; private set; } = null;
        public InstructionBlockNode CompiledOnLoadFirstCubeFaceScript { get; private set; } = null;

        private AuraScriptExecution scriptExecution;

        private void Awake()
        {
            scriptExecution = FindObjectOfType<AuraScriptExecution>();
            if (scriptExecution == null)
                throw new MissingComponentException("AuraScene cannot find the script execution object");

            ParseScripts();
        }

        private void ParseScripts()
        {
            var tokenizer = new Tokenizer(SceneName, onLoadSceneScript);
            CompiledOnLoadSceneScript = new CellScriptParser(tokenizer).ParseCellScript();

            tokenizer = new Tokenizer(SceneName, onLoadFirstCubeFaceScript);
            CompiledOnLoadFirstCubeFaceScript = new CellScriptParser(tokenizer).ParseCellScript();
        }

        private void Start()
        {
            scriptExecution.Interpreter.Execute(CompiledOnLoadSceneScript);
        }
    }
}
