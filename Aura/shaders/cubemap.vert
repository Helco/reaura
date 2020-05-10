#version 450

layout(location = 0) in vec2 vertexPos;

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
}
