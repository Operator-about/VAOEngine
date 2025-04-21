using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Microsoft.Win32;
using ModelLoad = Load;
using Light = LightComponent;
using SkyBox = SkyBoxComponent;

class BackgroundProcess : GameWindow
{

    public BackgroundProcess() : base(GameWindowSettings.Default, new NativeWindowSettings())
    {
        Size = new Vector2i(10, 10);
        Title = "BackgroundProcess don't responde";

    }

    public List<ModelLoad> ImportThread(ref List<ModelLoad> _Loader)
    {
        var _File = new OpenFileDialog();
        if (_File.ShowDialog() == true)
        {
            _Loader.Add(new ModelLoad(_File.FileName));
        }
        return _Loader;
    }

    public List<Light> AddLight(ref List<Light> _Lighter)
    {
        _Lighter.Add(new Light(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(10.0f, 10.0f, 10.0f)));

        return _Lighter;
    }

    public SkyBox AddSkyBox(ref SkyBox _SkyBox, List<string> _TextureSky)
    {
        _SkyBox = SkyBox.LoadTexture(_TextureSky);
        return _SkyBox;
    }
}