namespace VisuALS_WPF_App
{
    /// <summary>
    /// The File Browser applet is for navigating, moving, viewing, and opening files.
    /// </summary>
    class FileBrowser : Applet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileBrowser() : base("ap_file_browser", "ap_desc_file_browser")
        {
            IconSource = "Resources/Images/folder.png";
            Role = ButtonRole.Settings;
        }

        /// <summary>
        /// Returns the main page.
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new FileBrowserPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return null;
        }

        /// <summary>
        /// Initialize config and settings.
        /// </summary>
        public override void InitializeSettingsValues()
        {

        }
    }
}
