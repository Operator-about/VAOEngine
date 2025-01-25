using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.IO;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;
using Comp = AddComponent;
using System.Runtime;
using System.Threading;






public class MainSystemEngine : GameWindow
{

    private int _Width, _Height;
    private Vector3 _LampPos = new Vector3(1.2f, 1.0f, 5.0f);
    private Vector3 _Position = new Vector3(1.0f,1.0f,1.0f);
    private Vector3 _Scale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _Rotation = new Vector3(1.0f,1.0f,1.0f);

    Shader _Shader, _ModelShader, _LampShader;
    Comp _Component;
    ModelLoad _LModel, _FModel;
    Camera _Camera;
    StreamWriter _FileWrite;
    StreamReader _FileRead;
    
    private string _Line;
    private string _Direction, _PathModelSave;
    private System.Numerics.Vector3 _Color = new System.Numerics.Vector3(2.0f,5.0f,2.0f);
    private bool _NewSave;



    //private int X, Y, Z;
    

    public MainSystemEngine(string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = new Vector2i(800, 800);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();


        Console.WriteLine("Input route to save file");
        //Path to save file
        _PathModelSave = Console.ReadLine()!;
        ReturnString(_PathModelSave);


        //Load shader system
        _Shader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragShader.glsl");
        _LampShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragShader.glsl");
        _ModelShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragLightShader.glsl");

        GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
        GL.Enable(EnableCap.DepthTest);



        //Load component for add
        _Component = new Comp();

        //Load Camera
        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);

        /*Load file system
         * If user got save file, that user maybe use this save file
        */
        if (File.Exists(_PathModelSave))
        {
            Console.WriteLine("Input: Yes. If you want use save file:");
            if (Console.ReadLine()! == "Yes" && File.Exists(_PathModelSave))
            {
                _NewSave = false;
            }
            else
            {
                _NewSave = true;
            }
        }
        else
        {
            _NewSave = true;
        }

        if (File.Exists(_PathModelSave) && _NewSave == false)
        {

            _FileRead = new StreamReader(_PathModelSave);

            _Line = _FileRead.ReadLine()!;
            _FModel = new ModelLoad(_Line);


            _Line = _FileRead.ReadLine()!;
            _LModel = new ModelLoad(_Line);
        }
        /*
        * If user got save file and agree use this file, that this file read
        */
        else
        {
            _FileWrite = new StreamWriter(_PathModelSave);
            Console.WriteLine("Input Direction For Model:");
            _Direction = Console.ReadLine()!;
            _FileWrite.WriteLine(_Direction);
            _FModel = new ModelLoad(_Direction);

            Console.WriteLine("Input Direction For Model Light:");
            _Direction = Console.ReadLine()!;
            _FileWrite.WriteLine(_Direction);
            _LModel = new ModelLoad(_Direction);



            ReturnSaveFile();
        }

        Console.WriteLine("Input position:");
        int X, Y, Z = 0;
        X = Int32.Parse(Console.ReadLine()!);
        Y = Int32.Parse(Console.ReadLine()!);
        Z = Int32.Parse(Console.ReadLine()!);
        _Position = new Vector3(X,Y,Z);
        ReturnVector3(_Position);


        Console.WriteLine("Input scale:");
        X = Int32.Parse(Console.ReadLine()!);
        Y = Int32.Parse(Console.ReadLine()!);
        Z = Int32.Parse(Console.ReadLine()!);
        _Scale = new Vector3(X, Y, Z);
        ReturnVector3(_Scale);

        Console.WriteLine("Input rotation:");
        X = Int32.Parse(Console.ReadLine()!);
        Y = Int32.Parse(Console.ReadLine()!);
        Z = Int32.Parse(Console.ReadLine()!);
        _Rotation = new Vector3(X, Y, Z);
        ReturnVector3(_Rotation);




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
            //Close the save file
            if (_FileWrite!=null)
            {
                _FileWrite.Close();
            }
            else
            {

            }
            Close();
        }

       
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


        _ModelShader.UseShader();

        var _ModelMatrixF = Matrix4.Identity; 
        var _PositionModelF = Matrix4.CreateTranslation(_Position);
        var _ScaleMatrixModelF = Matrix4.CreateScale(_Scale);
        var _MatrixX = Matrix4.CreateRotationX(_Rotation.X);
        var _MatrixY = Matrix4.CreateRotationY(_Rotation.Y);
        var _MatrixZ = Matrix4.CreateRotationZ(_Rotation.Z);

        _ModelMatrixF = _PositionModelF * _ScaleMatrixModelF * (_MatrixX * _MatrixY * _MatrixZ);
        _Component.SetModel(_ModelShader, _Camera, ref _FModel, _LampPos, _ModelMatrixF);
        








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


    
    private StreamWriter ReturnSaveFile()
    {
        return _FileWrite;
    }
    private string ReturnString(string _String)
    {
        return _String;
    }
    private Vector3 ReturnVector3(Vector3 _Vector3)
    {
        return _Vector3;
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
