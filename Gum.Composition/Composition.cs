using System;
using System.Collections.Generic;
using Gum.Composition.Exception;
using Gum.Composition.Generated;
using Gum.Composition.Internal;
using Gum.Pooling.Collections;

namespace Gum.Composition
{
	public readonly struct Composition : IDisposable
	{
		private readonly PooledDictionary<AspectType, IAspect> _aspectLookUp;

		public IAspect this[AspectType aspectType] => _aspectLookUp[aspectType];

		public int AspectCount => IsValid
			? _aspectLookUp.Count
			: 0;

		public readonly bool IsValid;

		private Composition(IAspect[] aspects)
		{
			_aspectLookUp = PooledDictionary<AspectType, IAspect>.Get();

			for (int index = 0; index < aspects?.Length; index++)
			{
				_aspectLookUp.Add(aspects[index].Type, aspects[index]);
			}

			IsValid = true;
		}

		public static Composition Create(IAspect[] aspects = null)
		{
			return new Composition(aspects ?? Array.Empty<IAspect>());
		}

		public TAspect GetAspect<TAspect>() where TAspect : IAspect
		{
			SanityCheck();
			
			return (TAspect) _aspectLookUp[AspectDatabase.GetAspectTypeOfType(typeof(TAspect))];
		}

		public void AddAspect<TAspect>(TAspect aspect) where TAspect : IAspect
		{
			SanityCheck();
			
			if (_aspectLookUp.ContainsKey(aspect.Type))
			{
				return;
			}

			_aspectLookUp.Add(aspect.Type, aspect);
		}

		public void SetAspect<TAspect>(TAspect aspect) where TAspect : IAspect
		{
			SanityCheck();
			
			if (!_aspectLookUp.ContainsKey(aspect.Type))
			{
				AddAspect(aspect);
				return;
			}
			
			_aspectLookUp[aspect.Type] = aspect;
		}

		public bool RemoveAspect(AspectType aspectType)
		{
			if (!HasAspect(aspectType))
			{
				return false;
			}

			_aspectLookUp.Remove(aspectType);
			return true;
		}

		public bool HasAspect(AspectType aspectType)
		{
			SanityCheck();
			return _aspectLookUp.ContainsKey(aspectType);
		}
		
		private void SanityCheck()
		{
			if (!IsValid)
			{
				throw new InvalidCompositionException(
					$"Composition is not valid, please make sure to use the {nameof(Create)} method while instantiating the {nameof(Composition)} object.");
			}
		}

		public void Dispose()
		{
			_aspectLookUp.Dispose();
		}
	}
}