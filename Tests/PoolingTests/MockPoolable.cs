using System;
using Gum.Pooling;

namespace Tests.PoolingTests
{
    public class MockPoolable : IPoolable
    {
        public event Action OnReturnToPoolRequested;
	public event Action OnDestroyed;

        public bool IsActive { get; private set; }
        public bool IsErased { get; private set; }

        public MockPoolable()
        {
            IsActive = true;
        }
        
        public void Reset()
        {
            IsActive = true;
        }

        public void ReturnToPool()
        {
            IsActive = false;

            OnReturnToPoolRequested?.Invoke();
        }

        public void Erase()
        {
            IsErased = true;
        }
    }
}