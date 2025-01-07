using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
//using Texture = TextureSystem;
using ModelLoad = Load;
using Camera = CameraSystem;





public class MainSystemEngine : GameWindow
{

    public int _Width, _Height;
    Shader _Shader;
    ModelLoad _Model;
    Camera _Camera;
    private double _Time;

   

   

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
        _Camera.CameraStartup(90.0f,_Width,_Height);

        GL.ClearColor(0.2f,0.3f,0.4f,0.1f);

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

        _Time += 4.0f + args.Time;

        GL.Clear(ClearBufferMask.ColorBufferBit);

        
        _Shader.UseShader();
        var _Mode = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_Time));
        _Shader.SetMatrix4("model",_Mode);
        _Shader.SetMatrix4("view",_Camera.GetView());
        _Shader.SetMatrix4("proj",_Camera.GetProjection());

        _Camera.InputCameraSystem(KeyboardState,MouseState);

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

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);
        
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
