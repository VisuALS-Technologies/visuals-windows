using System.Windows.Controls;

namespace VisuALS_WPF_App
{
    /// <summary>
    /// All pages for applets should derive from this class.
    /// This class provides easy access to the config and session
    /// settings files after you set the ParentApplet property.
    /// </summary>
    public class AppletPage : Page
    {
        /// <summary>
        /// Stores a reference to the parent applet.
        /// Set this property on page initialization.
        /// </summary>
        public Applet ParentApplet
        {
            get
            {
                return parentApp_;
            }
            set
            {
                parentApp_ = value;
                Config = value.Config;
                Session = value.Session;
                SettingsPath = value.SettingsPath;
            }
        }



        /// <summary>
        /// Private reference to the parent applet.
        /// </summary>
        private Applet parentApp_;

        /// <summary>
        /// Reference to the parent applet's config file.
        /// </summary>
        public SettingsFile Config;

        /// <summary>
        /// Reference to the parent applet's session data file.
        /// </summary>
        public SettingsFile Session;

        /// <summary>
        /// The parent applet's settings path.
        /// </summary>
        public string SettingsPath;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppletPage() { }
    }
}
