#version 330 core
layout(location=0) in aPosition;
layout(location=1) in aTexCoord;

out vec2 coord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

int main()
{

	gl_Position = vec4(aPosition,1.0f)*model*view*proj;
	coord=aTexCoord;

}