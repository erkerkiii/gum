﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gum.Signal.Core
{
	public sealed class SignalCenter
	{
		private readonly object _lock = new object();

		private readonly Dictionary<Type, Subscription[]> _subscriptions = new Dictionary<Type, Subscription[]>();

		public void Subscribe<T>(Action<T> action)
		{
			Add(typeof(T), new Subscription(action, false));
		}

		public void Subscribe<T>(Func<T, Task> func)
		{
			Add(typeof(T), new Subscription(func, true));
		}

		public void Unsubscribe<T>(Func<T, Task> func)
		{
			Remove(typeof(T), func);
		}

		public void Unsubscribe<T>(Action<T> action)
		{
			Remove(typeof(T), action);
		}

		public bool TryUnsubscribe<T>(Action<T> action)
		{
			return Remove(typeof(T), action);
		}

		public void Fire<T>(T signal)
		{
			Subscription[] snapshot;
			lock (_lock)
			{
				if (!_subscriptions.TryGetValue(typeof(T), out snapshot))
				{
					return;
				}
			}

			for (int index = 0; index < snapshot.Length; index++)
			{
				Subscription subscription = snapshot[index];

				if (subscription.IsRemoved)
				{
					continue;
				}

				if (!subscription.IsAsync)
				{
					((Action<T>)subscription.Delegate).Invoke(signal);
					continue;
				}

				((Func<T, Task>)subscription.Delegate).Invoke(signal);
			}
		}

		public async Task FireAsync<T>(T signal)
		{
			await Task.Factory.StartNew(() => Fire(signal));
		}

		public bool Exists<T>(Action<T> action)
		{
			return Contains(typeof(T), action);
		}

		public bool Exists<T>(Func<T, Task> func)
		{
			return Contains(typeof(T), func);
		}

		private void Add(Type type, Subscription subscription)
		{
			lock (_lock)
			{
				Subscription[] current;
				int count = _subscriptions.TryGetValue(type, out current) ? current.Length : 0;

				Subscription[] next = new Subscription[count + 1];
				if (count > 0)
				{
					Array.Copy(current, next, count);
				}

				next[count] = subscription;
				_subscriptions[type] = next;
			}
		}

		private bool Remove(Type type, Delegate @delegate)
		{
			lock (_lock)
			{
				Subscription[] current;
				if (!_subscriptions.TryGetValue(type, out current))
				{
					return false;
				}

				int remaining = 0;
				for (int index = 0; index < current.Length; index++)
				{
					if (Equals(current[index].Delegate, @delegate))
					{
						current[index].IsRemoved = true;
						continue;
					}

					remaining++;
				}

				if (remaining == current.Length)
				{
					return false;
				}

				if (remaining == 0)
				{
					_subscriptions.Remove(type);
					return true;
				}

				Subscription[] next = new Subscription[remaining];
				int nextIndex = 0;
				for (int index = 0; index < current.Length; index++)
				{
					if (!current[index].IsRemoved)
					{
						next[nextIndex++] = current[index];
					}
				}

				_subscriptions[type] = next;
				return true;
			}
		}

		private bool Contains(Type type, Delegate @delegate)
		{
			lock (_lock)
			{
				Subscription[] current;
				if (!_subscriptions.TryGetValue(type, out current))
				{
					return false;
				}

				for (int index = 0; index < current.Length; index++)
				{
					if (Equals(current[index].Delegate, @delegate))
					{
						return true;
					}
				}

				return false;
			}
		}

		private sealed class Subscription
		{
			public readonly Delegate Delegate;

			public readonly bool IsAsync;

			public volatile bool IsRemoved;

			public Subscription(Delegate @delegate, bool isAsync)
			{
				Delegate = @delegate;
				IsAsync = isAsync;
			}
		}
	}
}
