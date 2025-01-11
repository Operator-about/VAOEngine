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
    private float _Speed = 0.05f;
    public OpenTK.Mathematics.Vector3 _Position { get; set; }
    public OpenTK.Mathematics.Vector3 _UpDefult = OpenTK.Mathematics.Vector3.UnitY;
    public OpenTK.Mathematics.Vector3 _FrontDefult = -OpenTK.Mathematics.Vector3.UnitZ;
    public OpenTK.Mathematics.Vector3 _RightDefult = OpenTK.Mathematics.Vector3.UnitX;

    public int _Width, _Hegth;
    public float _YawDefult = -MathHelper.PiOver2;
    public float _PitchDefult;
    public float _Sentensity = 0.2f;
    public float _Aspect { private get; set; }
    public bool _First = true;
    public float _FOVDefult = MathHelper.PiOver2;
    private OpenTK.Mathematics.Vector2 _LastPos;

    public OpenTK.Mathematics.Vector3 _Front => _FrontDefult;
    public OpenTK.Mathematics.Vector3 _Up => _UpDefult;
    public OpenTK.Mathematics.Vector3 _Right => _RightDefult;


    
    

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
        get => MathHelper.RadiansToDegrees(_YawDefult);
        set
        {
            _YawDefult = MathHelper.DegreesToRadians(value);
            UpdateVector();
        }
    }

    public float _FOV
    {
        get => MathHelper.RadiansToDegrees(_FOVDefult);
        set
        {
            var _Angle = MathHelper.Clamp(value, 1f, 90f);
            _FOVDefult = MathHelper.DegreesToRadians(_Angle);
        }
    }





    public CameraSystem(OpenTK.Mathematics.Vector3 _Pos, float _Asp)
    {
        _Position = _Pos;
        _Aspect = _Asp;
    }

    

    public void InputCameraSystem(KeyboardState _Key, MouseState _Mouse,CursorState _Cursor)
    {

        
        _Cursor = CursorState.Grabbed;
        


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

            _Position -= _Right * _Speed;
            Console.WriteLine("Output: A, Position(X):" + _Position.X + " (Y):" + _Position.Y + " (Z):" + _Position.Z);
        }
        if (_Key.IsKeyDown(Keys.D))
        {
            
            _Position += _Right * _Speed;
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

        if (_Mouse.IsButtonDown(MouseButton.Left))
        {
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

                _DeltaX *= _Sentensity;
                _DeltaY *= _Sentensity;

                _Yaw += _DeltaX;
                _Pitch -= _DeltaY;
            }
        }
        if (_Mouse.IsButtonReleased(MouseButton.Left))
        {
            _First = true;
            _Cursor = CursorState.Normal;
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
        _UpDefult = OpenTK.Mathematics.Vector3.Normalize(OpenTK.Mathematics.Vector3.Cross(_RightDefult, _FrontDefult));
    }
}
