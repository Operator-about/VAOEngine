using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.IO;





public class LightComponent
{
    private Vector3 _LightColor;
    public Vector3 _LightPosition;
    private int _VAO;
    private int _Count;
    private string _TextureLight = @".\Texture\Light.png";

    private float[] _Vert = new float[]
    {
        -0.5f,-0.5f, 0.0f,    0,0f,0,0f,
        -0.5f, 0.5f, 0.0f,    0.0f,1.0f,
         0,5f, 0.5f, 0.0f,    1.0f,1.0f,
         0.5f,-0.5f, 0.0f,    1.0f,0.0f,
    };

    private int[] _Index = new int[] 
    { 
        0,2,1,
        0,3,2
    };


    public LightComponent(Vector3 _LLightColor, Vector3 _LLightPosition)
    {
        _LightColor = _LLightColor;
        _LightPosition = _LLightPosition;
        AddPlaneTexture();
        AddTextureToPlane();
    }

    public void DrawLight(Shader _Shader, Shader _TextureShader, Camera _Camera)
    {
        GL.BindVertexArray(_VAO);

        //Shader for texture
        _TextureShader.UseShader();
        var _PositionTexturePanel = Matrix4.CreateTranslation(_LightPosition);
        var _ScaleTexturePanel = Matrix4.CreateScale(1, 1, 1);
        var _MatrixTexturePanel = _PositionTexturePanel * _ScaleTexturePanel;
        _TextureShader.SetMatrix4("view", _Camera.GetView());
        _TextureShader.SetMatrix4("proj", _Camera.GetProjection());
        _TextureShader.SetMatrix4("model", _MatrixTexturePanel);
        _TextureShader.SetInt("texture0",0);

        //Shader for light
        _Shader.SetVector3("lightColor",_LightColor);
        _Shader.SetVector3("lightPosition",_LightPosition);
        _Shader.SetVector3("lightView",_Camera._Position);



        GL.BindTexture(TextureTarget.Texture2D, _Count);
        GL.DrawElements(PrimitiveType.Triangles, _Index.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    private void AddPlaneTexture()
    {
        _VAO = GL.GenVertexArray();
        int _VBO = GL.GenBuffer();
        int _EBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _Vert.Length * sizeof(float), _Vert, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _Index.Length * sizeof(int), _Index, BufferUsageHint.StaticDraw);  

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8*sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        
    }

    private int AddTextureToPlane()
    {
        _Count = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _Count);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);

        StbImage.stbi_set_flip_vertically_on_load(1);

        Stream _FileTexture = File.OpenRead(_TextureLight);
        ImageResult _Texture = ImageResult.FromStream(_FileTexture, ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _Texture.Width, _Texture.Height, 0, PixelFormat.Rgba,
            PixelType.UnsignedByte, _Texture.Data);

        


        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.BindTexture(TextureTarget.Texture2D, 0);

        return _Count;
    }

}
