using System;

namespace Gum.Pooling
{
    public interface IPool<out T> : IDisposable where T : IPoolable
    {
        int Count { get; }
        
        T Get(object[] args = null);
    }
}