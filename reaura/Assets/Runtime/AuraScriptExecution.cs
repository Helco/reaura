using UnityEngine;
using Aura.Script;

namespace Aura
{
    public class AuraScriptExecution : MonoBehaviour
    {
        private Interpreter interpreter = null;
        public Interpreter Interpreter => interpreter ?? (interpreter = new Interpreter());
    }
}
