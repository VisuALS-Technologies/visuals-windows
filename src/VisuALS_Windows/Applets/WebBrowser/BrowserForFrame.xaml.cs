using CefSharp;
using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VisuALS_WPF_App.Browser;
using C_IBrowser = CefSharp.IBrowser;

namespace VisuALS_WPF_App
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public event Action<string> PopupRequest;

        public bool OnBeforePopup(IWebBrowser browserControl, C_IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition,
                           bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            browserControl.Load(targetUrl);

            newBrowser = null;

            if (PopupRequest != null)
            {
                PopupRequest(targetUrl);
            }

            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, C_IBrowser browser)
        {

        }

        public bool DoClose(IWebBrowser browserControl, C_IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, C_IBrowser browser)
        {

        }
    }

    /// <summary>
    /// Interaction logic for BrowserForFrame.xaml
    /// </summary>
    public partial class BrowserForFrame : UserControl
    {
        public string email { get; set; }
        public int zoomLevel { get; set; }

        public ChromiumWebBrowser chromiumBrowser { get; set; }
        private int verticalScrollCount { get; set; }
        private int horizontalScrollCount { get; set; }

        public string url
        {
            get { return chromiumBrowser.Address; }
            set
            {
                try
                {
                    chromiumBrowser.Load(value);
                }
                catch
                {
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        try
                        {
                            chromiumBrowser.Load(value);
                        }
                        catch { }
                    });
                }
            }
        }

        /// <summary>
        /// This event is raised when the browser is navigated to a new page
        /// </summary>
        public static readonly RoutedEvent UrlChangedEvent = EventManager.RegisterRoutedEvent(
            "UrlChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BrowserForFrame));

        /// <summary>
        /// The handler add/remove logic for the Url Changed Event
        /// </summary>
        public event RoutedEventHandler UrlChanged
        {
            add { AddHandler(UrlChangedEvent, value); }
            remove { RemoveHandler(UrlChangedEvent, value); }
        }

        public BrowserForFrame()
        {
            InitializeComponent();
            try
            {
                InitializeCEFSharp();
            }
            catch (Exception ex)
            {
                DialogWindow.ShowMessage(ex.Message);
            }

            this.Dispatcher.Invoke(() =>
            {
                Loader.IsBusy = false;
            });

            SetupChromiumBrowser();

            verticalScrollCount = 0;
            horizontalScrollCount = 0;
            zoomLevel = 0;
            BrowserKeyboard.Layout = KeyboardManager.GetCurrentLayout();
            BrowserKeyboard.DisableTextPrediction();

            Unloaded += BrowserForFrame_Unloaded;
        }

        private void SetupChromiumBrowser()
        {
            //General setup
            chromiumBrowser = new ChromiumWebBrowser();
            chromiumBrowser.Address = url;
            chromiumBrowser.LoadingStateChanged += ChromiumBrowser_LoadingStateChanged;
            chromiumBrowser.AddressChanged += ChromiumBrowser_AddressChanged;
            chromiumBrowser.LifeSpanHandler = new LifeSpanHandler();
            chromiumBrowserFrame.Content = chromiumBrowser;

            //Zoom setup
            ScaleTransform scale = new ScaleTransform();
            TranslateTransform translate = new TranslateTransform();
            TransformCollection t = new TransformCollection();
            t.Add(scale);
            t.Add(translate);
            TransformGroup group = new TransformGroup();
            group.Children = t;
            chromiumBrowser.RenderTransform = group;
        }

        private void ChromiumBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                RoutedEventArgs args = new RoutedEventArgs(BrowserForFrame.UrlChangedEvent);
                RaiseEvent(args);
            });
        }

        private void BrowserForFrame_Unloaded(object sender, RoutedEventArgs e)
        {
            chromiumBrowser.Dispose();
            chromiumBrowser = null;
        }

        [STAThread]
        public static void InitializeCEFSharp()
        {
            var settings = new CefBrowserSettings();
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("--enable-system-flash", "1");
            settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream");
            settings.WindowlessRenderingEnabled = true;

            //settings.CachePath = DataManager.GetDirPath("settings/apps/web_browser/cache");
            if (Cef.IsInitialized != true)
            {
                Cef.Initialize(settings);
            }
        }

        public void openKeyboard()
        {
            Grid.SetRowSpan(chromiumBrowserFrame, 1);
            Grid.SetRowSpan(Loader, 1);
            BrowserKeyboard.FocusElement = chromiumBrowser;
            BrowserKeyboard.Visibility = Visibility.Visible;
        }

        public void openKeyboard(UIElement focusElement)
        {
            Grid.SetRowSpan(chromiumBrowserFrame, 1);
            Grid.SetRowSpan(Loader, 1);
            BrowserKeyboard.FocusElement = focusElement;
            BrowserKeyboard.Visibility = Visibility.Visible;
        }

        public void closeKeyboard()
        {
            Grid.SetRowSpan(chromiumBrowserFrame, 2);
            Grid.SetRowSpan(Loader, 2);
            BrowserKeyboard.Visibility = Visibility.Hidden;
        }

        private void ChromiumBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Loader.IsBusy = true;
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Loader.IsBusy = false;
                });
            }
        }

        public void scrollDown()
        {
            if (zoomLevel != 0)
            {
                if (!Loader.IsBusy)
                {
                    foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                    {
                        if (t.GetType() == typeof(TranslateTransform))
                        {
                            {
                                if (zoomLevel == 1 && verticalScrollCount < 5)
                                {
                                    (t as TranslateTransform).Y -= 120;
                                    verticalScrollCount++;
                                }
                                else if (zoomLevel == 2 && verticalScrollCount < 8)
                                {
                                    (t as TranslateTransform).Y -= 230;
                                    verticalScrollCount++;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += scrollDownWorker_DoWork;
                worker.RunWorkerCompleted += scrollDownWorker_RunWorkerCompleted;
                if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                {
                    worker.RunWorkerAsync();
                }
            }
        }

        private enum websiteType
        {
            normal,
            email,
            pdf
        }

        private void scrollDownWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string checkString = "";
            string checkCheckString = "";
            string pdfCheck = "";

            try
            {
                checkString = url.Substring(0, 30);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                checkCheckString = url.Substring(0, 22);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                pdfCheck = url.Substring(url.Length - 3, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (checkString == "https://mail.google.com/mail/u" || checkCheckString == "https://mg.mail.yahoo." || checkCheckString == "https://mail.aol.com/w" || checkCheckString == "https://mail.yahoo.com")
            {
                e.Result = websiteType.email;
            }
            else if (pdfCheck == "pdf")
            {
                e.Result = websiteType.pdf;
            }
            else
            {
                e.Result = websiteType.normal;
            }
        }

        private void scrollDownWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if ((websiteType)e.Result == websiteType.normal)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                        {
                            chromiumBrowser.ExecuteScriptAsync("(function() { window.scrollBy(0, 300); })();");
                        }
                    });
                }
                else if ((websiteType)e.Result == websiteType.email)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                        {
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('list-view-items loading')[0].scrollTop += 100; })();");
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementById(':4').scrollTop += 100; })();");
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('p_R Z_0 iy_h iz_A H_6D6F W_6D6F X_6Fd5 N_6Fd5 k_w')[0].scrollTop += 100; })();");
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('dojoxGrid-scrollbox')[0].scrollTop += 100; })();");
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('iz_A em_0 Z_0')[0].scrollTop += 100; })();");
                            chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('messageArea needLayout')[0].scrollTop += 100; })();");
                        }
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Keyboard.Focus(chromiumBrowser);
                        SendKey((byte)System.Windows.Forms.Keys.PageDown);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void scrollUp()
        {
            if (zoomLevel != 0)
            {
                if (!Loader.IsBusy)
                {
                    foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                    {
                        if (t.GetType() == typeof(TranslateTransform))
                        {
                            if ((t as TranslateTransform).Y != 0)
                            {
                                if (zoomLevel == 1)
                                {
                                    (t as TranslateTransform).Y += 120;
                                    verticalScrollCount--;
                                }
                                else
                                {
                                    (t as TranslateTransform).Y += 230;
                                    verticalScrollCount--;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += scrollUpWorker_DoWork;
                worker.RunWorkerCompleted += scrollUpWorker_RunWorkerCompleted;
                if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                {
                    worker.RunWorkerAsync();
                }
            }
        }

        private void scrollUpWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            string checkString = "";
            string checkCheckString = "";
            string pdfCheck = "";

            try
            {
                checkString = url.Substring(0, 30);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                checkCheckString = url.Substring(0, 22);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                pdfCheck = url.Substring(url.Length - 3, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            if (checkString == "https://mail.google.com/mail/u" || checkCheckString == "https://mail.aol.com/w" || checkCheckString == "https://mg.mail.yahoo." || checkCheckString == "https://mail.yahoo.com")
            {
                e.Result = websiteType.email;
            }
            else if (pdfCheck == "pdf")
            {
                e.Result = websiteType.pdf;
            }
            else
            {
                e.Result = websiteType.normal;
            }
        }

        private void scrollUpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((websiteType)e.Result == websiteType.normal)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                    {
                        chromiumBrowser.ExecuteScriptAsync("(function() { window.scrollBy(0, -300); })();");
                    }
                });
            }
            else if ((websiteType)e.Result == websiteType.email)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                    {
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('list-view-items loading')[0].scrollTop -= 100; })();");
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementById(':4').scrollTop -= 100; })();");
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('p_R Z_0 iy_h iz_A H_6D6F W_6D6F X_6Fd5 N_6Fd5 k_w')[0].scrollTop -= 100; })();");
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('dojoxGrid-scrollbox')[0].scrollTop -= 100; })();");
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('iz_A em_0 Z_0')[0].scrollTop -= 100; })();");
                        chromiumBrowser.ExecuteScriptAsync("(function() { document.getElementsByClassName('messageArea needLayout')[0].scrollTop -= 100; })();");

                        //Keyboard.Focus(chromiumBrowser);
                        //SendKey((byte)System.Windows.Forms.Keys.PageUp);
                    }
                });
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    Keyboard.Focus(chromiumBrowser);
                    SendKey((byte)System.Windows.Forms.Keys.PageUp);
                });
            }
        }

        public void GoToPageFrom()
        {
            if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
            {
                string urlString = "'" + url + "'";
                chromiumBrowser.ExecuteScriptAsync("(function() { window.location.href = " + urlString + "; })()");
            }
        }

        public void GoBack()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                {
                    chromiumBrowser.ExecuteScriptAsync("(function() { history.back(); })();");
                }
            });
        }

        public void GoForward()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (chromiumBrowser.CanExecuteJavascriptInMainFrame)
                {
                    chromiumBrowser.ExecuteScriptAsync("(function() { history.forward(); })();");
                }
            });
        }

        public static void SendKey(byte k)
        {
            keybd_event(k, 0x45, 0x1, (UIntPtr)0);
            keybd_event(k, 0x45, 0x1 | 0x2, (UIntPtr)0);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public void scrollLeft()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!Loader.IsBusy)
                {
                    if (zoomLevel != 0)
                    {
                        foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                        {
                            if (t.GetType() == typeof(TranslateTransform))
                            {
                                if ((t as TranslateTransform).X != 0)
                                {
                                    if (zoomLevel == 1)

                                    {
                                        //var scale = (ScaleTransform)t; 
                                        (t as TranslateTransform).X += 260;
                                        horizontalScrollCount--;
                                    }
                                    else

                                    {
                                        (t as TranslateTransform).X += 600;
                                        horizontalScrollCount--;
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        public void scrollRight()
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!Loader.IsBusy)
                {
                    if (zoomLevel != 0)
                    {
                        foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                        {
                            if (t.GetType() == typeof(TranslateTransform))
                            {
                                if (zoomLevel == 1 && horizontalScrollCount < 5)
                                {
                                    //var scale = (ScaleTransform)t; 
                                    (t as TranslateTransform).X -= 260;
                                    horizontalScrollCount++;
                                }
                                else if (zoomLevel == 2 && horizontalScrollCount < 11)
                                {
                                    (t as TranslateTransform).X -= 360;
                                    horizontalScrollCount++;
                                }
                            }
                        }
                    }
                }
            });
        }

        public void Tab()
        {
            Keyboard.Focus(chromiumBrowser);
            SendKey((byte)System.Windows.Forms.Keys.Tab);
        }

        public void StopLoading()
        {
            chromiumBrowser.Stop();
        }

        public void SetZoom(int zoom)
        {
            zoomLevel = zoom;
            //chromiumBrowser.SetZoomLevel(zoom);
            if (!Loader.IsBusy)
            {
                if (zoom == 1)
                {
                    BackgroundWorker zoomInWorker = new BackgroundWorker();
                    this.Dispatcher.Invoke(() =>
                    {
                        foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                        {
                            if (t.GetType() == typeof(ScaleTransform))
                            {
                                (t as ScaleTransform).ScaleX = 2;
                                (t as ScaleTransform).ScaleY = 2;
                            }
                        }
                    });
                }
                else if (zoom == 2)
                {
                    BackgroundWorker doubleZoomWorker = new BackgroundWorker();
                    this.Dispatcher.Invoke(() =>
                    {
                        foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                        {
                            if (t.GetType() == typeof(ScaleTransform))
                            {
                                //var scale = (ScaleTransform)t; 
                                (t as ScaleTransform).ScaleX = 4;
                                (t as ScaleTransform).ScaleY = 4;
                            }
                            if (t.GetType() == typeof(TranslateTransform))
                            {
                                (t as TranslateTransform).X = 0;
                                (t as TranslateTransform).Y = 0;
                            }
                        }
                    });
                }
                else
                {
                    BackgroundWorker zoomOutWorker = new BackgroundWorker();
                    this.Dispatcher.Invoke(() =>
                    {
                        //browser.ZoomOut(); 
                        foreach (Transform t in ((chromiumBrowser.RenderTransform as TransformGroup).Children as TransformCollection))
                        {
                            if (t.GetType() == typeof(ScaleTransform))
                            {
                                //var scale = (ScaleTransform)t; 
                                (t as ScaleTransform).ScaleX = 1;
                                (t as ScaleTransform).ScaleY = 1;
                            }
                            if (t.GetType() == typeof(TranslateTransform))
                            {
                                (t as TranslateTransform).X = 0;
                                (t as TranslateTransform).Y = 0;
                            }
                        }
                    });
                }
            }
        }
    }
}
