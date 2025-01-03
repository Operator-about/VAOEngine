﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class ShaderSystem
{
    public int _Count;

    public ShaderSystem(string _VertexPathShader, string _FragPathShader)
    {
        string _VertexSource = File.ReadAllText(_VertexPathShader);
        string _FragSource = File.ReadAllText(_FragPathShader);

        int _VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(_VertexShader,_VertexSource);

        int _FragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(_FragShader, _FragSource);

        GL.CompileShader(_VertexShader);
        GL.GetShader(_VertexShader, ShaderParameter.CompileStatus, out int _Success);
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

        _Count = GL.CreateProgram();

        GL.AttachShader(_Count, _VertexShader);
        GL.AttachShader(_Count, _FragShader);

        GL.LinkProgram(_Count);
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
}