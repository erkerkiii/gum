using System.Collections.Generic;
using Gum.Pooling.Collections;

#if GUM_DEBUG
using Gum.Pooling.Diagnostics;
#endif

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
#if GUM_DEBUG
				PoolMonitor.UnreleasedPooledLists.Remove(list);
#endif
			}
		}

		public PooledList<T> Get()
		{
			PooledList<T> pooledList = _pool.Count > 0
				? _pool.Pop()
				: new PooledList<T>();

#if GUM_DEBUG
			PoolMonitor.UnreleasedPooledLists.Add(pooledList, new System.Diagnostics.StackTrace().ToString());
#endif
			
			return pooledList;
		}
	}
}