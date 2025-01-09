using System;
using System.IO;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using MeshC = MeshComponent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class Convert
{
    public static Vector3 ConvertToAssimpVec3(this Vector3D _AssimpVector)
    {
        return Unsafe.As<Vector3D, Vector3>(ref _AssimpVector);
    }
}



public class Load
{
    
    private List<MeshC> _MeshComp;
    string _Direction;
    private Shader _Shader;

    public Load(string _Path)
    {
        LoadModelFromFile(_Path);
    }

    private void LoadModelFromFile(string _Path)
    {
     
        AssimpContext _Import = new AssimpContext();
        Scene _Scene = _Import.ImportFile(_Path,PostProcessSteps.Triangulate);

        _MeshComp = new List<MeshC>();

        _Direction = _Path.Substring(0,_Path.LastIndexOf('/'));

        Node(_Scene.RootNode, _Scene);
    }

   
    private void Node(Node _Node, Scene _Scene)
    {
        for (int i = 0; i<_Node.MeshCount;i++)
        {
            Mesh _Mesh = _Scene.Meshes[_Node.MeshIndices[i]];
            LoadModelInWorld(_Mesh,_Scene);
        }
        for (int i = 0; i<_Node.ChildCount;i++)
        {
            Node(_Node.Children[i],_Scene);
        }
    }

    private MeshC LoadModelInWorld(Mesh _Mesh,Scene _Scene)
    {


        List<VertexMesh> _VertexG = new List<VertexMesh>();
        List<int> _Index = new List<int>();

        for (int i = 0;i<_Mesh.VertexCount;i++)
        {

            VertexMesh _Vertex = new VertexMesh();

            _Vertex._Position = _Mesh.Vertices[i].ConvertToAssimpVec3();

            if (_Mesh.HasNormals)
            {
                _Vertex._Normal = _Mesh.Normals[i].ConvertToAssimpVec3();
            }

            _VertexG.Add(_Vertex);
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

        return new MeshC(CollectionsMarshal.AsSpan(_VertexG), CollectionsMarshal.AsSpan(_Index));

    }

    

    //Draw model system
    public void Draw()
    {
        foreach (MeshC _Mesh in _MeshComp)
        {
            _Mesh.DrawMesh();
        }
    }
}
