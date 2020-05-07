using UnityEngine;
using Aura.Script;
using System;

namespace Aura
{
    public enum CursorType
    {
        None,
        Up,
        Down
    }

    [RequireComponent(typeof(SphericalArea))]
    public class AuraCell : MonoBehaviour
    {
        [SerializeField]
        private CursorType cursor;
        [SerializeField]
        private string scriptFile;
        [SerializeField]
        [TextArea(3, 24)]
        private string script;

        public CursorType Cursor
        {
            get => cursor;
            set
            {
                cursor = value;
            }
        }

        public string Script
        {
            get => script;
            set
            {
                script = value;
                if (CompiledScript != null)
                    ParseScript();
            }
        }
        public InstructionBlockNode CompiledScript { get; private set; } = null;

        public string ScriptFile
        {
            get => scriptFile;
            set => scriptFile = value;
        }

        private SphericalArea area;
        private MouseCameraInput mouseInput;
        private AuraScriptExecution scriptExecution;

        private void Awake()
        {
            area = GetComponent<SphericalArea>();
            if (area == null)
                throw new MissingComponentException("AuraCell is missing a SphericalArea component");
            mouseInput = FindObjectOfType<MouseCameraInput>();
            if (mouseInput == null)
                throw new MissingComponentException("AuraCell cannot find the mouse camera input object");
            scriptExecution = FindObjectOfType<AuraScriptExecution>();
            if (scriptExecution == null)
                throw new MissingComponentException("AuraCell cannot find the script execution object");

            mouseInput.OnClicked += OnClicked;
            ParseScript();
        }

        private void ParseScript()
        {
            var tokenizer = new Tokenizer(ScriptFile, Script);
            CompiledScript = new CellScriptParser(tokenizer).ParseCellScript();
        }

        private void OnClicked(Vector2 pos)
        {
            if (!area.IsPointInside(pos))
                return;
            scriptExecution.Interpreter.Execute(CompiledScript);
        }
    }
}
