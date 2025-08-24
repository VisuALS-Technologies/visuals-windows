using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DevToolsPage : AppletPage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DevToolsPage()
        {
            InitializeComponent();
            ParentApplet = AppletManager.GetApplet<FileBrowser>();
        }

        /// <summary>
        /// Click handler for Find Missing Translation button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindMissingTranslations_Click(object sender, RoutedEventArgs e)
        {
            List<ResourceDictionary> dicts = LanguageManager.GetLanguageDictionaries();

            foreach (ResourceDictionary d1 in dicts)
            {
                (outputScrollBox.DisplayContent as VTextBlock).Text += "\n\nLanguage Dictionary \"" + d1.Source.ToString() + "\" is missing the following keys:\n";
                foreach (ResourceDictionary d2 in dicts)
                {
                    foreach (string k in d2.Keys)
                    {
                        if (!d1.Contains(k))
                        {
                            if (translationsShowMode.Value)
                            {
                                (outputScrollBox.DisplayContent as VTextBlock).Text += "\t" + k + "\n";
                            }
                            else
                            {
                                (outputScrollBox.DisplayContent as VTextBlock).Text += "\t - " + LanguageManager.Tokens[k] + "\n";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Click handler for Clear Output button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearOutput_Click(object sender, RoutedEventArgs e)
        {
            (outputScrollBox.DisplayContent as VTextBlock).Text = "";
        }

        /// <summary>
        /// Click handler for Reset All Settings button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResetAllSettings_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse response = await DialogWindow.ShowMessage("Are you sure you want to reset all settings?", new string[] { "Yes", "No" });

            if (response.ResponseString == "Yes")
            {
                foreach (string file in Directory.EnumerateFiles(AppPaths.SettingsPath, "*.json", SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }
                App.Restart();
            }
        }

        /// <summary>
        /// Click handler for Reset All Text Predictions button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResetAllTextPredictions_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse response = await DialogWindow.ShowMessage("Are you sure you want to reset all text prediction data?", new string[] { "Yes", "No" });

            if (response.ResponseString == "Yes")
            {
                //Delete old text prediction databases
                Directory.Delete(AppPaths.SettingsPath + "\\global\\language\\text_prediction", true);

                //Copy over fresh text prediction databases
                Directory.CreateDirectory(AppPaths.SettingsPath + "\\global\\language\\");
                string[] stuff = Directory.GetFiles(".\\Resources\\TextPrediction");
                Directory.Move(".\\Resources\\TextPrediction", AppPaths.SettingsPath + "\\global\\language\\text_prediction");
            }
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((outputScrollBox.DisplayContent as VTextBlock).Text);
        }

        private async void LoadTextTranslations_Click(object sender, RoutedEventArgs e)
        {
            DialogResponse response = await DialogWindow.Show(new VFileBrowser());
            if (response.ResponseString != null)
            {
                List<Tuple<string, string>> dict = new List<Tuple<string, string>>();
                string key = "";
                foreach (string line in File.ReadAllLines(response.ResponseObject as string))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        string sl = string.Join(":", line.Split(':').Skip(1));
                        if (key == "")
                        {
                            key = LanguageManager.KeyFromToken(sl, true);
                        }
                        else
                        {
                            string val = sl.TrimStart();
                            dict.Add(new Tuple<string, string>(key, val));
                            key = "";
                        }
                    }
                }
                dict = dict.OrderBy(x => x.Item1).ToList();
                using (StreamWriter sw = new StreamWriter(Path.Combine(AppPaths.DocumentsPath, "exported_translations.xaml")))
                {
                    foreach (Tuple<string, string> kvp in dict)
                    {
                        sw.WriteLine("<system:String x:Key=\"" + kvp.Item1 + "\" xml:space=\"preserve\">" + kvp.Item2 + "</system:String>");
                    }
                }
            }
        }
    }
}
