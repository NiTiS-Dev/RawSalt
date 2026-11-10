using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Threading;

namespace RawSalt.Graphics;

[ThreadSafety(AllowedThread.Render)]
public abstract class GraphicsDevice : Disposable
{
	public abstract Adapter Adapter { get; }

	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

	public abstract Buffer CreateBuffer(in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData);
	public abstract Texture2D CreateTexture(in Texture2DDescriptor descriptor, ReadOnlySpan<byte> initialData);
	public abstract Pipeline CreatePipeline(PipelineDescriptor descriptor);

	public Buffer CreateBuffer<T>(BufferUsage usage, ReadOnlySpan<T> data) where T : unmanaged
	{
		return CreateBuffer(new BufferDescriptor((ulong)(data.Length * Unsafe.SizeOf<T>()), usage),
			MemoryMarshal.AsBytes(data));
	}

	public Texture2D CreateTexture(in Texture2DDescriptor descriptor)
	{
		return CreateTexture(descriptor, []);
	}

	public Texture2D CreateTexture(PixelMap2D pixels)
	{
		Texture2DDescriptor descriptor = new(new(pixels.Width, pixels.Height), pixels.Format);
		return CreateTexture(descriptor, pixels.GetBytes());
	}

	public abstract void WaitIdle();
}