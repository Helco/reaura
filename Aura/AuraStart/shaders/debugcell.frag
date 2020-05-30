#version 450
#extension GL_KHR_vulkan_glsl: enable

layout(location = 0) out vec4 fsout_color;

layout(location = 0) in vec2 fsin_uv;
layout(location = 1) in vec4 fsin_color;
layout(set = 0, binding = 0) uniform UniformBlock
{
	mat4 projection;
	mat4 view;
	vec4 color;
	vec4 inactiveColor;
	vec4 selectedColor;
	float borderWidth;
	float borderAlpha;
	int selected;
};

void main()
{
	float isBorder = fsin_uv.x <= borderWidth || fsin_uv.y <= borderWidth ? 1 : 0;
	fsout_color = vec4(fsin_color.rgb, fsin_color.a * (1 - isBorder) + borderAlpha * isBorder);
}
