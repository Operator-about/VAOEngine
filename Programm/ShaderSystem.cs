using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class ShaderSystem
{
    public int _Count;
    private readonly Dictionary<string, int> _UnifLoc;

    public ShaderSystem(string _VertexPathShader, string _FragPathShader)
    {
        _Count = GL.CreateProgram();
        string _VertexSource = File.ReadAllText(_VertexPathShader);
        string _FragSource = File.ReadAllText(_FragPathShader);

        int _VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_VertexShader,_VertexSource);

        int _FragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_FragShader, _FragSource);

        GL.CompileShader(_VertexShader);
        GL.GetShader(_VertexShader, ShaderParameter.CompileStatus, out int _Success);
        GL.AttachShader(_Count, _VertexShader);
        GL.AttachShader(_Count, _FragShader);

        GL.LinkProgram(_Count);
        


        GL.GetProgram(_Count, GetProgramParameterName.ActiveUniforms, out var _NumverUnif);
        _UnifLoc = new Dictionary<string, int>();

        for (var i = 0; i < _NumverUnif; i++)
        {
            var _Key = GL.GetActiveUniform(_Count, i, out _, out _);
            var _Location = GL.GetUniformLocation(_Count, _Key);

            _UnifLoc.Add(_Key, _Location);
        }

        if (_Success==0)
        {
            string _Log = GL.GetShaderInfoLog(_VertexShader);
            Console.WriteLine(_Log);
        }

        GL.CompileShader(_FragShader);
        GL.GetShader(_FragShader, ShaderParameter.CompileStatus, out int _SuccessF);
        if (_SuccessF == 0)
        {
            string _Log = GL.GetShaderInfoLog(_FragShader);
            Console.WriteLine(_Log);
        }

        

        GL.GetProgram(_Count,GetProgramParameterName.LinkStatus, out int _SuccessP);
        if (_SuccessP != 0)
        {
            string _Log = GL.GetProgramInfoLog(_Count);
            Console.WriteLine(_Log);
        }

    }

    public void UseShader()
    {
        GL.UseProgram(_Count);    
    }

    public void SetMatrix4(string _Name,Matrix4 _Parameter)
    {
        GL.UseProgram(_Count);
       
        GL.UniformMatrix4(GL.GetUniformLocation(_Count, _Name),true, ref _Parameter);
    }

    public void SetVector3(string _Name, Vector3 _Parameter)
    {
        GL.UseProgram(_Count);
        GL.Uniform3(GL.GetUniformLocation(_Count, _Name),_Parameter);
    }

    public int GetAttrib(string _Name)
    {
        return GL.GetAttribLocation(_Count, _Name); 
    }
}