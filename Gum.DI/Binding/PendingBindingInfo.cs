using System;
using Gum.DI.Container;

namespace Gum.DI.Binding
{
	internal struct PendingBindingInfo
	{
		public Type bindingType;
		public Type objectType;

		public BindingStrategy bindingStrategy;

		public DiContainer diContainer;

		public object instance;

		public static implicit operator BindingInfo(PendingBindingInfo pendingBindingInfo)
		{
			return new BindingInfo(pendingBindingInfo.bindingType, pendingBindingInfo.objectType,
				pendingBindingInfo.bindingStrategy, pendingBindingInfo.instance);
		}
	}
}