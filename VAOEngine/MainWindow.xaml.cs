using System.Windows;
using System.Windows.Input;
using OpenTK.Wpf;
using BackGround = BackgroundProcess;
using ModelLoad = Load;
using Shader = ShaderSystem;
using Camera = CameraSystem;
using SkyBox = SkyBoxComponent;
using Light = LightComponent;
using Debug = DebugComponent;
using Shadow = ShadowComponent;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;




namespace VAOEngine
{
    public partial class MainWindow : Window
    {
        private Shader _ModelShader, _SkyShader, _LightShader, _ShadowShader;
        private BackGround _Process = new BackGround();
        private string _ModelPos;
        private List<ModelLoad> _ModelLoader;
        private ModelLoad _TestModelForShadow;
        private List<Light> _LightLoader;
        private Light _TestLight;
        private Shadow _Shadow;
        private Camera _Camera;
        private int _FBO, _FBOShadow, _ShadowWidth = 2048, _ShadowHeight = 2048;
        private Debug _Debug;
        private SkyBox _SkyBox;
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
            
            _TestModelForShadow = new ModelLoad("D:\\VAOEngine\\VAOEngine\\MeshModel\\Cube.fbx");
            _TestLight = new Light(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

            GL.ClearColor(0.2f, 0.3f, 0.4f, 0.1f);
            _ModelLoader = new List<ModelLoad>();
            _LightLoader = new List<Light>();
            



            _ModelShader.UseShader();
              
            _SkyShader.UseShader();

            _FBO = GL.GenFramebuffer();
            _FBOShadow = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _FBOShadow);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, _ShadowWidth, _ShadowHeight, 0, PixelFormat.DepthComponent, PixelType.Float, (nint)null);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            int[] _ClampColor = new int[] { 1, 1, 1, 1 };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, _ClampColor);

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _Control.Framebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _FBOShadow, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            

            Matrix4 _LightProjOht = Matrix4.CreateOrthographicOffCenter(-35.0f, 35.0f, -35.0f, 35.0f, 0.1f, 75.0f);
            Matrix4 _LightView = Matrix4.LookAt(20.0f * _TestLight._LightPosition, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 _LightProj = _LightProjOht * _LightView;
            _ShadowShader.UseShader();
            //_ShadowShader.SetMatrix4("model", _TestModelForShadow._OutModel._ModelMatrixF);
            _ShadowShader.SetMatrix4("lightproj", _LightProj);

            _Camera = new Camera(Vector3.UnitZ * 1, (float)Width / (float)Height);
            _Process.IsVisible = false;
            _Shadow = new Shadow(_FBO, _FBOShadow);
            _Debug = new Debug();
            GL.DebugMessageCallback(_Debug._Debuger, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);





        }



        private void _Control_Render(TimeSpan obj)
        {
            GL.Enable(EnableCap.DepthTest);
            for (int i = 0; i<_ModelLoader.Count; i++)
            {
                GL.Viewport(0, 0, _ShadowWidth, _ShadowHeight);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                _Shadow.DrawShadow(_ModelShader, _ShadowShader, _ModelLoader[i], _LightLoader);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.Viewport(0, 0, (int)_Control.Width, (int)_Control.Height);
            }

            GL.Enable(EnableCap.CullFace);   
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.DebugMessageCallback(_Debug._Debuger, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);

            _Camera.InputCameraSystem();
            _ShadowShader.UseShader();
            _ModelShader.UseShader();
            


            for (int i = 0; i < _ModelLoader.Count; i++)
            {
                _ModelLoader[i].Draw(_ModelShader, _Camera);
                _ModelPos = _ModelLoader[i]._OutModel._MatrixModel._Position.ToString();
            }
            if(_LightLoader!=null)
            {
                for (int i = 0; i < _LightLoader.Count; i++)
                {
                    _LightLoader[i].DrawLight(_ModelShader, _LightShader, _Camera);
                                    
                }
            }
            if (_SkyBox != null)
            {
                _SkyBox.Draw(_SkyShader, _Camera);
         
            }

            

            if (Mouse.LeftButton==MouseButtonState.Released)
            {
                //var _Position = Mouse.GetPosition(this);
                
            }

            Log.Items.Add($"Main System Status:{GL.GetError().ToString()}");
            Log.Items.Add($"Model Shader Status:{_ModelShader._Log}");
            Log.Items.Add($"Shadow Shader Status:{_ShadowShader._Log}");
            
        }


        private void SkyBoxAdd_Click(object sender, RoutedEventArgs e)
        {
            _Process.AddSkyBox(ref _SkyBox, _SkyTexture);
        }
        private void LightAdd_Click(object sender, RoutedEventArgs e)
        {
            _Process.AddLight(ref _LightLoader);
            ListLightScene.Items.Add($"Light_{_LightLoader.Count}");
        }
        private void ImportModel_Click(object sender, RoutedEventArgs e)
        {
            _Process.ImportThread(ref _ModelLoader);
            ListSceneObject.Items.Add($"Mesh_{_ModelLoader.Count}");

        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            Log.Items.Clear();  
        }

        private void Destroy_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Items.Remove(OutputLog);
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


        
    }
}

