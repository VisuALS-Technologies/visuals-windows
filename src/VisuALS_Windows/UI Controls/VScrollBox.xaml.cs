using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VList.xaml
    /// </summary>
    [ContentProperty("DisplayContent")]
    public partial class VScrollBox : UserControl
    {
        public object DisplayContent
        {
            get { return contentPresenter.Content; }
            set { contentPresenter.Content = value; }
        }

        public VScrollBox()
        {
            InitializeComponent();
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPresenter.Margin = new Thickness(0, contentPresenter.Margin.Top - 100, 0, 0);
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            contentPresenter.Margin = new Thickness(0, contentPresenter.Margin.Top + 100, 0, 0);
        }
    }
}
