using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
    [TestFixture]
    public class PoolCollectionTests
    {
        private PoolCollection<MockPoolable> _poolCollection;

        [SetUp]
        public void SetUp()
        {
            PoolBuilder<MockPoolable> poolBuilder = new PoolBuilder<MockPoolable>();
            poolBuilder
                .FromPoolableInstanceProvider(new MockInstanceProvider())
                .SetPoolType(PoolType.Stack);

            _poolCollection = new PoolCollection<MockPoolable>(poolBuilder);
        }

        [Test]
        public void PoolCollection_Creates_Instance()
        {
            MockPoolable mockPoolable = _poolCollection.Get(0);
            
            Assert.NotNull(mockPoolable);
            Assert.IsTrue(mockPoolable.IsActive);
        }
        
        [Test]
        public void PoolCollection_Stores_And_Reuses_Instance()
        {
            MockPoolable mockPoolable = new MockPoolable();
            _poolCollection.Put(0, mockPoolable);
            
            Assert.IsFalse(mockPoolable.IsActive);

            MockPoolable pooledObject = _poolCollection.Get(0);
            
            Assert.NotNull(pooledObject);
            Assert.IsTrue(pooledObject.IsActive);
        }
        
        [TearDown]
        public void CleanUp()
        {
            _poolCollection = null;
        }
    }
}