using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : Disposable
{
	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

	public abstract Buffer CreateBuffer(in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData);
	public abstract ShaderModule CreateShader(ShaderModuleStage stage, ReadOnlySpan<byte> code);
	public abstract Pipeline CreatePipeline(PipelineDescriptor descriptor);

	public Buffer CreateBuffer<T>(BufferUsage usage, ReadOnlySpan<T> data) where T : unmanaged
	{
		return CreateBuffer(new BufferDescriptor((ulong)(data.Length * Unsafe.SizeOf<T>()), usage),
			MemoryMarshal.AsBytes(data));
	}

	public abstract void WaitIdle();
}