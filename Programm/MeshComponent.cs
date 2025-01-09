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
    private List<Texture> _TextureList;

    

    public void DrawMesh(Shader _Shader)
    {
        GL.BindVertexArray(_VAO);

        int _DiffuseNr = 1;
        int _SpecularNr = 1;
        int _NormalNr = 1;
        int _HeigthNr = 1;

        for (int i = 0; i<_TextureList.Count();i++)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + i);

            string _Number = new string("0");
            string _Name = _TextureList[i]._Type;
            if (_Name== "texture_diffuse")
            {
                _Number = new string(""+_DiffuseNr+"");
            }
            else if (_Name=="texture_specular")
            {
                _Number = new string("" + _SpecularNr + "");
            }
            else if (_Name=="texture_normal")
            {
                _Number = new string("" + _NormalNr + "");
            }
            else if (_Name== "texture_height")
            {
                _Number = new string("" + _HeigthNr + "");
            }

            GL.Uniform1(GL.GetUniformLocation(_Shader._Count,(_Name+_Number)),1);
            GL.BindTexture(TextureTarget.Texture2D, _TextureList[i]._Count);
        }

        

        GL.DrawElements(PrimitiveType.Triangles, _IndexG, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

        GL.ActiveTexture(TextureUnit.Texture0);
    }

    public MeshComponent(Span<VertexMesh> _Vertex, Span<int> _Index, List<Texture> _Tex)
    {
        this._TextureList = _Tex;   
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

        Console.WriteLine("Load Attribute: 2,2");
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2,2,VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._TexCoord)));
        

        GL.BindVertexArray(0);
    }
}


