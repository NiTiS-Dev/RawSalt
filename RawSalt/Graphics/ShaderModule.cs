namespace RawSalt.Graphics;

public abstract class ShaderModule : Disposable
{
	public abstract ShaderStages Stage { get; }
}