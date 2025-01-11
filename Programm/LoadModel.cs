using System;
using System.IO;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using MeshC = MeshComponent;
using Texture = TextureSystem;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    
    private List<MeshC> _MeshComp;
    //private List<Texture> _TextureComponent;
    string _Direction;
    private Shader _Shader;

    public Load(string _Path)
    {
        //_TextureComponent = new List<Texture>();

        LoadModelFromFile(_Path);
    }

    private void LoadModelFromFile(string _Path)
    {
     
        AssimpContext _Import = new AssimpContext();
        Scene _Scene = _Import.ImportFile(_Path,PostProcessSteps.Triangulate);

        _MeshComp = new List<MeshC>();

        _Direction = _Path.Substring(0,_Path.LastIndexOf('/'));

        Node(_Scene.RootNode, _Scene);

        _Import.Dispose();
    }

   
    private void Node(Node _Node, Scene _Scene)
    {
        for (int i = 0; i<_Node.MeshCount;i++)
        {
            Mesh _Mesh = _Scene.Meshes[_Node.MeshIndices[i]];
            _MeshComp.Add(LoadModelInWorld(_Mesh,_Scene));
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
        //List<Texture> _TexG = new List<Texture>();

        for (int i = 0;i<_Mesh.VertexCount;i++)
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
                //_Vertex._TexCoord = _Vec;
            }
            else
            {
                //_Vertex._TexCoord = new Vector2(0.0f,0.0f);
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

        //Material _Mat = _Scene.Materials[_Mesh.MaterialIndex];

        //List<Texture> _Diffuse = LoadTexture(_Mat,TextureType.Diffuse, "texture_diffuse");
        //_Diffuse.AddRange(_Diffuse);

        //List<Texture> _Specular = LoadTexture(_Mat, TextureType.Specular, "texture_specular");
        //_Specular.AddRange(_Specular);

        //List<Texture> _Normal = LoadTexture(_Mat, TextureType.Height, "texture_normal");
        //_Normal.AddRange(_Normal);

        //List<Texture> _Heigth = LoadTexture(_Mat, TextureType.Ambient, "texture_height");
        //_Heigth.AddRange(_Heigth);

        //Console.WriteLine("Output data from model. Vertex count:"+_Mesh.VertexCount+". Index count:"+_Mesh.FaceCount);

        return new MeshC(_VertexG.ToArray(), _Index.ToArray());

    }

    //private List<Texture> LoadTexture(Material _Material, TextureType _Type, string _TypeName)
    //{
    //    List<Texture> _Textures = new List<Texture>();

    //    for (int i = 0; i<_Material.GetMaterialTextureCount(_Type); i++)
    //    {
    //        TextureSlot _Slot;
    //        _Material.GetMaterialTexture(_Type,i,out _Slot);

    //        bool _SkipTexture = false;
    //        for (int j = 0;j<_TextureComponent.Count;i++)
    //        {
    //            if (_TextureComponent[j]._Path.CompareTo(_Slot.FilePath)==0)
    //            {
    //                _Textures.Add(_TextureComponent[j]);
    //                _SkipTexture = true;
    //                break;
    //            }
    //        }
    //        if (!_SkipTexture)
    //        {
    //            Texture _Texture = Texture.Texture(_Slot.FilePath,_Direction,_TypeName);
    //            _Textures.Add(_Texture);
    //            _TextureComponent.Add(_Texture);
    //        }
            
    //    }
    //    return _Textures;
    //}
    

    //Draw model system
    public void Draw(Shader _Shader)
    {
        foreach (MeshC _Mesh in _MeshComp)
        {
            _Mesh.DrawMesh(_Shader);
        }
    }
}
