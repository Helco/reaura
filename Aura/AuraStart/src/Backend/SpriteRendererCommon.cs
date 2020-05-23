using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class SpriteRendererCommon : BaseDisposable
    {
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

        public struct Vertex
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

        public GraphicsDevice Device { get; }
        public ResourceFactory Factory => Device.ResourceFactory;
        public Sampler PointSampler { get; }
        public ResourceLayout ResourceLayout { get; }

        private Shader[] spriteShaders;
        private Dictionary<PixelFormat, Pipeline> pipelines = new Dictionary<PixelFormat, Pipeline>();

        public SpriteRendererCommon(GraphicsDevice device)
        {
            Device = device;
            PointSampler = Factory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                AddressModeW = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinPoint_MagPoint_MipPoint
            });
            ResourceLayout = Factory.CreateResourceLayout(resourceLayoutDescr);
            spriteShaders = Factory.LoadShadersFromFiles("sprite");
        }

        protected override void DisposeManaged()
        {
            PointSampler.Dispose();
            ResourceLayout.Dispose();
            foreach (var shader in spriteShaders)
                shader.Dispose();
            foreach (var pipeline in pipelines.Values)
                pipeline.Dispose();
        }

        public Pipeline GetPipeline(PixelFormat framebufferFormat)
        {
            if (pipelines.TryGetValue(framebufferFormat, out var pipeline))
                return pipeline;

            var pipelineDescr = new GraphicsPipelineDescription(
                blendState: BlendStateDescription.SingleAlphaBlend,
                depthStencilStateDescription: DepthStencilStateDescription.Disabled,
                rasterizerState: RasterizerStateDescription.CullNone,
                primitiveTopology: PrimitiveTopology.TriangleList,
                shaderSet: new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                    shaders: spriteShaders),
                resourceLayout: ResourceLayout,
                outputs: new OutputDescription(
                    depthAttachment: null,
                    new OutputAttachmentDescription(framebufferFormat)));
            pipeline = Factory.CreateGraphicsPipeline(ref pipelineDescr);
            pipelines.Add(framebufferFormat, pipeline);
            return pipeline;
        }
    }
}
