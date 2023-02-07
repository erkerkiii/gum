#if UNITY_2019_1_OR_NEWER
using System.Collections;
using Gum.Composer.Unity.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.CompositionTests.Unity
{
    public class MonoComposableTests
    {
        private MonoComposable _mockMonoComposable;

        private const int Value = 12;

        [OneTimeSetUp]
        public void SetUp()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            FooMonoComposable fooMonoComposable = cube.AddComponent<FooMonoComposable>();
            _mockMonoComposable = fooMonoComposable;
        }

        [UnityTest]
        public IEnumerator Get_Aspect_From_MonoComposable()
        {
            FooAspect fooAspect = _mockMonoComposable.Composition.GetAspect<FooAspect>();
            Assert.AreEqual(Value, fooAspect.MyInt);
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator Try_Get_Aspect_From_MonoComposable()
        {
            Assert.IsTrue(_mockMonoComposable.Composition.TryGetAspect(out FooAspect fooAspect));
            Assert.AreEqual(Value, fooAspect.MyInt);
            Assert.IsFalse(_mockMonoComposable.Composition.TryGetAspect(out BarAspect _));
            
            yield return null;
        }
    }
}
#endif