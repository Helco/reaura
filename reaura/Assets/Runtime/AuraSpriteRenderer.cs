using UnityEngine;
using System.Collections;

namespace Aura
{
    public class AuraSpriteRenderer : MonoBehaviour
    {
        [SerializeField]
        private Texture texture = null;
        [SerializeField]
        private Vector2 texturePos = Vector2.zero;
        [SerializeField]
        private CubemapFace face = CubemapFace.Unknown;

        public Texture Texture
        {
            get => texture;
            set
            {
                texture = value;
                IsDirty = true;
            }
        }

        public Vector2 TexturePos
        {
            get => texturePos;
            set
            {
                texturePos = value;
                IsDirty = true;
            }
        }

        public CubemapFace Face
        {
            get => face;
            set
            {
                face = value;
                if (worldRenderer != null)
                {
                    worldRenderer.UnregisterSprite(this);
                    worldRenderer.RegisterSprite(this);
                }
            }
        }

        public bool IsDirty
        {
            set
            {
                if (value && worldRenderer != null)
                    worldRenderer.SetSpriteDirty(this);
            }
        }

        private AuraWorldRenderer worldRenderer;

        private void Awake()
        {
            worldRenderer = FindObjectOfType<AuraWorldRenderer>();
            if (worldRenderer == null)
                throw new MissingReferenceException("AuraSprite cannot find the world renderer");
            enabled = false;
        }

        private void OnEnable()
        {
            worldRenderer.RegisterSprite(this);
        }

        private void OnDisable()
        {
            worldRenderer.UnregisterSprite(this);
        }
    }
}