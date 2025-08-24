using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for AudioSettings.xaml
    /// </summary>
    public partial class ManageApplets : Page
    {
        public ManageApplets()
        {
            InitializeComponent();
            UpdateAppletList();
            AppletDescription.Text = LanguageManager.Tokens["ap_select_for_details"];
            EnableApplet.Visibility = Visibility.Hidden;
            AppletSettings.Visibility = Visibility.Hidden;
        }

        private string GetAppletLabel(Applet applet)
        {
            string label = "";
            if (AppletManager.IsActive(applet))
                label += "(" + LanguageManager.Tokens["ap_enabled"] + ")\n";
            label += LanguageManager.Tokens[applet.Name];
            return label;
        }

        private void UpdateAppletList()
        {
            AppletsList.SetItems(AppletManager.ActiveApplets.Concat(AppletManager.InactiveApplets), GetAppletLabel);
        }

        private void UpdateDetails()
        {
            EnableApplet.Visibility = Visibility.Visible;
            if ((AppletsList.SelectedItem as Applet).CreateSettingsPage() == null)
            {
                AppletSettings.Visibility = Visibility.Hidden;
            }
            else
            {
                AppletSettings.Visibility = Visibility.Visible;
            }
            Applet applet = AppletsList.SelectedItem as Applet;
            AppletName.Content = LanguageManager.Tokens[applet.Name];
            AppletDescription.Text = LanguageManager.Tokens[applet.Description];
            EnableApplet.Value = !AppletManager.IsActive(applet);
        }

        private void AppletsList_ItemSelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateDetails();
        }

        private void Enable_OptionSelected(object sender, RoutedEventArgs e)
        {
            if (AppletsList.SelectedItem != null)
            {
                Applet applet = AppletsList.SelectedItem as Applet;
                if (!EnableApplet.Value)
                {
                    AppletManager.ActivateApplet(applet);
                }
                else
                {
                    AppletManager.DeactivateApplet(applet);
                }
            }
            UpdateAppletList();
        }

        private void AppletSettings_Click(object sender, RoutedEventArgs e)
        {
            Applet applet = AppletsList.SelectedItem as Applet;
            MainWindow.ShowPage(applet.CreateSettingsPage());
        }
    }
}
