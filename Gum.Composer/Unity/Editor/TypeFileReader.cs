#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Gum.Composer.Unity.Config;
using Gum.Composer.Utility;

namespace Gum.Composer.Unity.Editor
{
    public static class TypeFileReader
    {
        private const string SEARCH_PATTERN = "*.gum";

        public static IEnumerable<string> ReadTypesAsString()
        {
            string text = GetTypeText();
            if (text == string.Empty)
            {
                TypeFileWriter.WriteTypes(Array.Empty<string>());
                text = GetTypeText();
            }

            IEnumerable<string> readTypes = ResolveText(text);
            return readTypes;
        }

        private static string GetTypeText()
        {
            FilePathHelper.EnsureFilePath($@"{UserConfig.OutputDirectoryPath}\{TypeFileConfig.TYPES_FILE}");

            string[] files = Directory.GetFiles(UserConfig.OutputDirectoryPath, SEARCH_PATTERN,
                SearchOption.AllDirectories);
            string text = File.ReadAllText(files[0]);
            return text;
        }

        private static IEnumerable<string> ResolveText(string text)
        {
            List<string> readStrings = new List<string>();

            char[] chars = text.ToCharArray();

            string typeName = string.Empty;

            for (int index = 0; index < chars.Length; index++)
            {
                char currentChar = chars[index];

                if (char.IsWhiteSpace(currentChar))
                {
                    readStrings.Add(typeName);
                    typeName = string.Empty;
                    continue;
                }

                typeName += currentChar;
            }

            return readStrings;
        }
    }
}
#endif