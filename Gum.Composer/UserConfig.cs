using System.IO;
using Gum.Core.Assert;

namespace Gum.Composer
{
	public static class UserConfig
	{
		private static readonly string ProjectDirectory = GetProjectDirectory(Directory.GetCurrentDirectory());
		public static readonly string AspectsDirectoryPath = $@"{ProjectDirectory}\{OUTPUT_FOLDER_NAME_PATTERN}\{ASPECT_FOLDER_NAME_PATTERN}";
		public static readonly string OutputDirectoryPath = $@"{ProjectDirectory}\{OUTPUT_FOLDER_NAME_PATTERN}";
		
		public const string NAMESPACE = @"Gum.Composer";
		public const string FOLDER_SEARCH_PATTERN = "Assets";
		private const string OUTPUT_FOLDER_NAME_PATTERN = @"Gum\Composer\Generated";
		private const string ASPECT_FOLDER_NAME_PATTERN = "Aspects";
		
		private static string GetProjectDirectory(string directory)
		{
			if (TryGetProjectDirectory(directory, out string foundDirectory))
			{
				CreateRequiredFoldersIfNotExist(foundDirectory);
				
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

		private static void CreateRequiredFoldersIfNotExist(string foundDirectory)
		{
			string[] directories;

			try
			{
				directories = Directory.GetDirectories(foundDirectory, OUTPUT_FOLDER_NAME_PATTERN, SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException directoryNotFoundException)
			{
				string outputFolderNamePattern = $@"{foundDirectory}\{OUTPUT_FOLDER_NAME_PATTERN}";
			
				Directory.CreateDirectory(outputFolderNamePattern);
				Directory.CreateDirectory($@"{outputFolderNamePattern}\{ASPECT_FOLDER_NAME_PATTERN}");
			}
		}
	}
}