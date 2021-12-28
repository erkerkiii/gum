using Gum.Pooling;

namespace Tests.PoolingTests
{
    public class MockInstanceProvider : IPoolableInstanceProvider<MockPoolable>
    {
        public MockPoolable Create(object[] args = null)
        {
            return new MockPoolable();
        }
    }
}