using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Herd.Data.Providers
{
    public class FileDataProvider : KeyValuePairDataProvider
    {
        public static string FILE_EXTENSION = ".json";

        public FileDataProvider()
            : base(GetOrCreateRootDataFolder(), Path.DirectorySeparatorChar.ToString())
        {
        }

        private static string GetOrCreateRootDataFolder()
        {
            var rootDataFolder = Path.Combine(Path.GetTempPath(), "HerdData");
            if (!Directory.Exists(rootDataFolder))
            {
                Directory.CreateDirectory(rootDataFolder);
            }
            return rootDataFolder;
        }

        protected override string ReadKey(string key)
        {
            return File.ReadAllText($"{key}{FILE_EXTENSION}");
        }

        protected override void WriteKey(string key, string value)
        {
            AutoCreateParentDirectory(key);
            File.WriteAllText($"{key}{FILE_EXTENSION}", value);
        }

        protected override IEnumerable<string> GetAllKeys(string rootKey)
        {
            AutoCreateDirectory(rootKey);
            foreach (var fileName in Directory.EnumerateFiles(rootKey).Where(f => f.EndsWith(FILE_EXTENSION)))
            {
                yield return fileName.Substring(0, fileName.Length - FILE_EXTENSION.Length);
            }
        }

        private static void AutoCreateParentDirectory(string key)
        {
            AutoCreateDirectory(Path.GetDirectoryName(key));
        }

        private static void AutoCreateDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}