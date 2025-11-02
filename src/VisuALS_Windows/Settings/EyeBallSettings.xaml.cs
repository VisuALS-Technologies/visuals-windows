using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for EyeTrackingSettings.xaml
    /// </summary>
    public partial class EyeBallSettings : Page
    {
        public EyeBallSettings()
        {
            InitializeComponent();

            //Setup Size Options
            EyeBallSizeComboBox.SetItems(new object[] { 10.0, 20.0, 30.0, 40.0, 50.0, 60.0 });
            EyeBallBorderComboBox.SetItems(new object[] { 0.0, 2.0, 4.0, 6.0, 8.0, 10.0 });

            //Initialize control values
            EyeBallBorderColorSelect.SelectedColor = EyeBall.LineColor.Color;
            EyeBallColorSelect.SelectedColor = EyeBall.BallColor.Color;
            EyeBallBorderComboBox.SelectedItem = EyeBall.LineSize;
            EyeBallSizeComboBox.SelectedItem = EyeBall.BallSize;
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
    }
}
