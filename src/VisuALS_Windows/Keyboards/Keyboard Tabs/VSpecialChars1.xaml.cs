using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VSpecialChars1.xaml
    /// </summary>
    public partial class VSpecialChars1 : VKeyboardTab
    {
        public VSpecialChars1()
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
            ParentKeyboard.SwitchTab("Special 2");
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
    }
}
