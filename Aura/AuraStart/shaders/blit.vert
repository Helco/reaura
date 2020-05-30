#version 450

layout(location = 0) in vec2 vsin_pos;

layout(location = 0) out vec2 fsin_tex;

void main()
{
    gl_Position = vec4(vsin_pos, 1, 1);
    fsin_tex = (vsin_pos + vec2(1, 1)) * vec2(0.5, -0.5);
}
