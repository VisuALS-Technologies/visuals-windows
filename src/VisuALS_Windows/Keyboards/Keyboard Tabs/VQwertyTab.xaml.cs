using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VQwertyTab.xaml
    /// </summary>
    public partial class VQwertyTab : VKeyboardTab
    {
        private bool shifted = false;
        public VQwertyTab()
        {
            InitializeComponent();
            name = "English Qwerty";
        }

        private void AlphaKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            VButton b = sender as VButton;
            InputSimulator.UnicodeKeyboardKeyClick(b.Content.ToString()[0]);
            shifted = false;
            UpdateShift();
        }

        private void ShiftKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.LEFT_SHIFT);
            shifted = !shifted;
            UpdateShift();
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
