﻿using System;
using System.IO;

namespace Gum.Composer
{
	public static class UserConfig
	{
		private static readonly string ProjectDirectory = $@"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent}\Gum.Composer";
		public static readonly string AspectsDirectoryPath = $@"{ProjectDirectory}\Aspects";
		public static readonly string OutputDirectoryPath = $@"{ProjectDirectory}\Generated";
		public const string NAMESPACE = @"Gum.Composer";
	}
}