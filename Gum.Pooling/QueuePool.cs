﻿using System;
using System.Collections.Generic;

namespace Gum.Pooling
{
	public sealed class QueuePool<T> : IPool<T> where T : IPoolable
	{
		private readonly Queue<IPoolable> _objectPool;

		private readonly IPoolableInstanceProvider<T> _poolableInstanceProvider;

		private readonly Func<object[], T> _instanceProviderCallback;

		private readonly ProviderType _providerType;

		public int Count => _objectPool.Count;

		internal QueuePool(IPoolableInstanceProvider<T> poolableInstanceProvider, int initialSize = 0)
		{
			_poolableInstanceProvider = poolableInstanceProvider;
			_objectPool = new Queue<IPoolable>();
			_providerType = ProviderType.FromPoolableInstanceProvider;

			CreatePool(initialSize);
		}

		internal QueuePool(Func<object[], T> instanceProviderCallback, int initialSize = 0)
		{
			_instanceProviderCallback = instanceProviderCallback;
			_objectPool = new Queue<IPoolable>();
			_providerType = ProviderType.FromMethod;

			CreatePool(initialSize);
		}

		private void CreatePool(int size)
		{
			for (int index = 0; index < size; index++)
			{
				T instance = Create();
				Put(instance);
			}
		}

		private void Put(IPoolable poolable)
		{
			_objectPool.Enqueue(poolable);
		}

		public T Get(object[] args = null)
		{
			IPoolable poolable = _objectPool.Count > 0
				? _objectPool.Dequeue()
				: Create(args);

			poolable.Reset();
			return (T)poolable;
		}

		private T Create(object[] args = null)
		{
			T instance;
			switch (_providerType)
			{
				case ProviderType.FromPoolableInstanceProvider:
					instance = _poolableInstanceProvider.Create(args);
					break;
				case ProviderType.FromMethod:
					instance = _instanceProviderCallback.Invoke(args);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			instance.OnReturnToPoolRequested += () => Put(instance);

			return instance;
		}

		private void DestroyPool()
		{
			if (_objectPool == null)
			{
				return;
			}

			int poolSize = _objectPool.Count;
			for (int count = 0; count < poolSize; count++)
			{
				_objectPool.Dequeue()?.Erase();
			}
		}

		private void ReleaseUnmanagedResources()
		{
			DestroyPool();
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~QueuePool()
		{
			ReleaseUnmanagedResources();
		}
	}
}