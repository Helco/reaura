using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class SpriteRenderer : BaseDisposable
    {
        private Framebuffer framebuffer;
        private DeviceBuffer uniformBuffer;
        private DynamicDeviceBuffer<SpriteRenderContext.Vertex> vertexBuffer;
        private SpriteRenderContext.Uniforms uniforms;
        private Dictionary<Texture, ResourceSet> resourceSets = new Dictionary<Texture, ResourceSet>();

        public Texture Target { get; }
        public CubeFace Face { get; }
        public SpriteRenderContext Context { get; }
        public bool WasFinished { get; } = true;

        public SpriteRenderer(Texture target, CubeFace face, SpriteRenderContext context)
        {
            Context = context;
            Target = target;
            Face = face;

            framebuffer = Context.ResourceFactory.CreateFramebuffer(new FramebufferDescription
            {
                ColorTargets = new FramebufferAttachmentDescription[]
                {
                    new FramebufferAttachmentDescription(target, (uint)face, 0)
                }
            });
            vertexBuffer = Context.CreateVertexBuffer();
            uniformBuffer = Context.CreateUniformBuffer();
            uniforms.projection = Matrix4x4.CreateOrthographicOffCenter(0.0f, target.Width, target.Height, 0.0f, 0.01f, 10.0f);
            uniforms.view = Matrix4x4.Identity;
            Context.GraphicsDevice.UpdateBuffer(uniformBuffer, 0, uniforms);
        }

        protected override void DisposeManaged()
        {
            Reset();
            framebuffer.Dispose();
            uniformBuffer.Dispose();
            vertexBuffer.Dispose();
        }

        public void Reset()
        {
            foreach (var set in resourceSets.Values)
                set.Dispose();
            resourceSets.Clear();
        }

        public void AddSprite(Texture texture, Vector2 upperLeft, Vector2 size)
        {

        }
    }
}
