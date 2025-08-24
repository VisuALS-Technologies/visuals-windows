using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VTextInputDialog.xaml
    /// </summary>
    public partial class VTextInputDialog : VSelectionControl
    {
        public string Prompt
        {
            get { return (string)base.GetValue(PromptProperty); }
            set
            {
                base.SetValue(PromptProperty, value);
                promptLabel.Content = value;
            }
        }

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(VTextInputDialog), new PropertyMetadata(default(string)));

        public VTextInputDialog()
        {
            InitializeComponent();
            keyboard.Layout = KeyboardManager.GetCurrentLayout();
            keyboard.FocusElement = textBox;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = textBox.Text;
            SelectedItemString = textBox.Text;
            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
            RaiseEvent(args);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.Text == "" || textBox.Text == null)
            {
                promptLabel.Visibility = Visibility.Visible;
            }
            else
            {
                promptLabel.Visibility = Visibility.Hidden;
            }
        }
    }
}
