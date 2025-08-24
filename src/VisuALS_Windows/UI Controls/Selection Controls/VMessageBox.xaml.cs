using System.Collections.Generic;
using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VDialog.xaml
    /// </summary>
    public partial class VMessageBox : VSelectionControl
    {


        public VMessageBox()
        {
            InitializeComponent();
        }

        public VMessageBox(string message)
        {
            InitializeComponent();
            messageText.Text = message;
            VButton optBtn = new VButton();
            optBtn.Content = "Ok";
            optBtn.Role = ButtonRole.Item;
            optBtn.Click += OptionClicked;
            optBtn.MinWidth = 250;
            buttonPanel.Children.Add(optBtn);
        }

        public VMessageBox(string message, params string[] options)
        {
            InitializeComponent();
            messageText.Text = message;
            foreach (string option in options)
            {
                VButton optBtn = new VButton();
                optBtn.Content = option;
                optBtn.Role = ButtonRole.Item;
                optBtn.Click += OptionClicked;
                optBtn.MinWidth = 250;
                buttonPanel.Children.Add(optBtn);
            }
        }

        public VMessageBox(string message, IEnumerable<string> options)
        {
            InitializeComponent();
            messageText.Text = message;
            foreach (string option in options)
            {
                VButton optBtn = new VButton();
                optBtn.Content = option;
                optBtn.Role = ButtonRole.Item;
                optBtn.Click += OptionClicked;
                optBtn.MinWidth = 200;
                buttonPanel.Children.Add(optBtn);
            }
        }

        public void OptionClicked(object sender, RoutedEventArgs e)
        {
            string optVal = (sender as VButton).Content.ToString();
            SelectedItem = optVal;
            SelectedItemString = optVal;
            RoutedEventArgs args = new RoutedEventArgs(ItemSelectionChangedEvent);
            RaiseEvent(args);
        }
    }
}