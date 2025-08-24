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

            //Setup Size Options
            EyeBallSizeComboBox.SetItems(new object[] { 10.0, 20.0, 30.0, 40.0, 50.0, 60.0 });
            EyeBallBorderComboBox.SetItems(new object[] { 0.0, 2.0, 4.0, 6.0, 8.0, 10.0 });

            //Initialize control values
            DwellTimeNumberPicker.Value = (int)Application.Current.Resources["DwellTime"] / 1000.0;
            EyeBallToggle.Value = EyeBall.IsEnabled;
            EyeBallBorderColorSelect.SelectedColor = EyeBall.LineColor.Color;
            EyeBallColorSelect.SelectedColor = EyeBall.BallColor.Color;
            EyeBallBorderComboBox.SelectedItem = EyeBall.LineSize;
            EyeBallSizeComboBox.SelectedItem = EyeBall.BallSize;
            DwellIndicatorToggle.Value = VButton.DwellIndicator;
        }

        private void DwellTime_NumberSelected(object sender, RoutedEventArgs e)
        {
            App.globalConfig.Set("dwell_time", DwellTimeNumberPicker.Value * 1000);
            Application.Current.Resources["DwellTime"] = Convert.ToInt32(DwellTimeNumberPicker.Value * 1000);
            VButton.UpdateDwellIndicatorAnimation();
        }

        private void EyeBallSize_OptionSelected(object sender, RoutedEventArgs e)
        {
            double size = Convert.ToDouble(EyeBallSizeComboBox.SelectedItem);
            App.globalConfig.Set("eyeball_size", size);
            EyeBall.BallSize = size;
        }

        private void EyeBallBorder_OptionSelected(object sender, RoutedEventArgs e)
        {
            double size = Convert.ToDouble(EyeBallBorderComboBox.SelectedItem);
            App.globalConfig.Set("eyeball_border_size", size);
            EyeBall.LineSize = size;
        }

        private void EyeBallColor_ColorSelected(object sender, RoutedEventArgs e)
        {
            string color = EyeBallColorSelect.SelectedColorString;
            App.globalConfig.Set("eyeball_color", color);
            EyeBall.BallColor = new SolidColorBrush(EyeBallColorSelect.SelectedColor);
        }

        private void EyeBallBorderColor_ColorSelected(object sender, RoutedEventArgs e)
        {
            string color = EyeBallBorderColorSelect.SelectedColorString;
            App.globalConfig.Set("eyeball_border_color", color);
            EyeBall.LineColor = new SolidColorBrush(EyeBallBorderColorSelect.SelectedColor);
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
    }
}
