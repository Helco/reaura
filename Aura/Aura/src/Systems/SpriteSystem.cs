using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Aura.Script;

namespace Aura.Systems
{
    public class SpriteSystem : BaseDisposable, IGameSystem, IGraphicListSystem
    {
        public string GraphicListName => "&Sprites";

        private IWorldSprite[] sprites = Array.Empty<IWorldSprite>();
        public int GraphicCount
        {
            get => sprites.Length;
            set
            {
                foreach (IWorldSprite sprite in sprites)
                    sprite.Texture?.Dispose();
                sprites = new IWorldSprite[value];
            }
        }

        protected override void DisposeManaged()
        {
            foreach (IWorldSprite sprite in sprites)
                sprite.Texture?.Dispose();
        }

        public void RegisterGameFunctions(Interpreter interpreter)
        {
            interpreter.RegisterFunction<int>("ShowSprite", idx => sprites[idx].IsEnabled = true);
            interpreter.RegisterFunction<int>("HideSprite", idx => sprites[idx].IsEnabled = false);
        }

        public void RegisterLoadFunctions(LoadSceneContext context, Interpreter interpreter)
        {
            int nextSpriteI = 0;

            interpreter.RegisterFunction<ITexture, int, int, CubeFace>("Sprite", (texture, posX, posY, face) =>
            {
                var sprite = sprites[nextSpriteI++] = context.AvailableWorldSprites.Dequeue();
                sprite.Face = face;
                sprite.Position = new Vector2(posX, posY);
                sprite.Texture = texture;
            });
        }
    }
}
