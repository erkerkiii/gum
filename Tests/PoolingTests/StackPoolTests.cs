using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
    [TestFixture]
    public class StackPoolTests
    {
        private IPool<MockPoolable> _pool;

        [SetUp]
        public void Setup()
        {
            PoolBuilder<MockPoolable> poolBuilder = new PoolBuilder<MockPoolable>();
            _pool = poolBuilder
                .SetPoolType(PoolType.Stack)
                .FromPoolableInstanceProvider(new MockInstanceProvider())
                .Build();

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
        public void StackPool_Object_Returns_To_Pool()
        {
            MockPoolable pooledObject = _pool.Get();
            
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.IsActive);
            
            pooledObject.ReturnToPool();
            
            Assert.IsFalse(pooledObject.IsActive);
        }

        [Test]
        public void StackPool_Gets_Erased()
        {
            _pool.Dispose();
            _pool = null;
            
            Assert.Pass();
        }

        [TearDown]
        public void CleanUp()
        {
            _pool?.Dispose();
            _pool = null;
        }
    }
}