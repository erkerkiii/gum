using System;
using System.Collections;
using System.Collections.Generic;

#if DEBUG || UNITY_EDITOR
namespace Gum.Pooling.Diagnostics
{
	internal static class PoolMonitor
	{
		public static readonly Dictionary<Array, string> UnreleasedPooledArrays = new Dictionary<Array, string>();
		
		public static readonly Dictionary<IList, string> UnreleasedPooledLists = new Dictionary<IList, string>();
		
		public static readonly Dictionary<IDictionary, string> UnreleasedPooledDictionaries = new Dictionary<IDictionary, string>();

		static PoolMonitor()
		{
			AppDomain.CurrentDomain.ProcessExit += CheckLeakedPools;
		}

		private static void CheckLeakedPools(object _, EventArgs __)
		{
			foreach (string stackTrace in UnreleasedPooledArrays.Values)
			{
				#if UNITY_EDITOR
				UnityEngine.Debug.LogError($"ArrayPool not disposed correctly: {kvp.Value}");
				#else
				Console.WriteLine($"ArrayPool not disposed correctly: {stackTrace}");
				#endif
			}
			
			foreach (string stackTrace in UnreleasedPooledLists.Values)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogError($"PooledList not disposed correctly: {kvp.Value}");
#else
				Console.WriteLine($"PooledList not disposed correctly: {stackTrace}");
#endif
			}
			
			foreach (string stackTrace in UnreleasedPooledDictionaries.Values)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.LogError($"PooledDictionary not disposed correctly: {kvp.Value}");
#else
				Console.WriteLine($"PooledDictionary not disposed correctly: {stackTrace}");
#endif
			}
		}
	}
}
#endif