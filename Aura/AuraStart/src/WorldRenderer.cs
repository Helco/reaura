using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using static Aura.EnumerableExtensions;

namespace Aura.Veldrid
{
    public class WorldRenderer : BaseDisposable
    {
        private const int FaceCount = 6;

        private readonly ResourceLayoutDescription resourceLayoutDescr = new ResourceLayoutDescription
        {
            Elements = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("MainTextureSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("UniformBlock", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment)
            }
        };
        private readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription
        {
            Stride = Vertex.SizeInBytes,
            Elements = new VertexElementDescription[]
            {
                new VertexElementDescription("Pos", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("UV", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
            }
        };

        private struct Vertex
        {
            public Vector2 pos;
            public Vector2 uv;
            public const int SizeInBytes = (2 + 2) * sizeof(float);

            public Vertex(Vector2 p, Vector2 u)
            {
                pos = p;
                uv = u;
            }
        }

        private class Sprite : BaseDisposable
        {
            public ResourceSet? ResourceSet = null;
            public Texture? Texture = null;
            public CubeFace Face;
            public bool OwnsTexture = false;
            public bool IsEnabled = false;

            protected override void DisposeManaged()
            {
                if (ResourceSet != null)
                    ResourceSet.Dispose();
                if (Texture != null && OwnsTexture)
                    Texture.Dispose();
            }
        }

        private GraphicsDevice device;
        private ResourceFactory factory => device.ResourceFactory;
        private CommandList[] faceLists = new CommandList[FaceCount];
        private Fence[] fences = new Fence[FaceCount];
        private Framebuffer[] framebuffers = new Framebuffer[FaceCount];
        private bool[] isFaceDirty = new bool[FaceCount];
        private Shader[] spriteShaders;
        private Sprite[] sprites = new Sprite[0];
        private ResourceLayout resourceLayout;
        private QuadIndexBuffer indexBuffer = default!;
        private DeviceBuffer uniformBuffer;
        private Matrix4x4 projectionMatrix;
        private Sampler pointSampler;
        private Pipeline spritePipeline;
        private DeviceBuffer vertexBuffer = default!;
        private Vertex[] vertices = new Vertex[0];
        private bool needVertexUpdate = true;
        private Texture? worldTexture = null;

        public int SpriteCapacity => indexBuffer.QuadCapacity;

        public Texture Target { get; }

        public Texture? WorldTexture
        {
            get => worldTexture;
            set
            {
                if (worldTexture != null)
                    worldTexture.Dispose();
                worldTexture = value;
                isFaceDirty = Enumerable.Repeat(true, FaceCount).ToArray();
            }
        }

        public WorldRenderer(GraphicsDevice graphicsDevice, int spriteCapacity, uint worldResolution = 1024)
        {
            device = graphicsDevice;

            Target = factory.CreateTexture(new TextureDescription
            {
                Width = worldResolution,
                Height = worldResolution,
                Depth = 1,
                MipLevels = 1,
                ArrayLayers = (uint)FaceCount,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled | TextureUsage.RenderTarget
            });
            for (int i = 0; i < FaceCount; i++)
            {
                faceLists[i] = factory.CreateCommandList();
                fences[i] = factory.CreateFence(true);
                framebuffers[i] = factory.CreateFramebuffer(new FramebufferDescription
                {
                    ColorTargets = new FramebufferAttachmentDescription[]
                    {
                        new FramebufferAttachmentDescription(Target, (uint)i, 0)
                    }
                });
            }

            pointSampler = factory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                AddressModeW = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinPoint_MagPoint_MipPoint
            });
            uniformBuffer = factory.CreateBuffer(
                new BufferDescription(4 * 4 * sizeof(float), BufferUsage.UniformBuffer));
            projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0.0f, worldResolution, worldResolution, 0.0f, 0.1f, 10.0f);
            device.UpdateBuffer(uniformBuffer, 0, projectionMatrix);
            resourceLayout = factory.CreateResourceLayout(resourceLayoutDescr);
            spriteShaders = factory.LoadShadersFromFiles("sprite");
            spritePipeline = CreateSpritePipeline();

            Reset(spriteCapacity);
        }

        protected override void DisposeManaged()
        {
            for (int i = 0; i < FaceCount; i++)
            {
                faceLists[i].Dispose();
                fences[i].Dispose();
                framebuffers[i].Dispose();
            }
            foreach (var shader in spriteShaders)
                shader.Dispose();
            foreach (var sprite in sprites)
                sprite.Dispose();
            resourceLayout.Dispose();
            indexBuffer.Dispose();
            uniformBuffer.Dispose();
            pointSampler.Dispose();
            spritePipeline.Dispose();
            vertexBuffer.Dispose();
            WorldTexture = null; // disposes also worldResourceSet
        }

        private Pipeline CreateSpritePipeline()
        {
            var pipelineDescr = new GraphicsPipelineDescription(
                blendState: BlendStateDescription.SingleAlphaBlend,
                depthStencilStateDescription: DepthStencilStateDescription.Disabled,
                rasterizerState: RasterizerStateDescription.CullNone,
                primitiveTopology: PrimitiveTopology.TriangleList,
                shaderSet: new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                    shaders: spriteShaders),
                resourceLayout: resourceLayout,
                outputs: framebuffers[0].OutputDescription);
            return factory.CreateGraphicsPipeline(ref pipelineDescr);
        }

        public void Reset(int newSpriteCapacity = -1)
        {
            foreach (var oldSprite in sprites)
                oldSprite.Dispose();

            if (newSpriteCapacity < 0)
                newSpriteCapacity = SpriteCapacity;
            sprites = Generate(newSpriteCapacity, _ => new Sprite()).ToArray();
            if (indexBuffer == null)
                indexBuffer = new QuadIndexBuffer(device, newSpriteCapacity);
            else
                indexBuffer.QuadCapacity = newSpriteCapacity;
            if (vertexBuffer != null)
                vertexBuffer.Dispose();
            vertexBuffer = factory.CreateBuffer(new BufferDescription(
                sizeInBytes: (uint)(newSpriteCapacity * 4 * Vertex.SizeInBytes),
                usage: BufferUsage.VertexBuffer));
            vertices = new Vertex[newSpriteCapacity * 4];
        }

        public void SetSprite(int i, Texture texture, CubeFace face, Vector2 upperLeft, bool ownsTexture, bool isEnabled = false)
        {
            if (i < 0 || i >= SpriteCapacity)
                throw new ArgumentOutOfRangeException(nameof(i));
            isFaceDirty[(int)sprites[i].Face] = true;
            isFaceDirty[(int)face] = true;
            sprites[i].Dispose();
            sprites[i] = new Sprite()
            {
                Face = face,
                Texture = texture,
                OwnsTexture = ownsTexture,
                IsEnabled = isEnabled,
                ResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                    resourceLayout, texture, pointSampler, uniformBuffer))
            };
            Vector2 right = Vector2.UnitX * texture.Width;
            Vector2 down = Vector2.UnitY * texture.Height;
            vertices[i * 4 + 0] = new Vertex(upperLeft, new Vector2(0.0f, 0.0f));
            vertices[i * 4 + 1] = new Vertex(upperLeft + right, new Vector2(1.0f, 0.0f));
            vertices[i * 4 + 2] = new Vertex(upperLeft + down, new Vector2(0.0f, 1.0f));
            vertices[i * 4 + 3] = new Vertex(upperLeft + right + down, new Vector2(1.0f, 1.0f));
            needVertexUpdate = true;
        }

        public void ToggleSprite(int i, bool isEnabled)
        {
            if (i < 0 || i >= SpriteCapacity)
                throw new ArgumentOutOfRangeException(nameof(i));
            sprites[i].IsEnabled = isEnabled;
            isFaceDirty[(int)sprites[i].Face] = true;
        }

        public void MarkSpriteDirty(int i)
        {
            if (i < 0 || i >= SpriteCapacity)
                throw new ArgumentOutOfRangeException(nameof(i));
            isFaceDirty[(int)sprites[i].Face] = true;
        }

        public void Render(bool waitUntilFinished = true)
        {
            if (worldTexture == null)
                return;
            if (needVertexUpdate)
            {
                needVertexUpdate = false;
                device.UpdateBuffer(vertexBuffer, 0, vertices);
            }

            List<Fence> fencesToWaitFor = new List<Fence>();
            for (int faceI = 0; faceI < FaceCount; faceI++)
            {
                if (!isFaceDirty[faceI])
                    continue;
                isFaceDirty[faceI] = false;
                fencesToWaitFor.Add(fences[faceI]);
                fences[faceI].Reset();
                faceLists[faceI].Begin();
                faceLists[faceI].CopyTexture(
                    worldTexture, 0, 0, 0, 0, (uint)faceI,
                    Target, 0, 0, 0, 0, (uint)faceI,
                    Target.Width, Target.Height, 1, 1);
                faceLists[faceI].SetFramebuffer(framebuffers[faceI]);
                faceLists[faceI].SetFullViewport(0);
                faceLists[faceI].SetPipeline(spritePipeline);
                faceLists[faceI].SetVertexBuffer(0, vertexBuffer);
                faceLists[faceI].SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
                
                for (int spriteI = 0; spriteI < sprites.Length; spriteI++)
                {
                    if (!sprites[spriteI].IsEnabled || sprites[spriteI].Face != (CubeFace)faceI)
                        continue;
                    faceLists[faceI].SetGraphicsResourceSet(0, sprites[spriteI].ResourceSet);
                    faceLists[faceI].DrawIndexed(
                        indexCount: 6,
                        indexStart: (uint)(spriteI * 6),
                        instanceCount: 1,
                        vertexOffset: 0,
                        instanceStart: 0);
                }

                faceLists[faceI].End();
                device.SubmitCommands(faceLists[faceI], fences[faceI]);
            }

            if (waitUntilFinished && fencesToWaitFor.Any())
                device.WaitForFences(fencesToWaitFor.ToArray(), waitAll: true, 1000000UL);
        }
    }
}
