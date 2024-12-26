#version 330 core
layout(location=0) in vec3 aPosition;
layout(location=1) in vec3 aNormal;
layout(location=2) in vec3 aColor;

out vec3 CurrentPos;
out vec3 Normal;
out vec3 Color;

uniform mat4 camMatrix;
uniform mat4 model;


void main(void)
{
	CurrentPos = vec3(model*vec4(aPosition,1.0f));

	Normal = aNormal;

	Color = aColor;

	gl_Position = camMatrix*vec4(CurrentPos,1.0f);
}