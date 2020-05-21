using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Numerics;
using Aura.Script;

namespace Aura
{
    public partial class Game
    {
        private void LoadScene(string sceneName)
        {
            var context = new LoadSceneContext(Backend, sceneName);

            var graphicLists = context.Scene.EntityLists.Values.OfType<GraphicListNode>();
            worldRenderer = Backend.CreateWorldRenderer(graphicLists.Sum(l => l.Graphics.Count));
            context.AvailableWorldSprites = new Queue<IWorldSprite>(worldRenderer.Sprites);

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

            if (context.Scene.Events.TryGetValue("@OnLoadScene", out var onLoadEvent))
                gameInterpreter.Execute(onLoadEvent.Action);
        }
    }
}
