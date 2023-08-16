using System.IO;
using Gum.Core.Assert;
using UnityEngine;

namespace Gum.Composer
{
	internal static class PathConfig
	{
		private static readonly string ProjectDirectory = GetProjectDirectory(Directory.GetCurrentDirectory());

		public const string NAMESPACE = @"Gum.Composer";
		private const string FOLDER_SEARCH_PATTERN = "Assets";
		private const string COMPOSER_FOLDER_PATH = "Composer";
		private const string GENERATED_FOLDER_PATH = "Generated";
		private const string GUM_FOLDER_PATH = "Gum";
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

		private static void CreateRequiredFoldersIfNotExist(string directoryPath)
		{
			string folderNamePattern = GetOutputFolderNamePattern();
			
			try
			{
				Directory.GetDirectories(directoryPath, folderNamePattern, SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException directoryNotFoundException)
			{
				string outputFolderNamePattern = Path.Combine(directoryPath, folderNamePattern);
				string outputAspectFolderNamePattern =
					Path.Combine(outputFolderNamePattern, ASPECT_FOLDER_NAME_PATTERN);

				Directory.CreateDirectory(outputFolderNamePattern);
				Directory.CreateDirectory(outputAspectFolderNamePattern);
			}
		}

		public static string GetOutputDirectoryPath()
		{
			return Path.Combine(ProjectDirectory, GetOutputFolderNamePattern());	
		}

		public static string GetAspectsDirectoryPath()
		{
			return Path.Combine(ProjectDirectory, GetOutputFolderNamePattern(),
				ASPECT_FOLDER_NAME_PATTERN);
		}

		private static string GetOutputFolderNamePattern()
		{
			return Path.Combine(GUM_FOLDER_PATH, COMPOSER_FOLDER_PATH, GENERATED_FOLDER_PATH);
		}
	}
}