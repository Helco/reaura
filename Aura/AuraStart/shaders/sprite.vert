#version 450

layout(location = 0) in vec2 vsin_pos;
layout(location = 1) in vec2 vsin_tex;

layout(location = 0) out vec2 fsin_tex;
layout(set = 0, binding = 2) uniform UniformBlock
{
    mat4 projection;
};

void main()
{
    gl_Position = projection * vec4(vsin_pos, -1, 1);
    fsin_tex = vsin_tex;
}
