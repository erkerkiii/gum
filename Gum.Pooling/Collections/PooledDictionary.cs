using System;
using System.Collections.Generic;

namespace Gum.Pooling.Collections
{
	public sealed class PooledDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable
	{
		public static PooledDictionary<TKey, TValue> Get()
		{
			return DictionaryPool<TKey, TValue>.Shared.Get();
		}

		public void Dispose()
		{
			Clear();
			DictionaryPool<TKey, TValue>.Shared.Put(this);
		}
	}
}