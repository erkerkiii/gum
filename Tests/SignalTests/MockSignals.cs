namespace Tests.SignalTests
{
	public readonly struct FooSignal
	{
		public readonly int Value;

		public FooSignal(int value)
		{
			Value = value;
		}
	}
	
	public readonly struct BarSignal
	{
		public readonly int Value;

		public BarSignal(int value)
		{
			Value = value;
		}
	}
}