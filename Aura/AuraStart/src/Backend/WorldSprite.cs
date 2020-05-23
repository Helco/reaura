using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Aura.Veldrid
{
    public class WorldSprite : BaseDisposable, IWorldSprite
    {
        private readonly IReadOnlyList<SpriteRenderer> renderers;
        private readonly int index;
        private SpriteRenderer CurrentRenderer => renderers[(int)face];
        private ResourceSet? resourceSet = null;
        private ITexture? texture = null;
        private bool isEnabled = false;
        private Vector2 position;
        private CubeFace face;

        public WorldSprite(IReadOnlyList<SpriteRenderer> renderers, int i)
        {
            this.renderers = renderers;
            index = i;
        }

        protected override void DisposeManaged()
        {
            resourceSet?.Dispose();
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                CurrentRenderer.SetSpriteResourceSet(index, value ? resourceSet : null);
            }
        }

        public CubeFace Face
        {
            get => face;
            set
            {
                if ((int)value >= renderers.Count)
                    throw new ArgumentOutOfRangeException(nameof(value));

                CurrentRenderer.SetSpriteResourceSet(index, null);
                face = value;
                CurrentRenderer.SetSpriteResourceSet(index, IsEnabled ? resourceSet : null);
                CurrentRenderer.SetSpriteQuad(index, position, texture?.Size ?? Vector2.Zero);
            }
        }

        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                CurrentRenderer.SetSpriteQuad(index, position, texture?.Size ?? Vector2.Zero);
            }
        }

        public ITexture? Texture
        {
            get => texture;
            set
            {
                resourceSet?.Dispose();
                texture = value;
                if (value == null)
                    return;
                resourceSet = CurrentRenderer.Common.Factory.CreateResourceSet(new ResourceSetDescription
                {
                    Layout = CurrentRenderer.Common.ResourceLayout,
                    BoundResources = new BindableResource[]
                    {
                        ((AuraTexture)value).Texture,
                        CurrentRenderer.Common.PointSampler,
                        CurrentRenderer.UniformBuffer // FIXME: this assumes uniform buffer is same for all face sprite renderers
                    }
                });
                CurrentRenderer.SetSpriteQuad(index, position, value.Size);
                CurrentRenderer.SetSpriteResourceSet(index, IsEnabled ? resourceSet : null);
            }
        }

        public void MarkDirty() => CurrentRenderer.MarkDirty();
    }
}
