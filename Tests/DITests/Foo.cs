using Gum.DI.CompilerServices.Attribute;

namespace Tests.DITests
{
	public class Foo : IFoo
	{
	}

	public interface IFoo
	{
	}
	
	public class Bar : IBar
	{
		private readonly IFoo _foo;
		
		public Bar(IFoo foo)
		{
			_foo = foo;
		}

		public IFoo GetFoo()
		{
			return _foo;
		}
	}

	public interface IBar
	{
		IFoo GetFoo();
	}
	
	public class Baz
	{
		[Inject]
		private readonly IBar _bar;

		public IBar GetBar()
		{
			return _bar;
		}
	}

	public class Qux
	{
		[Inject]
		public Baz Baz { get; private set; }

		private IFoo _foo;

		[Inject]
		public void Construct(IFoo foo)
		{
			_foo = foo;
		}
		
		public IFoo GetFoo()
		{
			return _foo;
		}
	}
}