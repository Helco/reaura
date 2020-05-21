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
        private IBackground? background;
        private IWorldRenderer? worldRenderer;

        private IEnumerable<T> SystemsWith<T>() where T : IGameSystem => systems.OfType<T>();

        public Game(IBackend backend)
        {
            Backend = backend;
            gameInterpreter = new Interpreter();
            systems = new IGameSystem[]
            {
                new Systems.SpriteSystem(),
                new Systems.FonAnimateSystem(),
                new Systems.CellSystem()
            };

            foreach (var vsSystem in SystemsWith<IGameVariableSet>())
                gameInterpreter.RegisterVariableSet(vsSystem.VariableSetName, vsSystem.VariableSet);
            foreach (var ihSystem in SystemsWith<IWorldInputHandler>())
                Backend.OnMouseClick += ihSystem.OnWorldClick;

            LoadScene("009");
        }

        public void Update(float timeDelta)
        {
        }
    }
}
