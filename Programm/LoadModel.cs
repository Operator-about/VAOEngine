using System;
using Assimp;
using Assimp.Unmanaged;
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
        

        LoadModelInWorld(_Shader);
    }

    private void LoadModelInWorld( Shader _Shader)
    {
        List<VectorModel> _Vertex = new List<VectorModel>();
        List<int> _Index = new List<int>();
        Mesh _Mesh = new Mesh();
       

        for (int i = 0;i<_Mesh.VertexCount;i++)
        {
            Vector3 _VertexOther;
            VectorModel _LocalVertex = new VectorModel();

            //Vertex load
            _VertexOther.X = _Mesh.Vertices[i].X;
            _VertexOther.Y = _Mesh.Vertices[i].Y;   
            _VertexOther.Z = _Mesh.Vertices[i].Z;
            _LocalVertex._Position = _VertexOther;

            if (_Mesh.HasNormals)
            {
                //Normal load
                _VertexOther.X = _Mesh.Normals[i].X;
                _VertexOther.Y = _Mesh.Normals[i].Y; 
                _VertexOther.Z = _Mesh.Normals[i].Z;
                _LocalVertex._Normal = _VertexOther;
            }
            if (_Mesh.HasTextureCoords(0))
            {
                //TexCoordd load
                Vector2 _TextureCoord;
                _TextureCoord.X = _Mesh.TextureCoordinateChannels[0][i].X;
                _TextureCoord.Y = _Mesh.TextureCoordinateChannels[0][i].Y;
                _LocalVertex._TexCoord = _TextureCoord;
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
        _VBO = GL.GenBuffer();
        _VAO = GL.GenBuffer();
        _EBO = GL.GenBuffer();

        _VAO = GL.GenVertexArray();
        GL.BindVertexArray(_VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VAO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);

        //Report data in buffer
        GL.BufferData(BufferTarget.ArrayBuffer,0,_Vertex.Count(),BufferUsageHint.StaticDraw);
        GL.BufferData(BufferTarget.ElementArrayBuffer,0,_Index.Count(),BufferUsageHint.StaticDraw);

        //Position
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,6*sizeof(float),0);
        GL.EnableVertexAttribArray(0);
        
        //Normal
        GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),6*sizeof(float));
        GL.EnableVertexAttribArray(1);

        //Color
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        //Shader
        _Shader.UseShader();
       
    }

    //Draw model system
    public void Draw(Shader _Shader, Camera _Camera)
    {
        

        _Shader.UseShader();
        GL.BindBuffer(BufferTarget.ArrayBuffer,_VAO);

        Matrix4 _Model = new Matrix4();
        OpenTK.Mathematics.Vector3 _PosModel = new OpenTK.Mathematics.Vector3(0.0f, 0.0f, 0.0f);
        GL.UniformMatrix4(GL.GetAttribLocation(_Shader._Count, "model"), false, ref _Model);

        GL.Uniform3(GL.GetAttribLocation(_Shader._Count,"camPos"),_Camera._Position.X, _Camera._Position.Y, _Camera._Position.Z);
        _Camera.SetCameraMatrix(_Shader,"camMatrix");

        GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, _EBO, DrawElementsType.UnsignedInt, 0);
    }
  
}
