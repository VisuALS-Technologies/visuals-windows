using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //switch from email to Web Browser information
        public double count = 0;

        //factories/hosts
        public static Predictor predictor = new Predictor();
        public string predictionDataDBPath;
        public string englishWordsListFilePath;
        public static bool wasEyeBallEnabled;
        public static string Name = Assembly.GetEntryAssembly().GetName().Name;
        public static SettingsFile globalConfig = new SettingsFile("global\\config");
        public static Version CurrentVersion = new Version(3, 0, 0, 0);

        public static Dictionary<string, string> setupCopyFilesOverwrite = new Dictionary<string, string>() {
            { ".\\Resources\\Text Prediction\\en.US.txt", AppPaths.SettingsPath + "\\global\\text_prediction\\en.US.txt" },
            { ".\\Resources\\Text Prediction\\es.txt", AppPaths.SettingsPath + "\\global\\text_prediction\\es.txt" },
        };

        public static Dictionary<string, string> setupCopyFilesNoOverwrite = new Dictionary<string, string>()
        {
            { ".\\Resources\\Text Prediction\\en.US.sqlite", AppPaths.SettingsPath + "\\global\\text_prediction\\en.US.sqlite" },
            { ".\\Resources\\Text To Speech\\Conversation.txt", AppPaths.DocumentsPath + "\\phrases\\Conversation.txt" },
            { ".\\Resources\\Text To Speech\\Feelings.txt", AppPaths.DocumentsPath + "\\phrases\\Feelings.txt" },
            { ".\\Resources\\Text To Speech\\Home Automation.txt", AppPaths.DocumentsPath + "\\phrases\\Home Automation.txt" },
            { ".\\Resources\\Text To Speech\\Needs.txt", AppPaths.DocumentsPath + "\\phrases\\Needs.txt" },
        };

        public static App CurrentApp
        {
            get { return Application.Current as App; }
        }

        public static MainWindow CurrentWindow
        {
            get { return CurrentApp.MainWindow as MainWindow; }
        }

        public static Version LastRunVersion
        {
            get
            {
                return globalConfig.Get<Version>("last_run_version");
            }
        }

        public static string ExecutableName
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name; }
        }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                    return false;
#endif
            }
        }

        public static bool IsFirstRun
        {
            get { return ApplicationDeployment.CurrentDeployment.IsFirstRun; }
        }

        public App()
        {
            try
            {
                EnforceSingleInstance();

                // Create important directories
                AppPaths.Initialize();

                // Initialize global config settings
                globalConfig.Initialize("last_run_version", "1.4.0.2");
                globalConfig.Initialize("compatibility_mode", false);
                globalConfig.Initialize("file_migration_complete", false);
            }
            catch (Exception e)
            {
                LogCrash(e);
                MessageBox.Show("An unexpected error occured on startup: \"" + e.Message + "\"\n A crash log can be found at Documents/VisuALS/logs/crash_log.txt", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    Shutdown();
                }
                catch { }
            }
        }

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void EnforceSingleInstance()
        {
            // If there is another process running with the same name ("VisuALS"), get it's handle
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();

            // If another instance was found, open it if it was minimized, and terminate this instance
            if (runningProcess != null)
            {
                int SW_RESTORE = 9;
                ShowWindow(runningProcess.MainWindowHandle, SW_RESTORE);
                CurrentApp.Shutdown();
            }
        }

        public static void SoftRestart()
        {
            VisuALS_WPF_App.MainWindow.ClearNavigationHistory();
            VisuALS_WPF_App.MainWindow.ShowPage(new MainMenu());
        }

        public static void Restart()
        {
            System.Diagnostics.Process.Start(Assembly.GetExecutingAssembly().Location);
            CurrentApp.Shutdown();
        }

        public static void Close()
        {
            CurrentApp.Shutdown();
        }

        public static void LogWrite(string str)
        {
            File.AppendAllText(System.IO.Path.Combine(AppPaths.LogsPath, "debug_log.txt"), str + "\n");
        }

        public static void InitializeConfig()
        {
            //Initialize settings values
            App.globalConfig.Initialize("language", "English");
            App.globalConfig.Initialize("keyboard", "kl_en_classic_qwerty");
            App.globalConfig.Initialize("tts_voice", SpeechUtils.InstalledVoiceNames[0]);
            App.globalConfig.Initialize("mute_alarm", false);
            App.globalConfig.Initialize("verbal_confirmations", false);
            App.globalConfig.Initialize("fontsizes", StyleControl.GetFontSizes());
            App.globalConfig.Initialize("fullscreen", true);
            App.globalConfig.Initialize("theme", "th_gumdrop");
            App.globalConfig.Initialize("accent_color", "#0056FF");
            App.globalConfig.Initialize("eyeball_size", 50.0);
            App.globalConfig.Initialize("eyeball_border_size", 5.0);
            App.globalConfig.Initialize("eyeball_color", "#FF9000");
            App.globalConfig.Initialize("eyeball_border_color", "#FF9000");
            App.globalConfig.Initialize("eyeball_enabled", true);
            App.globalConfig.Initialize("dwell_time", 1000);
            App.globalConfig.Initialize("dwell_indicator", "di_pie");

            //Apply initialized values
            LanguageManager.CurrentLanguage = App.globalConfig.Get<string>("language");
            KeyboardManager.CurrentLayoutName = App.globalConfig.Get<string>("keyboard");
            FontSizes fontSizes = App.globalConfig.Get<FontSizes>("fontsizes");
            StyleControl.SetFontSizes(fontSizes);
            StyleControl.SetTheme(App.globalConfig.Get<string>("theme"));
            StyleControl.SetAccentColor(ColorUtils.FromString(App.globalConfig.Get<string>("accent_color")));
            VisuALS_WPF_App.MainWindow.Fullscreen(App.globalConfig.Get<bool>("fullscreen"));
            EyeBall.BallSize = App.globalConfig.Get<double>("eyeball_size");
            EyeBall.LineSize = App.globalConfig.Get<double>("eyeball_border_size");
            string color = App.globalConfig.Get<string>("eyeball_color");
            EyeBall.BallColor = new SolidColorBrush(ColorUtils.FromString(color));
            string borderColor = App.globalConfig.Get<string>("eyeball_border_color");
            EyeBall.LineColor = new SolidColorBrush(ColorUtils.FromString(borderColor));
            EyeBall.IsEnabled = App.globalConfig.Get<bool>("eyeball_enabled");
            Application.Current.Resources["DwellTime"] = App.globalConfig.Get<int>("dwell_time");
            VButton.UpdateDwellIndicatorAnimation();
            if (App.globalConfig.Get<string>("dwell_indicator") == "di_pie")
            {
                VButton.DwellIndicator = true;
            }
            else
            {
                VButton.DwellIndicator = false;
            }
        }

        /// <summary>
        /// Log a crash in a Log file
        /// </summary>
        /// <param name="e"></param>
        public static void LogCrash(Exception e)
        {
            Directory.CreateDirectory(AppPaths.LogsPath);
            string log = "";
            log += DateTime.Now.ToLocalTime() + "\n\n";
            log += e.ToString() + "\n\n";
            File.WriteAllText(System.IO.Path.Combine(AppPaths.LogsPath, "crash_log.txt"), log);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogCrash(e.Exception);
            MessageBox.Show("An unexpected error occured: \"" + e.Exception.Message + "\"\n A crash log can be found at Documents/VisuALS/logs/crash_log.txt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                globalConfig.Set("last_run_version", CurrentVersion.ToString());
            }
            catch { }

            if (GazeTrackerManager.SelectedGazeTracker != null)
                GazeTrackerManager.SelectedGazeTracker.EndGazePointStream();
        }
    }
}
