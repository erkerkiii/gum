using System;

namespace Gum.Pooling
{
    public interface IPoolable
    {
        event Action<IPoolable> OnReturnToPoolRequested;

        void Reset();
        void Erase();
    }
}