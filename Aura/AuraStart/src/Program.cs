using FFmpeg.AutoGen;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Aura.Veldrid
{
    class Program
    {
        

        static void Main(string[] args)
        {
            ffmpeg.RootPath = @"C:\dev\aura\ffmpeg";
            FFmpegHelpers.SetupLoggingToConsole();

            var window = VeldridStartup.CreateWindow(new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 1024,
                WindowHeight = 768,
                WindowTitle = "Aura ReEngined"
            });
            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions
            {
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
                SyncToVerticalBlank = true
            }, GraphicsBackend.Vulkan);

            var factory = graphicsDevice.ResourceFactory;

            var time = new GameTime();
            time.TargetFramerate = 60;
            InputSnapshot? inputSnapshot = null;
            var backend = new VeldridBackend(window, graphicsDevice);
            backend.AssetPath = @"C:\Program Files (x86)\Steam\steamapps\common\Aura Fate of the Ages";
            var game = new Game(backend);

            window.Resized += () =>
            {
                graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
                //panorama.Framebuffer = graphicsDevice.SwapchainFramebuffer;
            };

            while (window.Exists)
            {
                time.BeginFrame();
                if (time.HasFramerateChanged)
                    window.Title = $"Aura Reengined | {graphicsDevice.BackendType} | FPS: {(int)(time.Framerate + 0.5)}";

                backend.Update(time.Delta);
                game.Update(time.Delta);
                backend.Render();
                graphicsDevice.SwapBuffers();
                inputSnapshot = window.PumpEvents(); // pump events after swapbuffers in case the window got destroyed
                backend.CurrentInput = inputSnapshot;

                time.EndFrame();
            }

            graphicsDevice.Dispose();
        }
    }
}
