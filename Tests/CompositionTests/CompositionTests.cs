using Gum.Composition;
using Gum.Composition.Exception;
using Gum.Composition.Generated;
using NUnit.Framework;

namespace Tests.CompositionTests
{
	[TestFixture]
	public class CompositionTests
	{
		private Composition _composition;

		private const int VALUE = 5;
		
		[SetUp]
		public void Setup()
		{
			_composition = new MockComposable(VALUE).GetComposition();
		}

		[Test]
		public void Get_Aspect()
		{
			BarAspect barAspect = _composition.GetAspect<BarAspect>();
			Assert.AreEqual(VALUE, barAspect.MyInt);
		}

		[Test]
		public void Get_Aspect_With_Indexer()
		{
			BarAspect barAspect = (BarAspect)_composition[BarAspect.ASPECT_TYPE];
			Assert.AreEqual(VALUE, barAspect.MyInt);
		}

		[Test]
		public void Has_Aspect()
		{
			Assert.IsTrue(_composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.IsFalse(_composition.HasAspect((AspectType)1));
		}

		[Test]
		public void Add_Aspect()
		{
			using Composition composition = Composition.Create();

			Assert.IsFalse(composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.AreEqual(0, composition.AspectCount);

			composition.AddAspect(new BarAspect(VALUE));
			Assert.IsTrue(composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.AreEqual(1, composition.AspectCount);
		}

		[Test]
		public void Set_Aspect()
		{
			using Composition composition = new MockComposable(VALUE).GetComposition();
			
			Assert.AreEqual(VALUE, composition.GetAspect<BarAspect>().MyInt);

			const int newValue = 10;
			composition.SetAspect(new BarAspect(newValue));
			Assert.AreEqual(newValue, composition.GetAspect<BarAspect>().MyInt);
		}

		[Test]
		public void Set_Non_Existing_Aspect()
		{
			using Composition composition = Composition.Create();
			Assert.IsFalse(composition.HasAspect(BarAspect.ASPECT_TYPE));

			const int newValue = 10;
			composition.SetAspect(new BarAspect(newValue));
			Assert.AreEqual(newValue, composition.GetAspect<BarAspect>().MyInt);
			Assert.AreEqual(1, composition.AspectCount);
		}
		
		[Test]
		public void Remove_Aspect()
		{
			using Composition composition = Composition.Create(new IAspect[] { new BarAspect() });
			Assert.IsTrue(composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.AreEqual(1, composition.AspectCount);

			composition.RemoveAspect(BarAspect.ASPECT_TYPE);
			Assert.IsFalse(composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.AreEqual(0, composition.AspectCount);
		}
		
		[Test]
		public void Sanity_Check_Throws_Exception()
		{
			Composition composition = new Composition();
			Assert.Catch<InvalidCompositionException>(() =>
			{
				composition.HasAspect(BarAspect.ASPECT_TYPE);
			});

			Assert.Catch(composition.Dispose);
		}

		[TearDown]
		public void TearDown()
		{
			_composition.Dispose();
		}
	}
}