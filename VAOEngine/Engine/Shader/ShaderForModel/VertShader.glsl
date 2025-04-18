﻿#version 330 core
layout(location=0) in vec3 aPosition;
layout(location=1) in vec3 aNormal;
layout(location=2) in vec2 aTexCoord;

out vec3 CurrentPos;
out vec3 Normal;
out vec2 TexCoord;


uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;



void main()
{
	gl_Position = vec4(aPosition,1.0f)*model*view*proj;
	CurrentPos = vec3(vec4(aPosition,1.0f)*model);
	Normal = aNormal * mat3(transpose(inverse(model)));
	TexCoord = aTexCoord;
}