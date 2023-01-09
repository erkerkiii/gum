using System.Collections.Generic;
using System.IO;
using Gum.Composer.CodeGen.Config;

namespace Gum.Composer.CodeGen.Internal
{
	internal static class AspectFileReader
	{
		private const string SEARCH_PATTERN = "*.gum";

		public static IEnumerable<AspectPrototype> ReadAspects()
		{
			string[] files = Directory.GetFiles(UserConfig.AspectsDirectoryPath, SEARCH_PATTERN, SearchOption.AllDirectories);
			List<AspectPrototype> aspectPrototypes = new List<AspectPrototype>();
			for (int index = 0; index < files.Length; index++)
			{
				string text = File.ReadAllText(files[index]);
				if (ResolveText(text, out AspectPrototype aspectPrototype))
				{
					aspectPrototypes.Add(aspectPrototype);
				}
			}

			return aspectPrototypes;
		}

		private static bool ResolveText(string text, out AspectPrototype aspectPrototype)
		{
			aspectPrototype = default;
			if (!text.StartsWith(Keywords.ASPECT_FILE))
			{
				return false;
			}

			string[] splittedText = text.Split(Keywords.BOF);
			string name = splittedText[0].Replace(Keywords.ASPECT_FILE, "").Trim();
			Dictionary<string, string> fields = new Dictionary<string, string>();
			
			char[] chars = splittedText[1].ToCharArray();
			bool isReadingField = false;
			bool isReadingFieldName = false;
			bool isReadingTypeName = false;
			string fieldName = string.Empty;
			string typeName = string.Empty;
			for (int index = 0; index < chars.Length; index++)
			{
				char currentChar = chars[index];
				if (currentChar == Keywords.EOF)
				{
					break;
				}
				
				if (char.IsWhiteSpace(currentChar))
				{
					if (!isReadingField)
					{
						continue;
					}

					if (isReadingFieldName)
					{
						isReadingFieldName = false;
						isReadingTypeName = true;
					}
				}
				else
				{
					isReadingField = true;
					if (!isReadingFieldName && !isReadingTypeName)
					{
						isReadingFieldName = true;
					}

					if (currentChar == Keywords.EOL)
					{
						fields.Add(fieldName, typeName);
						isReadingField = false;
						isReadingFieldName = false;
						isReadingTypeName = false;
						fieldName = string.Empty;
						typeName = string.Empty;
						continue;
					}

					if (isReadingFieldName)
					{
						typeName += currentChar;
					}
					else
					{
						fieldName += currentChar;
					}
				}
			}


			aspectPrototype = new AspectPrototype(name, fields);
			
			return true;
		}
	}
}