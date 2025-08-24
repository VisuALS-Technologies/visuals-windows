using System.Collections.Generic;
using System.Linq;

namespace VisuALS_WPF_App
{
    static class AppletManager
    {
        static public List<Applet> Applets = new List<Applet>
        {
            new DevTools(),
            new Notepad(),
            new Recorder(),
            new TextToSpeech(),
            new WebBrowser(),
            new FileBrowser(),
            new PhotoViewer()
        };

        static public List<Applet> ActiveApplets;

        static public List<Applet> InactiveApplets
        {
            get
            {
                return Applets.Where(x => !ActiveApplets.Contains(x)).ToList();
            }
        }

        static public List<string> AppletNames
        {
            get
            {
                return Applets.Select(x => x.Name).ToList();
            }
        }

        static public List<string> ActiveAppletNames
        {
            get
            {
                return ActiveApplets.Select(x => x.Name).ToList();
            }
        }

        static public List<string> InactiveAppletNames
        {
            get
            {
                return InactiveApplets.Select(x => x.Name).ToList();
            }
        }

        static AppletManager()
        {
            SettingsManager.InitializeValue("global\\config", "active_applets", new string[] { "ap_web_browser", "ap_notepad", "ap_text_to_speech", "ap_photo_viewer", "ap_recorder" });
            string[] applet_names = SettingsManager.Get<string[]>("global\\config", "active_applets");
            ActiveApplets = applet_names.Select(x => GetApplet(x)).ToList();
        }

        static public void ActivateApplet<T>()
        {
            ActivateApplet(GetApplet<T>());
        }

        static public void DeactivateApplet<T>()
        {
            DeactivateApplet(GetApplet<T>());
        }

        static public void ActivateApplet(string name)
        {
            ActivateApplet(GetApplet(name));
        }

        static public void DeactivateApplet(string name)
        {
            DeactivateApplet(GetApplet(name));
        }

        static public void ActivateApplet(Applet applet)
        {
            if (!ActiveApplets.Contains(applet))
                ActiveApplets.Add(applet);
            SettingsManager.Set("global\\config", "active_applets", ActiveApplets.Select(x => x.Name).ToArray());
        }

        static public void DeactivateApplet(Applet applet)
        {
            ActiveApplets.Remove(applet);
            SettingsManager.Set("global\\config", "active_applets", ActiveApplets.Select(x => x.Name).ToArray());
        }

        static public Applet GetApplet<T>()
        {
            return Applets.Where(x => x.GetType() == typeof(T)).First();
        }

        static public Applet GetApplet(string name)
        {
            return Applets.Where(x => x.Name == name).First();
        }

        static public bool IsActive(Applet applet)
        {
            return ActiveApplets.Contains(applet);
        }

        static public bool IsActive(string name)
        {
            return ActiveAppletNames.Contains(name);
        }
    }
}
