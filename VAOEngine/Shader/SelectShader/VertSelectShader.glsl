#version 330 core
layout(location=0) in vec4 aPos;

uniform mat4 proj;
uniform mat4 view;
uniform mat4 model;

void main()
{

	gl_Position = proj*view*model*aPos;

}