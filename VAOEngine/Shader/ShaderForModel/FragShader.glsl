#version 330 core

out vec4 FragColor;

struct Material
{

	vec3 Ambient;
	vec3 Diffuse;
	vec3 Specular;
	vec3 ModelColor;
	float Shininess;

};



struct Light
{

	vec3 Position;
	vec3 Ambient;
	vec3 Diffuse;
	vec3 Specular;
	vec3 LightColor;

};


in vec3 Normal;
in vec2 TexCoord;
in vec3 CurrentPos;

uniform vec3 _ViewPosition;
uniform Light _Light;
uniform Material _Material;

void main()
{
	//Ambient
	vec3 _Ambient = _Light.Ambient*_Material.Ambient;

	//Diffuse
	vec3 _Normal = normalize(Normal);
	vec3 _LightDir = normalize(_Light.Position-CurrentPos);
	float _Diff = max(dot(_Normal, _LightDir),0.0);
	vec3 _Diffuse = _Light.Diffuse*(_Diff*_Material.Diffuse);

	//Specular
	vec3 _ViewDir = normalize(_ViewPosition-CurrentPos);
	vec3 _ReflectionDir = reflect(-_LightDir, _Normal);
	float _Spec = pow(max(dot(_ViewDir, _ReflectionDir),0.0),_Material.Shininess);
	vec3 _Specular = _Light.Specular*(_Spec*_Material.Shininess);

	//OutPut
	vec3 _Result = (_Ambient+_Diffuse+_Specular)*_Material.ModelColor;
	FragColor = vec4(_Result,1.0);
}

