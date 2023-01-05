using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
    [TestFixture]
    public class PoolCollectionTests
    {
        private PoolCollection<int, MockPoolable> _poolCollection;

        [SetUp]
        public void SetUp()
        {
            PoolBuilder<MockPoolable> poolBuilder = new PoolBuilder<MockPoolable>();
            poolBuilder
                .FromPoolableInstanceProvider(new MockInstanceProvider())
                .SetPoolType(PoolType.Stack);

            _poolCollection = new PoolCollection<int, MockPoolable>(poolBuilder);
        }

        [Test]
        public void PoolCollection_Creates_Instance()
        {
            MockPoolable mockPoolable = _poolCollection.Get(0);
            
            Assert.NotNull(mockPoolable);
            Assert.IsTrue(mockPoolable.IsActive);
        }

        [TearDown]
        public void CleanUp()
        {
            _poolCollection = null;
        }
    }
}