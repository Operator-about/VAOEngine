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

class AddComponent
{

    public void SetModel(Shader _Shader, Camera _Camera, Model _Model, Vector3 _PosModel, Matrix4 _MatrixModel)
    {
        _Shader.UseShader();
        _Shader.SetMatrix4("model", _MatrixModel);
        _Shader.SetMatrix4("view", _Camera.GetView());
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        _Shader.SetVector3("objColor", new OpenTK.Mathematics.Vector3(0.0f, 0.1f, 0.31f));
        _Shader.SetVector3("lightColor", new OpenTK.Mathematics.Vector3(3.0f, 4.0f, 1.0f));
        _Shader.SetVector3("lightPosition", _PosModel);
        _Shader.SetVector3("lightView", _Camera._Position);
        _Model.Draw(_Shader);
    }

    public void SetLight(Shader _Shader, Camera _Camera,Model _Ligth, Vector3 _LampPos, Matrix4 _Model)
    {
        
        _Shader.UseShader();
        _Shader.SetMatrix4("model", _Model);
        _Shader.SetMatrix4("view", _Camera.GetView());
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        

        _Ligth.Draw(_Shader);
    }
}
