using System.Windows;
using System.Windows.Controls;

// Test

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        #region Constructors
        public MainMenu()
        {
            InitializeComponent();

            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
        #endregion

        #region Button Clicks

        /// <summary>
        /// Navigate to the Settings Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new SettingsMenu());
        }

        /// <summary>
        /// Navigate to the Alarm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Alarm_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Alarm());
        }

        private void AppletBtn_Click(object sender, RoutedEventArgs e)
        {
            VButton b = sender as VButton;
            AppletManager.GetApplet((string)b.Content).Run();
        }
        #endregion

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AppletMenu());
        }
    }
}
