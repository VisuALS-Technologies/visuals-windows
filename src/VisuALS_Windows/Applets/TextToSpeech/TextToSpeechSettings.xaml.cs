using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class TextToSpeechSettings : AppletPage
    {
        public TextToSpeechSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<TextToSpeech>();
            PhrasesFolder.SelectedFile = Config.Get<string>("phrases_folder");
            if (App.globalConfig.Get<bool>("compatibility_mode"))
            {
                PhrasesFolder.IsEnabled = false;
            }
        }

        private void PhrasesFolder_FileSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("phrases_folder", PhrasesFolder.SelectedFile);
        }
    }
}
