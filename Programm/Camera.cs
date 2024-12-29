using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shader = ShaderSystem;


class CameraSystem
{
    private float _Speed = 10.0f;
    public OpenTK.Mathematics.Vector3 _Position = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 0.0f);
    public OpenTK.Mathematics.Vector3 _Up = OpenTK.Mathematics.Vector3.UnitY;
    public OpenTK.Mathematics.Vector3 _Front = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, -1.0f);
    private Matrix4 _CameraMatrix = new Matrix4();


    public float _FOV;
    public int _Width, _Hegth;
    public Matrix4 _View;
    public float _Yaw = -90.0f;
    public float _Pitch = 0.0f;
    public float _Fov = 45.0f;

    public void CameraStartup()
    { 
        OpenTK.Mathematics.Vector3 _Target = OpenTK.Mathematics.Vector3.Zero;
        OpenTK.Mathematics.Vector3 _Direction = OpenTK.Mathematics.Vector3.Normalize(_Position-_Target);

        OpenTK.Mathematics.Vector3 _Right = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Up, _Direction));

        UpdateVector();
        
    }

    public void UpdateVector()
    {
        _Front = new OpenTK.Mathematics.Vector3(
            MathF.Cos(MathHelper.DegreesToRadians(_Yaw) * MathF.Cos(MathHelper.DegreesToRadians(_Pitch))),
            MathF.Sin(MathHelper.DegreesToRadians(_Pitch)),
            MathF.Sin(MathHelper.DegreesToRadians(_Yaw) * MathF.Cos(MathHelper.DegreesToRadians(_Pitch)))
        );

        _Front.Normalize(); 
    }

    public void InputCameraSystem(KeyboardState _Key)
    {
        if (_Key.IsKeyDown(Keys.W))
        {
            
            _Position += _Front * _Speed;
            Console.WriteLine("Output: W, Position(X):" + _Position.X + " (Y):" + _Position.Y + " (Z):" + _Position.Z);
        }
        if (_Key.IsKeyDown(Keys.S))
        {
            
            _Position -= _Front * _Speed;
            Console.WriteLine("Output: S, Position(X):" + _Position.X + " (Y):" + _Position.Y + " (Z):" + _Position.Z);
        }
        if (_Key.IsKeyDown(Keys.A))
        {
            
            _Position -= OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Front,_Up)) * _Speed;
            Console.WriteLine("Output: A, Position(X):" + _Position.X + " (Y):" + _Position.Y + " (Z):" + _Position.Z);
        }
        if (_Key.IsKeyDown(Keys.D))
        {
            
            _Position += OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Front, _Up)) * _Speed;
            Console.WriteLine("Output: D, Position(X):" + _Position.X + " (Y):" + _Position.Y + " (Z):" + _Position.Z);
        }
        if (_Key.IsKeyDown(Keys.Space))
        {
            _Position += _Up * _Speed;
        }
        if (_Key.IsKeyDown(Keys.Space))
        {
            _Position -= _Up * _Speed;
        }
    }

    public void SetCameraMatrix(Shader _Shader, string _NameParameterInShader)
    {
        GL.UniformMatrix4(GL.GetAttribLocation(_Shader._Count,_NameParameterInShader),false, ref _CameraMatrix);
    }

    public object UpdateCameraMatrix(int _WindowWidth, int _WindowHeigth, float _FOVl, float _NearePlane, float _FarPlane, Shader _Shader)
    {
        _Width = _WindowWidth;
        _Hegth = _WindowHeigth;

        Matrix4 _Model = Matrix4.Identity;
        Matrix4 _View = Matrix4.LookAt(_Position, _Position + _Front, _Up);
        Matrix4 _Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_FOVl), (float)(_WindowWidth / _WindowHeigth), _NearePlane, _FarPlane);
        GL.UniformMatrix4(GL.GetUniformLocation(_Shader._Count, "model"), false, ref _Model);
        GL.UniformMatrix4(GL.GetUniformLocation(_Shader._Count, "view"), false, ref _View);
        GL.UniformMatrix4(GL.GetUniformLocation(_Shader._Count, "projection"), false, ref _Projection);

        _CameraMatrix = _View * _Projection;
        SetCameraMatrix(_Shader, "camMatrix");

        return _CameraMatrix;
    }
}
