using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for CarlsPause.xaml
    /// </summary>
    public partial class CarlsPause : AppletPage
    {
        public CarlsPause()
        {
            InitializeComponent();
            MainWindow.HideNavBar();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ShowNavBar();
            MainWindow.GoBack();
        }
    }
}
