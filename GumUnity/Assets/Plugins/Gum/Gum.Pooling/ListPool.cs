using System.Collections.Generic;
using Gum.Pooling.Collections;

namespace Gum.Pooling
{
	internal class ListPool<T>
	{
		private readonly Stack<PooledList<T>> _pool = new Stack<PooledList<T>>();

		public static readonly ListPool<T> Shared = new ListPool<T>();

		public void Put(PooledList<T> list)
		{
			if (!_pool.Contains(list))
			{
				_pool.Push(list);
			}
		}

		public PooledList<T> Get()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}

			return new PooledList<T>();
		}
	}
}