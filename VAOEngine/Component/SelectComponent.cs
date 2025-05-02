using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class SelectComponent
{

    private Vector3 _StartL, _EndL;

    public Vector3 _Start { get { return _StartL; } }
    public Vector3 _End { get { return _EndL; } }   

    public SelectComponent(int _X, int _Y, int _Width, int _Height)
    {
        int[] _Viewport = new int[4];
        Matrix4 _Model, _Projection;
        GL.GetFloat(GetPName.Modelview0MatrixExt, out _Model);
        GL.GetFloat(GetPName.TransposeProjectionMatrix, out _Projection);
        GL.GetInteger(GetPName.Viewport, _Viewport);
        var _AllMatrix = _Model * _Projection;

        _StartL = Vector3.Unproject(new Vector3(_X, _Y, 0), _X, _Y, _Width, _Height, _Viewport[2], _Viewport[3], _AllMatrix);
        _EndL = Vector3.Unproject(new Vector3(_X, _Y, 1.0f), _X, _Y, _Width, _Height, _Viewport[2], _Viewport[3], _AllMatrix);
    }

    

}

