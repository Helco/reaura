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
        private Stopwatch watch = new Stopwatch();
        
        public VideoImageTrack ImageTrack { get; }
        public VideoAudioTrack? AudioTrack { get; } = null;
        public double Duration { get; }
        public double Timestamp => watch.Elapsed.TotalSeconds;
        public bool IsPlaying => watch.IsRunning;
        public bool IsStopped => !IsPlaying && Timestamp == 0.0;
        public bool IsPaused => !IsPlaying && Timestamp > 0.0;
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

        public void Update(CommandList commandList)
        {
            ImageTrack.CommandList = commandList;
            ImageTrack.Update();
            AudioTrack?.Update();

            if (Timestamp >= Duration)
            {
                Stop();
                if (IsLooping)
                    Play();
            }
        }

        public void Play() => watch.Start();
        public void Pause() => watch.Stop();
        public void Stop()
        {
            watch.Stop();
            watch.Reset();

            long timestamp = 0;
            if (format.Ptr->start_time != ffmpeg.AV_NOPTS_VALUE)
                timestamp = format.Ptr->start_time;
            Check(ffmpeg.av_seek_frame(format, -1, timestamp, ffmpeg.AVSEEK_FLAG_BACKWARD));
            ImageTrack.Reset();
            AudioTrack?.Reset();
        }
    }
}
