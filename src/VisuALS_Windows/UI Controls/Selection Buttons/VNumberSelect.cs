using System;
using System.Windows;

namespace VisuALS_WPF_App
{
    class VNumberSelect : VButton
    {
        public double Value
        {
            get { return (double)base.GetValue(ValueProperty); }
            set
            {
                base.SetValue(ValueProperty, value);
                RoutedEventArgs args = new RoutedEventArgs(VNumberPicker.ItemSelectionChangedEvent);
                RaiseEvent(args);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(VNumberPicker), new PropertyMetadata(default(double)));

        public double Increment
        {
            get { return (double)base.GetValue(IncrementProperty); }
            set { base.SetValue(IncrementProperty, value); }
        }

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register(
            "Increment", typeof(double), typeof(VNumberPicker), new PropertyMetadata(default(double)));

        public double Max
        {
            get { return (double)base.GetValue(MaxProperty); }
            set { base.SetValue(MaxProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", typeof(double), typeof(VNumberPicker), new PropertyMetadata(double.PositiveInfinity));

        public double Min
        {
            get { return (double)base.GetValue(MinProperty); }
            set { base.SetValue(MinProperty, value); }
        }

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
            "Min", typeof(double), typeof(VNumberPicker), new PropertyMetadata(double.NegativeInfinity));

        public static readonly RoutedEvent NumberSelectedEvent = EventManager.RegisterRoutedEvent(
            "NumberSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VNumberPicker));

        public event RoutedEventHandler NumberSelected
        {
            add { AddHandler(NumberSelectedEvent, value); }
            remove { RemoveHandler(NumberSelectedEvent, value); }
        }

        public VNumberSelect()
        {
            Click += Button_Click;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //VNumberPicker np = new VNumberPicker();
            //np.Min = Min;
            //np.Max = Max;
            //np.Value = Value;
            //np.Increment = Increment;
            //DialogResponse r = await DialogWindow.Show(np);
            //Value = (double)r.ResponseObject;
        }
    }
}
