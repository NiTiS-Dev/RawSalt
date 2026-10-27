using System;
using System.Collections;
using System.Collections.Generic;

namespace RawSalt.Logging;

/// <summary>
/// Thread-safe small list. This list is not pre-allocate memory for future operations.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
internal sealed class PendingSmallList<T> : IEnumerable<T>
{
	private readonly Lock _lock = new();
	private T[] _items = [];

	public void Add(T item)
	{
		using (_lock.EnterScope())
		{
			Array.Resize(ref _items, _items.Length + 1);
			_items[^1] = item;
		}
	}

	public bool Remove(T item)
	{
		using (_lock.EnterScope())
		{
			int index = Array.IndexOf(_items, item);
			if (index < 0)
			{
				return false;
			}

			if (index < _items.Length - 1)
			{
				Array.Copy(_items, index + 1, _items, index, _items.Length - index - 1);
			}

			Array.Resize(ref _items, _items.Length - 1);
			return true;
		}
	}

	public T[] GetSnapshot()
	{
		return _items;
	}

	public T[] ClearAndGet()
	{
		using (_lock.EnterScope())
		{
			var oldItems = _items;
			_items = [];
			return oldItems;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(GetSnapshot());
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public struct Enumerator : IEnumerator<T>
	{
		private readonly T[] _items;
		private int _index;

		public Enumerator(T[] items)
		{
			_items = items;
			_index = -1;
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _items.Length;
		}

		public T Current => _items[_index];

		object IEnumerator.Current => Current!;

		public void Reset()
		{
			_index = -1;
		}

		public void Dispose()
		{
		}
	}
}

