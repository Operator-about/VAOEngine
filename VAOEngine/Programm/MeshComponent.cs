using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



public struct MeshPosScaleRot
{
    public Vector3 _Position;
    public Vector3 _Scale;
    public Vector3 _Rotation;
    public Vector3 _Color;
}


public struct VertexMesh
{
    public Vector3 _Position;
    public Vector3 _Normal;
    public Vector2 _TexCoord;
}


public class MeshComponent 
{

    private readonly int _VAO;
    private readonly int _IndexG;
    public MeshPosScaleRot _MatrixModel;

    public void DrawMesh(Shader _Shader, Camera _Camera)
    {
        //Bind VAO
        GL.BindVertexArray(_VAO);

        _MatrixModel._Position = new Vector3(0,0,0);
        _MatrixModel._Scale = new Vector3(1,1,1);
        _MatrixModel._Rotation = new Vector3(1,1,1);
        _MatrixModel._Color = new Vector3(1,1,1);

        //Set matrix for model
        var _ModelMatrixF = Matrix4.Identity;
        var _PositionModelF = Matrix4.CreateTranslation(_MatrixModel._Position);
        var _ScaleMatrixModelF = Matrix4.CreateScale(_MatrixModel._Scale);
        var _MatrixX = Matrix4.CreateRotationX(_MatrixModel._Rotation.X);
        var _MatrixY = Matrix4.CreateRotationY(_MatrixModel._Rotation.Y);
        var _MatrixZ = Matrix4.CreateRotationZ(_MatrixModel._Rotation.Z);

        _ModelMatrixF = _PositionModelF * _ScaleMatrixModelF * (_MatrixX * _MatrixY * _MatrixZ);

        //Use shader and bind
        _Shader.UseShader();
        _Shader.SetMatrix4("model", _ModelMatrixF);
        _Shader.SetMatrix4("view", _Camera.GetView());
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        _Shader.SetVector3("objColor", _MatrixModel._Color);
        

        //Draw
        GL.DrawElements(PrimitiveType.Triangles, _IndexG, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);     
    }

    public MeshComponent(Span<VertexMesh> _Vertex, Span<int> _Index)
    {
        //this._TextureList = _Tex;   
        _IndexG = _Index.Length;


        //Bind
        _VAO = GL.GenVertexArray();
        int _VBO = GL.GenBuffer();
        int _EBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);



        //Report data in buffer
        Console.WriteLine("Report data in buffer: Vertex");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _Vertex.Length * Unsafe.SizeOf<VertexMesh>(), ref MemoryMarshal.GetReference(_Vertex), BufferUsageHint.StaticDraw);

        Console.WriteLine("Report data in buffer: Index");
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _Index.Length * sizeof(int), ref MemoryMarshal.GetReference(_Index), BufferUsageHint.StaticDraw);

        //Position
        Console.WriteLine("Load Attribute: 0,3");
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Position)));


        //Normal
        Console.WriteLine("Load Attribute: 1,3");
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Normal)));


        //TexCoord
        //GL.EnableVertexAttribArray(2);
        //GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._TexCoord)));

        GL.BindVertexArray(0);
    }
}



