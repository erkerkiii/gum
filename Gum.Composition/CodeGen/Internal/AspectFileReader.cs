using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gum.Composition.CodeGen.Config;

namespace Gum.Composition.CodeGen.Internal
{
	internal static class AspectFileReader
	{
		private const string SEARCH_PATTERN = "*.gum";

		public static async Task<IEnumerable<AspectPrototype>> ReadAspectsAsync()
		{
			string[] files = Directory.GetFiles(Gum.Composition.UserConfig.ASPECTS_DIRECTORY_PATH, SEARCH_PATTERN, SearchOption.AllDirectories);
			List<AspectPrototype> aspectPrototypes = new List<AspectPrototype>();
			for (int index = 0; index < files.Length; index++)
			{
				string text = await File.ReadAllTextAsync(files[index]);
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
			string typeName = string.Empty;
			string fieldName = string.Empty;
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
						typeName = string.Empty;
						fieldName = string.Empty;
						continue;
					}

					if (isReadingFieldName)
					{
						fieldName += currentChar;
					}
					else
					{
						typeName += currentChar;
					}
				}
			}


			aspectPrototype = new AspectPrototype(name, fields);
			
			return true;
		}
	}
}