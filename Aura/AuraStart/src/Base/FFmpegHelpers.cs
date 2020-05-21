using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;
using static Aura.Veldrid.FFmpegHelpers;

namespace Aura.Veldrid
{
    public static unsafe class FFmpegHelpers
    {
        public static int Check(int errorCode)
        {
            if (errorCode < 0)
            {
                var textMaxSize = 1024UL;
                var textBuffer = stackalloc byte[(int)textMaxSize];
                ffmpeg.av_strerror(errorCode, textBuffer, textMaxSize);
                throw new Exception(Marshal.PtrToStringUTF8((IntPtr)textBuffer));
            }
            return errorCode;
        }

        public static double ConvertTimestamp(long ts, AVRational tb)
        {
            double freq = (double)tb.num / tb.den;
            return freq * ts;
        }
    }

    public unsafe abstract class FFmpegPtr<T> : BaseDisposable where T : unmanaged
    {
        public T* Ptr { get; protected set; }
        protected abstract void FreeFFmpeg(T** ptr);

        protected override void DisposeNative()
        {
            if (Ptr != null)
            {
                var ptr = Ptr;
                FreeFFmpeg(&ptr);
                Ptr = null;
            }
        }

        public static implicit operator T* (FFmpegPtr<T>? ptr) => ptr == null ? null : ptr.Ptr;
    }

    public unsafe class AVFormatContextPtr : FFmpegPtr<AVFormatContext>
    {
        public AVFormatContextPtr()
        {
            Ptr = ffmpeg.avformat_alloc_context();
        }

        public void OpenInput(string? url)
        {
            var ptr = Ptr;
            Check(ffmpeg.avformat_open_input(&ptr, url, null, null));
        }

        protected override unsafe void FreeFFmpeg(AVFormatContext** ptr) =>
            ffmpeg.avformat_close_input(ptr);
    }

    public unsafe class AVCodecContextPtr : FFmpegPtr<AVCodecContext>
    {
        public AVCodecContextPtr(AVCodec* codec)
        {
            Ptr = ffmpeg.avcodec_alloc_context3(codec);
        }

        protected override unsafe void FreeFFmpeg(AVCodecContext** ptr) =>
            ffmpeg.avcodec_close(*ptr);
    }

    public unsafe class AVPacketPtr : FFmpegPtr<AVPacket>
    {
        public AVPacketPtr()
        {
            Ptr = ffmpeg.av_packet_alloc();
        }

        public void Unref()
        {
            ffmpeg.av_packet_unref(Ptr);
            Ptr = null;
        }
        
        protected override unsafe void FreeFFmpeg(AVPacket** ptr) =>
            ffmpeg.av_packet_free(ptr);
    }

    public unsafe class AVFramePtr : FFmpegPtr<AVFrame>
    {
        public AVFramePtr()
        {
            Ptr = ffmpeg.av_frame_alloc();
        }

        public void Unref()
        {
            ffmpeg.av_frame_unref(Ptr);
            Ptr = null;
        }

        protected override unsafe void FreeFFmpeg(AVFrame** ptr) =>
            ffmpeg.av_frame_free(ptr);
    }

    public unsafe class SwsContextPtr : FFmpegPtr<SwsContext>
    {
        public SwsContextPtr(int srcW, int srcH, AVPixelFormat srcFormat, int dstW, int dstH, AVPixelFormat dstFormat, int flags)
        {
            Ptr = ffmpeg.sws_getContext(srcW, srcH, srcFormat, dstW, dstH, dstFormat, flags, null, null, null);
        }

        public void Scale(AVFramePtr from, AVFramePtr to)
        {
            Check(ffmpeg.sws_scale(this, from.Ptr->data, from.Ptr->linesize, 0, from.Ptr->height, to.Ptr->data, to.Ptr->linesize));
        }

        protected override unsafe void FreeFFmpeg(SwsContext** ptr) =>
            ffmpeg.sws_freeContext(*ptr);
    }
}
