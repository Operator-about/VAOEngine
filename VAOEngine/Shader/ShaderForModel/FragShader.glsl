#version 330

out vec4 FragColor;

uniform vec3 objColor;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform vec3 lightView;

in vec3 Normal;
in vec3 CurrentPos;
in vec4 fragpos;

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
	vec3 lightcoords = fragpos.xyz / fragpos.w;
	if(lightcoords.z<=1.0f)
	{

		lightcoords = (lightcoords+1.0f)/2.0f;

		float closetDepth = texture(shadowmap,lightcoords.xy).r;
		float currentDepth = lightcoords.z;

		if(currentDepth>closetDepth)
		{
			shaddow=1.0f;
		}

	}
	vec3 Resultation = (Diffuse*(1.0f-shadow)+Ambient+Specular*(1.0f-shadow))*objColor;
	FragColor = vec4(Resultation,1.0f);

}