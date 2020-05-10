using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace Aura.Veldrid
{
    public class QuadIndexBuffer : BaseDisposable
    {
        private readonly ushort[] IndexPattern = new ushort[] { 0, 1, 2, 1, 3, 2 };

        private GraphicsDevice graphicsDevice;
        private ResourceFactory ResourceFactory => graphicsDevice.ResourceFactory;

        public DeviceBuffer Buffer { get; private set; } = default!; // will be set by IndexQuadCapacity
        public int QuadCapacity
        {
            get => (int)(Buffer.SizeInBytes / (2 * 6));
            set
            {
                if (Buffer != null && QuadCapacity >= value)
                    return;
                if (Buffer != null)
                    Buffer.Dispose();
                Buffer = ResourceFactory.CreateBuffer(new BufferDescription((uint)(value * 2), BufferUsage.IndexBuffer));
                ushort[]? indices = Enumerable
                    .Repeat(0, value * 6)
                    .Select((_, i) => (ushort)(IndexPattern[i % 6] + i / 6 * 4))
                    .ToArray();
                graphicsDevice.UpdateBuffer(Buffer, 0, indices);
                indices = null;
                OnBufferChanged(Buffer);
            }
        }

        public event Action<DeviceBuffer> OnBufferChanged = _ => { };

        public QuadIndexBuffer(GraphicsDevice gd, int initialCapacity)
        {
            graphicsDevice = gd;
            QuadCapacity = initialCapacity;
        }

        protected override void DisposeManaged()
        {
            Buffer.Dispose();
        }
        
        public static implicit operator DeviceBuffer (QuadIndexBuffer qiBuffer) => qiBuffer.Buffer;
    }
}
