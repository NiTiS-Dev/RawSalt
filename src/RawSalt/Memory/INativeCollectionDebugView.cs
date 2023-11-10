using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawSalt.Memory;

internal sealed class INativeCollectionDebugView<T>
	where T : unmanaged
{
	private readonly NativeCollection<T> collection;

	public INativeCollectionDebugView(NativeCollection<T> collection)
	{
		ArgumentNullException.ThrowIfNull(collection, nameof(collection));
		this.collection = collection;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public T[] Items
	{
		get
		{
			T[] array = new T[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}
	}

}
