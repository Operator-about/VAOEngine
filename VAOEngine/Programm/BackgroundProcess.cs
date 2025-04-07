using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using Microsoft.Win32;
using ModelLoad = Load;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

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
}