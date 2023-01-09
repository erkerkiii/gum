using System;

namespace Gum.Pooling
{
    public interface IPoolable
    {
        event Action OnReturnToPoolRequested;

        void Reset();
        void Erase();
    }
}