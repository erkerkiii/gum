using System;
using System.Collections.Generic;
using Gum.Composition.Generated;
using Gum.Pooling.Collections;

namespace Gum.Composition
{
	public readonly struct Composition : IDisposable
	{
		private readonly PooledDictionary<AspectType, IAspect> _aspectLookUp;

		public IAspect this[AspectType aspectType] => _aspectLookUp[aspectType];

		internal Composition(IAspect[] aspects)
		{
			_aspectLookUp = PooledDictionary<AspectType, IAspect>.Get();

			for (int index = 0; index < aspects.Length; index++)
			{
				_aspectLookUp.Add(aspects[index].Type, aspects[index]);
			}
		}

		public TAspect GetAspect<TAspect>() where TAspect : IAspect
		{
			foreach (KeyValuePair<AspectType, IAspect> keyValuePair in _aspectLookUp)
			{
				if (keyValuePair.Value is TAspect value)
				{
					return value;
				}
			}

			return default;
		}

		public void AddAspect<TAspect>(TAspect aspect) where TAspect : IAspect
		{
			if (_aspectLookUp.ContainsKey(aspect.Type))
			{
				return;
			}

			_aspectLookUp.Add(aspect.Type, aspect);
		}

		public bool HasAspect(AspectType aspectType)
		{
			return _aspectLookUp.ContainsKey(aspectType);
		}

		public void Dispose()
		{
			_aspectLookUp.Dispose();
		}
	}
}