using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VSpecialChars1Es.xaml
    /// </summary>
    public partial class VSpecialChars1Es : VKeyboardTab
    {
        private bool shifted = false;

        public VSpecialChars1Es()
        {
            InitializeComponent();
            name = "Spanish Special 1";
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

        private void ShiftKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.LEFT_SHIFT);
            shifted = !shifted;
            UpdateShift();
        }

        private void UpdateShift()
        {
            foreach (VGrid g in mainGrid.Children)
            {
                foreach (VButton b in g.Children)
                {
                    string str = b.Content.ToString();
                    if (str.Length == 1 && char.IsLetter(str[0]))
                    {
                        if (shifted)
                        {
                            b.Content = str.ToUpper();
                        }
                        else
                        {
                            b.Content = str.ToLower();
                        }
                    }
                }
            }
        }
    }
}
