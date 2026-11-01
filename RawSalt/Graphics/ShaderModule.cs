namespace RawSalt.Graphics;

public abstract class ShaderModule : Disposable
{
	public abstract ShaderModuleStage Stage { get; }
}