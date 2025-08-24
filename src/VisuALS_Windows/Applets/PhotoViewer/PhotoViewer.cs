namespace VisuALS_WPF_App
{
    /// <summary>
    /// The File Browser applet is for navigating, moving, viewing, and opening files.
    /// </summary>
    class PhotoViewer : Applet
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PhotoViewer() : base("ap_photo_viewer", "ap_desc_photo_viewer")
        {
            IconSource = "Resources/Images/photo.png";
            Role = ButtonRole.Display;
        }

        /// <summary>
        /// Returns the main page.
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new PhotoViewerPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return new PhotoViewerSettings();
        }

        /// <summary>
        /// Initialize config and settings.
        /// </summary>
        public override void InitializeSettingsValues()
        {
            Config.Initialize("photos_folder", AppPaths.PicturesPath);
        }
    }
}
