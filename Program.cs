using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using Texture = TextureSystem;
using ModelLoad = Load;
using Camera = CameraSystem;





public class MainSystemEngine : GameWindow
{
    public int _VAO, _VBO, _EBO, _VertexTexture;
    public int _Width, _Height;
    Shader _Shader;
    ModelLoad _Model;
    Camera _Camera;

    public uint[] _Index =
    {
        0, 1, 3,
        1, 2, 3
    };

    public MainSystemEngine(int _Widht, int _Height, string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = (_Widht, _Height);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        _Shader = new Shader("D:\\VAOEngine\\VAOEngine\\Shader\\VertShader.glsl", "D:\\VAOEngine\\VAOEngine\\Shader\\FragShader.glsl");
        _Model = new ModelLoad();
        _Model.LoadModelFromFile("D:\\3D\\Cube.obj", _Shader);

        _Camera = new Camera();
        _Camera.CameraStartup();

        GL.ClearColor(0.2f,0.3f,0.4f,0.1f);

        

        //Matrix4.CreateOrthographicOffCenter(0.0f, _Width, 0.0f, _Height, 0.1f, 100.0f);

        //Use Shader
        _Shader.UseShader();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _Shader.UseShader();

        _Camera.UpdateCameraMatrix(_Width, _Height, 45.0f, 0.1f, 100.0f);
        _Camera.InputCameraSystem(KeyboardState);

        _Model.Draw(_Shader,_Camera);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0,0,e.Width,e.Height);
        _Width = e.Width;
        _Height = e.Height;
    }


}


class StartupEngine
{
    static void Main()
    {
        MainSystemEngine _Engine = new MainSystemEngine(800, 800, "VAOEngine");
        _Engine.Run();
    }
}
