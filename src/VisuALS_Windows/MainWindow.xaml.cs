using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Sound player for playing alarm noise
        /// </summary>
        public SoundPlayer p = new SoundPlayer(VisuALS_WPF_App.Properties.Resources.AlarmBeep);

        public static bool IsFullscreen { get; private set; } = false;

        bool IsPaused = false;
        bool IsScreenBlocked = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();

            WindowState = WindowState.Normal;
            GoWindowed();

            if (args.Contains("uninstall_options"))
            {
                MainFrame.Navigate(new UninstallOptions());
            }
            else
            {
                MainFrame.Navigate(new MainMenu());
            }

            p.Load();

            // Perform Initializations
            LanguageManager.Initialize();
            App.InitializeConfig();

            // Setup EyeBall
            GazePlane.Children.Add(EyeBall.ball);

            // Clear keyboard frame navigation shortcuts
            NavigationCommands.BrowseBack.InputGestures.Clear();
            NavigationCommands.BrowseForward.InputGestures.Clear();

            // Setup dialog window
            DialogFrame.Content = DialogWindow.DialogOverlay;
            DialogWindow.ShowDialogEvent += ShowDialogHandler;
            DialogWindow.CloseDialogEvent += CloseDialogHandler;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Perform overhaul migration if the user has not done so already
            if (!App.globalConfig.Get<bool>("file_migration_complete") && !App.IsDebug)
            {
                await OverhaulFileMigration.StartFileMigrationProcess();
            }

            SetupCopyFiles();

            // I moved actually accessing the database and common words into seperate functions
            // from setting their file paths, as setting the paths happens in language management (as it should)
            // but we can't access these until we copy the files into the AppData locations, which we can't do
            // until we do the Overhaul Migration, which requires the language management to initialize for the
            // migration dialogs... yay.

            App.predictor.SetupDatabase();
            App.predictor.LoadCommonWords();

            UpdateNews.ShowNews();
        }

        public static void SetupCopyFiles()
        {
            Directory.CreateDirectory(AppPaths.SettingsPath + "\\global\\text_prediction");
            Directory.CreateDirectory(AppPaths.DocumentsPath + "\\phrases");
            foreach (string key in App.setupCopyFilesOverwrite.Keys)
            {
                File.Delete(App.setupCopyFilesOverwrite[key]);
                File.Copy(key, App.setupCopyFilesOverwrite[key]);
            }
            foreach (string key in App.setupCopyFilesNoOverwrite.Keys)
            {
                if (!File.Exists(App.setupCopyFilesNoOverwrite[key]))
                {
                    File.Copy(key, App.setupCopyFilesNoOverwrite[key]);
                }
            }
        }

        public static void ShowPage(Page page)
        {
            App.CurrentWindow.MainFrame.Navigate(page);
        }

        private void ShowDialogHandler(object sender, EventArgs e)
        {
            BlockScreen();
        }

        private void CloseDialogHandler(object sender, EventArgs e)
        {
            UnBlockScreen();
        }

        /// <summary>
        /// Set whether to display the application as fullscreen or windowed.
        /// </summary>
        /// <param name="fullscreen"></param>
        public static void Fullscreen(bool fullscreen)
        {
            if (fullscreen != IsFullscreen)
            {
                if (fullscreen)
                {
                    GoFullscreen();
                }
                else
                {
                    GoWindowed();
                }
                IsFullscreen = fullscreen;
            }
        }

        private static void GoFullscreen()
        {
            App.CurrentWindow.Left = 0;
            App.CurrentWindow.Top = 0;
            App.CurrentWindow.Height = SystemParameters.PrimaryScreenHeight - 40;
            App.CurrentWindow.Width = SystemParameters.PrimaryScreenWidth;
            App.CurrentWindow.ResizeMode = ResizeMode.NoResize;
            App.CurrentWindow.WindowStyle = WindowStyle.None;
        }

        private static void GoWindowed()
        {
            App.CurrentWindow.Width = SystemParameters.PrimaryScreenWidth * 3 / 4;
            App.CurrentWindow.Height = SystemParameters.PrimaryScreenHeight / 1.44578;
            App.CurrentWindow.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 7.60563;
            App.CurrentWindow.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 8;
            App.CurrentWindow.ResizeMode = ResizeMode.CanResize;
            App.CurrentWindow.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        public static void Refresh()
        {
            App.CurrentWindow.Pausebtn.Value = App.CurrentWindow.Pausebtn.Value;
            App.CurrentWindow.MainFrame.Refresh();
        }

        public static void HardRefresh()
        {
            App.CurrentWindow.Pausebtn.Value = App.CurrentWindow.Pausebtn.Value;
            Page page = (Page)Activator.CreateInstance(App.CurrentWindow.MainFrame.Content.GetType());
            App.CurrentWindow.MainFrame.Navigate(page);
            RemoveLastNavigationEntry();
        }

        public async static void RemoveLastNavigationEntry()
        {
            await Task.Delay(100);
            App.CurrentWindow.MainFrame.RemoveBackEntry();
        }

        public async static void ClearNavigationHistory()
        {
            await Task.Delay(100);
            while (App.CurrentWindow.MainFrame.CanGoBack)
            {
                App.CurrentWindow.MainFrame.RemoveBackEntry();
            }
        }

        /// <summary>
        /// Shows veil over entire screen including nav bar. 
        /// This is used to cover the screen when displaying a dialog window.
        /// </summary>
        public void BlockScreen()
        {
            UnPause();
            IsScreenBlocked = true;

            MainFrame.IsEnabled = false;
            Pausebtn.IsEnabled = false;
            Homebtn.IsEnabled = false;
            BackBtn.IsEnabled = false;
            fullPauselbl.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides full screen veil. Used to uncover screen when a dialog window
        /// is done.
        /// </summary>
        public void UnBlockScreen()
        {
            IsScreenBlocked = false;

            MainFrame.IsEnabled = true;
            Pausebtn.IsEnabled = true;
            Homebtn.IsEnabled = true;
            BackBtn.IsEnabled = true;
            fullPauselbl.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows pause veil over all buttons except the pause/unpause button
        /// </summary>
        public void Pause()
        {
            if (!IsScreenBlocked)
            {
                IsPaused = true;
                pauseRect.Visibility = Visibility.Visible;
                pauseHomeRect.Visibility = Visibility.Visible;
                pauseBackRect.Visibility = Visibility.Visible;
                MainFrame.IsEnabled = false;
                Homebtn.IsEnabled = false;
                BackBtn.IsEnabled = false;
                EyeBall.ball.Opacity = .25;
            }
        }

        /// <summary>
        /// Removes pause veil from screen
        /// </summary>
        public void UnPause()
        {
            if (!IsScreenBlocked)
            {
                IsPaused = false;
                pauseRect.Visibility = Visibility.Hidden;
                pauseHomeRect.Visibility = Visibility.Hidden;
                pauseBackRect.Visibility = Visibility.Hidden;
                MainFrame.IsEnabled = true;
                Homebtn.IsEnabled = true;
                BackBtn.IsEnabled = true;
                EyeBall.ball.Opacity = 1;
            }
        }

        public static void HideNavBar()
        {
            App.CurrentWindow.NavRow.Height = new GridLength(0);
        }

        public static void ShowNavBar()
        {
            App.CurrentWindow.NavRow.Height = new GridLength(3, GridUnitType.Star);
        }

        private void Homebtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainMenu());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public static void GoBack()
        {
            if (App.CurrentWindow.MainFrame.CanGoBack)
            {

                App.CurrentWindow.MainFrame.GoBack();
            }
        }

        public static void GoForward()
        {
            if (App.CurrentWindow.MainFrame.CanGoForward)
            {
                App.CurrentWindow.MainFrame.GoForward();
            }
        }

        private void Pause_Selected(object sender, RoutedEventArgs e)
        {
            if (Pausebtn.Value)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }
}