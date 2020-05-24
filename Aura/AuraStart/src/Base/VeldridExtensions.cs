using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace Aura.Veldrid
{
    public static class VeldridExtensions
    {
        public static Shader[] LoadShadersFromFiles(this ResourceFactory factory, string shaderName)
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

        public static DeviceBuffer CreateBufferFrom<T>(this GraphicsDevice gd, BufferUsage usage, params T[] array) where T : struct
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

        public static RgbaFloat WithAlpha(this RgbaFloat c, float alpha) =>
            new RgbaFloat(c.R, c.G, c.B, alpha);

    }
}
