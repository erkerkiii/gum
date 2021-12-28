using System.Collections.Generic;

namespace Gum.Pooling
{
    public class PoolCollection<T> where T : IPoolable
    {
        private readonly Dictionary<int, IPool<T>> _poolDictionary;

        private readonly PoolBuilder<T> _poolBuilder;

        public PoolCollection(PoolBuilder<T> poolBuilder)
        {
            _poolDictionary = new Dictionary<int, IPool<T>>();
            _poolBuilder = poolBuilder;
        }

        public void Put(int key, T value)
        {
            EnsureExistence(key);

            _poolDictionary[key].Put(value);
        }
        
        public T Get(int key, object[] args = null)
        {
            EnsureExistence(key);
            
            return _poolDictionary[key].Get(args);
        }

        private void EnsureExistence(int key)
        {
            if (_poolDictionary.ContainsKey(key))
            {
                return;
            }

            IPool<T> pool = _poolBuilder.Build();
            _poolDictionary.Add(key, pool);
        }
    }
}