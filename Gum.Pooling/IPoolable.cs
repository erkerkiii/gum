using System;

namespace Gum.Pooling
{
    public interface IPoolable
    {
        event Action OnReturnToPoolRequested;
        event Action OnDestroyed;

        void Reset(object[] args = null);
        void Erase();
    }
}