using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    class VSeparator : Border
    {
        /// <summary>
        /// The separator's orientation property
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)base.GetValue(OrientationProperty); }
            set { base.SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Register the orientation property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(VSeparator), new PropertyMetadata(Orientation.Vertical));
    }
}
