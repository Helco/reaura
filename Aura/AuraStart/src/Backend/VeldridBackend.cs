using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;

namespace Aura.Veldrid
{
    public class AuraTexture : BaseDisposable, ITexture
    {
        public Texture Texture { get; }
        private bool ownsTexture;

        public AuraTexture(Texture texture, bool ownsTexture = true)
        {
            Texture = texture;
            this.ownsTexture = ownsTexture;
        }

        protected override void DisposeManaged()
        {
            if (ownsTexture)
                Texture.Dispose();
        }

        public Vector2 Size => new Vector2(Texture.Width, Texture.Height);
    }

    public class VeldridBackend : BaseDisposable, IBackend
    {
        private Sdl2Window window;
        private GraphicsDevice device;
        private SpriteRendererCommon spriteRendererCommon;
        private VideoTextureSet videoTextureSet;
        private WorldRendererSet worldRendererSet;
        private ResourceFactory factory => device.ResourceFactory;

        public VeldridBackend(Sdl2Window window, GraphicsDevice device)
        {
            this.window = window;
            this.device = device;
            spriteRendererCommon = new SpriteRendererCommon(device);
            videoTextureSet = new VideoTextureSet(device);
            worldRendererSet = new WorldRendererSet(device);

            window.MouseMove += HandleMouseMove;
            window.MouseDown += HandleMouseDown;
        }

        protected override void DisposeManaged()
        {
            spriteRendererCommon.Dispose();
            videoTextureSet.Dispose();
        }

        public void Update(float timeDelta)
        {
            videoTextureSet.Update(timeDelta);
        }

        public void Render()
        {
            worldRendererSet.RenderAll();
        }

        public InputSnapshot? CurrentInput { get; set; }
        public string? AssetPath { get; set; }

        public Vector2 CursorPosition
        {
            get => CurrentInput?.MousePosition ?? Vector2.Zero;
            set => window.SetMousePosition(value);
        }

        public event Action<Vector2> OnClick = _ => { };
        public event Action<Vector2> OnViewDrag = _ => { };

        private void HandleMouseDown(MouseEvent args)
        {
            if (args.MouseButton == MouseButton.Left && CurrentInput != null)
                OnClick(CurrentInput.MousePosition);
        }

        private void HandleMouseMove(MouseMoveEventArgs args)
        {
            if (args.State.IsButtonDown(MouseButton.Right))
                OnViewDrag(window.MouseDelta);
        }

        public Stream? OpenAssetFile(string resourceName)
        {
            if (AssetPath == null)
                return null;
            var fullPath = Path.Combine(AssetPath, resourceName);
            try
            {
                return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            }
            catch(IOException)
            {
                return null;
            }
        }

        public ITexture CreateImage(Stream stream) =>
            new AuraTexture(ImageLoader.LoadImage(stream, device), ownsTexture: true);

        public IVideoTexture CreateVideo(Stream stream) =>
            videoTextureSet.CreateFromStream(stream);

        public IPanoramaWorldRenderer CreatePanorama(Stream stream, int spriteCapacity)
        {
            var worldRenderer = new PanoramaWorldRenderer(spriteCapacity, spriteRendererCommon, device.SwapchainFramebuffer);
            worldRenderer.WorldTexture = ImageLoader.LoadCubemap(stream, device);
            worldRenderer.WorldRendererSet = worldRendererSet;
            return worldRenderer;
        }
    }
}
