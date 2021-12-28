using System;
using System.Collections.Generic;

namespace Gum.Pooling
{
    public sealed class Pool<T> : IPool<T> where T : IPoolable
    {
        private readonly Stack<IPoolable> _objectPool;

        private readonly IPoolableInstanceProvider<T> _poolableInstanceProvider;

        private readonly Func<object[], T> _instanceProviderCallback;

        private readonly ProviderType _providerType;

        internal Pool(IPoolableInstanceProvider<T> poolableInstanceProvider, int initialSize = 0)
        {
            _poolableInstanceProvider = poolableInstanceProvider;
            _objectPool = new Stack<IPoolable>();
            _providerType = ProviderType.FromPoolableInstanceProvider;

            CreatePool(initialSize);
        }
        
        internal Pool(Func<object[], T> instanceProviderCallback, int initialSize = 0)
        {
            _instanceProviderCallback = instanceProviderCallback;
            _objectPool = new Stack<IPoolable>();
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

        public void Put(T poolable)
        {
            poolable.Deactivate();
            _objectPool.Push(poolable);
        }

        public T Get(object[] args = null)
        {
            IPoolable poolable = _objectPool.Count > 0
                ? _objectPool.Pop()
                : Create(args);
            
            poolable.Reset();
            return (T)poolable;
        }

        private T Create(object[] args = null)
        {
            switch (_providerType)
            {
                case ProviderType.FromPoolableInstanceProvider:
                    return _poolableInstanceProvider.Create(args);
                case ProviderType.FromMethod:
                    return _instanceProviderCallback.Invoke(args);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                _objectPool.Pop()?.Erase();
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
        
        ~Pool()
        {
            ReleaseUnmanagedResources();
        }
        
        internal enum ProviderType
        {
            Undefined,
            FromPoolableInstanceProvider,
            FromMethod
        }
    }
}