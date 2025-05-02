using Model = Load;
using Shader = ShaderSystem;
using Light = LightComponent;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class ShadowComponent
{

    private readonly int _FBO, _ShadowCount, _ShaddowWidth = 2048, _ShadowHeight = 2048;

    public ShadowComponent(int _LFBO, int _LShadowCount)
    {
        _FBO = _LFBO;
        _ShadowCount = _LShadowCount;   
    }

    public void DrawShadow(Shader _ModelShader, Shader _ShadowShader, Model _Model, List<Light> _Light)
    {
        GL.ActiveTexture(TextureUnit.Texture1);
        GL.BindTexture(TextureTarget.Texture2D, _ShadowCount);
        _ModelShader.UseShader();
        _ModelShader.SetInt("_ShadowMap", 1);
        for (int i = 0; i < _Light.Count; i++)
        {
            Matrix4 _LightProjOht = Matrix4.CreateOrthographicOffCenter(-35.0f, 35.0f, -35.0f, 35.0f, 0.1f, 75.0f);
            Matrix4 _LightView = Matrix4.LookAt(20.0f * _Light[i]._LightPosition, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            Matrix4 _LightProj = _LightProjOht * _LightView;
            _ModelShader.SetMatrix4("lightproj", _LightProj);
            _ShadowShader.UseShader();
            _ShadowShader.SetMatrix4("lightproj", _LightProj);
        }
    }
}
