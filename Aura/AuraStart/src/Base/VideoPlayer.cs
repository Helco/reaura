using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Veldrid;
using FFmpeg.AutoGen;
using static Aura.Veldrid.FFmpegHelpers;

namespace Aura.Veldrid
{
    public unsafe class VideoPlayer : BaseDisposable
    {
        private AVFormatContextPtr format = new AVFormatContextPtr();
        private StreamAVIOContext avioStream;
        private Queue<AVPacketPtr> packetPool = new Queue<AVPacketPtr>();

        public VideoImageTrack ImageTrack { get; }
        public VideoAudioTrack? AudioTrack { get; } = null;
        public double Duration { get; }
        public double Time { get; private set; } = 0.0;
        public bool IsPlaying { get; private set; } = false;
        public bool IsStopped => !IsPlaying && Time == 0.0;
        public bool IsPaused => !IsPlaying && Time > 0.0;
        public bool IsLooping { get; set; } = true;

        public VideoPlayer(GraphicsDevice graphicsDevice, string fileName)
            : this(graphicsDevice, new FileStream(fileName, FileMode.Open, FileAccess.Read)) { }

        public VideoPlayer(GraphicsDevice graphicsDevice, Stream stream)
        {
            avioStream = new StreamAVIOContext(stream);
            format.Ptr->pb = avioStream.Context;
            format.OpenInput(null);

            int imageStreamIndex = Check(ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, null, 0));
            int audioStreamIndex = ffmpeg.av_find_best_stream(format, AVMediaType.AVMEDIA_TYPE_AUDIO, -1, -1, null, 0);
            if (audioStreamIndex >= 0)
                AudioTrack = new VideoAudioTrack(this, format.Ptr->streams[audioStreamIndex]);
            else if (audioStreamIndex != ffmpeg.AVERROR_STREAM_NOT_FOUND)
                Check(audioStreamIndex);

            var imageStream = format.Ptr->streams[imageStreamIndex];
            ImageTrack = new VideoImageTrack(graphicsDevice, this, imageStream);
            Duration = ConvertTimestamp(imageStream->duration, imageStream->time_base);

            ImageTrack.Reset();
            ImageTrack.Update(); // send first frame to texture
            AudioTrack?.Reset();
        }

        protected override void DisposeNative()
        {
            ImageTrack.Dispose();
            if (AudioTrack != null)
                AudioTrack.Dispose();
            while (packetPool.Any())
                packetPool.Dequeue().Dispose();
            format.Dispose();
            avioStream.Dispose();
        }

        public void ReturnPacket(AVPacketPtr packet)
        {
            packetPool.Enqueue(packet);
        }
        private AVPacketPtr GetFreePacket() => packetPool.Any()
            ? packetPool.Dequeue()
            : new AVPacketPtr();

        public bool TryGetPacketFor(int streamIndex, out AVPacketPtr? packet)
        {
            while(true)
            {
                packet = GetFreePacket();
                int errorCode = ffmpeg.av_read_frame(format, packet);
                if (errorCode == ffmpeg.AVERROR_EOF)
                {
                    ReturnPacket(packet);
                    packet = null;
                    return false;
                }
                Check(errorCode);

                if (packet.Ptr->stream_index == streamIndex)
                    return true;
                else if (packet.Ptr->stream_index == ImageTrack.StreamIndex)
                    ImageTrack.TakePacket(packet);
                else if (AudioTrack != null && packet.Ptr->stream_index == AudioTrack.StreamIndex)
                    AudioTrack.TakePacket(packet);
            }
        }

        public void Update(double deltaTime)
        {
            if (!IsPlaying)
                return;
            Time += Math.Min(deltaTime, 1.0 / ImageTrack.Framerate);

            ImageTrack.Update();
            AudioTrack?.Update();

            if (ImageTrack.IsFinished)
            {
                Stop();
                if (IsLooping)
                {
                    Play();
                    ImageTrack.Update();
                    AudioTrack?.Update();
                }
            }
        }

        public void Render(CommandList commandList)
        {
            ImageTrack.Render(commandList);
        }

        public void Play() => IsPlaying = true;
        public void Pause() => IsPlaying = false;
        public void Stop()
        {
            IsPlaying = false;
            Time = 0.0;

            long startTS = 0;
            if (format.Ptr->start_time != ffmpeg.AV_NOPTS_VALUE)
                startTS = format.Ptr->start_time;
            Check(ffmpeg.av_seek_frame(format, -1, startTS, ffmpeg.AVSEEK_FLAG_BACKWARD));
            ImageTrack.Reset();
            AudioTrack?.Reset();
        }

        internal double GuessFramerate(AVStream* stream, AVFramePtr? currentFrame)
        {
            var frameRate = ffmpeg.av_guess_frame_rate(format, stream, currentFrame);
            if (frameRate.num == 0)
                frameRate = stream->avg_frame_rate;
            if (frameRate.num == 0)
                frameRate = stream->codec->framerate;
            if (frameRate.num == 0)
                throw new NotSupportedException("Could not figure out any frame rate...");
            return ConvertTimestamp(1, frameRate);
        }
    }
}
