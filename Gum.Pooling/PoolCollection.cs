using System.Collections.Generic;

namespace Gum.Pooling
{
    public sealed class PoolCollection<TKey, TValue> where TValue : IPoolable
    {
        private readonly Dictionary<TKey, IPool<TValue>> _poolDictionary;

        private readonly PoolBuilder<TValue> _poolBuilder;

        public PoolCollection(PoolBuilder<TValue> poolBuilder)
        {
            _poolDictionary = new Dictionary<TKey, IPool<TValue>>();
            _poolBuilder = poolBuilder;
        }

        public TValue Get(TKey key, object[] args = null)
        {
            EnsureExistence(key);
            
            return _poolDictionary[key].Get(args);
        }

        private void EnsureExistence(TKey key)
        {
            if (_poolDictionary.ContainsKey(key))
            {
                return;
            }

            IPool<TValue> pool = _poolBuilder.Build();
            _poolDictionary.Add(key, pool);
        }
    }
}