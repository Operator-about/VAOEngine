using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using Shader = ShaderSystem;
using ModelLoad = Load;
using Camera = CameraSystem;
using Light = LightComponent;






public class MainSystemEngine
{

    private int _Width, _Height;


    private Shader _Shader, _ModelShader, _LampShader;
    private List<ModelLoad> _ModelLoader;
    private List<Light> _LightLoader;
    private ModelLoad _LocalModel;
    private Camera _Camera;


    public readonly object _Lock = new object();
    private System.Numerics.Vector3 _Color = new System.Numerics.Vector3(2.0f, 5.0f, 2.0f);



    public void Load()
    {


        //Load shader system
        _Shader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragShader.glsl");
        _ModelShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragLightShader.glsl");

        GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
        GL.Enable(EnableCap.DepthTest);


        //Load Camera
        _Camera = new Camera(Vector3.UnitZ * 3, _Width / (float)_Height);


        _ModelLoader = new List<ModelLoad>();
        _LightLoader = new List<Light>();


        //Use Shader
        _Shader.UseShader();
    }

    

    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      

        _ModelShader.UseShader();

        for (int i = 0; i < _ModelLoader.Count; i++)
        {
            _ModelLoader[i].Draw(_ModelShader, _Camera);
        }
        for (int i = 0; i < _LightLoader.Count; i++)
        {
            _LightLoader[i].DrawLight(_ModelShader, _Camera);
        }


        //Use main shader
        _Shader.UseShader();
        int _Location = GL.GetUniformLocation(_Shader._Count, "ourColor");
        GL.Uniform4(_Location, _Color.X, _Color.Y, _Color.Z, 1.0f);
    }

    



    
    

    
    
}