#if UNITY_2019_1_OR_NEWER
using Gum.Composer;
using Gum.Composer.Unity.Runtime;
using Gum.Pooling;

namespace Tests.CompositionTests.Unity
{
    public class FooMonoComposable : MonoComposable
    {
        private int _value = 12;

        protected override IAspect[] GetAspects()
        {
            IAspect[] aspects = ArrayPool<IAspect>.GetPool(1).Get();
            aspects[0] = new FooAspect(_value);
            return aspects;
        }
    }
}
#endif