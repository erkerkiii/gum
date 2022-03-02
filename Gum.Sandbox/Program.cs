using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

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

                DataEntry<string>[] dataEntries = 
                {
                    new("First"),
                    new("Second"),
                    new("Third")
                };
                
                for (int index = 0; index < dataEntries.Length; index++)
                {
                    stringBuilder.AppendLine($"\t\t{dataEntries[index].Value},");
                    Console.WriteLine(dataEntries[index].ToJson());
                }
                
                KeywordMap[CONTENT] = stringBuilder.ToString();
                KeywordMap[NAMESPACE] = "Gum";
                KeywordMap[ENUM] = "TestEnum";

                string finalString = KeywordMap.Aggregate(ENUM_TEMPLATE,
                    (current, keyValuePair) => current.Replace(keyValuePair.Key, keyValuePair.Value));

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
        
        [Serializable]
        public readonly struct DataEntry<T>
        {
            public readonly T Value;

            public DataEntry(T value)
            {
                Value = value;
            }

            public string ToJson() => JsonSerializer.Serialize(Value);
        }
    }
}