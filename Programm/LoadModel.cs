using System;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using MeshC = MeshComponent;

class Load
{
    private List<float> _Vertex = new List<float>();
    private List<float> _Normal = new List<float>();
    private List<float> _TexCoord = new List<float>();  
    private List<int> _Index = new List<int>();
    private List<MeshC> _MeshComp = new List<MeshC>();


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
        
       

        for (int i = 0;i<_Mesh.VertexCount;i++)
        {

            //Vertex load
            Console.WriteLine("Load Vertex:X");
            _Vertex.Add(_Mesh.Vertices[i].X);
            Console.WriteLine("Load Vertex:Y");
            _Vertex.Add(_Mesh.Vertices[i].Y);
            Console.WriteLine("Load Vertex:Z");
            _Vertex.Add(_Mesh.Vertices[i].Z);
            Console.WriteLine("Load vertex done!");        

            if (_Mesh.HasNormals)
            {
                //Normal load
                Console.WriteLine("Load Normal:X");
                _Normal.Add(_Mesh.Normals[i].X);
                Console.WriteLine("Load Normal:Y");
                _Normal.Add(_Mesh.Normals[i].Y);
                Console.WriteLine("Load Normal:Z");
                _Normal.Add(_Mesh.Normals[i].Z);
                Console.WriteLine("Load normal done!");
            }
            if (_Mesh.HasTextureCoords(0))
            {
                //TexCoordd load

                _TexCoord.Add(_Mesh.TextureCoordinateChannels[0][i].X);
                _TexCoord.Add(_Mesh.TextureCoordinateChannels[0][i].Y);

            }
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

        Console.WriteLine("Output data from model. Vertex count:"+_Mesh.VertexCount+". Index count:"+_Mesh.FaceCount);
    }

    //Draw model system
    public void Draw(Shader _Shader, Camera _Camera)
    {
        for (int i = 0; i<_MeshComp.Count(); i++)
        {
            _MeshComp[i].DrawMesh(_Shader,_Camera);
        }
    }
}
