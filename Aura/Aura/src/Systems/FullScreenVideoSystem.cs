using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Aura.Script;

namespace Aura.Systems
{
    public class FullScreenVideoSystem : BaseDisposable, IGameSystem
    {
        private LoadSceneContext? context;
        private IPuzzleWorldRenderer? renderer;
        private IWorldSprite? sprite;
        private IVideoTexture? currentVideo;

        public void CrossInitialize(IGameSystemContainer container)
        {
            renderer = container.Backend.CreatePuzzleRenderer(1);
            renderer.Order = 2000;
            sprite = renderer.Sprites.Single();
            sprite.IsEnabled = false;
            sprite.Face = CubeFace.Front;
            sprite.Position = Vector2.Zero;
        }

        public void OnBeforeSceneChange(LoadSceneContext context)
        {
            this.context = context;
        }

        public void Update(float timeDelta)
        {
            if (currentVideo != null)
            {
                sprite?.MarkDirty();
                if (!currentVideo.IsPlaying)
                {
                    currentVideo.Dispose();
                    currentVideo = null;
                }
            }
        }

        [ScriptFunction]
        private Task ScrPlayFullScreenAVI(string fileName)
        {
            if (context == null || sprite == null)
                throw new InvalidProgramException("Necessary initialization events were not triggered");
            if (currentVideo != null)
                throw new InvalidProgramException("A fullscreen video is already playing");

            var videoStream = context.OpenSceneAsset(fileName);
            if (videoStream == null)
                throw new FileNotFoundException($"Could not find video \"{fileName}\"");
            currentVideo = context.Backend.CreateVideo(videoStream);
            sprite.Texture = currentVideo;
            sprite.IsEnabled = true;
            currentVideo.IsLooping = false;
            currentVideo.Play();

            var tcs = new TaskCompletionSource<int>();
            currentVideo.OnFinished += () =>
            {
                sprite.IsEnabled = false;
                sprite.Texture = null;
                // delay disposing currentVideo to sometime outside of its control flow
                tcs.SetResult(0);
            };
            return tcs.Task;
        }
    }
}
