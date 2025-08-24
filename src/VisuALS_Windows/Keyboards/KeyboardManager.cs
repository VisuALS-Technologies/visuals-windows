using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisuALS_WPF_App
{
    static class KeyboardManager
    {
        static DocumentFolder CustomLayouts = new DocumentFolder(Path.Combine(AppPaths.SettingsPath, "global\\custom_keyboards"));

        static Dictionary<string, Type> StandardLayouts = new Dictionary<string, Type>()
        {
            { "kl_en_classic_qwerty", typeof(KeyboardLayouts.EnglishClassicQwertyLayout) },
            { "kl_es_classic_qwerty", typeof(KeyboardLayouts.SpanishClassicQwertyLayout) }
        };

        static Dictionary<string, Type> StandardTabs = new Dictionary<string, Type>()
        {
            { "Cursor Control", typeof(VCursorControlTab) },
            { "Menu", typeof(VMenuTab) },
            { "English Qwerty", typeof(VQwertyTab) },
            { "Spanish Qwerty", typeof(VQwertyTabEs) },
            { "English Special 1", typeof(VSpecialChars1) },
            { "English Special 2", typeof(VSpecialChars2) },
            { "Spanish Special 1", typeof(VSpecialChars1Es) },
            { "Spanish Special 2", typeof(VSpecialChars2Es) }
        };

        public static string CurrentLayoutName = "kl_en_classic_qwerty";

        static KeyboardManager() { }

        public static VKeyboardLayout GetCurrentLayout()
        {
            return LayoutFromName(CurrentLayoutName);
        }

        public static List<string> GetTabNames()
        {
            return StandardTabs.Keys.ToList();
        }

        public static List<string> GetAllLayoutNames()
        {
            List<string> l = StandardLayouts.Keys.Select(x => LanguageManager.Tokens[x]).ToList();
            l.AddRange(GetCustomLayoutNames());
            return l;
        }

        public static VKeyboardLayout LayoutFromName(string name)
        {
            if (StandardLayouts.ContainsKey(name))
            {
                return (VKeyboardLayout)Activator.CreateInstance(StandardLayouts[name]);
            }
            else
            {
                return GetCustomLayout(name);
            }
        }

        public static VKeyboardTab TabFromName(string name)
        {
            return (VKeyboardTab)Activator.CreateInstance(StandardTabs[name]);
        }

        public static void SaveCustomLayout(string name, VKeyboardLayout layout)
        {
            CustomLayouts.NewFile(name, ".json");
            CustomLayouts.WriteToFile(name, JsonConvert.SerializeObject(layout));
        }

        public static void DeleteCustomLayout(string name)
        {
            CustomLayouts.DeleteFile(name);
        }

        public static VKeyboardLayout GetCustomLayout(string name)
        {
            return JsonConvert.DeserializeObject<VKeyboardLayout>(CustomLayouts.GetFileContents(name));
        }

        public static List<string> GetCustomLayoutNames()
        {
            return CustomLayouts.GetFileNames();
        }

        public static VKeyboardLayout LanguageSuggestedKeyboard()
        {
            switch (LanguageManager.CurrentLanguage)
            {
                case "English": return new KeyboardLayouts.EnglishClassicQwertyLayout();
                case "Español": return new KeyboardLayouts.SpanishClassicQwertyLayout();
                default: return null;
            }
        }
    }
}
