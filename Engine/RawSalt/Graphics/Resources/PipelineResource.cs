using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// GPU pipeline. Render thread only; users hold a main-thread-safe <see cref="Pipelines.Pipeline"/>
/// handle instead.
/// </summary>
[ThreadSafety(AllowedThread.Render)]
public abstract class PipelineResource : Disposable
{
	public abstract ShaderStages Stages { get; }
	public abstract PixelFormat ColorFormat { get; }
}
