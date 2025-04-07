using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Mathematics;

class LightComponent
{

    private Vector3 _LightColor;
    private Vector3 _LightPosition;
    

    public void AddPointLight()
    {
        _LightColor = new Vector3(1,1,1);
        _LightPosition = new Vector3(10,10,10);
    }
    public void DrawLight(Shader _Shader, Camera _Camera)
    {
        _Shader.SetVector3("lightColor", _LightColor);
        _Shader.SetVector3("lightPosition", _LightPosition);
        _Shader.SetVector3("lightView", _Camera._Position);
    }
}
