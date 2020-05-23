using System;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class CubemapPanorama : BaseDisposable
    {
        public const float VerticalFOV = 0.97056514f;
        public const float OriginalAspect = 1.6f;
        private const string ShaderName = "cubemap";

        private static readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate));
        private static readonly ResourceLayoutDescription resourceLayoutDescr = new ResourceLayoutDescription
        {
            Elements = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription
                {
                    Kind = ResourceKind.TextureReadOnly,
                    Stages = ShaderStages.Fragment,
                    Name = "MainTexture"
                },
                new ResourceLayoutElementDescription
                {
                    Kind = ResourceKind.Sampler,
                    Stages = ShaderStages.Fragment,
                    Name = "MainTextureSampler"
                },
                new ResourceLayoutElementDescription
                {
                    Kind = ResourceKind.UniformBuffer,
                    Stages = ShaderStages.Vertex,
                    Name = "UniformBlock"
                }
            }
        };

        private GraphicsDevice graphicsDevice;
        public DeviceBuffer vertexBuffer;
        public DeviceBuffer indexBuffer;
        public DeviceBuffer uniformBuffer;
        private Shader[] shaders;
        public ResourceSet? resourceSet;
        private Sampler sampler;
        public Pipeline pipeline;
        private ResourceLayout resourceLayout;
        private Matrix4x4[] matrices = new Matrix4x4[2];
        private bool areMatricesDirty = true;
        private Vector2 viewRotation = Vector2.Zero;
        private Texture? texture = null;
        private Framebuffer framebuffer;

        public Matrix4x4 InvProjectionMatrix
        {
            get => matrices[0];
            private set
            {
                matrices[0] = value;
                areMatricesDirty = true;
            }
        }
        public Viewport Viewport { get; private set; }
        public Framebuffer Framebuffer
        {
            get => framebuffer;
            set
            {
                framebuffer = value;
                SetViewport();
            }
        }
        public Texture? Texture
        {
            get => texture;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (texture != null)
                    texture.Dispose();
                texture = value;
                if (resourceSet != null)
                    resourceSet.Dispose();
                resourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(new ResourceSetDescription
                {
                    Layout = resourceLayout,
                    BoundResources = new BindableResource[]
                    {
                        texture,
                        sampler,
                        uniformBuffer
                    }
                });
            }
        }
        public Vector2 ViewRotation
        {
            get => viewRotation;
            set
            {
                viewRotation = value;
                matrices[1] = Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, ViewRotation.X) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, ViewRotation.Y));
                Matrix4x4.Invert(matrices[1], out matrices[1]);
                areMatricesDirty = true;
            }
        }

        public CubemapPanorama(GraphicsDevice gd, Framebuffer fb)
        {
            graphicsDevice = gd;
            framebuffer = fb;
            SetViewport();

            resourceLayout = gd.ResourceFactory.CreateResourceLayout(resourceLayoutDescr);
            shaders = gd.ResourceFactory.LoadShadersFromFiles(ShaderName);
            pipeline = CreatePipeline();

            vertexBuffer = graphicsDevice.CreateBufferFrom(BufferUsage.VertexBuffer,
                new Vector2(-1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-1f, -1f),
                new Vector2(1f, -1f));
            indexBuffer = graphicsDevice.CreateBufferFrom<ushort>(BufferUsage.IndexBuffer, 0, 1, 2, 3);
            uniformBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription(
                usage: BufferUsage.UniformBuffer,
                sizeInBytes: 2 * 4 * 4 * sizeof(float)));
            ViewRotation = Vector2.Zero; // initialise view matrix

            sampler = gd.ResourceFactory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                AddressModeW = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinLinear_MagLinear_MipPoint
            });
        }

        protected override void DisposeManaged()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            uniformBuffer.Dispose();
            foreach (var shader in shaders)
                shader.Dispose();
            if (resourceSet != null)
                resourceSet.Dispose();
            sampler.Dispose();
            pipeline.Dispose();
            resourceLayout.Dispose();
            if (Texture != null)
                Texture.Dispose();
        }

        public void Render(CommandList commandList)
        {
            if (Texture == null || resourceSet == null)
                return;
            commandList.SetFramebuffer(Framebuffer);
            commandList.SetViewport(0, Viewport);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetPipeline(pipeline);
            if (areMatricesDirty)
            {
                areMatricesDirty = false;
                commandList.UpdateBuffer(uniformBuffer, 0, matrices);
            }
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed(
                    indexCount: 4,
                    instanceCount: 1,
                    indexStart: 0,
                    vertexOffset: 0,
                    instanceStart: 0);
        }

        public void SetViewport(bool useOriginalAspect = false) =>
            SetViewport(0, 0, Framebuffer.Width, Framebuffer.Height, useOriginalAspect);

        public void SetViewport(float x, float y, float width, float height, bool useOriginalAspect = false)
        {
            if (useOriginalAspect)
            {
                float newHeight = width / OriginalAspect;
                y += (height - newHeight) / 2;
                height = newHeight;
            }

            Viewport = new Viewport(x, y, width, height, 0.0f, 1.0f);
            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(VerticalFOV, width / height, 0.01f, 10.0f);
            Matrix4x4.Invert(projectionMatrix, out projectionMatrix);
            InvProjectionMatrix = projectionMatrix;
        }

        private Pipeline CreatePipeline()
        {
            var pipelineDescr = new GraphicsPipelineDescription(
                blendState: BlendStateDescription.SingleOverrideBlend,
                depthStencilStateDescription: DepthStencilStateDescription.Disabled,
                rasterizerState: RasterizerStateDescription.Default,
                primitiveTopology: PrimitiveTopology.TriangleStrip,
                shaderSet: new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                    shaders: shaders),
                resourceLayout: resourceLayout,
                outputs: Framebuffer.OutputDescription);
            return graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref pipelineDescr);
        }

        public bool ConvertMouseToAura(Vector2 mouse, out Vector2 worldPos)
        {
            Vector4 clipSpace = new Vector4(
                (mouse.X - Viewport.X - Viewport.Width / 2) / (Viewport.Width / 2),
                -(mouse.Y - Viewport.Y - Viewport.Height / 2) / (Viewport.Height / 2),
                1.0f, 1.0f);
            var cameraSpace = Vector4.Transform(clipSpace, InvProjectionMatrix);
            var viewSpace = Vector4.Transform(cameraSpace, matrices[1]);
            worldPos = AuraMath.SphereToAura(new Vector3(viewSpace.X, viewSpace.Y, -viewSpace.Z));
            return (Math.Abs(clipSpace.X) <= 1 && Math.Abs(clipSpace.Y) <= 1);
        }

        public void SetViewAt(Vector2 auraPos)
        {
            ViewRotation = AuraMath.AuraToAngleRadians(auraPos);
        }
    }
}