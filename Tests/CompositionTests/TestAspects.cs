using Gum.Composition;
using Gum.Composition.Generated;

namespace Tests.CompositionTests
{
	public readonly struct BarAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = 0;
		
		public AspectType Type => ASPECT_TYPE;

		public readonly int MyInt;
		
		public BarAspect(int myInt)
		{
			MyInt = myInt;
		}
	}
}