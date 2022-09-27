using Silk.NET.OpenGL;

namespace RawSalt.Graphics.Textures;

/// <summary>
/// OpenGL Texture2D object
/// </summary>
public readonly struct Texture
{
	public readonly uint Handle;
	public Texture(uint handle)
	{
		Handle = handle;
	}

	public void Bind(GL gl, TextureUnit unit)
	{
		gl.ActiveTexture(unit);
		gl.BindTexture(TextureTarget.Texture2D, Handle);
	}
	public void Bind(GL gl, int unit)
	{
		gl.ActiveTexture((TextureUnit)(unit + (int)TextureUnit.Texture0));
		gl.BindTexture(TextureTarget.Texture2D, Handle);
	}
	public static explicit operator TextureRegion(Texture texture)
		=> new(texture, vec2.Zero, vec2.One);
}
