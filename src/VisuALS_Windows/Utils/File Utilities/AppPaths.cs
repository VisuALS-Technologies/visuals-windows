using System;
using System.IO;

namespace VisuALS_WPF_App
{
    static class AppPaths
    {
        public static string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VisuALS");
        public static string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisuALS");
        public static string PicturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public static string VideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        public static string AudioPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public static string LogsPath = Path.Combine(DocumentsPath, "logs");

        public static void Initialize()
        {
            Directory.CreateDirectory(SettingsPath);
            Directory.CreateDirectory(DocumentsPath);
            Directory.CreateDirectory(PicturesPath);
            Directory.CreateDirectory(VideosPath);
            Directory.CreateDirectory(AudioPath);
            Directory.CreateDirectory(LogsPath);
        }
    }
}
