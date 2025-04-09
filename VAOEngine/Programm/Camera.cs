using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using System.Windows.Input;
using System.Drawing;
using System.ComponentModel.DataAnnotations;


public class CameraSystem
{
    private float _Speed = 0.05f;
    public Vector3 _Position { get; set; }
    public Vector3 _UpDefult = Vector3.UnitY;
    public Vector3 _FrontDefult = -Vector3.UnitZ;
    public Vector3 _RightDefult = Vector3.UnitX;

    private float _YawDefult = -MathHelper.PiOver2;
    private float _PitchDefult;
    public float _Sentensity = 0.2f;
    public float _Aspect { private get; set; }
    public bool _First = true;
    private float _FOVDefult = MathHelper.PiOver2;
    public Vector2 _LastPos;

    private Vector3 _Front => _FrontDefult;
    private Vector3 _Up => _UpDefult;
    private Vector3 _Right => _RightDefult;


    
    

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



    public void InputCameraSystem()
    {

        if (Keyboard.IsKeyDown(Key.W))
        {
            _Position += _Front * _Speed;
        }
        if (Keyboard.IsKeyDown(Key.S))
        {
            _Position -= _Front * _Speed;
        }
        if (Keyboard.IsKeyDown(Key.A))
        { 
            _Position -= _Right * _Speed;
        }
        if (Keyboard.IsKeyDown(Key.D))
        { 
            _Position += _Right * _Speed;
        }
        if (Keyboard.IsKeyDown(Key.Space))
        {
            _Position += _Up * _Speed;
        }
        if (Keyboard.IsKeyDown(Key.Space))
        {
            _Position -= _Up * _Speed;
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

        _FrontDefult = Vector3.Normalize(_FrontDefult);

        _RightDefult = Vector3.Normalize(Vector3.Cross(_FrontDefult, Vector3.UnitY));
        _UpDefult = Vector3.Normalize(Vector3.Cross(_RightDefult, _FrontDefult));
    }
}
