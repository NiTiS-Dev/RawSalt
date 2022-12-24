//using NiTiS.IO;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.PixelFormats;
//using System;
//using File = NiTiS.IO.File;

//namespace RawSalt.Graphics.Textures;

///// <summary>
///// OpenGL Texture2D object
///// </summary>
//public readonly struct Texture
//{
//	public readonly uint Handle;
//	public Texture(uint handle)
//	{
//		Handle = handle;
//	}
//	public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
//	{
//		Handle = gl.GenTexture();
//		Bind(gl);

//		fixed (void* d = &data[0])
//		{
//			gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
//			SetParameters(gl);
//		}
//	}
//	private void SetParameters(GL gl)
//	{
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
//		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
//		gl.GenerateMipmap(TextureTarget.Texture2D);
//	}
//	public void Bind(GL gl, TextureUnit unit = TextureUnit.Texture0)
//	{
//		gl.ActiveTexture(unit);
//		gl.BindTexture(TextureTarget.Texture2D, Handle);
//	}
//	public void Bind(GL gl, int unit)
//	{
//		gl.ActiveTexture((TextureUnit)(unit + (int)TextureUnit.Texture0));
//		gl.BindTexture(TextureTarget.Texture2D, Handle);
//	}
//	public static explicit operator TextureRegion(Texture texture)
//		=> new(texture, vec2.Zero, vec2.One);


//	public static unsafe Texture Create(GL gl, File image)
//	{
//		Texture tex = new(gl.GenTexture());
//		tex.Bind(gl);

//		using Image<Rgba32> img = Image.Load<Rgba32>(image.Path);

//		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

//		img.ProcessPixelRows(accessor =>
//		{ 
//			for (int y = 0; y < accessor.Height; y++)
//			{
//				fixed (void* data = accessor.GetRowSpan(y))
//				{
//					gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, accessor.Height - y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
//				}
//			}
//		});
//		tex.SetParameters(gl);

//		return tex;
//	}
//	public void Dispose(GL gl)
//		=> gl.DeleteTexture(Handle);
//}