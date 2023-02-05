using System.Collections.Generic;
using System.IO;
using System.Text;
using Gum.Composer.Utility;

namespace Gum.Composer.CodeGen.Internal
{
    public static class AspectFileWriter
    {
        private const string FILE_EXTENTION = ".gum";
        private const string TAB = "\t";
        private const string LINE = "\n";
        private const string TYPE = "$type";
        private const string FIELD_NAME = "$fieldName";
        private const string ASPECT_NAME = "$aspectName";
        private const string BODY = "$BODY";
        private const string ASPECT_FILE_TEMPLATE = "aspect" + " " + ASPECT_NAME +
                                                    LINE + "{" +
                                                    LINE + BODY +
                                                    "}";
        private const string FIELD_TEMPLATE = TAB + TYPE + " " + FIELD_NAME + ";" + LINE + LINE;
        
        public static void WriteAspects(IEnumerable<AspectPrototype> aspectPrototypes)
        {
            StringBuilder aspectStringBuilder = new StringBuilder();
            StringBuilder bodyStringBuilder = new StringBuilder();
            foreach (AspectPrototype aspectPrototype in aspectPrototypes)
            {
                string aspectName = aspectPrototype.Name;
                aspectStringBuilder.Append(ASPECT_FILE_TEMPLATE).Replace(ASPECT_NAME, aspectName);
                
                foreach (KeyValuePair<string,string> aspectPrototypeKvp in aspectPrototype.Fields)
                {
                    string fieldType = aspectPrototypeKvp.Value;
                    string fieldName = aspectPrototypeKvp.Key;
                    bodyStringBuilder.Append(FIELD_TEMPLATE)
                        .Replace(TYPE, fieldType)
                        .Replace(FIELD_NAME, fieldName);
                }
                
                aspectStringBuilder.Replace(BODY, bodyStringBuilder.ToString());
                string aspectFile = $@"{UserConfig.AspectsDirectoryPath}\{aspectName + FILE_EXTENTION}";
                FilePathHelper.EnsureFilePath(aspectFile);
                File.WriteAllText(aspectFile, aspectStringBuilder.ToString());
            }
        }
    }
}