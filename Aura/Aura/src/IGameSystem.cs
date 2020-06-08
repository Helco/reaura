using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Aura.Script;

namespace Aura
{
    public interface IGameSystemContainer
    {
        IReadOnlyCollection<IGameSystem> Systems { get; }
        IEnumerable<T> SystemsWith<T>() where T : IGameSystem;
        IBackend Backend { get; }
    }
    
    public interface IGameSystem : IDisposable
    {
        void CrossInitialize(IGameSystemContainer container) { }
        void OnBeforeSceneChange(LoadSceneContext context) { }
        void OnAfterSceneChange() { }
        void RegisterGameFunctions(Interpreter interpreter) { }
        void Update(float timeDelta) { }
    }

    public interface IGameVariableSet : IGameSystem, IVariableSet
    {
        string VariableSetName { get; }
    }

    public interface IWorldInputHandler : IGameSystem
    {
        void OnWorldClick(Vector2 pos);
    }

    public interface IGraphicListSystem : IGameSystem
    {
        string GraphicListName { get; }
        int GraphicCount { get; set; }
        void RegisterLoadFunctions(LoadSceneContext context, Interpreter interpreter);
    }

    public interface IObjectListSystem : IGameSystem
    {
        string ObjectListName { get; }

        void AddObject(LoadSceneContext context, ObjectNode objectNode);
    }
}
