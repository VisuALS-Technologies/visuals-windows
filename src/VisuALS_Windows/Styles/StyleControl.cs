using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    public struct FontSizes
    {
        public double title;
        public double header;
        public double big;
        public double normal;
        public double small;
    }

    public static class StyleControl
    {
        static Dictionary<string, string> ThemeUris = new Dictionary<string, string>()
        {
            {"th_gumdrop", "..\\Styles\\Themes\\GumdropTheme.xaml" },
            {"th_licorice", "..\\Styles\\Themes\\LicoriceTheme.xaml" },
            {"th_coastline", "..\\Styles\\Themes\\CoastlineTheme.xaml" }
        };

        public static List<string> GetAllThemeNames()
        {
            return ThemeUris.Keys.Select(x => LanguageManager.Tokens[x]).ToList();
        }

        public static void SetTheme(string selectedTheme)
        {
            RemoveCurrentThemeDictionary();
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri(ThemeUris[selectedTheme], UriKind.Relative);
            App.CurrentApp.Resources.MergedDictionaries.Add(dict);
        }

        private static void RemoveCurrentThemeDictionary()
        {
            foreach (string uri in ThemeUris.Values)
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(uri, UriKind.Relative);
                App.CurrentApp.Resources.MergedDictionaries.Remove(dict);
            }
        }

        public static void IncreaseFontSize(int size = 5)
        {
            if (LargestFontSize() < 100)
            {
                FontSizes fontSizes = GetFontSizes();
                fontSizes.title += size;
                fontSizes.header += size;
                fontSizes.big += size;
                fontSizes.normal += size;
                fontSizes.small += size;
                SetFontSizes(fontSizes);
            }
        }

        public static void DecreaseFontSize(int size = 5)
        {
            if (SmallestFontSize() > size)
            {
                FontSizes fontSizes = GetFontSizes();
                fontSizes.title -= size;
                fontSizes.header -= size;
                fontSizes.big -= size;
                fontSizes.normal -= size;
                fontSizes.small -= size;
                SetFontSizes(fontSizes);
            }
        }

        public static double LargestFontSize()
        {
            FontSizes fontSizes = GetFontSizes();
            double size = fontSizes.title;
            if (fontSizes.header > size)
            {
                size = fontSizes.header;
            }
            if (fontSizes.big > size)
            {
                size = fontSizes.big;
            }
            if (fontSizes.normal > size)
            {
                size = fontSizes.normal;
            }
            if (fontSizes.small > size)
            {
                size = fontSizes.small;
            }
            return size;
        }

        public static double SmallestFontSize()
        {
            FontSizes fontSizes = GetFontSizes();
            double size = fontSizes.small;
            if (fontSizes.normal < size)
            {
                size = fontSizes.normal;
            }
            if (fontSizes.big < size)
            {
                size = fontSizes.big;
            }
            if (fontSizes.header < size)
            {
                size = fontSizes.header;
            }
            if (fontSizes.title < size)
            {
                size = fontSizes.title;
            }
            return size;
        }

        public static void SetFontSizes(FontSizes fontSizes)
        {
            App.CurrentApp.Resources["TitleFontSize"] = fontSizes.title;
            App.CurrentApp.Resources["HeaderFontSize"] = fontSizes.header;
            App.CurrentApp.Resources["BigFontSize"] = fontSizes.big;
            App.CurrentApp.Resources["NormalFontSize"] = fontSizes.normal;
            App.CurrentApp.Resources["SmallFontSize"] = fontSizes.small;
        }

        public static FontSizes GetFontSizes()
        {
            FontSizes fontsizes = new FontSizes();
            fontsizes.title = (double)App.CurrentApp.FindResource("TitleFontSize");
            fontsizes.header = (double)App.CurrentApp.FindResource("HeaderFontSize");
            fontsizes.big = (double)App.CurrentApp.FindResource("BigFontSize");
            fontsizes.normal = (double)App.CurrentApp.FindResource("NormalFontSize");
            fontsizes.small = (double)App.CurrentApp.FindResource("SmallFontSize");
            return fontsizes;
        }

        public static void SetAccentColor(Color color)
        {
            App.CurrentApp.Resources["AccentColor"] = new SolidColorBrush(color);
            int[] hsv = color.HSV();
            App.CurrentApp.Resources["DarkAccentColor"] = new SolidColorBrush(ColorUtils.FromHSV(hsv[0], hsv[1] * 2, hsv[2] / 3));
            App.CurrentApp.Resources["LightAccentColor"] = new SolidColorBrush(ColorUtils.FromHSV(hsv[0], hsv[1] / 3, hsv[2] * 2));
        }

        public static Color GetAccentColor()
        {
            return ((SolidColorBrush)App.CurrentApp.Resources["AccentColor"]).Color;
        }
    }
}
