using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aura.Script;
using Aura.Systems;

namespace Aura
{
    public class Game : BaseDisposable, IGameSystemContainer
    {
        private IGameSystem[] systems;
        private Interpreter gameInterpreter;
        private Action? onNextUpdate = null;

        public IBackend Backend { get; }
        public IReadOnlyCollection<IGameSystem> Systems => systems;
        public IEnumerable<T> SystemsWith<T>() where T : IGameSystem => systems.OfType<T>();

        public Game(IBackend backend, params IGameSystem[] backendSystems)
        {
            Backend = backend;
            gameInterpreter = new Interpreter();
            systems = new IGameSystem[]
            {
                new GameWorldRendererSystem(Backend),
                new GlobalsSystem(Backend),
                new InventorySystem(Backend),
                new SpriteSystem(),
                new AnimateSystem(),
                new FonAnimateSystem(),
                new CellSystem(),
                new CursorSystem(),
                new FullScreenVideoSystem(),
                new DummyScriptSystem()
            }.Concat(backendSystems).ToArray();
            foreach (var system in Systems)
                system.CrossInitialize(this);
            foreach (var vsSystem in SystemsWith<IGameVariableSet>())
                gameInterpreter.RegisterVariableSet(vsSystem.VariableSetName, vsSystem);
            foreach (var fSystem in Systems)
            {
                gameInterpreter.RegisterAllFunctionsIn(fSystem);
                fSystem.RegisterGameFunctions(gameInterpreter);
            }
            gameInterpreter.RegisterAllFunctionsIn(this);

            LoadScene("010", SceneType.Panorama);
        }

        protected override void DisposeManaged()
        {
            foreach (var system in Systems)
                system.Dispose();
        }

        public void Update(float timeDelta)
        {
            foreach (var ptSystem in Systems)
                ptSystem.Update(timeDelta);
            gameInterpreter.Continue();
            onNextUpdate?.Invoke();
            onNextUpdate = null;
        }

        [ScriptFunction]
        [ScriptFunction("LoadScene")]
        private void ScrLoadSceneTransfuse(string sceneName, int startPosX, int startPosY)
        {
            onNextUpdate += () =>
            {
                LoadScene(sceneName, SceneType.Panorama);
                SystemsWith<GameWorldRendererSystem>().Single().WorldRenderer?.SetViewAt(new Vector2(startPosX, startPosY));
            };
            gameInterpreter.CancelCurrentExecution();
        }

        [ScriptFunction]
        private void ScrLoadPuzzleTransfuse(string puzzleName)
        {
            onNextUpdate += () =>
            {
                LoadScene(puzzleName, SceneType.Puzzle);
            };
            gameInterpreter.CancelCurrentExecution();
        }

        private void LoadScene(string sceneName, SceneType type)
        {
            Console.WriteLine($"Loading scene \"{sceneName}\"");
            var context = new LoadSceneContext(Backend, sceneName, type);
            foreach (var evSystem in Systems)
                evSystem.OnBeforeSceneChange(context);

            var graphicListSystems = SystemsWith<IGraphicListSystem>();
            var graphicListInterpreter = new Interpreter();
            graphicListInterpreter.RegisterArgumentMapper(typeof(ITexture), typeof(StringNode), context.ScrArgLoadImage);
            graphicListInterpreter.RegisterArgumentMapper(typeof(IVideoTexture), typeof(StringNode), context.ScrArgLoadVideo);
            foreach (var glSystem in graphicListSystems)
            {
                if (!context.Scene.EntityLists.TryGetValue(glSystem.GraphicListName, out var entityList))
                    continue;
                if (!(entityList is GraphicListNode))
                    throw new InvalidDataException($"{entityList.Position}: Expected {entityList.Name} to be a graphic list");
                var graphicList = (GraphicListNode)entityList;

                var curGLInterpreter = graphicListInterpreter.Clone();
                glSystem.RegisterLoadFunctions(context, curGLInterpreter);
                glSystem.GraphicCount = graphicList.Graphics.Count;
                foreach (var graphic in graphicList.Graphics.Values)
                    curGLInterpreter.ExecuteSync(graphic.Value);
            }

            var objectListSystems = SystemsWith<IObjectListSystem>();
            foreach (var olSystem in objectListSystems)
            {
                if (!context.Scene.EntityLists.TryGetValue(olSystem.ObjectListName, out var entityList))
                    continue;
                if (!(entityList is ObjectListNode))
                    throw new InvalidDataException($"{entityList.Position}: Expected {entityList.Name} to be an object list");
                var objectList = (ObjectListNode)entityList;

                foreach (var obj in objectList.Objects.Values)
                    olSystem.AddObject(context, obj);
            }

            foreach (var evSystem in Systems)
                evSystem.OnAfterSceneChange();
            if (context.Scene.Events.TryGetValue("@OnLoadScene", out var onLoadEvent))
                gameInterpreter.ExecuteSync(onLoadEvent.Action);
        }
    }
}
