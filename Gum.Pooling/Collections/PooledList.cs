using System;
using System.Collections.Generic;

namespace Gum.Pooling.Collections
{
	public sealed class PooledList<T> : List<T>, IDisposable
	{
		public static PooledList<T> Get()
		{
			return ListPool<T>.Shared.Get();
		}

		public void Dispose()
		{
			Clear();
			ListPool<T>.Shared.Put(this);
		}
	}
}