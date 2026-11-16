using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace RawSalt.Collections;

public sealed class SmallDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	where TKey : notnull
{
	private struct Entry
	{
		public TKey Key;
		public TValue Value;

		public Entry(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}
	}

	private Entry[] _entries = [];
	private IEqualityComparer<TKey>? _comparer;

	public SmallDictionary()
	{
		_comparer = null;
	}

	public SmallDictionary(IEqualityComparer<TKey> comparer)
	{
		_comparer = comparer;
	}

	public SmallDictionary(IDictionary<TKey, TValue> dictionary)
		: this(dictionary, null)
	{
	}

	public SmallDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
		: this(comparer)
	{
		if (dictionary is null)
		{
			throw new ArgumentNullException(nameof(dictionary));
		}

		foreach (var pair in dictionary)
		{
			Add(pair.Key, pair.Value);
		}
	}

	private IEqualityComparer<TKey> Comparer => _comparer ?? EqualityComparer<TKey>.Default;

	public TValue this[TKey key]
	{
		get
		{
			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (TryGetValue(key, out var value))
			{
				return value;
			}

			throw new KeyNotFoundException();
		}
		set
		{
			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			int index = FindEntry(key);
			if (index >= 0)
			{
				_entries[index] = new Entry(key, value);
				return;
			}

			Array.Resize(ref _entries, _entries.Length + 1);
			_entries[^1] = new Entry(key, value);
		}
	}

	public ICollection<TKey> Keys
	{
		get
		{
			var keys = new TKey[_entries.Length];
			for (int i = 0; i < _entries.Length; i++)
			{
				keys[i] = _entries[i].Key;
			}
			return keys;
		}
	}

	public ICollection<TValue> Values
	{
		get
		{
			var values = new TValue[_entries.Length];
			for (int i = 0; i < _entries.Length; i++)
			{
				values[i] = _entries[i].Value;
			}
			return values;
		}
	}

	public int Count => _entries.Length;

	public bool IsReadOnly => false;

	public void Add(TKey key, TValue value)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		if (ContainsKey(key))
		{
			throw new ArgumentException("An item with the same key has already been added.", nameof(key));
		}

		Array.Resize(ref _entries, _entries.Length + 1);
		_entries[^1] = new Entry(key, value);
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public void Clear()
	{
		_entries = [];
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		if (TryGetValue(item.Key, out var value))
		{
			return EqualityComparer<TValue>.Default.Equals(value, item.Value);
		}

		return false;
	}

	public bool ContainsKey(TKey key)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		return FindEntry(key) >= 0;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array is null)
		{
			throw new ArgumentNullException(nameof(array));
		}

		if (arrayIndex < 0 || arrayIndex > array.Length)
		{
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));
		}

		if (array.Length - arrayIndex < _entries.Length)
		{
			throw new ArgumentException("Destination array is not large enough.");
		}

		for (int i = 0; i < _entries.Length; i++)
		{
			array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(_entries[i].Key, _entries[i].Value);
		}
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		for (int i = 0; i < _entries.Length; i++)
		{
			yield return new KeyValuePair<TKey, TValue>(_entries[i].Key, _entries[i].Value);
		}
	}

	public bool Remove(TKey key)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		int index = FindEntry(key);
		if (index < 0)
		{
			return false;
		}

		if (index < _entries.Length - 1)
		{
			Array.Copy(_entries, index + 1, _entries, index, _entries.Length - index - 1);
		}

		Array.Resize(ref _entries, _entries.Length - 1);
		return true;
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		if (TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
		{
			return Remove(item.Key);
		}

		return false;
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		if (key is null)
		{
			throw new ArgumentNullException(nameof(key));
		}

		int index = FindEntry(key);
		if (index >= 0)
		{
			value = _entries[index].Value;
			return true;
		}

		value = default;
		return false;
	}

	private int FindEntry(TKey key)
	{
		var comparer = Comparer;
		for (int i = 0; i < _entries.Length; i++)
		{
			if (comparer.Equals(_entries[i].Key, key))
			{
				return i;
			}
		}

		return -1;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}