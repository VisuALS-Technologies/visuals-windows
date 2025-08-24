using System.Windows;
using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for VWindow.xaml
    /// </summary>
    public partial class VWindow : UserControl
    {
        public event RoutedEventHandler CloseClick
        {
            add { CancelBtn.Click += value; }
            remove { CancelBtn.Click -= value; }
        }

        public bool DisableClose
        {
            get
            {
                return !CancelBtn.IsEnabled;
            }
            set
            {
                CancelBtn.IsEnabled = !value;
            }
        }

        public VWindow(bool fullscreen = false)
        {
            InitializeComponent();
            FullScreen(fullscreen);
        }

        public void FullScreen(bool fullscreen)
        {
            if (fullscreen)
            {
                Row1.Height = new GridLength(0);
                Row2.Height = new GridLength(1, GridUnitType.Star);
                Row3.Height = new GridLength(0);

                Col1.Width = new GridLength(0);
                Col2.Width = new GridLength(1, GridUnitType.Star);
                Col3.Width = new GridLength(0);
            }
            else
            {
                Row1.Height = new GridLength(1, GridUnitType.Star);
                Row2.Height = new GridLength(7, GridUnitType.Star);
                Row3.Height = new GridLength(1, GridUnitType.Star);

                Col1.Width = new GridLength(1, GridUnitType.Star);
                Col2.Width = new GridLength(7, GridUnitType.Star);
                Col3.Width = new GridLength(1, GridUnitType.Star);
            }
        }

        public void Navigate(UIElement e)
        {
            ContentFrame.Navigate(e);
        }

        public void GoForward()
        {
            ContentFrame.GoForward();
        }

        public void GoBack()
        {
            ContentFrame.GoBack();
        }
    }
}
