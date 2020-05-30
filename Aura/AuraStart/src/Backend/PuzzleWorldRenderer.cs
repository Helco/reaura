using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class PuzzleWorldRenderer : BaseDisposable, IVeldridWorldRenderer, IPuzzleWorldRenderer
    {
        private Viewport viewport;
        private GraphicsDevice device;
        private WorldRendererSet? worldRendererSet = null;
        private SpriteRenderer spriteRenderer;
        private WorldSprite[] sprites;
        private Texture texture;
        private DeviceBuffer vertexBuffer;
        private ResourceLayout resourceLayout;
        private ResourceSet resourceSet;
        private Shader[] shaders;
        private Pipeline pipeline;
        private Framebuffer framebuffer;

        public Vector2 ViewportOffset
        {
            get => new Vector2(viewport.X, viewport.Y);
            set
            {
                viewport.X = value.X;
                viewport.Y = value.Y;
            }
        }
        public Vector2 ViewportSize
        {
            get => new Vector2(viewport.Width, viewport.Height);
            set
            {
                viewport.Width = value.X;
                viewport.Height = value.Y;
            }
        }
        public bool IsActive { get; set; } = true;
        public int Order { get; set; } = 0;
        public Matrix4x4 ProjectionMatrix => Matrix4x4.Identity;
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity;
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

        public IReadOnlyList<IWorldSprite> Sprites => sprites;

        public PuzzleWorldRenderer(int spriteCapacity, SpriteRendererCommon spriteRendererCommon, Framebuffer framebuffer, uint worldWidth = 800, uint worldHeight = 500)
        {
            viewport = new Viewport(0.0f, 0.0f, framebuffer.Width, framebuffer.Height, -10.0f, 10.0f);
            device = spriteRendererCommon.Device;
            this.framebuffer = framebuffer;
            var factory = device.ResourceFactory;
            texture = factory.CreateTexture(new TextureDescription(
                width: worldWidth,
                height: worldHeight,
                depth: 1,
                mipLevels: 1,
                arrayLayers: 1,
                format: PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled | TextureUsage.RenderTarget,
                TextureType.Texture2D));
            spriteRenderer = new SpriteRenderer(spriteRendererCommon, spriteCapacity, texture, CubeFace.Front);
            sprites = Enumerable
                .Range(0, spriteCapacity)
                .Select(i => new WorldSprite(new SpriteRenderer[] { spriteRenderer }, i))
                .ToArray();
            vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * 2 * sizeof(float), BufferUsage.VertexBuffer));
            device.UpdateBuffer(vertexBuffer, 0, new[] { new Vector2(-1, -1), new Vector2(+1, -1), new Vector2(-1, +1), new Vector2(+1, +1) });
            resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("MainTextureSampler", ResourceKind.Sampler, ShaderStages.Fragment)));
            resourceSet = factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, texture, device.LinearSampler));
            shaders = factory.LoadShadersFromFiles("Blit");
            var vertexLayout = new VertexLayoutDescription(new VertexElementDescription("Pos", VertexElementFormat.Float2, VertexElementSemantic.Position));
            pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleStrip,
                new ShaderSetDescription(
                    new VertexLayoutDescription[] { vertexLayout },
                    shaders),
                resourceLayout,
                framebuffer.OutputDescription));
        }

        protected override void DisposeManaged()
        {
            WorldRendererSet = null;
            spriteRenderer.WorldTexture?.Dispose();
            spriteRenderer.Dispose();
            foreach (var sprite in sprites)
                sprite.Dispose();
            texture.Dispose();
            vertexBuffer.Dispose();
            resourceLayout.Dispose();
            resourceSet.Dispose();
            foreach (var shader in shaders)
                shader.Dispose();
            pipeline.Dispose();
        }

        public bool ConvertScreenToWorld(Vector2 screenPos, out Vector2 worldPos)
        {
            // TODO: is world resolution always world size?
            worldPos.X = (screenPos.X - viewport.X) / viewport.Width * texture.Width;
            worldPos.Y = (screenPos.Y - viewport.Y) / viewport.Height * texture.Height;
            return (worldPos.X >= 0 && worldPos.Y >= 0 && worldPos.X < texture.Width && worldPos.Y < texture.Height);
        }

        public bool ConvertWorldToScreen(Vector2 worldPos, out Vector2 screenPos)
        {
            // TODO is world resolution always world size?
            screenPos.X = worldPos.X / texture.Width * viewport.Width + viewport.X;
            screenPos.Y = worldPos.Y / texture.Height * viewport.Height + viewport.Y;
            return (worldPos.X >= 0 && worldPos.Y >= 0 && worldPos.X < texture.Width && worldPos.Y < texture.Height);
        }

        public void LoadBackground(Stream stream)
        {
            spriteRenderer.WorldTexture?.Dispose();
            spriteRenderer.WorldTexture = ImageLoader.LoadImage(stream, device);
        }

        public IEnumerable<Fence> RenderPrePasses()
        {
            spriteRenderer.Render();
            return new Fence[] { spriteRenderer.Fence };
        }

        public void RenderMainPass(CommandList commandList)
        {
            commandList.SetFramebuffer(framebuffer);
            commandList.SetViewport(0, viewport);
            commandList.SetPipeline(pipeline);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.Draw(4);
        }

        public void SetViewAt(Vector2 worldPos)
        {
            ConvertWorldToScreen(worldPos, out Vector2 screenPos);
            screenPos.X = Math.Clamp(screenPos.X, viewport.X, viewport.X + viewport.Width);
            screenPos.Y = Math.Clamp(screenPos.Y, viewport.Y, viewport.Y + viewport.Height);
            throw new NotImplementedException();
        }
    }
}
