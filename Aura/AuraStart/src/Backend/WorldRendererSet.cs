using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace Aura.Veldrid
{
    public interface IVeldridWorldRenderer : IWorldRenderer
    {
        WorldRendererSet? WorldRendererSet { get; set; }
        IEnumerable<Fence> RenderFirstPass();
        IEnumerable<Fence> RenderSecondPass();
    }

    public class WorldRendererSet : IEnumerable<IVeldridWorldRenderer>
    {
        private GraphicsDevice device;
        private ISet<IVeldridWorldRenderer> renderers = new HashSet<IVeldridWorldRenderer>();

        public WorldRendererSet(GraphicsDevice device)
        {
            this.device = device;
        }

        public void Add(IVeldridWorldRenderer ren) => renderers.Add(ren);
        public void Remove(IVeldridWorldRenderer ren) => renderers.Remove(ren);

        public void RenderAll()
        {
            var sortedRenderers = renderers
                .Where(r => r.IsActive)
                .OrderBy(r => r.Order)
                .ToArray();

            var fences = sortedRenderers.SelectMany(r => r.RenderFirstPass()).ToArray();
            //device.WaitForFences(fences.Where(f => !f.Signaled).ToArray(), true, TimeSpan.FromSeconds(1));
            device.WaitForIdle();
            fences = sortedRenderers.SelectMany(r => r.RenderSecondPass()).ToArray();
            device.WaitForIdle();
            //device.WaitForFences(fences.Where(f => !f.Signaled).ToArray(), true);
        }

        public IEnumerator<IVeldridWorldRenderer> GetEnumerator() => renderers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => renderers.GetEnumerator();
    }
}
