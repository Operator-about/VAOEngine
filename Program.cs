using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Threading;
using System.Collections.Concurrent;
using OpenTK.Mathematics;
using System.Threading;
using System.Threading.Tasks;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;
using Comp = AddComponent;
using Assimp;





public class MainSystemEngine : GameWindow
{

    private int _Width, _Height;
    private Vector3 _LampPos = new Vector3(1.2f, 1.0f, 5.0f);
    private Vector3 _ModelPos = new Vector3();
    Shader _Shader, _ModelShader, _LampShader;
    Comp _Component;
    ModelLoad _FModel, _LModel;
    Camera _Camera;
    private string _Direction, _DirectionVert, _DirectionFrag;
    


    private System.Numerics.Vector3 _Color = new System.Numerics.Vector3(2.0f,5.0f,2.0f);

    

    public MainSystemEngine(string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = new Vector2i(800, 800);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        Console.WriteLine("Input Direction For Vertex Shader:");
        _DirectionVert = Console.ReadLine()!;
        Console.WriteLine("Input Direction For Fragment Shader:");
        _DirectionFrag = Console.ReadLine()!;
        _Shader = new Shader(_DirectionVert, _DirectionFrag);

        _LampShader = new Shader(_DirectionVert, _DirectionFrag);

        Console.WriteLine("Input Direction For Fragment Shader:");
        _DirectionFrag = Console.ReadLine()!;
        _ModelShader = new Shader(_DirectionVert, _DirectionFrag);

        GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
        GL.Enable(EnableCap.DepthTest);

        


        _Component = new Comp();

        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);

        CursorState = CursorState.Grabbed;

        Console.WriteLine("Input Direction For Model:");
        _Direction = Console.ReadLine()!;
        _FModel = new ModelLoad(_Direction);

        Console.WriteLine("Input Direction For Model Light:");
        _Direction = Console.ReadLine()!;
        _LModel = new ModelLoad(_Direction);


        //Console.WriteLine("Input Coord For Model Position:");
        //int X = Int32.Parse(Console.ReadLine());
        //int Y = Int32.Parse(Console.ReadLine());
        //int Z = Int32.Parse(Console.ReadLine());
        //_ModelPos = new Vector3(X, Y, Z);

        Console.WriteLine("Input Coord For Lamp Position:");
        int X = Int32.Parse(Console.ReadLine()!);
        int Y = Int32.Parse(Console.ReadLine()!);
        int Z = Int32.Parse(Console.ReadLine()!);
        _LampPos = new Vector3(X, Y, Z);


        //Use Shader
        _Shader.UseShader();
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
        //model load in frag shader...
        _Component.SetModel(_ModelShader, _Camera, _FModel, _LampPos, _ModelF);






        //Light position
        var _ModelLight = Matrix4.CreateTranslation(_LampPos);
        _Component.SetLight(_LampShader, _Camera, _LModel, _LampPos, _ModelLight);

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
