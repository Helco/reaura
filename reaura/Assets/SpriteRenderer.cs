using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRenderer : MonoBehaviour
{
    [SerializeField]
    private RenderTexture renderTexture;

    public Texture2D testTexture;
    public Vector2 pos;
    public Vector2 size = Vector2.one;
    public CubemapFace face;
    public Material blend;
    public Texture worldMap;

    private void Awake()
    {
        if (renderTexture == null)
            throw new MissingReferenceException("SpriteRenderer is missing a render texture");
        if (renderTexture.dimension != UnityEngine.Rendering.TextureDimension.Cube)
            throw new MissingReferenceException("SpriteRenderer has to use a cube RenderTexture");
    }

    private void OnPreRender()
    {
        Graphics.SetRenderTarget(renderTexture, 0, face);
        GL.Clear(false, true, Color.clear);
        size.x = testTexture.width;
        size.y = testTexture.height;
        blend.SetVector("sprite", new Vector4(size.x, size.y, pos.x, pos.y + size.y) / 1024.0f);
        Graphics.Blit(testTexture, blend);
        Graphics.SetRenderTarget(null);
    }

    private void Update() { }
}
