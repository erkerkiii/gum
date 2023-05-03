using Gum.DI.Container;
using Gum.DI.Exception;
using NUnit.Framework;

namespace Tests.DITests
{
	[TestFixture]
	public class DiContainerTests
	{
		private DiContainer _diContainer;

		[SetUp]
		public void SetUp()
		{
			_diContainer = new DiContainer();
		}

		[Test]
		public void Bind_AsSingle()
		{
			_diContainer.Bind<IFoo>().To<Foo>().AsSingle();
			Assert.Pass();
		}
		
		[Test]
		public void Binding_Multiple_AsSingle_Throws()
		{
			Assert.Throws<BindingAlreadyExistsException>(() =>
			{
				_diContainer.Bind<IFoo>().To<Foo>().AsSingle();
				_diContainer.Bind<IFoo>().To<Foo>().AsSingle();
			});
		}
		
		[Test]
		public void DiContainer_Resolve()
		{
			_diContainer.Bind<IFoo>().To<Foo>().AsSingle();
			_diContainer.Bind<IBar>().To<Bar>().AsSingle();
			_diContainer.Bind<Baz>().To<Baz>().AsSingle();
			_diContainer.Bind<Qux>().To<Qux>().AsSingle();

			Qux qux = _diContainer.Resolve<Qux>();

			Assert.NotNull(qux);
			Assert.NotNull(qux.GetFoo());
			Assert.NotNull(qux.Baz);
			Assert.NotNull(qux.Baz.GetBar());
			Assert.NotNull(qux.Baz.GetBar().GetFoo());
		}
		
		[Test]
		public void DiContainer_Resolve_FromInstance()
		{
			_diContainer.Bind<IFoo>().FromInstance(new Foo()).AsSingle();
			_diContainer.Bind<IBar>().To<Bar>().AsSingle();

			IBar bar = _diContainer.Resolve<IBar>();

			Assert.NotNull(bar);
			Assert.NotNull(bar.GetFoo());
		}
		
		[Test]
		public void DiContainer_No_Binding_Resolve_Throws()
		{
			Assert.Throws<BindingNotFoundException>(() =>
			{
				_diContainer.Bind<IBar>().To<Bar>().AsSingle();
				_diContainer.Resolve<IBar>();
			});
		}
	}
}