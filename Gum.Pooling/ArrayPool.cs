using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if GUM_DEBUG
using Gum.Pooling.Diagnostics;
#endif

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
			
#if GUM_DEBUG
			PoolMonitor.UnreleasedPooledArrays.Remove(array);
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] Get()
		{
			T[] array = _pool.Count > 0
				? _pool.Pop()
				: new T[_length];
			
#if GUM_DEBUG
			PoolMonitor.UnreleasedPooledArrays.Add(array, new System.Diagnostics.StackTrace().ToString());
#endif
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArrayPool<T> GetPool(int length, bool isExactLengthRequired = true)
		{
			if (!isExactLengthRequired && _highestCapactiyPool >= length)
			{
				return ArrayPools[_highestCapactiyPool];
			}

			if (ArrayPools.TryGetValue(length, out ArrayPool<T> arrayPool))
			{
				return arrayPool;
			}

			arrayPool = new ArrayPool<T>(length);
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
			ArrayPool<T> arrayPool = GetPool(length, false);
			return new PooledArray<T>(arrayPool, arrayPool.Get(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Clear()
		{
			_highestCapactiyPool = 0;
			ArrayPools.Clear();
#if GUM_DEBUG
			PoolMonitor.UnreleasedPooledArrays.Clear();
#endif
		}
	}
}