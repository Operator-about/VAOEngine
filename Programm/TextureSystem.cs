using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

class TextureSystem
{

    public int _Count;

   


    public void LoadTexture(string _FileTexture)
    {
        _Count = GL.GenTexture();

        Console.WriteLine("Load Texture from file...");
        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult _Texture = ImageResult.FromStream(File.OpenRead(_FileTexture), ColorComponents.RedGreenBlueAlpha);
        Console.WriteLine("Load Texture parameter");
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 800, 800, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _Texture.Data);

        float[] _Boarder =
        {
            1.0f, 1.0f, 0.0f, 1.0f
        };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, _Boarder);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        

        Console.WriteLine("Use Texture");
        UseTexture();
    }

    private void UseTexture()
    {
        GL.BindTexture(TextureTarget.Texture2D, _Count);
    }
}
