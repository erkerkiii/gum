#if UNITY_2019_1_OR_NEWER
using UnityEngine;

namespace Gum.Composer.Unity.Runtime
{
    public abstract class MonoComposable : MonoBehaviour, IComposable
    {
        public Composition Composition { get; protected set; }
        
        protected virtual void Awake()
        {
            Composition = Composition.Create(GetAspects());
        }

        public Composition GetComposition()
        {
            return Composition;
        }

        protected virtual void OnDestroy()
        {
            Composition.Dispose();
        }

        protected abstract IAspect[] GetAspects();
    }
}
#endif