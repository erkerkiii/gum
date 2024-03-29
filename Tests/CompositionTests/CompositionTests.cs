﻿using Gum.Composer;
using Gum.Composer.Exception;
using NUnit.Framework;

namespace Tests.CompositionTests
{
	[TestFixture]
	public class CompositionTests
	{
		private const int VALUE = 5;
		
		[Test]
		public void Get_Aspect()
		{
			using Composition composition = new MockComposable(VALUE).GetComposition();
			BarAspect barAspect = composition.GetAspect<BarAspect>();
			Assert.AreEqual(VALUE, barAspect.MyInt);
		}
		
		[Test]
		public void Try_Get_Aspect()
		{
			using Composition composition = new MockComposable(VALUE).GetComposition();

			Assert.IsTrue(composition.TryGetAspect(out BarAspect barAspect));
			Assert.AreEqual(VALUE, barAspect.MyInt);
			Assert.IsFalse(composition.TryGetAspect(out FooAspect _));
		}
		
		[Test]
		public void Get_Aspect_Fluent()
		{
			using Composition composition =
				Composition.Create(new IAspect[] { new BarAspect(VALUE), new FooAspect(VALUE) });

			composition
				.GetAspectFluent(out FooAspect fooAspect)
				.GetAspectFluent(out BarAspect barAspect);
			
			Assert.AreEqual(VALUE, barAspect.MyInt);
			Assert.AreEqual(VALUE, fooAspect.MyInt);
		}

		
		[Test]
		public void Get_Aspect_With_Indexer()
		{
			using Composition composition = new MockComposable(VALUE).GetComposition();
			BarAspect barAspect = (BarAspect)composition[BarAspect.ASPECT_TYPE];
			Assert.AreEqual(VALUE, barAspect.MyInt);
		}

		[Test]
		public void Has_Aspect()
		{
			using Composition composition = new MockComposable(VALUE).GetComposition();

			Assert.IsTrue(composition.HasAspect(BarAspect.ASPECT_TYPE));
			Assert.IsFalse(composition.HasAspect(FooAspect.ASPECT_TYPE));
			Assert.IsTrue(composition.HasAspect(TagAspect.ASPECT_TYPE));
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
		
		[Test]
		public void Enumerator()
		{
			using Composition composition = Composition.Create(new IAspect[] {new BarAspect(), new FooAspect()});
			foreach (IAspect aspect in composition)
			{
				Assert.IsTrue(aspect is BarAspect || aspect is FooAspect);
			}

			Assert.Pass();
		}
	}
}