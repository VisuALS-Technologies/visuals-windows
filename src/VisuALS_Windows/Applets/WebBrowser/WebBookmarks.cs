using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    class WebBookmarks
    {
        private string path = AppletManager.GetApplet<WebBrowser>().Config.Get<string>("bookmarks_file");

        public WebBookmarks()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        public List<string> GetBookmarks()
        {
            return File.ReadAllLines(path).ToList();
        }

        public Dictionary<string, object> GetLabeledBookmarks()
        {
            List<string> bookmarks = File.ReadAllLines(path).ToList();
            List<string> labels = bookmarks.Select(x => UrlUtils.GetAnglicizedUrl(x)).ToList();
            return labels.Zip(bookmarks, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v as object);
        }

        public void AddBookmark(string url)
        {
            if (!GetBookmarks().Contains(url))
            {
                File.AppendAllLines(path, new string[] { url });
            }
        }

        public void RemoveBookmark(string url)
        {
            List<string> bookmarks = GetBookmarks();
            bookmarks.Remove(url);
            File.WriteAllLines(path, bookmarks);
        }

        public bool IsEmpty()
        {
            return GetBookmarks().Count() <= 0;
        }
    }
}
