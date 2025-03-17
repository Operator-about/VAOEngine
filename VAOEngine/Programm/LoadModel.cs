using Assimp;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using MeshC = MeshComponent;
using CameraSys = CameraSystem;
using System.Runtime.CompilerServices;


public static class Convert
{
    public static Vector3 ConvertToAssimpVec3(this Vector3D _AssimpVector)
    {
        return Unsafe.As<Vector3D, Vector3>(ref _AssimpVector);
    }
    public static Matrix4 ConvertToAssimpMat4(this Matrix4x4 _AssimpMatrix)
    {
        return Unsafe.As<Matrix4x4, Matrix4>(ref _AssimpMatrix);
    }
}



public class Load
{

    private List<MeshC> _MeshComp = new List<MeshC>();
    public MeshC _OutModel;

    public Load(string _Path)
    {

        LoadModelFromFile(_Path);
    }

    private void LoadModelFromFile(string _Path)
    {

        AssimpContext _Import = new AssimpContext();
        Scene _Scene = _Import.ImportFile(_Path, PostProcessSteps.Triangulate);

        Node(_Scene.RootNode, _Scene);

        _Import.Dispose();
    }


    private void Node(Node _Node, Scene _Scene)
    {
        for (int i = 0; i < _Node.MeshCount; i++)
        {
            Mesh _Mesh = _Scene.Meshes[_Node.MeshIndices[i]];
            _MeshComp.Add(LoadModelInWorld(_Mesh, _Scene));
        }
        for (int i = 0; i < _Node.ChildCount; i++)
        {
            Node(_Node.Children[i], _Scene);
        }
    }

    private MeshC LoadModelInWorld(Mesh _Mesh, Scene _Scene)
    {


        List<VertexMesh> _VertexG = new List<VertexMesh>();
        List<int> _Index = new List<int>();

        for (int i = 0; i < _Mesh.VertexCount; i++)
        {

            VertexMesh _Vertex = new VertexMesh();

            _Vertex._Position = _Mesh.Vertices[i].ConvertToAssimpVec3();

            if (_Mesh.HasNormals)
            {
                _Vertex._Normal = _Mesh.Normals[i].ConvertToAssimpVec3();
            }
            if (_Mesh.HasTextureCoords(0))
            {
                Vector2 _Vec;
                _Vec.X = _Mesh.TextureCoordinateChannels[0][i].X;
                _Vec.Y = _Mesh.TextureCoordinateChannels[0][i].Y;
                _Vertex._TexCoord = _Vec;
            }
            else
            {

            }


            _VertexG.Add(_Vertex);
        }

        //Load Index
        for (int i = 0; i < _Mesh.FaceCount; i++)
        {
            Face _Face = _Mesh.Faces[i];
            for (int j = 0; j < _Face.IndexCount; j++)
            {
                _Index.Add(_Face.Indices[j]);
            }
        }


        Console.WriteLine($"Count value:{_MeshComp.Count}");
        return new MeshC(_VertexG.ToArray(), _Index.ToArray());

    }




    //Draw model system
    public void Draw(Shader _Shader, CameraSys _Camera)
    {
        foreach (MeshC _Mesh in _MeshComp)
        {
            _OutModel = _Mesh;
            _OutModel.DrawMesh(_Shader, _Camera);
        }
    }


}