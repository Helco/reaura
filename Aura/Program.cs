using FFmpeg.AutoGen;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using System.Threading.Tasks;

namespace Aura.Veldrid
{
    class Program
    {
        private const string VertexCode = @"
#version 450

layout(location = 0) in vec2 vertexPos;
layout(location = 1) in vec4 vertexColor;

layout(location = 0) out vec4 rayDir;
layout(set = 0, binding = 2) uniform UniformBlock
{
    mat4 invProjection;
    mat4 invView;
};

void main()
{
    gl_Position = vec4(vertexPos, 0, 1);
    rayDir = invProjection * vec4(vertexPos, 0, 1);
    rayDir.w = 0;
    rayDir = invView * rayDir;
}";

        private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_rayDir;
layout(location = 0) out vec4 fsout_Color;
layout(set = 0, binding = 0) uniform sampler2DArray mainTexture;

void main()
{
    vec3 rayDir = normalize(fsin_rayDir.xyz);
    float sc, tc, ma, f;
    if (abs(rayDir.x) > abs(rayDir.y) && abs(rayDir.x) > abs(rayDir.z))
    {
        sc = rayDir.z * sign(rayDir.x);
        tc = -rayDir.y;
        ma = abs(rayDir.x);
        f = rayDir.x < 0 ? 1 : 3;
    }
    else if (abs(rayDir.y) > abs(rayDir.z))
    {
        sc = -rayDir.x;
        tc = rayDir.z * sign(rayDir.y);
        ma = abs(rayDir.y);
        f = rayDir.y < 0 ? 4 : 5;
    }
    else
    {
        sc = -rayDir.x * sign(rayDir.z);
        tc = -rayDir.y;
        ma = abs(rayDir.z);
        f = rayDir.z < 0 ? 2 : 0;
    }
    vec3 uvw = vec3(
        (sc / ma + 1) / 2,
        (tc / ma + 1) / 2,
        f);
    fsout_Color = texture(mainTexture, uvw);

    //fsout_Color = vec4((rayDir.xyz + vec3(1,1,1)) / 2, 1);
}";

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

            //VeldridStartup.CreateWindowAndGraphicsDevice(, out var window, out var graphicsDevice);
            var window = VeldridStartup.CreateWindow(new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowWidth = (int)(768 * 1.6f),
                WindowHeight = 768,
                WindowTitle = "Aura ReEngined"
            });
            var graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, new GraphicsDeviceOptions
            {
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true
            });

            Console.WriteLine("Using " + graphicsDevice.BackendType);
            graphicsDevice.SyncToVerticalBlank = true;
            var factory = graphicsDevice.ResourceFactory;
            VertexPositionColor[] quadVertices =
            {
                new VertexPositionColor(new Vector2(-1f, 1f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(1f, 1f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-1f, -1f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(1f, -1f), RgbaFloat.Yellow),
                new VertexPositionColor(new Vector2(-1f, 1f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(1f, 1f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-1f, -1f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(1f, -1f), RgbaFloat.Yellow)
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

            var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(0.97056514f, window.Width / (float)window.Height, 0.1f, 1.0f);
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
                projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(55.0f * 3.141592653f / 180.0f, window.Width / window.Height, 0.1f, 1.0f);
                Matrix4x4.Invert(projectionMatrix, out projectionMatrix);
                graphicsDevice.UpdateBuffer(uniformBuffer, 0, projectionMatrix);
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
                commandList.ClearColorTarget(0, RgbaFloat.Black);
                commandList.SetVertexBuffer(0, _vertexBuffer);
                commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
                commandList.SetPipeline(pipeline);
                commandList.UpdateBuffer(uniformBuffer, 4 * 16, viewMatrix);
                commandList.SetGraphicsResourceSet(0, resourceSet);
                commandList.DrawIndexed(
                    indexCount: 6,
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
