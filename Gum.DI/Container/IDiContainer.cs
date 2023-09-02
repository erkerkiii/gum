using System;
using Gum.DI.Binding;

namespace Gum.DI.Container
{
	public interface IDiContainer
	{
		T Resolve<T>();
		object Resolve(Type type);

		ObjectTypeBuilder<TBinding> Bind<TBinding>();
	}
}