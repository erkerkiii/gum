using System;

namespace Gum.DI.CompilerServices.Attribute
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method |
	                AttributeTargets.Property)]
	public class InjectAttribute : System.Attribute
	{
		public readonly string Id;

		public InjectAttribute()
		{
		}
		
		public InjectAttribute(string id)
		{
			Id = id;
		}
	}
}