using System.Windows;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VColorPicker.xaml
    /// VColorPicker allows the user to select a color from a palette
    /// or create a color by changing Hue, Saturation, and Brightness levels
    /// </summary>
    public partial class VColorPicker : VSelectionControl
    {
        private bool advancedHidden = true; // true if showing palette, false if showing HSB selection
        private int increment = 10; // HSB selection increment size
        private int hue = 0; // hue of current color
        private int saturation = 100; // saturation of current color
        private int brightness = 100; // brightness of current color

        /// <summary>
        /// Default constructor for VColorPicker
        /// </summary>
        public VColorPicker()
        {
            InitializeComponent();
            SelectedItem = Colors.White;
            SelectedItemString = ColorUtils.ToString(Colors.White);
            UpdateToHSB();
        }

        public VColorPicker(Color color)
        {
            InitializeComponent();
            SelectedItem = color;
            SelectedItemString = ColorUtils.ToString(color);
            UpdateToHSB();
        }

        /// <summary>
        /// Click handler for advanced/basic color picker toggle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchToAdvanced_Click(object sender, RoutedEventArgs e)
        {
            VButton b = sender as VButton;
            advancedHidden = !advancedHidden;
            if (advancedHidden)
            {
                AdvancedPickerGrid.Visibility = Visibility.Hidden;
                BasicPickerGrid.Visibility = Visibility.Visible;
                b.Content = LanguageManager.Tokens["vf_switch_to_advanced"];
            }
            else
            {
                AdvancedPickerGrid.Visibility = Visibility.Visible;
                BasicPickerGrid.Visibility = Visibility.Hidden;
                b.Content = "Switch to Basic";
            }
        }

        /// <summary>
        /// Click handler for individual color buttons on basic selection screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicColor_Click(object sender, RoutedEventArgs e)
        {
            VButton b = sender as VButton;
            SelectedItem = (b.Background as SolidColorBrush).Color;
            SelectedItemString = ColorUtils.ToString((Color)SelectedItem);
            UpdateToHSB();
        }

        /// <summary>
        /// Updates the Selected Color Label using the hue, saturation, and brightness variables
        /// </summary>
        private void UpdateFromHSB()
        {
            SelectedItem = ColorUtils.FromHSV(hue, saturation, brightness);
            SelectedItemString = ColorUtils.ToString((Color)SelectedItem);
            HueBorder.Content = "Hue : " + hue.ToString();
            SaturationBorder.Content = "Saturation : " + saturation.ToString() + "%";
            BrightnessBorder.Content = "Brightness : " + brightness.ToString() + "%";
            SelectedColorBorder.Background = new SolidColorBrush((Color)SelectedItem);
        }

        /// <summary>
        /// Updates the hue, saturation, and birghtness variables using the color of the Selected Color Label
        /// </summary>
        private void UpdateToHSB()
        {
            int[] hsv = ((Color)SelectedItem).HSV();
            hue = hsv[0];
            saturation = hsv[1];
            brightness = hsv[2];
            SelectedColorBorder.Background = new SolidColorBrush((Color)SelectedItem);
        }

        /// <summary>
        /// Click handler for hue decrement button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueDec_Click(object sender, RoutedEventArgs e)
        {
            if (hue >= increment)
            {
                hue -= increment;
            }
            else
            {
                hue = 0;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for hue increment button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HueInc_Click(object sender, RoutedEventArgs e)
        {
            if (hue <= (360 - increment))
            {
                hue += increment;
            }
            else
            {
                hue = 360;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for saturation decrement click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaturationDec_Click(object sender, RoutedEventArgs e)
        {
            if (saturation > increment)
            {
                saturation -= increment;
            }
            else
            {
                saturation = 0;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for saturation increment button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaturationInc_Click(object sender, RoutedEventArgs e)
        {
            if (saturation < (100 - increment))
            {
                saturation += increment;
            }
            else
            {
                saturation = 100;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for brightness decrement button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrightnessDec_Click(object sender, RoutedEventArgs e)
        {
            if (brightness > increment)
            {
                brightness -= increment;
            }
            else
            {
                brightness = 0;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for brightness increment button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrightnessInc_Click(object sender, RoutedEventArgs e)
        {
            if (brightness < (100 - increment))
            {
                brightness += increment;
            }
            else
            {
                brightness = 100;
            }
            UpdateFromHSB();
        }

        /// <summary>
        /// Click handler for color submit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitColor_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
            RaiseEvent(args);
        }
    }
}
