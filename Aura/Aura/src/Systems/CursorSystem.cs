using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Aura.Script;

namespace Aura.Systems
{
    public enum CursorType
    {
        Empty = 0,
        Default,
        Active,
        Down,
        Left,
        Right,
        Up
        // TODO: Custom?
    }

    public class CursorSystem : BaseDisposable, IGameSystem
    {
        private static readonly IReadOnlyDictionary<CursorType, string> cursorTextureNames = new Dictionary<CursorType, string>()
        {
            { CursorType.Default, @"Global\Cursors\Cursor_Default.dds" },
            { CursorType.Active, @"Global\Cursors\Cursor_Active.dds" },
            { CursorType.Down, @"Global\Cursors\Down_cursor.dds" },
            { CursorType.Left, @"Global\Cursors\Left_cursor.dds" },
            { CursorType.Right, @"Global\Cursors\Right_cursor.dds" },
            { CursorType.Up, @"Global\Cursors\Up_cursor.dds" }
        };
        private IBackend? backend;
        private GameWorldRendererSystem? worldRendererSystem;
        private IPuzzleWorldRenderer? renderer;
        private IWorldSprite? sprite;
        private ITexture[] textures = Array.Empty<ITexture>();
        private CursorType backgroundType = CursorType.Empty;
        private CursorType? foregroundType = null;

        public CursorType BackgroundType
        {
            get => backgroundType;
            set
            {
                if (ForegroundType == null && backgroundType != value)
                    Type = value;
                backgroundType = value;
            }
        }

        public CursorType? ForegroundType
        {
            get => foregroundType;
            set
            {
                if (foregroundType != value)
                    Type = value ?? BackgroundType;
                foregroundType = value;
            }
        }

        public CursorType Type
        {
            get => ForegroundType ?? BackgroundType;
            private set
            {
                if (sprite == null)
                    return;
                sprite.IsEnabled = value != CursorType.Empty;
                sprite.Texture = textures[(int)value];
            }
        }

        public Vector2? WorldPos
        {
            get
            {
                if (backend == null || worldRendererSystem?.WorldRenderer == null)
                    return null;
                var worldPos = Vector2.Zero;
                return worldRendererSystem?.WorldRenderer?.ConvertScreenToWorld(backend.CursorPosition, out worldPos) ?? false
                    ? new Vector2?(worldPos) : null;
            }
        }

        protected override void DisposeManaged()
        {
            renderer?.Dispose();
            foreach (var texture in textures)
                texture.Dispose();
        }

        public void CrossInitialize(IGameSystemContainer container)
        {
            backend = container.Backend;
            worldRendererSystem = container.SystemsWith<GameWorldRendererSystem>().Single();
            renderer = backend.CreatePuzzleRenderer(1);
            renderer.Order = 1000;
            sprite = renderer.Sprites.Single();
            sprite.IsEnabled = false;
            sprite.Face = CubeFace.Front;
            textures = new ITexture[Enum.GetValues(typeof(CursorType)).Length];
            foreach (var pair in cursorTextureNames)
            {
                using var imageStream = backend.OpenAssetFile(pair.Value);
                if (imageStream == null)
                    throw new FileNotFoundException($"Could not find cursor texture: {pair.Value}");
                textures[(int)pair.Key] = backend.CreateImage(imageStream);
            }
        }

        public void OnBeforeSceneChange(LoadSceneContext context)
        {
            Type = CursorType.Default;
        }

        public void Update(float timeDelta)
        {
            if (sprite == null || sprite.Texture == null || backend == null)
                return;
            sprite.Position = backend.CursorPosition - sprite.Texture.Size / 2.0f;
        }
    }
}
