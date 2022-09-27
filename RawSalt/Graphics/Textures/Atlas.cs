using System.Collections.Generic;

namespace RawSalt.Graphics.Textures;

public class Atlas
{
	private readonly Dictionary<string, Texture> textures = new();
	private readonly Dictionary<string, TextureRegion> regions = new();

	public Texture GetTexture(string textureName)
		=> textures[textureName];
	public TextureRegion GetRegion(string regionName)
		=> regions[regionName];

	public void AddRegion(TextureRegion region, string name)
		=> regions[name] = region;
	public void AddTexture(Texture texture, string name)
		=> textures[name] = texture;

}
