using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gum.Pooling
{
	public sealed class ArrayPool<T>
	{
		private static readonly Dictionary<int, ArrayPool<T>> ArrayPools = new Dictionary<int, ArrayPool<T>>();

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
			if (ArrayPools.ContainsKey(length))
			{
				return ArrayPools[length];
			}

			ArrayPool<T> arrayPool = new ArrayPool<T>(length);
			ArrayPools.Add(length, arrayPool);

			return arrayPool;
		}
	}
}