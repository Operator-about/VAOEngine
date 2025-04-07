using System.Windows;
using System.Windows.Input;
using OpenTK.Wpf;
using BackGround = BackgroundProcess;
using ModelLoad = Load;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using SkyBox = SkyBoxComponent;
using Light = LightComponent;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;




namespace VAOEngine
{
    public partial class MainWindow : Window
    {
        private Shader _Shader, _ModelShader, _SkyShader;
        private BackGround _Process = new BackGround();
        private string _ModelPos;
        private List<ModelLoad> _ModelLoader;
        private Camera _Camera;
        private SkyBox _SkyBox;
        private Light _LightComponent;
        private static List<string> _SkyTexture = new List<string>() {
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg",
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg",
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg",
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg",
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg",
        @"D:\VAOEngine\VAOEngine\SkyBoxTexture\CloudSky.jpg" };

        public MainWindow()
        {

            InitializeComponent();



            var _Settings = new GLWpfControlSettings()
            {
                MajorVersion = 4,
                MinorVersion = 0
            };
            
           
            _Control.Start(_Settings);
            _Shader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragLightShader.glsl");
            _SkyShader = new Shader(@$".\Shader\VertSkyShader.glsl", @$".\Shader\FragSkyShader.glsl");
            _ModelShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragLightShader.glsl");
            _LightComponent = new Light();
            GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
            _ModelLoader = new List<ModelLoad>();

            
            _SkyBox = SkyBox.LoadTexture(_SkyTexture);
            _SkyBox.UseTexture(TextureUnit.Texture0);
            
            _Camera = new Camera(Vector3.UnitZ * 3, (float)Width / (float)Height);
            _Process.IsVisible = false;
            _Shader.UseShader();

        


        }

        
        
        private void _Control_Render(TimeSpan obj)
        {
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _Camera.InputCameraSystem();

            _ModelShader.UseShader();
            

            for (int i = 0; i < _ModelLoader.Count; i++)
            {
                _LightComponent.DrawLight(_ModelShader, _Camera);
                _ModelLoader[i].Draw(_ModelShader, _Camera);
                _ModelPos = _ModelLoader[i]._OutModel._MatrixModel._Position.ToString();
                

            }

            _SkyShader.UseShader();
            _SkyBox.Draw(_SkyShader, _Camera);
            _SkyBox.UseTexture(TextureUnit.Texture0);



            if (_Shader._Log=="")
            {
                Log.Text = "ExitCode:0";
            }
            else
            {
                Log.Text = _Shader._Log;
            }

            //Use main shader
            _Shader.UseShader();

            ModelCount.Text = _ModelLoader.Count.ToString();
            ModelPosition.Text = _ModelPos;
            
            ShaderMainStatus.Text = $"Shader status:{_Shader.ToString()}";
            CameraPos.Text = $"X:{_Camera._Position.X.ToString()}.Y:{_Camera._Position.Y.ToString()}.Z:{_Camera._Position.Z.ToString()}";
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var _Position = Mouse.GetPosition(this);
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                if (_Camera._First)
                {
                    _Camera._LastPos = new Vector2((float)_Position.X, (float)_Position.Y);
                    _Camera._First = false;
                }
                else
                {

                    float _DeltaX = (float)_Position.X - (float)_Camera._LastPos.X;
                    float _DeltaY = (float)_Position.Y - (float)_Camera._LastPos.Y;
                    _Camera._LastPos = new Vector2((float)_Position.X, (float)_Position.Y);

                    _DeltaX *= _Camera._Sentensity;
                    _DeltaY *= _Camera._Sentensity;

                    _Camera._Yaw += _DeltaX;
                    _Camera._Pitch -= _DeltaY;
                }
            }
            if (Mouse.RightButton == MouseButtonState.Released)
            {
                _Camera._First = true;
            }
        }

        private void ImportModel_Click(object sender, RoutedEventArgs e)
        {
            _Process.ImportThread(ref _ModelLoader);   
        }

       



        
    }
}


