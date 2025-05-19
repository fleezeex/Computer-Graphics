using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System;
using System.IO;

namespace KolobokGame
{
    public class Texture : IDisposable
    {
        public readonly int Handle;

        public Texture(string path)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            try
            {
                using (Stream stream = File.OpenRead(path))
                {
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                                 image.Width, image.Height, 0,
                                 PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }
            }
            catch (Exception ex)
            {
                byte[] data = new byte[4] { 100, 100, 100, 255 };
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                              1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            }
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}