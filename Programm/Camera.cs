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
using OpenTK.Windowing.Common;


public class CameraSystem
{
    private float _Speed = 10.0f;
    public OpenTK.Mathematics.Vector3 _Position = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 0.0f);
    public OpenTK.Mathematics.Vector3 _Up = OpenTK.Mathematics.Vector3.UnitY;
    public OpenTK.Mathematics.Vector3 _Front = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, -1.0f);
    public Matrix4 _CameraMatrix = new Matrix4();

    public OpenTK.Mathematics.Vector3 _Target = OpenTK.Mathematics.Vector3.Zero;
    public OpenTK.Mathematics.Vector3 _Direction;
    public OpenTK.Mathematics.Vector3 _CameraRight;
    public OpenTK.Mathematics.Vector3 _CameraUp;

    public OpenTK.Mathematics.Vector3 _Right;

    public float _FOV;
    public int _Width, _Hegth;
    public Matrix4 _View;
    public float _Yaw = -90.0f;
    public float _Pitch = 0.0f;
    public float _Sentensity = 90.0f;
    public bool _First = true;
   

    public object CameraStartup(float _FOVLoc, int _WidthL, int _HegthL)
    {
        _Direction = OpenTK.Mathematics.Vector3.Normalize(_Position - _Target);
        _Right = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Up, _Direction));
        _CameraRight = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Up, _CameraRight));
        _CameraUp = OpenTK.Mathematics.Vector3.Cross(_Direction, _CameraRight);
        _View = Matrix4.LookAt(_Position, _Position + _Front, _Up);

        _FOV = _FOVLoc;
        _Width = _WidthL;
        _Hegth = _HegthL;

        return (_FOV, _Width, _Hegth);
    }

    

    public void InputCameraSystem(KeyboardState _Key, MouseState _Mouse)
    {

        
        CursorState _Cursor = CursorState.Grabbed;
        OpenTK.Mathematics.Vector2 _LastPos = new OpenTK.Mathematics.Vector2();


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

        if (_First)
        {
            _LastPos = new OpenTK.Mathematics.Vector2(_Mouse.X, _Mouse.Y);
            _First = false;
        }
        else
        {
            float _DeltaX = _Mouse.X - _LastPos.X;
            float _DeltaY = _Mouse.Y - _LastPos.Y;
            _LastPos = new OpenTK.Mathematics.Vector2(_Mouse.X, _Mouse.Y);

            _Yaw += _DeltaX * _Sentensity;
            if (_Pitch>89.0f)
            {
                _Pitch = 89.0f;
            }
            else if (_Pitch > -89.0f)
            {
                _Pitch = -89.0f;
            }
            else
            {
                _Pitch -= _DeltaX * _Sentensity;
            }
        }

        _Front.X = (float)Math.Sin(MathHelper.DegreesToRadians(_Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_Yaw));
        _Front.Z = (float)Math.Sin(MathHelper.DegreesToRadians(_Pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(_Yaw));
        _Front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(_Pitch));

        _Front = OpenTK.Mathematics.Vector3.Normalize(_Front);

    }

    public void SetCameraMatrix(Shader _Shader, string _NameParameterInShader)
    {
        GL.UniformMatrix4(GL.GetAttribLocation(_Shader._Count,_NameParameterInShader),false, ref _CameraMatrix);
    }

    public Matrix4 GetView()
    {
        return Matrix4.LookAt(_Position, _Position + _Front, _Up);
    }

    public Matrix4 GetProjection()
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_FOV), (float)(_Width / _Hegth), 0.1f, 100.0f);
    }
}
