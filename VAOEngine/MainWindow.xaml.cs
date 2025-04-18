﻿using System.Windows;
using System.Windows.Input;
using OpenTK.Wpf;
using BackGround = BackgroundProcess;
using ModelLoad = Load;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using SkyBox = SkyBoxComponent;
using Light = LightComponent;
using Debug = DebugComponent;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;




namespace VAOEngine
{
    public partial class MainWindow : Window
    {
        private static string _LLog;
        private Shader _ModelShader, _SkyShader, _LightShader, _ShadowShader;
        private BackGround _Process = new BackGround();
        private string _ModelPos;
        private List<ModelLoad> _ModelLoader;
        private List<Light> _LightLoader;
        private Camera _Camera;
        private Debug _Debug;
        private SkyBox _SkyBox;
        private Matrix4 _OrthogProj, _LightView, _LightProj;
        private static List<string> _SkyTexture = new List<string>() {
        @".\SkyBoxTexture\SunSky.jpg",
        @".\SkyBoxTexture\SunSky.jpg",
        @".\SkyBoxTexture\SunSky.jpg",
        @".\SkyBoxTexture\SunSky.jpg",
        @".\SkyBoxTexture\SunSky.jpg",
        @".\SkyBoxTexture\SunSky.jpg" };

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
            GL.Enable(EnableCap.DepthTest);
            
            
            
            _SkyShader = new Shader(@$".\Shader\SkyBoxShader\VertSkyShader.glsl", @$".\Shader\SkyBoxShader\FragSkyShader.glsl");
            _ModelShader = new Shader(@$".\Shader\VertShader.glsl", @$".\Shader\ShaderForModel\FragShader.glsl");
            _LightShader = new Shader(@$".\Shader\TextureShaderLight\VertTextureLightShader.glsl", @$".\Shader\TextureShaderLight\FragTextureLightShader.glsl");
            _ShadowShader = new Shader(@$".\Shader\ShadowShader\VertShadowShader.glsl", @$".\Shader\ShadowShader\FragShadowShader.glsl");


            GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
            _ModelLoader = new List<ModelLoad>();
            _LightLoader = new List<Light>();
            _Debug = new Debug();
            
            

            _ModelShader.UseShader();
            _SkyShader.UseShader();
            _ShadowShader.UseShader();


            
            _Camera = new Camera(Vector3.UnitZ * 3, (float)Width / (float)Height);
            _Process.IsVisible = false;
           





        }



        private void _Control_Render(TimeSpan obj)
        {
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.DebugMessageCallback(_Debug._Debuger, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            _Camera.InputCameraSystem();

            _ModelShader.UseShader();
            _ShadowShader.UseShader();


            int _ModelCount = 0;

            if (_SkyBox!=null)
            {
                _SkyBox.Draw(_SkyShader, _Camera);
                ConsoleMessage.Items.Add($"SkyBox status:{_SkyBox._Log}");
            }
            for (int i = 0; i < _ModelLoader.Count; i++)
            {
                _ModelCount = i;
                _ModelLoader[i].Draw(_ModelShader, _Camera, _LightProj);
                _ModelPos = _ModelLoader[i]._OutModel._MatrixModel._Position.ToString();


            }
            if(_LightLoader!=null)
            {
                for (int i = 0; i < _LightLoader.Count; i++)
                {
                    _LightLoader[i].DrawLight(_ModelShader, _LightShader, _Camera);
                    _OrthogProj = Matrix4.CreateOrthographicOffCenter(-35.0f, 35.0f, -35.0f, 35.0f, 0.1f, 75.0f);
                    _LightView = Matrix4.LookAt(20.0f * _LightLoader[i]._LightPosition, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
                    _LightProj = _OrthogProj * _LightView;
                    if (_ModelLoader.Count>0)
                    {
                        _ModelLoader[_ModelCount].DrawShadow(_ModelShader, _ShadowShader, _Camera, _LightProj);
                    }
                    
                    
                }
            }
            
            ConsoleMessage.Items.Add($"Main status:{GL.GetError().ToString()}");
            ConsoleMessage.Items.Add($"Model shader status:{_ModelShader._Log}");
            ConsoleMessage.Items.Add($"SkyBox shader status:{_SkyShader._Log}");
            ConsoleMessage.Items.Add($"Light shader status:{_LightShader._Log}");
            ConsoleMessage.Items.Add($"Shadow shader status:{_ShadowShader._Log}");
            ConsoleMessage.Items.Add($"Viewport scale: W: {Width}, H: {Height}");

            Log.Text = _LLog;
            //Use main shader


            //ModelCount.Text = _ModelLoader.Count.ToString();
            ModelPosition.Text = _ModelPos;

            CameraPos.Text = $"X:{_Camera._Position.X.ToString()}.Y:{_Camera._Position.Y.ToString()}.Z:{_Camera._Position.Z.ToString()}";

            if (Mouse.LeftButton==MouseButtonState.Released)
            {
                //var _Position = Mouse.GetPosition(this);
                
            }

            
        }

        private void SkyBoxAdd_Click(object sender, RoutedEventArgs e)
        {
            _Process.AddSkyBox(ref _SkyBox, _SkyTexture);
        }

        private void LightAdd_Click(object sender, RoutedEventArgs e)
        {
            _Process.AddLight(ref _LightLoader);
        }

        private void _Control_MouseMove(object sender, MouseEventArgs e)
        {
            var _Position = Mouse.GetPosition(this);
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

        
    }
}

