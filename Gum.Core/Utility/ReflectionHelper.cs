using System;
using System.Linq;
using System.Threading;

namespace Gum.Core.Utility
{
	public static class ReflectionHelper
	{
		public static Type[] GetDerivedTypesOfInterface(this Type baseType)
		{
			if (!baseType.IsInterface)
			{
				return Array.Empty<Type>();
			}
            
			return Thread.GetDomain().GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(t => t != baseType && t.GetInterfaces().Contains(baseType))
				.ToArray();
		}
		
		public static Type[] GetAllAvailableTypes(this Type baseType)
		{
			return Thread.GetDomain().GetAssemblies()
				.SelectMany(a => a.GetTypes()).ToArray();
		}
	}
}