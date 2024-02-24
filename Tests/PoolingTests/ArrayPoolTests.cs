﻿using Gum.Core.Assert;
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
				array[index] = index + 1;
			}

			Assert.AreEqual(1, array[0]);
		}

		[Test]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(10)]
		public void Gets_Pooled_Array(int arrayLength)
		{
			PooledArray<int> array = ArrayPool<int>.Get(arrayLength * 2);
			int[] source = array.GetSource();
			array.Dispose();

			using PooledArray<int> pooledArray = ArrayPool<int>.Get(arrayLength);

			Assert.AreSame(source, pooledArray.GetSource());
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

		[Test]
		[TestCase(1)]
		[TestCase(5)]
		[TestCase(10)]
		public void ToArray(int arrayLength)
		{
			PooledArray<int> array = ArrayPool<int>.Get(arrayLength * 2);
			array.Dispose();
			using PooledArray<int> pooledArray = ArrayPool<int>.Get(arrayLength);
			for (int index = 0; index < pooledArray.Length; index++)
			{
				pooledArray[index] = index + 1;
			}
			
			int[] createdArray = pooledArray.ToArray();
			
			Assert.AreEqual(pooledArray[0], createdArray[0]);
			Assert.AreEqual(pooledArray.Length, createdArray.Length);
		}

		[Test]
		public void SanityCheck_Throws()
		{
			PooledArray<int> pooledArray = new PooledArray<int>();
			Assert.Throws<GumException>(() =>
			{
				pooledArray[0] = 0;
			});
			
			Assert.Throws<GumException>(() =>
			{
				int a = pooledArray[0];
			});
			
			Assert.Throws<GumException>(() =>
			{
				pooledArray.ToArray();
			});
			
			Assert.Throws<GumException>(() =>
			{
				pooledArray.GetSource();
			});
			
			Assert.Throws<GumException>(() =>
			{
				pooledArray.Dispose();
			});
		}

		[TearDown]
		public void Teardown()
		{
			ArrayPool<int>.Clear();
		}
	}
}