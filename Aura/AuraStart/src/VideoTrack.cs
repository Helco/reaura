using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FFmpeg.AutoGen;
using static Aura.Veldrid.FFmpegHelpers;

namespace Aura.Veldrid
{
    public enum VideoTrackType
    {
        Image,
        Audio
    }

    public abstract unsafe class VideoTrack : BaseDisposable
    {
        public VideoTrackType TrackType { get; }
        public int StreamIndex => stream->index;

        protected VideoPlayer player;
        protected AVStream* stream; // this pointer does not belong to us (to VideoPlayer.format)
        private Queue<AVPacketPtr> packetQueue = new Queue<AVPacketPtr>();
        private bool isDraining = false;
        protected AVCodecContextPtr codecContext;
        protected AVFramePtr? currentFrame = null;
        protected AVFramePtr? nextFrame = null;
        protected double nextPTS = float.NaN;

        protected VideoTrack(VideoPlayer player, AVStream* stream)
        {
            this.player = player;
            this.stream = stream;
            TrackType = stream->codec->codec_type switch
            {
                AVMediaType.AVMEDIA_TYPE_VIDEO => VideoTrackType.Image,
                AVMediaType.AVMEDIA_TYPE_AUDIO => VideoTrackType.Audio,
                var _ => throw new NotSupportedException("Only image and audio tracks are supported")
            };

            var codec = ffmpeg.avcodec_find_decoder(stream->codecpar->codec_id);
            if (codec == null)
                throw new InvalidDataException("Could not find detected decoder?");
            codecContext = new AVCodecContextPtr(codec);
            ffmpeg.avcodec_parameters_to_context(codecContext, stream->codecpar);
            Check(ffmpeg.avcodec_open2(codecContext, codec, null));
        }

        protected override void DisposeNative()
        {
            if (currentFrame != null)
                currentFrame.Dispose();
            if (nextFrame != null)
                nextFrame.Dispose();
            codecContext.Dispose();
        }

        public void Reset()
        {
            isDraining = false;
            while (packetQueue.Any())
                packetQueue.Dequeue().Unref();
            if (currentFrame != null)
                currentFrame.Unref();
            if (nextFrame != null)
                nextFrame.Unref();
            ffmpeg.avcodec_flush_buffers(codecContext);
        }

        protected double ConvertTimestamp(long ts) => FFmpegHelpers.ConvertTimestamp(ts, stream->time_base);

        protected abstract double OnGotNextFrame();
        protected abstract void OnSwitchedFrames();

        public void Update()
        {
            if (nextFrame == null && TryDecodeNext())
                nextPTS = OnGotNextFrame();
            if (nextFrame != null && player.Timestamp >= nextPTS)
            {
                if (currentFrame != null)
                    currentFrame.Unref();
                currentFrame = nextFrame;
                nextFrame = null;
                OnSwitchedFrames();
                if (TryDecodeNext())
                    nextPTS = OnGotNextFrame();
            }
        }

        public void TakePacket(AVPacketPtr packet) => packetQueue.Enqueue(packet);

        private bool TryDecodeNext()
        {
            var decodedFrame = new AVFramePtr();
            int errorCode = ffmpeg.avcodec_receive_frame(codecContext, decodedFrame);
            if (errorCode == 0)
            {
                if (nextFrame != null)
                    nextFrame.Unref();
                nextFrame = decodedFrame;
                return true;
            }
            else if (errorCode != ffmpeg.AVERROR_EOF && errorCode != ffmpeg.AVERROR(ffmpeg.EAGAIN))
                Check(errorCode);

            while(errorCode == ffmpeg.AVERROR(ffmpeg.EAGAIN) && !isDraining)
            {
                AVPacketPtr? packet = null;
                if (!packetQueue.TryDequeue(out packet) && !player.TryGetPacketFor(stream->index, out packet))
                    isDraining = true;

                Check(ffmpeg.avcodec_send_packet(codecContext, packet));
                errorCode = ffmpeg.avcodec_receive_frame(codecContext, decodedFrame);
                if (packet != null)
                    player.ReturnPacket(packet);
            }

            while (errorCode == ffmpeg.AVERROR(ffmpeg.EAGAIN) && isDraining)
            {
                errorCode = ffmpeg.avcodec_receive_frame(codecContext, decodedFrame);
            }

            if (errorCode == 0)
            {
                if (nextFrame != null)
                    nextFrame.Unref();
                nextFrame = decodedFrame;
                return true;
            }
            decodedFrame.Dispose();
            if (errorCode != ffmpeg.AVERROR_EOF)
                Check(errorCode); // oh no, an actual error :(
            return false;
        }
    }
}
