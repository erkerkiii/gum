#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gum.Composer.Unity.Config;
using Gum.Composer.Utility;
using UnityEngine;

namespace Gum.Composer.Unity.Editor
{
    internal static class TypeFileWriter
    {
        private const string LINE = "\n";
        private const string TYPE = "$type";
        private const string TYPE_TEMPLATE = TYPE + LINE;

        public static void WriteTypes(IEnumerable<string> types)
        {
            StringBuilder typeFileStringBuilder = new StringBuilder();
            StringBuilder bodyStringBuilder = new StringBuilder();
            foreach (string type in types) 
            {
                AppendTypeName(type, bodyStringBuilder);
            }

            typeFileStringBuilder.Append(bodyStringBuilder.ToString());

            string typeFilePath = Path.Combine(PathConfig.GetOutputDirectoryPath(), TypeFileConfig.TYPES_FILE);
            
            FilePathHelper.EnsureFilePath(typeFilePath);
            File.WriteAllText(typeFilePath, typeFileStringBuilder.ToString());
        }

        private static void AppendTypeName(string type, StringBuilder bodyStringBuilder)
        {
            string typeString = type;
            bodyStringBuilder.Append(TYPE_TEMPLATE)
                .Replace(TYPE, typeString);
        }
    }
}
#endif
