using System;

namespace Gum.Composer
{
	public readonly struct AspectType : IEquatable<AspectType>
	{
		public readonly int Value;

		public AspectType(int value)
		{
			Value = value;
		}

		public bool Equals(AspectType other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			return obj is AspectType other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Value;
		}
		
		public static implicit operator int(AspectType aspectType) => aspectType.Value;
		
		public static implicit operator AspectType(int value) => new AspectType(value);
	}
}