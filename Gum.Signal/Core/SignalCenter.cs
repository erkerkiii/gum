using System;
using System.Collections.Generic;
using System.Linq;

namespace Gum.Signal.Core
{
	public class SignalCenter
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
				Entry[] entriesToRemove = _entries.Where(e => (Action<T>)e.Delegate == action).ToArray();
				for (int index = 0; index < entriesToRemove.Length; index++)
				{
					_entries.Remove(entriesToRemove[index]);
				}
			}
		}
		
		public void Fire<T>(T signal)
		{
			Type type = typeof(T);
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					if (_entries[index].Type == type)
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

			public readonly Type Type;

			public Entry(Delegate @delegate, Type type)
			{
				Delegate = @delegate;
				Type = type;
			}

			public bool Equals(Entry other)
			{
				return Equals(Delegate, other.Delegate) && Type == other.Type;
			}

			public override bool Equals(object obj)
			{
				return obj is Entry other && Equals(other);
			}

			public override int GetHashCode()
			{
				return HashCode.Combine(Delegate, Type);
			}
		}
	}
}