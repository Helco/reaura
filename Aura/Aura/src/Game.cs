using Aura.Script;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aura
{
    public partial class Game
    {
        private IBackend Backend { get; }
        private IGameSystem[] systems;
        private Interpreter gameInterpreter;
        private IPanoramaWorldRenderer? panorama;
        private float lastTimeDelta = 0.0f;

        private IEnumerable<T> SystemsWith<T>() where T : IGameSystem => systems.OfType<T>();

        public Game(IBackend backend)
        {
            Backend = backend;
            gameInterpreter = new Interpreter();
            systems = new IGameSystem[]
            {
                new Systems.SpriteSystem(),
                new Systems.FonAnimateSystem(),
                new Systems.CellSystem(),
                new Systems.GlobalsSystem(Backend),
                new Systems.InventorySystem(Backend),
                new Systems.DummyScriptSystem()
            };

            foreach (var vsSystem in SystemsWith<IGameVariableSet>())
                gameInterpreter.RegisterVariableSet(vsSystem.VariableSetName, vsSystem);
            foreach (var fSystem in SystemsWith<IGameFunctions>())
                fSystem.RegisterGameFunctions(gameInterpreter);

            LoadScene("009");

            Backend.OnViewDrag += mouseMove =>
            {
                if (panorama == null)
                    return;
                mouseMove *= 20.0f * 3.141592653f / 180.0f * lastTimeDelta;
                var rot = panorama.ViewRotation;
                rot.X += mouseMove.Y;
                rot.Y += mouseMove.X;
                if (rot.Y < 0)
                    rot.Y += 2 * 3.141592653f;
                if (rot.Y > 2 * 3.141592653f)
                    rot.Y -= 2 * 3.141592653f;
                rot.X = MathF.Min(MathF.Max(rot.X, -MathF.PI / 2), MathF.PI / 2);
                panorama.ViewRotation = rot;
            };
        }

        public void Update(float timeDelta)
        {
            lastTimeDelta = timeDelta;
            foreach (var ptSystem in SystemsWith<IPerTickSystem>())
                ptSystem.Update(timeDelta);
        }
    }
}
