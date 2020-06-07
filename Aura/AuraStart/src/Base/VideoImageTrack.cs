using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veldrid;
using FFmpeg.AutoGen;
using static Aura.Veldrid.FFmpegHelpers;

namespace Aura.Veldrid
{
    public unsafe class VideoImageTrack : VideoTrack
    {
        private GraphicsDevice device;
        private Texture staging;
        private SwsContextPtr? sws;
        private bool isTextureReady = false;

        public Texture Target { get; }
        public int Width => codecContext.Ptr->width;
        public int Height => codecContext.Ptr->height;

        public VideoImageTrack(GraphicsDevice device, VideoPlayer player, AVStream* stream) : base(player, stream)
        {
            this.device = device;
            if (codecContext.Ptr->pix_fmt != AVPixelFormat.AV_PIX_FMT_RGBA)
                sws = new SwsContextPtr(Width, Height, codecContext.Ptr->pix_fmt, Width, Height, AVPixelFormat.AV_PIX_FMT_RGBA, ffmpeg.SWS_POINT);

            var textureDescr = new TextureDescription
            {
                Width = (uint)Width,
                Height = (uint)Height,
                Depth = 1,
                MipLevels = 1,
                ArrayLayers = 1,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled
            };
            Target = device.ResourceFactory.CreateTexture(textureDescr);
            textureDescr.Usage = TextureUsage.Staging;
            staging = device.ResourceFactory.CreateTexture(textureDescr);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();
            staging.Dispose();
            Target.Dispose();
        }

        protected override void DisposeNative()
        {
            if (sws != null)
                sws.Dispose();
            base.DisposeNative();
        }

        protected override double OnGotNextFrame()
        {
            if (nextFrame == null)
                throw new InvalidProgramException("Got next frame but it is null");
            var mappedStaging = device.Map(staging, MapMode.Write);
            var mappedPointer = (byte*)mappedStaging.Data.ToPointer();
            if (sws != null)
            {
                var convertedFrame = new AVFramePtr();
                byte_ptrArray4 dataPointers;
                int_array4 lineSizes;
                Check(ffmpeg.av_image_fill_arrays(ref dataPointers, ref lineSizes, mappedPointer, AVPixelFormat.AV_PIX_FMT_RGBA, Width, Height, 1));
                convertedFrame.Ptr->data.UpdateFrom(dataPointers);
                convertedFrame.Ptr->linesize.UpdateFrom(lineSizes);
                sws.Scale(nextFrame, convertedFrame);
                convertedFrame.Dispose();
            }
            else
                Buffer.MemoryCopy(nextFrame.Ptr->data[0], mappedPointer, mappedStaging.SizeInBytes, Width * Height * 4);
            device.Unmap(staging);

            if (nextFrame.Ptr->best_effort_timestamp != ffmpeg.AV_NOPTS_VALUE)
                return ConvertTimestamp(nextFrame.Ptr->best_effort_timestamp);
            else if (nextFrame.Ptr->pts != ffmpeg.AV_NOPTS_VALUE)
                return ConvertTimestamp(nextFrame.Ptr->pts);
            else
                throw new NotSupportedException("Can't figure out the pts of this video frame");
        }

        protected override void OnSwitchedFrames()
        {
            isTextureReady = true;
        }

        public void Render(CommandList commandList)
        {
            if (isTextureReady)
            {
                isTextureReady = false;
                commandList.CopyTexture(staging, Target);
            }
        }
    }
}
