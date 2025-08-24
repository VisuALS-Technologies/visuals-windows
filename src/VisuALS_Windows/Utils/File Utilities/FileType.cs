using System.Collections.Generic;
using System.Linq;

namespace VisuALS_WPF_App
{
    public class FileType
    {
        public string name;
        public string icon;
        public string[] extensions;

        public FileType(string _name, string _icon, string[] _extensions)
        {
            name = _name;
            icon = _icon;
            extensions = _extensions;
        }

        public FileType(string extension)
        {
            name = "";
            icon = defaultIcon;
            extensions = new string[] { extension };
        }

        static public string defaultIcon = "⬜";
        static public string folderIcon = "📁";
        static public List<FileType> knownFileTypes = new List<FileType>()
        {
            new FileType("image", "🖼", new string[]{ ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".gif", ".heic" }),
            new FileType("text", "📝", new string[]{ ".txt", ".json" }),
            new FileType("web_document", "📄", new string[]{ ".pdf", ".html" }),
            new FileType("audio", "🎵", new string[]{ ".wav", ".mp3" }),
            new FileType("video", "🎞", new string[]{ ".mp4", ".mov" }),
            new FileType("archive", "🗃", new string[]{ ".zip", ".rar", ".tar" })
        };

        static public FileType GetFileType(string path)
        {
            string extension = System.IO.Path.GetExtension(path).ToLower();
            FileType[] icons = knownFileTypes.Where(x => x.extensions.Contains(extension)).ToArray();
            if (icons.Length > 0)
            {
                return icons[0];
            }
            else
            {
                return new FileType(extension);
            }
        }
    };
}
