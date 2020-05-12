#version 450
#extension GL_KHR_vulkan_glsl: enable

layout(location = 0) in vec2 fsin_tex;
layout(location = 0) out vec4 fsout_Color;
layout(set = 0, binding = 0) uniform sampler2D mainTexture;

void main()
{
    fsout_Color = texture(mainTexture, fsin_tex);
}
