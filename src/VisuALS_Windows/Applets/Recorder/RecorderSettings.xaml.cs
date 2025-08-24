using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class RecorderSettings : AppletPage
    {
        public RecorderSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<Recorder>();
            RecordingsFolder.SelectedFile = Config.Get<string>("recordings_folder");
        }

        private void RecordingsFolder_FileSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("recordings_folder", RecordingsFolder.SelectedFile);
        }
    }
}
