using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Veldrid;

namespace Aura.Veldrid
{
    public class VideoTextureSet : BaseDisposable, IEnumerable<IVideoTexture>
    {
        private class VideoTexture : AuraTexture, IVideoTexture
        {
            public VideoTextureSet Parent { get; }
            public VideoPlayer VideoPlayer { get; }

            public VideoTexture(VideoTextureSet parent, VideoPlayer videoPlayer) : base(videoPlayer.ImageTrack.Target, ownsTexture: false)
            {
                Parent = parent;
                VideoPlayer = videoPlayer;
            }

            protected override void DisposeManaged()
            {
                Parent.textures.Remove(this);
                VideoPlayer.Dispose();
            }

            public bool IsLooping
            {
                get => VideoPlayer.IsLooping;
                set => VideoPlayer.IsLooping = value;
            }

            public bool IsPlaying => VideoPlayer.IsPlaying;
            public void Pause() => VideoPlayer.Pause();
            public void Play() => VideoPlayer.Play();
            public void Stop() => VideoPlayer.Stop();
        }

        private GraphicsDevice device;
        private CommandList commandList;
        private Fence fence;
        private HashSet<VideoTexture> textures = new HashSet<VideoTexture>();

        public VideoTextureSet(GraphicsDevice device)
        {
            this.device = device;
            commandList = device.ResourceFactory.CreateCommandList();
            fence = device.ResourceFactory.CreateFence(true);
        }

        protected override void DisposeManaged()
        {
            commandList.Dispose();
            fence.Dispose();
            foreach (var tex in textures)
                tex.Dispose();
        }

        public void Update(float timeDelta)
        {
            foreach (var tex in textures)
                tex.VideoPlayer.Update(timeDelta);
        }

        public void RenderAll()
        {
            fence.Reset();
            commandList.Begin();
            foreach (var tex in textures)
                tex.VideoPlayer.Render(commandList);
            commandList.End();
            device.SubmitCommands(commandList, fence);
            device.WaitForFence(fence);
        }

        public IVideoTexture CreateFromStream(Stream stream)
        {
            var videoPlayer = new VideoPlayer(device, stream);
            var newTexture = new VideoTexture(this, videoPlayer);
            textures.Add(newTexture);
            return newTexture;
        }

        public IEnumerator<IVideoTexture> GetEnumerator() => textures.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => textures.GetEnumerator();
    }
}
