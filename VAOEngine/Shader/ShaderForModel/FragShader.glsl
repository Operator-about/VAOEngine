#version 330

out vec4 FragColor;

uniform vec3 objColor;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform vec3 lightView;

in vec3 Normal;
in vec2 TexCoord;
in vec3 CurrentPos;
in vec4 fragPosLight;

//uniform sampler2D MainTex;
uniform sampler2D shadowmap;

void main()
{

	float AmbientS = 0.1f;
	vec3 Ambient = AmbientS*lightColor;
	
	
	vec3 Norm = normalize(Normal);
	vec3 LigthDirect = normalize(lightPosition-CurrentPos);

	float Diff = max(dot(Norm, LigthDirect),0.0f);
	vec3 Diffuse = Diff * lightColor;

	float SpecularS = 0.5f;
	vec3 ViewDirection = normalize(lightView-CurrentPos);
	vec3 ReflectDirection = reflect(-LigthDirect, Norm);
	float Spec = pow(max(dot(ViewDirection, ReflectDirection),0.0f),32.0f);
	vec3 Specular = SpecularS * Spec * lightColor;

	


	float shadow = 0.0f;
	vec3 lightCoords = fragPosLight.xyz/fragPosLight.w;
	if(lightCoords.z<=1.0f)
	{
		lightCoords = (lightCoords+1.0f)/2.0f;

		float closesDepth = texture(shadowmap,lightCoords.xy).r;
		float currentDepth = lightCoords.z;
	    
		if(currentDepth<closesDepth)
		{
			shadow = 1.0f;
		}
	};

	

	vec3 Resultat = (Diffuse*(1.0f-shadow)+Ambient+Specular*(1.0f-shadow))*objColor;
	FragColor = vec4(Resultat,1.0f);
	
	
	
}