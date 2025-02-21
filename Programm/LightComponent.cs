using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Mathematics;

class LightComponent
{

    private Vector3 _LightColor;
    private Vector3 _LightPosition;
    

    public void AddPointLight()
    {
        Console.WriteLine("Input color(only in type Int32):");
        _LightColor = new Vector3(Int32.Parse(Console.ReadLine()!), Int32.Parse(Console.ReadLine()!), Int32.Parse(Console.ReadLine()!));
        Console.WriteLine("Input light position(only in type Int32):");
        _LightPosition = new Vector3(Int32.Parse(Console.ReadLine()!), Int32.Parse(Console.ReadLine()!), Int32.Parse(Console.ReadLine()!));
    }
    public void DrawLight(Shader _Shader, Camera _Camera)
    {
        _Shader.SetVector3("lightColor", _LightColor);
        _Shader.SetVector3("lightPosition", _LightPosition);
        _Shader.SetVector3("lightView", _Camera._Position);
    }
}
