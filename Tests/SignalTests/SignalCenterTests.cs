using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gum.Signal.Core;
using NUnit.Framework;

namespace Tests.SignalTests
{
	[TestFixture]
	public class SignalCenterTests
	{
		private SignalCenter _signalCenter;

		private const int VALUE = 5;
		
		[SetUp]
		public void Setup()
		{
			_signalCenter = new SignalCenter();
		}

		[Test]
		public void Subscribe()
		{
			void Action(FooSignal _) { }

			_signalCenter.Subscribe<FooSignal>(Action);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(Action));
		}
		
		[Test]
		public void Unsubscribe()
		{
			void BarAction(BarSignal _) { }
			void FooAction(FooSignal _) { }

			_signalCenter.Subscribe<BarSignal>(BarAction);
			_signalCenter.Subscribe<FooSignal>(FooAction);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(FooAction));
			_signalCenter.Unsubscribe<FooSignal>(FooAction);
			Assert.IsFalse(_signalCenter.Exists<FooSignal>(FooAction));
		}
		
		[Test]
		public void TryUnsubscribe()
		{
			void BarAction(BarSignal _) { }
			void FooAction(FooSignal _) { }

			_signalCenter.Subscribe<BarSignal>(BarAction);
			Assert.IsTrue(_signalCenter.Exists<BarSignal>(BarAction));
			Assert.IsFalse(_signalCenter.Exists<FooSignal>(FooAction));
			bool isUnsubscribedFromBar = _signalCenter.TryUnsubscribe<BarSignal>(BarAction);
			bool isUnsubscribedFromFoo = _signalCenter.TryUnsubscribe<FooSignal>(FooAction);
			Assert.IsFalse(_signalCenter.Exists<FooSignal>(FooAction));
			Assert.IsFalse(_signalCenter.Exists<BarSignal>(BarAction));
			Assert.IsTrue(isUnsubscribedFromBar);
			Assert.IsFalse(isUnsubscribedFromFoo);
		}
		
		[Test]
		public void Unsubscribe_From_Async_Method()
		{
			async Task BarActionAsync(BarSignal _)
			{
				await Task.Delay(100);
			}

			_signalCenter.Subscribe<BarSignal>(BarActionAsync);
			Assert.IsTrue(_signalCenter.Exists<BarSignal>(BarActionAsync));
			
			_signalCenter.Unsubscribe<BarSignal>(BarActionAsync);
			Assert.IsFalse(_signalCenter.Exists<BarSignal>(BarActionAsync));
		}

		[Test]
		public async Task Subscribe_Async_And_Fire()
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			cancellationTokenSource.CancelAfter(200);
			CancellationToken cancellationToken = cancellationTokenSource.Token;
			bool isSignalReceived = false;
			
			async Task BarActionAsync(BarSignal _)
			{
				await Task.Delay(100, cancellationToken);
				isSignalReceived = true;
			}
			
			_signalCenter.Subscribe<BarSignal>(BarActionAsync);
			_signalCenter.Fire(new BarSignal());
			while (!isSignalReceived)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					Assert.Fail("Timed out.");
				}
				
				await Task.Yield();
			}
			
			cancellationTokenSource.Dispose();
		}

		[Test]
		public void Subscribe_And_Fire()
		{
			void FooAction(FooSignal fooSignal)
			{
				Assert.AreEqual(VALUE, fooSignal.Value);
			}

			_signalCenter.Subscribe<FooSignal>(FooAction);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(FooAction));

			_signalCenter.Fire(new FooSignal(VALUE));
		}
		
		[Test]
		public async Task Subscribe_And_Fire_Async()
		{
			FooSignal fooSignalReceived = default;
			void FooAction(FooSignal fooSignal)
			{
				fooSignalReceived = fooSignal;
			}
			
			_signalCenter.Subscribe<FooSignal>(FooAction);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(FooAction));

			await _signalCenter.FireAsync(new FooSignal(VALUE));

			Assert.AreEqual(VALUE, fooSignalReceived.Value);
		}

		[Test]
		public void Fire_When_Handler_Unsubscribes_Itself_Next_Subscriber_Is_Still_Invoked()
		{
			List<string> calls = new List<string>();

			void First(FooSignal _) => calls.Add("First");
			void SelfUnsubscribing(FooSignal _)
			{
				calls.Add("SelfUnsubscribing");
				_signalCenter.Unsubscribe<FooSignal>(SelfUnsubscribing);
			}
			void Last(FooSignal _) => calls.Add("Last");

			_signalCenter.Subscribe<FooSignal>(First);
			_signalCenter.Subscribe<FooSignal>(SelfUnsubscribing);
			_signalCenter.Subscribe<FooSignal>(Last);

			_signalCenter.Fire(new FooSignal(VALUE));

			CollectionAssert.AreEqual(new[] { "First", "SelfUnsubscribing", "Last" }, calls);
		}

		[Test]
		public void Fire_When_Handler_Unsubscribes_Earlier_Subscriber_Remaining_Subscribers_Are_Invoked()
		{
			List<string> calls = new List<string>();

			void First(FooSignal _) => calls.Add("First");
			void UnsubscribesFirst(FooSignal _)
			{
				calls.Add("UnsubscribesFirst");
				_signalCenter.Unsubscribe<FooSignal>(First);
			}
			void Last(FooSignal _) => calls.Add("Last");

			_signalCenter.Subscribe<FooSignal>(First);
			_signalCenter.Subscribe<FooSignal>(UnsubscribesFirst);
			_signalCenter.Subscribe<FooSignal>(Last);

			_signalCenter.Fire(new FooSignal(VALUE));

			CollectionAssert.AreEqual(new[] { "First", "UnsubscribesFirst", "Last" }, calls);
		}

		[Test]
		public void Fire_When_Handler_Unsubscribes_Later_Subscriber_It_Is_Not_Invoked()
		{
			List<string> calls = new List<string>();

			void Middle(FooSignal _) => calls.Add("Middle");
			void UnsubscribesMiddle(FooSignal _)
			{
				calls.Add("UnsubscribesMiddle");
				_signalCenter.Unsubscribe<FooSignal>(Middle);
			}
			void Last(FooSignal _) => calls.Add("Last");

			_signalCenter.Subscribe<FooSignal>(UnsubscribesMiddle);
			_signalCenter.Subscribe<FooSignal>(Middle);
			_signalCenter.Subscribe<FooSignal>(Last);

			_signalCenter.Fire(new FooSignal(VALUE));

			CollectionAssert.AreEqual(new[] { "UnsubscribesMiddle", "Last" }, calls);
		}

		[Test]
		public void Fire_When_Nested_Fire_Handler_Unsubscribes_Outer_Subscribers_Are_Invoked()
		{
			List<string> calls = new List<string>();

			void FiresBar(FooSignal _)
			{
				calls.Add("FiresBar");
				_signalCenter.Fire(new BarSignal(VALUE));
			}
			void UnsubscribesFiresBar(BarSignal _)
			{
				calls.Add("UnsubscribesFiresBar");
				_signalCenter.Unsubscribe<FooSignal>(FiresBar);
			}
			void Last(FooSignal _) => calls.Add("Last");

			_signalCenter.Subscribe<BarSignal>(UnsubscribesFiresBar);
			_signalCenter.Subscribe<FooSignal>(FiresBar);
			_signalCenter.Subscribe<FooSignal>(Last);

			_signalCenter.Fire(new FooSignal(VALUE));

			CollectionAssert.AreEqual(new[] { "FiresBar", "UnsubscribesFiresBar", "Last" }, calls);
		}

		[Test]
		public void Fire_When_Handler_Subscribes_During_Fire_New_Subscriber_Is_Invoked_From_Next_Fire()
		{
			List<string> calls = new List<string>();
			bool hasSubscribedLate = false;

			void Late(FooSignal _) => calls.Add("Late");
			void SubscribesLate(FooSignal _)
			{
				calls.Add("SubscribesLate");
				if (hasSubscribedLate)
				{
					return;
				}

				hasSubscribedLate = true;
				_signalCenter.Subscribe<FooSignal>(Late);
			}

			_signalCenter.Subscribe<FooSignal>(SubscribesLate);

			_signalCenter.Fire(new FooSignal(VALUE));
			CollectionAssert.AreEqual(new[] { "SubscribesLate" }, calls);

			_signalCenter.Fire(new FooSignal(VALUE));
			CollectionAssert.AreEqual(new[] { "SubscribesLate", "SubscribesLate", "Late" }, calls);
		}

		[Test]
		public void Unsubscribe_Removes_All_Duplicate_Subscriptions()
		{
			int callCount = 0;

			void FooAction(FooSignal _) => callCount++;

			_signalCenter.Subscribe<FooSignal>(FooAction);
			_signalCenter.Subscribe<FooSignal>(FooAction);
			_signalCenter.Unsubscribe<FooSignal>(FooAction);

			Assert.IsFalse(_signalCenter.Exists<FooSignal>(FooAction));
			_signalCenter.Fire(new FooSignal(VALUE));
			Assert.AreEqual(0, callCount);
		}

		[Test]
		public void TryUnsubscribe_Removes_All_Duplicate_Subscriptions()
		{
			int callCount = 0;

			void FooAction(FooSignal _) => callCount++;

			_signalCenter.Subscribe<FooSignal>(FooAction);
			_signalCenter.Subscribe<FooSignal>(FooAction);

			Assert.IsTrue(_signalCenter.TryUnsubscribe<FooSignal>(FooAction));
			Assert.IsFalse(_signalCenter.Exists<FooSignal>(FooAction));
			_signalCenter.Fire(new FooSignal(VALUE));
			Assert.AreEqual(0, callCount);
		}

		[Test]
		public void Exists_Ignores_Subscriptions_Of_Other_Signal_Types()
		{
			Action<object> handler = _ => { };

			_signalCenter.Subscribe<FooReferenceSignal>(handler);

			Assert.IsTrue(_signalCenter.Exists<FooReferenceSignal>(handler));
			Assert.IsFalse(_signalCenter.Exists<BarReferenceSignal>(handler));
		}
	}
}