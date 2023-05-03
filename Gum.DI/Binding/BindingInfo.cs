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

		public BindingInfo(Type bindingType, Type objectType, BindingStrategy bindingStrategy, object instance)
		{
			BindingType = bindingType;
			ObjectType = objectType;
			BindingStrategy = bindingStrategy;
			Instance = instance;
		}

		public bool Equals(BindingInfo other)
		{
			return Equals(BindingType, other.BindingType) && Equals(ObjectType, other.ObjectType) &&
			       BindingStrategy == other.BindingStrategy && Equals(Instance, other.Instance);
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
				return hashCode;
			}
		}
	}
}