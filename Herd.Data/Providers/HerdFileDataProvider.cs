using System;
using System.Collections.Generic;
using System.Text;
using Herd.Data.Models;
using System.IO;
using Newtonsoft.Json;

namespace Herd.Data.Providers
{
    public class HerdFileDataProvider : HerdKeyValuePairDataProvider
    {
        public HerdFileDataProvider()
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

        protected override string ReadKey(string key, string autoCreateValue = null)
        {
            try
            {
                return File.ReadAllText(key + ".json");
            }
            catch (FileNotFoundException) when (autoCreateValue != null)
            {
                WriteKey(key, autoCreateValue);
                return autoCreateValue;
            }
        }

        protected override void WriteKey(string key, string value)
        {
            var dir = Path.GetDirectoryName(key);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // This deserialises into a temp object just to parse it back into 
            // JSON BUT it formats it pretty in the output file. My eyes are happier.
            File.WriteAllText(key + ".json", JsonConvert.SerializeObject(JsonConvert.DeserializeObject(value), Formatting.Indented));
        }
    }
}
