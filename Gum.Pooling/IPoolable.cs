using System;

namespace Gum.Pooling
{
    public interface IPoolable
    {
        event Action OnReturnToPoolRequested;
        event Action OnDestroyed;

        void Reset();
        void Erase();
    }
}