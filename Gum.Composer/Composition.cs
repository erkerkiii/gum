using System;
using Gum.Composer.Exception;
using Gum.Composer.Internal;
using Gum.Pooling;
using Gum.Pooling.Collections;

namespace Gum.Composer
{
	public readonly struct Composition : IDisposable
	{
		private readonly PooledDictionary<AspectType, IAspect> _aspectLookUp;

		public IAspect this[AspectType aspectType] => _aspectLookUp[aspectType];

		private readonly IAspect[] _aspects;

		public int AspectCount
		{
			get
			{
				SanityCheck();
				return _aspectLookUp.Count;
			}
		}

		public readonly bool IsValid;

		private Composition(IAspect[] aspects)
		{
			_aspects = aspects;
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

			return (TAspect)_aspectLookUp[AspectDatabase.GetAspectTypeOfType(typeof(TAspect))];
		}

		public bool TryGetAspect<TAspect>(out TAspect aspect) where TAspect : IAspect
		{
			SanityCheck();

			aspect = default;

			if (!HasAspect(AspectDatabase.GetAspectTypeOfType(typeof(TAspect))))
			{
				return false;
			}

			aspect = (TAspect)_aspectLookUp[AspectDatabase.GetAspectTypeOfType(typeof(TAspect))];
			return true;
		}

		public Composition GetAspectFluent<TAspect>(out TAspect aspect)
		{
			SanityCheck();
			aspect = (TAspect)_aspectLookUp[AspectDatabase.GetAspectTypeOfType(typeof(TAspect))];
			return this;
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
			if (_aspects.Length > 0)
			{
				ArrayPool<IAspect>.GetPool(_aspects.Length).Put(_aspects);
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		public ref struct Enumerator
		{
			public IAspect Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _source[_index - 1];
			}

			private int _index;

			private readonly int _count;

			private readonly IAspect[] _source;

			public Enumerator(Composition composition)
			{
				_source = composition._aspectLookUp.Values.ToArray();
				_index = 0;
				_count = _source.Length;
			}

			public bool MoveNext()
			{
				return _index++ < _count;
			}

			public void Reset()
			{
				_index = 0;
			}

			public void Dispose()
			{
				_index = 0;
			}
		}
	}
}