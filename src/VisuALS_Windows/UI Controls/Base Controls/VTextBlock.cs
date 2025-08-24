using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public enum TextBlockRole
    {
        Text,
        Code,
        Compact,
        Quote,
        Message,
        Notification
    }

    public class VTextBlock : TextBlock
    {
        public TextBlockRole Role
        {
            get { return (TextBlockRole)base.GetValue(RoleProperty); }
            set { base.SetValue(RoleProperty, value); }
        }

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(TextBlockRole), typeof(VTextBlock), new PropertyMetadata(default(TextBlockRole)));

        public VTextBlock() : base() { }
    }
}
