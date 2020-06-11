using System;
using System.Threading.Tasks;
using Aura.Script;

namespace Aura.Systems
{
    public class AnimateSystem : BaseAnimateSystem
    {
        public override string GraphicListName => "&Animate";

        [ScriptFunction]
        private Task ScrPlayAVI(int index)
        {
            var tcs = new TaskCompletionSource<int>();
            Action action = () => { }; // have it already assigned when referenced
            action = () =>
            {
                videos[index].OnFinished -= action;
                tcs.SetResult(0);
            };
            videos[index].OnFinished += action;
            videos[index].IsLooping = false;
            videos[index].Play();
            sprites[index].IsEnabled = true;
            return tcs.Task;
        }
    }
}
