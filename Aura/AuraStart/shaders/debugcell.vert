#version 450

layout(location = 0) in vec3 vsin_pos;
layout(location = 1) in vec2 vsin_uv;
layout(location = 2) in int vsin_cellIndex;

layout(location = 0) out vec2 fsin_uv;
layout(location = 1) out vec4 fsin_color;
layout(set = 0, binding = 0) uniform UniformBlock
{
	mat4 projection;
	mat4 view;
	vec4 color;
	vec4 selectedColor;
	float borderWidth;
	float borderAlpha;
	int selected;
};

void main()
{
	gl_Position = projection * view * vec4(vsin_pos, 1);
	fsin_uv = vsin_uv;
	fsin_color = selected == vsin_cellIndex ? selectedColor : color;
}
