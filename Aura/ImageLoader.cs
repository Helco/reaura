using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using FFmpeg.AutoGen;
using Veldrid;
using static Aura.Veldrid.FFmpegHelpers;
using System.Runtime.InteropServices;

namespace Aura.Veldrid
{
    public unsafe class ImageLoader : BaseDisposable
    {
        private AVFormatContextPtr format = new AVFormatContextPtr();
        private StreamAVIOContext avioStream;
        private AVCodecContextPtr codecContext;
        private AVFramePtr frame = new AVFramePtr();
        private AVPacketPtr packet = new AVPacketPtr();
        private int streamIndex;

        private AVFramePtr? convertedFrame = null;
        private SwsContextPtr? sws = null;

        private int Width => codecContext.Ptr->width;
        private int Height => codecContext.Ptr->height;
        private AVPixelFormat AVFormat => codecContext.Ptr->pix_fmt;

        private ImageLoader(Stream stream)
        {
            avioStream = new StreamAVIOContext(stream);
            format.Ptr->pb = avioStream.Context;
            format.OpenInput(null);
            AVCodec* codec = null;
            streamIndex = Check(ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0));
            codecContext = new AVCodecContextPtr(codec);
            ffmpeg.avcodec_parameters_to_context(codecContext, format.Ptr->streams[streamIndex]->codecpar);
            Check(ffmpeg.avcodec_open2(codecContext, codec, null));
        }

        protected override void DisposeNative()
        {
            if (convertedFrame != null)
                convertedFrame.Dispose();
            if (sws != null)
                sws.Dispose();
            frame.Dispose();
            packet.Dispose();
            codecContext.Dispose();
            format.Dispose();
            avioStream.Dispose();
        }

        private bool MoveToNextFrame()
        {
            int errorCode;
            do
            {
                do
                {
                    errorCode = ffmpeg.av_read_frame(format, packet);
                    if (errorCode == ffmpeg.AVERROR_EOF)
                        return false;
                    Check(errorCode);
                } while (packet.Ptr->stream_index != streamIndex);

                Check(ffmpeg.avcodec_send_packet(codecContext, packet));
                errorCode = ffmpeg.avcodec_receive_frame(codecContext, frame);
            } while (errorCode == ffmpeg.EAGAIN);
            Check(errorCode);
            return true;
        }

        private void ConvertCurrentFrameToRGBA()
        {
            if (sws == null || convertedFrame == null)
            {
                sws = new SwsContextPtr(Width, Height, AVFormat, Width, Height, AVPixelFormat.AV_PIX_FMT_RGBA, ffmpeg.SWS_POINT);
                convertedFrame = new AVFramePtr();
                convertedFrame.Ptr->buf[0] = ffmpeg.av_buffer_alloc(Width * Height * 4);
                byte_ptrArray4 dataPointers;
                int_array4 lineSizes;
                Check(ffmpeg.av_image_fill_arrays(ref dataPointers, ref lineSizes, convertedFrame.Ptr->buf[0]->data, AVPixelFormat.AV_PIX_FMT_RGBA, Width, Height, 1));
                convertedFrame.Ptr->data.UpdateFrom(dataPointers);
                convertedFrame.Ptr->linesize.UpdateFrom(lineSizes);
            }

            sws.Scale(frame, convertedFrame);
        }

        public static Texture LoadImage(string file, GraphicsDevice gd)
        {
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            return LoadImage(stream, gd);
        }

        public static Texture LoadCubemap(string file, GraphicsDevice gd)
        {
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            return LoadCubemap(stream, gd);
        }

        public static Texture LoadImage(Stream stream, GraphicsDevice gd)
        {
            using var me = new ImageLoader(stream);
            if (!me.MoveToNextFrame())
                throw new InvalidDataException("Image stream does not have a single frame");
            me.ConvertCurrentFrameToRGBA();
            if (me.convertedFrame == null)
                throw new InvalidProgramException("Frame was not converted");

            var texture = gd.ResourceFactory.CreateTexture(new TextureDescription
            {
                Width = (uint)me.Width,
                Height = (uint)me.Height,
                Depth = 1,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                MipLevels = 1,
                ArrayLayers = 1,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled
            });
            gd.UpdateTexture(texture,
                new IntPtr(me.convertedFrame.Ptr->data[0]), (uint)(me.Width * me.Height * 4),
                0, 0, 0, (uint)me.Width, (uint)me.Height, 1, // area to update
                0, 0); // mipmapLevel, arrayLevel
            return texture;
        }

        public static Texture LoadCubemap(Stream stream, GraphicsDevice gd)
        {
            using var me = new ImageLoader(stream);
            var texture = gd.ResourceFactory.CreateTexture(new TextureDescription
            {
                Width = (uint)me.Width,
                Height = (uint)me.Height,
                Depth = 1,
                Format = PixelFormat.R8_G8_B8_A8_UNorm,
                MipLevels = 1,
                ArrayLayers = 1,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled | TextureUsage.Cubemap
            });

            for (uint i = 0; i < 6; i++)
            {
                if (!me.MoveToNextFrame())
                    throw new InvalidDataException("Image stream does not have a single frame");
                me.ConvertCurrentFrameToRGBA();
                if (me.convertedFrame == null)
                    throw new InvalidProgramException("Frame was not converted");

                gd.UpdateTexture(texture,
                    new IntPtr(me.convertedFrame.Ptr->data[0]), (uint)(me.Width * me.Height * 4),
                    0, 0, 0, (uint)me.Width, (uint)me.Height, 1, // area to update
                    0, i); // mipmapLevel, arrayLevel
            }
            return texture;
        }
    }
}
