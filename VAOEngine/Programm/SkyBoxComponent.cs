using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using OpenTK.Mathematics;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class SkyBoxComponent
{
    private static DebugProc _Debug = OnDebugMessage;
    public string _Log;
    private int _VAO;
    private int[] _Index = new int[]
    {
        1,2,6,
        6,5,1,
        0,4,7,
        7,3,0,
        4,5,6,
        6,7,4,
        0,3,2,
        2,1,0,
        0,1,5,
        5,4,0,
        3,7,0,
        6,2,3
    };


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

        UseTexture(TextureUnit.Texture0);


        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        

        GL.BindVertexArray(0);
        GL.DepthFunc(DepthFunction.Less);
    }

    private void AddSkyBox()
    {

        _VAO = GL.GenVertexArray();
        int _VBO = GL.GenBuffer();
        int _EBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _Vert.Length * sizeof(float), _Vert, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _Index.Length * sizeof(int), _Index, BufferUsageHint.StaticDraw);


        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);



    }

    public static SkyBoxComponent LoadTexture(List<string> _SkyTexture)
    {
        int _LCount = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.TextureCubeMap, _LCount);




        for (int i = 0; i < _SkyTexture.Count; i++)
        {


            using (Stream _FileTexture = File.OpenRead(_SkyTexture[i]))
            {
                StbImage.stbi_set_flip_vertically_on_load(1);

                ImageResult _Texture = ImageResult.FromStream(_FileTexture, ColorComponents.RedGreenBlue);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX+i, 0, PixelInternalFormat.Rgb, _Texture.Width, _Texture.Height,
                0, PixelFormat.Rgb, PixelType.UnsignedByte, _Texture.Data);

                GL.DebugMessageCallback(_Debug, IntPtr.Zero);
                GL.Enable(EnableCap.DebugOutput);
                GL.Enable(EnableCap.DebugOutputSynchronous);

            }
            


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
        _Log = GL.GetError().ToString();
        AddSkyBox();
    }

    public void UseTexture(TextureUnit _Texture)
    {
        GL.ActiveTexture(_Texture);
        GL.BindTexture(TextureTarget.TextureCubeMap, _Count);
    }

    private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
    {

        string message = Marshal.PtrToStringAnsi(pMessage, length);

        Debug.WriteLine("[{0} source={1} type={2} id={3}] {4}. " + severity + ". " + source + ". " + type + ". " + id + ". " + message);
        //_LLog = "[{0} source={1} type={2} id={3}] {4}. " + severity + ". " + source + ". " + type + ". " + id + ". " + message;

        if (type == DebugType.DebugTypeError)
        {
            throw new Exception(message);
        }
    }


}
