using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics.Pipelines;

/// <summary>
/// Main-thread safe handle to a GPU pipeline. GPU work happens on the render thread.
/// </summary>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public readonly struct Pipeline : IDisposable
{
	public GraphicsDevice? Owner { get; }
	public ResourceHandle Handle { get; }

	/// <summary>
	/// Whether the GPU pipeline exists. A pipeline created off the render thread is not ready
	/// until the render thread has created it.
	/// </summary>
	public bool IsReady => Owner?.IsPipelineReady(Handle) ?? false;

	internal Pipeline(GraphicsDevice owner, ResourceHandle handle)
	{
		Owner = owner;
		Handle = handle;
	}

	[ThreadSafety(AllowedThread.Render)]
	public void Dispose()
	{
		Owner?.Destroy(this);
	}
}
