using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class FileUtil
    {
        public static T LoadJsonFile<T>(string fileName) where T : class
        {
            var json = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveJsonFile<T>(T jsonObject, string path) where T : class
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
            }
        }

        public static bool CheckExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
