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
	public class DiContainer : IDiContainer
	{
		private readonly DiContainer[] _parentContainers;

		private readonly Dictionary<Type, BindingInfo> _bindings = new Dictionary<Type, BindingInfo>(32);
		
		private readonly Hashtable _singleInstances = new Hashtable(32);

		public DiContainer()
		{
		}

		public DiContainer(DiContainer[] parentContainers)
		{
			_parentContainers = parentContainers;
		}

		internal DiContainer CreateSubContainer()
		{
			int parentContainerCount = (_parentContainers?.Length ?? 0) + 1;
			
			DiContainer[] parentContainers = new DiContainer[parentContainerCount];
			if (_parentContainers != null)
			{
				Array.Copy(_parentContainers, parentContainers, parentContainerCount - 1);
			}
			parentContainers[parentContainerCount - 1] = this;
			
			return new DiContainer(parentContainers);
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

		private object Resolve(Type type)
		{
			if (!_bindings.ContainsKey(type))
			{
				if (_parentContainers != null)
				{
					for (var index = 0; index < _parentContainers.Length; index++)
					{
						return _parentContainers[index].Resolve(type);
					}
				}
				
				throw new BindingNotFoundException($"No binding found for: {type.Name}");
			}

			BindingInfo bindingInfo = _bindings[type];
			if (bindingInfo.BindingStrategy == BindingStrategy.Single && _singleInstances.ContainsKey(type))
			{
				return _singleInstances[type];
			}

			object[] dependencies = GetDependenciesOf(bindingInfo.ObjectType);
			object instance = Activator.CreateInstance(bindingInfo.ObjectType, args: dependencies);
			InjectInto(instance);

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
	}
}