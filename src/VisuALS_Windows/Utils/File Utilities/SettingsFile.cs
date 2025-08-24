using System.Collections.Generic;

namespace VisuALS_WPF_App
{
    public class SettingsFile
    {
        string RelativePath;

        public SettingsFile(string path)
        {
            RelativePath = path;
            SettingsManager.CreateFile(path);
        }

        public T Get<T>(string key)
        {
            return SettingsManager.Get<T>(RelativePath, key);
        }

        public void Set<T>(string key, T value)
        {
            SettingsManager.Set(RelativePath, key, value);
        }

        public void RemoveKey(string key)
        {
            SettingsManager.RemoveKey(RelativePath, key);
        }

        public Dictionary<string, object> AsDictionary()
        {
            return SettingsManager.Get(RelativePath);
        }

        public void Initialize<T>(string key, T value)
        {
            SettingsManager.InitializeValue(RelativePath, key, value);
        }
    }
}
