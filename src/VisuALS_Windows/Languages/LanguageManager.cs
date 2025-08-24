using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VisuALS_WPF_App
{
    static class LanguageManager
    {
        public class LangTokens
        {
            public string this[string key]
            {
                get
                {
                    string k = (string)App.CurrentApp.Resources[key];
                    if (k != null)
                        return k;
                    else
                        return "---";

                }
            }
        }

        public static LangTokens Tokens = new LangTokens();

        private static string _currentlang = "English";

        public static string KeyFromToken(string token, bool trim_whitespace = false)
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri(langDictUris[CurrentLanguage], UriKind.Relative);
            string token1 = trim_whitespace ? token.Trim() : token;
            foreach (string k in dict.Keys)
            {
                string token2 = trim_whitespace ? Tokens[k].Trim() : Tokens[k];
                if (token1 == token2)
                {
                    return k as string;
                }
            }
            throw new ArgumentException("Token \"" + token + "\" not found in current language dictionary.");
        }

        private static Dictionary<string, string> langDictUris = new Dictionary<string, string>
        {
            {"English", "..\\Languages\\en.US.xaml" },
            {"Español", "..\\Languages\\es.xaml" }
        };

        private static Dictionary<string, string> langCommonWordPaths = new Dictionary<string, string>
        {
            { "English", AppPaths.SettingsPath + "\\global\\text_prediction\\en.US.txt" },
            { "Español", AppPaths.SettingsPath + "\\global\\text_prediction\\es.txt" }
        };

        private static Dictionary<string, string> langTextPredictPaths = new Dictionary<string, string>
        {
            { "English", AppPaths.SettingsPath + "\\global\\text_prediction\\en.US.sqlite" },
            { "Español", AppPaths.SettingsPath + "\\global\\text_prediction\\es.sqlite" }
        };

        public static void Initialize()
        {
            CurrentLanguage = CurrentLanguage;
        }

        public static List<string> SupportedLanguages
        {
            get
            {
                return langDictUris.Keys.ToList();
            }
        }

        public static string CurrentLanguage
        {
            get
            {
                return _currentlang;
            }
            set
            {
                _currentlang = value;
                ChangeLanguageDictionary(value);
                App.predictor.SetDatabasePath(langTextPredictPaths[CurrentLanguage]);
                App.predictor.SetCommonWordsList(langCommonWordPaths[CurrentLanguage]);
            }
        }

        public static List<string> GetLanguageDictionaryURIs()
        {
            return langDictUris.Values.ToList();
        }

        public static List<ResourceDictionary> GetLanguageDictionaries()
        {
            return langDictUris.Select(x => new ResourceDictionary()
            {
                Source = new Uri(x.Value, UriKind.Relative)
            }).ToList();
        }

        private static void ChangeLanguageDictionary(string selectedLang)
        {
            RemoveCurrentLanguageDictionary();
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri(langDictUris[selectedLang], UriKind.Relative);
            App.CurrentApp.Resources.MergedDictionaries.Add(dict);
        }

        private static void RemoveCurrentLanguageDictionary()
        {
            foreach (string uri in langDictUris.Values)
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(uri, UriKind.Relative);
                App.CurrentApp.Resources.MergedDictionaries.Remove(dict);
            }
        }
    }
}
