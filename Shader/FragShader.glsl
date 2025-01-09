#version 330
out vec4 FragColor;

in vec3 CurrentPos;
in vec3 Normal;



void main()
{
	FragColor = vec4(2.0f,4.0f,3.0f,1.0f);
	vec3 normal = normalize(Normal);
}