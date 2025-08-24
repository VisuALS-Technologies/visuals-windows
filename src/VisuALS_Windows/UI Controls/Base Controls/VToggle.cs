using System.Windows;

namespace VisuALS_WPF_App
{
    public class VToggle : VButton
    {
        public bool ColorOptions
        {
            get { return (bool)base.GetValue(ColorOptionsProperty); }
            set { base.SetValue(ColorOptionsProperty, value); }
        }

        public static readonly DependencyProperty ColorOptionsProperty = DependencyProperty.Register(
            "ColorOptions", typeof(bool), typeof(VToggle), new PropertyMetadata(false));

        public ButtonRole TrueOptionRole
        {
            get { return (ButtonRole)base.GetValue(TrueOptionRoleProperty); }
            set { base.SetValue(TrueOptionRoleProperty, value); }
        }

        public static readonly DependencyProperty TrueOptionRoleProperty = DependencyProperty.Register(
            "TrueOptionRole", typeof(ButtonRole), typeof(VToggle), new PropertyMetadata(ButtonRole.Yes));

        public ButtonRole FalseOptionRole
        {
            get { return (ButtonRole)base.GetValue(FalseOptionRoleProperty); }
            set { base.SetValue(FalseOptionRoleProperty, value); }
        }

        public static readonly DependencyProperty FalseOptionRoleProperty = DependencyProperty.Register(
            "FalseOptionRole", typeof(ButtonRole), typeof(VToggle), new PropertyMetadata(ButtonRole.No));

        public string TrueOption
        {
            get { return (string)base.GetValue(TrueOptionProperty); }
            set { base.SetValue(TrueOptionProperty, value); }
        }

        public static readonly DependencyProperty TrueOptionProperty = DependencyProperty.Register(
            "TrueOption", typeof(string), typeof(VToggle), new PropertyMetadata(default(string)));

        public string FalseOption
        {
            get { return (string)base.GetValue(FalseOptionProperty); }
            set { base.SetValue(FalseOptionProperty, value); }
        }

        public static readonly DependencyProperty FalseOptionProperty = DependencyProperty.Register(
            "FalseOption", typeof(string), typeof(VToggle), new PropertyMetadata(default(string)));

        public string SelectedOption
        {
            get { return (string)base.GetValue(SelectedOptionProperty); }
            set { base.SetValue(SelectedOptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedOptionProperty = DependencyProperty.Register(
            "SelectedOption", typeof(string), typeof(VToggle), new PropertyMetadata(default(string), SelectedOptionChanged));

        public bool Value
        {
            get { return (bool)base.GetValue(ValueProperty); }
            set { base.SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(bool), typeof(VToggle), new PropertyMetadata(default(bool), ValueChanged));

        public static readonly RoutedEvent OptionSelectedEvent = EventManager.RegisterRoutedEvent(
            "OptionSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VToggle));

        public event RoutedEventHandler OptionSelected
        {
            add { AddHandler(OptionSelectedEvent, value); }
            remove { RemoveHandler(OptionSelectedEvent, value); }
        }
        public string Prefix
        {
            get { return (string)base.GetValue(PrefixProperty); }
            set { base.SetValue(PrefixProperty, value); }
        }

        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
            "Prefix", typeof(string), typeof(VToggle), new PropertyMetadata(default(string)));

        public VToggle() : base()
        {
            Prefix = "";
            Click += VToggle_Click;
            Loaded += VToggle_Loaded;
        }

        private void VToggle_Loaded(object sender, RoutedEventArgs e)
        {
            if (Value)
            {
                SelectedOption = TrueOption;
            }
            else
            {
                SelectedOption = FalseOption;
            }
            UpdateContent();
            RoutedEventArgs args = new RoutedEventArgs(VToggle.OptionSelectedEvent);
            RaiseEvent(args);
        }

        private static void ValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VToggle tg = o as VToggle;
            if ((bool)e.NewValue && tg.SelectedOption != tg.TrueOption)
            {
                tg.SelectedOption = tg.TrueOption;
            }
            else if (!(bool)e.NewValue && tg.SelectedOption != tg.FalseOption)
            {
                tg.SelectedOption = tg.FalseOption;
            }
            tg.UpdateContent();
            // We need to wait until the VToggle is loaded to trigger the event
            // in case the event handlers deal with the UI
            if (tg.IsLoaded)
            {
                RoutedEventArgs args = new RoutedEventArgs(VToggle.OptionSelectedEvent);
                tg.RaiseEvent(args);
            }
        }

        private static void SelectedOptionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            VToggle tg = o as VToggle;
            tg.UpdateContent();
            if ((string)e.NewValue == tg.TrueOption && tg.Value != true)
            {
                tg.Value = true;
            }
            else if ((string)e.NewValue == tg.FalseOption && tg.Value != false)
            {
                tg.Value = false;
            }
            // We need to wait until the VToggle is loaded to trigger the event
            // in case the event handlers deal with the UI
            if (tg.IsLoaded)
            {
                RoutedEventArgs args = new RoutedEventArgs(VToggle.OptionSelectedEvent);
                tg.RaiseEvent(args);
            }
        }

        public void UpdateContent()
        {
            Content = Prefix + SelectedOption;
            if (ColorOptions)
            {
                if (Value)
                {
                    Role = TrueOptionRole;
                }
                else
                {
                    Role = FalseOptionRole;
                }
            }
        }

        private void VToggle_Click(object sender, RoutedEventArgs e)
        {
            Value = !Value;
        }
    }
}
