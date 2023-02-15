using System;
using System.Collections.Generic;
using System.Linq;

namespace Gum.Signal.Core
{
	public sealed class SignalCenter
	{
		private readonly object _lock = new object();

		private readonly List<Entry> _entries = new List<Entry>();

		public void Subscribe<T>(Action<T> action)
		{
			lock (_lock)
			{
				_entries.Add(new Entry(action, typeof(T)));
			}
		}
		
		public void Unsubscribe<T>(Action<T> action)
		{
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					Entry entry = _entries[index];

					if (entry.Delegate as Action<T> == action)
					{
						_entries.Remove(entry);
					}
				}
			}
		}
		
		public void Fire<T>(T signal)
		{
			int hashCode = typeof(T).GetHashCode();
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					if (_entries[index].TypeHashCode == hashCode)
					{
						((Action<T>)_entries[index].Delegate).Invoke(signal);
					}
				}	
			}
		}
		
		public bool Exists<T>(Action<T> action)
		{
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					if (_entries[index].Delegate.Equals(action))
					{
						return true;
					}
				}	
			}

			return false;
		}
		
		private readonly struct Entry : IEquatable<Entry>
		{
			public readonly Delegate Delegate;

			public readonly int TypeHashCode;

			public Entry(Delegate @delegate, Type type)
			{
				Delegate = @delegate;
				TypeHashCode = type.GetHashCode();
			}
			
			public bool Equals(Entry other)
			{
				return Equals(Delegate, other.Delegate) && TypeHashCode == other.TypeHashCode;
			}

			public override bool Equals(object obj)
			{
				return obj is Entry other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((Delegate != null ? Delegate.GetHashCode() : 0) * 397) ^ TypeHashCode;
				}
			}
		}
	}
}