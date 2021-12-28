using System;

namespace Gum.Pooling
{
    public class PoolBuilder<T> where T : IPoolable
    {
        private CreationInfo _creationInfo;

        public PoolBuilder<T> FromMethod(Func<object[], T> func)
        {
            _creationInfo.providerType = Pool<T>.ProviderType.FromMethod;
            _creationInfo.instanceProviderCallback = func;

            return this;
        }

        public PoolBuilder<T> FromPoolableInstanceProvider(IPoolableInstanceProvider<T> instanceProvider)
        {
            _creationInfo.providerType = Pool<T>.ProviderType.FromPoolableInstanceProvider;
            _creationInfo.poolableInstanceProvider = instanceProvider;

            return this;
        }

        public PoolBuilder<T> WithInitialSize(int size)
        {
            _creationInfo.initialSize = size;

            return this;
        }

        public IPool<T> Build()
        {
            switch (_creationInfo.providerType)
            {
                case Pool<T>.ProviderType.FromPoolableInstanceProvider:
                    return new Pool<T>(_creationInfo.poolableInstanceProvider, _creationInfo.initialSize);
                case Pool<T>.ProviderType.FromMethod:
                    return new Pool<T>(_creationInfo.instanceProviderCallback, _creationInfo.initialSize);
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

            public Pool<T>.ProviderType providerType;
        }
    }
}