using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Aura.Script;

namespace Aura.Systems
{
    public class GameWorldRendererSystem : BaseDisposable, IGameSystem
    {
        public IWorldRenderer? WorldRenderer { get; private set; }
        public bool IsPanorama => WorldRenderer is IPanoramaWorldRenderer;
        public event Action<Vector2> OnWorldClick = _ => { };

        private float lastTimeDelta = 0.0f;

        public GameWorldRendererSystem(IBackend backend)
        {
            backend.OnClick += OnScreenClick;
            backend.OnViewDrag += OnViewDrag;
        }

        public void CrossInitialize(IGameSystemContainer container)
        {
            foreach (var system in container.SystemsWith<IWorldInputHandler>())
                OnWorldClick += system.OnWorldClick;
        }

        public void OnBeforeSceneChange(LoadSceneContext context)
        {
            var graphicLists = context.Scene.EntityLists.Values.OfType<GraphicListNode>();
            int spriteCapacity = graphicLists.Sum(l => l.Graphics.Count);
            WorldRenderer = context.Backend.CreatePanorama(context.SceneAssets[$"{context.SceneName}.bik"], spriteCapacity);
            context.AvailableWorldSprites = new Queue<IWorldSprite>(WorldRenderer.Sprites);
        }

        public void Update(float timeDelta) => lastTimeDelta = timeDelta;

        private void OnScreenClick(Vector2 screenPos)
        {
            if (WorldRenderer != null && WorldRenderer.ConvertScreenToWorld(screenPos, out var worldPos))
                OnWorldClick(worldPos);
        }

        private void OnViewDrag(Vector2 mouseMove)
        {
            if (WorldRenderer == null || !IsPanorama)
                return;
            var panorama = (IPanoramaWorldRenderer)WorldRenderer;
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
        }
    }
}
