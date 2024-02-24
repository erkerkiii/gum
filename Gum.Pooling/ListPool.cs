using System.Collections.Generic;
using Gum.Pooling.Collections;
using Gum.Pooling.Diagnostics;

namespace Gum.Pooling
{
	internal sealed class ListPool<T>
	{
		private readonly Stack<PooledList<T>> _pool = new Stack<PooledList<T>>();

		public static readonly ListPool<T> Shared = new ListPool<T>();

		public void Put(PooledList<T> list)
		{
			if (!_pool.Contains(list))
			{
				_pool.Push(list);
#if DEBUG || UNITY_EDITOR
				PoolMonitor.UnreleasedPooledLists.Remove(list);
#endif
			}
		}

		public PooledList<T> Get()
		{
			PooledList<T> pooledList = _pool.Count > 0
				? _pool.Pop()
				: new PooledList<T>();

#if DEBUG || UNITY_EDITOR
			PoolMonitor.UnreleasedPooledLists.Add(pooledList, new System.Diagnostics.StackTrace().ToString());
#endif
			
			return pooledList;
		}
	}
}