using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for DisplaySettings.xaml
    /// </summary>
    public partial class DisplaySettings : Page
    {
        public SettingsFile WebBrowserConfigFile;

        public DisplaySettings()
        {
            InitializeComponent();
            FullscreenToggle.Value = App.globalConfig.Get<bool>("fullscreen");
            WebBrowserConfigFile = AppletManager.GetApplet<WebBrowser>().Config;
            BrowserToggle.SelectedOption = LanguageManager.Tokens[WebBrowserConfigFile.Get<string>("browser")];
            ThemeComboBox.SetItems(StyleControl.GetAllThemeNames());
            ThemeComboBox.SelectedItemName = LanguageManager.Tokens[App.globalConfig.Get<string>("theme")];
            AccentColorSelect.SelectedColor = ColorUtils.FromString(App.globalConfig.Get<string>("accent_color"));
        }

        private void FontSizeUp_Click(object sender, RoutedEventArgs e)
        {
            StyleControl.IncreaseFontSize();
            App.globalConfig.Set("fontsizes", StyleControl.GetFontSizes());
        }

        private void FontSizeDown_Click(object sender, RoutedEventArgs e)
        {
            StyleControl.DecreaseFontSize();
            App.globalConfig.Set("fontsizes", StyleControl.GetFontSizes());
        }

        private void Fullscreen_OptionSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("fullscreen", FullscreenToggle.Value);
            MainWindow.Fullscreen(FullscreenToggle.Value);
        }

        private void BrowserToggle_OptionSelected(object sender, RoutedEventArgs e)
        {
            WebBrowserConfigFile.Set("browser", LanguageManager.KeyFromToken(BrowserToggle.SelectedOption));
        }

        private void ThemeComboBox_ItemSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("theme", LanguageManager.KeyFromToken(ThemeComboBox.SelectedItemName));
            StyleControl.SetTheme(LanguageManager.KeyFromToken(ThemeComboBox.SelectedItemName));
        }

        private void AccentColorSelect_ColorSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("accent_color", ColorUtils.ToString(AccentColorSelect.SelectedColor));
            StyleControl.SetAccentColor(AccentColorSelect.SelectedColor);
        }
    }
}
