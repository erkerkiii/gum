using System;
using System.Collections.Generic;
using System.Reflection;
using Gum.DI.CompilerServices.Attribute;

namespace Gum.DI.Internal
{
	internal class TypeCache
	{
		private static readonly Dictionary<Type, TypeCache> TypeCaches = new Dictionary<Type, TypeCache>(64);

		public readonly Type Type;

		public readonly ConstructorInfo DefaultConstructorInfo;

		public readonly FieldInfo[] FieldInfosToInject;

		public readonly PropertyInfo[] PropertyInfosToInject;

		public readonly MethodInfo[] MethodInfosToInject;

		private TypeCache(Type type)
		{
			Type = type;
			DefaultConstructorInfo = GetDefaultConstructorInfo(type);
			FieldInfosToInject = GetInjectableFields(type);
			PropertyInfosToInject = GetInjectableProperties(type);
			MethodInfosToInject = GetInjectableMethods(type);
		}

		private static ConstructorInfo GetDefaultConstructorInfo(Type type)
		{
			ConstructorInfo[] constructorInfos = type.GetConstructors();
			return constructorInfos.Length == 0
				? type.GetConstructor(Type.EmptyTypes)
				: constructorInfos[0];
		}

		public static TypeCache Get<T>()
		{
			return Get(typeof(T));
		}

		public static TypeCache Get(Type type)
		{
			if (TypeCaches.TryGetValue(type, out TypeCache typeCache))
			{
				return typeCache;
			}

			typeCache = Add(type);
			return typeCache;
		}

		private static TypeCache Add(Type type)
		{
			TypeCache typeCache = new TypeCache(type);
			TypeCaches.Add(type, typeCache);
			return typeCache;
		}

		private static MethodInfo[] GetInjectableMethods(Type type)
		{
			MethodInfo[] methodInfos =
				type.GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
			int count = 0;
			for (int index = 0; index < methodInfos.Length; index++)
			{
				if (methodInfos[index].GetCustomAttribute<InjectAttribute>() != null)
				{
					methodInfos[count++] = methodInfos[index];
				}
			}

			Array.Resize(ref methodInfos, count);

			return methodInfos;
		}

		private static PropertyInfo[] GetInjectableProperties(Type type)
		{
			PropertyInfo[] propertyInfos =
				type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			int count = 0;
			for (int index = 0; index < propertyInfos.Length; index++)
			{
				if (propertyInfos[index].GetCustomAttribute<InjectAttribute>() != null)
				{
					propertyInfos[count++] = propertyInfos[index];
				}
			}

			Array.Resize(ref propertyInfos, count);

			return propertyInfos;
		}

		private static FieldInfo[] GetInjectableFields(Type type)
		{
			FieldInfo[] fieldInfos =
				type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			int count = 0;
			for (int index = 0; index < fieldInfos.Length; index++)
			{
				if (fieldInfos[index].GetCustomAttribute<InjectAttribute>() != null)
				{
					fieldInfos[count++] = fieldInfos[index];
				}
			}

			Array.Resize(ref fieldInfos, count);

			return fieldInfos;
		}
	}
}