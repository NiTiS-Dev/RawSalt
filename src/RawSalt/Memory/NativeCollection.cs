using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RawSalt.Memory;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(INativeCollectionDebugView<>))]
public sealed unsafe class NativeCollection<T> : IEnumerable<T>
	where T : unmanaged
{
	private const int DefaultCollectionSize = 16;

	private T* ptr;
	private nuint size;
	private nuint count;

	/// <summary>
	/// The size of collection in bytes.
	/// </summary>
	public nuint RealSize => size * (nuint)sizeof(T);
	
	/// <summary>
	/// Reference to the first element of collection.
	/// </summary>
	/// <remarks>
	/// May changes value if <see cref="Add(T)"/> is invoked!
	/// </remarks>
	public nint DataReference => (nint)ptr;

	/// <summary>
	/// Count of elements in collection.
	/// </summary>
	public ulong Count => count;

	/// <summary>
	/// Size of collection.
	/// </summary>
	public ulong Size => size;

	public NativeCollection() : this(DefaultCollectionSize) { }
	public NativeCollection(uint size)
	{
		this.size = size;

		this.ptr = (T*)NativeMemory.Alloc(RealSize);
		count = 0;
	}

	/// <summary>
	/// Adds new element to collection.
	/// </summary>
	/// <param name="item">Item to be added.</param>
	public void Add(T item)
	{
		EnsureFreeSpace();

		T* nextItem = ptr + count;

		*nextItem = item;
		count++;
	}
	
	/// <summary>
	/// Removes element on top of collection.
	/// </summary>
	/// <returns>The top element of collection.</returns>
	/// <exception cref="InvalidOperationException">The collection is empty.</exception>
	public T Pop()
	{
		if (count != 0)
			return ptr[--count];
		else
			throw new InvalidOperationException("Collection is empty.");
	}

	/// <inheritdoc/>
	public void CopyTo(T[] array, int arrayIndex)
	{
		if ((nuint)(array.Length + arrayIndex) > count)
			throw new ArgumentException("Array is too small.");

		else
		{
			fixed (T* pArray = array)
			{
				// ISSUE: copy data corruption! (2,3,4 indexes)
				NativeMemory.Copy(ptr, pArray + arrayIndex, count * (nuint)sizeof(T));
			}
		}
	}

	private void EnsureFreeSpace()
	{
		if (count == size)
		{
			nuint oldSize = RealSize;

			size *= 2;
			T* newPtr = (T*)NativeMemory.Realloc(ptr, RealSize);

			NativeMemory.Copy(ptr, newPtr, oldSize);
			ptr = newPtr;
		}
	}

	public IEnumerator<T> GetEnumerator()
		=> new Enumerator(this);

	IEnumerator IEnumerable.GetEnumerator() =>
			Count == 0 ? (IEnumerator<T>)Array.Empty<T>().GetEnumerator() :
			GetEnumerator();

	private sealed class Enumerator : IEnumerator<T>
	{
		private readonly NativeCollection<T> values;
		private nint index;

		public Enumerator(NativeCollection<T> values)
		{
			this.values = values;
			index = -1;
		}

		public T Current
			=> *(values.ptr + index);

		object IEnumerator.Current
			=> Current;

		public void Dispose() { }

		public bool MoveNext()
		{
			if ((nuint)index + 1 < values.count)
			{
				index++;
				return true;
			}

			return false;
		}

		public void Reset()
		{
			index = -1;
		}
	}
}