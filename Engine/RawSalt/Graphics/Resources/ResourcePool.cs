using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RawSalt.Threading;

namespace RawSalt.Graphics.Resources;

/// <summary>
/// Slot-based ownership of GPU resources addressed by <see cref="ResourceHandle"/>.
/// </summary>
[ThreadSafety(AllowedThread.Render)]
public sealed class ResourcePool<T> : Disposable where T : class, IDisposable
{
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
		ThrowIfDisposed();

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

		// Generation 0 is reserved, so a default handle never resolves.
		int generation = _generations[index] + 1;
		_generations[index] = generation == 0 ? 1 : generation;
		_items[index] = resource;
		return new(index, _generations[index]);
	}

	public bool TryGet(ResourceHandle handle, [NotNullWhen(true)] out T? resource)
	{
		resource = null;
		if ((uint)handle.Index >= (uint)_items.Length || _generations[handle.Index] != handle.Generation)
		{
			return false;
		}

		resource = _items[handle.Index];
		return resource != null;
	}

	public T Get(ResourceHandle handle)
	{
		if (TryGet(handle, out T? resource))
		{
			return resource;
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
		if (!TryGet(handle, out T? resource))
		{
			return null;
		}

		_items[handle.Index] = null;
		_free.Push(handle.Index);
		return resource;
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

		for (int i = 0; i < _next; i++)
		{
			_items[i]?.Dispose();
			_items[i] = null;
		}

		_free.Clear();
		_next = 0;
	}
}