using Gum.Composer;
using Gum.Pooling;

namespace Tests.CompositionTests
{
	public struct MockComposable : IComposable
	{
		public int MyInt;

		public MockComposable(int myInt)
		{
			MyInt = myInt;
		}

		public Composition GetComposition()
		{
			IAspect[] aspects = ArrayPool<IAspect>.GetPool(2).Get();
			aspects[0] = new BarAspect(MyInt);
			aspects[1] = new TagAspect();
			return Composition.Create(aspects);
		}
	}
}