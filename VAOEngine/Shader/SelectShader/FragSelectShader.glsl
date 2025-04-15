#version 330 core

out vec4 FragColor;

uniform float dist;

void main()
{

	FragColor = vec4(dist/225.0f,0.0f,0.0f,0.0f);

}