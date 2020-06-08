using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using static Aura.EnumerableExtensions;
using Vertex = Aura.Veldrid.SpriteRendererCommon.Vertex;

namespace Aura.Veldrid
{
    public class SpriteRenderer : BaseDisposable
    {
        private CommandList commandList;
        private Framebuffer framebuffer;
        private ResourceSet?[] spriteResourceSets;
        private QuadIndexBuffer indexBuffer;
        private DeviceBuffer vertexBuffer;
        private Matrix4x4 ProjectionMatrix;
        private Vertex[] vertices = new Vertex[0];
        private bool needVertexUpdate = true;
        private bool needSpriteUpdate = true;
        private bool needUniformUpdate = true;
        private Texture? worldTexture = null;
        private TextureView? worldTextureView = null;

        public SpriteRendererCommon Common { get; }
        public DeviceBuffer UniformBuffer { get; }
        public Fence Fence { get; }
        public int SpriteCapacity => spriteResourceSets.Length - 1;
        public Texture Target { get; }
        public CubeFace TargetFace { get; }

        public Texture? WorldTexture
        {
            get => worldTexture;
            set
            {
                worldTexture = value;
                worldTextureView?.Dispose();
                uint worldWidth = worldTexture?.Width ?? Target.Width;
                uint worldHeight = worldTexture?.Height ?? Target.Height;
                ProjectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0.0f, worldWidth, worldHeight, 0.0f, 0.1f, 10.0f);
                needUniformUpdate = true;

                if (value == null)
                    return;
                worldTextureView = Common.Factory.CreateTextureView(new TextureViewDescription(
                    worldTexture, 0, 1, (uint)TargetFace, 1));
                spriteResourceSets[0]?.Dispose();
                spriteResourceSets[0] = Common.Factory.CreateResourceSet(new ResourceSetDescription(
                    Common.ResourceLayout, worldTextureView, Common.PointSampler, UniformBuffer));
                SetSpriteQuadInternal(0, Vector2.Zero, new Vector2(worldWidth, worldHeight));
            }
        }

        public SpriteRenderer(SpriteRendererCommon common, int spriteCapacity, Texture target, CubeFace targetFace)
        {
            this.Common = common;
            Target = target;
            TargetFace = targetFace;

            commandList = common.Factory.CreateCommandList();
            Fence = common.Factory.CreateFence(true);
            framebuffer = common.Factory.CreateFramebuffer(new FramebufferDescription
            {
                ColorTargets = new FramebufferAttachmentDescription[]
                {
                    new FramebufferAttachmentDescription(Target, (uint)TargetFace)
                }
            });

            spriteCapacity++; // reserve first for world texture
            spriteResourceSets = Enumerable.Repeat<ResourceSet?>(null, spriteCapacity).ToArray();
            indexBuffer = new QuadIndexBuffer(common.Device, spriteCapacity);
            vertexBuffer = common.Factory.CreateBuffer(new BufferDescription(
                sizeInBytes: (uint)(spriteCapacity * 4 * Vertex.SizeInBytes),
                usage: BufferUsage.VertexBuffer));
            vertices = new Vertex[spriteCapacity * 4];
            UniformBuffer = common.Factory.CreateBuffer(
                new BufferDescription(4 * 4 * sizeof(float), BufferUsage.UniformBuffer));
            WorldTexture = null;
        }

        protected override void DisposeManaged()
        {
            commandList.Dispose();
            framebuffer.Dispose();
            indexBuffer.Dispose();
            vertexBuffer.Dispose();
            UniformBuffer.Dispose();
            spriteResourceSets[0]?.Dispose();
            worldTextureView?.Dispose();
        }

        public void SetSpriteQuad(int i, Vector2 upperLeft, Vector2 size)
        {
            if (i < 0 || i >= SpriteCapacity)
                throw new ArgumentOutOfRangeException(nameof(i));
            SetSpriteQuadInternal(i + 1, upperLeft, size); // first sprite is worldTexture
        }

        private void SetSpriteQuadInternal(int i, Vector2 upperLeft, Vector2 size)
        {
            Vector2 right = Vector2.UnitX * size.X;
            Vector2 down = Vector2.UnitY * size.Y;
            vertices[i * 4 + 0] = new Vertex(upperLeft, new Vector2(0.0f, 0.0f));
            vertices[i * 4 + 1] = new Vertex(upperLeft + right, new Vector2(1.0f, 0.0f));
            vertices[i * 4 + 2] = new Vertex(upperLeft + down, new Vector2(0.0f, 1.0f));
            vertices[i * 4 + 3] = new Vertex(upperLeft + right + down, new Vector2(1.0f, 1.0f));
            needVertexUpdate = needSpriteUpdate = true;
        }

        public void SetSpriteResourceSet(int i, ResourceSet? resourceSet)
        {
            if (i < 0 || i >= SpriteCapacity)
                throw new ArgumentOutOfRangeException(nameof(i));
            i++; // first sprite is worldTexture
            spriteResourceSets[i] = resourceSet;
            needSpriteUpdate = true;
        }

        public void MarkDirty() => needSpriteUpdate = true;

        public void Render()
        {
            if (!needSpriteUpdate)
                return;
            needSpriteUpdate = false;

            Fence.Reset();
            commandList.Begin();
            if (needVertexUpdate)
            {
                needVertexUpdate = false;
                commandList.UpdateBuffer(vertexBuffer, 0, vertices);
            }
            if (needUniformUpdate)
            {
                needUniformUpdate = false;
                commandList.UpdateBuffer(UniformBuffer, 0, ProjectionMatrix);
            }

            commandList.SetFramebuffer(framebuffer);
            commandList.SetFullViewport(0);
            commandList.SetPipeline(Common.GetPipeline(Target.Format));
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            if (worldTexture == null)
            {
                commandList.ClearColorTarget(0, RgbaFloat.Clear);
            }

            for (int spriteI = 0; spriteI <= SpriteCapacity; spriteI++)
            {
                var resourceSet = spriteResourceSets[spriteI];
                if (resourceSet == null)
                    continue;
                commandList.SetGraphicsResourceSet(0, resourceSet);
                commandList.DrawIndexed(
                    indexCount: 6,
                    indexStart: (uint)(spriteI * 6),
                    instanceCount: 1,
                    vertexOffset: 0,
                    instanceStart: 0);
            }
            commandList.End();
            Common.Device.SubmitCommands(commandList, Fence);
        }
    }
}
