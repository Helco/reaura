#version 450
#extension GL_KHR_vulkan_glsl: enable

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
}
