﻿using Shader = ShaderSystem;
using Camera = CameraSystem;
using Debug = DebugComponent;
using Texture = TextureComponent;
using Light = LightComponent;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



public struct MeshPosScaleRot
{
    public Vector3 _Position;
    public Vector3 _Scale;
    public Vector3 _Rotation;
    public Vector3 _Color;
}


public struct VertexMesh
{
    public Vector3 _Position;
    public Vector3 _Normal;
    public Vector2 _TexCoord;
}


public class MeshComponent
{

    private readonly int _VAO;
    private readonly int _FBO, _FBOShadow;
    private readonly int _IndexG;
    private int _ShadowWidht = 2048, _ShadowHeight = 2048;
    public MeshPosScaleRot _MatrixModel;
    public Matrix4 _ModelMatrixF;
    private Debug _Debug;
    private Texture _Texture;


    public Matrix4 DrawMesh(Shader _Shader, Camera _Camera)
    {
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Multisample);

        

        //Bind VAO
        GL.BindVertexArray(_VAO);
        
        
        _MatrixModel._Position = new Vector3(0, 0, 0);
        _MatrixModel._Scale = new Vector3(1, 1, 1);
        _MatrixModel._Rotation = new Vector3(0, 0, 0);
        _MatrixModel._Color = new Vector3(1, 1, 1);

        //Set matrix for model
        _ModelMatrixF = Matrix4.Identity;
        var _PositionModelF = Matrix4.CreateTranslation(_MatrixModel._Position);
        var _ScaleMatrixModelF = Matrix4.CreateScale(_MatrixModel._Scale);
        var _MatrixX = Matrix4.CreateRotationX(_MatrixModel._Rotation.X);
        var _MatrixY = Matrix4.CreateRotationY(_MatrixModel._Rotation.Y);
        var _MatrixZ = Matrix4.CreateRotationZ(_MatrixModel._Rotation.Z);

        _ModelMatrixF = _PositionModelF * _ScaleMatrixModelF * (_MatrixX * _MatrixY * _MatrixZ);

        //Use shader and bind
        _Texture.UseTexture();
        _Shader.UseShader();
        _Shader.SetMatrix4("model", _ModelMatrixF);
        _Shader.SetMatrix4("view", _Camera.GetView());
        _Shader.SetMatrix4("proj", _Camera.GetProjection());
        _Shader.SetBool("_TextureBindValid", true);
        _Shader.SetVector3("_Material.ModelColor", _MatrixModel._Color);
        _Shader.SetInt("_Material.Diffuse2D", 0);
        _Shader.SetVector3("_Material.Ambient", new Vector3(0.0f, 0.1f, 0.06f));
        _Shader.SetVector3("_Material.Diffuse", new Vector3(0.0f, 0.50980392f, 0.50980392f));
        _Shader.SetVector3("_Material.Specular", new Vector3(0.50196078f, 0.50196078f, 0.50196078f));
        _Shader.SetFloat("_Material.Shininess", 32.0f);
        
        
        


        //Draw
        GL.DrawElements(PrimitiveType.Triangles, _IndexG, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
       

        GL.DebugMessageCallback(_Debug._Debuger, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
        GL.Enable(EnableCap.DebugOutputSynchronous);
        return _ModelMatrixF;
    }

    public void DrawShadow(Shader _ModelShader, Shader _ShadowShader, List<Light> _Light, Camera _Camera)
    {

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);

        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _FBOShadow);
        _ModelShader.UseShader();
        _ModelShader.SetInt("_ShadowMap", 1);
        for (int i = 0; i < _Light.Count; i++)
        {
            Matrix4 _LightProjOht = Matrix4.CreateOrthographicOffCenter(-35.0f, 35.0f, -35.0f, 35.0f, 0.1f, 75.0f);
            Matrix4 _LightView = Matrix4.LookAt(20.0f * _Light[i]._LightPosition, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 _LightProj = _LightProjOht * _LightView;
            _ModelShader.SetMatrix4("lightproj", _LightProj);
            _ShadowShader.UseShader();
            _ShadowShader.SetMatrix4("model", _ModelMatrixF);
            _ShadowShader.SetMatrix4("lightproj", _LightProj);
        }
        
        
    }

    

   

    public MeshComponent(Span<VertexMesh> _Vertex, Span<int> _Index)
    {
        
        _Debug = new Debug();
        _IndexG = _Index.Length;


        //Bind
        _VAO = GL.GenVertexArray();
        _Texture = Texture.AddTexture(@".\SkyBoxTexture\CloudSky.jpg");
        _Texture.UseTexture();
        int _VBO = GL.GenBuffer();
        int _EBO = GL.GenBuffer();
        _FBO = GL.GenFramebuffer();
        _FBOShadow = GL.GenTexture();

        GL.BindVertexArray(_VAO);


        //Report data in buffer VBO
        Console.WriteLine("Report data in buffer: Vertex");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, _Vertex.Length * Unsafe.SizeOf<VertexMesh>(), ref MemoryMarshal.GetReference(_Vertex), BufferUsageHint.StaticDraw);

        //Report data in buffer EBO
        Console.WriteLine("Report data in buffer: Index");
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _Index.Length * sizeof(int), ref MemoryMarshal.GetReference(_Index), BufferUsageHint.StaticDraw);

        //Position
        Console.WriteLine("Load Attribute: 0,3");
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Position)));


        //Normal
        Console.WriteLine("Load Attribute: 1,3");
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._Normal)));


        //TexCoord
        GL.EnableVertexAttribArray(2);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<VertexMesh>(), Marshal.OffsetOf<VertexMesh>(nameof(VertexMesh._TexCoord)));





        GL.BindTexture(TextureTarget.Texture2D, _FBOShadow);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, _ShadowWidht, _ShadowHeight, 0, PixelFormat.DepthComponent, PixelType.Float, (nint)null);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

        int[] _ClampColor = new int[] { 1, 1, 1, 1 };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, _ClampColor);

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBO);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _FBOShadow, 0);
        GL.DrawBuffer(DrawBufferMode.None);
        GL.ReadBuffer(ReadBufferMode.None);
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        GL.BindVertexArray(0);
    }
}


