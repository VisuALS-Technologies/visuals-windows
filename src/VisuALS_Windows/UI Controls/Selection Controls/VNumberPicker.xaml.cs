using System;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for NumberPicker.xaml
    /// </summary>
    public partial class VNumberPicker : VSelectionControl
    {
        public double Value
        {
            get { return (double)base.GetValue(ValueProperty); }
            set
            {
                double adjusted_value;
                if (value != Max && value != Min)
                {
                    adjusted_value = Math.Round(value / Increment) * Increment;
                }
                else
                {
                    adjusted_value = value;
                }
                base.SetValue(ValueProperty, adjusted_value);
                inputBox.Text = adjusted_value.ToString() + ' ' + Unit;
                if (!string.IsNullOrWhiteSpace(Label))
                {
                    inputBox.Text = Label + "\n" + inputBox.Text;
                }
                SelectedItemString = inputBox.Text;
                SelectedItem = Value;
                RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
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
            "Increment", typeof(double), typeof(VNumberPicker), new PropertyMetadata(1.0));

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

        public string Label
        {
            get { return (string)base.GetValue(LabelProperty); }
            set { base.SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(VNumberPicker), new PropertyMetadata(default(string)));

        public string Unit
        {
            get { return (string)base.GetValue(UnitProperty); }
            set { base.SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            "Unit", typeof(string), typeof(VNumberPicker), new PropertyMetadata(default(string)));

        public Orientation Direction
        {
            get { return (Orientation)base.GetValue(DirectionProperty); }
            set { base.SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            "Direction", typeof(Orientation), typeof(VNumberPicker), new PropertyMetadata(Orientation.Vertical, DirectionChanged));

        public VNumberPicker()
        {
            InitializeComponent();
        }

        public static void DirectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VNumberPicker np = o as VNumberPicker;
            if ((Orientation)e.NewValue == Orientation.Vertical)
            {
                np.col0.Width = new GridLength(0);
                np.col2.Width = new GridLength(0);
                np.row0.Height = new GridLength(1, GridUnitType.Star);
                np.row2.Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                np.col0.Width = new GridLength(1, GridUnitType.Star);
                np.col2.Width = new GridLength(1, GridUnitType.Star);
                np.row0.Height = new GridLength(0);
                np.row2.Height = new GridLength(0);
            }
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            if ((Value + Increment) <= Max)
            {
                Value += Increment;
            }
            else
            {
                Value = Max;
            }
        }

        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            if ((Value - Increment) >= Min)
            {
                Value -= Increment;
            }
            else
            {
                Value = Min;
            }

        }
    }
}
