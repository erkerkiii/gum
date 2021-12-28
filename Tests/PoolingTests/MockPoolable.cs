using Gum.Pooling;

namespace Tests.PoolingTests
{
    public class MockPoolable : IPoolable
    {
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

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Erase()
        {
            IsErased = true;
        }
    }
}