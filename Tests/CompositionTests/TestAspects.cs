using Gum.Composer;

namespace Tests.CompositionTests
{
	public readonly struct BarAspect : IAspect
	{
		public static readonly Gum.Composer.AspectType ASPECT_TYPE = 0;
		
		public Gum.Composer.AspectType Type => ASPECT_TYPE;

		public readonly int MyInt;
		
		public BarAspect(int myInt)
		{
			MyInt = myInt;
		}
	}
	
	public readonly struct FooAspect : IAspect
	{
		public static readonly Gum.Composer.AspectType ASPECT_TYPE = 1;
		
		public Gum.Composer.AspectType Type => ASPECT_TYPE;

		public readonly int MyInt;
		
		public FooAspect(int myInt)
		{
			MyInt = myInt;
		}
	}
}