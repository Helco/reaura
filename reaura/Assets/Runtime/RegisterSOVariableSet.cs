using UnityEngine;

namespace Aura
{
    public class RegisterSOVariableSet : MonoBehaviour
    {
        [SerializeField]
        private string scriptName = "";
        [SerializeField]
        private AuraSOVariableSet variableSet = null;

        private void OnEnable()
        {
            transform.GetComponentInParent<AuraScriptExecution>()
                .Interpreter.RegisterVariableSet(scriptName, variableSet);
        }
    }
}
