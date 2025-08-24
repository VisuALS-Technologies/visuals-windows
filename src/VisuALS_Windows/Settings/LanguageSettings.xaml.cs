using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class LanguageSettings : Page
    {
        public LanguageSettings()
        {
            InitializeComponent();
            LanguageComboBox.SetItems(LanguageManager.SupportedLanguages);
            KeyboardComboBox.SetItems(KeyboardManager.GetAllLayoutNames());
            LanguageComboBox.SelectedItemName = App.globalConfig.Get<string>("language");
            KeyboardComboBox.SelectedItemName = LanguageManager.Tokens[App.globalConfig.Get<string>("keyboard")];
        }

        private void KeyboardSelected_Click(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("keyboard", LanguageManager.KeyFromToken(KeyboardComboBox.SelectedItemName));
            KeyboardManager.CurrentLayoutName = LanguageManager.KeyFromToken(KeyboardComboBox.SelectedItemName);
        }

        private void LanguageSelected_Click(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("language", LanguageComboBox.SelectedItem);
            if (IsLoaded && LanguageManager.CurrentLanguage != LanguageComboBox.SelectedItemName)
            {
                LanguageManager.CurrentLanguage = LanguageComboBox.SelectedItemName;
                MainWindow.HardRefresh();
            }
        }
    }
}
