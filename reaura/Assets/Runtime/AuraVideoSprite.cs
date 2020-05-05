using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Aura
{
    [RequireComponent(typeof(AuraSpriteRenderer))]
    [RequireComponent(typeof(VideoPlayer))]
    public class AuraVideoSprite : MonoBehaviour
    {
        private AuraSpriteRenderer spriteRenderer;
        private VideoPlayer videoPlayer;
        private RenderTexture renderTexture;

        private void Awake()
        {
            spriteRenderer = GetComponent<AuraSpriteRenderer>();
            videoPlayer = GetComponent<VideoPlayer>();
            if (spriteRenderer == null)
                throw new MissingComponentException("AuraVideoSprite needs an AuraASpriteRenderer");
            if (videoPlayer == null)
                throw new MissingComponentException("AuraVideoSprite needs a VideoPlayer");
            if (videoPlayer.clip == null)
                throw new MissingComponentException("No VideoClip is assigned to AuraVideoSprite's VideoPlayer");

            renderTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 0, RenderTextureFormat.ARGB32);
            renderTexture.name = gameObject.name + " VideoTexture";
            videoPlayer.targetTexture = renderTexture;
            spriteRenderer.Texture = renderTexture;
        }

        private void Start()
        {
            videoPlayer.Play();
        }

        private void Update()
        {
            if (videoPlayer.isPlaying)
                spriteRenderer.IsDirty = true;
        }
    }
}