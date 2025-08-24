using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class WebBrowserSettings : AppletPage
    {
        public WebBrowserSettings()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<WebBrowser>();
            BrowserType.SelectedOption = LanguageManager.Tokens[Config.Get<string>("browser")];
        }

        private void BrowserType_OptionSelected(object sender, RoutedEventArgs e)
        {
            Config.Set("browser", LanguageManager.KeyFromToken(BrowserType.SelectedOption));
        }
    }
}
