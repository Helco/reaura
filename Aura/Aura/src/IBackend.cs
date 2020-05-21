using System;
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
        void Play();
        void Pause();
        void Stop();
    }

    public interface IWorldSprite
    {
        int Index { get; }
        bool IsEnabled { get; set; }
        CubeFace Face { get; set; }
        Vector2 Position { get; set; }
        ITexture Texture { get; set; }
        void MarkDirty();
        void Set(CubeFace face, Vector2 pos, ITexture texture);
    }

    public interface IWorldRenderer : IDisposable
    {
        IReadOnlyList<IWorldSprite> Sprites { get; }
    }

    public interface IBackground : IDisposable
    { }

    public interface IPanoramaBackground : IDisposable
    {
        void LoadCubemap(Stream stream);
    }

    public interface IBackend
    {
        Stream? OpenAssetFile(string resourceName);
        IWorldRenderer CreateWorldRenderer(int spriteCapacity);
        ITexture CreateImage(Stream stream);
        IVideoTexture CreateVideo(Stream stream);
        void CreatePanoramaBackground(Stream stream);

        IWorldRenderer ActiveWorldRenderer { get; set; }
        Vector2 CursorPosition { get; set; }
        ITexture CursorTexture { get; set; }
        event Action<Vector2> OnMouseClick;
    }
}
