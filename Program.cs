using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Shader = ShaderSystem;
using System;





public class MainSystemEngine : GameWindow
{
    public int _VAO, _VBO, _EBO;
    Shader _Shader;

    public MainSystemEngine(int _Widht, int _Height, string _Title) : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = (_Widht, _Height);
        Title = _Title;
        
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        float[] _Vert =
        {
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
            0.0f,  0.5f, 0.0f
        };

        _Shader = new Shader("(You're direction)\\Shader\\VertShader.glsl", "(You're direction)\\Shader\\FragShader.glsl");

        GL.ClearColor(0.2f,0.3f,0.4f,0.1f);

        _VAO = GL.GenBuffer();
        _VAO = GL.GenVertexArray();

        _VBO = GL.GenBuffer();

        GL.BindVertexArray(_VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);

        GL.BufferData(BufferTarget.ArrayBuffer, _Vert.Length*sizeof(float), _Vert,BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false, 3*sizeof(float),0);
        GL.EnableVertexAttribArray(0);

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
        GL.BindVertexArray(_VAO);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0,0,e.Width,e.Height);
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
