using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for EyeTrackingSettings.xaml
    /// </summary>
    public partial class EyeTrackingSettings : Page
    {
        public EyeTrackingSettings()
        {
            InitializeComponent();

            //Set eye tracker devices list

            EyeTrackerCombo.SetItems(GazeTrackerManager.NamesAndGazeTrackersDictionary());
            EyeTrackerCombo.AddItem("None", "None");

            //Initialize control values
            DwellTimeNumberPicker.Value = (int)Application.Current.Resources["DwellTime"] / 1000.0;
            EyeBallToggle.Value = EyeBall.IsEnabled;
            DwellIndicatorToggle.Value = VButton.DwellIndicator;
        }

        private void DwellTime_NumberSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("dwell_time", DwellTimeNumberPicker.Value * 1000);
            Application.Current.Resources["DwellTime"] = Convert.ToInt32(DwellTimeNumberPicker.Value * 1000);
            VButton.UpdateDwellIndicatorAnimation();
        }

        private void EyeBallToggle_OptionSelected(object sender, RoutedEventArgs e)
        {
            bool enabled = EyeBallToggle.Value;
            App.globalConfig.Set("eyeball_enabled", enabled);
            EyeBall.IsEnabled = enabled;
        }

        private void DwellIndicatorToggle_OptionSelected(object sender, RoutedEventArgs e)
        {
            VButton.DwellIndicator = DwellIndicatorToggle.Value;

            string dwellind = "di_none";
            if (DwellIndicatorToggle.Value)
            {
                dwellind = "di_pie";
            }

            App.globalConfig.Set("dwell_indicator", dwellind);
        }

        private void EyeBallCustomization_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EyeBallSettings());
        }

        private void EyeTracker_ItemSelected(object sender, RoutedEventArgs e)
        {
            if (EyeTrackerCombo.SelectedItemName == "None")
            {
                GazeTrackerManager.SelectedGazeTracker = null;
            }
            else
            {
                GazeTrackerManager.SelectedGazeTracker = (IGazeTracker)EyeTrackerCombo.SelectedItem;
            }
        }
    }
}
