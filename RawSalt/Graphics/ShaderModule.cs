namespace RawSalt.Graphics;

public abstract class ShaderModule : IDisposable
{
	protected abstract void Dispose(bool disposing);

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	~ShaderModule()
	{
		Dispose(false);
	}
}