using System;
using System.Collections.Generic;
using System.IO;
using Gum.Composer.CodeGen.Config;

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
            return ResolveText(text);
        }

        private static IEnumerable<Type> ResolveText(string text)
        {
            List<Type> typePrototypes = new ();

            char[] chars = text.ToCharArray();

            bool isReadingField = false;
            bool isReadingFieldName = false;
            bool isReadingTypeName = false;
            string fieldName = string.Empty;
            string typeName = string.Empty;
            
            for (int index = 0; index < chars.Length; index++)
            {
               
            }

            return typePrototypes;
        }
    }
}