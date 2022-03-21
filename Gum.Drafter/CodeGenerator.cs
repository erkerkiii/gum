using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Gum.Drafter.Model;

namespace Gum.Drafter
{
    public static class CodeGenerator
    {
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

        private const string CLASS_TEMPLATE = @"namespace " + NAMESPACE + ".Generated" +
                                              "\n{" +
                                              "\n\tpublic static class " + CLASS +
                                              "\n\t{" +
                                              "\n" + CONTENT +
                                              "\t}" +
                                              "\n}";

        public static async Task GenerateFromRepositoryAsync(Repository repository, string namespaceName,
            string rootFolder)
        {
            try
            {
                StringBuilder contentStringBuilder = new StringBuilder();

                int entitiesCount = repository.entities.Count;
                for (int entityIndex = 0; entityIndex < entitiesCount; entityIndex++)
                {
                    Entity entity = repository.entities[entityIndex];
                    contentStringBuilder.AppendLine($"\t\tpublic const int {entity.name} = {entity.id};");
                    
                    await GenerateEntityClassAsync(entity);
                }

                Dictionary<string, string> keyWordMap = GetKeyWordMap();
                keyWordMap[CLASS] = repository.Name;
                keyWordMap[NAMESPACE] = namespaceName;
                keyWordMap[CONTENT] = contentStringBuilder.ToString();

                string fileContent = keyWordMap
                    .Select((_, index) => keyWordMap.ElementAt(index)).Aggregate(
                        CLASS_TEMPLATE, (current, keyValuePair) => current.Replace(keyValuePair.Key, keyValuePair.Value));

                string folderPath = $"{rootFolder}/{namespaceName}/Generated";
                EnsureFolderPath(folderPath);

                string filePath = $"{folderPath}/{repository.Name}.cs";
                EnsureFilePath(filePath);

                await WriteToPathAsync(filePath, fileContent);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static async Task GenerateEntityClassAsync(Entity entity)
        {
            List<Aspect> entityAspects = entity.aspects;
            for (int aspectIndex = 0; aspectIndex < entityAspects.Count; aspectIndex++)
            {
                Aspect aspect = entityAspects[aspectIndex];
            }
        }
        
        private static async Task WriteToPathAsync(string filePath, string fileContent)
        {
            await using StreamWriter streamWriter = new StreamWriter(filePath);
            await streamWriter.WriteAsync(fileContent);
            streamWriter.Close();
        }
        
        private static void EnsureFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        private static void EnsureFolderPath(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private static Dictionary<string, string> GetKeyWordMap() => new()
        {
            { NAMESPACE, NAMESPACE },
            { CLASS, CLASS },
            { ENUM, ENUM },
            { CONTENT, CONTENT }
        };
    }
}