using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class PanoramaWorldRenderer : BaseDisposable, IPanoramaWorldRenderer, IVeldridWorldRenderer
    {
        private const uint FaceCount = 6;

        private GraphicsDevice device;
        private CubemapPanorama panorama;
        private Texture cubemap;
        private SpriteRenderer[] spriteRenderers = new SpriteRenderer[FaceCount];
        private WorldSprite[] sprites;
        private WorldRendererSet? worldRendererSet = null;
        private Texture? worldTexture = null;

        public WorldRendererSet? WorldRendererSet
        {
            get => worldRendererSet;
            set
            {
                worldRendererSet?.Remove(this);
                worldRendererSet = value;
                worldRendererSet?.Add(this);
            }
        }

        public Texture? WorldTexture
        {
            get => worldTexture;
            set
            {
                worldTexture = value;
                foreach (var spriteRenderer in spriteRenderers)
                    spriteRenderer.WorldTexture = value;
            }
        }

        public PanoramaWorldRenderer(int spriteCapacity, SpriteRendererCommon common, Framebuffer framebuffer, uint worldResolution = 1024)
        {
            device = common.Device;
            cubemap = common.Factory.CreateTexture(new TextureDescription
            {
                Width = worldResolution,
                Height = worldResolution,
                Depth = 1,
                MipLevels = 1,
                ArrayLayers = FaceCount,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled | TextureUsage.RenderTarget
            });
            panorama = new CubemapPanorama(device, framebuffer);
            panorama.Texture = cubemap;
            for (uint i = 0; i < FaceCount; i++)
                spriteRenderers[i] = new SpriteRenderer(common, spriteCapacity, cubemap, (CubeFace)i);
            sprites = new WorldSprite[spriteCapacity];
            for (int i = 0; i < spriteCapacity; i++)
                sprites[i] = new WorldSprite(spriteRenderers, i);
        }

        protected override void DisposeManaged()
        {
            panorama.Dispose();
            cubemap.Dispose();
            foreach (var spriteRenderer in spriteRenderers)
                spriteRenderer.Dispose();
            foreach (var sprite in sprites)
                sprite.Dispose();
            worldRendererSet?.Remove(this);
            worldTexture?.Dispose();
        }

        public IEnumerable<Fence> RenderPrePasses()
        {
            foreach (var spriteRenderer in spriteRenderers)
                spriteRenderer.Render();
            return spriteRenderers.Select(r => r.Fence);
        }

        public void RenderMainPass(CommandList commandList) => panorama.Render(commandList);

        public Vector2 ViewRotation
        {
            get => panorama.ViewRotation;
            set => panorama.ViewRotation = value;
        }

        public Vector2 ViewportOffset
        {
            get => new Vector2(panorama.Viewport.X, panorama.Viewport.Y);
            set => panorama.SetViewport(value.X, value.Y, panorama.Viewport.Width, panorama.Viewport.Height);
        }

        public Vector2 ViewportSize
        {
            get => new Vector2(panorama.Viewport.Width, panorama.Viewport.Height);
            set => panorama.SetViewport(panorama.Viewport.X, panorama.Viewport.Y, value.X, value.Y);
        }

        public bool IsActive { get; set; } = true;
        public int Order { get; set; } = 0;

        public IReadOnlyList<IWorldSprite> Sprites => sprites;

        public bool ConvertScreenToWorld(Vector2 screenPos, out Vector2 worldPos) => panorama.ConvertMouseToAura(screenPos, out worldPos);
        public bool ConvertWorldToScreen(Vector2 worldPos, out Vector2 screenPos) => throw new NotImplementedException();
        public void SetViewAt(Vector2 worldPos) => panorama.SetViewAt(worldPos);
        public Matrix4x4 ProjectionMatrix
        {
            get
            {
                Matrix4x4 result;
                Matrix4x4.Invert(panorama.InvProjectionMatrix, out result);
                return result;
            }
        }
        public Matrix4x4 ViewMatrix
        {
            get
            {
                Matrix4x4 result;
                Matrix4x4.Invert(panorama.InvViewMatrix, out result);
                return result;
            }
        }
    }
}
