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

		public readonly bool IsValid;

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				SanityCheck();
				if (index >= Length)
				{
					throw new GumException($"Index {index} is greater than the pooled array's length.");
				}

				return _source[index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				SanityCheck();
				if (index >= Length)
				{
					throw new GumException($"Index {index} is greater than the pooled array's length.");
				}

				_source[index] = value;
			}
		}

		internal PooledArray(ArrayPool<T> arrayPool, T[] source, int length)
		{
			_arrayPool = arrayPool;
			_source = source;
			Length = length;
			IsValid = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] GetSource()
		{
			SanityCheck();
			return _source;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T[] ToArray()
		{
			SanityCheck();

			T[] array = new T[Length];
			Array.Copy(_source, array, Length);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SanityCheck()
		{
			if (!IsValid)
			{
				throw new GumException("PooledArray is not valid! Use ArrayPool<>.Get() to use the array pool.");
			}
		}

		public void Dispose()
		{
			SanityCheck();
			_arrayPool.Put(_source);
		}
	}
}