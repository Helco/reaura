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
            interpreter.RegisterFunction<string>("StopSound", a => DummyScriptFunction("StopSound"));
            interpreter.RegisterFunction<string, int, int, string>("StartSound", (a,b,c,d) => DummyScriptFunction("StartSound"));
            interpreter.RegisterFunction<string, int>("StartBackgroundSound", (a, b) => DummyScriptFunction("StartBackgroundSound"));
            interpreter.RegisterFunction<string>("StopBackgroundSound", (a) => DummyScriptFunction("StopBackgroundSound"));
            interpreter.RegisterFunction<string, int>("StartNoise", (a, b) => DummyScriptFunction("StartNoise"));
            interpreter.RegisterFunction<string, string>("StopMP3", (a, b) => DummyScriptFunction("StopMP3"));
            interpreter.RegisterFunction<string, int, string>("StartMP3", (a, b, c) => DummyScriptFunction("StartMP3"));
            interpreter.RegisterFunction<int>("Fade", (a) => DummyScriptFunction("Fade"));
            interpreter.RegisterFunction<int>("UnFade", (a) => DummyScriptFunction("UnFade"));
            interpreter.RegisterFunction<int, int>("Mouse_Go_To", (a, b) => DummyScriptFunction("Mouse_Go_To"));
        }

        private void DummyScriptFunction(string name)
        {
            Console.WriteLine($"Warning: Unimplemented script function \"{name}\"");
        }
    }
}
