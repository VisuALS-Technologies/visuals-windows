using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VBasicKeyBar.xaml
    /// </summary>
    public partial class VBasicKeyBar : VKeyboardTab
    {
        public VBasicKeyBar()
        {
            InitializeComponent();
        }

        private void SpaceBar_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.SPACEBAR);
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            ParentKeyboard.SwitchTab("Menu");
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
