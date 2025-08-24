using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    class VZoomBox : Grid
    {
        private double zoom = 1;
        public double ZoomLevel
        {
            get { return zoom; }
            set
            {
                zoom = value;
                UpdateChildSize();
            }
        }

        private UIElement content;
        public UIElement Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                viewbox.Child = value;
                UpdateChildSize();
            }
        }

        private Viewbox viewbox = new Viewbox();

        public VZoomBox() : base()
        {
            viewbox.Stretch = Stretch.Uniform;
            Children.Add(viewbox);
            SizeChanged += VZoomBox_SizeChanged;
        }

        //protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        //{
        //    UpdateChildSize();
        //}

        private void VZoomBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateChildSize();
        }

        private void UpdateChildSize()
        {
            FrameworkElement e = viewbox.Child as FrameworkElement;
            if (e != null)
            {
                e.Width = ActualWidth / ZoomLevel;
                e.Height = ActualHeight / ZoomLevel;
            }
        }
    }
}
