using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VList.xaml
    /// </summary>
    [ContentProperty("DisplayContent")]
    public partial class VCollapseBox : UserControl
    {
        public string Text         {
            get { return textLabel.Content.ToString(); }
            set { textLabel.Content = value; }
        }
        public bool IsCollapsed { get; set; } = true;
        public object DisplayContent
        {
            get { return contentPresenter.Content; }
            set { contentPresenter.Content = value; }
        }
        public VCollapseBox()
        {
            InitializeComponent();
        }
        private void CollapseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsCollapsed)
            {
                contentPresenter.Visibility = Visibility.Visible;
                CollapseRow.Height = GridLength.Auto;
                IsCollapsed = false;
            }
            else
            {
                contentPresenter.Visibility = Visibility.Collapsed;
                CollapseRow.Height = new GridLength(0);
                IsCollapsed = true;
            }
        }
    }
}
