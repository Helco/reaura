using UnityEngine;
using UnityEngine.Video;

namespace Aura
{
    public class AuraSOItem : ScriptableObject
    {
        public Texture2D sprite;
        public Texture2D spriteActive;
        public Texture2D cursorSprite;
        public Texture2D cursorSpriteActive;
        public VideoClip animate;

        public string description;
    }
}
