using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    static class SettingsManager
    {
        public static JObject GetJObject(string path)
        {
            string txt = File.ReadAllText(Path.Combine(AppPaths.SettingsPath, path + ".json"));
            return JObject.Parse(txt);
        }

        public static JToken GetJToken(string path, string key)
        {
            JObject obj = GetJObject(path);
            return obj[key];
        }

        public static Dictionary<string, object> Get(string path)
        {
            return GetJObject(path).ToObject<Dictionary<string, object>>();
        }

        public static T Get<T>(string path, string key)
        {
            return GetJToken(path, key).ToObject<T>();
        }

        public static void Set<T>(string path, string key, T value)
        {
            JObject obj = GetJObject(path);
            obj[key] = JToken.FromObject(value);
            File.WriteAllText(Path.Combine(AppPaths.SettingsPath, path + ".json"), obj.ToString());
        }

        public static void RemoveKey(string path, string key)
        {
            JObject obj = GetJObject(path);
            obj.Remove(key);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(Path.Combine(AppPaths.SettingsPath, path + ".json"));
        }

        public static bool KeyExists(string path, string key)
        {
            JObject obj = GetJObject(path);
            return obj.ContainsKey(key);
        }

        public static void CreateFile(string path)
        {
            if (!FileExists(path))
            {
                List<string> dirList = path.Split('\\').ToList();
                dirList.RemoveAt(dirList.Count - 1);
                string dir = "";
                foreach (string s in dirList)
                {
                    dir += s + "\\";
                }

                dir = Path.Combine(AppPaths.SettingsPath, dir);

                if (!Directory.Exists(dir))
                {
                    DirectoryInfo d = Directory.CreateDirectory(dir);
                }
                File.Create(Path.Combine(AppPaths.SettingsPath, path + ".json")).Close();
                File.WriteAllText(Path.Combine(AppPaths.SettingsPath, path + ".json"), "{}");
            }
        }

        public static void InitializeValue<T>(string path, string key, T value)
        {
            if (!KeyExists(path, key))
            {
                Set(path, key, value);
            }
        }
    }
}
