using System.Collections.Generic;
using Gum.Pooling.Collections;

namespace Gum.Pooling
{
	internal sealed class DictionaryPool<TKey, TValue>
	{
		private readonly Stack<PooledDictionary<TKey, TValue>> _pool = new Stack<PooledDictionary<TKey, TValue>>();
		
		public static readonly DictionaryPool<TKey, TValue> Shared = new DictionaryPool<TKey, TValue>();

		public void Put(PooledDictionary<TKey, TValue> dictionary)
		{
			if (!_pool.Contains(dictionary))
			{
				_pool.Push(dictionary);
			}
		}

		public PooledDictionary<TKey, TValue> Get()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}

			return new PooledDictionary<TKey, TValue>();
		}
	}
}