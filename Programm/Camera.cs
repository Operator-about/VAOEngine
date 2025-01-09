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
using OpenTK.Windowing.Common;


public class CameraSystem
{
    private float _Speed = 0.5f;
    public OpenTK.Mathematics.Vector3 _Position = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 0.0f);
    public OpenTK.Mathematics.Vector3 _UpDefult = OpenTK.Mathematics.Vector3.UnitY;
    public OpenTK.Mathematics.Vector3 _FrontDefult = -OpenTK.Mathematics.Vector3.UnitZ;
    public OpenTK.Mathematics.Vector3 _RightDefult = -OpenTK.Mathematics.Vector3.UnitX;

    public int _Width, _Hegth;
    public float _YawDefult = -MathHelper.PiOver2;
    public float _PitchDefult;
    public float _Sentensity = 0.2f;
    public float _Aspect;
    public bool _First = true;


    public float _FOVDefult = MathHelper.PiOver2;
    public float _FOV
    {
        get => MathHelper.RadiansToDegrees(_FOVDefult);
        set
        { 
            var _Angle = MathHelper.Clamp(value, 1f, 90f);
            _FOVDefult = MathHelper.DegreesToRadians(_Angle);
        }
    }

    public float _Pitch
    {
        get => MathHelper.RadiansToDegrees(_PitchDefult);
        set
        {
            var _Angle = MathHelper.Clamp(value, -89f,89f);
            _PitchDefult = MathHelper.DegreesToRadians(_Angle);
            UpdateVector();
        }
    }

    public float _Yaw
    {
        get => MathHelper.RadiansToDegrees(_PitchDefult);
        set
        {
            _PitchDefult = MathHelper.DegreesToRadians(value);
            UpdateVector();
        }
    }

    public OpenTK.Mathematics.Vector3 _Front => _FrontDefult;
    public OpenTK.Mathematics.Vector3 _Up => _UpDefult;
    public OpenTK.Mathematics.Vector3 _Right => _RightDefult;



    public CameraSystem(OpenTK.Mathematics.Vector3 _Pos, float _Asp)
    {
        _Position = _Pos;
        _Aspect = _Asp;
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

        

    }

    

    public Matrix4 GetView()
    {
        return Matrix4.LookAt(_Position, _Position + _FrontDefult, _UpDefult);
    }

    public Matrix4 GetProjection()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_FOVDefult, _Aspect, 0.01f, 100.0f);
    }


    private void UpdateVector()
    {
        _FrontDefult.X = MathF.Cos(_PitchDefult) * MathF.Cos(_YawDefult);
        _FrontDefult.Y = MathF.Sin(_PitchDefult);
        _FrontDefult.X = MathF.Cos(_PitchDefult) * MathF.Sin(_YawDefult);

        _FrontDefult = OpenTK.Mathematics.Vector3.Normalize(_FrontDefult);

        _RightDefult = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_FrontDefult, OpenTK.Mathematics.Vector3.UnitY));
        _UpDefult = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_Right, _FrontDefult));
    }
}
