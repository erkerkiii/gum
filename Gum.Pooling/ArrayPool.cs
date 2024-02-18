using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gum.Pooling
{
	public sealed class ArrayPool<T>
	{
		private static readonly Dictionary<int, ArrayPool<T>> ArrayPools = new Dictionary<int, ArrayPool<T>>();

		private static int _highestCapactiyPool;

		private readonly Stack<T[]> _pool = new Stack<T[]>();

		private readonly int _length;

		private ArrayPool(int length)
		{
			_length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Put(T[] array)
		{
			for (int index = 0; index < array.Length; index++)
			{
				array[index] = default;
			}

			_pool.Push(array);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] Get()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}

			return new T[_length];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayPool<T> GetPool(int length)
		{
			if (_highestCapactiyPool >= length && ArrayPools.TryGetValue(_highestCapactiyPool, out ArrayPool<T> pool))
			{
				return pool;
			}

			ArrayPool<T> arrayPool = new ArrayPool<T>(length);
			ArrayPools.Add(length, arrayPool);

			if (length > _highestCapactiyPool)
			{
				_highestCapactiyPool = length;
			}

			return arrayPool;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PooledArray<T> Get(int length)
		{
			ArrayPool<T> arrayPool = GetPool(length);
			return new PooledArray<T>(arrayPool, arrayPool.Get(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear()
		{
			_highestCapactiyPool = 0;
			ArrayPools.Clear();
		}
	}
}