﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Aura
{
    public interface ITexture : IDisposable
    {
        Vector2 Size { get; }
    }

    public interface IVideoTexture : ITexture
    {
        bool IsLooping { get; set; }
        bool IsPlaying { get; }
        void Play();
        void Pause();
        void Stop();
    }

    public interface IWorldSprite
    {
        bool IsEnabled { get; set; }
        CubeFace Face { get; set; }
        Vector2 Position { get; set; }
        ITexture? Texture { get; set; }
        void MarkDirty();
    }

    public interface IWorldRenderer : IDisposable
    {
        static readonly Vector2 MaxViewportSize = new Vector2(1024.0f, 768.0f);

        Vector2 ViewportOffset { get; set; }
        Vector2 ViewportSize { get; set; }
        bool IsActive { get; set; }
        int Order { get; set; }
        IReadOnlyList<IWorldSprite> Sprites { get; }

        Vector2 ConvertScreenToWorld(Vector2 screenPos);
        Vector2 ConvertWorldToScreen(Vector2 worldPos);
        void SetViewAt(Vector2 worldPos);
    }

    public interface IPanoramaWorldRenderer : IWorldRenderer
    {
        Vector2 ViewRotation { get; set; }
    }

    public interface IBackend
    {
        Stream? OpenAssetFile(string resourceName);
        ITexture CreateImage(Stream stream);
        IVideoTexture CreateVideo(Stream stream);
        IPanoramaWorldRenderer CreatePanorama(Stream stream, int spriteCapacity);

        Vector2 CursorPosition { get; set; }
        event Action<Vector2> OnClick;
        event Action<Vector2> OnViewDrag;
    }
}
