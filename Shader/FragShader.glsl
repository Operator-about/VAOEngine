#version 330
out vec4 FragColor;


in vec3 CurrentPos;
in vec3 Normal;
in vec3 Color;


void main()
{
	vec3 normal = normalize(Normal);
	
	FragColor = vec4(Color,1.0f);

}