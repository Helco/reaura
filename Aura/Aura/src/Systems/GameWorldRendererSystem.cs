using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aura.Script;

namespace Aura.Systems
{
    public class GameWorldRendererSystem : BaseDisposable, IGameSystem
    {
        public IWorldRenderer? WorldRenderer { get; private set; }
        public bool IsPanorama => WorldRenderer is IPanoramaWorldRenderer;
        public bool IsPuzzle => WorldRenderer is IPuzzleWorldRenderer;
        public event Action<Vector2> OnWorldClick = _ => { };

        private float lastTimeDelta = 0.0f;
        private LoadSceneContext? context = null; // TODO: this should not be a member

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
            this.context = context;
            var graphicLists = context.Scene.EntityLists.Values.OfType<GraphicListNode>();
            int spriteCapacity = graphicLists.Sum(l => l.Graphics.Count);
            WorldRenderer = context.Type switch
            {
                SceneType.Panorama => context.Backend.CreatePanoramaRenderer(context.SceneAssets[$"{context.SceneName}.bik"], spriteCapacity),
                SceneType.Puzzle => context.Backend.CreatePuzzleRenderer(spriteCapacity),
                var _ => throw new NotSupportedException($"Unsupported scene type to load world renderer {context.Type}")
            };
            context.AvailableWorldSprites = new Queue<IWorldSprite>(WorldRenderer.Sprites);
        }

        public void Update(float timeDelta) => lastTimeDelta = timeDelta;

        [ScriptFunction]
        private void ScrLoad_Fon(string fonName)
        {
            if (!IsPuzzle || WorldRenderer == null)
                throw new InvalidProgramException("Load_Fon is only supported for puzzles");
            if (context == null || !context.SceneAssets.TryGetValue(fonName.Replace(".\\", ""), out var fonStream))
                throw new FileNotFoundException($"Could not find background asset {fonName}");
            var puzzleRenderer = (IPuzzleWorldRenderer)WorldRenderer;
            puzzleRenderer.LoadBackground(fonStream);
        }

        // TODO: This is Input, not world rendering, what does it do here?!
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
