using System;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using Camera = CameraSystem;



struct VectorModel
{
    public Vector3 _Position;
    public Vector3 _Normal;
    public Vector2 _TexCoord;
}


class Load
{
    private int _VBO, _VAO, _EBO;

    public void LoadModelFromFile(string _Path, Shader _Shader)
    {
     
        AssimpContext _Import = new AssimpContext();
        Scene _Scene = _Import.ImportFile(_Path,PostProcessSteps.Triangulate|PostProcessSteps.FlipUVs);

        Node(_Scene.RootNode, _Scene,_Shader);
    }

   
    private void Node(Node _Node, Scene _Scene,Shader _Shader)
    {
        for (int i = 0; i<_Node.MeshCount;i++)
        {
            Mesh _Mesh = _Scene.Meshes[_Node.MeshIndices[i]];
            LoadModelInWorld(_Mesh,_Shader,_Scene);
        }
        for (int i = 0; i<_Node.ChildCount;i++)
        {
            Node(_Node.Children[i],_Scene,_Shader);
        }
    }

    private void LoadModelInWorld(Mesh _Mesh,Shader _Shader,Scene _Scene)
    {
        List<VectorModel> _Vertex = new List<VectorModel>();
        List<int> _Index = new List<int>();
       

        for (int i = 0;i<_Mesh.VertexCount;i++)
        {
            Vector3 _VertexOther;
            VectorModel _LocalVertex = new VectorModel();

            //Vertex load
            Console.WriteLine("Load Vertex:X");
            _VertexOther.X = _Mesh.Vertices[i].X;
            Console.WriteLine("Load Vertex:Y");
            _VertexOther.Y = _Mesh.Vertices[i].Y;
            Console.WriteLine("Load Vertex:Z");
            _VertexOther.Z = _Mesh.Vertices[i].Z;
            _LocalVertex._Position = _VertexOther;
            Console.WriteLine("Load vertex done!");        

            if (_Mesh.HasNormals)
            {
                //Normal load
                Console.WriteLine("Load Normal:X");
                _VertexOther.X = _Mesh.Normals[i].X;
                Console.WriteLine("Load Normal:Y");
                _VertexOther.Y = _Mesh.Normals[i].Y;
                Console.WriteLine("Load Normal:Z");
                _VertexOther.Z = _Mesh.Normals[i].Z;
                _LocalVertex._Normal = _VertexOther;
                Console.WriteLine("Load normal done!");
            }
            if (_Mesh.HasTextureCoords(0))
            {
                //TexCoordd load
                Vector2 _TextureCoord;
                _TextureCoord.X = _Mesh.TextureCoordinateChannels[0][i].X;
                _TextureCoord.Y = _Mesh.TextureCoordinateChannels[0][i].Y;
                _LocalVertex._TexCoord = _TextureCoord;
            }
            else
            {
                _LocalVertex._TexCoord.X = 0;
                _LocalVertex._TexCoord.Y = 0;
            }

            //Load all data in model
            _Vertex.Add(_LocalVertex);
        }

        //Load Index
        for (int i = 0; i<_Mesh.FaceCount;i++)
        {
            Face _Face = _Mesh.Faces[i];
            for (int j = 0;j<_Face.IndexCount;j++)
            {
                _Index.Add(_Face.Indices[j]);
            }
        }



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
        GL.BufferData(BufferTarget.ArrayBuffer,0,_Vertex.Count(),BufferUsageHint.StaticDraw);
        Console.WriteLine("Report data in buffer: Index");
        GL.BufferData(BufferTarget.ElementArrayBuffer,0,_Index.Count(),BufferUsageHint.StaticDraw);

        //Position
        Console.WriteLine("Load Attribute: 0,3");
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,6*sizeof(float),0);
        GL.EnableVertexAttribArray(0);

        //Normal
        Console.WriteLine("Load Attribute: 1,3");
        GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),6*sizeof(float));
        GL.EnableVertexAttribArray(1);

        //Color
        Console.WriteLine("Load Attribute: 2,3");
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        Console.WriteLine("Output data from model. Vertex count:"+_Mesh.VertexCount+". Index count:"+_Mesh.FaceCount);
        //Shader
        _Shader.UseShader();
       
    }

    //Draw model system
    public void Draw(Shader _Shader, Camera _Camera)
    {
        _Shader.UseShader();
        GL.BindBuffer(BufferTarget.ArrayBuffer,_VAO);

        
        
        OpenTK.Mathematics.Vector3 _PosModel = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 0.0f);
        


        GL.Uniform3(GL.GetUniformLocation(_Shader._Count,"camPos"),_Camera._Position.X, _Camera._Position.Y, _Camera._Position.Z);
       

        GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _EBO, DrawElementsType.UnsignedInt, 0);
    }
  
}
