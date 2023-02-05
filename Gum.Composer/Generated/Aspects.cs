using UnityEngine;

namespace Gum.Composer.Generated
{
	public readonly struct HeroAspectAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.HeroAspect;

		public AspectType Type => ASPECT_TYPE;

		public readonly int Value1;

		public readonly int Value2;

		public HeroAspectAspect(int arg0, int arg1)
		{		

			 Value1 = arg0;
			 Value2 = arg1;
		}
	}
	public readonly struct MyNewAspectAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.MyNewAspect;

		public AspectType Type => ASPECT_TYPE;

		public readonly int hobaa;

		public readonly int Value2;

		public MyNewAspectAspect(int arg0, int arg1)
		{		

			 hobaa = arg0;
			 Value2 = arg1;
		}
	}
	public readonly struct nnAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.nn;

		public AspectType Type => ASPECT_TYPE;

		public readonly int bb;

		public readonly long dd;

		public nnAspect(int arg0, long arg1)
		{		

			 bb = arg0;
			 dd = arg1;
		}
	}
	public readonly struct ggAspectAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.ggAspect;

		public AspectType Type => ASPECT_TYPE;

		public readonly int Valyu;

		public readonly long Valyu2;

		public readonly double DubleYollar;

		public readonly Vector3 paparazi;

		public ggAspectAspect(int arg0, long arg1, double arg2, Vector3 arg3)
		{		

			 Valyu = arg0;
			 Valyu2 = arg1;
			 DubleYollar = arg2;
			 paparazi = arg3;
		}
	}
}