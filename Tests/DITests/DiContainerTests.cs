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
		public void Creates_SubContainers()
		{
			DiContainer subContainer = _diContainer
				.CreateSubContainer()
				.CreateSubContainer()
				.CreateSubContainer();

			Assert.AreSame(_diContainer, subContainer.GetParentContainer().GetParentContainer().GetParentContainer());
		}

		[Test]
		public void Creates_SubContainers_And_Resolves()
		{
			DiContainer first = _diContainer.CreateSubContainer();
			DiContainer second = first.CreateSubContainer();
			DiContainer third = second.CreateSubContainer();
			
			_diContainer.Bind<IFoo>().To<Foo>().AsSingle().Lazy();
			first.Bind<IBar>().To<Bar>().AsSingle().Lazy();
			second.Bind<Baz>().To<Baz>().AsSingle().Lazy();
			third.Bind<Qux>().To<Qux>().AsSingle().Lazy();
			
			Qux qux = third.Resolve<Qux>();

			Assert.NotNull(qux);
			Assert.NotNull(qux.GetFoo());
			Assert.NotNull(qux.Baz);
			Assert.NotNull(qux.Baz.GetBar());
			Assert.NotNull(qux.Baz.GetBar().GetFoo());
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
				_diContainer.Bind<IFoo>().To<Foo>().AsSingle().Lazy();
				_diContainer.Bind<IFoo>().To<Foo>().AsSingle().Lazy();
			});
		}
		
		[Test]
		public void DiContainer_Resolve()
		{
			_diContainer.Bind<IFoo>().To<Foo>().AsSingle().Lazy();
			_diContainer.Bind<IBar>().To<Bar>().AsSingle().Lazy();
			_diContainer.Bind<Baz>().To<Baz>().AsSingle().Lazy();
			_diContainer.Bind<Qux>().To<Qux>().AsSingle().Lazy();

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
			_diContainer.Bind<IFoo>().FromInstance(new Foo());
			_diContainer.Bind<IBar>().To<Bar>().AsSingle().Lazy();

			IBar bar = _diContainer.Resolve<IBar>();

			Assert.NotNull(bar);
			Assert.NotNull(bar.GetFoo());
		}
		
		[Test]
		public void DiContainer_No_Binding_Resolve_Throws()
		{
			Assert.Throws<BindingNotFoundException>(() =>
			{
				_diContainer.Bind<IBar>().To<Bar>().AsSingle().Lazy();
				_diContainer.Resolve<IBar>();
			});
		}
	}
}