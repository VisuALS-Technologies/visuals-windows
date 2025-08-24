using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    public class VSelectionControl : UserControl
    {
        public string SelectedItemString
        {
            get { return (string)base.GetValue(SelectedItemStringProperty); }
            set { base.SetValue(SelectedItemStringProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemStringProperty = DependencyProperty.Register(
            "SelectedItem_String", typeof(string), typeof(VSelectionControl), new PropertyMetadata(default(string)));

        public object SelectedItem
        {
            get { return (object)base.GetValue(SelectedItemProperty); }
            set { base.SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(object), typeof(VSelectionControl), new PropertyMetadata(default(object)));

        public static readonly RoutedEvent ItemSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "ItemSelectionChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(VSelectionControl));

        public event RoutedEventHandler ItemSelectionChanged
        {
            add { AddHandler(ItemSelectionChangedEvent, value); }
            remove { RemoveHandler(ItemSelectionChangedEvent, value); }
        }
    }
}
