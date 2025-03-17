#version 330

out vec4 OutColor;

in vec2 coord;

uniform sampler2D texture0;


void main()
{
	OutColor = texture(texture0, coord);
}