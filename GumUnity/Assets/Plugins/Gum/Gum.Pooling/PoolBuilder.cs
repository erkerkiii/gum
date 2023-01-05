using System;
using Gum.Core.Assert;

namespace Gum.Pooling
{
    public sealed class PoolBuilder<T> where T : IPoolable
    {
        private CreationInfo _creationInfo;

        public PoolBuilder<T> FromMethod(Func<object[], T> func)
        {
            _creationInfo.providerType = ProviderType.FromMethod;
            _creationInfo.instanceProviderCallback = func;

            return this;
        }

        public PoolBuilder<T> FromPoolableInstanceProvider(IPoolableInstanceProvider<T> instanceProvider)
        {
            _creationInfo.providerType = ProviderType.FromPoolableInstanceProvider;
            _creationInfo.poolableInstanceProvider = instanceProvider;

            return this;
        }

        public PoolBuilder<T> WithInitialSize(int size)
        {
            _creationInfo.initialSize = size;

            return this;
        }
        
        public PoolBuilder<T> SetPoolType(PoolType poolType)
        {
            _creationInfo.poolType = poolType;
            
            return this;
        }

        public IPool<T> Build()
        {
            GumAssert.IsTrue(_creationInfo.providerType != ProviderType.Undefined, "No provider type defined!");

            switch (_creationInfo.poolType)
            {
                case PoolType.Stack:
                    return GetStackPool();
                case PoolType.WeakStack:
                    return GetWeakStackPool();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IPool<T> GetWeakStackPool()
        {
            switch (_creationInfo.providerType)
            {
                case ProviderType.FromPoolableInstanceProvider:
                    return new WeakStackPool<T>(_creationInfo.poolableInstanceProvider, _creationInfo.initialSize);
                case ProviderType.FromMethod:
                    return new WeakStackPool<T>(_creationInfo.instanceProviderCallback, _creationInfo.initialSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IPool<T> GetStackPool()
        {
            switch (_creationInfo.providerType)
            {
                case ProviderType.FromPoolableInstanceProvider:
                    return new StackPool<T>(_creationInfo.poolableInstanceProvider, _creationInfo.initialSize);
                case ProviderType.FromMethod:
                    return new StackPool<T>(_creationInfo.instanceProviderCallback, _creationInfo.initialSize);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Clear()
        {
            _creationInfo = default;
        }

        private struct CreationInfo
        {
            public int initialSize;

            public IPoolableInstanceProvider<T> poolableInstanceProvider;

            public Func<object[], T> instanceProviderCallback;

            public ProviderType providerType;

            public PoolType poolType;
        }
    }
}