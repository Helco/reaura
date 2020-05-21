using System;
using System.Collections.Generic;
using FFmpeg.AutoGen;

namespace Aura.Veldrid
{
    public unsafe class VideoAudioTrack : VideoTrack
    {
        public VideoAudioTrack(VideoPlayer player, AVStream* stream) : base(player, stream)
        {
            Console.WriteLine("Audio is not supported yet");
        }

        protected override double OnGotNextFrame()
        {
            return 0.0;
        }

        protected override void OnSwitchedFrames()
        {
        }
    }
}
