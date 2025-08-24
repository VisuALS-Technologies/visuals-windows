using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    static class UpdateNews
    {
        public static void ShowNews()
        {
            List<string> formattedNews = GetFormattedNews();
            if (formattedNews.Count > 0)
            {
                DialogWindow.ShowPaginatedText("Changes in the latest update(s):", GetFormattedNews());
            }
        }

        public static Dictionary<Version, List<string>> GetNews()
        {
            return GetVersionsSinceLastUpdate().ToDictionary(x => x, x => GetLogLines(x));
        }

        public static List<string> GetFormattedNews()
        {
            return GetNews().Select(x => x.Key.ToString() + "\n• " + string.Join("\n• ", x.Value)).ToList();
        }

        public static List<Version> GetVersionsSinceLastUpdate()
        {
            List<Version> vs = GetAllVersionsWithNews().FindAll(x => x > App.LastRunVersion && x <= App.CurrentVersion).ToList();
            return vs;
        }

        public static List<Version> GetAllVersionsWithNews()
        {
            string[] files = Directory.GetFiles(".\\Update Management\\Update Logs\\");
            List<Version> vs = files.Select(x => new Version(x.Split('\\').Last())).ToList();
            return vs;
        }

        public static List<string> GetLogLines(Version v)
        {
            return GetLog(v).Split('\n').ToList();
        }

        public static string GetLog(Version v)
        {
            return File.ReadAllText(".\\Update Management\\Update Logs\\" + v);
        }
    }
}
