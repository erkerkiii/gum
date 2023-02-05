using System;
using System.Collections.Generic;
using System.IO;
using Gum.Composer.CodeGen.Config;
using UnityEngine;

namespace Gum.Composer.CodeGen.Internal
{
    public static class TypeFileReader
    {
        private const string SEARCH_PATTERN = "*.gumtypes";

        public static IEnumerable<Type> ReadTypes()
        {
            string[] files = Directory.GetFiles(UserConfig.TypesDirectoryPath, SEARCH_PATTERN,
                SearchOption.AllDirectories);

            string text = File.ReadAllText(files[0]);
            (IEnumerable<Type> types, IEnumerable<string> _) readTypes = ResolveText(text);
            return readTypes.types;
        }
        
        public static IEnumerable<string> ReadTypesAsString()
        {
            string[] files = Directory.GetFiles(UserConfig.TypesDirectoryPath, SEARCH_PATTERN,
                SearchOption.AllDirectories);

            string text = File.ReadAllText(files[0]);
            (IEnumerable<Type> _, IEnumerable<string> strings) readTypes = ResolveText(text);
            return readTypes.strings;
        }

        private static (IEnumerable<Type> types, IEnumerable<string> strings) ResolveText(string text)
        {
            List<Type> readTypes = new();
            List<string> readStrings = new();

            char[] chars = text.ToCharArray();

            bool isReadingType = false;
            string typeName = string.Empty;
            
            for (int index = 0; index < chars.Length; index++)
            {
                char currentChar = chars[index];

                if (char.IsWhiteSpace(currentChar))
                {
                    readStrings.Add(typeName);
                    readTypes.Add( Type.GetType(typeName));
                    typeName = string.Empty;
                    continue;
                }

                typeName += currentChar;
            }

            Debug.Log(readStrings.Count);
            return (readTypes, readStrings);
        }
    }
}