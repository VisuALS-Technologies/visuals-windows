using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// This control is used for selecting an applet to run.
    /// It handles everything from fetching available applets from 
    /// list in App.xaml.cs to paginating the applet buttons to running them
    /// when clicked. The only config for this is the number of rows and columns
    /// of applet buttons per page.
    /// </summary>
    public partial class AppletMenu : UserControl
    {
        /// <summary>
        /// Dictionary containing the display names and corresponding applets.
        /// </summary>
        private Dictionary<string, Applet> AppletDict = new Dictionary<string, Applet>();

        /// <summary>
        /// Number of rows per page of applets.
        /// </summary>
        public int Rows
        {
            get { return (int)base.GetValue(RowsProperty); }
            set { base.SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// Register the is Rows property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            "Rows", typeof(int), typeof(AppletMenu), new PropertyMetadata(default(int)));

        /// <summary>
        /// Number of columns per page of applets.
        /// </summary>
        public int Cols
        {
            get { return (int)base.GetValue(ColsProperty); }
            set { base.SetValue(ColsProperty, value); }
        }

        /// <summary>
        /// Register the is Cols property so it can be used in XAML
        /// </summary>
        public static readonly DependencyProperty ColsProperty = DependencyProperty.Register(
            "Cols", typeof(int), typeof(AppletMenu), new PropertyMetadata(default(int)));

        /// <summary>
        /// Current page number.
        /// </summary>
        public int page
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// Sets the Rows value to 1 and the Cols value to 3.
        /// </summary>
        public AppletMenu()
        {
            InitializeComponent();
            Rows = 1;
            Cols = 3;
            SetRows(Rows);
            SetCols(Cols);
        }

        /// <summary>
        /// Constructor allowing for row and column number initialization.
        /// </summary>
        /// <param name="rows">Number of rows per page.</param>
        /// <param name="cols">Number of columns per page.</param>
        public AppletMenu(int rows, int cols)
        {
            InitializeComponent();
            Rows = rows;
            Cols = cols;
            SetRows(Rows);
            SetCols(Cols);
        }

        /// <summary>
        /// Updates the page contents on loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppletMenu_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateItemList();
        }

        /// <summary>
        /// Updates/populates the current page with the correct applet
        /// buttons for that page.
        /// </summary>
        public void UpdateItemList()
        {
            AppletDict.Clear();
            ListGrid.Children.Clear();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    try
                    {
                        string appName = AppletManager.ActiveAppletNames[i * Cols + j + Rows * Cols * page];
                        AppletDict.Add(LanguageManager.Tokens[appName], AppletManager.GetApplet(appName));
                        VButton b = new VButton();
                        b.Role = AppletManager.GetApplet(appName).Role;
                        b.Content = LanguageManager.Tokens[appName];
                        b.Click += Item_Clicked;

                        double m = 20 / Rows;
                        if (i == 0) { b.Margin = new Thickness(m, 5, m, m); }
                        else if (i == Rows - 1) { b.Margin = new Thickness(m, m, m, 5); }
                        else { b.Margin = new Thickness(m); }

                        string s = "/" + App.Name + ";component/" + AppletManager.GetApplet(appName).IconSource;
                        b.IconSource = new BitmapImage(new Uri(s, UriKind.Relative));

                        ListGrid.Children.Add(b);
                        Grid.SetRow(b, i);
                        Grid.SetColumn(b, j);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {

                    }
                }
            }

            if (AppletManager.ActiveApplets.Count > Rows * Cols * (page + 1))
            {
                NextPage.Visibility = Visibility.Visible;
            }
            else
            {
                NextPage.Visibility = Visibility.Hidden;
            }

            if (page > 0)
            {
                PreviousPage.Visibility = Visibility.Visible;
            }
            else
            {
                PreviousPage.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// The click handler for the applet buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_Clicked(object sender, RoutedEventArgs e)
        {
            VButton b = sender as VButton;
            AppletDict[(string)b.Content].Run();
        }

        /// <summary>
        /// Set the number of rows per page.
        /// Used on initialization to actually set the grid row definitions.
        /// </summary>
        /// <param name="rows">Number of rows per page.</param>
        private void SetRows(int rows)
        {
            for (int i = 0; i < rows; i++)
            {
                ListGrid.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Set the number of columns per page.
        /// Used on initialization to actually set the grid column definitions.
        /// </summary>
        /// <param name="cols">Number of columns per page.</param>
        private void SetCols(int cols)
        {
            for (int i = 0; i < cols; i++)
            {
                ListGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        /// <summary>
        /// The click handler for the next page button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (AppletManager.ActiveApplets.Count > Rows * Cols * (page + 1))
            {
                page++;
            }
            UpdateItemList();
        }

        /// <summary>
        /// The click handler for the previous page button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page--;
            }
            UpdateItemList();
        }
    }
}
