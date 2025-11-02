using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>

    public partial class OldBrowser : AppletPage
    {
        #region Class Variables

        private WebBookmarks Bookmarks = new WebBookmarks();

        //indicates whether or not the keyboard is currently open
        private bool keyboardOpen { get; set; }

        private const int PhantomRows = 25;
        private const int PhantomCols = 40;

        #endregion

        #region Constructors
        public OldBrowser()
        {
            InitializeComponent();
            CommonInitialization();
            url.Text = Session.Get<string>("url");
            browser.url = url.Text;
        }

        //Constructor, preinitialized
        public OldBrowser(string urlString)
        {
            InitializeComponent();
            CommonInitialization();
            url.Text = urlString;
            Session.Set("url", urlString);
            browser.url = url.Text;
        }

        //actions common to all constructors
        void CommonInitialization()
        {
            ParentApplet = AppletManager.GetApplet<WebBrowser>();
            keyboardOpen = false;
            browser.UrlChanged += Browser_UrlChanged;
            SetupPhantomGrid();
        }

        private void Browser_UrlChanged(object sender, RoutedEventArgs e)
        {
            if (browser.url != null)
            {
                url.Text = browser.url;
                Session.Set("url", browser.url);
            }
            else
            {
                url.Text = "https://www.google.com";
                Session.Set("url", "https://www.google.com");
            }
        }

        void SetupPhantomGrid()
        {
            for (int i = 0; i < PhantomRows; i++)
            {
                phantomButtonGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < PhantomCols; i++)
            {
                phantomButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < PhantomRows; i++)
            {
                for (int j = 0; j < PhantomCols; j++)
                {
                    VButton b = new VButton();
                    b.Role = ButtonRole.Phantom;
                    b.Click += PhantomButton_Click;
                    Grid.SetRow(b, i);
                    Grid.SetColumn(b, j);
                    phantomButtonGrid.Children.Add(b);
                }
            }
        }
        #endregion

        #region Button Clicks

        private void PhantomButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickRead.Value)
            {
                VButton b = sender as VButton;
                Point p = b.PointToScreen(new Point(0, 0));
                p.X += b.ActualWidth / 2;
                p.Y += b.ActualHeight / 2;

                InputSimulator.SetCursor(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
                phantomButtonGrid.IsHitTestVisible = false;

                Task.Delay(100).ContinueWith(_ =>
               {
                   this.Dispatcher.Invoke(() =>
                  {
                      browser.Focus();
                      if (!ClickType.Value)
                      {
                          InputSimulator.MouseButtonClick(InputSimulator.MouseButtons.LEFT);
                      }
                      else
                      {
                          InputSimulator.MouseButtonClick(InputSimulator.MouseButtons.LEFT);
                          InputSimulator.MouseButtonClick(InputSimulator.MouseButtons.LEFT);
                      }
                      phantomButtonGrid.IsHitTestVisible = true;
                  });
               });
            }
        }

        //add a bookmark to the bookmarks page
        private async void AddBookmark_Click(object sender, RoutedEventArgs e)
        {
            Bookmarks.AddBookmark(browser.url);
            await DialogWindow.ShowMessage($"'{browser.url}' {LanguageManager.Tokens["wb_bookmark_added"]}.");
        }

        public async void Bookmarks_Click(object sender, RoutedEventArgs e)
        {
            if (!Bookmarks.IsEmpty())
            {
                DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["wb_bookmarks"], Bookmarks.GetLabeledBookmarks(), 4, 1);
                // Reload the page with the url selected
                if (r.ResponseObject != null)
                {
                    browser.url = r.ResponseObject as string;
                }
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["wb_no_bookmarks"]);
            }
        }

        private async void DeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            if (!Bookmarks.IsEmpty())
            {
                DialogResponse r = await DialogWindow.ShowList(LanguageManager.Tokens["wb_bookmarks"], Bookmarks.GetLabeledBookmarks(), 4, 1);
                Bookmarks.RemoveBookmark(r.ResponseObject as string);
            }
            else
            {
                await DialogWindow.ShowMessage(LanguageManager.Tokens["wb_no_bookmarks"]);
            }
        }

        //zoom control
        private void Zoom_Click(object sender, RoutedEventArgs e)
        {
            switch (browser.zoomLevel)
            {
                case 0:
                    browser.SetZoom(1);
                    Zoom.Content = LanguageManager.Tokens["wb_zoom_in_x2"];
                    break;
                case 1:
                    browser.SetZoom(2);
                    Zoom.Content = LanguageManager.Tokens["wb_zoom_out"];
                    break;
                case 2:
                    browser.SetZoom(0);
                    Zoom.Content = LanguageManager.Tokens["wb_zoom_in"];
                    break;
            }
        }

        //web navigation tools
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            browser.GoBack();
        }

        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            browser.GoForward();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            browser.url = browser.url;
            Refresh.IsEnabled = false;
            Task.Delay(1000).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Refresh.IsEnabled = true;
                });
            });
        }

        private void StopLoading_Click(object sender, RoutedEventArgs e)
        {
            browser.StopLoading();
            StopLoading.IsEnabled = false;
            Task.Delay(1000).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    StopLoading.IsEnabled = true;
                });
            });
        }

        private void ScrollUpBtn_Click(object sender, RoutedEventArgs e)
        {
            browser.scrollUp();
        }

        private void ScrollDownBtn_Click(object sender, RoutedEventArgs e)
        {
            browser.scrollDown();
        }

        private void ScrollLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            browser.scrollLeft();
        }

        private void ScrollRightBtn_Click(object sender, RoutedEventArgs e)
        {
            browser.scrollRight();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            browser.url = url.Text;
        }

        private void url_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                browser.url = url.Text;
                browser.closeKeyboard();
                keyboardOpen = false;
            }
            else if (Keyboard.IsKeyDown(Key.Clear))
            {
                url.Text = "";
            }
        }

        private void InputUrl_Click(object sender, RoutedEventArgs e)
        {
            if (!keyboardOpen)
            {
                browser.openKeyboard(url);
                keyboardOpen = true;
                phantomButtonGrid.IsHitTestVisible = false;
                phantomButtonGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                browser.closeKeyboard();
                keyboardOpen = false;
                phantomButtonGrid.IsHitTestVisible = true;
                phantomButtonGrid.Visibility = Visibility.Visible;
            }
        }

        private void BrowserInput_Click(object sender, RoutedEventArgs e)
        {
            if (!keyboardOpen)
            {
                browser.openKeyboard();
                keyboardOpen = true;
                phantomButtonGrid.IsHitTestVisible = false;
                phantomButtonGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                browser.closeKeyboard();
                keyboardOpen = false;
                phantomButtonGrid.IsHitTestVisible = true;
                phantomButtonGrid.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
