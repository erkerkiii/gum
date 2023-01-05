using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
    [TestFixture]
    public class WeakStackPoolTests
    {
        private IPool<MockPoolable> _pool;

        private PoolBuilder<MockPoolable> _poolBuilder;
        
        [SetUp]
        public void Setup()
        {
            _poolBuilder = new PoolBuilder<MockPoolable>();
            _pool = _poolBuilder
                .SetPoolType(PoolType.WeakStack)
                .FromPoolableInstanceProvider(new MockInstanceProvider())
                .Build();

            _poolBuilder.Clear();
            Assert.IsTrue(_pool is WeakStackPool<MockPoolable>);
        }

        [Test]
        public void WeakStackPool_Creates_Instance()
        {
            MockPoolable mockPoolable = _pool.Get();
            
            Assert.IsNotNull(mockPoolable);
            Assert.IsTrue(mockPoolable.IsActive);
        }
        
        [Test]
        public void WeakStackPool_Object_Returns_To_Pool()
        {
            MockPoolable pooledObject = _pool.Get();
            
            Assert.IsNotNull(pooledObject);
            Assert.IsTrue(pooledObject.IsActive);
            
            pooledObject.ReturnToPool();
            
            Assert.IsFalse(pooledObject.IsActive);
        }

        [Test]
        public void WeakStackPool_Gets_Erased()
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
            _poolBuilder.Clear();
            _poolBuilder = null;
        }
    }
}