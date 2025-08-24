using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class PhotoViewerSettings : AppletPage
    {
        public PhotoViewerSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<PhotoViewer>();
            NotesFolder.SelectedFile = Config.Get<string>("photos_folder");
        }

        private void PhotosFolder_FileSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("photos_folder", NotesFolder.SelectedFile);
        }
    }
}
