using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public enum TextInputRole
    {
        SingleLine,
        TextBlock,
        Code
    }

    public class VTextInput : TextBox
    {
        public TextInputRole Role
        {
            get { return (TextInputRole)base.GetValue(RoleProperty); }
            set { base.SetValue(RoleProperty, value); }
        }

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(TextInputRole), typeof(VTextInput), new PropertyMetadata(default(TextInputRole)));

        public void WriteLine(string str)
        {
            Text += str + '\n';
        }

        public void WriteString(string str)
        {
            Text += str;
        }

        public void WriteChar(char c)
        {
            Text += c;
        }

        public VTextInput() : base() { }
    }
}
