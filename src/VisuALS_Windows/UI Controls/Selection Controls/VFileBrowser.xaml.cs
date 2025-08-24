using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VFileBrowser.xaml
    /// </summary>
    public partial class VFileBrowser : VSelectionControl
    {
        public bool HideSubmit
        {
            get
            {
                return Submit.Visibility != Visibility.Visible;
            }

            set
            {
                if (value)
                {
                    Submit.Visibility = Visibility.Collapsed;
                    Grid.SetColumnSpan(Back, 2);
                    Grid.SetColumn(OpenFolder, 2);
                }
                else
                {
                    Submit.Visibility = Visibility.Visible;
                    Grid.SetColumnSpan(Back, 1);
                    Grid.SetColumn(OpenFolder, 1);
                }
            }
        }

        public bool SelectDirectory
        {
            get { return (bool)base.GetValue(SelectDirectoryProperty); }
            set { base.SetValue(SelectDirectoryProperty, value); }
        }

        public static readonly DependencyProperty SelectDirectoryProperty = DependencyProperty.Register(
            "SelectDirectory", typeof(bool), typeof(VFileBrowser), new PropertyMetadata(false));

        public string Directory
        {
            get { return DirList.Directory; }
            set { DirList.Directory = value; }
        }

        public VFileBrowser()
        {
            InitializeComponent();
            CurrentPath.Content = DirList.Directory;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (SelectDirectory)
            {
                SelectedItemString = DirList.Directory;
                SelectedItem = DirList.Directory;
            }
            else
            {
                SelectedItemString = DirList.SelectedItemString;
                SelectedItem = DirList.SelectedItem;
            }
            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
            RaiseEvent(args);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DirList.page = 0;
                DirList.ClearSelection();
                DirList.Directory = System.IO.Directory.GetParent(DirList.Directory).FullName;
                CurrentPath.Content = DirList.Directory;
            }
            catch { }
        }

        private async void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists((string)DirList.SelectedItem))
            {
                if (FilePermissions.HasAccess((string)DirList.SelectedItem, FileSystemRights.ListDirectory))
                {
                    DirList.page = 0;
                    DirList.Directory = (string)DirList.SelectedItem;
                    DirList.ClearSelection();
                    CurrentPath.Content = DirList.Directory;
                }
                else
                {
                    await DialogWindow.ShowMessage(LanguageManager.Tokens["fb_unauthorized_access"]);
                }
            }
        }

        public void Refresh()
        {
            DirList.UpdateList();
        }
    }
}
