using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
    [TestFixture]
    public class StackPoolTests
    {
        private IPool<MockPoolable> _pool;

        private PoolBuilder<MockPoolable> _poolBuilder;
        
        [SetUp]
        public void Setup()
        {
            _poolBuilder = new PoolBuilder<MockPoolable>();
            _pool = _poolBuilder
                .SetPoolType(PoolType.Stack)
                .FromPoolableInstanceProvider(new MockInstanceProvider())
                .Build();

            _poolBuilder.Clear();
            Assert.IsTrue(_pool is StackPool<MockPoolable>);
        }

        [Test]
        public void StackPool_Creates_Instance()
        {
            MockPoolable mockPoolable = _pool.Get();
            
            Assert.IsNotNull(mockPoolable);
            Assert.IsTrue(mockPoolable.IsActive);
        }
        
        [Test]
        public void StackPool_Stores_And_Reuses_Instance()
        {
            MockPoolable mockPoolable = new MockPoolable();
            _pool.Put(mockPoolable);
            
            Assert.IsFalse(mockPoolable.IsActive);
            
            MockPoolable pooledObject = _pool.Get();
            
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.IsActive);
        }

        [Test]
        public void StackPool_Gets_Erased()
        {
            MockPoolable mockPoolable = new MockPoolable();
            _pool.Put(mockPoolable);
            
            Assert.IsFalse(mockPoolable.IsActive);

            _pool.Dispose();
            _pool = null;
            
            Assert.IsTrue(mockPoolable.IsErased);
        }

        [TearDown]
        public void CleanUp()
        {
            _pool?.Dispose();
            _pool = null;
            _poolBuilder.Clear();
            _poolBuilder = null;
        }
    }
}