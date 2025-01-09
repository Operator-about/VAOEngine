using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using OpenTK.Mathematics;
using System.Threading;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;





public class MainSystemEngine : GameWindow
{

    public int _Width, _Height;
    Shader _Shader;
    ModelLoad _Model;
    Camera _Camera;

  

    public MainSystemEngine(string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = new Vector2i(800, 800);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
        GL.Enable(EnableCap.DepthTest);



       
       _Model = new ModelLoad("/You're model.(.obj, .fbx)");
       
        


        
        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);
            



        _Shader = new Shader("/You're direction/Shader/VertShader.glsl", "/You're direction/Shader/FragShader.glsl");

        //Use Shader
        _Shader.UseShader();

        CursorState = CursorState.Grabbed;
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);


        _Camera.InputCameraSystem(KeyboardState, MouseState, CursorState);

        if (!IsFocused)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

       
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        
        _Shader.UseShader();
        var _Mode = Matrix4.Identity;
        _Shader.SetMatrix4("model",_Mode);
        _Shader.SetMatrix4("view",_Camera.GetView());
        _Shader.SetMatrix4("proj",_Camera.GetProjection());

        

        _Model.Draw(_Shader);
        _Shader.UseShader();


        

       
        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0,0,e.Width,e.Height);
        _Width = e.Width;
        _Height = e.Height; 
        //_Camera._Aspect = e.Width / (float)e.Height;


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
        MainSystemEngine _Engine = new MainSystemEngine("VAOEngine");
        _Engine.Run();
    }
}
