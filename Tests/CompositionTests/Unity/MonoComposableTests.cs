#if UNITY_2019_1_OR_NEWER
using Gum.Composer;
using Gum.Composer.Exception;
using Gum.Composer.Unity.Runtime;
using NUnit.Framework;
using UnityEngine;

namespace Tests.CompositionTests.Unity
{
    public class MonoComposableTests
    {
        private MonoComposable _mockMonoComposable;

        private const int VALUE = 12;

        [SetUp]
        public void SetUp()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            FooMonoComposable fooMonoComposable = cube.AddComponent<FooMonoComposable>();
            _mockMonoComposable = fooMonoComposable;
        }

        [Test]
        public void Add_Aspect_To_MonoComposable()
        {
            Assert.IsFalse(_mockMonoComposable.Composition.HasAspect(BarAspect.ASPECT_TYPE));
            
            _mockMonoComposable.Composition.AddAspect(new BarAspect(VALUE));
            
            Assert.IsTrue(_mockMonoComposable.Composition.HasAspect(BarAspect.ASPECT_TYPE));
        }

        [Test]
        public void MonoComposable_Has_Aspect()
        {
            Assert.IsTrue(_mockMonoComposable.Composition.HasAspect(FooAspect.ASPECT_TYPE));
        }
        
        [Test]
        public void Get_Aspect_From_MonoComposable()
        {
            FooAspect fooAspect = _mockMonoComposable.Composition.GetAspect<FooAspect>();
            Assert.AreEqual(VALUE, fooAspect.MyInt);
        }

        [Test]
        public void Try_Get_Aspect_From_MonoComposable()
        {
            Assert.IsTrue(_mockMonoComposable.Composition.TryGetAspect(out FooAspect fooAspect));
            Assert.AreEqual(VALUE, fooAspect.MyInt);
            Assert.IsFalse(_mockMonoComposable.Composition.TryGetAspect(out BarAspect _));
        }

        [Test]
        public void Get_Aspect_Fluent()
        {
            _mockMonoComposable.Composition.GetAspectFluent(out FooAspect fooAspect);
            
            Assert.AreEqual(VALUE, fooAspect.MyInt);
        }

        [Test]
        public void Get_Aspect_With_Indexer()
        {
            _mockMonoComposable.Composition.AddAspect(new BarAspect(VALUE));
            BarAspect barAspect = (BarAspect)_mockMonoComposable.Composition[BarAspect.ASPECT_TYPE];
            
            Assert.IsNotNull(barAspect);
            Assert.AreEqual(VALUE, barAspect.MyInt);
        }

        [Test]
        public void Set_Aspect()
        {
            FooAspect fooAspect = _mockMonoComposable.Composition.GetAspect<FooAspect>();
            
            Assert.AreEqual(VALUE, fooAspect.MyInt);

            const int newValue = 20;
            _mockMonoComposable.Composition.SetAspect(new FooAspect(newValue));
            Assert.AreEqual(newValue, _mockMonoComposable.Composition.GetAspect<FooAspect>().MyInt);
            Assert.AreEqual(1, _mockMonoComposable.Composition.AspectCount);
        }
        
        [Test]
        public void Set_Non_Existing_Aspect()
        {
            Assert.IsFalse(_mockMonoComposable.Composition.HasAspect(BarAspect.ASPECT_TYPE));

            const int newValue = 20;
            _mockMonoComposable.Composition.SetAspect(new BarAspect(newValue));
            
            Assert.AreEqual(newValue, _mockMonoComposable.Composition.GetAspect<BarAspect>().MyInt);
            Assert.AreEqual(VALUE, _mockMonoComposable.Composition.GetAspect<FooAspect>().MyInt);
            
            Assert.AreEqual(2, _mockMonoComposable.Composition.AspectCount);
        }

        [Test]
        public void Remove_Aspect()
        {
            Assert.IsTrue(_mockMonoComposable.Composition.HasAspect(FooAspect.ASPECT_TYPE));

            _mockMonoComposable.Composition.RemoveAspect(FooAspect.ASPECT_TYPE);
            
            Assert.IsFalse(_mockMonoComposable.Composition.HasAspect(FooAspect.ASPECT_TYPE));
        }

        [Test]
        public void Enumerator()
        {
            _mockMonoComposable.Composition.AddAspect(new BarAspect(VALUE));

            foreach (IAspect aspect in _mockMonoComposable.Composition)
            {
                Assert.IsTrue(aspect is BarAspect || aspect is FooAspect);
            }
            
            Assert.Pass();
        }
        
        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_mockMonoComposable.gameObject);
        }
    }
}
#endif