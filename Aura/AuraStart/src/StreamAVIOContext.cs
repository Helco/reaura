using System;
using System.IO;
using FFmpeg.AutoGen;
using static Aura.Veldrid.FFmpegHelpers;

namespace Aura.Veldrid
{
    public unsafe class StreamAVIOContext : BaseDisposable
    {
        public Stream Stream { get; }
        public AVIOContext* Context { get; private set; }

        // keep this delegates as members so they won't be deleted by GC
        private avio_alloc_context_read_packet readPacketFunc;
        private avio_alloc_context_seek? seekFunc;

        public StreamAVIOContext(Stream stream, int bufferSize = 4096 * 4)
        {
            Stream = stream;
            Context = ffmpeg.avio_alloc_context(
                (byte*)ffmpeg.av_malloc((ulong)bufferSize), bufferSize,
                0, // write flag
                null, // opaque
                readPacketFunc = ReadPacket,
                null, // write packet
                seekFunc = stream.CanSeek ? (avio_alloc_context_seek)Seek : null);
        }

        public static implicit operator AVIOContext* (StreamAVIOContext avioContext) => avioContext.Context;

        protected override void DisposeNative()
        {
            base.DisposeNative();
            if (Context->buffer != null)
            {
                ffmpeg.av_free(Context->buffer);
                Context->buffer = null;
            }
            if (Context != null)
            {
                var context = Context;
                ffmpeg.avio_context_free(&context);
                Context = null;
            }
        }

        private int ReadPacket(void* opaque, byte* outBuffer, int size)
        {
            if (outBuffer == null || size < 0)
                return ffmpeg.AVERROR(ffmpeg.EINVAL);
            try
            {
                int result = Stream.Read(new Span<byte>(outBuffer, size));
                return result <= 0 ? ffmpeg.AVERROR_EOF : result;
            }
            catch(IOException)
            {
                return ffmpeg.AVERROR_STREAM_NOT_FOUND;
            }
        }

        private long Seek(void* opaque, long offset, int whence)
        {
            if ((whence & ffmpeg.AVSEEK_SIZE) > 0)
            {
                try
                {
                    return Stream.Length;
                }
                catch(NotSupportedException)
                {
                    return -1;
                }
            }

            // whence or SEEK_* or SeekOrigin seem to be unspoken standards...
            try
            {
                return Stream.Seek(offset, (SeekOrigin)(whence & 3));
            }
            catch(IOException)
            {
                return ffmpeg.AVERROR_STREAM_NOT_FOUND;
            }
        }
    }
}
