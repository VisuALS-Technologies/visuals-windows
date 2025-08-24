using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    public static class FileUtils
    {
        public static string getInvalidPathChars(string path)
        {
            string invchars = "";
            if (path.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
            {
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    if (path.Contains(c))
                    {
                        invchars += " " + c;
                    }
                }
            }
            return invchars;
        }

        public static bool isVisible(string path)
        {
            char prefixChar = path.Split('\\').Last()[0];

            if (prefixChar == '.' || prefixChar == '~')
            {
                return false;
            }
            else if (File.Exists(path))
            {
                return !File.GetAttributes(path).HasFlag(FileAttributes.Hidden);
            }
            else if (System.IO.Directory.Exists(path))
            {
                return !(new DirectoryInfo(path)).Attributes.HasFlag(FileAttributes.Hidden);
            }
            else
            {
                return false;
            }
        }
    }
}
