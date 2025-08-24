using System.Windows;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// A single button that shows a color picker dialog and changes its color to match the selected color
    /// It has similar properties and events to the color picker itself for retrieving the selected color
    /// </summary>
    class VColorSelect : VButton
    {
        /// <summary>
        /// The string representation of the color
        /// This property is set when the selected color property is set
        /// </summary>
        public string SelectedColorString
        {
            get { return (string)base.GetValue(SelectedColorStringProperty); }
            set { base.SetValue(SelectedColorStringProperty, value); }
        }

        /// <summary>
        /// Register selected color string so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty SelectedColorStringProperty = DependencyProperty.Register(
            "SelectedColorString", typeof(string), typeof(VColorSelect), new PropertyMetadata(default(string)));

        /// <summary>
        /// The currently selected color
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)base.GetValue(SelectedColorProperty); }
            set
            {
                base.SetValue(SelectedColorProperty, value);
                Background = new SolidColorBrush(value);
            }
        }

        /// <summary>
        /// Register selected color so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor", typeof(Color), typeof(VColorSelect), new PropertyMetadata(default(Color)));

        /// <summary>
        /// This event is raised when the user chooses a color from the dialog
        /// </summary>
        public static readonly RoutedEvent ColorSelectedEvent = EventManager.RegisterRoutedEvent(
            "ColorSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VColorSelect));

        /// <summary>
        /// The handler add/remove logic for the Color Selected Event
        /// </summary>
        public event RoutedEventHandler ColorSelected
        {
            add { AddHandler(ColorSelectedEvent, value); }
            remove { RemoveHandler(ColorSelectedEvent, value); }
        }

        /// <summary>
        /// Default constructor for VColorSelect
        /// </summary>
        public VColorSelect() : base()
        {
            Click += ColorSelect_Click;
        }

        /// <summary>
        /// Click handler for the color select button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.Show(new VColorPicker(SelectedColor));
            if (r.ResponseObject != null)
            {
                SelectedColor = (Color)r.ResponseObject;
                SelectedColorString = r.ResponseString;
                RoutedEventArgs args = new RoutedEventArgs(VColorSelect.ColorSelectedEvent);
                RaiseEvent(args);
            }
        }
    }
}
