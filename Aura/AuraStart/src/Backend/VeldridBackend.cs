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

    public interface IDebugGameSystem : IGameSystem
    {
        void OnKeyDown(Key key) { }
    }

    public class VeldridBackend : BaseDisposable, IBackend
    {
        public Sdl2Window Window { get; }
        public GraphicsDevice Device { get; }
        public SpriteRendererCommon SpriteRendererCommon { get; }
        public VideoTextureSet VideoTextureSet { get; }
        public WorldRendererSet WorldRendererSet { get; }
        public ResourceFactory Factory => Device.ResourceFactory;

        public VeldridBackend(Sdl2Window window, GraphicsDevice device)
        {
            this.Window = window;
            this.Device = device;
            SpriteRendererCommon = new SpriteRendererCommon(device);
            VideoTextureSet = new VideoTextureSet(device);
            WorldRendererSet = new WorldRendererSet(device);

            window.MouseMove += HandleMouseMove;
            window.MouseDown += HandleMouseDown;
        }

        protected override void DisposeManaged()
        {
            SpriteRendererCommon.Dispose();
            VideoTextureSet.Dispose();
        }

        public void Update(float timeDelta)
        {
            VideoTextureSet.Update(timeDelta);
        }

        public void Render()
        {
            WorldRendererSet.RenderAll();
        }

        public InputSnapshot? CurrentInput { get; set; }
        public string? AssetPath { get; set; }

        public Vector2 CursorPosition
        {
            get => CurrentInput?.MousePosition ?? Vector2.Zero;
            set => Window.SetMousePosition(value);
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
                OnViewDrag(Window.MouseDelta);
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
            new AuraTexture(ImageLoader.LoadImage(stream, Device), ownsTexture: true);

        public IVideoTexture CreateVideo(Stream stream) =>
            VideoTextureSet.CreateFromStream(stream);

        public IPanoramaWorldRenderer CreatePanorama(Stream stream, int spriteCapacity)
        {
            var worldRenderer = new PanoramaWorldRenderer(spriteCapacity, SpriteRendererCommon, Device.SwapchainFramebuffer);
            worldRenderer.WorldTexture = ImageLoader.LoadCubemap(stream, Device);
            worldRenderer.WorldRendererSet = WorldRendererSet;
            return worldRenderer;
        }
    }
}
