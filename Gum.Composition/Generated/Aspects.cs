namespace Gum.Composition.Generated
{
	public readonly struct TestAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.Test;

		public AspectType Type => ASPECT_TYPE;

		public readonly int intValue;

		public readonly string stringValue;

		public TestAspect(int arg0, string arg1)
		{		

			 intValue = arg0;
			 stringValue = arg1;
		}
	}
	public readonly struct Test2Aspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.Test2;

		public AspectType Type => ASPECT_TYPE;

		public readonly long MyLong;

		public readonly short MyShort;

		public readonly bool MyBool;

		public Test2Aspect(long arg0, short arg1, bool arg2)
		{		

			 MyLong = arg0;
			 MyShort = arg1;
			 MyBool = arg2;
		}
	}
}