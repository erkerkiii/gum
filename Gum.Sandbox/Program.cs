using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gum.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (StreamWriter sw = File.CreateText(
                       $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/Test.cs"))
            {
                DataEntry[] dataEntries = new DataEntry[3];
                dataEntries[0] = new DataEntry("First");
                dataEntries[1] = new DataEntry("Second");
                dataEntries[2] = new DataEntry("Third");
                
                

                for (int index = 0; index < dataEntries.Length; index++)
                {
                    stringBuilder.AppendLine($"\t\t{dataEntries[index].Value},");
                }
                
                KeywordMap[CONTENT] = stringBuilder.ToString();
                KeywordMap[NAMESPACE] = "Gum";
                KeywordMap[ENUM] = "TestEnum";

                string finalString = ENUM_TEMPLATE;
                foreach (KeyValuePair<string,string> keyValuePair in KeywordMap)
                {
                    finalString = finalString.Replace(keyValuePair.Key, keyValuePair.Value);
                }
                
                sw.Write(finalString);
                sw.Close();
            }
        }

        private static readonly Dictionary<string, string> KeywordMap = new()
        {
            { NAMESPACE, NAMESPACE },
            { CLASS, CLASS },
            { ENUM, ENUM },
            { CONTENT, CONTENT }
        };

        private const string NAMESPACE = "$namespace";
        private const string CLASS = "$class";
        private const string ENUM = "$enum";
        private const string CONTENT = "$content";

        private const string ENUM_TEMPLATE = @"namespace " + NAMESPACE + ".Generated" +
                                             "\n{" +
                                             "\n\tpublic enum $enum" +
                                             "\n\t{" +
                                             "\n" + CONTENT +
                                             "\t}" +
                                             "\n}";
        
        public readonly struct DataEntry
        {
            public readonly string Value;

            public DataEntry(string value)
            {
                Value = value;
            }
        }
    }
}