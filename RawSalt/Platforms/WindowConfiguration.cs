namespace RawSalt.Platforms;

public struct WindowConfiguration
{
	/// <summary>
	/// Title shown in the topbar of the application widnow.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Initial window position within virtual space.
	/// </summary>
	public Vec2i? Position { get; set; }

	/// <summary>
	/// Size of the window in virtual space. Does not include window borders.
	/// </summary>
	public Vec2i Size { get; set; }
}