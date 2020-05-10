using System;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class SpriteRenderContext : BaseDisposable
    {
        public const int InitialQuadCapacity = 20;
        private const string ShaderName = "sprite";

        private static readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementFormat.Float3, VertexElementSemantic.Position),
            new VertexElementDescription("UV", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
            new VertexElementDescription("Color", VertexElementFormat.Byte4_Norm, VertexElementSemantic.Color));
        private static readonly ResourceLayoutDescription resourceLayoutDescr = new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("MainTextureSampler", ResourceKind.Sampler, ShaderStages.Fragment),
            new ResourceLayoutElementDescription("UniformBlock", ResourceKind.UniformBuffer, ShaderStages.Vertex));

        private Shader[] shaders;
        private Sampler pointSampler;
        private ResourceLayout resourceLayout;

        public GraphicsDevice GraphicsDevice { get; }
        public ResourceFactory ResourceFactory => GraphicsDevice.ResourceFactory;
        public QuadIndexBuffer IndexBuffer { get; }
        public Pipeline Pipeline { get; }

        public struct Uniforms
        {
            public Matrix4x4 projection;
            public Matrix4x4 view;
            public const int SizeInBytes = 2 * 16 * sizeof(float);
        }

        public struct Vertex
        {
            public Vector3 pos;
            public Vector2 uv;
            public RgbaByte color;
            public const int SizeInBytes = (3 + 2) * sizeof(float) + 4;
        }

        public SpriteRenderContext(GraphicsDevice graphicsDevice, OutputDescription outputDescription)
        {
            GraphicsDevice = graphicsDevice;
            shaders = ResourceFactory.LoadShadersFromFiles(ShaderName);
            pointSampler = ResourceFactory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                AddressModeW = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinPoint_MagPoint_MipPoint
            });
            resourceLayout = ResourceFactory.CreateResourceLayout(resourceLayoutDescr);
            IndexBuffer = new QuadIndexBuffer(graphicsDevice, InitialQuadCapacity);

            var pipelineDescr = new GraphicsPipelineDescription(
                blendState: BlendStateDescription.SingleAlphaBlend,
                depthStencilStateDescription: DepthStencilStateDescription.Disabled,
                rasterizerState: RasterizerStateDescription.Default,
                primitiveTopology: PrimitiveTopology.TriangleList,
                shaderSet: new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                    shaders: shaders),
                resourceLayout: resourceLayout,
                outputs: outputDescription);
            Pipeline = ResourceFactory.CreateGraphicsPipeline(ref pipelineDescr);
        }

        protected override void DisposeManaged()
        {
            foreach (var shader in shaders)
                shader.Dispose();
            resourceLayout.Dispose();
            pointSampler.Dispose();
            IndexBuffer.Dispose();
            Pipeline.Dispose();
        }

        public ResourceSet CreateResourceSet(Texture texture, DeviceBuffer uniformBuffer)
        {
            return ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                resourceLayout,
                texture,
                pointSampler,
                uniformBuffer));
        }

        public DeviceBuffer CreateUniformBuffer() =>
            ResourceFactory.CreateBuffer(new BufferDescription(Uniforms.SizeInBytes, BufferUsage.UniformBuffer));

        public DynamicDeviceBuffer<Vertex> CreateVertexBuffer() =>
            new DynamicDeviceBuffer<Vertex>(GraphicsDevice, BufferUsage.VertexBuffer, InitialQuadCapacity * 4);
    }
}
