namespace Gum.Composer.Generated
{
	public readonly struct ScoreAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.Score;

		public AspectType Type => ASPECT_TYPE;

		public readonly int Score;

		public ScoreAspect(int arg0)
		{		

			 Score = arg0;
		}
	}
	public readonly struct IdAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.Id;

		public AspectType Type => ASPECT_TYPE;

		public readonly int Id;

		public IdAspect(int arg0)
		{		

			 Id = arg0;
		}
	}
	public readonly struct NameAspect : IAspect
	{
		public const AspectType ASPECT_TYPE = AspectType.Name;

		public AspectType Type => ASPECT_TYPE;

		public readonly string Name;

		public NameAspect(string arg0)
		{		

			 Name = arg0;
		}
	}
}