using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using Camera = CameraSystem;

public class MeshComponent 
{ 

    private int _VBO, _VAO, _EBO;

    List<Vector3> _Vertex = new List<Vector3>();
    List<int> _Index = new List<int>();
    

    public MeshComponent(List<Vector3> _VertexLocal, List<int> _IndexLocal)
    {
        this._Vertex = _VertexLocal;
        this._Index = _IndexLocal;

        ActivMesh();
    }

    public void DrawMesh(Shader _Shader, Camera _Camera)
    {
        _Shader.UseShader();
        GL.BindVertexArray(_VAO);

        _Camera.SetCameraMatrix(_Shader, "camMatrix");

        GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _Index.Count(), DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    private void ActivMesh()
    {
        


        //Bind
        Console.WriteLine("Gen VAO");
        _VAO = GL.GenBuffer();
        Console.WriteLine("Gen VBO");
        _VBO = GL.GenBuffer();
        Console.WriteLine("Gen EBO");
        _EBO = GL.GenBuffer();

        Console.WriteLine("Gen VAO");
        _VAO = GL.GenVertexArray();
        GL.BindVertexArray(_VAO);

        Console.WriteLine("Bind buffer: VAO");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VAO);
        Console.WriteLine("Bind buffer: VBO");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        Console.WriteLine("Bind buffer: EBO");
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);

        //Report data in buffer
        Console.WriteLine("Report data in buffer: Vertex");
        GL.BufferData(BufferTarget.ArrayBuffer, 0, _Vertex.Count * sizeof(float), BufferUsageHint.StaticDraw);
        Console.WriteLine("Report data in buffer: Index");
        GL.BufferData(BufferTarget.ElementArrayBuffer, 0, _Index.Count * sizeof(float), BufferUsageHint.StaticDraw);

        //Position
        Console.WriteLine("Load Attribute: 0,3");
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        //Normal
        Console.WriteLine("Load Attribute: 1,3");
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        //Color
        Console.WriteLine("Load Attribute: 2,3");
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(2);
    }
}


