using System;
using Aura.Script;

namespace Aura.Systems
{
    public class FonAnimateSystem : BaseAnimateSystem
    {
        public override string GraphicListName => "&Fon_Animate";

        [ScriptFunction]
        private void ScrLoadFonAVI(int index)
        {
            videos[index].Stop();
            videos[index].Play();
            sprites[index].IsEnabled = true;
        }

        [ScriptFunction]
        private void ScrImmediatelySuspendFonAVI(int index)
        {
            videos[index].Pause();
        }

        [ScriptFunction]
        private void ScrResumeFonAVI(int index)
        {
            videos[index].Play();
        }
    }
}
