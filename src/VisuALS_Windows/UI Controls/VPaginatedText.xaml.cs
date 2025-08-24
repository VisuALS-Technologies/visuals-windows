using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for PaginatedText.xaml
    /// </summary>
    public partial class VPaginatedText : UserControl
    {
        public List<string> Text { get; set; }

        public string Title
        {
            get { return (string)base.GetValue(TitleProperty); }
            set
            {
                base.SetValue(TitleProperty, value);
                titleLabel.Content = value;
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(VPaginatedText), new PropertyMetadata(default(string)));

        public int Page { get; set; }

        public Action CloseAction { get; set; }

        public VPaginatedText()
        {
            InitializeComponent();
        }

        public VPaginatedText(ICollection<string> text, Action closeAction = null)
        {
            InitializeComponent();
            Text = text.ToList();
            Loaded += VPaginatedText_Loaded;
            CloseAction = closeAction;
        }

        private void VPaginatedText_Loaded(object sender, RoutedEventArgs e)
        {
            MainTextBlock.Text = Text[Page];
            if (CloseAction != null)
            {
                Grid.SetColumnSpan(PreviousPage, 1);
                Grid.SetColumn(NextPage, 1);
                Close.Visibility = Visibility.Visible;
            }
            UpdateButtons();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (Text.Count - 1 > Page)
            {
                Page++;
                MainTextBlock.Text = Text[Page];
            }
            UpdateButtons();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (Page > 0)
            {
                Page--;
                MainTextBlock.Text = Text[Page];
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (Page > 0)
            {
                PreviousPage.IsEnabled = true;
            }
            else
            {
                PreviousPage.IsEnabled = false;
            }

            if (Text.Count - 1 > Page)
            {
                NextPage.IsEnabled = true;
            }
            else
            {
                NextPage.IsEnabled = false;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (CloseAction != null)
            {
                CloseAction();
            }
        }
    }
}
