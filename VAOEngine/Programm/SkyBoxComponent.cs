using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Mathematics;
using System.IO;

public class SkyBoxComponent
{

    public string _Log;
    private int _VAO;
    private float[] _Vert = new float[]
    {-1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
            1.0f,  1.0f, -1.0f,
            1.0f,  1.0f,  1.0f,
            1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
            1.0f, -1.0f,  1.0f};




    




    private readonly int _Count;

    public void Draw(Shader _Shader, Camera _Camera)
    {
        GL.DepthFunc(DepthFunction.Lequal);

        _Shader.UseShader();
        _Shader.SetInt("skybox", 0);

        var _View = new Matrix4(new Matrix3(_Camera.GetView()));
        _Shader.SetMatrix4("view", _View);
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        

        GL.BindVertexArray(_VAO);

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _Count);
        

        GL.DrawArrays(PrimitiveType.Triangles,0,36);

        GL.BindVertexArray(0);
        GL.DepthFunc(DepthFunction.Less);
    }

    private void AddSkyBox()
    {
        
        _VAO = GL.GenVertexArray();
        int _VBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _Vert.Length * sizeof(float), _Vert, BufferUsageHint.StaticDraw);

        
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,3*sizeof(float),0);
        GL.EnableVertexAttribArray(0);
        


    }

    public static SkyBoxComponent LoadTexture(List<string> _SkyTexture)
    {
        int _LCount = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _LCount);
        



        for (int i = 0;i<_SkyTexture.Count;i++)
        {


            Stream _FileTexture = File.OpenRead(_SkyTexture[i]);
            ImageResult _Texture = ImageResult.FromStream(_FileTexture, ColorComponents.RedGreenBlue);
            
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgb, _Texture.Width, _Texture.Height,
            0, PixelFormat.Rgb, PixelType.UnsignedByte, _Texture.Data);


        }

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

        return new SkyBoxComponent(_LCount);

    }

    public SkyBoxComponent(int _HCount)
    {
        _Count = _HCount;
        AddSkyBox();
    }

    public void UseTexture(TextureUnit _Texture)
    {
        //GL.ActiveTexture(_Texture);
        GL.BindTexture(TextureTarget.TextureCubeMap, _Count);
    }
    
}
