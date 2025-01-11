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
using LightComp = LightComponent;





public class MainSystemEngine : GameWindow
{

    private int _Width, _Height;
    private Vector3 _LampPos = new Vector3(1.2f, 1.0f, 5.0f);
    Shader _Shader, _ModelShader, _LampShader;
    LightComp _LightComponent;
    ModelLoad _FModel, _LModel;
    Camera _Camera;
    
    private System.Numerics.Vector3 _Color = new System.Numerics.Vector3(2.0f,5.0f,2.0f);

    

    public MainSystemEngine(string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = new Vector2i(800, 800);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        _Shader = new Shader("/You're route/Shader/VertShader.glsl", "/You're route/Shader/FragShader.glsl");
        _LampShader = new Shader("/You're route/Shader/VertShader.glsl", "/You're route/Shader/FragShader.glsl");
        _ModelShader = new Shader("/You're route/Shader/VertShader.glsl", "/You're route/Shader/FragLightShader.glsl");

        GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
        GL.Enable(EnableCap.DepthTest);

        _FModel = new ModelLoad("/You're route/MeshModel/Cube.obj");
        _LModel = new ModelLoad("/You're route/MeshModel/Sphere.obj");
        _LightComponent = new LightComp();

        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);

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
        
        
        _ModelShader.UseShader();



        var _ModelF = Matrix4.Identity;

        //Light position
        var _ModelLight = Matrix4.CreateTranslation(_LampPos);

        //model load in frag shader...
        _ModelShader.UseShader();
        _ModelShader.SetMatrix4("model", _ModelF);
        _ModelShader.SetMatrix4("view", _Camera.GetView());
        _ModelShader.SetMatrix4("proj", _Camera.GetProjection());
        _ModelShader.SetVector3("objColor", new OpenTK.Mathematics.Vector3(0.0f, 0.5f, 0.31f));
        _ModelShader.SetVector3("lightColor", new OpenTK.Mathematics.Vector3(1.0f, 1.0f, 1.0f));
        _ModelShader.SetVector3("lightPosition", _LampPos);
        _ModelShader.SetVector3("lightView", _Camera._Position);
        _FModel.Draw(_Shader);


        //Load light...
        _LightComponent.SetLight(_LampShader, _Camera,_LModel, _LampPos, _ModelLight);


        _Shader.UseShader();
        int _Location = GL.GetUniformLocation(_Shader._Count, "ourColor");
        GL.Uniform4(_Location, _Color.X, _Color.Y, _Color.Z, 1.0f);

        

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
