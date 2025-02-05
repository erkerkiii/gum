using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
				_entries.Add(new Entry(action, typeof(T), false));
			}
		}

		public void Subscribe<T>(Func<T, Task> func)
		{
			lock (_lock)
			{
				_entries.Add(new Entry(func, typeof(T), true));
			}
		}

		public void Unsubscribe<T>(Func<T, Task> func)
		{
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					Entry entry = _entries[index];

					if (entry.Delegate as Func<T, Task> == func)
					{
						_entries.Remove(entry);
					}
				}
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
					Entry entry = _entries[index];
					if (entry.TypeHashCode != hashCode)
					{
						continue;
					}

					if (!entry.IsAsync)
					{
						((Action<T>)entry.Delegate).Invoke(signal);
						continue;
					}

					((Func<T, Task>)entry.Delegate).Invoke(signal);
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
		
		public bool Exists<T>(Func<T, Task> func)
		{
			lock (_lock)
			{
				for (int index = 0; index < _entries.Count; index++)
				{
					if (_entries[index].Delegate.Equals(func))
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

			public readonly bool IsAsync;

			public Entry(Delegate @delegate, Type type, bool isAsync)
			{
				Delegate = @delegate;
				IsAsync = isAsync;
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