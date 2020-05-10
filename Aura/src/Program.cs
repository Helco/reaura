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

        private static Shader[] LoadShaders(ResourceFactory factory, string shaderName)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(File.ReadAllText($"shaders/{shaderName}.vert")),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(File.ReadAllText($"shaders/{shaderName}.frag")),
                "main");
            return factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
        }

        private static DeviceBuffer CreateBufferFrom<T>(GraphicsDevice gd, BufferUsage usage, params T[] array) where T : struct
        {
            int stride = Marshal.SizeOf<T>();
            bool isStructured = usage == BufferUsage.StructuredBufferReadOnly || usage == BufferUsage.StructuredBufferReadWrite;
            var buffer = gd.ResourceFactory.CreateBuffer(new BufferDescription
            {
                Usage = usage,
                SizeInBytes = (uint)(array.Length * stride),
                StructureByteStride = (uint)(isStructured ? stride : 0)
            });
            gd.UpdateBuffer(buffer, 0, array);
            return buffer;
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
                PreferStandardClipSpaceYDirection = true
            }, GraphicsBackend.Direct3D11);

            Console.WriteLine("Using " + graphicsDevice.BackendType);
            graphicsDevice.SyncToVerticalBlank = true;
            var factory = graphicsDevice.ResourceFactory;
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

            var _vertexBuffer = CreateBufferFrom(graphicsDevice, BufferUsage.VertexBuffer,
                new Vector2(-1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-1f, -1f),
                new Vector2(1f, -1f));
            var _indexBuffer = CreateBufferFrom<ushort>(graphicsDevice, BufferUsage.IndexBuffer, 0, 1, 2, 3);

            var texture = ImageLoader.LoadImage(@"C:\Program Files (x86)\Steam\steamapps\common\Aura Fate of the Ages\Global\Cursors\Cursor_Active.dds", graphicsDevice);
            var cubemap = ImageLoader.LoadCubemap(@"C:\dev\aura\out\009\009.pvd\009.bik", graphicsDevice, asRenderTexture: true);
            var sampler = factory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                AddressModeW = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinLinear_MagLinear_MipPoint
            });

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription
            {
                Elements = new ResourceLayoutElementDescription[]
                {
                    new ResourceLayoutElementDescription
                    {
                        Kind = ResourceKind.TextureReadOnly,
                        Stages = ShaderStages.Fragment,
                        Name = "MainTexture",
                        Options = ResourceLayoutElementOptions.None
                    },
                    new ResourceLayoutElementDescription
                    {
                        Kind = ResourceKind.Sampler,
                        Stages = ShaderStages.Fragment,
                        Name = "MainTextureSampler"
                    },
                    new ResourceLayoutElementDescription
                    {
                        Kind = ResourceKind.UniformBuffer,
                        Stages = ShaderStages.Vertex,
                        Name = "UniformBlock"
                    }
                }
            });
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { resourceLayout };

            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(0.97056514f, 1.6f, 0.1f, 1.0f);
            Matrix4x4.Invert(projectionMatrix, out projectionMatrix);
            var viewMatrix = Matrix4x4.Identity;
            var uniformBuffer = factory.CreateBuffer(new BufferDescription
            {
                SizeInBytes = 2 * 4 * 16,
                Usage = BufferUsage.UniformBuffer
            });
            graphicsDevice.UpdateBuffer(uniformBuffer, 0, new Matrix4x4[] { projectionMatrix, viewMatrix });

            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription
            {
                Layout = resourceLayout,
                BoundResources = new BindableResource[]
                {
                    cubemap,
                    sampler,
                    uniformBuffer
                }
            });

            var _shaders = LoadShaders(factory, "cubemap");

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);

            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            var commandList = factory.CreateCommandList();

            var time = new System.Diagnostics.Stopwatch();
            time.Start();

            float targetFrametime = 1 / 60.0f;
            var lastSecond = time.Elapsed;
            int fps = 0;
            Vector2 rot = Vector2.Zero;
            TimeSpan lastFrame = TimeSpan.Zero;
            double frameDelta = 0.0;

            window.MouseMove += args =>
            {
                if (args.State.IsButtonDown(MouseButton.Right))
                {
                    rot += window.MouseDelta * (float)frameDelta * -50.0f * 3.141592653f / 180.0f;
                    if (rot.X < 0)
                        rot.X += 2 * 3.141592653f;
                    if (rot.X > 2 * 3.141592653f)
                        rot.X -= 2 * 3.141592653f;
                    rot.Y = MathF.Min(MathF.Max(rot.Y, -MathF.PI / 2), MathF.PI / 2);
                    viewMatrix = Matrix4x4.CreateFromYawPitchRoll(rot.X, rot.Y, 0.0f);
                }
            };

            window.Resized += () =>
            {
                graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
            };

            while (window.Exists)
            {
                frameDelta = (time.Elapsed - lastFrame).TotalSeconds;
                lastFrame = time.Elapsed;

                fps++;
                if ((time.Elapsed - lastSecond).TotalSeconds >= 1)
                {
                    double fpsHP = fps / (time.Elapsed - lastSecond).TotalSeconds;
                    lastSecond = time.Elapsed;
                    fps = 0;
                    window.Title = "Aura Reengined | FPS: " + (int)(fpsHP + 0.5);
                }

                window.PumpEvents();
                commandList.Begin();
                commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
                commandList.SetViewport(0, new Viewport(0, 0, window.Width, window.Width / 1.6f, 0.0f, 1.0f));
                commandList.ClearColorTarget(0, RgbaFloat.Black);
                commandList.SetVertexBuffer(0, _vertexBuffer);
                commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
                commandList.SetPipeline(pipeline);
                commandList.UpdateBuffer(uniformBuffer, 4 * 16, viewMatrix);
                commandList.SetGraphicsResourceSet(0, resourceSet);
                commandList.DrawIndexed(
                    indexCount: 4,
                    instanceCount: 1,
                    indexStart: 0,
                    vertexOffset: 0,
                    instanceStart: 0);
                commandList.End();
                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers();

                var frametime = time.Elapsed - lastFrame;
                float delay = targetFrametime - (float)frametime.TotalSeconds;
                if (delay > 0)
                    System.Threading.Thread.Sleep((int)(delay * 1000.0f));
            }

            resourceSet.Dispose();
            resourceLayout.Dispose();
            sampler.Dispose();
            texture.Dispose();
            pipeline.Dispose();
            commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            graphicsDevice.Dispose();
        }
    }
}
