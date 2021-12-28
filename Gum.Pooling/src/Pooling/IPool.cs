using System;

namespace Gum.Pooling
{
    public interface IPool<T> : IDisposable where T : IPoolable
    {
        T Get(object[] args = null);
        
        void Put(T poolable);
    }
}