using System;
using Gum.DI.Container;

namespace Gum.DI.Binding
{
	internal readonly struct BindingInfo : IEquatable<BindingInfo>
	{
		public readonly Type BindingType;
		public readonly Type ObjectType;

		public readonly BindingStrategy BindingStrategy;

		public readonly object Instance;

		public readonly bool IsLazy;

		public BindingInfo(Type bindingType, Type objectType, BindingStrategy bindingStrategy, object instance,
			bool isLazy)
		{
			BindingType = bindingType;
			ObjectType = objectType;
			BindingStrategy = bindingStrategy;
			Instance = instance;
			IsLazy = isLazy;
		}
		
		public bool Equals(BindingInfo other)
		{
			return Equals(BindingType, other.BindingType) && Equals(ObjectType, other.ObjectType) &&
			       BindingStrategy == other.BindingStrategy && Equals(Instance, other.Instance) &&
			       IsLazy == other.IsLazy;
		}

		public override bool Equals(object obj)
		{
			return obj is BindingInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (BindingType != null ? BindingType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ObjectType != null ? ObjectType.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int)BindingStrategy;
				hashCode = (hashCode * 397) ^ (Instance != null ? Instance.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsLazy.GetHashCode();
				return hashCode;
			}
		}
	}
}