using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class UninstallOptions : Page
    {
        public UninstallOptions()
        {
            InitializeComponent();
            MainWindow.HideNavBar();
        }

        private void DeleteAppDataFiles_Click(object sender, RoutedEventArgs e)
        {
            Directory.Delete(AppPaths.SettingsPath, true);
            App.Close();
        }

        private void KeepAppDataFiles_Click(object sender, RoutedEventArgs e)
        {
            App.Close();
        }
    }
}