using System;
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
		public void Subscribe_And_Fire()
		{
			void FooAction(FooSignal fooSignal)
			{
				Assert.AreEqual(VALUE, fooSignal.Value);
			}
			void BarAction(BarSignal _) { }
			
			_signalCenter.Subscribe<BarSignal>(BarAction);
			_signalCenter.Subscribe<FooSignal>(FooAction);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(FooAction));

			_signalCenter.Fire(new FooSignal(VALUE));
		}
	}
}