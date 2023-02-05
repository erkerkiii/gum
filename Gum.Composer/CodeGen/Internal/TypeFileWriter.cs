using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gum.Composer.CodeGen.Internal
{
    public static class TypeFileWriter
    {
        private const string TYPES_FILE = "Types.gumtypes";
        private const string TAB = "\t";
        private const string LINE = "\n";
        private const string TYPE_NAME = "$typeName";
        private const string TYPE = "$type";
        private const string BODY = "$BODY";
        private const string TYPE_FILE_TEMPLATE = "types" +
                                                  LINE + "{" +
                                                  LINE + BODY +
                                                  "}";
        private const string TYPE_TEMPLATE = TAB + TYPE_NAME + ", " + TYPE + ";" + LINE;

        public static void WriteTypes(IEnumerable<TypePrototype> typePrototypes)
        {
            StringBuilder typeFileStringBuilder = new StringBuilder();
            StringBuilder bodyStringBuilder = new StringBuilder();
            foreach (TypePrototype typePrototype in typePrototypes)
            {
                string typeName = typePrototype.TypeName;
                string type = typePrototype.Type.ToString();

                bodyStringBuilder.Append(TYPE_TEMPLATE)
                    .Replace(TYPE_NAME, typeName)
                    .Replace(TYPE, type);
            }
            typeFileStringBuilder
                .Append(TYPE_FILE_TEMPLATE)
                .Replace(BODY, bodyStringBuilder.ToString());

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