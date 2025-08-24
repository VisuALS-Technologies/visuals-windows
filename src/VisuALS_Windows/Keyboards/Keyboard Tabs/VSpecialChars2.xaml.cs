using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VSpecialChars2.xaml
    /// </summary>
    public partial class VSpecialChars2 : VKeyboardTab
    {
        public VSpecialChars2()
        {
            InitializeComponent();
            name = "English Special 1";
        }

        private void AlphaKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            VButton b = sender as VButton;
            InputSimulator.UnicodeKeyboardKeyClick(b.Content.ToString()[0]);
        }

        private void OtherKey_Click(object sender, RoutedEventArgs e)
        {
            ParentKeyboard.SwitchTab("Special 1");
        }

        private void EnterKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.RETURN);
        }
        private void BackspaceKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.BACKSPACE);
        }
        private void EscapeKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.ESCAPE);
        }
    }
}
