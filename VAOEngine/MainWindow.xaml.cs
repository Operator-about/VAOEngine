using System.Windows;
using System.Windows.Input;
using OpenTK;
using OpenTK.Wpf;
using BackGround = BackgroundProcess;
using ModelLoad = Load;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using SkyBox = SkyBoxComponent;
using Light = LightComponent;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Diagnostics;




namespace VAOEngine
{
    public partial class MainWindow : Window
    {
        private static string _LLog;
        private Shader _Shader, _ModelShader, _SkyShader, _LightShader;
        private BackGround _Process = new BackGround();
        private string _ModelPos;
        private List<ModelLoad> _ModelLoader;
        private Camera _Camera;
        private SkyBox _SkyBox;
        private Light _LightComponent;
        private static DebugProc _Debug = OnDebugMessage;
        private static List<string> _SkyTexture = new List<string>() {
        @".\SkyBoxTexture\CloudSky.jpg",
        @".\SkyBoxTexture\CloudSky.jpg",
        @".\SkyBoxTexture\CloudSky.jpg",
        @".\SkyBoxTexture\CloudSky.jpg",
        @".\SkyBoxTexture\CloudSky.jpg",
        @".\SkyBoxTexture\CloudSky.jpg" };

        public MainWindow()
        {

            InitializeComponent();



            var _Settings = new GLWpfControlSettings()
            {
                MajorVersion = 4,
                MinorVersion = 0,
                ContextFlags = OpenTK.Windowing.Common.ContextFlags.Debug

            };
            
           
            _Control.Start(_Settings);
            _Shader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragShader.glsl");
            _SkyShader = new Shader(@$".\Shader\VertSkyShader.glsl", @$".\Shader\FragSkyShader.glsl");
            _ModelShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragShader.glsl");
            _LightShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\FragLightShader.glsl");
            _LightComponent = new Light();
            GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
            _ModelLoader = new List<ModelLoad>();

            _SkyBox = SkyBox.LoadTexture(_SkyTexture);
            _SkyShader.UseShader();
            _SkyShader.SetInt("skybox", 0);



            _Camera = new Camera(Vector3.UnitZ * 3, (float)Width / (float)Height);
            _Process.IsVisible = false;
            _Shader.UseShader();

            



        }

        
        
        private void _Control_Render(TimeSpan obj)
        {
            
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _Camera.InputCameraSystem();

            _ModelShader.UseShader();

            _SkyBox.Draw(_SkyShader, _Camera);

            for (int i = 0; i < _ModelLoader.Count; i++)
            {
                _LightComponent.DrawLight(_LightShader, _Camera);
                _ModelLoader[i].Draw(_ModelShader, _Camera);
                _ModelPos = _ModelLoader[i]._OutModel._MatrixModel._Position.ToString();
            }

            

            ConsoleMessage.Items.Add($"SkyBox status:{_SkyBox._Log}");
            ConsoleMessage.Items.Add($"Main status:{GL.GetError().ToString()}");
            ConsoleMessage.Items.Add($"Shader status:{_Shader._Log}");
            ConsoleMessage.Items.Add($"SkyBox shader status:{_SkyShader._Log}");




            Log.Text = _LLog;
            //Use main shader
            _Shader.UseShader();

            ModelCount.Text = _ModelLoader.Count.ToString();
            ModelPosition.Text = _ModelPos;
            
            ShaderMainStatus.Text = $"Shader status:{_Shader.ToString()}";
            CameraPos.Text = $"X:{_Camera._Position.X.ToString()}.Y:{_Camera._Position.Y.ToString()}.Z:{_Camera._Position.Z.ToString()}";
        }

        private void SkyBoxAdd_Click(object sender, RoutedEventArgs e)
        {
            
            _SkyBox.UseTexture(TextureUnit.Texture0);
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

                    _Camera._Yaw += _DeltaX *= _Camera._Sentensity;
                    _Camera._Pitch -= _DeltaY *= _Camera._Sentensity;
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

        private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
        {
            
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            Debug.WriteLine("[{0} source={1} type={2} id={3}] {4}. " + severity + ". " + source + ". " + type + ". " + id + ". " + message);
            //_LLog = "[{0} source={1} type={2} id={3}] {4}. " + severity + ". " + source + ". " + type + ". " + id + ". " + message;

            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(message);
            }
        }








    }
}


