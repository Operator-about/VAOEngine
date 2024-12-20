using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

public class ShaderSystem
{
    int _Count;

    public ShaderSystem(string _VertexPathShader, string _FragPathShader)
    {
        string _VertexSource = File.ReadAllText(_VertexPathShader);
        string _FragSource = File.ReadAllText(_FragPathShader);

        int _VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_VertexShader,_VertexSource);

        int _FragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_FragShader, _FragSource);

        GL.CompileShader(_VertexShader);
        GL.CompileShader(_FragShader);

        _Count = GL.CreateProgram();

        GL.AttachShader(_Count, _VertexShader);
        GL.AttachShader(_Count, _FragShader);

        GL.LinkProgram(_Count); 

    }

    public void UseShader()
    {
        GL.UseProgram(_Count);
    }
}