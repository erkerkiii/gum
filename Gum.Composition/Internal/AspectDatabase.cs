using System;
using System.Collections.Generic;
using System.Reflection;
using Gum.Composition.Generated;
using Gum.Core.Utility;

namespace Gum.Composition.Internal
{
	internal static class AspectDatabase
	{
		private static readonly Dictionary<int, AspectType> TypeToAspectTypeLookUp = new Dictionary<int, AspectType>();

		static AspectDatabase()
		{
			Type[] types = typeof(IAspect).GetDerivedTypesOfInterface();
			for (int index = 0; index < types.Length; index++)
			{
				Type type = types[index];

				FieldInfo fieldInfo = type.GetField("ASPECT_TYPE");
				AspectType aspectType = (AspectType)fieldInfo.GetValue(null);

				TypeToAspectTypeLookUp.Add(type.GetHashCode(), aspectType);
			}
		}

		public static AspectType GetAspectTypeOfType(Type type)
		{
			return TypeToAspectTypeLookUp[type.GetHashCode()];
		}
	}
}