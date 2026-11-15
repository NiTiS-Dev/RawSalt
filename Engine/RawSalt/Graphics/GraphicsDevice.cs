using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RawSalt.Graphics.Devices;
using RawSalt.Graphics.Drawing;
using RawSalt.Graphics.Pipelines;
using RawSalt.Graphics.Rendering;
using RawSalt.Graphics.Resources;
using RawSalt.Threading;

namespace RawSalt.Graphics;

[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public abstract class GraphicsDevice : Disposable
{
	private readonly ResourcePool<TextureResource> _textures = new();
	private readonly ResourcePool<BufferResource> _buffers = new();
	private readonly Queue<(IDisposable Resource, ulong Frame)> _retired = new();
	private readonly ConcurrentQueue<Action> _pending = new();
	private readonly int _renderThreadId = Environment.CurrentManagedThreadId;
	private ulong _currentFrame;

	public abstract Adapter Adapter { get; }

	public abstract Swapchain CreateSwapchain(RenderSurface surface);
	public abstract Renderer CreateRenderer(Swapchain swapchain);

	protected abstract BufferResource CreateBufferResource(in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData);
	protected abstract TextureResource CreateTextureResource(in TextureDescriptor descriptor, ReadOnlySpan<byte> initialData);
	public abstract Pipeline CreatePipeline(PipelineDescriptor descriptor);

	public Buffer CreateBuffer(in BufferDescriptor descriptor, ReadOnlySpan<byte> initialData)
	{
		AssertRenderThread();
		BufferResource resource = CreateBufferResource(descriptor, initialData);
		return new(this, _buffers.Add(resource), resource.Size);
	}

	public Buffer CreateBuffer<T>(BufferUsage usage, ReadOnlySpan<T> data) where T : unmanaged
	{
		return CreateBuffer(new BufferDescriptor((ulong)(data.Length * Unsafe.SizeOf<T>()), usage),
			MemoryMarshal.AsBytes(data));
	}

	public Texture1D CreateTexture(int width, PixelFormat format, Sampling sampling, ReadOnlySpan<byte> initialData)
	{
		TextureDescriptor descriptor = TextureDescriptor.Create(width, format, sampling);
		return new(this, CreateTextureHandle(descriptor, initialData), descriptor.Format, descriptor.Size.X);
	}

	public Texture1D CreateTexture(int width, PixelFormat format, Sampling sampling)
	{
		return CreateTexture(width, format, sampling, []);
	}

	public Texture2D CreateTexture(int width, int height, PixelFormat format, Sampling sampling,
		ReadOnlySpan<byte> initialData)
	{
		TextureDescriptor descriptor = TextureDescriptor.Create(width, height, format, sampling);
		return new(this, CreateTextureHandle(descriptor, initialData), descriptor.Format,
			new(descriptor.Size.X, descriptor.Size.Y));
	}

	public Texture2D CreateTexture(int width, int height, PixelFormat format, Sampling sampling)
	{
		return CreateTexture(width, height, format, sampling, []);
	}

	public Texture3D CreateTexture(int width, int height, int depth, PixelFormat format, Sampling sampling,
		ReadOnlySpan<byte> initialData)
	{
		TextureDescriptor descriptor = TextureDescriptor.Create(width, height, depth, format, sampling);
		return new(this, CreateTextureHandle(descriptor, initialData), descriptor.Format, descriptor.Size);
	}

	public Texture3D CreateTexture(int width, int height, int depth, PixelFormat format, Sampling sampling)
	{
		return CreateTexture(width, height, depth, format, sampling, []);
	}

	public TextureCube CreateTextureCube(int width, int height, PixelFormat format, Sampling sampling,
		ReadOnlySpan<byte> initialData)
	{
		TextureDescriptor descriptor = TextureDescriptor.CreateCube(width, height, format, sampling);
		return new(this, CreateTextureHandle(descriptor, initialData), descriptor.Format, descriptor.Size.X);
	}

	public TextureCube CreateTextureCube(int width, int height, PixelFormat format, Sampling sampling)
	{
		return CreateTextureCube(width, height, format, sampling, []);
	}

	public Texture2D CreateTexture(PixelMap2D pixels, Sampling sampling)
	{
		return CreateTexture(pixels.Width, pixels.Height, pixels.Format, sampling, pixels.GetBytes());
	}

	/// <summary>
	/// Creates the GPU texture on the render thread, or reserves its slot and queues the work when
	/// called from another thread.
	/// </summary>
	private ResourceHandle CreateTextureHandle(in TextureDescriptor descriptor, ReadOnlySpan<byte> initialData)
	{
		if (IsRenderThread)
		{
			return _textures.Add(CreateTextureResource(descriptor, initialData));
		}

		// Lambdas cannot capture `in` parameters, and the span must outlive this call.
		TextureDescriptor captured = descriptor;
		byte[] data = [.. initialData];
		ResourceHandle handle = _textures.Reserve();
		_pending.Enqueue(() =>
		{
			TextureResource resource = CreateTextureResource(captured, data);
			if (!_textures.Set(handle, resource))
			{
				// The handle was disposed before the render thread got to create it.
				resource.Dispose();
			}
		});
		return handle;
	}

	/// <summary>
	/// Whether the GPU texture exists. Textures created off the render thread are not ready until
	/// the render thread has created them.
	/// </summary>
	public bool IsTextureReady(ResourceHandle handle)
	{
		return _textures.IsReady(handle);
	}

	public TextureResource GetTextureResource(Texture1D texture)
	{
		return GetTextureResource(texture.Owner, texture.Handle, nameof(texture));
	}

	public TextureResource GetTextureResource(Texture2D texture)
	{
		return GetTextureResource(texture.Owner, texture.Handle, nameof(texture));
	}

	public TextureResource GetTextureResource(Texture3D texture)
	{
		return GetTextureResource(texture.Owner, texture.Handle, nameof(texture));
	}

	public TextureResource GetTextureResource(TextureCube texture)
	{
		return GetTextureResource(texture.Owner, texture.Handle, nameof(texture));
	}

	public TextureResource GetTextureResource(GraphicsDevice? owner, ResourceHandle handle, string parameterName)
	{
		AssertRenderThread();
		ValidateOwner(owner, parameterName);
		return _textures.Get(handle);
	}

	public BufferResource GetBufferResource(Buffer buffer)
	{
		AssertRenderThread();
		ValidateOwner(buffer.Owner, nameof(buffer));
		return _buffers.Get(buffer.Handle);
	}

	public void UpdateTexture(Texture1D texture, ReadOnlySpan<byte> pixels)
	{
		GetTextureResource(texture).Update(pixels);
	}

	public void UpdateTexture(Texture2D texture, ReadOnlySpan<byte> pixels)
	{
		GetTextureResource(texture).Update(pixels);
	}

	public void UpdateTexture(Texture3D texture, ReadOnlySpan<byte> pixels)
	{
		GetTextureResource(texture).Update(pixels);
	}

	public void UpdateTexture(TextureCube texture, ReadOnlySpan<byte> pixels)
	{
		GetTextureResource(texture).Update(pixels);
	}

	public void UpdateTexture(Texture2D texture, PixelMap2D pixels)
	{
		GetTextureResource(texture).Update(pixels);
	}

	public byte[] GetTexturePixels(Texture1D texture)
	{
		return GetTextureResource(texture).GetPixels();
	}

	public byte[] GetTexturePixels(Texture2D texture)
	{
		return GetTextureResource(texture).GetPixels();
	}

	public byte[] GetTexturePixels(Texture3D texture)
	{
		return GetTextureResource(texture).GetPixels();
	}

	public byte[] GetTexturePixels(TextureCube texture)
	{
		return GetTextureResource(texture).GetPixels();
	}

	internal void Destroy(Texture1D texture)
	{
		Retire(_textures, texture.Owner, texture.Handle, nameof(texture));
	}

	internal void Destroy(Texture2D texture)
	{
		Retire(_textures, texture.Owner, texture.Handle, nameof(texture));
	}

	internal void Destroy(Texture3D texture)
	{
		Retire(_textures, texture.Owner, texture.Handle, nameof(texture));
	}

	internal void Destroy(TextureCube texture)
	{
		Retire(_textures, texture.Owner, texture.Handle, nameof(texture));
	}

	internal void Destroy(Buffer buffer)
	{
		Retire(_buffers, buffer.Owner, buffer.Handle, nameof(buffer));
	}

	private void Retire<T>(ResourcePool<T> pool, GraphicsDevice? owner, ResourceHandle handle, string parameterName)
		where T : class, IDisposable
	{
		ValidateOwner(owner, parameterName);

		// Release on the calling thread so the handle is stale immediately, even from the main thread.
		if (pool.Release(handle) is not { } resource)
		{
			return;
		}

		if (IsRenderThread)
		{
			RetireNow(resource);
			return;
		}

		_pending.Enqueue(() => RetireNow(resource));
	}

	private void RetireNow(IDisposable resource)
	{
		AssertRenderThread();
		_retired.Enqueue((resource, _currentFrame));
	}

	/// <summary>
	/// Marks the start of a new frame being recorded and returns its id.
	/// </summary>
	public ulong BeginFrame()
	{
		AssertRenderThread();
		DrainPending();
		return ++_currentFrame;
	}

	private void DrainPending()
	{
		while (_pending.TryDequeue(out Action? action))
		{
			action();
		}
	}

	/// <summary>
	/// Disposes resources destroyed up to <paramref name="completedFrame"/>. Call after the GPU
	/// finished that frame.
	/// </summary>
	public void CollectGarbage(ulong completedFrame)
	{
		AssertRenderThread();
		while (_retired.TryPeek(out (IDisposable Resource, ulong Frame) entry) && entry.Frame <= completedFrame)
		{
			_retired.Dequeue();
			entry.Resource.Dispose();
		}
	}

	public abstract void WaitIdle();

	[Conditional("DEBUG")]
	protected void AssertRenderThread()
	{
		Debug.Assert(IsRenderThread, "Graphics device used off the render thread.");
	}

	private bool IsRenderThread => Environment.CurrentManagedThreadId == _renderThreadId;

	private void ValidateOwner(GraphicsDevice? owner, string parameterName)
	{
		if (!ReferenceEquals(owner, this))
		{
			throw new ArgumentException("Resource was created by a different device.", parameterName);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		AssertRenderThread();
		DrainPending();
		WaitIdle();
		CollectGarbage(ulong.MaxValue);
		_textures.Dispose();
		_buffers.Dispose();
	}
}