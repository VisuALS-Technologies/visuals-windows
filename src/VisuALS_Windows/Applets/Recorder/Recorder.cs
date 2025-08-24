using System.IO;

namespace VisuALS_WPF_App
{
    class Recorder : Applet
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Recorder() : base("ap_recorder", "ap_desc_recorder")
        {
            IconSource = "Resources/Images/microphone.png";
            Role = ButtonRole.Audio;
        }


        /// <summary>
        /// Returns main page
        /// </summary>
        /// <returns></returns>
        public override AppletPage CreateMainPage()
        {
            return new RecorderPage();
        }

        public override AppletPage CreateSettingsPage()
        {
            return new RecorderSettings();
        }

        /// <summary>
        /// Initialize config and settings
        /// </summary>
        public override void InitializeSettingsValues()
        {
            Config.Initialize("recordings_folder", Path.Combine(AppPaths.DocumentsPath, "recordings"));
            Session.Initialize("filename", "");
        }
    }
}
