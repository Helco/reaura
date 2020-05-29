using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aura.Script;
using Aura.Systems;

namespace Aura
{
    public partial class Game : BaseDisposable, IGameSystemContainer
    {
        private IBackend Backend { get; }
        private IGameSystem[] systems;
        private Interpreter gameInterpreter;

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
                new FonAnimateSystem(),
                new CellSystem(),
                new DummyScriptSystem()
            }.Concat(backendSystems).ToArray();
            foreach (var system in Systems)
                system.CrossInitialize(this);
            foreach (var vsSystem in SystemsWith<IGameVariableSet>())
                gameInterpreter.RegisterVariableSet(vsSystem.VariableSetName, vsSystem);
            foreach (var fSystem in Systems)
                fSystem.RegisterGameFunctions(gameInterpreter);
            gameInterpreter.RegisterFunction<string, int, int>("LoadSceneTransfuse", ScrLoadSceneTransfuse);

            LoadScene("008");
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
        }

        private void ScrLoadSceneTransfuse(string sceneName, int startPosX, int startPosY)
        {
            LoadScene(sceneName);
            SystemsWith<GameWorldRendererSystem>().Single().WorldRenderer?.SetViewAt(new Vector2(startPosX, startPosY));
        }

        private void LoadScene(string sceneName)
        {
            var context = new LoadSceneContext(Backend, sceneName);
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
                    curGLInterpreter.Execute(graphic.Value);
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
                gameInterpreter.Execute(onLoadEvent.Action);
        }
    }
}
