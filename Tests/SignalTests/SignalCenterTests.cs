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
			void BarAction(BarSignal _) { }
			
			_signalCenter.Subscribe<BarSignal>(BarAction);
			_signalCenter.Subscribe<FooSignal>(FooAction);
			Assert.IsTrue(_signalCenter.Exists<FooSignal>(FooAction));

			_signalCenter.Fire(new FooSignal(VALUE));
		}
	}
}