using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Shader = ShaderSystem;
using Texture = TextureSystem;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public struct MeshPosScaleRot
{
    public Vector3 _Position;
    public Vector3 _Scale;
    public Vector3 _Rotation;
}


public struct VertexMesh
{
    public Vector3 _Position;
    public Vector3 _Normal;
    //public Vector2 _TexCoord;
}


public class MeshComponent 
{

    private readonly int _VAO;
    private readonly int _IndexG;
    

    

    public void DrawMesh(Shader _Shader)
    {
        GL.BindVertexArray(_VAO);



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
        GL.BufferData(BufferTarget.ArrayBuffer, _Vertex.Length * Unsafe.SizeOf<VertexMesh>(),ref MemoryMarshal.GetReference(_Vertex), BufferUsageHint.StaticDraw);
        
        Console.WriteLine("Report data in buffer: Index");
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _Index.Length * sizeof(int),ref MemoryMarshal.GetReference(_Index), BufferUsageHint.StaticDraw);

        //Position
        Console.WriteLine("Load Attribute: 0,3");
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Position)));
        

        //Normal
        Console.WriteLine("Load Attribute: 1,3");
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Normal)));

        //Console.WriteLine("Load Attribute: 2,2");
        //GL.EnableVertexAttribArray(2);
        //GL.VertexAttribPointer(2,2,VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._TexCoord)));
        

        GL.BindVertexArray(0);
    }
}


