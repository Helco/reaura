using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Veldrid;

namespace Aura.Veldrid
{
    public class DynamicDeviceBuffer<T> : BaseDisposable where T : struct
    {
        public static readonly uint Stride = (uint)Marshal.SizeOf<T>();

        private GraphicsDevice graphicsDevice;
        private BufferUsage usage;
        private DeviceBuffer? previous = null;
        private int previousCount = 0;
        private T[] localStorage = default!; // will be set by Capacity

        private bool IsStructured =>
            usage == BufferUsage.StructuredBufferReadOnly ||
            usage == BufferUsage.StructuredBufferReadWrite;

        public DeviceBuffer Current { get; private set; } = default!; // will be set by Capacity
        public int Count { get; private set; } = 0;
        public bool IsPrepared { get; private set; } = true;
        public int Capacity
        {
            get => (int)(Current.SizeInBytes / Stride);
            set
            {
                if (Current != null && Capacity >= value)
                    return;
                if (Count > 0)
                {
                    if (previous != null)
                        throw new NotSupportedException("DynamicDeviceBuffer does not support two capacity changes in one update");
                    previous = Current;
                    previousCount = Count;
                }
                else if (Current != null)
                    Current.Dispose();

                Current = graphicsDevice.ResourceFactory.CreateBuffer(
                    new BufferDescription((uint)(value * Stride), usage));
                localStorage = new T[value];
                OnBufferChanged(Current);
            }
        }
        public float Growth { get; set; } = 2.0f;

        public event Action<DeviceBuffer> OnBufferChanged = _ => { };

        public DynamicDeviceBuffer(GraphicsDevice graphicsDevice, BufferUsage usage, int initialCapacity)
        {
            this.graphicsDevice = graphicsDevice;
            this.usage = usage;
            Capacity = initialCapacity;
        }

        protected override void DisposeManaged()
        {
            Current.Dispose();
            if (previous != null)
                previous.Dispose();
        }

        public static implicit operator DeviceBuffer(DynamicDeviceBuffer<T> dynBuffer) => dynBuffer.Current;

        public void Prepare(CommandList list)
        {
            IsPrepared = true;
            uint previousSize = (uint)previousCount * Stride;
            if (Count > 0 && Count > previousCount)
                list.UpdateBuffer(Current, previousSize, ref localStorage[previousCount], (uint)(Count - previousCount) * Stride);
            if (previous != null)
            {
                list.CopyBuffer(previous, 0, Current, 0, previousSize);
                previous = null; // no dispose, rely on GC to free after buffer was fully copied
                previousCount = 0;
            }
        }

        public int Add(params T[] values)
        {
            if (IsPrepared)
            {
                IsPrepared = false;
                Count = 0;
            }
            if (Count + values.Length > Capacity)
                Capacity += values.Length;
            Array.Copy(values, 0, localStorage, Count, values.Length);
            Count += values.Length;
            return Count;
        }
    }
}
