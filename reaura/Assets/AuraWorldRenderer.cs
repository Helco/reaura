using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AuraWorldRenderer : MonoBehaviour
{
    private static readonly IReadOnlyDictionary<CubemapFace, Vector2Int> worldFacePositions = new Dictionary<CubemapFace, Vector2Int>()
    {
        { CubemapFace.PositiveX, new Vector2Int(1, 0) },
        { CubemapFace.NegativeX, new Vector2Int(3, 0) },
        { CubemapFace.PositiveY, new Vector2Int(1, 1) },
        { CubemapFace.NegativeY, new Vector2Int(0, 1) },
        { CubemapFace.PositiveZ, new Vector2Int(0, 0) },
        { CubemapFace.NegativeZ, new Vector2Int(2, 0) }
    };
    private static readonly IEnumerable<CubemapFace> allFaces = Enum
        .GetValues(typeof(CubemapFace))
        .Cast<CubemapFace>()
        .Except(new CubemapFace[] { CubemapFace.Unknown });

    [SerializeField]
    private Material spriteMaterial = null;
    [SerializeField]
    private RenderTexture renderTexture = null;
    [SerializeField]
    private Texture2D worldMap = null;

    Texture2D WorldMap
    {
        get => worldMap;
        set
        {
            worldMap = value;
            allDirty = true;
        }
    }

    private bool allDirty = true;
    private HashSet<CubemapFace> facesDirty = new HashSet<CubemapFace>();
    private int clipNameID, spriteNameID;
    private Dictionary<CubemapFace, HashSet<AuraSpriteRenderer>> sprites = new Dictionary<CubemapFace, HashSet<AuraSpriteRenderer>>()
    {
        // do it here to always ensure it is already initialized
        { CubemapFace.PositiveX, new HashSet<AuraSpriteRenderer>() },
        { CubemapFace.NegativeX, new HashSet<AuraSpriteRenderer>() },
        { CubemapFace.PositiveY, new HashSet<AuraSpriteRenderer>() },
        { CubemapFace.NegativeY, new HashSet<AuraSpriteRenderer>() },
        { CubemapFace.PositiveZ, new HashSet<AuraSpriteRenderer>() },
        { CubemapFace.NegativeZ, new HashSet<AuraSpriteRenderer>() }
    };

    private void Awake()
    {
        if (spriteMaterial == null)
            throw new MissingReferenceException("SpriteRenderer is missing its material");
        if (renderTexture == null)
            throw new MissingReferenceException("SpriteRenderer is missing a render texture");
        if (renderTexture.dimension != UnityEngine.Rendering.TextureDimension.Cube)
            throw new MissingReferenceException("SpriteRenderer has to use a cube RenderTexture");

        clipNameID = Shader.PropertyToID("clip");
        spriteNameID = Shader.PropertyToID("sprite");
    }

    private void OnPreRender()
    {
        if (worldMap == null)
            return;

        IEnumerable<CubemapFace> facesToRender = facesDirty;
        if (allDirty)
            facesToRender = allFaces;
        spriteMaterial.SetVector(clipNameID, new Vector4(0.0f, 0.0f, 1.0f, 1.0f));

        foreach (var face in facesToRender)
        {
            Graphics.SetRenderTarget(renderTexture, 0, face);
            spriteMaterial.SetVector(spriteNameID, GetWorldSpriteFor(face));
            Graphics.Blit(worldMap, spriteMaterial);

            var faceSprites = sprites[face];
            foreach (var sprite in faceSprites)
            {
                spriteMaterial.SetVector(spriteNameID, new Vector4(
                    sprite.Texture.width,
                    sprite.Texture.height,
                    sprite.TexturePos.x,
                    sprite.TexturePos.y) / 1024.0f);
                Graphics.Blit(sprite.Texture, spriteMaterial);
            }
        }

        allDirty = false;
        facesDirty.Clear();
    }

    private Vector4 GetWorldSpriteFor(CubemapFace face)
    {
        var pos = worldFacePositions[face];
        return new Vector4(4.0f, 2.0f, -pos.x, -pos.y);
    }

    public void RegisterSprite(AuraSpriteRenderer sprite)
    {
        if (sprite.Face == CubemapFace.Unknown)
            return;
        sprites[sprite.Face].Add(sprite);
        facesDirty.Add(sprite.Face);
    }

    public void UnregisterSprite(AuraSpriteRenderer sprite)
    {
        if (sprite.Face == CubemapFace.Unknown)
            return;
        sprites[sprite.Face].Remove(sprite);
        facesDirty.Add(sprite.Face);
    }

    public void SetSpriteDirty(AuraSpriteRenderer sprite)
    {
        if (sprite.Face == CubemapFace.Unknown)
            return;
        facesDirty.Add(sprite.Face);
    }
}
