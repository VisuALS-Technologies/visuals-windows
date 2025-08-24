using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VisuALS_WPF_App
{
    class VOptionBox : VButton
    {
        public string SelectedOption
        {
            get { return (string)base.GetValue(SelectedOptionProperty); }
            set
            {
                base.SetValue(SelectedOptionProperty, value);
                Content = Prefix + value;
                RoutedEventArgs args = new RoutedEventArgs(VComboBox.ItemSelectedEvent);
                RaiseEvent(args);
            }
        }

        public static readonly DependencyProperty SelectedOptionProperty = DependencyProperty.Register(
            "SelectedOption", typeof(string), typeof(VOptionBox), new PropertyMetadata(default(string)));

        public static readonly RoutedEvent OptionSelectedEvent = EventManager.RegisterRoutedEvent(
            "OptionSelected", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VOptionBox));

        public event RoutedEventHandler OptionSelected
        {
            add { AddHandler(OptionSelectedEvent, value); }
            remove { RemoveHandler(OptionSelectedEvent, value); }
        }

        public string Prompt
        {
            get { return (string)base.GetValue(PromptProperty); }
            set { base.SetValue(PromptProperty, value); }
        }

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(
            "Prompt", typeof(string), typeof(VOptionBox), new PropertyMetadata(default(string)));

        public string Prefix
        {
            get { return (string)base.GetValue(PrefixProperty); }
            set { base.SetValue(PrefixProperty, value); }
        }

        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register(
            "Prefix", typeof(string), typeof(VOptionBox), new PropertyMetadata(default(string)));

        public List<string> Options = new List<string>();


        public VOptionBox() : base()
        {
            Click += Button_Click;
            Prefix = "";
            Loaded += Button_Loaded;
        }

        public VOptionBox(string prompt, params string[] options) : base()
        {
            Prompt = prompt;
            Options = options.ToList();
            Click += Button_Click;
            Prefix = "";
            Loaded += Button_Loaded;
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedOption == null)
            {
                if (Options.Count > 0)
                {
                    SelectedOption = Options[0];
                }
                else
                {
                    SelectedOption = "";
                }
            }
            else
            {
                SelectedOption = SelectedOption;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse r = await DialogWindow.ShowMessage(Prompt, Options);
            if (r.ResponseObject != null)
            {
                SelectedOption = r.ResponseString;
                RoutedEventArgs args = new RoutedEventArgs(VOptionBox.OptionSelectedEvent);
                RaiseEvent(args);
            }
        }
    }
}
