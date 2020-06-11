using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Aura.Script;

namespace Aura.Systems
{
    public abstract class BaseAnimateSystem : BaseDisposable, IGameSystem, IGraphicListSystem
    {
        public abstract string GraphicListName { get; }

        protected IWorldSprite[] sprites = Array.Empty<IWorldSprite>();
        protected IVideoTexture[] videos = Array.Empty<IVideoTexture>();
        public int GraphicCount
        {
            get => sprites.Length;
            set
            {
                foreach (var video in videos)
                    video?.Dispose();
                sprites = new IWorldSprite[value];
                videos = new IVideoTexture[value];
            }
        }

        protected override void DisposeManaged()
        {
            foreach (var video in videos)
                video?.Dispose();
        }

        public void Update(float timeDelta)
        {
            for (int i = 0; i < sprites.Length; i++)
                if (sprites[i].IsEnabled && videos[i].IsPlaying)
                    sprites[i].MarkDirty();
        }

        public void RegisterLoadFunctions(LoadSceneContext context, Interpreter interpreter)
        {
            int nextVideoI = 0;

            interpreter.RegisterFunction<IVideoTexture, string?, int, int, CubeFace>("PlayAVI", (video, _, posX, posY, face) =>
            {
                videos[nextVideoI] = video;
                var sprite = sprites[nextVideoI++] = context.AvailableWorldSprites.Dequeue();
                sprite.Face = face;
                sprite.Position = new Vector2(posX, posY);
                sprite.Texture = video;
            });
        }
    }
}
