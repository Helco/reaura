using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Aura.Systems;
using Veldrid;

namespace Aura.Veldrid
{
    public class DebugCellSystem : BaseDisposable, IDebugGameSystem, IVeldridWorldRenderer
    {
        public bool IsActive { get; set; } = true;
        public int Order { get; set; } = 10000;

        private readonly ResourceLayoutDescription resourceLayoutDescr = new ResourceLayoutDescription
        {
            Elements = new ResourceLayoutElementDescription[]
            {
                new ResourceLayoutElementDescription("UniformBlock", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment)
            }
        };

        private readonly VertexLayoutDescription vertexLayout = new VertexLayoutDescription
        {
            Stride = Vertex.SizeInBytes,
            Elements = new VertexElementDescription[]
            {
                new VertexElementDescription("Pos", VertexElementFormat.Float3, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("UV", VertexElementFormat.Float2, VertexElementSemantic.TextureCoordinate),
                new VertexElementDescription("CellIndex", VertexElementFormat.Int1, VertexElementSemantic.TextureCoordinate)
            }
        };

        private struct Vertex
        {
            public Vector3 pos;
            public Vector2 uv;
            public int cellIndex;
            public const uint SizeInBytes = (3 + 2) * sizeof(float) + sizeof(int);
        }

        private struct Uniforms
        {
            public Matrix4x4 projection;
            public Matrix4x4 view;
            public RgbaFloat color;
            public RgbaFloat selectedColor;
            public float borderWidth;
            public float borderAlpha;
            public int selected;

            public const uint SizeInBytes = (4 * 4 * 2 + 4 * 2 + 2) * sizeof(float) + sizeof(int);
            public const uint AlignedSizeInBytes = (SizeInBytes + 15) / 16 * 16;
        }

        private VeldridBackend backend;
        private GameWorldRendererSystem? worldRendererSystem;
        private CellSystem? cellSystem;
        private DeviceBuffer? vertexBuffer;
        private DeviceBuffer? indexBuffer;
        private DeviceBuffer uniformBuffer;
        private ResourceLayout resourceLayout;
        private ResourceSet resourceSet;
        private Shader[] shaders;
        private Pipeline pipeline;
        private Uniforms uniforms;
        private Viewport viewport;
        private int indexCount = 0;

        private bool ReadyToRender =>
            worldRendererSystem != null &&
            cellSystem != null &&
            vertexBuffer != null &&
            indexBuffer != null &&
            indexCount > 0;

        public DebugCellSystem(VeldridBackend backend)
        {
            this.backend = backend;
            resourceLayout = backend.Factory.CreateResourceLayout(resourceLayoutDescr);
            uniformBuffer = backend.Factory.CreateBuffer(new BufferDescription(Uniforms.AlignedSizeInBytes, BufferUsage.UniformBuffer));
            resourceSet = backend.Factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, uniformBuffer));
            shaders = backend.Factory.LoadShadersFromFiles("debugcell");
            var pipelineDescr = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                DepthStencilStateDescription.Disabled,
                RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(
                    new VertexLayoutDescription[] { vertexLayout },
                    shaders),
                new ResourceLayout[] { resourceLayout },
                backend.Device.SwapchainFramebuffer.OutputDescription);
            pipeline = backend.Factory.CreateGraphicsPipeline(ref pipelineDescr);
            WorldRendererSet = backend.WorldRendererSet;
        }

        protected override void DisposeManaged()
        {
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            uniformBuffer.Dispose();
            resourceLayout.Dispose();
            resourceSet.Dispose();
            foreach (var shader in shaders)
                shader.Dispose();
            pipeline.Dispose();
            WorldRendererSet = null;
        }

        public void CrossInitialize(IGameSystemContainer container)
        {
            worldRendererSystem = container.SystemsWith<GameWorldRendererSystem>().SingleOrDefault();
            cellSystem = container.SystemsWith<CellSystem>().SingleOrDefault();
        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.C)
                IsActive = !IsActive;
        }

        private readonly float sectionSize = AuraMath.MaxAuraAngle.X / 60.0f;
        public void OnAfterSceneChange()
        {
            if (worldRendererSystem == null || cellSystem == null)
                return;
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            var vertices = Enumerable.Empty<Vertex>();
            var indices = Enumerable.Empty<ushort>();
            int cellI = 0;
            
            foreach (var cell in cellSystem.Cells)
            {
                int basebaseI = vertices.Count();
                int sectionsX = (int)Math.Ceiling(cell.Size.X / sectionSize);
                int sectionsY = (int)Math.Ceiling(cell.Size.Y / sectionSize);
                Vector2 right = Vector2.UnitX * (cell.Size.X / sectionsX);
                Vector2 down = Vector2.UnitY * (cell.Size.Y / sectionsY);

                vertices = vertices.Concat(Enumerable
                    .Range(0, sectionsY + 1)
                    .SelectMany(y => Enumerable
                        .Range(0, sectionsX + 1)
                        .Select(x =>
                        {
                            var auraPos = cell.UpperLeft + x * right + y * down;
                            return new Vertex()
                            {
                                pos = AuraMath.AuraOnSphere(auraPos),
                                uv = AuraMath.DistanceToBorder(auraPos, cell.UpperLeft, cell.UpperLeft + cell.Size), // unnormalized lowerright
                                cellIndex = cellI
                            };
                        })
                    ));

                int line = sectionsX + 1;
                indices = indices.Concat(Enumerable
                    .Range(0, sectionsY)
                    .SelectMany(y => Enumerable
                        .Range(0, sectionsX)
                        .Select(x => y * line + x)
                        .SelectMany(baseI => new[] {
                            0, 1, line,
                            1, line + 1, line
                        }.Select(i => i + baseI +basebaseI))
                    ).Select(i => (ushort)i));

                cellI++;
            }

            var vertexArray = vertices.ToArray();
            var indexArray = indices.ToArray();
            vertexBuffer = backend.Factory.CreateBuffer(new BufferDescription((uint)(vertexArray.Length * Vertex.SizeInBytes), BufferUsage.VertexBuffer));
            backend.Device.UpdateBuffer(vertexBuffer, 0, vertexArray);
            indexBuffer = backend.Factory.CreateBuffer(new BufferDescription((uint)(indexArray.Length * sizeof(ushort)), BufferUsage.IndexBuffer));
            backend.Device.UpdateBuffer(indexBuffer, 0, indexArray);
            indexCount = indexArray.Length;

            uniforms.borderWidth = 1.0f;
            uniforms.borderAlpha = 0.7f;
            uniforms.color = RgbaFloat.Yellow.WithAlpha(0.2f);
            uniforms.selectedColor = RgbaFloat.Red.WithAlpha(0.2f);
            uniforms.selected = -1;
            var wr = (IVeldridWorldRenderer?)worldRendererSystem.WorldRenderer;
            if (wr == null)
                throw new InvalidProgramException("WorldRendererSystem has no renderer initialized after scene change");
            uniforms.projection = wr.ProjectionMatrix;
            uniforms.view = wr.ViewMatrix;
        }

        public IEnumerable<Fence> RenderPrePasses()
        {
            var wr = (worldRendererSystem?.WorldRenderer as IVeldridWorldRenderer);
            if (wr != null)
            {
                uniforms.view = wr.ViewMatrix;
                viewport = new Viewport(
                    wr.ViewportOffset.X, wr.ViewportOffset.Y,
                    wr.ViewportSize.X, wr.ViewportSize.Y,
                    -10.0f, 10.0f);
            }
            return Enumerable.Empty<Fence>();
        }

        public void RenderMainPass(CommandList commandList)
        {
            if (!ReadyToRender)
                return;

            commandList.SetFramebuffer(backend.Device.SwapchainFramebuffer);
            commandList.SetViewport(0, viewport);
            commandList.SetPipeline(pipeline);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.UpdateBuffer(uniformBuffer, 0, ref uniforms);
            commandList.DrawIndexed((uint)indexCount);
        }

        private WorldRendererSet? worldRendererSet;
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

        public Matrix4x4 ProjectionMatrix => uniforms.projection;
        public Matrix4x4 ViewMatrix => uniforms.view;
        public Vector2 ViewportOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Vector2 ViewportSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IReadOnlyList<IWorldSprite> Sprites => throw new NotImplementedException();

        public bool ConvertScreenToWorld(Vector2 screenPos, out Vector2 worldPos) => throw new NotImplementedException();
        public bool ConvertWorldToScreen(Vector2 worldPos, out Vector2 screenPos) => throw new NotImplementedException();
        public void SetViewAt(Vector2 worldPos) => throw new NotImplementedException();
    }
}
