using System;

namespace Gum.DI.Container
{
	public interface IInstantiator
	{
		T Instantiate<T>();
		object Instantiate(Type type);
	}
}