using System.IO;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

public class TextureComponent
{
    private readonly int _Count;

    public static TextureComponent AddTexture(string _FileTexture)
    {
        int _LCount = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _LCount);
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);

        var _File = File.OpenRead(_FileTexture);
        ImageResult _Texture = ImageResult.FromStream(_File, ColorComponents.RedGreenBlueAlpha);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _Texture.Width, _Texture.Height,
        0, PixelFormat.Rgba, PixelType.UnsignedByte, _Texture.Data);

        return new TextureComponent(_LCount);
    }

    public TextureComponent(int _HCount)
    {
        _Count = _HCount;
    }

    public void UseTexture(TextureUnit _Unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(_Unit);
        GL.BindTexture(TextureTarget.Texture2D, _Count);
    }

}