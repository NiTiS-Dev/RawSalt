using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// Slot-based ownership of GPU resources addressed by <see cref="ResourceHandle"/>.
/// </summary>
/// <remarks>
/// A slot can be reserved on any thread and filled later on the render thread. Reserving or
/// releasing a slot bumps its generation, so handles to earlier uses stop resolving immediately.
/// </remarks>
[ThreadSafety(AllowedThread.Main | AllowedThread.Render)]
public sealed class ResourcePool<T> : Disposable where T : class, IDisposable
{
	private readonly Lock _gate = new();
	private T?[] _items;
	private int[] _generations;
	private readonly Stack<int> _free = new();
	private int _next;

	public ResourcePool(int capacity = 8)
	{
		_items = new T[capacity];
		_generations = new int[capacity];
	}

	public ResourceHandle Add(T resource)
	{
		ArgumentNullException.ThrowIfNull(resource);
		lock (_gate)
		{
			ThrowIfDisposed();
			ResourceHandle handle = TakeLocked();
			_items[handle.Index] = resource;
			return handle;
		}
	}

	/// <summary>
	/// Claims an empty slot without a resource. Fill it later with <see cref="Set"/>.
	/// </summary>
	public ResourceHandle Reserve()
	{
		lock (_gate)
		{
			ThrowIfDisposed();
			return TakeLocked();
		}
	}

	/// <summary>
	/// Fills a slot claimed by <see cref="Reserve"/>.
	/// </summary>
	/// <returns><see langword="false"/> when the slot was released in the meantime.</returns>
	public bool Set(ResourceHandle handle, T resource)
	{
		ArgumentNullException.ThrowIfNull(resource);
		lock (_gate)
		{
			ThrowIfDisposed();
			if (!IsReservedLocked(handle))
			{
				return false;
			}

			_items[handle.Index] = resource;
			return true;
		}
	}

	/// <summary>
	/// Whether the slot is filled. A reserved but not yet filled slot is not ready.
	/// </summary>
	public bool IsReady(ResourceHandle handle)
	{
		lock (_gate)
		{
			return IsLiveLocked(handle, out _);
		}
	}

	public bool TryGet(ResourceHandle handle, [NotNullWhen(true)] out T? resource)
	{
		lock (_gate)
		{
			return IsLiveLocked(handle, out resource);
		}
	}

	public T Get(ResourceHandle handle)
	{
		lock (_gate)
		{
			if (IsLiveLocked(handle, out T? resource))
			{
				return resource;
			}
		}

		throw new ObjectDisposedException(typeof(T).Name, "The resource handle is stale.");
	}

	/// <summary>
	/// Invalidates <paramref name="handle"/> and returns the resource so the caller can retire it
	/// after the GPU is done with it.
	/// </summary>
	/// <returns>The released resource, or <see langword="null"/> when the handle was already stale.</returns>
	public T? Release(ResourceHandle handle)
	{
		lock (_gate)
		{
			if ((uint)handle.Index >= (uint)_items.Length || _generations[handle.Index] != handle.Generation)
			{
				return null;
			}

			T? resource = _items[handle.Index];
			InvalidateLocked(handle.Index);
			return resource;
		}
	}

	private ResourceHandle TakeLocked()
	{
		int index;
		if (_free.TryPop(out int free))
		{
			index = free;
		}
		else
		{
			if (_next == _items.Length)
			{
				Grow();
			}
			index = _next++;
		}
		Debug.Assert(_items[index] == null);

		return new ResourceHandle(index, Bump(index));
	}

	private void InvalidateLocked(int index)
	{
		_items[index] = null;
		Bump(index);
		_free.Push(index);
	}

	// Generation 0 is reserved, so a default handle never resolves.
	private int Bump(int index)
	{
		int generation = _generations[index] + 1;
		_generations[index] = generation == 0 ? 1 : generation;
		return _generations[index];
	}

	private bool IsLiveLocked(ResourceHandle handle, [NotNullWhen(true)] out T? resource)
	{
		resource = null;
		if ((uint)handle.Index >= (uint)_items.Length || _generations[handle.Index] != handle.Generation)
		{
			return false;
		}

		resource = _items[handle.Index];
		return resource != null;
	}

	private bool IsReservedLocked(ResourceHandle handle)
	{
		return (uint)handle.Index < (uint)_items.Length &&
		       _generations[handle.Index] == handle.Generation &&
		       _items[handle.Index] == null;
	}

	private void Grow()
	{
		int capacity = _items.Length * 2;
		Array.Resize(ref _items, capacity);
		Array.Resize(ref _generations, capacity);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		lock (_gate)
		{
			for (int i = 0; i < _next; i++)
			{
				_items[i]?.Dispose();
				_items[i] = null;
			}

			_free.Clear();
			_next = 0;
		}
	}
}
