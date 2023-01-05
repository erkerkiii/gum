using Gum.Composer;
using Gum.Composer.Generated;

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
	
	public readonly struct FooAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = (AspectType)1;
		
		public AspectType Type => ASPECT_TYPE;

		public readonly int MyInt;
		
		public FooAspect(int myInt)
		{
			MyInt = myInt;
		}
	}
}