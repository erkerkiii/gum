using Gum.Core.Assert;
using Gum.Pooling;
using NUnit.Framework;

namespace Tests.PoolingTests
{
	[TestFixture]
	public class ArrayPoolTests
	{
		[Test]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(10)]
		public void Gets_New_Array_Pool(int arrayLength)
		{
			using PooledArray<int> array = ArrayPool<int>.Get(arrayLength);

			for (int index = 0; index < array.Length; index++)
			{
				array[index] = index;
			}

			Assert.AreEqual(0, array[0]);
		}

		[Test]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(10)]
		public void Gets_Pooled_Array(int arrayLength)
		{
			PooledArray<int> array = ArrayPool<int>.Get(arrayLength * 2);
			array.Dispose();

			using PooledArray<int> pooledArray = ArrayPool<int>.Get(arrayLength);

			Assert.AreEqual(arrayLength * 2, pooledArray.GetSource().Length);
		}
		
		[Test]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(10)]
		public void Throws_Out_Of_Range_Exception(int arrayLength)
		{
			PooledArray<int> array = ArrayPool<int>.Get(arrayLength * 2);
			array.Dispose();

			using PooledArray<int> pooledArray = ArrayPool<int>.Get(arrayLength);

			Assert.Throws<GumException>(() =>
			{
				pooledArray[arrayLength] = 0;
			});
			
			Assert.Throws<GumException>(() =>
			{
				pooledArray[arrayLength + 1] = 0;
			});
			
			Assert.Throws<GumException>(() =>
			{
				int a = pooledArray[arrayLength];
			});
			
			Assert.Throws<GumException>(() =>
			{
				int a = pooledArray[arrayLength + 1];
			});
		}

		[TearDown]
		public void Teardown()
		{
			ArrayPool<int>.Clear();
		}
	}
}