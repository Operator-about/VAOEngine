#version 330 core
layout(location=0) in vec3 aPos;

uniform mat4 lightproj;
uniform mat4 model;

void main()
{

	gl_Position = lightproj*model*vec4(aPos,1.0);

}