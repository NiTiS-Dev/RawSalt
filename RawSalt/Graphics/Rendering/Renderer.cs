using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Graphics.Rendering;

public abstract class Renderer : IDisposable
{
	/// <summary>
	/// Try to begin drawing the frame.
	/// </summary>
	/// <returns>
	/// <see langword="true"/> when begin drawing frame. In case renderer can't start drawing returns <see langword="false"/>.
	/// </returns>
	public abstract bool TryBeginFrame([NotNullWhen(true)] out RenderFrame? frame);

	/// <summary>
	/// Finishes recording, submits and presents frame.
	/// </summary>
	public abstract void EndFrame(RenderFrame frame);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected abstract void Dispose(bool disposing);

	~Renderer()
	{
		Dispose(false);
	}
}