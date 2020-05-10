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
        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_WARNING);

            // do not convert to local function
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                if (level > ffmpeg.av_log_get_level()) return;

                var lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(line);
                Console.ResetColor();
            };

            ffmpeg.av_log_set_callback(logCallback);
        }

        static void Main(string[] args)
        {
            ffmpeg.RootPath = @"C:\dev\aura\ffmpeg";
            SetupLogging();

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
            }, GraphicsBackend.Direct3D11);

            var factory = graphicsDevice.ResourceFactory;

            var texture = ImageLoader.LoadImage(@"C:\Program Files (x86)\Steam\steamapps\common\Aura Fate of the Ages\Global\Cursors\Cursor_Active.dds", graphicsDevice);
            var cubemap = ImageLoader.LoadCubemap(@"C:\dev\aura\out\009\009.pvd\009.bik", graphicsDevice);
            var panorama = new CubemapPanorama(graphicsDevice, graphicsDevice.SwapchainFramebuffer);
            panorama.Texture = cubemap;

            var time = new GameTime();
            Vector2 rot = Vector2.Zero;

            window.MouseMove += args =>
            {
                if (args.State.IsButtonDown(MouseButton.Right))
                {
                    rot += window.MouseDelta * time.Delta * -20.0f * 3.141592653f / 180.0f;
                    if (rot.X < 0)
                        rot.X += 2 * 3.141592653f;
                    if (rot.X > 2 * 3.141592653f)
                        rot.X -= 2 * 3.141592653f;
                    rot.Y = MathF.Min(MathF.Max(rot.Y, -MathF.PI / 2), MathF.PI / 2);
                    panorama.ViewRotation = rot;
                }
            };

            window.Resized += () =>
            {
                graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
                panorama.Framebuffer = graphicsDevice.SwapchainFramebuffer;
            };

            var commandList = factory.CreateCommandList();

            while (window.Exists)
            {
                time.BeginFrame();
                if (time.HasFramerateChanged)
                    window.Title = $"Aura Reengined | {graphicsDevice.BackendType} | FPS: {(int)(time.Framerate + 0.5)}";

                window.PumpEvents();
                commandList.Begin();
                panorama.Render(commandList);
                commandList.End();
                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers();

                time.EndFrame();
            }

            commandList.Dispose();
            graphicsDevice.Dispose();
        }
    }
}
