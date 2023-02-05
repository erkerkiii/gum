using System;
using System.IO;

namespace Gum.Composer
{
	public static class UserConfig
	{
		private static readonly string ProjectDirectory = $@"Assets\Plugins\gum\Gum.Composer";
		public static readonly string AspectsDirectoryPath = $@"{ProjectDirectory}\Aspects";
		public static readonly string TypesDirectoryPath = $@"{ProjectDirectory}\Types";
		public static readonly string OutputDirectoryPath = $@"{ProjectDirectory}\Generated";
		public const string NAMESPACE = @"Gum.Composer";
	}
}