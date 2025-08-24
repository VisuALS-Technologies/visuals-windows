using System.IO;

namespace VisuALS_WPF_App
{
    public class WebBrowser : Applet
    {
        public WebBrowser() : base("ap_web_browser", "ap_desc_web_browser")
        {
            IconSource = "Resources/Images/globe.png";
            Role = ButtonRole.Web;
        }

        public override AppletPage CreateMainPage()
        {
            string browser = Config.Get<string>("browser");
            if (browser == "br_classic")
            {
                return new OldBrowser();
            }
            else if (browser == "br_win_eye_control")
            {
                return new NewBrowser();
            }
            else
            {
                return new OldBrowser();
            }
        }

        public override AppletPage CreateSettingsPage()
        {
            return new WebBrowserSettings();
        }

        public override void InitializeSettingsValues()
        {
            Config.Initialize("browser", "br_classic");
            Config.Initialize("bookmarks_file", Path.Combine(SettingsPath, "bookmarks.txt"));
            Session.Initialize("url", "https://www.google.com");
        }
    }
}
