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
			void Action(FooSignal _) { }

			_signalCenter.Subscribe<FooSignal>(Action);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(Action));
			_signalCenter.Unsubscribe((Action<FooSignal>)Action);
			Assert.IsFalse(_signalCenter.Exists<FooSignal>(Action));
		}
		
		[Test]
		public void Subscribe_And_Fire()
		{
			void Action(FooSignal fooSignal)
			{
				Assert.AreEqual(VALUE, fooSignal.Value);
			}
			
			_signalCenter.Subscribe<FooSignal>(Action);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(Action));

			_signalCenter.Fire(new FooSignal(VALUE));
		}
	}
}