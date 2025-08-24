using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class PhotoViewerPage : AppletPage
    {
        /// <summary>
        /// Default constructor
        /// </summary>

        List<VButton> im_buttons = new List<VButton>();
        Dictionary<VButton, string> im_paths = new Dictionary<VButton, string>();
        string[] extensions = { "bmp", "png", "jpg", "jpeg", "gif" };
        int page = 0;
        int rows = 3;
        int cols = 3;

        public PhotoViewerPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<PhotoViewer>();
            PopulatePhotosGrid();
            UpdatePhotosGrid();
        }

        private void PopulatePhotosGrid()
        {
            PhotosGrid.Children.Clear();
            im_buttons.Clear();
            for (int i = 0; i < rows; i++)
            {
                PhotosGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int j = 0; j < cols; j++)
            {
                PhotosGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    VButton b = new VButton();
                    b.Click += ImButton_Click;
                    PhotosGrid.Children.Add(b);
                    im_buttons.Add(b);
                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                }
            }
        }

        private void UpdatePhotosGrid()
        {
            IEnumerable<string> files = Directory.EnumerateFiles(Config.Get<string>("photos_folder"), "*.*", SearchOption.AllDirectories).Where(x => extensions.Contains(Path.GetExtension(x).TrimStart('.').ToLower()));
            files = files.Skip(page * rows * cols).Take(rows * cols);
            IEnumerator<string> iter = files.GetEnumerator();
            foreach (VButton b in im_buttons)
            {
                try
                {
                    iter.MoveNext();
                    ImageBrush brush = new ImageBrush(new BitmapImage(new Uri(iter.Current)));
                    brush.Stretch = Stretch.UniformToFill;
                    b.Background = brush;
                    im_paths[b] = iter.Current;
                    b.IsEnabled = true;
                }
                catch
                {
                    b.Background = new SolidColorBrush(Colors.Transparent);
                    b.IsEnabled = false;
                }
            }
        }

        private void ImButton_Click(object sender, EventArgs e)
        {
            Image im = new Image();
            im.Source = new BitmapImage(new Uri(im_paths[(VButton)sender]));
            DialogWindow.Show(im);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page--;
            }
            UpdatePhotosGrid();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            page++;
            UpdatePhotosGrid();
        }
    }
}