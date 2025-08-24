using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace VisuALS_WPF_App

{

    /// <summary>
    /// Interaction logic for Alarm.xaml
    /// </summary>

    public partial class Alarm : Page

    {
        public Alarm()
        {
            InitializeComponent();

            //If mute alarm setting is off
            if (!App.globalConfig.Get<bool>("mute_alarm"))
            {
                //If sound player is loaded
                if (App.CurrentWindow.p.IsLoadCompleted)
                {
                    //Play alarm noise
                    App.CurrentWindow.p.PlayLooping();
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            App.CurrentWindow.p.Stop();
        }

        private void TurnOffAlarm_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentWindow.p.Stop();
            NavigationService.GoBack();
        }
    }
}