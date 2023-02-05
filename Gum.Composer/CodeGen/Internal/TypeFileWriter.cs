using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gum.Composer.CodeGen.Internal
{
    public static class TypeFileWriter
    {
        private const string TYPES_FILE = "Types.gumtypes";
        private const string LINE = "\n";
        private const string TYPE = "$type";
        private const string TYPE_TEMPLATE = TYPE + LINE;

        public static void WriteTypes(IEnumerable<Type> types)
        {
            StringBuilder typeFileStringBuilder = new StringBuilder();
            StringBuilder bodyStringBuilder = new StringBuilder();
            foreach (Type type in types)
            {
                string typeString = type.ToString();

                bodyStringBuilder.Append(TYPE_TEMPLATE)
                    .Replace(TYPE, typeString);
            }
            typeFileStringBuilder.Append(bodyStringBuilder.ToString());

            string typeFilePath = $@"{UserConfig.TypesDirectoryPath}\{TYPES_FILE}";

            EnsureFilePath(typeFilePath);
            File.WriteAllText(typeFilePath, typeFileStringBuilder.ToString());
        }
        
        private static void EnsureFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }
    }
}