using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class SettingsMenu : Page
    {
        public SettingsMenu(bool deleteBackEntry = false)
        {
            InitializeComponent();

            if (!App.IsDebug)
            {
                Version.Content = App.ExecutableName + ": " + App.CurrentVersion;
            }
            else
            {
                Version.Content = LanguageManager.Tokens["st_debug"];
            }

            Application.Current.MainWindow.WindowState = WindowState.Normal;

            if (deleteBackEntry)
            {
                Loaded += SettingsMenu_Loaded;
            }
        }

        private void SettingsMenu_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowMessage(LanguageManager.Tokens["st_exit_prompt"], LanguageManager.Tokens["yes"], LanguageManager.Tokens["no"]);
            if (r.ResponseString == LanguageManager.Tokens["yes"])
            {
                App.Close();
            }
        }

        private void DisplaySettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DisplaySettings());
        }

        private void AudioSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AudioSettings());
        }

        private void EyeTrackingSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EyeTrackingSettings());
        }

        private void LanguageSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LanguageSettings());
        }

        private void ManageApplets_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageApplets());
        }
    }
}