using UnityEngine;
using System.Collections;

public class AuraSprite : MonoBehaviour
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
            worldRenderer.UnregisterSprite(this);
            worldRenderer.RegisterSprite(this);
        }
    }

    public bool IsDirty
    {
        set
        {
            if (value)
                worldRenderer.SetSpriteDirty(this);
        }
    }

    private WorldRenderer worldRenderer;

    private void Awake()
    {
        worldRenderer = FindObjectOfType<WorldRenderer>();
        if (worldRenderer == null)
            throw new MissingReferenceException("AuraSprite cannot find the world renderer");
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
