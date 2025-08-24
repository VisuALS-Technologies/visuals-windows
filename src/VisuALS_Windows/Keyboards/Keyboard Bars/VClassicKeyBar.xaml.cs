using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for DefaultKeyBar.xaml
    /// </summary>
    public partial class VClassicKeyBar : VKeyboardTab
    {
        public VClassicKeyBar()
        {
            InitializeComponent();
        }

        private void SpaceBar_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.SPACEBAR);
        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            ParentKeyboard.SwitchTab("Special 1");
            ShowMainButton();
        }

        private void CursorControl_Click(object sender, RoutedEventArgs e)
        {
            ParentKeyboard.SwitchTab("Cursor Control");
            ShowMainButton();
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            ParentKeyboard.SwitchTab("Main");
            HideMainButton();
        }

        private void ShowMainButton()
        {
            MoreButton.Visibility = Visibility.Hidden;
            CursorControlButton.Visibility = Visibility.Hidden;
            MainButton.Visibility = Visibility.Visible;
        }

        private void HideMainButton()
        {
            MoreButton.Visibility = Visibility.Visible;
            CursorControlButton.Visibility = Visibility.Visible;
            MainButton.Visibility = Visibility.Hidden;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.A);
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.DELETE);
        }
    }
}
