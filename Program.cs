using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;






public class MainSystemEngine : GameWindow
{

    private int _Width, _Height;
    private Vector3 _Position = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _Scale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _Rotation = new Vector3(1.0f, 1.0f, 1.0f);

    Shader _Shader, _ModelShader, _LampShader;
    private List<ModelLoad> _ModelLoader;
    Camera _Camera;
    StreamWriter _FileWrite;
    StreamReader _FileRead;

    private string _Line;
    private string _Direction, _PathModelSave;
    private System.Numerics.Vector3 _Color = new System.Numerics.Vector3(2.0f, 5.0f, 2.0f);
    private bool _NewSave;
    private bool _Flow = true;


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


        //Load Camera
        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);


        _ModelLoader = new List<ModelLoad>();

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
            ModelLoad _LocalModel = new ModelLoad(_Line);
            _ModelLoader.Add(_LocalModel);
        }
        /*
        * If user got save file and agree use this file, that this file read
        */
        else
        {
            //New save: new model
            _FileWrite = new StreamWriter(_PathModelSave);
            Console.WriteLine("Input Direction For Model:");
            _Direction = Console.ReadLine()!;
            _FileWrite.WriteLine(_Direction);
            ModelLoad _LocalModel = new ModelLoad(_Direction);
            _ModelLoader.Add(_LocalModel);


            //Save
            ReturnSaveFile();
        }

        //Position
        Console.WriteLine("Input position:");
        int X, Y, Z = 0;
        X = Int32.Parse(Console.ReadLine()!);
        Y = Int32.Parse(Console.ReadLine()!);
        Z = Int32.Parse(Console.ReadLine()!);
        _Position = new Vector3(X, Y, Z);
        ReturnVector3(_Position);

        //Scale
        Console.WriteLine("Input scale:");
        X = Int32.Parse(Console.ReadLine()!);
        Y = Int32.Parse(Console.ReadLine()!);
        Z = Int32.Parse(Console.ReadLine()!);
        _Scale = new Vector3(X, Y, Z);
        ReturnVector3(_Scale);


        //Rotation
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
            if (_FileWrite != null)
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

        //Render model
        foreach (var _OutModelLoader in _ModelLoader)
        {
            _OutModelLoader.Draw(_ModelShader, _Camera);
            _OutModelLoader._OutModel._MatrixModel._Position = _Position;
            _OutModelLoader._OutModel._MatrixModel._Rotation.X = _Rotation.X;
            _OutModelLoader._OutModel._MatrixModel._Rotation.Y = _Rotation.Y;
            _OutModelLoader._OutModel._MatrixModel._Rotation.Z = _Rotation.Z;
            _OutModelLoader._OutModel._MatrixModel._Scale = _Scale;
        }

        //Use main shader
        _Shader.UseShader();
        int _Location = GL.GetUniformLocation(_Shader._Count, "ourColor");
        GL.Uniform4(_Location, _Color.X, _Color.Y, _Color.Z, 1.0f);




        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
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