using System.Diagnostics.CodeAnalysis;
using RawSalt.Graphics.Drawing;
using RawSalt.Threading;

namespace RawSalt.Graphics.Rendering;

[ThreadSafety(AllowedThread.Render)]
public abstract class Renderer : Disposable
{
	public abstract PixelFormat BackBufferFormat { get; }

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
}