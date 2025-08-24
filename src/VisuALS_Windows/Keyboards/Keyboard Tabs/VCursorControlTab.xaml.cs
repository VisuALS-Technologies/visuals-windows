using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VQwertyTab.xaml
    /// </summary>
    public partial class VCursorControlTab : VKeyboardTab
    {
        public VCursorControlTab()
        {
            InitializeComponent();
            name = "Cursor Control";
        }

        private void UpKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.UP_ARROW);
        }

        private void LeftKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.LEFT_ARROW);
        }

        private void DownKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.DOWN_ARROW);
        }

        private void RightKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.RIGHT_ARROW);
        }

        private void PageStartKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.HOME);
        }

        private void LineStartKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.HOME);
        }

        private void LineEndKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.END);
        }
        private void PageEndKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.END);
        }

        private void CopyKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.C);
        }

        private void PasteKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.V);
        }

        private void UndoKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.Z);
        }

        private void RedoKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.Y);
        }

        private void ClearSelectionKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClick(InputSimulator.KeyboardKeys.END);
        }

        private void SelectAllKey_Click(object sender, RoutedEventArgs e)
        {
            Refocus();
            InputSimulator.VirtualKeyboardKeyClickWithModifier(
                InputSimulator.KeyboardKeys.LEFT_CONTROL,
                InputSimulator.KeyboardKeys.A);
        }
    }
}
