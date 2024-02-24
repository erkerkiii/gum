﻿using System.Collections.Generic;
using Gum.Pooling.Collections;
using Gum.Pooling.Diagnostics;

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
#if DEBUG || UNITY_EDITOR
				PoolMonitor.UnreleasedPooledDictionaries.Remove(dictionary);
#endif
			}
		}

		public PooledDictionary<TKey, TValue> Get()
		{
			PooledDictionary<TKey, TValue> pooledDictionary = _pool.Count > 0
				? _pool.Pop()
				: new PooledDictionary<TKey, TValue>();

#if DEBUG || UNITY_EDITOR
			PoolMonitor.UnreleasedPooledDictionaries.Add(pooledDictionary, new System.Diagnostics.StackTrace().ToString());
#endif
			
			return pooledDictionary;
		}
	}
}