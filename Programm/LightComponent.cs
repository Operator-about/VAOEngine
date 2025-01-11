using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Shader = ShaderSystem;
using Model = Load;
using Camera = CameraSystem;
using OpenTK.Mathematics;

class LightComponent
{

    public void SetLight(Shader _Shader, Camera _Camera,Model _Ligth, Vector3 _LampPos, Matrix4 _Model)
    {
        
        _Shader.UseShader();
        _Shader.SetMatrix4("model", _Model);
        _Shader.SetMatrix4("view", _Camera.GetView());
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        

        _Ligth.Draw(_Shader);
    }
}
