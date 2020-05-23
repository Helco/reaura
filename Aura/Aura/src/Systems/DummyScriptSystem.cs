using System;
using System.Numerics;
using Aura.Script;

namespace Aura.Systems
{
    public class DummyScriptSystem : BaseDisposable, IGameSystem
    {
        public void RegisterGameFunctions(Interpreter interpreter)
        {
            interpreter.RegisterFunction<string, int, int>("SetSoundVolume_And_Pan", (a, b, c) => DummyScriptFunction("SetSoundVolume_And_Pan"));
        }

        private void DummyScriptFunction(string name)
        {
            Console.WriteLine($"Warning: Unimplemented script function \"{name}\"");
        }
    }
}
