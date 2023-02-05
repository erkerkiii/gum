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
        private static readonly string[] CommonUnityTypeNames =
        {
            typeof(Vector3).FullName,
            typeof(Vector2).FullName,
            typeof(Transform).FullName,
            typeof(Quaternion).FullName,
            typeof(GameObject).FullName,
            typeof(Collider).FullName,
        };

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

            foreach (string type in CommonUnityTypeNames)
            {
                AppendTypeName(type, bodyStringBuilder);
            }
            
            foreach (string type in typeof(Type).Assembly.GetTypes()
                         .Where(x => x.IsPrimitive).Select(x => x.FullName))
            {
                AppendTypeName(type, bodyStringBuilder);
            }

            typeFileStringBuilder.Append(bodyStringBuilder.ToString());

            string typeFilePath = $@"{UserConfig.OutputDirectoryPath}\{TypeFileConfig.TYPES_FILE}";

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
