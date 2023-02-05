using System;
using System.Collections.Generic;
using System.IO;
using Gum.Composer.CodeGen.Config;

namespace Gum.Composer.CodeGen.Internal
{
    public static class TypeFileReader
    {
        private const string SEARCH_PATTERN = "*.gumtypes";

        public static IEnumerable<TypePrototype> ReadTypes()
        {
            string[] files = Directory.GetFiles(UserConfig.TypesDirectoryPath, SEARCH_PATTERN,
                SearchOption.AllDirectories);

            string text = File.ReadAllText(files[0]);
            return ResolveText(text);
        }

        private static IEnumerable<TypePrototype> ResolveText(string text)
        {
            List<TypePrototype> typePrototypes = new List<TypePrototype>();

            string[] splittedText = text.Split(Keywords.BOF);
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
                        typePrototypes.Add(new TypePrototype(typeName, Type.GetType(fieldName)));
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

            return typePrototypes;
        }
    }
}