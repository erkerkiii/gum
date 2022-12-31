using Gum.Composition;

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
			return Composition.Create(new IAspect[]
			{
				new BarAspect(MyInt)
			});
		}
	}
}