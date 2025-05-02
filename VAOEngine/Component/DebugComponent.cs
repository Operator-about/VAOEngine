using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Windows;

public class DebugComponent
{

    private static DebugProc _LocalDebuger = OnDebugMessage;
    public DebugProc _Debuger;


    public DebugComponent()
    {
        _Debuger = _LocalDebuger;
    }

    private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
    {

        string message = Marshal.PtrToStringAnsi(pMessage, length);


        if (type == DebugType.DebugTypeError)
        {
            //MessageBox.Show(message);
            throw new Exception(message);
        }
    }
}
