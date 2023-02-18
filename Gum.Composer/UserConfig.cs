using System;
using System.IO;
using Gum.Core.Assert;

namespace Gum.Composer
{
	public static class UserConfig
	{
		private static readonly string ProjectDirectory = GetProjectDirectory(Directory.GetCurrentDirectory());
		public static readonly string AspectsDirectoryPath = $@"{ProjectDirectory}\Aspects";
		public static readonly string OutputDirectoryPath = $@"{ProjectDirectory}\Generated";
		
		public const string NAMESPACE = @"Gum.Composer";
		public const string FOLDER_SEARCH_PATTERN = "Gum.Composer";
		
		private static string GetProjectDirectory(string directory)
		{
			if (TryGetProjectDirectory(directory, out string foundDirectory))
			{
				return foundDirectory;
			}

			DirectoryInfo parentDirectoryInfo = Directory.GetParent(directory);
			if (parentDirectoryInfo != null)
			{
				return GetProjectDirectory(parentDirectoryInfo.FullName);
			}

			throw new GumException("Unable to find Gum.Composer directory!");
		}

		private static bool TryGetProjectDirectory(string directory, out string foundDirectory)
		{
			foundDirectory = string.Empty;
			
			string[] directories =
				Directory.GetDirectories(directory, FOLDER_SEARCH_PATTERN, SearchOption.AllDirectories);
			if (directories.Length > 0)
			{
				foundDirectory = directories[0];
				return true;
			}

			return false;
		}
	}
}