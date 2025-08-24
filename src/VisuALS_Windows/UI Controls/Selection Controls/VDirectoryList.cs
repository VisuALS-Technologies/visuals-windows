using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;

namespace VisuALS_WPF_App
{
    struct FileIcon
    {
        public string icon;
        public string[] extensions;

        public FileIcon(string _icon, string[] _extensions)
        {
            icon = _icon;
            extensions = _extensions;
        }
    };

    class VDirectoryList : VList
    {
        public string Directory
        {
            get { return (string)base.GetValue(DirectoryProperty); }
            set { base.SetValue(DirectoryProperty, value); }
        }

        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.Register(
            "Directory", typeof(string), typeof(VList), new PropertyMetadata("C:\\", DirectoryChanged));

        private List<Predicate<string>> Filters = new List<Predicate<string>>() { IsVisibleFilter };

        public VDirectoryList() : base()
        {
            InitializeValues();
        }

        public VDirectoryList(int rows, int cols) : base(rows, cols)
        {
            InitializeValues();
        }

        public void InitializeValues()
        {
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SelectedItem = null;
            SelectedItemString = null;
        }

        private static void DirectoryChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((VDirectoryList)o).UpdateList();
        }

        public static bool HavePermissionFilter(string path)
        {
            return FilePermissions.HasAccess(path, FileSystemRights.FullControl);
        }

        public static bool IsVisibleFilter(string path)
        {
            return FileUtils.isVisible(path);
        }

        private bool testAllFilters(string path)
        {
            foreach (Predicate<string> filter in Filters)
            {
                if (!filter(path))
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdateList()
        {
            Dictionary<string, object> dir_contents = System.IO.Directory.GetDirectories(Directory).ToDictionary(x => FileType.folderIcon + " " + x.Split('\\').Last(), y => (object)y);
            dir_contents = dir_contents.Concat(System.IO.Directory.GetFiles(Directory).ToDictionary(x => FileType.GetFileType(x).icon + " " + System.IO.Path.GetFileName(x), y => (object)y)).ToDictionary(x => x.Key, y => y.Value);
            dir_contents = dir_contents.Where(x => testAllFilters((string)x.Value)).ToDictionary(x => x.Key, y => y.Value);
            SetItems(dir_contents);
        }
    }
}
