using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Gum.DI.Binding;
using Gum.DI.Exception;
using Gum.DI.Internal;
using Gum.Pooling.Collections;

namespace Gum.DI.Container
{
	public sealed class DiContainer : IDiContainer, IInstantiator
	{
		private readonly DiContainer _parentContainer;

		private readonly Dictionary<Type, BindingInfo> _bindings = new Dictionary<Type, BindingInfo>(32);
		
		private readonly Hashtable _singleInstances = new Hashtable(32);

		public DiContainer()
		{
		}

		public DiContainer(DiContainer parentContainer)
		{
			_parentContainer = parentContainer;
		}

		public DiContainer CreateSubContainer()
		{
			return new DiContainer(this);
		}
		
		public DiContainer GetParentContainer()
		{
			return _parentContainer;
		}
		
		public ObjectTypeBuilder<TBinding> Bind<TBinding>()
		{
			return new ObjectTypeBuilder<TBinding>(new PendingBindingInfo()
				{ bindingType = typeof(TBinding), diContainer = this });
		}

		internal void CompleteBinding(BindingInfo bindingInfo)
		{
			if (_bindings.ContainsKey(bindingInfo.BindingType))
			{
				throw new BindingAlreadyExistsException($"Binding type already exists: {bindingInfo.BindingType.Name}");
			}
			
			_bindings.Add(bindingInfo.BindingType, bindingInfo);

			if (bindingInfo.BindingStrategy == BindingStrategy.Single && bindingInfo.Instance != null)
			{
				AddSingleInstance(bindingInfo.BindingType, bindingInfo.Instance);
			}
		}
		
		private void AddSingleInstance(Type type, object instance)
		{
			_singleInstances.Add(type, instance);
		}

		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		public bool HasBindingFor(Type type)
		{
			return _bindings.ContainsKey(type);
		}

		private object Resolve(Type type)
		{
			if (!HasBindingFor(type))
			{
				if (_parentContainer == null)
				{
					throw new BindingNotFoundException($"No binding found for: {type.Name}");
				}

				return _parentContainer.Resolve(type);
			}

			BindingInfo bindingInfo = _bindings[type];
			if (bindingInfo.BindingStrategy == BindingStrategy.Single && _singleInstances.ContainsKey(type))
			{
				return _singleInstances[type];
			}

			object instance = Instantiate(bindingInfo.ObjectType);

			if (bindingInfo.BindingStrategy == BindingStrategy.Single)
			{
				AddSingleInstance(type, instance);
			}

			return instance;
		}

		private void InjectInto(object instance)
		{
			Type type = instance.GetType();
			TypeCache typeCache = TypeCache.Get(type);

			FieldInfo[] fieldInfosToInject = typeCache.FieldInfosToInject;
			MethodInfo[] methodInfosToInject = typeCache.MethodInfosToInject;
			PropertyInfo[] propertyInfosToInject = typeCache.PropertyInfosToInject;
			
			for (int index = 0; index < fieldInfosToInject.Length; index++)
			{
				FieldInfo fieldInfo = fieldInfosToInject[index];
				fieldInfo.SetValue(instance, Resolve(fieldInfo.FieldType));
			}

			for (int index = 0; index < methodInfosToInject.Length; index++)
			{
				MethodInfo methodInfo = methodInfosToInject[index];
				object[] dependencies = GetDependenciesOf(methodInfo);
				methodInfo.Invoke(instance, dependencies);
			}

			for (int index = 0; index < propertyInfosToInject.Length; index++)
			{
				PropertyInfo propertyInfo = propertyInfosToInject[index];
				propertyInfo.SetValue(instance, Resolve(propertyInfo.PropertyType));
			}
		}

		private object[] GetDependenciesOf(Type type)
		{
			TypeCache typeCache = TypeCache.Get(type);
			ParameterInfo[] parameterInfos = typeCache.DefaultConstructorInfo.GetParameters();
			using (PooledList<object> dependencies = PooledList<object>.Get())
			{
				for (int index = 0; index < parameterInfos.Length; index++)
				{
					dependencies.Add(Resolve(parameterInfos[index].ParameterType));
				}

				return dependencies.ToArray();
			}
		}
		
		private object[] GetDependenciesOf(MethodInfo methodInfo)
		{
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();
			using (PooledList<object> dependencies = PooledList<object>.Get())
			{
				for (int index = 0; index < parameterInfos.Length; index++)
				{
					dependencies.Add(Resolve(parameterInfos[index].ParameterType));
				}

				return dependencies.ToArray();
			}
		}
		
		public T Instantiate<T>()
		{
			return (T)Instantiate(typeof(T));
		}

		public object Instantiate(Type type)
		{
			object[] dependencies = GetDependenciesOf(type);
			object instance = Activator.CreateInstance(type, args: dependencies);
			InjectInto(instance);
			return instance;
		}
	}
}