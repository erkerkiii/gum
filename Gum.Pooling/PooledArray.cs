using System;
using System.Runtime.CompilerServices;
using Gum.Core.Assert;

namespace Gum.Pooling
{
	public readonly struct PooledArray<T> : IDisposable
	{
		private readonly ArrayPool<T> _arrayPool;

		private readonly T[] _source;

		public readonly int Length;

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= Length)
				{
					throw new GumException($"Index {index} is greater than the pooled array's length.");
				}
				
				return _source[index];
			}
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (index >= Length)
				{
					throw new GumException($"Index {index} is greater than the pooled array's length.");
				}
				
				_source[index] = value;
			}
		}
		
		public PooledArray(ArrayPool<T> arrayPool, T[] source, int length)
		{
			_arrayPool = arrayPool;
			_source = source;
			Length = length;
		}

		public T[] GetSource()
		{
			return _source;
		}

		public void Dispose()
		{
			_arrayPool.Put(_source);
		}
	}
}