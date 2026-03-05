using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class CameraSettings : AppletPage
    {
        public CameraSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Camera>();
            PhotosFolder.SelectedFile = Config.Get<string>("photos_folder");
        }

        private void PhotosFolder_FileSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("photos_folder", PhotosFolder.SelectedFile);
        }
    }
}
