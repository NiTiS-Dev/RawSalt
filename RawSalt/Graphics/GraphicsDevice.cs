using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;

namespace RawSalt.Graphics;

public abstract class GraphicsDevice : Disposable
{
	public abstract Adapter Adapter { get; }

	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

	public abstract Buffer CreateBuffer(in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData);
	public abstract ShaderModule CreateShader(ShaderStages stage, ReadOnlySpan<byte> code);
	public abstract BindGroupLayout CreateBindGroupLayout(params ReadOnlySpan<BindingEntry> bindings);
	public abstract Pipeline CreatePipeline(PipelineDescriptor descriptor);

	public Buffer CreateBuffer<T>(BufferUsage usage, ReadOnlySpan<T> data) where T : unmanaged
	{
		return CreateBuffer(new BufferDescriptor((ulong)(data.Length * Unsafe.SizeOf<T>()), usage),
			MemoryMarshal.AsBytes(data));
	}

	public abstract void WaitIdle();
}