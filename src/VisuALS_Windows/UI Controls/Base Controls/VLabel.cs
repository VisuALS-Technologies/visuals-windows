using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public enum LabelRole
    {
        NotSpecified,
        Text, TextBlock, Message, Prompt, Hint,
        Error,
        Title, Header, Label, Name,
        Emphasized,
        Deemphasized,
        Notification
    }

    public class VLabel : Label
    {
        public LabelRole Role
        {
            get { return (LabelRole)base.GetValue(RoleProperty); }
            set { base.SetValue(RoleProperty, value); }
        }

        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
            "Role", typeof(LabelRole), typeof(VLabel), new PropertyMetadata(default(LabelRole)));

        /// <summary>
        /// The corner radius property, used in styling
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Register the corner radius property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof(CornerRadius), typeof(VLabel), new PropertyMetadata(default(CornerRadius)));

        public VLabel() : base() { }
    }
}
