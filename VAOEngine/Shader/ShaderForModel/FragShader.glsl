﻿#version 330 core

out vec4 FragColor;

struct Material
{

	vec3 Ambient;
	vec3 Diffuse;
	sampler2D Diffuse2D;
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
in vec4 FragPosLight;

uniform vec3 _ViewPosition;
uniform int _TextureBindValid;
uniform Light _Light;
uniform Material _Material;
uniform sampler2D _ShadowMap;

vec3 ResultReturn(vec3 _Relust);

void main()
{
	vec3 _Result = vec3(1.0,1.0,1.0);

	if(_TextureBindValid==0)
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

		float _Shadow = 0.0;
		vec3 _LightCoord = FragPosLight.xyz/FragPosLight.w;
		if(_LightCoord.z<=1.0)
		{
			_LightCoord = (_LightCoord+1.0)/2.0;
			float _ClosesDepth = texture(_ShadowMap, _LightCoord.xy).r;
			float _CurrentDepth = _LightCoord.z;
			if(_CurrentDepth<_ClosesDepth)
			{
				_Shadow = 1.0;
			}
		}

		_Result = (_Ambient*(_Shadow-1.0)+_Diffuse+_Specular)*_Material.ModelColor;
		ResultReturn(_Result);

	};
	if(_TextureBindValid==1)
	{

		//Ambient
		vec3 _Ambient = _Light.Ambient*vec3(texture(_Material.Diffuse2D, TexCoord));

		//Diffuse
		vec3 _Normal = normalize(Normal);
		vec3 _LightDir = normalize(_Light.Position-CurrentPos);
		float _Diff = max(dot(_Normal, _LightDir),0.0);
		vec3 _Diffuse = _Light.Diffuse*_Diff*vec3(texture(_Material.Diffuse2D, TexCoord));

		//Specular
		vec3 _ViewDir = normalize(_ViewPosition-CurrentPos);
		vec3 _ReflectionDir = reflect(-_LightDir, _Normal);
		float _Spec = pow(max(dot(_ViewDir, _ReflectionDir),0.0),_Material.Shininess);
		vec3 _Specular = _Light.Specular*(_Spec*_Material.Shininess);

		
		float _Shadow = 0.0;
		vec3 _LightCoord = FragPosLight.xyz/FragPosLight.w;
		if(_LightCoord.z<=1.0)
		{
			_LightCoord = (_LightCoord+1.0)/2.0;
			float _ClosesDepth = texture(_ShadowMap, _LightCoord.xy).r;
			float _CurrentDepth = _LightCoord.z;
			if(_CurrentDepth<_ClosesDepth)
			{
				_Shadow = 1.0;
			}
		}

		_Result = (_Ambient*(1.0-_Shadow)+_Diffuse+_Specular);
		ResultReturn(_Result);

	}

	

	//OutPut
	FragColor = vec4(_Result,1.0);
}

vec3 ResultReturn(vec3 _Result)
{

	return _Result;

}

