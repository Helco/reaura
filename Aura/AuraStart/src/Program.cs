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
            }, GraphicsBackend.Vulkan);

            var factory = graphicsDevice.ResourceFactory;

            var time = new GameTime();
            time.TargetFramerate = 60;
            InputSnapshot? inputSnapshot = null;

            window.MouseMove += args =>
            {
                if (args.State.IsButtonDown(MouseButton.Right))
                {
                    /*var mouseMove = window.MouseDelta * time.Delta * 20.0f * 3.141592653f / 180.0f;
                    var rot = panorama.ViewRotation;
                    rot.X += mouseMove.Y;
                    rot.Y += mouseMove.X;
                    if (rot.Y < 0)
                        rot.Y += 2 * 3.141592653f;
                    if (rot.Y > 2 * 3.141592653f)
                        rot.Y -= 2 * 3.141592653f;
                    rot.X = MathF.Min(MathF.Max(rot.X, -MathF.PI / 2), MathF.PI / 2);
                    panorama.ViewRotation = rot;*/
                }
            };


            window.Resized += () =>
            {
                graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
                //panorama.Framebuffer = graphicsDevice.SwapchainFramebuffer;
            };

            window.MouseDown += args =>
            {
                if (args.MouseButton != MouseButton.Left || inputSnapshot == null)
                    return;
                //var aura = panorama.ConvertMouseToAura(inputSnapshot.MousePosition);
                //Console.WriteLine($"Click on  {aura.X}, {aura.Y}");
            };

            var commandList = factory.CreateCommandList();
            var videoPlayersList = factory.CreateCommandList();
            var videoPlayersFence = factory.CreateFence(false);

            while (window.Exists)
            {
                time.BeginFrame();
                if (time.HasFramerateChanged)
                    window.Title = $"Aura Reengined | {graphicsDevice.BackendType} | FPS: {(int)(time.Framerate + 0.5)}";

                videoPlayersList.Begin();
                //videoPlayer.Update(time.Delta, videoPlayersList);
                videoPlayersList.End();
                videoPlayersFence.Reset();
                graphicsDevice.SubmitCommands(videoPlayersList, videoPlayersFence);
                graphicsDevice.WaitForFence(videoPlayersFence);
                //worldRenderer.MarkSpriteDirty(1);

                //worldRenderer.Render(waitUntilFinished: true);
                commandList.Begin();
                //panorama.Render(commandList);
                commandList.End();
                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers();
                inputSnapshot = window.PumpEvents(); // pump events after swapbuffers in case the window got destroyed

                time.EndFrame();
            }

            //worldRenderer.Dispose();
            //panorama.Dispose();
            videoPlayersFence.Dispose();
            videoPlayersList.Dispose();
            commandList.Dispose();
            graphicsDevice.Dispose();
        }
    }
}
