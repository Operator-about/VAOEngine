using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Platform.Windows;
using StbImageSharp;
using System.IO;

public class TextureSystem
{

    public int _Count;
    public string _Path;
    public string _Type;
    
    public static TextureSystem Texture(string _File,string _Direct, string _Type)
    {
        string _FileName = new string(_File);
        _FileName = _Direct + '/' + _FileName;

        int _ID = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _ID);

        Stream _TextureFile = File.OpenRead(_FileName);

        ImageResult _Texture = ImageResult.FromStream(_TextureFile, ColorComponents.RedGreenBlueAlpha);

        GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba, _Texture.Width, _Texture.Height,0, PixelFormat.Rgba, PixelType.UnsignedByte, _Texture.Data);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new TextureSystem(_ID,_File,_Type);
    }

    public TextureSystem(int _ID, string _Path, string _Type)
    {
        this._Count = _ID;
        this._Path = _Path; 
        this._Type = _Type;
    }

    public void UseTexture(TextureUnit _Unit)
    {
        GL.ActiveTexture(_Unit);
        GL.BindTexture(TextureTarget.Texture2D, _Count);
    }
}
