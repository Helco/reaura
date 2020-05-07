using UnityEngine;
using Aura.Script;
using System;

namespace Aura
{
    public class ScriptSoundFunctions : MonoBehaviour
    {
        private void Awake()
        {
            var execution = GetComponentInParent<AuraScriptExecution>();
            if (execution == null)
                throw new MissingComponentException("ScriptSoundFunctions cannot find the script execution object");
            var interpreter = execution.Interpreter;
            interpreter.RegisterFunction<string, int, int>("StartSound", ScrStartSound);
            interpreter.RegisterFunction<string>("StopSound", ScrStopSound);
            interpreter.RegisterFunction<string, int, int>("SetSoundVolume_And_Pan", ScrSetSoundVolume_And_Pan);
            interpreter.RegisterFunction<string, int>("SetBackgroundSoundVolume", ScrSetBackgroundSoundVolume);
            interpreter.RegisterFunction<string>("StopBackgroundSound", ScrStopBackgroundSound);

        }

        private void ScrStartSound(string arg1, int arg2, int arg3)
        {
            throw new NotImplementedException();
        }

        private void ScrStopSound(string obj)
        {
            throw new NotImplementedException();
        }

        private void ScrSetSoundVolume_And_Pan(string arg1, int arg2, int arg3)
        {
            Debug.LogWarning("SetSoundVolume_And_Pan is not implemented");
        }

        private void ScrSetBackgroundSoundVolume(string arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        private void ScrStopBackgroundSound(string obj)
        {
            throw new NotImplementedException();
        }
    }
}
