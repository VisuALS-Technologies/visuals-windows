using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for NewBrowser.xaml
    /// </summary>
    public partial class NewBrowser : AppletPage
    {
        #region Class Variables

        private WebBookmarks Bookmarks = new WebBookmarks();

        private const int PhantomRows = 25;
        private const int PhantomCols = 40;

        #endregion

        #region Constructors
        public NewBrowser()
        {
            InitializeComponent();
            CommonInitialization();
            url.Text = Session.Get<string>("url");
            browser.url = url.Text;
        }

        //Constructor, preinitialized
        public NewBrowser(string urlString)
        {
            InitializeComponent();
            CommonInitialization();
            url.Text = urlString;
            Session.Set("url", urlString);
            browser.url = url.Text;
        }

        void CommonInitialization()
        {
            ParentApplet = AppletManager.GetApplet<WebBrowser>(); ;
            browser.UrlChanged += Browser_UrlChanged;
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

        #endregion

        #region Button Clicks

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
        #endregion

        #region Workers

        private void addBookmarkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //bookmarks.AddBookmark(browser.url);

                AddBookmark.IsEnabled = false;
                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        AddBookmark.IsEnabled = true;
                    });
                });
            });
        }
        #endregion
    }
}
