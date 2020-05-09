using FFmpeg.AutoGen;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using System.Threading.Tasks;

namespace AuraStart
{
    public static class h
    {
        public static unsafe string av_strerror(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int error)
        {
            if (error < 0) throw new ApplicationException(av_strerror(error));
            return error;
        }
    }

    class Program
    {
        private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_Pos;

void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
    fsin_Pos = Position;
}";

        private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_Pos;
layout(location = 0) out vec4 fsout_Color;
layout(set = 0, binding = 0) uniform sampler2D mainTexture;

void main()
{
    fsout_Color = texture(mainTexture, fsin_Pos);
}";

        private static unsafe void SetupLogging()
        {
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);

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

        private static unsafe void testffmpeg()
        {
            var context = ffmpeg.avformat_alloc_context();
            var frame = ffmpeg.av_frame_alloc();
            ffmpeg.avformat_open_input(&context, @"C:\dev\aura\out\009\009.pvd\009.bik", null, null).ThrowExceptionIfError();
           // ffmpeg.avformat_open_input(&context, @"C:\Program Files (x86)\Steam\steamapps\common\Aura Fate of the Ages\FullAnim\Intro_snd.bik", null, null).ThrowExceptionIfError();
            ffmpeg.avformat_find_stream_info(context, null).ThrowExceptionIfError();
            Console.WriteLine("Found " + context->nb_streams);
            for (uint i = 0; i < context->nb_streams; i++)
                ffmpeg.av_dump_format(context, (int)i, "009.bik", 0);

            AVCodec* codec = null;
            var index = ffmpeg.av_find_best_stream(context, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0).ThrowExceptionIfError();
            Console.WriteLine("best stream is encoded with: " + ffmpeg.avcodec_get_name(codec->id));

            var codecContext = ffmpeg.avcodec_alloc_context3(codec);
            ffmpeg.avcodec_parameters_to_context(codecContext, context->streams[0]->codecpar).ThrowExceptionIfError();
            ffmpeg.avcodec_open2(codecContext, codec, null).ThrowExceptionIfError();
            Console.WriteLine($"Size: {codecContext->width}x{codecContext->height} pixFmt: {codecContext->pix_fmt.ToString()}");
            ffmpeg.avcodec_close(codecContext);
            ffmpeg.av_frame_unref(frame);
            ffmpeg.av_frame_free(&frame);
            ffmpeg.avformat_close_input(&context);
        }

        static void Main(string[] args)
        {
            ffmpeg.RootPath = @"C:\dev\aura\ffmpeg";
            SetupLogging();
            testffmpeg();


            VeldridStartup.CreateWindowAndGraphicsDevice(new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = 1024,
                WindowHeight = 768,
                WindowTitle = "Aura ReEngined"
            }, out var window, out var graphicsDevice);
            Console.WriteLine("Using " + graphicsDevice.BackendType);
            graphicsDevice.SyncToVerticalBlank = true;
            var factory = graphicsDevice.ResourceFactory;
            VertexPositionColor[] quadVertices =
            {
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow),
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
            };
            ushort[] quadIndices = { 0, 1, 2, 2, 1, 3 };
            var _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            var _indexBuffer = factory.CreateBuffer(new BufferDescription(6 * sizeof(ushort), BufferUsage.IndexBuffer));
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            graphicsDevice.UpdateBuffer(_vertexBuffer, 0, ref quadVertices[0], 4 * VertexPositionColor.SizeInBytes);
            graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(VertexCode),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(FragmentCode),
                "main");
            var _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            //var swTexture = new Veldrid.ImageSharp.ImageSharpTexture(@"C:\dev\aura\out\009\009.psp\Temper1.jpg");
            //var texture = swTexture.CreateDeviceTexture(graphicsDevice, factory);
            var texture = factory.CreateTexture(new TextureDescription
            {
                Width = 1,
                Height = 1,
                Format = PixelFormat.R8_UNorm,
                ArrayLayers = 1,
                MipLevels = 1,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Sampled
            });
            var sampler = factory.CreateSampler(new SamplerDescription
            {
                AddressModeU = SamplerAddressMode.Clamp,
                AddressModeV = SamplerAddressMode.Clamp,
                Filter = SamplerFilter.MinPoint_MagPoint_MipPoint
            });

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;

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
                    }
                }
            });
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { resourceLayout };

            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription
            {
                Layout = resourceLayout,
                BoundResources = new BindableResource[]
                {
                    texture,
                    sampler
                }
            });


            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);

            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            var commandList = factory.CreateCommandList();
            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.SetVertexBuffer(0, _vertexBuffer);
            commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            commandList.SetPipeline(pipeline);
            commandList.SetGraphicsResourceSet(0, resourceSet);
            commandList.DrawIndexed(
                indexCount: 6,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
            commandList.End();

            var time = new System.Diagnostics.Stopwatch();
            time.Start();

            float targetFrametime = 1 / 60.0f;
            var lastSecond = time.Elapsed;
            int fps = 0;

            while (window.Exists)
            {
                var framestart = time.Elapsed;

                fps++;
                if ((time.Elapsed - lastSecond).TotalSeconds >= 1)
                {
                    double fpsHP = fps / (time.Elapsed - lastSecond).TotalSeconds;
                    lastSecond = time.Elapsed;
                    fps = 0;
                    Console.WriteLine($"FPS: {(int)(fpsHP + 0.5)}");
                }

                window.PumpEvents();
                graphicsDevice.SubmitCommands(commandList);
                graphicsDevice.SwapBuffers();

                var frametime = time.Elapsed - framestart;
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

        struct VertexPositionColor
        {
            public Vector2 Position; // This is the position, in normalized device coordinates.
            public RgbaFloat Color; // This is the color of the vertex.
            public VertexPositionColor(Vector2 position, RgbaFloat color)
            {
                Position = position;
                Color = color;
            }
            public const uint SizeInBytes = 24;
        }
    }
}
