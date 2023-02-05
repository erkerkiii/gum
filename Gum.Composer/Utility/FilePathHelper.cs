using System.IO;

namespace Gum.Composer.Utility
{
    internal static class FilePathHelper
    {
        public static void EnsureFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }
    }
}