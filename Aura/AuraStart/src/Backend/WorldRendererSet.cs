using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public interface IVeldridWorldRenderer : IWorldRenderer
    {
        WorldRendererSet? WorldRendererSet { get; set; }
        IEnumerable<Fence> RenderPrePasses();
        void RenderMainPass(CommandList commandList);
        Matrix4x4 ProjectionMatrix { get; }
        Matrix4x4 ViewMatrix { get; }
    }

    public class WorldRendererSet : BaseDisposable, IEnumerable<IVeldridWorldRenderer>
    {
        private GraphicsDevice device;
        private ISet<IVeldridWorldRenderer> renderers = new HashSet<IVeldridWorldRenderer>();
        private CommandList commandList;
        private Fence fence;

        public WorldRendererSet(GraphicsDevice device)
        {
            this.device = device;
            commandList = device.ResourceFactory.CreateCommandList();
            fence = device.ResourceFactory.CreateFence(true);
        }

        protected override void DisposeManaged()
        {
            commandList.Dispose();
            fence.Dispose();
        }

        public void Add(IVeldridWorldRenderer ren) => renderers.Add(ren);
        public void Remove(IVeldridWorldRenderer ren) => renderers.Remove(ren);

        public void RenderAll()
        {
            var sortedRenderers = renderers
                .Where(r => r.IsActive)
                .OrderBy(r => r.Order)
                .ToArray();

            var fences = sortedRenderers.SelectMany(r => r.RenderPrePasses()).ToArray();
            device.WaitForFences(fences.Where(f => !f.Signaled).ToArray(), true);
            fence.Reset();
            commandList.Begin();
            foreach (var ren in sortedRenderers)
                ren.RenderMainPass(commandList);
            commandList.End();
            device.SubmitCommands(commandList, fence);
            device.WaitForFence(fence);
        }

        public IEnumerator<IVeldridWorldRenderer> GetEnumerator() => renderers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => renderers.GetEnumerator();
    }
}
