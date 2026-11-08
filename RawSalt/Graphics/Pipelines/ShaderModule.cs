namespace RawSalt.Graphics.Pipelines;

public abstract class ShaderModule : Disposable
{
	public abstract ShaderStages Stage { get; }
}