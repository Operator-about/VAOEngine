using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;





public class MainSystemEngine : GameWindow
{

    public int _Width, _Height;
    Shader _Shader;
    ModelLoad _Model;
    Camera _Camera;
    System.Numerics.Vector3 _Color = new System.Numerics.Vector3(0.5f, 0.5f, 0.0f);
    private int _VAO;

    private readonly float[] _Vert =
    {
            -0.5f, -0.5f, 0.0f, 
             0.5f, -0.5f, 0.0f, 
             0.0f,  0.5f, 0.0f  
    };


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

        _Model = new ModelLoad("D:/3D/Craft.fbx");
        
        
     



        _Camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);


        int _VBO = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer,_Vert.Length*sizeof(float), _Vert, BufferUsageHint.StaticDraw);

        _VAO = GL.GenVertexArray();
        GL.BindVertexArray(_VAO);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        

        _Shader = new Shader("D:\\VAOEngine\\VAOEngine\\Shader\\VertShader.glsl", "D:\\VAOEngine\\VAOEngine\\Shader\\FragShader.glsl");

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
        var _Mode = Matrix4.Identity;
        _Shader.SetMatrix4("model",_Mode);
        _Shader.SetMatrix4("view",_Camera.GetView());
        _Shader.SetMatrix4("proj",_Camera.GetProjection());

        _Camera.InputCameraSystem(KeyboardState,MouseState);

        _Model.Draw();
        _Shader.UseShader();


        GL.BindVertexArray(_VAO);

        int _VertexLocation = GL.GetUniformLocation(_Shader._Count, "ourColor");
        GL.Uniform4(_VertexLocation, _Color.X, _Color.Y, _Color.Z, 1.0f);
        GL.DrawArrays(PrimitiveType.Triangles,0,3);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0,0,Size.X,Size.Y);
        //_Camera._Aspect = Size.X / (float)Size.Y;

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
