using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;
using Light = LightComponent;






public class MainSystemEngine : GameWindow
{

    private int _Width, _Height;


    private Shader _Shader, _ModelShader, _LampShader;
    private List<ModelLoad> _ModelLoader;
    private List<Light> _LightLoader;
    private ModelLoad _LocalModel;
    private Camera _Camera;


    public readonly object _Lock = new object();
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
        lock (_Lock)
        {
            Context.MakeCurrent();
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
            _LightLoader = new List<Light>();
            

            //Use Shader
            _Shader.UseShader();
            CursorState = CursorState.Grabbed;

            Context.MakeNoneCurrent();
            
            
        }
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
        lock (_Lock)
        {
            Context.MakeCurrent();
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Thread for load model
            if (_Flow==true)
            {
                _Flow = false;
                Thread _FlowThread = new Thread(Command);
                _FlowThread.Start();
            }

            _ModelShader.UseShader();

            for (int i = 0; i < _ModelLoader.Count; i++)
            {
                _ModelLoader[i].Draw(_ModelShader, _Camera);
            }
            for (int i = 0;i<_LightLoader.Count;i++)
            {
                _LightLoader[i].DrawLight(_ModelShader, _Camera);
            }


            //Use main shader
            _Shader.UseShader();
            int _Location = GL.GetUniformLocation(_Shader._Count, "ourColor");
            GL.Uniform4(_Location, _Color.X, _Color.Y, _Color.Z, 1.0f);

            

            SwapBuffers();
            Context.MakeNoneCurrent();
        }
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



    
    private string ReturnString(string _String)
    {
        return _String;
    }

    //Command function
    private void Command()
    {
        Console.WriteLine("Input command: model or other:");
        string _Command = Console.ReadLine()!;
        if (_Command=="model")
        {
            ModelLoaderFunc();
        }
        else if (_Command=="light")
        {
            LightLoaderFunc();
        }
        else if (_Command == "position model")
        {
            Console.WriteLine("Input ID model(only in type Int32):");
            int _IDModel = Int32.Parse(Console.ReadLine()!);
            Console.WriteLine("Set new position(X,Y,Z):");
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Position.X = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Position.Y = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Position.Z = Int32.Parse(Console.ReadLine()!);
            _Flow = true;
        }
        else if (_Command == "rotation model")
        {
            Console.WriteLine("Input ID model(only in type Int32):");
            int _IDModel = Int32.Parse(Console.ReadLine()!);
            Console.WriteLine("Set new rotation(X,Y,Z):");
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Rotation.X = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Rotation.Y = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Rotation.Z = Int32.Parse(Console.ReadLine()!);
            _Flow = true;
        }
        else if (_Command == "scale model")
        {
            Console.WriteLine("Input ID model(only in type Int32):");
            int _IDModel = Int32.Parse(Console.ReadLine()!);
            Console.WriteLine("Set new scale(X,Y,Z):");
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Scale.X = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Scale.Y = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Scale.Z = Int32.Parse(Console.ReadLine()!);
            _Flow = true;
        }
        else if (_Command == "color model")
        {
            Console.WriteLine("Input ID model(only in type Int32):");
            int _IDModel = Int32.Parse(Console.ReadLine()!);
            Console.WriteLine("Set new color(X,Y,Z):");
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Color.X = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Color.Y = Int32.Parse(Console.ReadLine()!);
            _ModelLoader[_IDModel]._OutModel._MatrixModel._Color.Z = Int32.Parse(Console.ReadLine()!);
            _Flow = true;
        }
        else
        {
            _Flow = true;
        }
    }

    //Model load function
    private void ModelLoaderFunc()
    {
        Task.Run(() =>
        {
            lock (_Lock)
            {
                
                Console.WriteLine("Input Direction model:");
                _Direction = Console.ReadLine()!;
                Context.MakeCurrent();
                _LocalModel = new ModelLoad(_Direction);
                _ModelLoader.Add(_LocalModel);
                Console.WriteLine($"Model ID:{_ModelLoader.Count}");
                Context.MakeNoneCurrent();
                _Flow = true;
            }

        });
    }
    private void LightLoaderFunc()
    {
        Task.Run(() =>
        {
            lock (_Lock)
            {
                Context.MakeCurrent();
                Light _LocalLight = new Light();
                _LocalLight.AddPointLight();
                _LightLoader.Add(_LocalLight);
                Context.MakeNoneCurrent();
                _Flow = true;
            }
        });
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