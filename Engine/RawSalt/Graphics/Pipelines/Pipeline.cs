namespace RawSalt.Graphics.Pipelines;

public abstract class Pipeline : Disposable
{
	public abstract ShaderStages Stages { get; }
}